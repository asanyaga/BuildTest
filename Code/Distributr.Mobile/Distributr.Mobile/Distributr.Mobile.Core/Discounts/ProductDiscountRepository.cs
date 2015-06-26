using System;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Utility.MasterData;
using Distributr.Mobile.Data;

namespace Distributr.Mobile.Core.Discounts
{
    public class ProductDiscountRepository : BaseRepository<ProductDiscount>, IProductDiscountRepository
    {
        private readonly Database database;

        public ProductDiscountRepository(Database database) : base(database)
        {
            this.database = database;
        }

        public ProductDiscount GetProductDiscount(Guid productId, Guid tierId)
        {
            return
                database.Find<ProductDiscount>(
                    p => p.ProductMasterId == productId && p.TierMasterId == tierId && p._Status == EntityStatus.Active);
        }

        // 
        // Currently not used on Mobile
        //
        public void AddDiscount(Guid discountId, DateTime effectiveDate, decimal discountRate, DateTime endDate, bool isByQuantity, decimal quantity)
        {            
            throw new NotImplementedException();
        }

        public void DeactivateLineItem(Guid lineItemId)
        {
            throw new NotImplementedException();
        }

        public QueryResult<ProductDiscount> Query(QueryStandard q)
        {
            throw new NotImplementedException();
        }
    }
}
