using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Master.CostCentreEntities;

namespace Distributr.Core.Factory.Master
{
  public  interface ICustomerDiscountFactory
    {
      /// <summary>
      /// 
      /// </summary>
      /// <param name="outletId"></param>
      /// <param name="productId"></param>
      /// <param name="discountRate"></param>
      /// <returns></returns>
      CustomerDiscount CreateCustomerDiscount(CostCentreRef outletId, ProductRef productId, decimal discountRate,DateTime effectiveDate);
    }
}
