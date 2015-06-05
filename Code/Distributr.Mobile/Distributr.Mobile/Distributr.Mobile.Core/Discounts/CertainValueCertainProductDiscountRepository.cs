using System;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Utility.MasterData;
using Distributr.Mobile.Data;

namespace Distributr.Mobile.Core.Discounts
{
    public class CertainValueCertainProductDiscountRepository : BaseRepository<CertainValueCertainProductDiscount>, ICertainValueCertainProductDiscountRepository
    {
        private readonly Database database;

        public CertainValueCertainProductDiscountRepository(Database database) : base(database)
        {
            this.database = database;
        }

        public CertainValueCertainProductDiscount GetByAmount(decimal amount)
        {
            return database.Table<CertainValueCertainProductDiscount>()
                .Where(
                    c =>
                        amount >= c.InitialValue && c.CurrentEffectiveDate.Date <= DateTime.Now.Date &&
                        c.CurrentEndDate >= DateTime.Now.Date)
                .OrderByDescending(c => c.InitialValue)
                .FirstOrDefault();
        }

        // 
        // Currently not used on Mobile
        //
        public void AddCertainValueCertainProductDiscount(Guid cvcpId, int ProductQuantity, ProductRef ProductRef, decimal CertainValue,
            DateTime effectiveDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        public void EditCertainValueCertainProductDiscount(Guid cvcpId, Guid lineItemId, int productQuantity, ProductRef productRef,
            decimal certainValue, DateTime effectiveDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        public CertainValueCertainProductDiscount GetByAmountAndProduct(decimal Amount, Guid productId)
        {
            throw new NotImplementedException();
        }

        public void DeactivateLineItem(Guid lineItemId)
        {
            throw new NotImplementedException();
        }

        public CertainValueCertainProductDiscount GetByInitialValue(decimal value)
        {
            throw new NotImplementedException();
        }

        public QueryResult<CertainValueCertainProductDiscount> Query(QueryStandard query)
        {
            throw new NotImplementedException();
        }
    }
}
