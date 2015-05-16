using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.CostCentreEntities;

namespace Distributr.HQ.Lib.ViewModels.Admin.CostCenter
{
   public class AreaViewModel
    {
       public Guid Id { get; set; }
       public string Name { get; set; }
       public string Description { get; set; }
      // public Region Region { get; set; }
       public Guid RegionId { get; set; }
       public bool isActive { get; set; }
    }
}
