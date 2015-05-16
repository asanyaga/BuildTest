using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.Domain.Master.CostCentreEntities;


namespace Distributr.HQ.Lib.ViewModels.Admin.CostCenter
{
    public class CostCentreViewModel
    {
        //cn
        public Guid Id { get; set; }
        public List<Distributr.Core.Domain.Master.CostCentreEntities.Contact> Contact { get; set; }
        public CostCentreRef ParentCostCentre { get; set; }
        public CostCentreType CostCentreType { get; set; }
    }
}
