using System;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Mobile.Core.Discounts;
using Distributr.Mobile.Core.Test.Support;
using NUnit.Framework;

namespace Distributr.Mobile.Core.Test.Discounts
{
    [TestFixture]
    [Category("Database")]
    public class FreeOfChargeDiscountRepositoryTest : WithEmptyDatabaseTest
    {

        public static FreeOfChargeDiscount MakeDiscount(Guid productId, int startDateOffset = -1, int endDateOffset = 1)
        {
            return new FreeOfChargeDiscount(Guid.NewGuid())
            {
                ProductRefMasterId = productId, 
                StartDate = DateTime.Now.AddDays(startDateOffset),
                EndDate = DateTime.Now.AddDays(endDateOffset),
            };
        }

        [Test]
        public void ReturnsTrueForActiveFreeOfChargeProduct()
        {
            var productId = Guid.NewGuid();
            Database.Insert(MakeDiscount(productId));
            var repository = Resolve<FreeOfChargeDiscountRepository>();

            Assert.IsTrue(repository.IsProductFreeOfCharge(productId), "is free of charge?");
        }

        [Test]
        public void ReturnsFalseForUnkownProductId()
        {
            var productId = Guid.NewGuid();
            Database.Insert(MakeDiscount(productId));
            var repository = Resolve<FreeOfChargeDiscountRepository>();

            Assert.IsFalse(repository.IsProductFreeOfCharge(Guid.NewGuid()), "is free of charge?");
        }

        [Test]
        public void ReturnsFalseWhenDiscountHasExpired()
        {
            var productId = Guid.NewGuid();
            Database.Insert(MakeDiscount(productId, endDateOffset: -1));
            var repository = Resolve<FreeOfChargeDiscountRepository>();

            Assert.IsFalse(repository.IsProductFreeOfCharge(productId), "is free of charge?");
        }
    }
}
