using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.HQ.Lib.Helper;
using MvcContrib.Pagination;
using System.ComponentModel.DataAnnotations;

namespace Distributr.HQ.Lib.ViewModels.Admin.CostCenter
{
  public  class TerritoryViewModel
    {
      public Guid Id { get; set; }
      [LocalizedRequired(ErrorMessage = "hq.vm.territory.name")]
      [RegularExpression(@"^[a-zA-Z0-9'.\s]{1,40}$", ErrorMessage = "Special characters are not allowed")]
      public string Name { get; set; }
      public bool isActive { get; set; }
      public IPagination<TerritoryViewModel> territoryPagedList { get; set; }
    }
}
