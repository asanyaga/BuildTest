using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Master.CostCentreEntities;

namespace Distributr.Core.Factory.Master.Impl
{
   public class CustomerDiscountFactory:ICustomerDiscountFactory
    {
        public CustomerDiscount CreateCustomerDiscount(CostCentreRef outletId, ProductRef productId, decimal discountRate,DateTime effectiveDate)
        {
            if (effectiveDate > DateTime.Now)
                throw new ArgumentException("Invalid effective date, must be in the past");
            if (productId.ProductId == Guid.Empty)
                throw new ArgumentException("Invalid product");
            if (outletId.Id == Guid.Empty)
                throw new ArgumentException("Invalid outlet");
            CustomerDiscount cDiscount = new CustomerDiscount(Guid.NewGuid())
            {
                Outlet = new CostCentreRef {Id=outletId.Id  },
                 Product= new ProductRef {ProductId=productId.ProductId},
               
            };
            cDiscount.CustomerDiscountItems.Add(new CustomerDiscount.CustomerDiscountItem(Guid.NewGuid())
            {
                 EffectiveDate=effectiveDate,
                 DiscountRate=discountRate
            });
            return cDiscount;
        }

        
    }
}
