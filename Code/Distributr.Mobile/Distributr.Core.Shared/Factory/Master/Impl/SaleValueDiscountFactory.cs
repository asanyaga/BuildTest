using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.ProductEntities;

namespace Distributr.Core.Factory.Master.Impl
{
  public  class SaleValueDiscountFactory:ISaleValueDiscountFactory
    {
      public SaleValueDiscount CreateSaleValueDiscount(Guid tierId, decimal rate, decimal saleValue, DateTime effectiveDate, DateTime endDate)
        {
           
            if (tierId == Guid.Empty)
                throw new ArgumentException("Invalid tier");
            SaleValueDiscount p = new SaleValueDiscount(Guid.NewGuid())
            {
               
                Tier = new ProductPricingTier(tierId),
                _Status = EntityStatus.Active,

            };
            p.DiscountItems.Add(new SaleValueDiscount.SaleValueDiscountItem(Guid.NewGuid())
            {
                 DiscountValue=rate,
                 DiscountThreshold=saleValue,
                 EffectiveDate = effectiveDate,
                 EndDate = endDate,
                 _Status = EntityStatus.New, 
            });
            return p;
        }
    }
}
