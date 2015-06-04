using System;
using System.Linq;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.Commands.DocumentCommands.Invoices;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Mobile.Core.Envelopes;

namespace Distributr.Mobile.Core.OrderSale.EnvelopeBuilders
{
    public class InvoiceEnvelopeBuilder : BaseEnvelopeBuilder
    {
        public InvoiceEnvelopeBuilder(IEnvelopeContext context) : base(context)
        {
            DocumentId = Context.InvoiceId;
        }

        public InvoiceEnvelopeBuilder(IEnvelopeContext context, IEnvelopeBuilder linkedBuilder) : base(context, linkedBuilder)
        {
            DocumentId = Context.InvoiceId;
        }

        protected override void CreateEnvelope()
        {
            Envelope = InitEnvelope();
            Envelope.DocumentTypeId = (int) DocumentType.Invoice;            
        }

        protected override DocumentCommand CreateFirstCommand()
        {
            var command = InitCommand(new CreateInvoiceCommand());
            command.OrderId = Context.ParentDocumentId;
            command.DocumentIssuerCostCentreId = Context.GeneratedByCostCentreId;
            command.DocumentRecipientCostCentreId = Context.RecipientCostCentreId;
            command.DocumentReference = Context.InvoiceReference();
            
            return command;
        }

        protected override void AddConfirmCommand()
        {
            var command = InitCommand(new ConfirmInvoiceCommand());

            Commands.Add(command);
        }

        protected override void ProcessLineItem(BaseProductLineItem item, decimal quantity)
        {
            var command = InitCommand(new AddInvoiceLineItemCommand());

            command.LineItemSequenceNo = Commands.OfType<AddInvoiceLineItemCommand>().Count() + 1;
            command.ProductId = item.ProductMasterId;
            command.Qty = quantity;
            command.ValueLineItem = item.Price;
            command.LineItemVatValue = item.VatRate * item.Price;
            command.LineItemId = Guid.NewGuid();

            Commands.Add(command);
        }

        protected override void ProcessPayment(Payment payment)
        {
            //Ignored
        }
    }
}
