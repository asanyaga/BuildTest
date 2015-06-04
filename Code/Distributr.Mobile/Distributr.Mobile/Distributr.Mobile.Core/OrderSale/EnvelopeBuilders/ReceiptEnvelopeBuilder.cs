using System;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.Commands.DocumentCommands.Receipts;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Mobile.Core.Envelopes;

namespace Distributr.Mobile.Core.OrderSale.EnvelopeBuilders
{
    public class ReceiptEnvelopeBuilder : BaseEnvelopeBuilder
    {
        public ReceiptEnvelopeBuilder(IEnvelopeContext context) : base(context)
        {
        }

        public ReceiptEnvelopeBuilder(IEnvelopeContext context, IEnvelopeBuilder linkedBuilder) : base(context, linkedBuilder)
        {
        }

        protected override void CreateEnvelope()
        {
            Envelope = InitEnvelope();
            Envelope.DocumentTypeId = (int) DocumentType.Receipt;
        }

        protected override DocumentCommand CreateFirstCommand()
        {
            var command = InitCommand(new CreateReceiptCommand());
            command.DocumentRecipientCostCentreId = Context.RecipientCostCentreId;
            command.InvoiceId = Context.InvoiceId;
            command.DocumentReference = Context.ReceiptReference();
            
            return command;
        }

        protected override void AddConfirmCommand()
        {
            var command = InitCommand(new ConfirmReceiptCommand());

            Commands.Add(command);
        }

        protected override void ProcessLineItem(BaseProductLineItem item, decimal quantity)
        {
            //Not used for Receipt
        }

        protected override void ProcessPayment(Payment payment)
        {
            var command = InitCommand(new AddReceiptLineItemCommand());
            command.Value = payment.Amount;
            command.PaymentTypeId = (int) PaymentNoteType.Availabe;
            command.LineItemId = Guid.NewGuid();
            command.LineItemType = 2; //(int) payment.PaymentMode;
            command.Description = payment.PaymentReference;
            command.LineItemSequenceNo = Commands.Count + 1;

            Commands.Add(command);
        }
    }
}
