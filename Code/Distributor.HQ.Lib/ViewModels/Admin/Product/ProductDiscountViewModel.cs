using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Distributr.HQ.Lib.Helper;

namespace Distributr.HQ.Lib.ViewModels.Admin.Product
{
   public class ProductDiscountViewModel
    {
       public ProductDiscountViewModel()
       {
           CurrentPage = 1;
           PageSize = 10;
           DiscountItemsList=new List<DiscountItemsVM> ();
       }

       public Guid Id { get; set; }

       public bool isActive { get; set; }

       public DateTime EffectiveDate { get; set; }

       public DateTime EndDate { get; set; }

       [Range(typeof(Decimal), "0", "100")]
       public decimal DiscountRate { get; set; }

       [LocalizedRequired(ErrorMessage = "hq.vm.disc.tier")]
       public Guid TierId { get; set; }

       [LocalizedRequired(ErrorMessage = "hq.vm.disc.product")]
       public Guid ProductId { get; set; }

       public string ProductName { get; set; }

       public string TierName { get; set; }

       public List<DiscountItemsVM> DiscountItemsList {get;set;}

       public List<DiscountItemsVM> CurrentPageItems
       {
           get
           {
               return DiscountItemsList.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
           }
       }

       public int PageSize { get; set; }

       public int NoPages
       {
           get
           {
               int totalpages = (int)Math.Ceiling((double)DiscountItemsList.Count() / (double)PageSize);
               return totalpages;
           }
       }
       public int CurrentPage { get; set; }

       public class DiscountItemsVM
       {
           [Range(typeof(Decimal), "0", "100")]
           public decimal discountRate {get;set;}

           public DateTime effectiveDate{get;set;}
       }
    }
}
