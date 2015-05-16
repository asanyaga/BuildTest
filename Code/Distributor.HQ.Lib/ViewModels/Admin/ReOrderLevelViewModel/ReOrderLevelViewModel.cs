using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Distributr.HQ.Lib.Helper;

namespace Distributr.HQ.Lib.ViewModels.Admin.ReOrderLevelViewModel
{
   public class ReOrderLevelViewModel
    {
       public string DistributorName { get; set; }
       public string ProductName { get; set; }
       [LocalizedRequired(ErrorMessage = "hq.vm.reorder.dist")]
       public Guid DistributorId { get; set; }
       [LocalizedRequired(ErrorMessage = "hq.vm.reorder.prod")]
       public Guid ProductId { get; set; }
       [LocalizedRequired(ErrorMessage = "hq.vm.reorder.level")]
       [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Please enter a numeric value with up to two decimal places.")]
       [Range(0.1, double.MaxValue, ErrorMessage = "The Unit Target must be above 0 (Zero) ")]
       public decimal ProductReOrderLevel { get; set; }
       public Guid Id { get; set; }
       public bool isActive { get; set; }
    }
}
