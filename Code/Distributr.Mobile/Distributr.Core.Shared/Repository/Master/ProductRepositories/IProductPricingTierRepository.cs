using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;

namespace Distributr.Core.Repository.Master.ProductRepositories
{
    //product pricing interface for the entity PRODUCT PRICING TIER
    public interface IProductPricingTierRepository:IRepositoryMaster<ProductPricingTier>
    {
        ProductPricingTier GetByCode(string tierCode);

        QueryResult<ProductPricingTier> Query(QueryStandard query);
    }
}
