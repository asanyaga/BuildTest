using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.Commands.DocumentCommands.CreditNotes;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Mobile.Core.Envelopes;

namespace Distributr.Mobile.Core.OrderSale.EnvelopeBuilders
{
    public class CreditNoteEnvelopeBuilder : BaseEnvelopeBuilder
    {
        public CreditNoteEnvelopeBuilder(IEnvelopeContext context) : base(context)
        {
        }

        public CreditNoteEnvelopeBuilder(IEnvelopeContext context, IEnvelopeBuilder linkedBuilder) : base(context, linkedBuilder)
        {
        }

        protected override void CreateEnvelope()
        {
            Envelope = InitEnvelope();
            Envelope.DocumentTypeId = (int)DocumentType.CreditNote;
        }

        protected override DocumentCommand CreateFirstCommand()
        {
            var command = InitCommand(new CreateCreditNoteCommand());
            command.CreditNoteType = (int) CreditNoteType.ProductReturnables;
            command.InvoiceId = Context.InvoiceId;
            return command;
        }

        protected override void AddConfirmCommand()
        {
            Commands.Add(InitCommand(new ConfirmCreditNoteCommand()));
        }

        protected override void ProcessLineItem(BaseProductLineItem item, decimal quantity)
        {
            var returnableItem = item as ReturnableProductLineItem;
            if (returnableItem == null) return;

            var creditNoteQuantity = returnableItem.ApprovedQuantity - returnableItem.SaleQuantity;

            if (creditNoteQuantity == 0) return;

            var command = InitCommand(new AddCreditNoteLineItemCommand());
            command.ProductId = returnableItem.ProductMasterId;
            command.Qty = creditNoteQuantity;
            command.Value = returnableItem.Price;
            command.LineItemId = returnableItem.Id;

            Commands.Add(command);
        }

        protected override void ProcessPayment(Payment payment)
        {
            //Ignored
        }
    }
}
