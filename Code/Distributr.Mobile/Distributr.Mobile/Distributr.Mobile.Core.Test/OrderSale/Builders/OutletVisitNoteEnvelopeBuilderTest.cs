using Distributr.Mobile.Core.MakeSale;
using Distributr.Mobile.Core.Outlets;
using NUnit.Framework;

namespace Distributr.Mobile.Core.Test.OrderSale
{
    [TestFixture]
    public class OutletVisitNoteEnvelopeBuilderTest : BaseEnvelopeBuilderTest
    {
        [Test]
        public void CanCreateAnOutletVisitNote()
        {
            //Given
            var orderAndContext = AFullyPaidCashSaleAndContext();
            var sale = orderAndContext.Order;
            var context = orderAndContext.Context;

            var builder = new SaleEnvelopeBuilder(sale, new OutletVisitNoteEnvelopeBuilder(context));

            //When 
            var result = builder.Build();

            //Then
            AssertCommonEnvelopeChecks(result, context);
        }
    }
}
