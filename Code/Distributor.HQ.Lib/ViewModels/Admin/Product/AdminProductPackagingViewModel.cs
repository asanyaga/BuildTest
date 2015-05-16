using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Distributr.HQ.Lib.Helper;
using MvcContrib.Pagination;

namespace Distributr.HQ.Lib.ViewModels.Admin.Product
{
    public class AdminProductPackagingViewModel
    {
        public Guid Id { get; set; }

        [LocalizedRequired(ErrorMessage = "hq.vm.packaging.pacname")]
        //[RegularExpression(@"^[a-zA-Z0-9'.\s]{1,40}$", ErrorMessage = "Special characters are not allowed")]
        public string Name { get; set; }
        [LocalizedRequired(ErrorMessage = "hq.vm.packaging.pacdesc")]
        //[RegularExpression(@"^[a-zA-Z0-9'.\s]{1,200}$", ErrorMessage = "Special characters are not allowed")]
        public string Description { get; set; }
        public bool isActive { get; set; }
        public string Code { get; set; }
        public IPagination<AdminProductPackagingViewModel> packagingPagedList { get; set; }
    }
}
