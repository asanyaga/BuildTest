using System;
using System.Linq;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Commands.DocumentCommands.Orders;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Mobile.Core.Envelopes;
using Distributr.Mobile.Core.Net;
using Distributr.Mobile.Core.Util;
using Distributr.Mobile.Envelopes;
using Newtonsoft.Json;

namespace Distributr.Mobile.Core.Sync.Incoming
{
    public class CommandEnvelopeDownloader : NetworkAwareSyncService<DownloadEnvelopeRequest>
    {
        private readonly CommandEnvelopeDownloadClient commandEnvelopeDownloadClient;
        private readonly LocalCommandEnvelopeRepository localCommandEnvelopeRepository;
        private readonly IncomingEnvelopeLogRepository incomingEnvelopeLogRepository;
        private Exception exception;

        public CommandEnvelopeDownloader(IConnectivityMonitor connectivityMonitor, CommandEnvelopeDownloadClient commandEnvelopeDownloadClient, 
            LocalCommandEnvelopeRepository localCommandEnvelopeRepository, IncomingEnvelopeLogRepository incomingEnvelopeLogRepository) 
            : base(connectivityMonitor)
        {
            this.commandEnvelopeDownloadClient = commandEnvelopeDownloadClient;
            this.localCommandEnvelopeRepository = localCommandEnvelopeRepository;
            this.incomingEnvelopeLogRepository = incomingEnvelopeLogRepository;
        }

        public Result<object> DownloadEnvelopes(string costCentreApplicationId)
        {
            var hasMoreEnvelopes = true;
            var lastEnvelopeIds = incomingEnvelopeLogRepository.GetIncomingEnvelopeLog().LastEnvelopeIds;

            var request = new DownloadEnvelopeRequest(lastEnvelopeIds, costCentreApplicationId);

            while (hasMoreEnvelopes && exception == null)
            {
                Process(request, delegate
                {
                    var result = GetNextEnvelopes(request);

                    if (result.HasMoreEnvelopes)
                    {
                        request = request.ButWithGuid(result.LastEnvelopeId());
                        incomingEnvelopeLogRepository.UpdateLastEnvelopeId(result.LastEnvelopeId());
                    }                    
                    hasMoreEnvelopes = result.HasMoreEnvelopes;
                });
            }

            if (exception == null)
            {
                return Result<object>.Success(null);
            }
            return Result<object>.Failure(exception, "Unable to complete Transactions download");
        }

        protected override void HandleFailure(DownloadEnvelopeRequest currentItem, Exception exception)
        {
            this.exception = exception;
        }

        protected override string CreateUpdateEventMessage(int index, int total)
        {
            return "Downloading...";
        }

        private DownloadEnvelopesResponse GetNextEnvelopes(DownloadEnvelopeRequest request)
        {
            var downloadResult = commandEnvelopeDownloadClient.DownloadCommandEnvelopes(request);

            downloadResult.Envelopes.ForEach(e =>
            {
                var commandEnvelope = new LocalCommandEnvelope
                {
                    Id = Guid.NewGuid(),
                    ParentDoucmentGuid = e.Envelope.ParentDocumentId,
                    DocumentType = (DocumentType) e.Envelope.DocumentTypeId,
                    Contents = JsonConvert.SerializeObject(e.Envelope),
                    RoutingStatus = RoutingStatus.Pending,
                    RoutingDirection = RoutingDirection.Incoming, 
                    ProcessingOrder = ProcessingOrder(e)
                };
                localCommandEnvelopeRepository.Save(commandEnvelope);
            });
            
            return downloadResult;
        }

        private int ProcessingOrder(CommandEnvelopeWrapper commandEnvelopeWrapper)
        {
            var command = commandEnvelopeWrapper.Envelope.CommandsList.First().Command;
            if (command is CreateMainOrderCommand)
                return 0;
            if (command is AddMainOrderLineItemCommand)
                return 1;
            if (command is ApproveOrderLineItemCommand)
                return 2;
            if (command is CloseOrderCommand)
                return 4;
            if (command is RejectMainOrderCommand)
                return 5;

            return 3;
        }
    }

}
