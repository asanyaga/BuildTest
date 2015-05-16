using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.BankEntities;
using Distributr.Core.Domain.Master.CentreEntity;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Utility.Validation;

namespace Distributr.HQ.Lib.ViewModels.Agrimanagr.CostCentreViewModel
{
    public class CommoditySupplierViewModel
    {
        public Guid CommoditySupplierId { get; set; }
        [Required(ErrorMessage = "Account Name is a required field")]
        public string Name { get; set; }
         [Required(ErrorMessage = "Commodity Supplier Code is a required field")]
        [StringLength(15, MinimumLength = 1, ErrorMessage = "Code length should not exceed 15 charachers.")]
        public string CostCentreCode { get; set; }
        public int CommoditySupplierType { get; set; }
        [DataType(DataType.Date)]
        public DateTime JoinDate { get; set; }
        [RegularExpression(@"^[a-zA-Z0-9'.\s]{1,40}$", ErrorMessage = "Special characters are not allowed")]
        [Required(ErrorMessage = "Account Number is a required field")]
        public string AccountNo { get; set; }
        public string AccountName { get; set; }
        [RegularExpression(@"^[a-zA-Z0-9'.\s]{1,40}$", ErrorMessage = "Special characters are not allowed")]
        public string PinNo { get; set; }

        [Required(ErrorMessage = "Bank is a required Field")]
        public Guid BankId { get; set; }
        [Required(ErrorMessage = "Bank Branch is a required Field")]
        public Guid BankBranchId { get; set; }
        [Display(Name = "Hub")]
        [Required(ErrorMessage = "Parent Cost Center is a required field")]
        public Guid ParentCostCentreId { get; set; }
        public int IsActive { get; set; }

        //Commodity Owner Details
        public Guid CommodityOwnerId { get; set; }
        [StringLength(15, MinimumLength = 1, ErrorMessage = "Code length should not exceed 15 charachers.")]
        [Required(ErrorMessage = "Farmer Code is a required field")]
        [Display(Name ="Farmer Code")]
        public string OwnerCode { get; set; }

        [Display(Name = "Farmer Pin Number")]
        [Required(ErrorMessage = "Farmer Pin is a Required Field!")]
        public string OwnerPinNo { get; set; }
        [Required(ErrorMessage = "Surname is a Required Field!")]
        public string Surname { get; set; }
        [Required(ErrorMessage = "First name is a Required Field!")]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Required(ErrorMessage = "ID Number is a Required Field!")]
        [RegularExpression(@"^[a-zA-Z0-9'.\s]{1,40}$", ErrorMessage = "Special characters are not allowed")]
        public string IdNo { get; set; }
       

        [Required(ErrorMessage = "Gender is a Required Field!")]
        public int Gender { get; set; }
        public string PhysicalAddress { get; set; }
        public string PostalAddress { get; set; }
        [RegularExpression(@"[a-zA-Z0-9_\-\.]+@[a-zA-Z0-9_\-\.]+\.[a-zA-Z]{2,5}", ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Phone number is a Required Field!")]
        [RegularExpression(@"((\(\d{3,4}\)|\d{3,4}-)\d{4,9}(-\d{1,5}|\d{0}))|(\d{4,12})", ErrorMessage = "Entered phone format is not valid.")]
        [StringLength(25, MinimumLength = 1, ErrorMessage = "Phone number too long.")]
        public string PhoneNumber { get; set; }
        public string BusinessNumber { get; set; }
        public string FaxNumber { get; set; }
        public string OfficeNumber { get; set; }
        public string Description { get; set; }

        [Required]
        [DateOfBirth(MinAge = 14, MaxAge = 120, ErrorMessage = "Invalid Date of birth")]
        // [Range(DateTime.Now.AddYears(-10).Year,DateTime.Now.Year)]
        public DateTime DateOfBirth { get; set; }
        public int MaritalStatus { get; set; }
        [Required(ErrorMessage = "Commodity Owner Type is a Required Field!")]
        public Guid CommodityOwnerType { get; set; }
        
        //Commodity Producer Details
        public Guid CommodityProducerId { get; set; }
         [Required(ErrorMessage = "Farmer Code is a required field")]
        [Display(Name="Farm Code")]
        public string FarmCode { get; set; }

        [Required(ErrorMessage = "Acreage is required")]
        public string Acrage { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string FarmName { get; set; }

        [Required(ErrorMessage = "Registration number is required")]
        public string RegNo { get; set; }

        public string FarmPhysicalAddress { get; set; }

        public string  FarmDescription { get; set; }

        [Required(ErrorMessage = "CostCentre is required")]
        [Display(Name = "Commodity supplier")]

        public Guid HubId { get; set; }

        [Required(ErrorMessage = "Must assign at least one center")]
        [Display(Name = "Center")]
        public Guid SelectedCentreId { get; set; }

        public List<Centre> AssignedFarmCentres { get; set; }

        public SelectList UnAsignedCentresList { get; set; } 

    }
}
