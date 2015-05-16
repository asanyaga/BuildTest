using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Distributr.HQ.Lib.Helper;
using MvcContrib.Pagination;

namespace Distributr.HQ.Lib.ViewModels.Admin.CostCenter
{
    public class OutletCategoryViewModel
    {
        [Required]
        public Guid Id { get; set; }

        [LocalizedRequired(ErrorMessage = "Outlet name is required")]
        [RegularExpression(@"^[a-zA-Z0-9'.\s]{1,40}$", ErrorMessage = "Special characters are not allowed")]
        public string Name { get; set; }
        public string code { get; set; }
        public bool isActive { get; set; }
        public IPagination<OutletCategoryViewModel> oCategoryPagedList { get; set; }
    }
}
