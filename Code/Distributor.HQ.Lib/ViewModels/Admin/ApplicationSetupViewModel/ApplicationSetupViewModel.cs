using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core;
using Distributr.Core.Domain.Master.UserEntities;

namespace Distributr.HQ.Lib.ViewModels.Admin.ApplicationSetupViewModel
{
    public class ApplicationSetupViewModel
    {
        public Guid CompanyId { get; set; }
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }
        public Guid AdminId { get; set; }
        [Display(Name="Super Admin")]
        [Required(ErrorMessage="Admin name is required.")]
        public string AdminName { get; set; }

        [Display(Name="Password")]
        [Required(ErrorMessage = "Admin password is required.")]
        [ValidatePasswordLength]
        [RegularExpression("[\\S]{6,}", ErrorMessage = "Password Must be at least 6 characters.")]
        [DataType(DataType.Password)]
        public string AdminPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.Web.Mvc.Compare("AdminPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Mobile phone number")]
        [Required(ErrorMessage = "Mobile Phone Number is required.")]
        public string Mobile { get; set; }

        [Display(Name = "PIN Number")]
        [Required(ErrorMessage = "PIN number is required.")]
        public string Pin { get; set; }
        
        [Required]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$", ErrorMessage = "Invalid email address.")]
        [Display(Name = "Email address")]
        public string Email { get; set; }
        public UserType userType { get; set; }
        public bool DatabaseExists { get; set; }
        
        [Required(ErrorMessage = "Select path for the database.")]
        public string DatabaseScriptPath { get; set; }
        
        [Display(Name = "Database Name")]
        public string DatabaseName { get; set; }
        
        [Display(Name = "Database Server")]
        public string DatabaseServer { get; set; }
        public bool CompanyIsSetup { get; set; }
    }
}
