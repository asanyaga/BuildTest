using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.ProductEntities;

namespace Distributr.Core.Factory.Master.Impl
{
    public class ProductDiscountFactory : IProductDiscountFactory
    {
        public ProductDiscount CreateProductDiscount(Guid productId, Guid tierId, decimal discountRate,
                                                     DateTime effectiveDate, DateTime endDate,bool isByQuantity,decimal quantity)
        {
           
            if (productId == Guid.Empty)
                throw new ArgumentException("Invalid product");
            if (tierId == Guid.Empty)
                throw new ArgumentException("Invalid tier");
            ProductDiscount p = new ProductDiscount(Guid.NewGuid())
                                    {
                                        ProductRef = new ProductRef {ProductId = productId},
                                        Tier = new ProductPricingTier(tierId),
                                    };
            p.DiscountItems.Add(new ProductDiscount.ProductDiscountItem(Guid.NewGuid())
                                    {
                                        EffectiveDate = effectiveDate,
                                        EndDate = endDate,
                                        DiscountRate = discountRate,
                                        _Status = EntityStatus.New,
                                        IsByQuantity = isByQuantity,
                                        Quantity = quantity,
                                    });
            return p;
        }
    }
}
