using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.Commands.DocumentCommands.DispatchNotes;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Mobile.Core.Envelopes;

namespace Distributr.Mobile.Core.OrderSale.EnvelopeBuilders
{
    public class DispatchNoteEnvelopeBuilder : BaseEnvelopeBuilder
    {
        public DispatchNoteEnvelopeBuilder(IEnvelopeContext context) : base(context)
        {
        }

        public DispatchNoteEnvelopeBuilder(IEnvelopeContext context, IEnvelopeBuilder linkedBuilder) : base(context, linkedBuilder)
        {
        }

        protected override void CreateEnvelope()
        {
            Envelope = InitEnvelope();
            Envelope.DocumentTypeId = (int) DocumentType.DispatchNote;
        }

        protected override DocumentCommand CreateFirstCommand()
        {
            var command = InitCommand(new CreateDispatchNoteCommand());
            command.DispatchNoteRecipientCostCentreId = Context.RecipientCostCentreId;
            command.DispatchNoteType = (int) DispatchNoteType.Delivery;
            command.OrderId = Context.ParentDocumentId;
            
            return command;
        }

        protected override void AddConfirmCommand()
        {
            Commands.Add(InitCommand(new ConfirmDispatchNoteCommand()));
        }

        protected override void ProcessLineItem(BaseProductLineItem item, decimal quantity)
        {
            var returnableItem = item as ReturnableProductLineItem;

            if (returnableItem != null)
            {
                if (returnableItem.SaleQuantity < 1) return;
                quantity = returnableItem.SaleQuantity;
            }

            var command = InitCommand(new AddDispatchNoteLineItemCommand());
            command.ProductId = item.ProductMasterId;
            command.Qty = quantity;
            command.Value = item.Value;
            command.LineItemVatValue = item.VatValue;

            Commands.Add(command);
        }

        protected override void ProcessPayment(Payment payment)
        {
            //Not used for DispatchNote
        }
    }
}
