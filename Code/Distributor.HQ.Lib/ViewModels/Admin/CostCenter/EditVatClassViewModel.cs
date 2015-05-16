using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Distributr.HQ.Lib.ViewModels.Admin.CostCenter
{
   public class EditVatClassViewModel
    {
       public EditVatClassViewModel()
       {
           VCItems = new List<VatClassItemVM>();
       }
       public Guid Id { get; set; }
       [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
       [Required(ErrorMessage = "VatClass is required")]
        public string VatClass { get; set; }
        public bool isActive { get; set; }
       [Required(ErrorMessage = "Rate is required")]
        public string SRate { get; set; }
        public List<VatClassItemVM> VCItems { get; set; }

        public class VatClassItemVM 
        {
            public Guid Id { get; set; }
            public decimal Rate { get; set; }
            public DateTime EffectiveDate { get; set; }
        }

       
    }
}
