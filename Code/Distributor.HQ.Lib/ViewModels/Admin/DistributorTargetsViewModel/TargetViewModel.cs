using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.HQ.Lib.Helper;

namespace Distributr.HQ.Lib.ViewModels.Admin.DistributorTargetsViewModel
{
   public class TargetViewModel
    {
       public Guid Id { get; set; }
       public string CostCentreName { get; set; }
       public Guid CostCentreId { get; set; }
       public string PeriodName { get; set; }
       public Guid Period { get; set; }
       public string ProductName { get; set; }
       public Guid Product { get; set; }
       [LocalizedRequired(ErrorMessage = "hq.vm.targets.amount")]
       [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Please enter a numeric value with up to two decimal places.")]
       [Range(0.1, double.MaxValue, ErrorMessage = "The Target amount must be above 0 (Zero) ")]
       public decimal TargetValue { get; set; }
       public bool IsQuantityTarget { get; set; }
       public bool isActive { get; set; }
       public CostCentreType CostCentreType { get; set; }
    }
}
