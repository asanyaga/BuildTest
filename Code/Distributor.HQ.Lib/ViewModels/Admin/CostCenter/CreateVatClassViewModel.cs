using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Distributr.HQ.Lib.ViewModels.Admin.CostCenter
{
  public  class CreateVatClassViewModel
    {
      public Guid Id { get; set; }
      [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        public string VatClass { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public bool isActive { get; set; }
        public decimal Rate { get; set; }
    }
}
