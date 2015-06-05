using System;
using System.Linq;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Utility.MasterData;
using Distributr.Mobile.Data;

namespace Distributr.Mobile.Core.Discounts
{
    public class PromotionDiscountRepository : BaseRepository<PromotionDiscount>, IPromotionDiscountRepository
    {
        private readonly Database database;

        public PromotionDiscountRepository(Database database) : base(database)
        {
            this.database = database;
        }

        public PromotionDiscount GetCurrentDiscount(Guid productMasterId)
        {
            return
                database.Table<PromotionDiscount>()
                    .FirstOrDefault(
                        s => s.ProductMasterId == productMasterId &&
                             s.CurrentEffectiveDate.Date <= DateTime.Now.Date &&
                             s.CurrentEndDate.Date >= DateTime.Now.Date);
        }

        // 
        // Currently not used on Mobile
        //
        public void AddFreeOfChargeDiscount(Guid focId, int parentProductQuantity, Guid? freeOfChargeProduct, int? freeOfChargeQuantity,
            DateTime effectiveDate, decimal DiscountRate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        public PromotionDiscount GetByProductId(Guid productMasterId)
        {
            throw new NotImplementedException();
        }

        public void DeactivateLineItem(Guid lineItemId)
        {
            throw new NotImplementedException();
        }

        public PromotionDiscount GetByProductAndQuantity(Guid productMasterId, int quantity)
        {
            throw new NotImplementedException();
        }

        public QueryResult<PromotionDiscount> Query(QueryStandard query)
        {
            throw new NotImplementedException();
        }
    }
}
