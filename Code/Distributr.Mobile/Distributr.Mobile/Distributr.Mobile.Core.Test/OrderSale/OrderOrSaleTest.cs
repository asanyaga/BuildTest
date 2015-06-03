using System;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Mobile.Core.OrderSale;
using Distributr.Mobile.Core.Test.Support;
using NUnit.Framework;
using SQLiteNetExtensions.Extensions;

namespace Distributr.Mobile.Core.Test.OrderSale
{
    [TestFixture, Category("Database")]
    public class OrderOrSaleTest : WithEmptyDatabaseTest
    {
        private void InsertOrder(string reference, ProcessingStatus processingStatus = ProcessingStatus.Deliverable)
        {
            var outlet = new Outlet(Guid.NewGuid()) {Name = "The Outlet"};

            Database.InsertWithChildren(new Order(Guid.NewGuid(), outlet)
            {
                OrderReference = reference,
                ProcessingStatus = processingStatus,
                OutletMasterId = outlet.Id
            });
        }

        [TearDown]
        public void DeleteOrders()
        {
            Database.DeleteAll(typeof(Order));
        }

        [Test]
        public void ReturnsOrdersByInsertOrderDescending()
        {
            InsertOrder("1");
            InsertOrder("2");
            
            var result = Database.Query<OrderOrSale>(OrderOrSale.AllOrderAndSales);

            Assert.AreEqual(2, result.Count, "count");
            Assert.AreEqual("2", result[0].OrderSaleReference, "first ref");
            Assert.AreEqual("1", result[1].OrderSaleReference, "second ref");
        }

        [Test]
        public void ReturnsOrdersPendingDelivery()
        {
            InsertOrder("1", ProcessingStatus.Approved);
            InsertOrder("2", ProcessingStatus.Deliverable);

            var result = Database.Query<OrderOrSale>(OrderOrSale.OrdersPendingDelivery);

            Assert.AreEqual(1, result.Count, "count");
            Assert.AreEqual("2", result[0].OrderSaleReference, "ref");
        }
    }
}
