using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.ProductEntities;

namespace Distributr.Core.Factory.Master
{
   public interface IProductDiscountGroupFactory
    {
       ProductGroupDiscount CreateProductGroupDiscount(DiscountGroup DiscountGroup, ProductRef ProductRef, decimal discountRate, DateTime effectiveDate,  DateTime endDate, bool isQuantity,decimal quantity);
    }
}
