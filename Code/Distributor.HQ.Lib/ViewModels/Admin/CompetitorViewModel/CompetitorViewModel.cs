using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Distributr.HQ.Lib.Helper;

namespace Distributr.HQ.Lib.ViewModels.Admin.CompetitorViewModel
{
   public class CompetitorViewModel
    {
       [LocalizedRequired(ErrorMessage = "hq.vm.competitor.name")]
       [RegularExpression(@"^[a-zA-Z0-9'.\s]{1,40}$", ErrorMessage = "Special characters are not allowed")]
        public string Name { get; set; }
       //[RegularExpression(@"^[a-zA-Z0-9'.\s]{1,40}$", ErrorMessage = "Special characters are not allowed")]
        public string PhysicalAddress { get; set; }
       //[RegularExpression(@"^[a-zA-Z0-9'.\s]{1,40}$", ErrorMessage = "Special characters are not allowed")]
        public string PostalAddress { get; set; }
       [RegularExpression(@"((\(\d{3,4}\)|\d{3,4}-)\d{4,9}(-\d{1,5}|\d{0}))|(\d{4,12})", ErrorMessage = "Entered phone format is not valid.")]
        public string Telephone { get; set; }
       [RegularExpression(@"^[a-zA-Z0-9'.\s]{1,40}$", ErrorMessage = "Special characters are not allowed")]
        public string ContactPerson { get; set; }
       [RegularExpression(@"^[a-zA-Z0-9'.\s]{1,40}$", ErrorMessage = "Special characters are not allowed")]
        public string City { get; set; }
       [RegularExpression(@"^[a-zA-Z0-9'.\s]{1,40}$", ErrorMessage = "Special characters are not allowed")]
        public string Longitude { get; set; }
       [RegularExpression(@"^[a-zA-Z0-9-+\s'""]{1,40}$", ErrorMessage = "Special characters are not allowed")]
        public string Lattitude { get; set; }
      
        public bool isActive { get; set; }
        public Guid Id { get; set; }
    }
}
