using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributr.HQ.Lib.ViewModels.Agrimanagr.CostCentreViewModel
{
    public class CommoditySupplierListingViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
       
        [StringLength(15, MinimumLength = 1, ErrorMessage = "Code length should not exceed 15 charachers.")]
        public string CostCentreCode { get; set; }
        public int CommoditySupplierType { get; set; }
        [DataType(DataType.Date)]
        public DateTime JoinDate { get; set; }
        [RegularExpression(@"^[a-zA-Z0-9'.\s]{1,40}$", ErrorMessage = "Special characters are not allowed")]
        public string AccountNo { get; set; }
        public string AccountName { get; set; }
        [RegularExpression(@"^[a-zA-Z0-9'.\s]{1,40}$", ErrorMessage = "Special characters are not allowed")]
        public string PinNo { get; set; }
        public string Bank { get; set; }
        public string BankBranch { get; set; }
        [Display(Name = "Hub")]
        [Required(ErrorMessage = "Parent CostCenter is a required field")]
        public Guid ParentCostCentreId { get; set; }

        public string Description { get; set; }
        public int IsActive { get; set; }
    }
}
