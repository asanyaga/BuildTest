using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Distributr.HQ.Lib.ViewModels.Admin.MaritalStatusViewModels
{
   public class MaritalStatusViewModel
    {
       [Required(ErrorMessage = "Status is required!")]
       public string Status { get; set; }
       public Guid Id { get; set; }
       public bool isActive { get; set; }
       public string Description { get; set; }
       [Required(ErrorMessage = "Code is required!")]
       public string Code { get; set; }
       public string ErrorText { get; set; }
    }
}
