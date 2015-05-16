using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Distributr.HQ.Lib.ViewModels.Admin.CostCenter
{
    public class SocioEconomicStatusViewModel
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Status is required.")]
        public string Status { get; set; }
        public bool isActive { get; set; }
    }
}
