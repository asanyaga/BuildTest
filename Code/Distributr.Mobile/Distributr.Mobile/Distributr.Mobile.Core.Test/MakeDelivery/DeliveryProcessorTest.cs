using Distributr.Core.Commands;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Mobile.Core.Envelopes;
using Distributr.Mobile.Core.MakeDelivery;
using Distributr.Mobile.Core.OrderSale;
using Distributr.Mobile.Core.Test.Support;
using NUnit.Framework;

namespace Distributr.Mobile.Core.Test.MakeDelivery
{
    public class DeliveryProcessorTest : WithFullDatabaseTest
    {
        [Test]
        public void CanMakeADeliveryForOneItemAndSellReturnableReceivingCashPayment()
        {
            //Given 
            var orderAndContext = AnOrderAndContextBuilder()
                .AddLineItem(ASaleProduct(), 1, sellReturnables:true)
                .Build();

            var order = orderAndContext.Sale;
            var context = orderAndContext.Context;
            
            order.AllItems.ForEach(i => i.LineItemStatus = LineItemStatus.Approved);
            order.ReturnableLineItems[0].SaleQuantity = 1;
            order.AddCashPayment("cash reference", order.TotalValueIncludingVat);            
            
            var deliveryProcessor = Resolve<DeliveryProcessor>();

            //When 
            var result = deliveryProcessor.Process(order, context);

            //Then
            CheckResult(result);

            var actualCommands = ExtractCommandEnvelopes(Resolve<ILocalCommandEnvelopeRepository>().GetNextOutgoingBatch());

            new CommandEnvelopeChecker(
              new CommandEnvelopeChecks(DocumentType.Order,
                  CommandType.CloseOrder),
              new CommandEnvelopeChecks(DocumentType.DispatchNote,
                  CommandType.CreateDispatchNote,
                  CommandType.AddDispatchNoteLineItem,
                  CommandType.AddDispatchNoteLineItem,
                  CommandType.ConfirmDispatchNote),
              new CommandEnvelopeChecks(DocumentType.InventoryAdjustmentNote,
                  CommandType.CreateInventoryAdjustmentNote,
                  CommandType.AddInventoryAdjustmentNoteLineItem,
                  CommandType.AddInventoryAdjustmentNoteLineItem,
                  CommandType.ConfirmInventoryAdjustmentNote),
              new CommandEnvelopeChecks(DocumentType.Receipt,
                  CommandType.CreateReceipt,
                  CommandType.AddReceiptLineItem,
                  CommandType.ConfirmReceipt),
              new CommandEnvelopeChecks(DocumentType.PaymentNote,
                  CommandType.CreatePaymentNote,
                  CommandType.AddPaymentNoteLineItem,
                  CommandType.ConfirmPaymentNote), 
              new CommandEnvelopeChecks(DocumentType.OutletVisitNote,
                  CommandType.CreateOutletVisitNote))
              .IsSatisfied(actualCommands);
        }

        [Test]
        public void CanMakeDeliveryWithoutReceivingPaymentForApprovedItemsOnly()
        {
            //Given 
            var orderAndContext = AnUnpaidOrderForTwoItems().Build();
            var order = orderAndContext.Sale;
            var context = orderAndContext.Context;

            var deliveryProcessor = Resolve<DeliveryProcessor>();
            //This step is normally perform by IncomingCommandHandleron on receiving the dispatched order from the Hub
            var item1 = order.LineItems[0];
            item1.LineItemStatus = LineItemStatus.Approved;
            item1.SaleQuantity = item1.Quantity;
            order.ReturnableLineItems[0].LineItemStatus = LineItemStatus.Approved;

            //When
            var result = deliveryProcessor.Process(order, context);

            //Then
            CheckResult(result);

            var actualCommands = ExtractCommandEnvelopes(Resolve<ILocalCommandEnvelopeRepository>().GetNextOutgoingBatch());

            new CommandEnvelopeChecker(
              new CommandEnvelopeChecks(DocumentType.Order,
                  CommandType.CloseOrder),
              new CommandEnvelopeChecks(DocumentType.DispatchNote,
                  CommandType.CreateDispatchNote,
                  CommandType.AddDispatchNoteLineItem,
                  CommandType.ConfirmDispatchNote),
              new CommandEnvelopeChecks(DocumentType.InventoryAdjustmentNote,
                  CommandType.CreateInventoryAdjustmentNote,
                  CommandType.AddInventoryAdjustmentNoteLineItem,
                  CommandType.ConfirmInventoryAdjustmentNote),
              new CommandEnvelopeChecks(DocumentType.OutletVisitNote,
                  CommandType.CreateOutletVisitNote),
              new CommandEnvelopeChecks(DocumentType.CreditNote,
                  CommandType.CreateCreditNote,
                  CommandType.AddCreditNoteLineItem,
                  CommandType.ConfirmCreditNote))
              .IsSatisfied(actualCommands);
        }
    }
}
