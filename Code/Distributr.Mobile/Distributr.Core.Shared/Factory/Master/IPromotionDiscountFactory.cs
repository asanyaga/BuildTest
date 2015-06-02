using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.ProductEntities;

namespace Distributr.Core.Factory.Master
{
   public interface IPromotionDiscountFactory
    {
       PromotionDiscount CreateFreeOfChargeDiscount(ProductRef parentProduct, Guid? freeOfChargeProduct, int parentProductQuantity, int? freeProductQuantity, DateTime effectiveDate, decimal DiscountRate, DateTime endDate);
    }
}
