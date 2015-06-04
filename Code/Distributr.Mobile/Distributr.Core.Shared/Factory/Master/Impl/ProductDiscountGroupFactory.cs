using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.ProductEntities;

namespace Distributr.Core.Factory.Master.Impl
{
   public class ProductDiscountGroupFactory:IProductDiscountGroupFactory
    {
        public ProductGroupDiscount CreateProductGroupDiscount(DiscountGroup DiscountGroup,ProductRef ProductRef, decimal discountRate, DateTime effectiveDate, DateTime endDate, bool isQuantity,decimal quantity)
       {
           
           if (ProductRef.ProductId == Guid.Empty)
               throw new ArgumentException("Invalid product");
           if (DiscountGroup.Id == Guid.Empty)
               throw new ArgumentException("Invalid outlet");
           ProductGroupDiscount pgd = new ProductGroupDiscount(Guid.NewGuid())
           {
               GroupDiscount = new DiscountGroup (DiscountGroup.Id)
           };

            pgd.Product = new ProductRef {ProductId = ProductRef.ProductId};
            pgd.DiscountRate = discountRate;
            pgd.EffectiveDate = effectiveDate;
            pgd.EndDate = endDate;
            pgd.IsByQuantity = isQuantity;
            pgd.Quantity = quantity;
            pgd._Status = EntityStatus.New;
       
           return pgd;
        }
    }
}
