using System;
using System.Linq;
using Distributr.Core.Commands;
using Distributr.Core.Domain.InventoryEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Mobile.Core.Envelopes;
using Distributr.Mobile.Core.MakeSale;
using Distributr.Mobile.Core.OrderSale;
using Distributr.Mobile.Core.Test.Support;
using Distributr.Mobile.Envelopes;
using NUnit.Framework;
using SQLiteNetExtensions.Extensions;
using Order = Distributr.Mobile.Core.OrderSale.Order;

namespace Distributr.Mobile.Core.Test.MakeSale
{
    
    [TestFixture]
    public class SaleProcessorTest : WithFullDatabaseTest
    {
        [TearDown]
        public void DeleteCommands()
        {
            Database.DeleteAll<LocalCommandEnvelope>();
            Database.DeleteAll<Order>();
        }

        [Test]
        public void CanMakeSaleForTwoItemsAndPayByCash()
        {
            //Given             
            var saleAndContext = AFullyPaidCashSaleWithTwoItems().Build();
            var sale = saleAndContext.Order;
            var context = saleAndContext.Context;
            var inventoryProductBalance = AnInventoryProduct().Balance;
            var anotherInventoryProductBalance = AnotherInventoryProduct().Balance;

            var saleProcessor = Resolve<SaleProcessor>();

            //When 
            var result = saleProcessor.Process(sale, context);

            //Then
            CheckResult(result);

            var actualCommands = ExtractCommandEnvelopes(Resolve<ILocalCommandEnvelopeRepository>().GetNextOutgoingBatch());

            //Make sure all expected command envelopes and commands were sent out, in order
            new CommandEnvelopeChecker(
                  new CommandEnvelopeChecks(DocumentType.Order,
                      CommandType.CreateMainOrder,
                      CommandType.AddMainOrderLineItem,
                      CommandType.AddMainOrderLineItem,
                      CommandType.OrderPaymentInfo,
                      CommandType.ConfirmMainOrder),
                  new CommandEnvelopeChecks(DocumentType.Order,
                      CommandType.CloseOrder),
                  new CommandEnvelopeChecks(DocumentType.Order,
                      CommandType.AddExternalDocRef),
                  new CommandEnvelopeChecks(DocumentType.Invoice,
                      CommandType.CreateInvoice,
                      CommandType.AddInvoiceLineItem,
                      CommandType.AddInvoiceLineItem,
                      CommandType.ConfirmInvoice),
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


            var saleFromDb = Database.GetWithChildren<Order>(sale.Id, recursive: true);

            CheckSale(saleFromDb, sale);
            CheckInventoryHasBeenAdjusted(AnInventoryProduct(), inventoryProductBalance, sale);
            CheckInventoryHasBeenAdjusted(AnotherInventoryProduct(), anotherInventoryProductBalance, sale);
        }

        [Test]
        public void CanMakeSaleForOneItemAndPayByCheque()
        {
            //Given             
            var saleAndContext = ASaleFullyPaidByChequeWithOneItem().Build();
            var sale = saleAndContext.Order;
            var context = saleAndContext.Context;

            var saleProcessor = Resolve<SaleProcessor>();

            //When
            var result = saleProcessor.Process(sale, context);
            
            //Then
            CheckResult(result);

            var actualCommands = ExtractCommandEnvelopes(Resolve<ILocalCommandEnvelopeRepository>().GetNextOutgoingBatch());

            //Make sure all expected command envelopes and commands were sent out, in order
            new CommandEnvelopeChecker(
                new CommandEnvelopeChecks(DocumentType.Order,
                    CommandType.CreateMainOrder,
                    CommandType.AddMainOrderLineItem,
                    CommandType.OrderPaymentInfo,
                    CommandType.ConfirmMainOrder),
                new CommandEnvelopeChecks(DocumentType.Order,
                    CommandType.CloseOrder),
                new CommandEnvelopeChecks(DocumentType.Order,
                    CommandType.AddExternalDocRef),
                new CommandEnvelopeChecks(DocumentType.Invoice,
                    CommandType.CreateInvoice,
                    CommandType.AddInvoiceLineItem,
                    CommandType.ConfirmInvoice),
                new CommandEnvelopeChecks(DocumentType.DispatchNote,
                    CommandType.CreateDispatchNote,
                    CommandType.AddDispatchNoteLineItem,
                    CommandType.ConfirmDispatchNote),
                new CommandEnvelopeChecks(DocumentType.InventoryAdjustmentNote,
                    CommandType.CreateInventoryAdjustmentNote,
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

            var saleFromDb = Database.GetWithChildren<Order>(sale.Id, recursive: true);

            Assert.AreEqual(sale.TotalValueIncludingVat, saleFromDb.TotalValueIncludingVat);
            CheckPayment(saleFromDb.Payments[0], sale.Payments[0]);
        }

        private void CheckInventoryHasBeenAdjusted(Inventory inventoryProduct, decimal balanceBeforeSale, Order order)
        {
            var saleQuantity = order.LineItems.First(l => l.ProductMasterId == inventoryProduct.ProductMasterID).Quantity;
            var expectedBalanceAfterSale = balanceBeforeSale - saleQuantity;
            var actualInventoryQuantity = Database.Get<Inventory>(inventoryProduct.Id).Balance;

            Assert.AreEqual(expectedBalanceAfterSale, actualInventoryQuantity, "local inventory after sale");
        }

        private void CheckSale(Order orderFromDb, Order order)
        {
            Assert.AreEqual(order.TotalValueIncludingVat, orderFromDb.TotalValueIncludingVat);
            Assert.AreEqual(order.ShipToAddress, orderFromDb.ShipToAddress);
            Assert.AreEqual(order.Note, orderFromDb.Note);
            Assert.AreEqual(order.Outlet.Id, orderFromDb.Outlet.Id);
            Assert.AreEqual(order.OutletMasterId, orderFromDb.OutletMasterId);
            Assert.AreEqual(order.SaleDiscount, orderFromDb.SaleDiscount);
            Assert.AreEqual(order.LineItems.Count, orderFromDb.LineItems.Count);

            CheckLineItem(orderFromDb.LineItems[0], order.LineItems[0]);
            CheckLineItem(orderFromDb.LineItems[1], order.LineItems[1]);

            Assert.AreEqual(order.Payments.Count, orderFromDb.Payments.Count);
            
            CheckPayment(orderFromDb.Payments[0], order.Payments[0]);

            Assert.AreEqual(0, orderFromDb.NewPayments.Count, "new payments");
        }

        private void CheckPayment(Payment paymentFromDb, Payment payment)
        {
            Assert.AreEqual(payment.PaymentMode, paymentFromDb.PaymentMode);
            Assert.AreEqual(payment.PaymentReference, paymentFromDb.PaymentReference);
            Assert.AreEqual(payment.Amount, paymentFromDb.Amount);
            Assert.AreEqual(payment.Bank, paymentFromDb.Bank);
            Assert.AreEqual(payment.BankBranch, paymentFromDb.BankBranch);

            //Compare dates ignoring milliseonds, which are truncated by SQLite
            var sameDay = Math.Abs((payment.DueDate - paymentFromDb.DueDate).TotalSeconds) < 1;
            Assert.IsTrue(sameDay, "Expected due date {0} but was {1}", payment.DueDate, paymentFromDb.DueDate);
        }

        private void CheckLineItem(ProductLineItem lineItemFromDb, ProductLineItem lineItem)
        {
            Assert.AreEqual(lineItem.Product.Id, lineItemFromDb.Product.Id);
            Assert.AreEqual(lineItem.Value, lineItemFromDb.Value);
            Assert.AreEqual(lineItem.VatValue, lineItemFromDb.VatValue);
            Assert.AreEqual(lineItem.Quantity, lineItemFromDb.Quantity);            
        }
    }
}

