using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master.CostCentreEntities;

namespace Distributr.HQ.Lib.ViewModels.Agrimanagr.CostCentreViewModel
{
    public class HubViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Name is a required field")]
        public string Name { get; set; }

        [Display(Name = "Code")]
        [StringLength(15, MinimumLength = 1, ErrorMessage = "Code length should not exceed 15 charachers.")]
        [Required(ErrorMessage = "Code is a required field")]
        public string CostCentreCode { get; set; }

        [Display(Name = "Region")]
        [Required(ErrorMessage = "Region is a required field")]
        public Guid RegionId { get; set; }

        public string RegionName { get; set; }

        [Display(Name = "VAT Registration No.")]
        [Required(ErrorMessage = "VAT Registration No. is a required field")]
        public string VatRegistrationNo { get; set; }

        public string Longitude { get; set; }
        public string Latitude { get; set; }

        [Display(Name = "Parent Costcentre")]
        [Required(ErrorMessage = "Parent CostCenter is a required field")]
        public Guid ParentCostCentreId { get; set; }

        public int IsActive { get; set; }

        public bool CanEditHubRegion { get; set; }

    }
}
