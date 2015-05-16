using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Distributr.HQ.Lib.ViewModels.Admin.MarketAuditViewModels
{
   public class OutletAuditViewModel
    {
       public Guid Id { get; set; }
       [Required(ErrorMessage="Question is a Required Field!")]
       public string Question { get; set; }
       public bool IsActive { get; set; }

    }
}
