using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.HQ.Lib.Helper;
using MvcContrib.Pagination;
using System.ComponentModel.DataAnnotations;

namespace Distributr.HQ.Lib.ViewModels.Admin.CostCenter
{
   public class VATClassViewModel
    {
       public Guid Id { get; set; }
       [LocalizedRequired(ErrorMessage = "hq.vm.vat.name")]
       public string Name { get; set; }
       [LocalizedRequired(ErrorMessage = "hq.vm.vat.vatclass")]
       public string VatClass { get; set; }
       public DateTime? EffectiveDate { get; set; }
      public bool isActive { get; set; }
       [LocalizedRequired(ErrorMessage = "hq.vm.vat.rate")]
      //[RegularExpression(@"^\d+$", ErrorMessage = "Only numbers allowed")]^\d+\.\d{0,3}$ 
       [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Please enter a numeric value with up to two decimal places.")]
       //[Range(0, double.MaxValue, ErrorMessage = "The Rate must be 0 (Zero) and above  ")]
       [Range(0, 100, ErrorMessage = "The Rate must be between 0 (Zero) and 100 ")]
       public decimal Rate { get; set; }
       public string ErrorText { get; set; }
       public IPagination<VATClassViewModel> vatClassPagedList { get; set; }
      // public decimal CurrentRate { get; set; }
    }
}
