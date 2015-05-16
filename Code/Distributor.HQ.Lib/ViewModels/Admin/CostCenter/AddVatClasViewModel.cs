using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.HQ.Lib.ViewModels.Admin.CostCenter
{
   public class AddVatClasViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string VatClass { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public bool isActive { get; set; }
        public decimal Rate { get; set; }
    }
}
