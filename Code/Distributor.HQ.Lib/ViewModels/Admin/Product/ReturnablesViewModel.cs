using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Distributr.HQ.Lib.ViewModels.Admin.Product
{
   public class ReturnablesViewModel
    {
       public int Id { get; set; }
        [Required(ErrorMessage = "Returnable Code is a Required Field!")]
        public string ReturnableCode { get; set; }
        [Required(ErrorMessage = "Returnables is a Required Field!")]
        public string Returnable { get; set; }
        public string Description { get; set; }
        [Required(ErrorMessage = "Shells is a Required Field!")]
        public int Shell { get; set; }
        public string ShellName { get; set; }
        public decimal Pricing { get; set; }
        public bool IsActive { get; set; }
        public decimal ExFactoryPrice { get; set; }
    }
}
