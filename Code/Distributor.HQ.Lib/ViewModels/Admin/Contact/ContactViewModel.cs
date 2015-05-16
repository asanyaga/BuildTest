using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;

namespace Distributr.HQ.Lib.ViewModels.Admin.Contact
{
   public class ContactViewModel
    {
       public Guid Id { get; set; }
       [Required(ErrorMessage="Firstname Is Required")]
       public  string Firstname{get;set;}
       public string Lastname { get; set; }
       //[DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
       [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
       public DateTime? DateofBirth { get; set; }
       [Required (ErrorMessage="Marital status is required")]
       public MaritalStatas MaritalStatusId { get; set; }
       public string MaritalStatus { get; set; }
       [Required(ErrorMessage = "Mobile is required")]
       [RegularExpression(@"((\(\d{3,4}\)|\d{3,4}-)\d{4,9}(-\d{1,5}|\d{0}))|(\d{4,12})", ErrorMessage = "Entered phone format is not valid.")]
       public string MobilePhone { get; set; }
       [RegularExpression(@"((\(\d{3,4}\)|\d{3,4}-)\d{4,9}(-\d{1,5}|\d{0}))|(\d{4,12})", ErrorMessage = "Entered phone format is not valid.")]
       public string BusinessPhone { get; set; }
       public string Fax { get; set; }
       public string PhysicalAddress { get; set; }
       [RegularExpression(@"^[a-zA-Z0-9'.\s]{1,40}$", ErrorMessage = "Special characters are not allowed")]
       public string PostalAddress { get; set; }
       public string Company { get; set; }
       public string PostalCode { get; set; }
       public string City { get; set; }
       public bool IsActive { get; set; }
       public string JobTitle { get; set; }
       public string SpouseName { get; set; }
       public string HomeTown { get; set; }
       [RegularExpression(@"((\(\d{3,4}\)|\d{3,4}-)\d{4,9}(-\d{1,5}|\d{0}))|(\d{4,12})", ErrorMessage = "Entered phone format is not valid.")]
       public string HomePhone { get; set; }
       [RegularExpression(@"((\(\d{3,4}\)|\d{3,4}-)\d{4,9}(-\d{1,5}|\d{0}))|(\d{4,12})", ErrorMessage = "Entered phone format is not valid.")]
       public string WorkExtPhone { get; set; }
       [RegularExpression(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$", ErrorMessage = "Invalid email address.")]
       public string Email { get; set; }
       public string ChildrenNames { get; set; }
       public Guid CostCentre { get; set; }
       public string CostCentreName { get; set; }
       public string ContactFor { get; set; }
       public string ErrorText { get; set; }
       public string Fullnames { get; set; }
       public Guid ContactTypeId { get; set; }
       public string ContactTypeName { get; set; }
       [Required]
       public int ContactOwner { get; set; }
       public SelectList ContactOwnerList { get; set; }
       public ContactClassification Classification { get; set; }
       public SelectList ClassificationList { get; set; }
       public SelectList ContactTypeList { get; set; }
       //public SelectList CostCenterList { get; set; }
    }
}
