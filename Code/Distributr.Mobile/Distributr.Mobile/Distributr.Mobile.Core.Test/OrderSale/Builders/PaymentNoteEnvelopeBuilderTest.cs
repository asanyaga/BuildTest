using System.Linq;
using Distributr.Core.Commands.DocumentCommands.Losses;
using Distributr.Core.Domain.Transactional;
using Distributr.Mobile.Core.MakeSale;
using Distributr.Mobile.Core.OrderSale;
using Distributr.Mobile.Core.OrderSale.EnvelopeBuilders;
using NUnit.Framework;

namespace Distributr.Mobile.Core.Test.OrderSale
{
    [TestFixture]
    public class PaymentNoteEnvelopeBuilderTest : BaseEnvelopeBuilderTest
    {
        [Test]
        public void CanGenerateAPaymentNote()
        {
            //Given
            var saleAndContext = AFullyPaidCashSaleAndContext();
            var sale = saleAndContext.Sale;
            var context = saleAndContext.Context;

            var builder = new SaleEnvelopeBuilder(sale, new PaymentNoteEnvelopeBuilder(context));

            //When 
            var result = builder.Build();
            
            //Then
            AssertCommonEnvelopeChecks(result, context);

            var documentCommands = ExtractDocumentCommands(result);

            //Check Create Payment Note 
            var createPaymentNote = documentCommands.OfType<CreatePaymentNoteCommand>().First();
            Assert.AreEqual(context.RecipientCostCentreId, createPaymentNote.PaymentNoteRecipientCostCentreId, "PaymentNoteRecipientCostCentreId");

            var addPaymentNoteLineItem = documentCommands.OfType<AddPaymentNoteLineItemCommand>().First();
            CheckAddPaymentNoteLineItemCommand(sale, addPaymentNoteLineItem);
        }

        private static void CheckAddPaymentNoteLineItemCommand(Order order, AddPaymentNoteLineItemCommand command)
        {
            Assert.AreEqual(order.TotalValueIncludingVat, command.Amount, "Total payment amount");
            Assert.AreEqual(0, command.PaymentModeId, "Payment mode");
        }
    }
}
