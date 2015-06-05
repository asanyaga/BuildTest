using System;
using System.Collections.Generic;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Mobile.Data;

namespace Distributr.Mobile.Core.Discounts
{
    public class ProductDiscountGroupRepository : BaseRepository<ProductGroupDiscount>, IProductDiscountGroupRepository
    {
        private readonly Database database;

        public ProductDiscountGroupRepository(Database database) : base(database)
        {
            this.database = database;
        }

        public ProductGroupDiscount GetCurrentCustomerDiscount(Guid groupId, Guid productId, decimal quantity)
        {
            var date = DateTime.Now;
            var discountdate = new DateTime(date.Year, date.Month, date.Day);

            return database.Table<ProductGroupDiscount>()
                .Where(
                    p =>
                        p.ProductMasterId == productId && p.DiscountGroupMasterId == groupId && p.Quantity <= quantity &&
                        p._Status == EntityStatus.Active && p.EffectiveDate.Date <= discountdate &&
                        p.EndDate >= discountdate)
                .OrderByDescending(p => p.Quantity)
                .ThenByDescending(p => p.EffectiveDate)
                .FirstOrDefault();
        }

        // 
        // Currently not used on Mobile
        //
        public void SetLineItemsInactive(ProductGroupDiscount entity)
        {
            throw new NotImplementedException();
        }

        public List<ProductGroupDiscount> GetByDiscountGroup(Guid discountGroup, bool includeDeactivated = false)
        {
            throw new NotImplementedException();
        }

        public ProductGroupDiscount GetByDiscountGroupCode(string discountgroupCode)
        {
            throw new NotImplementedException();
        }

        public ProductGroupDiscount GetByGroupbyProductByQuantity(Guid groupId, Guid productId, decimal quantity)
        {
            throw new NotImplementedException();
        }
    }
}
