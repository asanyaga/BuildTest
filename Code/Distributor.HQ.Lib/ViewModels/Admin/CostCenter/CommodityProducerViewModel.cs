using System;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.Domain.Master.CostCentreEntities;

namespace Distributr.HQ.Lib.ViewModels.Admin.CostCenter
{
    public class CommodityProducerViewModel
    {
        public Guid Id { get; set; }
        //[StringLength(15, MinimumLength = 1, ErrorMessage = "Code length should not exceed 15 charachers.")]
        [StringLength(15, MinimumLength = 1, ErrorMessage="Code length should not exceed 15 charachers.")]
        public string Code { get; set; }
        [Required(ErrorMessage = "Acreage is required")]
        public string Acrage { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Registration number is required")]
        public string RegNo { get; set; }
        [Required(ErrorMessage = "Region is required")]
        public Guid RegionId { get; set; }
        public string PhysicalAddress { get; set; }
        public string Description { get; set; }
        [Required(ErrorMessage = "CostCentre is required")]
        public string CommoditySupplierId { get; set; }

    }
}
