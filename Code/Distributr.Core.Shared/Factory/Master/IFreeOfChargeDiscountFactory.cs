using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.ProductEntities;

namespace Distributr.Core.Factory.Master
{
   public interface IFreeOfChargeDiscountFactory
    {
       FreeOfChargeDiscount CreateFreeOfChargeDiscount(ProductRef parentProduct, ProductRef freeOfChargeProduct, int parentProductQuantity, int freeOfChargeProductQuantity,DateTime effectiveDate);
    }
}
