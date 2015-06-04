using System.Linq;
using Distributr.Core.Commands.DocumentCommands.Invoices;
using Distributr.Mobile.Core.MakeSale;
using Distributr.Mobile.Core.OrderSale;
using Distributr.Mobile.Core.OrderSale.EnvelopeBuilders;
using NUnit.Framework;

namespace Distributr.Mobile.Core.Test.OrderSale
{
    [TestFixture]
    public class InvoiceEnvelopeBuilderTest : BaseEnvelopeBuilderTest
    {
        [Test]
        public void CanCreateAnInvoiceEnvelope()
        {
            //Given 
            var saleAndContext = AFullyPaidCashSaleAndContext();
            var order = saleAndContext.Sale;
            var context = saleAndContext.Context;

            var builder = new SaleEnvelopeBuilder(order, new InvoiceEnvelopeBuilder(context));

            //When 
            var result = builder.Build();

            //Then
            AssertCommonEnvelopeChecks(result, context);

            Assert.AreEqual(context.InvoiceId, result[0].DocumentId, "DocumentId");

            var documentCommands = ExtractDocumentCommands(result);

            //Check Create Invoice
            var createInvoice = documentCommands.OfType<CreateInvoiceCommand>().First();
            Assert.AreEqual(context.ParentDocumentId, createInvoice.OrderId, "Order Id");
            Assert.AreEqual(order.SaleDiscount, createInvoice.SaleDiscount, "Sale Discount");
            Assert.AreEqual(context.GeneratedByCostCentreId, createInvoice.DocumentIssuerCostCentreId, "DocumentIssuerCostCentreId");
            Assert.AreEqual(context.RecipientCostCentreId, createInvoice.DocumentRecipientCostCentreId, "DocumentRecipientCostCentreId");

            CheckReference("I", createInvoice.DocumentReference, context.User.Username);

            var addInvoiceLineItems = documentCommands.OfType<AddInvoiceLineItemCommand>().ToList();

            CheckAddInvoiceLineItemCommand(order.LineItems[0], addInvoiceLineItems[0]);
        }

        private static void CheckAddInvoiceLineItemCommand(ProductLineItem item, AddInvoiceLineItemCommand command)
        {
            Assert.AreEqual(item.Product.Id, command.ProductId, "Line item Product ID");
            Assert.AreEqual(item.Quantity, command.Qty, "Line item quantity");
            Assert.AreEqual(item.Value, command.ValueLineItem, "Line item value");
            Assert.AreEqual(item.VatValue, command.LineItemVatValue, "Line item VAT value");
        }
    }
}
