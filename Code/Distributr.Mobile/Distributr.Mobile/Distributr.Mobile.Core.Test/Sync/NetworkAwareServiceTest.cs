using System;
using System.Collections.Generic;
using System.Net;
using Distributr.Mobile.Core.Net;
using Distributr.Mobile.Core.Sync;
using Distributr.Mobile.Sync;
using Moq;
using NUnit.Framework;

namespace Distributr.Mobile.Core.Test.Sync
{
    
    [TestFixture]
    public class NetworkAwareServiceTest
    {
        private Mock<IConnectivityMonitor> connectivityMonitor;

        [SetUp]
        public void Setup()
        {
            connectivityMonitor = new Mock<IConnectivityMonitor>();
        }

        [Test]
        public void CanUploadPendingEnvelopesWhenConnectionIsAvailable()
        {
            //Given                        
            connectivityMonitor.Setup(c => c.IsNetworkAvailable())
                .Returns(true);

            var networkAwareService = new NetworkAwareSyncService<object>(connectivityMonitor.Object);

            var eventRecorder = new EventRecorder( 
                typeof(SyncUpdateEvent<object>),
                typeof(SyncUpdateEvent<object>),
                typeof(SyncCompletedEvent<object>));
            
            networkAwareService.StatusUpdate += (e) => { eventRecorder.ActualTypes.Add(e.GetType()); };

            var dummyWork = new List<object>() { "one", "two" };

            //When
            networkAwareService.Process(dummyWork, (o, i) => Console.WriteLine(o));

            //Then
            eventRecorder.IsSatisfied();
        }

        [Test]
        public void CanResumeProcessingWhenNetworkBecomesAvailable()
        {
            //Given                        
            connectivityMonitor.SetupSequence(c => c.IsNetworkAvailable())
                .Returns(true)
                .Returns(false) // Returns false after network error leading to SyncPausedEvent
                .Returns(true) 
                .Returns(true);

            var networkAwareService = new NetworkAwareSyncService<string>(connectivityMonitor.Object);

            var eventRecorder = new EventRecorder(
                typeof(SyncUpdateEvent<string>),
                typeof(SyncUpdateEvent<string>),
                typeof(SyncPausedEvent<string>),
                typeof(SyncUpdateEvent<string>),
                typeof(SyncCompletedEvent<string>));

            networkAwareService.StatusUpdate += (e) => { eventRecorder.ActualTypes.Add(e.GetType()); };
            
            var dummyWork = new List<string>(){ "one", "two" };

            bool thrown = false;

            //When
            networkAwareService.Process(dummyWork, (o, i) =>
            {
                if (i == 1 && !thrown)
                {
                    thrown = true; 
                    throw new WebException("simulate a network error on processing second item");
                }
            });

            //Then
            eventRecorder.IsSatisfied();
        }

    }

    //Records Events that are emitted from the NetworkAwareService during processing
    public class EventRecorder
    {
        private readonly List<Type> expectedTypes;
        public readonly List<Type> ActualTypes;

        public EventRecorder(params Type[] expectedTypes)
        {
            this.expectedTypes = new List<Type>(expectedTypes);
            ActualTypes = new List<Type>();            
        }

        //Checks that all expected events where received in order
        public bool IsSatisfied()
        {
            CollectionAssert.AreEqual(expectedTypes, ActualTypes, "Events in order");
            return true;
        }

    }
}