using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Distributr.HQ.Lib.Helper;

namespace Distributr.HQ.Lib.ViewModels.Admin.DistributorTargetsViewModel
{
  public class TargetPeriodViewModel
    {
      public Guid Id { get; set; }
      [LocalizedRequired(ErrorMessage = "hq.vm.targets.name")]
      public string Name { get; set; }

      public DateTime StartDate { get; set; }
      public DateTime EndDate { get; set; }
      public bool IsActive { get; set; }
    }
}
