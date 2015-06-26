using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Mobile.Core.Test.Support;
using NUnit.Framework;
using SQLiteNetExtensions.Extensions;

namespace Distributr.Mobile.Core.Test.Discounts
{
    [TestFixture]
    [Category("Database")]
    public class SaleValueDiscountRepositoryTest : WithEmptyDatabaseTest
    {
        public SaleValueDiscount MakeDiscount(ProductPricingTier tier, int effectiveDateOffset = -1, int endDateOffset = 1, 
            decimal amount = 100, EntityStatus status = EntityStatus.Active)
        {
            return new SaleValueDiscount(Guid.NewGuid())
            {
                Tier = tier,
                _Status = status,
                DiscountItems = new List<SaleValueDiscount.SaleValueDiscountItem>()
                {
                    MakeItem(effectiveDateOffset, endDateOffset, amount, status)
                }
            };
        }

        public static SaleValueDiscount.SaleValueDiscountItem MakeItem( int effectiveDateOffset = -1, int endDateOffset = 1, 
            decimal amount = 100, EntityStatus status = EntityStatus.Active)
        {
            return new SaleValueDiscount.SaleValueDiscountItem(Guid.NewGuid())
            {
                _Status = status,
                DiscountThreshold = amount,
                EffectiveDate = DateTime.Now.AddDays(effectiveDateOffset),
                EndDate = DateTime.Now.AddDays(endDateOffset),
                DiscountValue = 0.10m
            };
        }

        [Test]
        public void LoadsCurrentItemWhenSaleValueGreaterThanThreshold()
        {
            var tierId = Guid.NewGuid();

            var tier = new ProductPricingTier(tierId) {Name = "tier"};

            var discount = MakeDiscount(tier, endDateOffset:-1);
            discount.DiscountItems.Add(MakeItem(endDateOffset: -1));
            discount.DiscountItems.Add(MakeItem(amount: 150));
            discount.DiscountItems.Add(MakeItem(amount: 175, status: EntityStatus.Inactive));
            discount.DiscountItems.Add(MakeItem(amount: 200, effectiveDateOffset: 100));

            Database.InsertWithChildren(discount, true);

            var repository = Resolve<ISaleValueDiscountRepository>();

            Assert.AreEqual(150, repository.GetCurrentDiscount(200, tierId).CurrentSaleValue);
        }

        [Test]
        public void ReturnsDefaultWhenSaleValueLessThanThreshold()
        {
            var tierId = Guid.NewGuid();
            var tier = new ProductPricingTier(tierId) { Name = "tier" };
            var discount = MakeDiscount(tier, amount: 250);

            Database.InsertWithChildren(discount, true);

            var repository = Resolve<ISaleValueDiscountRepository>();

            Assert.IsNull(repository.GetCurrentDiscount(249, tierId), "discount not null");            
        }
    }
}
