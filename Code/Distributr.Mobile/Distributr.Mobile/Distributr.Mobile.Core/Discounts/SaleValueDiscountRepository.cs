using System;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Utility.MasterData;
using Distributr.Mobile.Data;

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
            return database.Table<SaleValueDiscount>()
                .Where(
                    s =>
                        s.TierMasterId == tierId && s.CurrentSaleValue <= amount &&
                        s.CurrentEffectiveDate.Date <= DateTime.Now.Date && s.CurrentEndDate.Date >= DateTime.Now.Date)
                .OrderBy(o => o.CurrentSaleValue)
                .FirstOrDefault();
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
