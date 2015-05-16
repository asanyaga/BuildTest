using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Distributr.HQ.Lib.ViewModels.Admin.Product
{
   public class PromotionDiscountViewModel
    {
       public PromotionDiscountViewModel()
       {
           CurrentPage = 1;
           PageSize = 10;
           freeOfChargeDiscountItems = new List<FreeOfChargeDiscountVM>();
       }
       public Guid Id { get; set; }
       public bool isActive { get; set; }
       public string ProductName { get; set; }
       public Guid Product { get; set; }
       public int ParentProductQuantity { get; set; }
       public int? FreeOfChargeProductQuantity { get; set; }
       public Guid? FreeProduct { get; set; }
       public string FreeProductName { get; set; }
       public DateTime EffectiveDate { get; set; }
       public DateTime EndDate { get; set; }

        [Range(typeof(Decimal), "0", "100")]
       public decimal DiscountRate { get; set; }

       public List<FreeOfChargeDiscountVM> freeOfChargeDiscountItems { get; set; }
       public List<FreeOfChargeDiscountVM> CurrentPageItems
       {
           get
           {
               return freeOfChargeDiscountItems.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
           }
       }

       public int PageSize { get; set; }
       public int NoPages
       {
           get
           {
               int totalpages = (int)Math.Ceiling((double)freeOfChargeDiscountItems.Count() / (double)PageSize);
               return totalpages;
           }
       }


       public int CurrentPage { get; set; }
           public class FreeOfChargeDiscountVM
           {
               public Guid id { get; set; }
               public bool isActive { get; set; }
               public int ParentProductQuantity { get; set; }
               public int FreeOfChargeProductQuantity { get; set; }
               public Guid FreeProduct { get; set; }
               public string FreeProductName { get; set; }
               public DateTime effectiveDate { get; set; }
               public DateTime endDate { get; set; }
                [Range(typeof(Decimal), "0", "100")]
               public decimal DiscountRate { get; set; }

                public Guid LineItemId { get; set; }
                public bool IsActive { get; set; }
           }
    }
}
