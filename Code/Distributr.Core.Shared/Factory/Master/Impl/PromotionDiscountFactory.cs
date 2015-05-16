using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.ProductEntities;

namespace Distributr.Core.Factory.Master.Impl
{
   public class PromotionDiscountFactory:IPromotionDiscountFactory
    {
       public PromotionDiscount CreateFreeOfChargeDiscount(ProductRef parentProduct, Guid? freeOfChargeProduct, int parentProductQuantity, int? freeProductQuantity, DateTime effectiveDate, decimal DiscountRate, DateTime endDate)
        {
            
            if (parentProduct.ProductId == Guid.Empty)
                throw new ArgumentException("Invalid product");
            //if (freeOfChargeProduct.ProductId == 0)
            //    throw new ArgumentException("Invalid product");
            PromotionDiscount foc = new PromotionDiscount(Guid.NewGuid())
            {
                ProductRef = new ProductRef { ProductId=parentProduct.ProductId }
            };
            foc.PromotionDiscountItems.Add(new PromotionDiscount.PromotionDiscountItem(Guid.NewGuid())
            {
                ParentProductQuantity = parentProductQuantity,
                FreeOfChargeQuantity = freeProductQuantity.Value,
                FreeOfChargeProduct =freeOfChargeProduct==null ?null: new ProductRef { ProductId=freeOfChargeProduct.Value},
                EffectiveDate=effectiveDate,
                EndDate = endDate,
                DiscountRate=DiscountRate,
                _Status = EntityStatus.New, 
             });
            return foc;
        }
    }
}
