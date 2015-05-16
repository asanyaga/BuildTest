using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.Domain.Master.CostCentreEntities;

namespace Distributr.HQ.Lib.ViewModels.Admin.RouteViewModel
{
    public class AdminRouteViewModel
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Root name is required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Code is required")]
        public string Code { get; set; }
        public string HubName { get; set; }
        public Guid HubId { get; set; }
        public bool isActive { get; set; }
        public Guid RegionId { get; set; }
        public string RegionName { get; set; }

    }
}
