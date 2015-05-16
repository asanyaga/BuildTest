using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.CostCentreEntities;
using System.ComponentModel.DataAnnotations;
using MvcContrib.Pagination;

namespace Distributr.HQ.Lib.ViewModels.Admin.CostCenter
{
    public class RegionViewModel
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Region name is required")]
        [RegularExpression(@"^[a-zA-Z0-9'.\s]{1,250}$", ErrorMessage = "Special characters are not allowed")]
        [StringLength(250,ErrorMessage = @"Name Cannot be more than 250 Characters")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Region description is required")]
        [RegularExpression(@"^[a-zA-Z0-9'.\s]{1,200}$", ErrorMessage = "Special characters are not allowed")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Country for  is required")]
        public Guid CountryId { get; set; }
        //[Required(ErrorMessage = "Province for  is required")]
        //public int ProvinceId { get; set; }
        //[Required(ErrorMessage = "District for  is required")]
        //public int DistrictId { get; set; }
        public string CountryName { get; set; }
        public bool isActive { get; set; }
        public IPagination<RegionViewModel> regionsPagedList { get; set; }
    }
}
