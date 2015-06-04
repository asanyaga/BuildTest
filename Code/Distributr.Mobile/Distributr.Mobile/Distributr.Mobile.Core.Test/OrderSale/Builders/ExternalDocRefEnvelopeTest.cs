using System.Linq;
using Distributr.Core.Commands.DocumentCommands.Orders;
using Distributr.Mobile.Core.MakeSale;
using Distributr.Mobile.Core.OrderSale.EnvelopeBuilders;
using NUnit.Framework;

namespace Distributr.Mobile.Core.Test.OrderSale
{
    [TestFixture]
    public class ExternalDocRefEnvelopeTest : BaseEnvelopeBuilderTest
    {
        [Test]
        public void CanGenerateAnExternalDocRefEnvelope()
        {
            //Given 
            var saleAndContext = AFullyPaidCashSaleAndContext();
            var sale = saleAndContext.Order;
            var context = saleAndContext.Context;

            var builder = new SaleEnvelopeBuilder(sale, new ExternalDocRefEnvelopeBuilder(context));

            //When 
            var result = builder.Build();

            //Then
            AssertCommonEnvelopeChecks(result, context);

            var documentCommands = ExtractDocumentCommands(result);

            var extDocRef = documentCommands.OfType<AddExternalDocRefCommand>().First();
            Assert.AreEqual(context.ExternalDocumentReference(), extDocRef.ExternalDocRef, "Ext Doc Ref");
        }
    }
}
