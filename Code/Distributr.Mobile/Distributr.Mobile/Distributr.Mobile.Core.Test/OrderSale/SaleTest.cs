using System;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Mobile.Core.OrderSale;
using NUnit.Framework;

namespace Distributr.Mobile.Core.Test.OrderSale
{
    [TestFixture]
    public class SaleTest
    {
        [Test]
        public void CanAddItemWithReturnables()
        {
            var sale = new Sale(Guid.NewGuid(), new Outlet());
            var product = MockOrderBuilder.AProductWithPrice(1);
            product.VATClass = MockOrderBuilder.AVatClassWithRate(0.10m);

            sale.AddItem(product, 1, 0, 1, true);

            Assert.AreEqual(1, sale.ProductLineItems.Count, "sale items");
            Assert.AreEqual(1, sale.SoldReturnables().Count, "returnable items");
        }

        [Test]
        public void CanAddItemWithoutSellingReturnables()
        {
            var sale = new Sale(Guid.NewGuid(), new Outlet());
            var product = MockOrderBuilder.AProductWithPrice(1);
            product.VATClass = MockOrderBuilder.AVatClassWithRate(0.10m);

            sale.AddItem(product, 1, 0, 1, false);

            Assert.AreEqual(1, sale.ProductLineItems.Count, "sale items");
            Assert.AreEqual(0, sale.SoldReturnables().Count, "returnable items");
        }

        [Test]
        public void CanAddItemsByCaseWithReturnables()
        {
            var sale = new Sale(Guid.NewGuid(), new Outlet());
            var product = MockOrderBuilder.AProductWithPrice(1);
            product.VATClass = MockOrderBuilder.AVatClassWithRate(0.10m);

            sale.AddItem(product, 0, 2, 1, true);

            Assert.AreEqual(1, sale.ProductLineItems.Count, "sale items");
            Assert.AreEqual(2, sale.ReturnableLineItems.Count, "returnable items");            
        }

        [Test]
        public void CanRemoveItemWithReturnables()
        {
            var sale = new Sale(Guid.NewGuid(), new Outlet());
            var product = MockOrderBuilder.AProductWithPrice(1);
            product.VATClass = MockOrderBuilder.AVatClassWithRate(0.10m);

            sale.AddItem(product, 1, 0, 1, true);
            sale.RemoveItem(sale.ProductLineItems[0].Id);

            Assert.AreEqual(0, sale.ProductLineItems.Count, "sale items");
            Assert.AreEqual(0, sale.ReturnableLineItems.Count, "returnable items");
        }

        [Test]
        public void CanAddAndRemoveMultipleItems()
        {
            var sale = new Sale(Guid.NewGuid(), new Outlet());
            var product = MockOrderBuilder.AProductWithPrice(1);
            product.VATClass = MockOrderBuilder.AVatClassWithRate(0.10m);

            var product2 = MockOrderBuilder.AProductWithPrice(1);
            product2.VATClass = MockOrderBuilder.AVatClassWithRate(0.10m);

            sale.AddItem(product, 1, 0, 1, true);            
            sale.AddItem(product2, 0, 1, 1, true);

            Assert.AreEqual(2, sale.ProductLineItems.Count, "sale items");
            Assert.AreEqual(3, sale.ReturnableLineItems.Count, "returnable items");            

            sale.RemoveItem(sale.ProductLineItems[0].Id);

            Assert.AreEqual(1, sale.ProductLineItems.Count, "sale items");
            Assert.AreEqual(2, sale.ReturnableLineItems.Count, "returnable items");            
        }

        [Test]
        public void AdjustsReturnablesForSpecifiedLineItemOnly()
        {
            var sale = new Sale(Guid.NewGuid(), new Outlet());
            var product = MockOrderBuilder.AProductWithPrice(1);
            product.VATClass = MockOrderBuilder.AVatClassWithRate(0.10m);

            var product2 = MockOrderBuilder.AProductWithPrice(1);
            product2.VATClass = MockOrderBuilder.AVatClassWithRate(0.10m);            

            product2.ReturnableProduct = product.ReturnableProduct;
            product2.ReturnableProductMasterId = product.ReturnableProductMasterId;

            product2.ReturnableContainer = product.ReturnableContainer;
            product2.ReturnableContainerMasterId = product.ReturnableContainerMasterId;

            sale.AddItem(product, 1, 0, 1, true);
            sale.AddItem(product2, 0, 1, 1, true);

            sale.RemoveItem(sale.ProductLineItems[0].Id);

            Assert.AreEqual(1, sale.ProductLineItems.Count, "sale items");
            Assert.AreEqual(2, sale.ReturnableLineItems.Count, "returnable items");
            Assert.AreEqual(24, sale.ReturnableLineItems[0].SaleQuantity, "returnable items sale quantity");
            Assert.AreEqual(1, sale.ReturnableLineItems[1].SaleQuantity, "returnable items case quantity");
        }
    }
}
