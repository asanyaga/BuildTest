using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.ProductEntities;

namespace Distributr.Core.Factory.Master
{
   public interface ISaleValueDiscountFactory
    {
       SaleValueDiscount CreateSaleValueDiscount(Guid tierId, decimal rate, decimal saleValue, DateTime effectiveDate, DateTime endDate);
    }
}
