using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.HQ.Lib.Helper;
using Distributr.HQ.Lib.Validation;

namespace Distributr.HQ.Lib.ViewModels.Agrimanagr.CostCentreViewModel
{
    
    public class CommodityOwnerViewModel:IValidatableObject
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Farmer Code is a required field!")]
        [StringLength(15, MinimumLength = 1, ErrorMessage = "Code length should not exceed 15 charachers.")]
        public string Code { get; set; }
        [Required(ErrorMessage = "Surname is a Required Field!")]
        public string Surname { get; set; }
        [Required(ErrorMessage = "First name is a Required Field!")]
        public string FirstName { get; set; }
        [DisplayName(@"Middle Name")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "ID Number is a Required Field!")]
        [RegularExpression(@"^[a-zA-Z0-9'.\s]{1,40}$", ErrorMessage = "Special characters are not allowed")]
        public string IdNo { get; set; }
        
        [StringLength(15, MinimumLength = 1, ErrorMessage = "PIN length should not exceed 15 charachers.")]
        [Required(ErrorMessage = "PIN Number is a Required Field!")]
        public string PinNo { get; set; }
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

        //[DateOfBirth(MinAge = 14,MaxAge = 120,ErrorMessage = "Invalid Date of birth")]
        //[DateRange(Min, DateTime.Now.Year.ToString())]
        public DateTime DateOfBirth { get; set; }
        public int MaritalStatus { get; set; }
        [Required(ErrorMessage = "Commodity Owner Type is a Required Field!")]
        public Guid CommodityOwnerType { get; set; }
        [Required(ErrorMessage = "Cost Centre is a Required Field!")]
        public Guid CommoditySupplier { get; set; }
        public int IsActive { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var currentDateOfBirth = (DateTime) DateOfBirth.AddYears(18);
            if (currentDateOfBirth > DateTime.Now)
            {
                yield return new ValidationResult("Minimum age is 18 years",new[]{"DatOfBirth"});
            }
        }
    }

 
}
