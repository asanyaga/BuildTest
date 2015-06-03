using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.ProductEntities;

namespace Distributr.Core.Factory.Master
{
    public interface ICertainValueCertainProductDiscountFactory
    {
        CertainValueCertainProductDiscount CreateCertainValueCertainProductDiscount(ProductRef ProductRef, int ProductQuantity, decimal CertainValue, DateTime effectiveDate, DateTime endDate);
    }
}
