using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.ProductEntities;

namespace Distributr.Core.Factory.Master
{
    public interface IProductPricingFactory
    {
        ProductPricing CreateProductPricing(Guid productId, Guid tierId, decimal exFactoryPrice, decimal sellingPrice, DateTime effectiveDate);
        
    }
}
