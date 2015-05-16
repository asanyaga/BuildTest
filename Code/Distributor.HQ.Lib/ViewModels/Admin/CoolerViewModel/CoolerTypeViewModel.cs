using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Distributr.HQ.Lib.ViewModels.Admin.CoolerViewModel
{
   public  class CoolerTypeViewModel
    {
       public Guid Id { get; set; }
       [Required(ErrorMessage="Name is a Required Field!")]
       public string Name { get; set; }
       [Required(ErrorMessage = "Code is a Required Field!")]
       [StringLength(15, MinimumLength = 1, ErrorMessage = "Code length should not exceed 15 charachers.")]
       public string Code { get; set; }
       public bool IsActive { get; set; }
    }
}
