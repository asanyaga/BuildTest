using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MvcContrib.Pagination;
using System.ComponentModel.DataAnnotations;

namespace Distributr.HQ.Lib.ViewModels.Admin.CostCenter
{
   public class ListVatClassViewModel
    {
       public Guid Id { get; set; }
        public string Name { get; set; }
        public string VatClass { get; set; }
        public DateTime EffectiveDate { get; set; }
        public bool isActive { get; set; }
       [Required(ErrorMessage="Rate Is Required")]
        public string SRate { get; set; }
        public decimal Rate { get; set; }
        public string ErrorText { get; set; }
        public IPagination<ListVatClassViewModel> vatClassPagedList { get; set; }
    }
}
