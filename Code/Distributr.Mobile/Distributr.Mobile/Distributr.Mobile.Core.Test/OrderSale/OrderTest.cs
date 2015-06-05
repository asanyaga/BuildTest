
using Distributr.Core.Domain.Transactional;
using Distributr.Mobile.Core.OrderSale;
using NUnit.Framework;

namespace Distributr.Mobile.Core.Test.OrderSale
{
    [TestFixture]
    public class OrderTest
    {
        [Test]
        public void CalculatesCorrectTotalIncludingVatRoundedUp()
        {
            var order = new MockOrderBuilder()
                .WithOrderLineItem(price: 1.5m, vatRate: 0.10m, quantity: 100m)
                .WithOrderLineItem(price: 0.99m, vatRate: 0.20m, quantity: 49m)
                .Build();

            //165
            //58.212
            //= 223.212
            //= 224 when rounded up
            Assert.AreEqual(224m, order.TotalValueIncludingVat, "Total inc VAT");
        }

        [Test]
        public void GeneratesCorrectChequePaymentDetails()
        {
            var order = new MockOrderBuilder()
                .WithChequePayment("12345678", "0267K", "09875", 1m)
                .Build();

            Assert.AreEqual(PaymentMode.Cheque, order.Payments[0].PaymentMode, "Payment Mode");
            Assert.AreEqual(string.Format("{0} - {1} - {2}", "12345678", "0267K", "09875"), order.Payments[0].PaymentReference, "Payment Reference");
            Assert.AreEqual(1m, order.Payments[0].Amount, "Payment Amount");
            Assert.AreEqual("0267K", order.Payments[0].Bank, "Bank");
            Assert.AreEqual("09875", order.Payments[0].BankBranch, "Bank Branch");
        }

        [Test]
        public void GeneratesCorrectCashPaymentDetails()
        {
            var order = new MockOrderBuilder()
                .WithCashPayment("cash ref", 99.9m)
                .Build();

            Assert.AreEqual(PaymentMode.Cash, order.Payments[0].PaymentMode, "Payment Mode");
            Assert.AreEqual("cash ref", order.Payments[0].PaymentReference, "Payment Reference");
            Assert.AreEqual(99.9m, order.Payments[0].Amount, "Payment Amount");            
        }

        [Test]
        public void CalculatesCorrectTotalPayments()
        {
            var order = new MockOrderBuilder()
                .WithCashPayment(amount: 100)
                .WithChequePayment(amount: 59.50m)
                .Build();

            Assert.AreEqual(159.50m, order.TotalPayments);
        }

        [Test]
        public void UpdatesAmountDueAfterReceivingPayment()
        {
            var order = new MockOrderBuilder()
                .WithOrderLineItem(price: 100, quantity: 1)
                .Build();

            //includes 10% VAT
            Assert.AreEqual(order.BalanceOutstanding, 110);

            order.AddCashPayment("partial payment ", 50m);

            Assert.AreEqual(order.BalanceOutstanding, 60);
        }

        [Test]
        public void ConfirmsNewPayments()
        {
            var order = new MockOrderBuilder()
                .WithOrderLineItem(price: 100, quantity: 1)
                .WithCashPayment()
                .Build();

            Assert.AreEqual(1, order.NewPayments.Count, "new payments before confirm");

            order.ConfirmNewPayments();

            Assert.AreEqual(0, order.NewPayments.Count, "new payments after confirm");
            Assert.AreEqual(PaymentStatus.Confirmed, order.Payments[0].PaymentStatus, "payment status");
        }

        [Test]
        public void ReportsFullyPaidWhenPaymentValueMatchesOrderValue()
        {
            var order = new MockOrderBuilder()
                .WithOrderLineItem(price: 100)
                //Includes 10% VAT
                .WithCashPayment(amount: 110)
                .Build();         
   
            Assert.IsTrue(order.IsFullyPaid, "fully paid");
        }

        [Test]
        public void DoesNotReportFullyPaidWhenPaymentValueIsLessThanOrderValue()
        {
            var order = new MockOrderBuilder()
                .WithOrderLineItem(price: 100)
                //Includes 10% VAT
                .WithCashPayment(amount: 10)
                .Build();

            Assert.IsFalse(order.IsFullyPaid, "fully paid");
        }
    }
}
