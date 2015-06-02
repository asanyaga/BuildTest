using System.Linq;
using Distributr.Core.Commands.DocumentCommands.Orders;
using Distributr.Mobile.Core.MakeSale;
using Distributr.Mobile.Core.OrderSale.EnvelopeBuilders;
using NUnit.Framework;

namespace Distributr.Mobile.Core.Test.OrderSale
{
    [TestFixture]
    public class CloseOrderEnvelopeBuilderTest : BaseEnvelopeBuilderTest
    {
        [Test]
        public void CanGenerateAClosedOrderEnvelope()
        {
            //Given 
            var saleAndContext = AFullyPaidCashSaleAndContext();
            var sale = saleAndContext.Order;
            var context = saleAndContext.Context;
            sale.ConfirmAllLineItems();
            sale.ConfirmNewPayments();

            var builder = new SaleEnvelopeBuilder(sale, new CloseOrderEnvelopeBuilder(context));

            //When 
            var result = builder.Build();

            //Then
            AssertCommonEnvelopeChecks(result, context);

            var documentCommands = ExtractDocumentCommands(result);
            var closeOrder = documentCommands.OfType<CloseOrderCommand>().First();
            
            //This must have the same ID as the order otherwise it wont show as completed in the Hub, even if it is fully paid. Arggh!
            Assert.AreEqual(sale.Id, closeOrder.DocumentId, "document id");
        }
    }
}
