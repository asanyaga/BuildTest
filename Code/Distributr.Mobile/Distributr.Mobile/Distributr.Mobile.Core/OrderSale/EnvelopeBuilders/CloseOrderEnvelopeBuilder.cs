
using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.Commands.DocumentCommands.Orders;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Mobile.Core.Envelopes;

namespace Distributr.Mobile.Core.OrderSale.EnvelopeBuilders
{
    public class CloseOrderEnvelopeBuilder : BaseEnvelopeBuilder
    {
        public CloseOrderEnvelopeBuilder(IEnvelopeContext context) : base(context)
        {
        }

        public CloseOrderEnvelopeBuilder(IEnvelopeContext context, IEnvelopeBuilder linkedBuilder) : base(context, linkedBuilder)
        {
        }

        protected override void CreateEnvelope()
        {
            Envelope = InitEnvelope();
            Envelope.DocumentTypeId = (int) DocumentType.Order;
            Envelope.DocumentId = Context.ParentDocumentId;
        }

        protected override DocumentCommand CreateFirstCommand()
        {
            var command =  InitCommand(new CloseOrderCommand());
            command.DocumentId = command.PDCommandId = Context.ParentDocumentId;
             
            return command;
        }

        public override bool HasOutput
        {
            get { return true; }
        }

        protected override void AddConfirmCommand()
        {
            //Not used for CloseOrder
        }

        protected override void ProcessLineItem(BaseProductLineItem item, decimal quantity)
        {
            //Not used for CloseOrder
        }

        protected override void ProcessPayment(Payment payment)
        {
            //Not used for CloseOrder
        }
    }
}
