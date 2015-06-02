using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using Distributr.Core.Commands.CommandPackage;
using Distributr.Mobile.Core.Data;
using Distributr.Mobile.Core.Envelopes;
using Distributr.Mobile.Core.Exceptions;
using Distributr.Mobile.Core.Util;
using Distributr.Mobile.Data;
using Distributr.Mobile.Envelopes;
using Distributr.Mobile.Sync;
using Newtonsoft.Json;
// ReSharper disable PossibleNullReferenceException

namespace Distributr.Mobile.Core.Sync.Incoming
{
    public class IncomingCommandEnvelopeProcessor
    {
        public event EventHandler StatusUpdate;

        private readonly CommandEnvelopeDownloader commandEnvelopeDownloader;
        private readonly IncomingCommandHandler incomingCommandHandler;
        private readonly LocalCommandEnvelopeRepository localCommandEnvelopeRepository;
        private readonly SyncLogRepository syncLogRepository;
        private readonly Database database;

        public IncomingCommandEnvelopeProcessor(CommandEnvelopeDownloader commandEnvelopeDownloader, IncomingCommandHandler incomingCommandHandler,
            LocalCommandEnvelopeRepository localCommandEnvelopeRepository, Database database, SyncLogRepository syncLogRepository)
        {
            this.commandEnvelopeDownloader = commandEnvelopeDownloader;
            this.commandEnvelopeDownloader.StatusUpdate += OnStatusUpdate;
            this.incomingCommandHandler = incomingCommandHandler;
            this.localCommandEnvelopeRepository = localCommandEnvelopeRepository;
            this.database = database;
            this.syncLogRepository = syncLogRepository;
        }

        private void OnStatusUpdate(object update)
        {
            StatusUpdate(update);
        }

        public void DownloadPendingEnvelopes(string costCentreApplicationId)
        {
            CheckListenerAttached();
            
            var result = commandEnvelopeDownloader.DownloadEnvelopes(costCentreApplicationId);
            
            var processingSuccess = ApplyNewEnvelopes();

            if (result.WasSuccessful() && processingSuccess)
            {
                StatusUpdate(new SyncCompletedEvent<DownloadEnvelopeRequest>());
                syncLogRepository.UpdateLastSyncTime(typeof (DownloadEnvelopeRequest));
            }
            else
            {
                var message = string.IsNullOrEmpty(result.Message)
                    ? "Error processing transaction after download"
                    : result.Message;
                StatusUpdate(new SyncFailedEvent<DownloadEnvelopeRequest>(message: message, exception: result.Exception));
            }
        }

        public bool ApplyNewEnvelopes(bool publishSyncEvents = true)
        {
            var envelopes = localCommandEnvelopeRepository.GetNextIncomingBatch();
            var count = 1;
            var success = true;

            while (envelopes.Any())
            {
                if (publishSyncEvents) 
                    OnStatusUpdate(CreateUpdate(count));

                var result = ProcessCurrentBatch(envelopes);

                if (!result.WasSuccessful())
                {
                    MarkAsFailed(envelopes[0].ParentDoucmentGuid, result);
                    success = false;
                }
                //Get the next batch
                envelopes = localCommandEnvelopeRepository.GetNextIncomingBatch();
                
                count++;
            }

            return success;
        }

        private object CreateUpdate(int count)
        {
            return new SyncUpdateEvent<DownloadEnvelopeRequest>(string.Format("Applying transaction {0}", count));
        }

        private Result<object> ProcessCurrentBatch(List<LocalCommandEnvelope> envelopes)
        {            
            return new Transactor(database).Transact(delegate
            {
                var parentId = envelopes.First().ParentDoucmentGuid;

                incomingCommandHandler.Init(parentId);

                envelopes.ForEach(e =>
                {
                    var envelope = JsonConvert.DeserializeObject<CommandEnvelope>(e.Contents);

                    foreach (dynamic command in envelope.CommandsList.Select(c => c.Command).ToList())
                    {
                        incomingCommandHandler.Handle(command);
                    }

                    localCommandEnvelopeRepository.Delete(e);
                });

                incomingCommandHandler.Save();
            });
        }

        private void MarkAsFailed(Guid parentDocumentGuid, Result<object> processingResult)
        {

            var errorMessage = string.Format("Error procesing transaction after download: \nID {0}\n{1}\n{2}", 
                    parentDocumentGuid, processingResult.Message, processingResult.Exception);

            localCommandEnvelopeRepository.MarkAsFailed(parentDocumentGuid, errorMessage);
        }

        private void CheckListenerAttached()
        {
            if (StatusUpdate == null)
            {
                throw new Bug("You have not attached an event handler (StatusUpdate)");
            }
        }
    }
}
