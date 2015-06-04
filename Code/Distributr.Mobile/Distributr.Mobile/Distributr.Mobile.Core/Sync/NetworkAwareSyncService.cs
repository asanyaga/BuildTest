using System;
using System.Collections.Generic;
using Distributr.Mobile.Core.Exceptions;
using Distributr.Mobile.Core.Net;
using Distributr.Mobile.Sync;

namespace Distributr.Mobile.Core.Sync
{
    public delegate void EventHandler(Object update);

    public class NetworkAwareSyncService<T>
    {
        public event EventHandler StatusUpdate;

        private readonly IConnectivityMonitor connectivityMonitor;

        public NetworkAwareSyncService(IConnectivityMonitor connectivityMonitor)
        {
            this.connectivityMonitor = connectivityMonitor;
        }

        public void Process(List<T> work, Action<T, int> action)
        {
            ProcessWithNetworkMonitoring(work, action);
            PublishCompletedEvent();
        }

        public void Process(T work, Action<T, int> action)
        {
            ProcessWithNetworkMonitoring(new List<T>(){work}, action);
        }

        private void ProcessWithNetworkMonitoring(List<T> work, Action<T, int> action)
        {
            CheckListenerAttached();
            var index = 0;

            if (!connectivityMonitor.IsNetworkAvailable())
            {
                PublishPausedEvent("Waiting for network");
            }

            while (index < work.Count)
            {
                try
                {
                    // This method will block if the network is currently unavailable
                    connectivityMonitor.WaitForNetwork();

                    PublishUpdateEvent(CreateUpdateEventMessage(index, work.Count));
                    action(work[index], index);

                    index++;
                }
                catch (Exception e)
                {
                    if (!connectivityMonitor.IsNetworkAvailable())
                    {
                        PublishPausedEvent("Waiting for network");
                        // Move to top of loop i.e. block until network becomes available
                        continue;
                    }

                    HandleFailure(work[index], e);

                    Console.WriteLine("Quitting Sync due to an unexpected Exception: \n{0}", e);

                    return;
                }
            }
        }

        protected virtual void HandleFailure(T currentItem, Exception exception)
        {
            // Override this in the base class - this would normally be an abstract method but wanted to unit test
            // this class
        }

        protected virtual string CreateUpdateEventMessage(int index, int total)
        {
            return string.Format("Processing {0} of {1}", index + 1, total);
        }

        private void CheckListenerAttached()
        {
            if (StatusUpdate == null)
            {
                throw new Bug("You have not attached an event handler (StatusUpdate)");
            }        
        }

        protected void PublishFailureEvent(Exception exception, string message)
        {
            StatusUpdate(new SyncFailedEvent<T>(message: message, exception: exception));
        }

        protected void PublishUpdateEvent(string update, params Object[] args)
        {
            StatusUpdate(new SyncUpdateEvent<T>(string.Format(update, args)));
        }

        protected void PublishPausedEvent(string update)
        {
            StatusUpdate(new SyncPausedEvent<T>(update));
        }

        protected void PublishCompletedEvent()
        {
            StatusUpdate(new SyncCompletedEvent<T>());
        }
    }
}
