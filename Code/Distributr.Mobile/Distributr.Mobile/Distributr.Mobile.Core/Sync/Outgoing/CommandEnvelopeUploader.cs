using System;
using System.Linq;
using Distributr.Mobile.Core.Envelopes;
using Distributr.Mobile.Core.Net;
using Distributr.Mobile.Core.Sync;
using Distributr.Mobile.Core.Sync.Outgoing;
using Distributr.Mobile.Envelopes;

// ReSharper disable All

namespace Distributr.Mobile.Sync.Outgoing
{
    public delegate void EventHandler(Object update);

    public class CommandEnvelopeUploader : NetworkAwareSyncService<LocalCommandEnvelope>
    {
        private readonly ILocalCommandEnvelopeRepository localCommandEnvelopeRepository;
        private readonly ICommandEnvelopeUploadClient commandEnvelopeUploadClient;
        private readonly SyncLogRepository syncLogRepository;

        public CommandEnvelopeUploader(ILocalCommandEnvelopeRepository localCommandEnvelopeRepository, IConnectivityMonitor connectivityMonitor, 
            ICommandEnvelopeUploadClient commandEnvelopeUploadClient, SyncLogRepository syncLogRepository)
            : base(connectivityMonitor)
        {
            this.localCommandEnvelopeRepository = localCommandEnvelopeRepository;
            this.commandEnvelopeUploadClient = commandEnvelopeUploadClient;
            this.syncLogRepository = syncLogRepository;
        }

        public void UploadPendingEnvelopes()
        {
            var envelopes = localCommandEnvelopeRepository.GetNextOutgoingBatch();
            while (envelopes.Any())
            {
                Process(envelopes, (envelope, index) =>
                {
                    ProcessEnvelope(envelope);
                });

                envelopes = localCommandEnvelopeRepository.GetNextOutgoingBatch();
            }
            syncLogRepository.UpdateLastSyncTime(typeof (LocalCommandEnvelope));
            
            PublishCompletedEvent();
        }

        private void ProcessEnvelope(LocalCommandEnvelope localCommandEnvelope)
        {
            var response = commandEnvelopeUploadClient.UploadCommandEnvelope(localCommandEnvelope);

            if (response.WasSuccessful)
            {
                localCommandEnvelopeRepository.Delete(localCommandEnvelope);
            }
            else
            {
                var errorMessage = string.Format("Error response returned from server: \n\n{0}\n{1}\n{2}", 
                    response.Result, response.ErrorInfo, response.ResultInfo);
                MarkAsFailed(localCommandEnvelope, errorMessage);
            }
        }

        protected override void HandleFailure(LocalCommandEnvelope currentItem, Exception exception)
        {
            var errorMessage = string.Format("Error while processing Transcation with ID {0}: \n\n{1}", 
                currentItem.ParentDoucmentGuid, exception);

            MarkAsFailed(currentItem, errorMessage);            
        }

        private void MarkAsFailed(LocalCommandEnvelope localCommandEnvelope, string errorMessage)
        {
            localCommandEnvelopeRepository.MarkAsFailed(localCommandEnvelope.ParentDoucmentGuid, errorMessage);
        }
    }
}
