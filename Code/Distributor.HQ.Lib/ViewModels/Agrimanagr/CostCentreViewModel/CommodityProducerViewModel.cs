using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Distributr.Core.Domain.Master.CentreEntity;

namespace Distributr.HQ.Lib.ViewModels.Agrimanagr.CostCentreViewModel
{
    public class CommodityProducerViewModel
    {
        public Guid Id { get; set; }

        //[StringLength(15, MinimumLength = 1, ErrorMessage = "Code length should not exceed 15 charachers.")]
        [StringLength(15, MinimumLength = 1, ErrorMessage = "Code length should not exceed 15 charachers.")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Acreage is required")]
        [Display(Name = "Acreage")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Please enter a numeric value with up to two decimal places.")]
        public string Acrage { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Registration number is required")]
        [Display(Name = "Registration No.")]
        public string RegNo { get; set; }

        [Display(Name = "Physical Address")]
        public string PhysicalAddress { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "CostCentre is required")]
        [Display(Name = "Commodity supplier")]
        public Guid CommoditySupplierId { get; set; }

        public Guid HubId { get; set; }

        [Display(Name = "Centre")]
        public Guid SelectedCentreId { get; set; }

        public List<Centre> AssignedFarmCentres { get; set; }

        public SelectList UnAsignedCentresList { get; set; } 

        public int IsActive { get; set; }


    }
}
