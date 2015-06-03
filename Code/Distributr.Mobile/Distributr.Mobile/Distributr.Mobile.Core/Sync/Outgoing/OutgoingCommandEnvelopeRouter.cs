using System;
using Distributr.Core.ClientApp;
using Distributr.Core.Commands.CommandPackage;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Mobile.Core.Envelopes;
using Distributr.Mobile.Envelopes;
using Newtonsoft.Json;

namespace Distributr.Mobile.Sync.Outgoing
{
    public class OutgoingCommandEnvelopeRouter : IOutgoingCommandEnvelopeRouter
    {
        private readonly LocalCommandEnvelopeRepository localCommandEnvelopeRepository;

        public OutgoingCommandEnvelopeRouter(LocalCommandEnvelopeRepository localCommandEnvelopeRepository)
        {
            this.localCommandEnvelopeRepository = localCommandEnvelopeRepository;
        }
        
        //This should always be called as part of a wider database transaction
        public void RouteCommandEnvelope(CommandEnvelope envelope)
        {
            var json = JsonConvert.SerializeObject(envelope, Formatting.Indented);

            var commandEnvelope = new LocalCommandEnvelope
            {
                Id = Guid.NewGuid(),
                ParentDoucmentGuid = envelope.ParentDocumentId,
                DocumentType = (DocumentType) envelope.DocumentTypeId,
                Contents = json,
                RoutingStatus = RoutingStatus.Pending,
                ProcessingOrder = OrderFor(envelope.DocumentTypeId),
                RoutingDirection = RoutingDirection.Outgoing
            };
            localCommandEnvelopeRepository.Save(commandEnvelope);
        }

        private int OrderFor(int documentTypeId)
        {
            DocumentType documentType = (DocumentType) documentTypeId;
            switch (documentType)
            {
                case DocumentType.Order :
                    return 1;
                case DocumentType.Invoice:
                    return 2;
                case DocumentType.DispatchNote:
                    return 3;
                case DocumentType.InventoryAdjustmentNote:
                    return 4;
                case DocumentType.Receipt:
                    return 5;
                case DocumentType.PaymentNote:
                    return 6;
                case DocumentType.OutletVisitNote:
                    return 7;
                default:
                    return 8;
            }
               
        }
    }
}