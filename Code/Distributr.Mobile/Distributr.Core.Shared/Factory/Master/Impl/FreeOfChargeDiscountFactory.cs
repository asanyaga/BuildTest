using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.ProductEntities;

namespace Distributr.Core.Factory.Master.Impl
{
   public class FreeOfChargeDiscountFactory:IFreeOfChargeDiscountFactory
    {
        public FreeOfChargeDiscount CreateFreeOfChargeDiscount(ProductRef parentProduct, ProductRef freeOfChargeProduct, int parentProductQuantity, int freeOfChargeProductQuantity, DateTime effectiveDate)
        {
            if (effectiveDate > DateTime.Now)
                throw new ArgumentException("Invalid effective date, must be in the past");
            if (parentProduct.ProductId == 0)
                throw new ArgumentException("Invalid product");
            if (freeOfChargeProduct.ProductId == 0)
                throw new ArgumentException("Invalid product");
            FreeOfChargeDiscount foc = new FreeOfChargeDiscount(0)
            {
                ProductRef = new ProductRef { ProductId=parentProduct.ProductId }
            };
            foc.FreeOfChargeDiscountItems.Add(new FreeOfChargeDiscount.FreeOfChargeDiscountItem(0)
         {
             ParentProductQuantity = parentProductQuantity,
             FreeOfChargeQuantity = freeOfChargeProductQuantity,
             FreeOfChargeProduct = new ProductRef { ProductId=freeOfChargeProduct.ProductId},
              EffectiveDate=effectiveDate
         });
            return foc;
        }
    }
}
