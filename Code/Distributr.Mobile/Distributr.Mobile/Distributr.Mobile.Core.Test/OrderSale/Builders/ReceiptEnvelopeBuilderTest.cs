using System.Linq;
using Distributr.Core.Commands.DocumentCommands.Receipts;
using Distributr.Core.Domain.Transactional;
using Distributr.Mobile.Core.MakeSale;
using Distributr.Mobile.Core.OrderSale;
using Distributr.Mobile.Core.OrderSale.EnvelopeBuilders;
using NUnit.Framework;

namespace Distributr.Mobile.Core.Test.OrderSale
{
    [TestFixture]
    public class ReceiptEnvelopeBuilderTest : BaseEnvelopeBuilderTest
    {
        [Test]
        public void CanCreateAReceiptEnvelope()
        {
            //Given
            var saleAndContext = AFullyPaidCashSaleAndContext();
            var sale = saleAndContext.Order;
            var context = saleAndContext.Context;

            var builder = new SaleEnvelopeBuilder(sale, new ReceiptEnvelopeBuilder(context));

            //When 
            var result = builder.Build();

            //Then
            AssertCommonEnvelopeChecks(result, context);

            var documentCommands = ExtractDocumentCommands(result);

            var createReceipt = documentCommands.OfType<CreateReceiptCommand>().First();
            CheckReference("R", createReceipt.DocumentReference, context.User.Username);

            //Check Receipt Line Item
            var addReceiptLineItem = documentCommands.OfType<AddReceiptLineItemCommand>().First();

            CheckReceiptLineItem(sale, addReceiptLineItem);
        }

        public static void CheckReceiptLineItem(Order order, AddReceiptLineItemCommand addReceiptLineItem)
        {                     
            Assert.AreEqual(order.Payments[0].PaymentMode, (PaymentMode)addReceiptLineItem.PaymentTypeId);
            Assert.AreEqual(order.TotalValueIncludingVat, addReceiptLineItem.Value);            
        }

    }
}
