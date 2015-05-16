using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Distributr.HQ.Lib.Helper;

namespace Distributr.HQ.Lib.ViewModels.Admin.BankViewModels
{
    
   public class BankViewModel
    {
       public Guid Id { get; set; }
        [LocalizedRequired(ErrorMessage = "hq.vm.bank.name")]
        public string Name { get; set; }
       [LocalizedRequired(ErrorMessage = "hq.vm.bank.code")]
        [RegularExpression(@"^[a-zA-Z0-9'.\s]{1,40}$", ErrorMessage = "Special characters are not allowed")]
        [StringLength(15, MinimumLength = 1, ErrorMessage = "Code length should not exceed 15 charachers.")]
        public string Code { get; set; }
        [LocalizedRequired(ErrorMessage = "hq.vm.bank.desc")]
        [RegularExpression(@"^[a-zA-Z0-9'.\s]{1,40}$", ErrorMessage = "Special characters are not allowed")]
        public string Description { get; set; }        
        public bool IsActive { get; set; }
    }
}
