using System.Linq;
using Distributr.Core.Commands.DocumentCommands.Orders;
using Distributr.Mobile.Core.MakeOrder;
using Distributr.Mobile.Core.MakeSale;
using Distributr.Mobile.Core.OrderSale;
using Distributr.Mobile.Core.OrderSale.EnvelopeBuilders;
using NUnit.Framework;

namespace Distributr.Mobile.Core.Test.OrderSale
{
    [TestFixture]
    public class CreateMainOrderEnvelopeBuilderTest : BaseEnvelopeBuilderTest
    {
        [Test]
        public void CanBuildASalerWithOneItemAndCashPaymentInfo()
        {
            //Given
            var orderAndContext = AFullyPaidCashSaleAndContext();
            var order = orderAndContext.Sale;
            var context = orderAndContext.Context;

            var builder = new SaleEnvelopeBuilder(order, new MainOrderEnvelopeBuilder(context));

            //When 
            var result = builder.Build();
            
            //Then
            AssertCommonEnvelopeChecks(result, context);

            var documentCommands = ExtractDocumentCommands(result);

            //Check Create Order Command
            var createOrder = documentCommands.OfType<CreateMainOrderCommand>().First();
            Assert.AreEqual(order.ShipToAddress, createOrder.ShipToAddress, "Ship To Address");
            Assert.AreEqual(order.SaleDiscount, createOrder.SaleDiscount, "Sale Discount");
            Assert.AreEqual((int)order.OrderType, createOrder.OrderTypeId, "Sale Type");
            Assert.AreEqual(context.IssuedOnBehalfOfCostCentre, createOrder.IssuedOnBehalfOfCostCentreId, "IssuedOnBehalfOfCostCentreId");

            CheckReference("S", createOrder.DocumentReference, context.User.Username);

            var addMainOrderLineItems = documentCommands.OfType<AddMainOrderLineItemCommand>().ToList();

            CheckOrderLineCommand(order.LineItems[0], addMainOrderLineItems[0]);

            //Check order payment info
            var orderPaymentInfo = documentCommands.OfType<AddOrderPaymentInfoCommand>().First();
            CheckOrderPaymentInfo(order, orderPaymentInfo);
        }


        [Test]
        public void CanMakeAnOrderForItemWithReturnable()
        {
            //Given
            var orderAndContext = AnOrderWithItemAndReturnable();
            var order = orderAndContext.Sale;
            var context = orderAndContext.Context;

            var builder = new OrderEnvelopeBuilder(order, new MainOrderEnvelopeBuilder(context));

            //When
            var result = builder.Build();

            //Then
            AssertCommonEnvelopeChecks(result, context);

            var documentCommands = ExtractDocumentCommands(result);
            var createOrder = documentCommands.OfType<CreateMainOrderCommand>().First();

            CheckReference("O", createOrder.DocumentReference, context.User.Username);

            var addMainOrderLineItems = documentCommands.OfType<AddMainOrderLineItemCommand>().ToList();

            CheckOrderLineCommand(order.LineItems[0], addMainOrderLineItems[0]);
            CheckOrderLineCommand(order.ReturnableLineItems[0], addMainOrderLineItems[1]);

        }

        private static void CheckOrderLineCommand(BaseProductLineItem item, AddMainOrderLineItemCommand command)
        {
            Assert.AreEqual(item.Price, command.ValueLineItem, "Line item  value");
            Assert.AreEqual(item.VatValue, command.LineItemVatValue, "Line item VAT value");
            Assert.AreEqual(item.Quantity, command.Qty, "Line item Qauntity");
            Assert.AreEqual(item.ProductMasterId, command.ProductId, "Line item Product ID");
            Assert.AreEqual(item.ProductDiscount, command.ProductDiscount, "Line item Product Discount");
        }
    }
}
