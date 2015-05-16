using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Distributr.HQ.Lib.Helper;
using MvcContrib.Pagination;

namespace Distributr.HQ.Lib.ViewModels.Admin.Product
{
   public class AdminProductPackagingTypeViewModel
    {
       public Guid Id { get; set; }
       [LocalizedRequired(ErrorMessage = "hq.vm.packaging.name")]
       //[RegularExpression(@"^[a-zA-Z0-9'.\s]{1,40}$", ErrorMessage = "Special characters are not allowed")]
       public string Name { get; set; }
       [LocalizedRequired(ErrorMessage = "hq.vm.packaging.desc")]
       //[RegularExpression(@"^[a-zA-Z0-9'.\s]{1,200}$", ErrorMessage = "Special characters are not allowed")]
       public string Description { get; set; }
       public string Code { get; set; }
       public bool isActive { get; set; }
       public IPagination<AdminProductPackagingTypeViewModel> packTypePagedList { get; set; }
    }
}
