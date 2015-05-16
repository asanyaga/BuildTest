using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Distributr.HQ.Lib.ViewModels.Admin.Product
{
   public class ShellsViewModel
    {
       public int Id { get; set; }
        [Required(ErrorMessage = "Shell Code is a Required Field!")]
        public string ShellCode { get; set; }
        [Required(ErrorMessage = "BottlesPerShell is a Required Field!")]
        public int BottlesPerShell { get; set; }
        [Required(ErrorMessage = "Shell Price is a Required Field!")]
        public decimal ShellPrice { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}
