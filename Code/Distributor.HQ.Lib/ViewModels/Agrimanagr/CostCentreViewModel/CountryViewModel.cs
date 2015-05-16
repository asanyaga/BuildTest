using System;
using System.ComponentModel.DataAnnotations;
using Distributr.HQ.Lib.Helper;
using MvcContrib.Pagination;

namespace Distributr.HQ.Lib.ViewModels.Agrimanagr.CostCentreViewModel
{
   public class CountryViewModel
    {
       public Guid id { get; set; }
       [LocalizedRequired(ErrorMessage = "hq.vm.country.name")]
       [RegularExpression(@"^[a-zA-Z0-9'.\s]{1,40}$", ErrorMessage = "Special characters are not allowed")]
       public string Name { get; set; }
        [LocalizedRequired(ErrorMessage = "hq.vm.country.code")]
        [RegularExpression(@"^[a-zA-Z0-9'.\s]{1,40}$", ErrorMessage = "Special characters are not allowed")]
       [StringLength(15, MinimumLength = 1, ErrorMessage = "Code length should not exceed 15 charachers.")]
       public string Code { get; set; }
       public string Currency { get; set; }
       public bool isActive { get; set; }
       
    }
}
