using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.HQ.Lib.ViewModels.Admin.Product
{
   public class CustomerDiscountItemsViewModel
    {
        public int id { get; set; }
        public bool isActive { get; set; }
        public int Product { get; set; }
        public decimal discountRate { get; set; }
    }
}
