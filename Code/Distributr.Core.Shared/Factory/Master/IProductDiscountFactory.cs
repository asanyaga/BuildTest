using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.ProductEntities;

namespace Distributr.Core.Factory.Master
{
  public  interface IProductDiscountFactory
    {
      ProductDiscount CreateProductDiscount(Guid productId, Guid tierId, decimal discountRate, DateTime effectiveDate, DateTime endDate, bool isByQuantity, decimal quantity);
    }
}
