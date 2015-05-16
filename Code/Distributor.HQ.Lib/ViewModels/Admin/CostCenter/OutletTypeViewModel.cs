using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.HQ.Lib.Helper;
using MvcContrib.Pagination;
using System.ComponentModel.DataAnnotations;

namespace Distributr.HQ.Lib.ViewModels.Admin.CostCenter
{
   public class OutletTypeViewModel
    {
       public Guid Id { get; set; }
       [LocalizedRequired(ErrorMessage = "hq.vm.outlettype.name")]
       public string Name { get;set;}
       public bool isActive { get; set; }
       public string code { get; set; }
       public IPagination<OutletTypeViewModel> oTypePagedList { get; set; }
    }
}
