using System;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Mobile.Core.Test.Support;
using NUnit.Framework;

namespace Distributr.Mobile.Core.Test.Discounts
{
    [TestFixture]
    [Category("Database")]
    public class ProductDiscountGroupRepositoryTest : WithEmptyDatabaseTest
    {

        public static ProductGroupDiscount MakeDiscount(Guid productId, Guid groupId, int effectiveDateOffset = -1,
            int endDateOffset = 1, decimal quantity = 100)
        {
            return new ProductGroupDiscount(Guid.NewGuid())
            {
                ProductMasterId = productId, 
                DiscountGroupMasterId = groupId, 
                EffectiveDate = DateTime.Now.AddDays(effectiveDateOffset),
                EndDate = DateTime.Now.AddDays(endDateOffset),
                _Status = EntityStatus.Active,
                Quantity = quantity
            };
        }

        [Test]
        public void LoadsMostSpecificDiscountItem()
        {
            var productId = Guid.NewGuid();
            var groupId = Guid.NewGuid();

            Database.Insert(MakeDiscount(productId, groupId, quantity: 50));
            Database.Insert(MakeDiscount(productId, groupId, quantity:150));
            Database.Insert(MakeDiscount(productId, Guid.NewGuid(), quantity: 150));
            Database.Insert(MakeDiscount(productId, groupId, effectiveDateOffset: 100, quantity: 200));
            Database.Insert(MakeDiscount(productId, groupId, endDateOffset: -1, quantity: 210));
            Database.Insert(MakeDiscount(Guid.NewGuid(), groupId, quantity: 250));

            var repository = Resolve<IProductDiscountGroupRepository>();

            Assert.AreEqual(150, repository.GetCurrentCustomerDiscount(groupId, productId, 300).Quantity);
        }

        [Test]
        public void ReturnsDefaultWhenDiscountHasExpired()
        {
            var productId = Guid.NewGuid();
            var groupId = Guid.NewGuid();

            Database.Insert(MakeDiscount(productId, groupId, quantity: 50, endDateOffset: -1));
            var repository = Resolve<IProductDiscountGroupRepository>();

            Assert.IsNull(repository.GetCurrentCustomerDiscount(groupId, productId, 60));
        }

        [Test]
        public void ReturnsDefaultWhenQuantityBelowThreshold()
        {
            var productId = Guid.NewGuid();
            var groupId = Guid.NewGuid();

            Database.Insert(MakeDiscount(productId, groupId, quantity: 100));
            var repository = Resolve<IProductDiscountGroupRepository>();

            Assert.IsNull(repository.GetCurrentCustomerDiscount(groupId, productId, 60));
        }

        [Test] public void ReturnsDiscountWhenQuantityIsEqualToThreshold()
        {
            var productId = Guid.NewGuid();
            var groupId = Guid.NewGuid();

            Database.Insert(MakeDiscount(productId, groupId, quantity: 100));
            var repository = Resolve<IProductDiscountGroupRepository>();

            Assert.IsNotNull(repository.GetCurrentCustomerDiscount(groupId, productId, 100));
        }
    }
}
