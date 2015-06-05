using System;
using Distributr.Core.Domain.InventoryEntities;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Mobile.Core.Test.Support;
using NUnit.Framework;
using SQLiteNetExtensions.Extensions;

namespace Distributr.Mobile.Core.Products
{
    [TestFixture, Category("Database")]
    public class ProductTest : WithEmptyDatabaseTest
    {
        private void InsertProduct(string description, decimal inventoryBalance = 0)
        {
            var saleProduct = new SaleProduct(Guid.NewGuid());
            saleProduct.Description = description;

            var inventoryProduct = new Inventory(Guid.NewGuid());
            inventoryProduct.Balance = inventoryBalance;
            inventoryProduct.ProductMasterID = saleProduct.Id;

            Database.InsertWithChildren(saleProduct, recursive: true);
            Database.InsertWithChildren(inventoryProduct, recursive:true);
        }

        [Test]
        public void CanLoadProductsOrderedByName()
        {
            InsertProduct("a");
            InsertProduct("g");
            InsertProduct("z");

            var result = Database.Query<ProductDetails>(ProductDetails.AllProductsByNameAscending);

            Assert.AreEqual(3, result.Count, "count");
            Assert.AreEqual("a", result[0].Description, "first description");
            Assert.AreEqual("g", result[1].Description, "second description");
            Assert.AreEqual("z", result[2].Description, "third description");

            result = Database.Query<ProductDetails>(ProductDetails.AllProductsByNameDescending);

            Assert.AreEqual(3, result.Count, "count");                       
            Assert.AreEqual("z", result[0].Description, "first description");
            Assert.AreEqual("g", result[1].Description, "second description");
            Assert.AreEqual("a", result[2].Description, "third description");
        }

        [Test]
        public void CanLoadInventoryProductsOrderedByBalance()
        {
            InsertProduct("1", 1);
            InsertProduct("2", 2);
            InsertProduct("3", 3);

            var result = Database.Query<ProductDetails>(ProductDetails.InventoryProductsByBalanceAscending);

            Assert.AreEqual(3, result.Count, "count");    
            Assert.AreEqual("1", result[0].Description, "first description");
            Assert.AreEqual("2", result[1].Description, "second description");
            Assert.AreEqual("3", result[2].Description, "third description");
           
            result = Database.Query<ProductDetails>(ProductDetails.InventoryProductsByBalanceDescending);

            Assert.AreEqual(3, result.Count, "count");    
            Assert.AreEqual("3", result[0].Description, "first description");
            Assert.AreEqual("2", result[1].Description, "second description");
            Assert.AreEqual("1", result[2].Description, "third description");
        }
    }
}
