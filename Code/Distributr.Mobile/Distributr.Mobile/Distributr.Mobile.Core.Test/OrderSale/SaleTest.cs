using System.Linq;
using Distributr.Mobile.Core.OrderSale;
using NUnit.Framework;

namespace Distributr.Mobile.Core.Test.OrderSale
{
    [TestFixture]
    public class SaleTest
    {

        [Test]
        public void CanSellReturnablesForExistingItem()
        {
            var sale = new MockOrderBuilder()
                .WithSaleLineItem()
                .Build();

            Assert.AreEqual(1, sale.AllInvoiceItems.Count, "invoice items");
            
            sale.SellReturnablesForItem(sale.LineItems.First());

            Assert.AreEqual(2, sale.AllInvoiceItems.Count, "invoice items");
            Assert.AreEqual(1, sale.ReturnableLineItems.First().SaleQuantity, "sale quantity");
            Assert.AreEqual(LineItemStatus.Approved, sale.ReturnableLineItems.First().LineItemStatus, "status");
        }

        [Test]
        public void CanSellReturnableIncludingContainerForExistingItem()
        {
            var sale = new MockOrderBuilder()
                .WithSaleLineItem(quantity:25)
                .Build();

            Assert.AreEqual(1, sale.AllInvoiceItems.Count, "invoice items");

            sale.SellReturnablesForItem(sale.LineItems.First());

            Assert.AreEqual(3, sale.AllInvoiceItems.Count, "invoice items");
            Assert.AreEqual(25, sale.ReturnableLineItems.First().SaleQuantity, "sale quantity");
            Assert.AreEqual(LineItemStatus.Approved, sale.ReturnableLineItems.First().LineItemStatus, "status 1");
            Assert.AreEqual(1, sale.ReturnableLineItems[1].SaleQuantity, "sale quantity");
            Assert.AreEqual(LineItemStatus.Approved, sale.ReturnableLineItems[1].LineItemStatus, "status 2");
        }

        [Test]
        public void CanUnsellReturnablesForExistingItem()
        {
            var sale = new MockOrderBuilder()
                      .WithSaleLineItem(quantity: 25, sellReturnables:true)
                      .Build();

            Assert.AreEqual(3, sale.AllInvoiceItems.Count, "invoice items");
            
            sale.UnsellReturnablesForItem(sale.LineItems.First());

            Assert.AreEqual(1, sale.AllInvoiceItems.Count, "invoice items");
        }
    }
}
