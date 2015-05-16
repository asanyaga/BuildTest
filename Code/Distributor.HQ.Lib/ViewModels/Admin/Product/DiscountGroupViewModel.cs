using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Distributr.HQ.Lib.Helper;

namespace Distributr.HQ.Lib.ViewModels.Admin.Product
{
   public class DiscountGroupViewModel
    {
       public Guid id { get; set; }
       public bool isActive { get; set; }
       [LocalizedRequired(ErrorMessage = "hq.vm.disc.grpname")]
       //[RegularExpression(@"^[a-zA-Z0-9'.\s]{1,410}$", ErrorMessage = "Special characters are not allowed")]
       [StringLength(40, ErrorMessage = "Name cannot exceed 40 characters. ")]
       public string Name { get; set; }
       [LocalizedRequired(ErrorMessage = "hq.vm.disc.grpcode")]
       //[RegularExpression(@"^[a-zA-Z0-9'.\s]{1,410}$", ErrorMessage = "Special characters are not allowed")]
       [StringLength(20, ErrorMessage = "Code cannot exceed 20 characters. ")]
       public string Code { get; set; }
    }
}
