using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Distributr.HQ.Lib.ViewModels.Admin.Product
{
   public class CustomerDiscountViewModel
    {
       public CustomerDiscountViewModel()
       {
           DiscountItems = new List<CustomerDiscountItemsVM>();
       }
       public Guid id { get; set; }
       public string ProductName { get; set; }
       public string OutletName { get; set; }
       public bool isActive { get; set; }
       public Guid Product { get; set; }
       public Guid ProductId { get; set; }
        [Range(typeof(Decimal), "0", "100")]
       public decimal discountRate { get; set; }
       public Guid Outlet { get; set; }
       public DateTime effectiveDate { get; set; }

       public List<CustomerDiscountItemsVM> DiscountItems { get; set; }
       public class CustomerDiscountItemsVM
       {
           public Guid id { get; set; }
           public bool isActive { get; set; }
            [Range(typeof(Decimal), "0", "100")]
           public decimal discountRate { get; set; }
           public DateTime effectiveDate { get; set; }
       }
    }
}
