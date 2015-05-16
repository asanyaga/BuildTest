using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Distributr.HQ.Lib.Helper;
using MvcContrib.Pagination;

namespace Distributr.HQ.Lib.ViewModels.Admin.Product
{
   public  class ProductPricingViewModel
    {
       public ProductPricingViewModel()
       {
           CurrentPage = 1;
           PageSize = 15;
           Items = new List<ProductPricingViewModelItems>();
       }
       public Guid Id { get; set; }
       [LocalizedRequired(ErrorMessage = "hq.vm.pricing.product")]
       public Guid ProductId { get; set; }
       public string ProductName { get; set; }
       [LocalizedRequired(ErrorMessage = "hq.vm.pricing.tier")]
       public Guid TierId { get; set; }
       public string TierName  { get; set; }
       [LocalizedRequired(ErrorMessage = "hq.vm.pricing.xfacprice")]
       [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Please enter a numeric value with up to two decimal places.")]
       public decimal CurrentExFactory { get; set; }
       [LocalizedRequired(ErrorMessage = "hq.vm.pricing.sprice")]
       [RegularExpression(@"^\d+(\.\d{1,4})?$", ErrorMessage = "Please enter a numeric value with up to Four decimal places.")]
       public decimal CurrentSellingPrice { get; set; }
       [LocalizedRequired(ErrorMessage = "hq.vm.pricing.edate")]
       public DateTime CurrentEffectiveDate { get; set; }
       public string ErrorText { get; set; }

       public bool Active { get; set; }
       public IPagination<ProductPricingViewModel> pricingPagedList { get; set; }

       public int CurrentPage { get; set; }
       public List<ProductPricingViewModelItems> Items { get; set;  }
       public List<ProductPricingViewModelItems> CurrentPageItems
       {
           get
           {
               return Items.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
           }
       }
       public int NoPages
       {
           get
           {
               int totalPages = (int)Math.Ceiling((double)Items.Count() / (double)PageSize);
               return totalPages;
           }
       }
       public int PageSize { get; set; }
       public class ProductPricingViewModelItems
       {
           public Guid Id { get; set; }
           public Guid ProductId { get; set; }
           public string ProductName { get; set; }
           public Guid TierId { get; set; }
           public string TierName { get; set; }
           public decimal CurrentExFactory { get; set; }
           public decimal CurrentSellingPrice { get; set; }
           public DateTime CurrentEffectiveDate { get; set; }
           public string ErrorText { get; set; }
           public bool Active { get; set; }
       }
    }
}
