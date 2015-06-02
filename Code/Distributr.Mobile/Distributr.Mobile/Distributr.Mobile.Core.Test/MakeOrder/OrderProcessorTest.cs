 using Distributr.Core.Commands;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Mobile.Core.Envelopes;
using Distributr.Mobile.Core.MakeSale;
using Distributr.Mobile.Core.OrderSale;
using Distributr.Mobile.Core.Test.Support;
using NUnit.Framework;
using SQLiteNetExtensions.Extensions;
using Order = Distributr.Mobile.Core.OrderSale.Order;

namespace Distributr.Mobile.Core.Test.MakeOrder
{
    [TestFixture]
    public class OrderProcessorTest : WithFullDatabaseTest
    {
        [Test]
        public void CanMakeAnOrderForTwoItemsWithoutPayment()
        {
            //Given 
            var orderAndContext = AnUnpaidOrderForTwoItems().Build();
            var order = orderAndContext.Order;
            var context = orderAndContext.Context;

            var orderProcessor = Resolve<OrderProcessor>();

            //When
            var result = orderProcessor.Process(order, context);

            CheckResult(result);

            var actualCommands = ExtractCommandEnvelopes(Resolve<ILocalCommandEnvelopeRepository>().GetNextOutgoingBatch());

            //Make sure all expected command envelopes and commands were sent out, in order
            new CommandEnvelopeChecker(
                  new CommandEnvelopeChecks(DocumentType.Order,
                      CommandType.CreateMainOrder,
                      CommandType.AddMainOrderLineItem,
                      CommandType.AddMainOrderLineItem,
                      CommandType.AddMainOrderLineItem,
                      CommandType.AddMainOrderLineItem,
                      CommandType.ConfirmMainOrder),
                  new CommandEnvelopeChecks(DocumentType.Order,
                      CommandType.AddExternalDocRef),
                  new CommandEnvelopeChecks(DocumentType.OutletVisitNote,
                      CommandType.CreateOutletVisitNote))
                  .IsSatisfied(actualCommands);

            var saleFromDb = Database.GetWithChildren<Order>(order.Id, recursive: true);

            Assert.AreEqual(ProcessingStatus.Submitted, saleFromDb.ProcessingStatus, "processing status");
        }

        [Test]
        public void CanMakeAnOrderForOneItemPayByCash()
        {
            //Given 
            var orderAndContext = AFullyPaidCashOrderForOneItem().Build();
            var order = orderAndContext.Order;
            var context = orderAndContext.Context;
            
            var orderProcessor = Resolve<OrderProcessor>();

            //When
            var result = orderProcessor.Process(order, context);

            CheckResult(result);

            var actualCommands = ExtractCommandEnvelopes(Resolve<ILocalCommandEnvelopeRepository>().GetNextOutgoingBatch());

            //Make sure all expected command envelopes and commands were sent out, in order
            new CommandEnvelopeChecker(
                  new CommandEnvelopeChecks(DocumentType.Order,
                      CommandType.CreateMainOrder,
                      CommandType.AddMainOrderLineItem,
                      CommandType.AddMainOrderLineItem, // Orders always include returnable items
                      CommandType.OrderPaymentInfo,
                      CommandType.ConfirmMainOrder),
                  new CommandEnvelopeChecks(DocumentType.Order,
                      CommandType.AddExternalDocRef),
                  new CommandEnvelopeChecks(DocumentType.PaymentNote,
                      CommandType.CreatePaymentNote,
                      CommandType.AddPaymentNoteLineItem,
                      CommandType.ConfirmPaymentNote),
                  new CommandEnvelopeChecks(DocumentType.OutletVisitNote,
                      CommandType.CreateOutletVisitNote))
                .IsSatisfied(actualCommands);

            var saleFromDb = Database.GetWithChildren<Order>(order.Id, recursive: true);

            Assert.AreEqual(1, saleFromDb.Payments.Count, "payments");
            Assert.AreEqual(0, saleFromDb.NewPayments.Count, "new payments");
            Assert.AreEqual(PaymentStatus.Confirmed, saleFromDb.Payments[0].PaymentStatus, "payments status");
        }
    }
}