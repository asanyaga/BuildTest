using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Distributr.HQ.Lib.ViewModels.Admin.Product
{
   public class SaleValueDiscountViewModel
    {
       public SaleValueDiscountViewModel()
       {
           CurrentPage = 1;
           PageSize = 10;
           SaleValueDiscountItems = new List<SaleValueDiscountItemsVM>();
       }
       public Guid Id { get; set; }
       public bool isActive { get; set; }
       public decimal SaleValue { get; set; }
       [Range(typeof(Decimal), "0", "100")]
       public decimal Rate { get; set; }
       public Guid TierId { get; set; }
       public string TierName { get; set; }
       public DateTime EffectiveDate { get; set; }
       public DateTime EndDate { get; set; }

       public List<SaleValueDiscountItemsVM> SaleValueDiscountItems { get; set; }
       public List<SaleValueDiscountItemsVM> CurrentPageItems
       {
           get
           {
               return SaleValueDiscountItems.ToList();
           }
       }

       public int PageSize { get; set; }
       public int NoPages
       {
           get
           {
               int totalpages = (int)Math.Ceiling((double)SaleValueDiscountItems.Count() / (double)PageSize);
               return totalpages;
           }
       }
       public int CurrentPage { get; set; }
       public class SaleValueDiscountItemsVM
       {
           [Range(typeof(Decimal), "0", "100")]
           public decimal Rate { get; set; }
           public decimal SaleValue { get; set; }
           public DateTime effectiveDate { get; set; }
           public DateTime endDate { get; set; }
           public Guid id { get; set; }
           public bool isActive { get; set; }
           public Guid LineItemId { get; set; }
           public bool IsActive { get; set; }
       }
    }
}
