using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Distributr.HQ.Lib.Helper;
using MvcContrib.Pagination;

namespace Distributr.HQ.Lib.ViewModels.Admin.Product
{
    public class ProductPricingTierViewModel
    {
        public Guid Id { get; set; }
        [LocalizedRequired(ErrorMessage = "hq.vm.pricing.name")]
        //[RegularExpression(@"^[a-zA-Z0-9'.\s]{1,40}$", ErrorMessage = "Special characters are not allowed")]
        public string Name { get; set; }
        [LocalizedRequired(ErrorMessage = "hq.vm.pricing.code")]
        //[RegularExpression(@"^[a-zA-Z0-9'.\s]{1,40}$", ErrorMessage = "Special characters are not allowed")]
        public string TierCode { get; set; }
        [LocalizedRequired(ErrorMessage = "hq.vm.pricing.desc")]
        //[RegularExpression(@"^[a-zA-Z0-9'.\s]{1,40}$", ErrorMessage = "Special characters are not allowed")]
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public IPagination<ProductPricingTierViewModel> pricingTierPagedList { get; set; }
    }
}
