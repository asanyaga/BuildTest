using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.ProductEntities;

namespace Distributr.Core.Factory.Master.Impl
{
    public class ProductPricingFactory : IProductPricingFactory
    {

        public ProductPricing CreateProductPricing(Guid productId, Guid tierId, decimal exFactoryPrice, decimal sellingPrice, DateTime effectiveDate)
        {
            if (effectiveDate > DateTime.Now)
                throw new ArgumentException("Invalid effective date, must be in the past");
            if (productId == Guid.Empty)
                throw new ArgumentException("Invalid product");
            if (tierId == Guid.Empty)
                throw new ArgumentException("Invalid tier");
            ProductPricing p = new ProductPricing(Guid.NewGuid())
            {
                ProductRef = new ProductRef {ProductId = productId },
                Tier =new ProductPricingTier(tierId),                
                //tier to do
              
            };
            p.ProductPricingItems.Add(new ProductPricing.ProductPricingItem(Guid.NewGuid()) {
                EffectiveDate = effectiveDate,
                ExFactoryRate = exFactoryPrice, 
                SellingPrice = sellingPrice });
            return p;
        }

    }
}
