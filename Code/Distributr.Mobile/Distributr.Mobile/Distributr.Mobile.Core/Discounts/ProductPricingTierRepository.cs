using System;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Utility.MasterData;
using Distributr.Mobile.Data;

namespace Distributr.Mobile.Core.Discounts
{
    public class ProductPricingTierRepository : BaseRepository<ProductPricingTier>, IProductPricingTierRepository
    {
        public ProductPricingTierRepository(Database database) : base(database)
        {
        }

        // 
        // Currently not used on Mobile
        //
        public ProductPricingTier GetByCode(string tierCode)
        {
            throw new NotImplementedException();
        }

        public QueryResult<ProductPricingTier> Query(QueryStandard query)
        {
            throw new NotImplementedException();
        }
    }
}
