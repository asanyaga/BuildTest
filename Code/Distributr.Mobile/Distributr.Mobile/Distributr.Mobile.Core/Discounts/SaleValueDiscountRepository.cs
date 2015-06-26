using System;
using System.Linq;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Utility.MasterData;
using Distributr.Mobile.Data;
using SQLiteNetExtensions.Extensions;

namespace Distributr.Mobile.Core.Discounts
{
    public class SaleValueDiscountRepository : BaseRepository<SaleValueDiscount>, ISaleValueDiscountRepository
    {
        private readonly Database database;

        public SaleValueDiscountRepository(Database database) : base(database)
        {
            this.database = database;
        }

        public SaleValueDiscount GetCurrentDiscount(decimal amount, Guid tierId)
        {
            var discount = database.GetAllWithChildren<SaleValueDiscount>(s => s.TierMasterId == tierId).FirstOrDefault();
            if (discount == null) return default(SaleValueDiscount);
            return discount.GetPercentageDiscount(amount) > 0 ? discount : default(SaleValueDiscount);
        }

        // 
        // Currently not used on Mobile
        //
        public void AddSaleValueDiscount(Guid discountId, DateTime effectiveDate, decimal discountRate, decimal saleValue, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        public SaleValueDiscount GetByAmount(decimal amount, Guid tier)
        {
            throw new NotImplementedException();
        }

        public void DeactivateLineItem(Guid lineItemId)
        {
            throw new NotImplementedException();
        }

        public QueryResult<SaleValueDiscount> Query(QueryStandard q)
        {
            throw new NotImplementedException();
        }
    }
}
