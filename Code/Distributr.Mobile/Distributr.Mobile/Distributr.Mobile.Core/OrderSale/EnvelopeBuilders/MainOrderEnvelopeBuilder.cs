using System.Linq;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.Commands.DocumentCommands.Orders;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Mobile.Core.Envelopes;

namespace Distributr.Mobile.Core.OrderSale.EnvelopeBuilders
{
    public class MainOrderEnvelopeBuilder : BaseEnvelopeBuilder
    {
        public MainOrderEnvelopeBuilder(IEnvelopeContext context) : this(context, new NoOpEnvelopeBuilder())
        {
            DocumentId = context.ParentDocumentId;
        }

        public MainOrderEnvelopeBuilder(IEnvelopeContext context, IEnvelopeBuilder linkedBuilder) : base(context, linkedBuilder)
        {
            DocumentId = context.ParentDocumentId;
        }

        protected override void ProcessLineItem(BaseProductLineItem item, decimal quantity)
        {
            var command = InitCommand(new AddMainOrderLineItemCommand());
            
            command.CommandId = item.Id;
            command.LineItemSequenceNo = Commands.OfType<AddMainOrderLineItemCommand>().Count() + 1;
            command.ProductId = item.ProductMasterId;
            command.Qty = quantity;
            command.ValueLineItem = item.Price;
            command.LineItemVatValue = item.VatRate * item.Price;
            command.ProductDiscount = item.ProductDiscount;
            command.LineItemType = 1;
            Commands.Add(command);
        }

        protected override void ProcessPayment(Payment payment)
        {
            var command = InitCommand(new AddOrderPaymentInfoCommand());
            command.Amount = payment.Amount;
            command.ConfirmedAmount = payment.Amount;
            command.Bank = payment.Bank;
            command.BankBranch = payment.BankBranch;
            command.PaymentRefId = payment.PaymentReference;
            command.PaymentModeId = (int) payment.PaymentMode;
            command.InfoId = command.CommandId = payment.Id;
            command.DueDate = payment.DueDate;
            command.IsProcessed = Context.OrderTypeId == (int) OrderType.DistributorPOS;
            command.IsConfirmed = true;
            Commands.Add(command);
        }

        protected override void CreateEnvelope()
        {
            Envelope = InitEnvelope();
            Envelope.DocumentTypeId = (int)DocumentType.Order;
        }

        protected override DocumentCommand CreateFirstCommand()
        {
            var command = InitCommand(new CreateMainOrderCommand());

            command.VisitId = Context.VisitId;
            command.OrderTypeId = Context.OrderTypeId;
            command.ShipToAddress = Context.ShipToAddress;
            command.SaleDiscount = Context.SaleDiscount;
            command.OrderStatusId = (int)OrderStatus.New;
            command.ParentId = DocumentId;
            command.DocumentDateIssued = Context.Timestamp;
            command.DocumentDateIssued = Context.Timestamp;
            command.DateOrderRequired = Context.Timestamp;
            command.DocumentIssuerCostCentreId = Context.GeneratedByCostCentreId;
            command.IssuedOnBehalfOfCostCentreId = Context.IssuedOnBehalfOfCostCentre;
            command.DocumentRecipientCostCentreId = Context.RecipientCostCentreId;
            command.DocumentReference = Context.OrderSaleReference();

            return command;
        }

        protected override void AddConfirmCommand()
        {
            var command = InitCommand(new ConfirmMainOrderCommand());
            Commands.Add(command);
        }
    }
}
