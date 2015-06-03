using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.Commands.DocumentCommands.Orders;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Mobile.Core.Envelopes;

namespace Distributr.Mobile.Core.OrderSale.EnvelopeBuilders
{
    public class ExternalDocRefEnvelopeBuilder : BaseEnvelopeBuilder
    {
        public ExternalDocRefEnvelopeBuilder(IEnvelopeContext context) : base(context)
        {
        }

        public ExternalDocRefEnvelopeBuilder(IEnvelopeContext context, IEnvelopeBuilder linkedBuilder) : base(context, linkedBuilder)
        {
        }

        protected override void CreateEnvelope()
        {
            Envelope = InitEnvelope();
            Envelope.DocumentTypeId = (int) DocumentType.Order;
        }

        protected override DocumentCommand CreateFirstCommand()
        {
            var command = InitCommand(new AddExternalDocRefCommand());
            command.ExternalDocRef = Context.ExternalDocumentReference();

            return command;
        }

        public override bool HasOutput
        {
            get { return true; }
        }

        protected override void AddConfirmCommand()
        {
            //Not used for ExternalDocRef
        }

        protected override void ProcessLineItem(BaseProductLineItem item, decimal quantity)
        {
            //Not used for ExternalDocRef
        }

        protected override void ProcessPayment(Payment payment)
        {
            //Not used for ExternalDocRef
        }
    }
}
