using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributr.HQ.Lib.ViewModels.Agrimanagr.CostCentreViewModel
{
    public class StoreViewModel
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Name is a required field")]
        public string Name { get; set; }
        [Display(Name = "Code")]
        [Required(ErrorMessage = "Code is a required field")]
        [StringLength(15, MinimumLength = 1, ErrorMessage = "Code length should not exceed 15 charachers.")]
        public string Code { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string VatRegistrationNo { get; set; }
        [Display(Name = "Parent CostCenter")]
        [Required(ErrorMessage = "Parent CostCenter is a required field")]
        public Guid ParentCostCentreId { get; set; }
        public string ParentCostCentreName { get; set; }
        //[Display(Name = "CostCenter Type")]
        //[Required(ErrorMessage = "CostCenter Type is a required field")]
        //public int CostCentreType { get; set; }
        public int IsActive { get; set; }
    }
}
