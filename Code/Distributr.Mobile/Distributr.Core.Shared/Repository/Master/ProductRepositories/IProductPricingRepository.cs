using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;

namespace Distributr.Core.Repository.Master.ProductRepositories
{
    public interface IProductPricingRepository : IRepositoryMaster<ProductPricing>
    {
        void AddProductPricing(Guid productPricingId, decimal exFactoryPrice, decimal sellingPrice, DateTime effectiveDate);
        ProductPricing GetByProductAndTierId(Guid productId, Guid tierId);
       //void UpdateExfactory(int productId, decimal exFactoryPrice);

        QueryResult<ProductPricing> Query(QueryStandard q);
    }
}
