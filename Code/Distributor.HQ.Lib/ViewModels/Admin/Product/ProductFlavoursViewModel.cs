using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.HQ.Lib.Helper;
using MvcContrib.Pagination;
using System.ComponentModel.DataAnnotations;

namespace Distributr.HQ.Lib.ViewModels.Admin.Product
{
    public class ProductFlavoursViewModel
    {
        public Guid Id { get; set; }
        [LocalizedRequired(ErrorMessage = "hq.vm.subbrand.code")]
        //[RegularExpression(@"^[a-zA-Z0-9'.\s]{1,50}$", ErrorMessage = "Special characters are not allowed")]
        [StringLength(49)]
        public string Code { get; set; }
        [LocalizedRequired(ErrorMessage = "hq.vm.subbrand.name")]
        //[RegularExpression(@"^[a-zA-Z0-9'.\s]{1,40}$", ErrorMessage = "Special characters are not allowed")]
        public string Name { get; set; }
        //[RegularExpression(@"^[a-zA-Z0-9'.\s]{1,200}$", ErrorMessage = "Special characters are not allowed")]
        [StringLength(200)]
        public string Description { get; set; }
        public bool isActive { get; set; }
        public Guid BrandId { get; set; }
        public string BrandName { get; set; }
        public string BrandCode { get; set; }
        public IPagination<ProductFlavoursViewModel> flavourPagedList { get; set; }
    }
}
