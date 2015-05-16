using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.HQ.Lib.ViewModels.Admin.Product
{
   public class ProductDiscountItemsViewModel
    {
       public decimal discountRate { get; set; }
       public DateTime effectiveDate { get; set; }
       public int id { get; set; }
    }
}
