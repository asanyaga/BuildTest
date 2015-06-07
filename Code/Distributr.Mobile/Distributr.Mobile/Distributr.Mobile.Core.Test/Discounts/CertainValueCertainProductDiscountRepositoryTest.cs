using System;
using System.Collections.Generic;
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
    public class CertainValueCertainProductDiscountRepositoryTest : WithEmptyDatabaseTest
    {
        public static CertainValueCertainProductDiscount MakeDiscount(decimal value = 100, int effectiveDateOffset = -1,
            int endDateOffset = 1, EntityStatus status = EntityStatus.Active)
        {
            return new CertainValueCertainProductDiscount(Guid.NewGuid())
            {
                CertainValueCertainProductDiscountItems =
                    new List<CertainValueCertainProductDiscount.CertainValueCertainProductDiscountItem>
                    {
                        MakeItem(value, effectiveDateOffset, endDateOffset, status)
                    }
            };
        }

        public static CertainValueCertainProductDiscount.CertainValueCertainProductDiscountItem MakeItem(decimal value = 100, int effectiveDateOffset = -1,
            int endDateOffset = 1, EntityStatus status = EntityStatus.Active)
        {
            return new CertainValueCertainProductDiscount.CertainValueCertainProductDiscountItem(Guid.NewGuid())
            {
                EffectiveDate = DateTime.Now.AddDays(effectiveDateOffset),
                EndDate = DateTime.Now.AddDays(endDateOffset),
                CertainValue = value,
                IsActive = status,
                _Status = status
            };
        }

        [Test]
        public void CanLoadDiscountWhichIsStillCurrent()
        {
            Database.InsertWithChildren(MakeDiscount(), true);
            var repository = Resolve<ICertainValueCertainProductDiscountRepository>();
            var discount = repository.GetByAmount(110);

            Assert.NotNull(discount, "discount not found");
        }

        [Test]
        public void CanLoadDiscountWhichIsStillCurrentAndValuesAreEqual()
        {
            Database.InsertWithChildren(MakeDiscount(), true);
            var repository = Resolve<ICertainValueCertainProductDiscountRepository>();
            var discount = repository.GetByAmount(100);

            Assert.NotNull(discount, "discount not found");
        }

        [Test]
        public void ReturnDefaultWhenDiscountHasExpired()
        {
            Database.InsertWithChildren(MakeDiscount(endDateOffset: -1), true);
            var repository = Resolve<ICertainValueCertainProductDiscountRepository>();
            var discount = repository.GetByAmount(110);

            Assert.Null(discount, "discount found");
        }

        [Test]
        public void ReturnDefaultWhenAmountBelowCertainValue()
        {
            Database.InsertWithChildren(MakeDiscount(200), true);
            var repository = Resolve<ICertainValueCertainProductDiscountRepository>();
            var discount = repository.GetByAmount(110);

            Assert.Null(discount, "discount found");
        }

        [Test]
        public void UsesCurrentDiscountWithGreatestAmount()
        {
            var discount = MakeDiscount(value: 100, endDateOffset: -1);
            discount.CertainValueCertainProductDiscountItems.Add(MakeItem(value: 150, endDateOffset:-1));
            discount.CertainValueCertainProductDiscountItems.Add(MakeItem(value: 200, endDateOffset:-1));
            discount.CertainValueCertainProductDiscountItems.Add(MakeItem(value: 175));
            Database.InsertWithChildren(discount, true);

            var repository = Resolve<ICertainValueCertainProductDiscountRepository>();
            var discountFromDb = repository.GetByAmount(250);

            Assert.NotNull(discount.LatestFreeOfChargeDiscountItem(), "discount item not found");
            Assert.AreEqual(175, discountFromDb.CurrentValue, "certain value");

        }
    }
}