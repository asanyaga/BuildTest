using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Distributr.HQ.Lib.ViewModels.Admin.CostCenter
{
  public  class VatClassLineItemViewModel
    {
      public Guid id { get; set; }
      [Required(ErrorMessage="Rate is required")]
      [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Please enter a numeric value with up to two decimal places.")]
      [Range(0.01, 1000000, ErrorMessage = "The Rate must be above 0 (Zero) ")]
      public decimal Rate { get; set; }
      [Required(ErrorMessage = "Effective Date is required")]
      public DateTime? effectiveDate { get; set; }
    }
}
