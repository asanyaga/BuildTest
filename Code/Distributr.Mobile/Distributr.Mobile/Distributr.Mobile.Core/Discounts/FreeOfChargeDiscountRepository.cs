using System;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Utility.MasterData;
using Distributr.Mobile.Data;

namespace Distributr.Mobile.Core.Discounts
{
    public class FreeOfChargeDiscountRepository : BaseRepository<FreeOfChargeDiscount>, IFreeOfChargeDiscountRepository
    {
        private readonly Database database;

        public FreeOfChargeDiscountRepository(Database database) : base(database)
        {
            this.database = database;
        }

        public bool IsProductFreeOfCharge(Guid productId)
        {
            return database.Table<FreeOfChargeDiscount>()
                .Count(a => a.ProductRefMasterId == productId && a.StartDate <= DateTime.Now &&
                            a.EndDate >= DateTime.Now) > 0;
        }

        // 
        // Currently not used on Mobile
        //
        public QueryResult<FreeOfChargeDiscount> QueryResult(QueryFOCDiscount query)
        {
            throw new NotImplementedException();
        }
    }
}
