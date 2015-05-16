using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Distributr.HQ.Lib.Helper;

namespace Distributr.HQ.Lib.ViewModels.Admin.Product
{
   public class CertainValueCertainProductDiscountViewModel
    {
       public CertainValueCertainProductDiscountViewModel()
       {
           CurrentPage = 1;
           PageSize = 10;
           CVCPItems = new List<CertainValueCertainProductDiscountVM>();
       }
       public Guid id { get; set; }

       [LocalizedRequired(ErrorMessage = "hq.vm.disc.prodname")]
       public Guid Product { get; set; }
        public string ProductName { get; set; }

        [LocalizedRequired(ErrorMessage = "hq.vm.disc.initialval")]
        [Range(0.1, double.MaxValue, ErrorMessage = "The Initial Value must be above 0 (Zero).")]
        public decimal InitialValue { get; set; }

        [LocalizedRequired(ErrorMessage = "hq.vm.disc.quantity")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Please enter a numeric value.")]
        [Range(0.1, double.MaxValue, ErrorMessage = "The Quantity must be above 0 (Zero).")]
        public int Quantity { get; set; }
        public bool isActive { get; set; }
        [Required(ErrorMessage = "Discount effective date is required")]
        public DateTime EffectiveDate { get; set; }
        [Required(ErrorMessage = "Discount end date is required")]
        public DateTime EndDate { get; set; }

        public List<CertainValueCertainProductDiscountVM> CVCPItems { get; set; }
        public List<CertainValueCertainProductDiscountVM> CurrentPageItems
        {
            get
            {
                return CVCPItems.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
            }
        }

        public int PageSize { get; set; }
        public int NoPages
        {
            get
            {
                int totalpages = (int)Math.Ceiling((double)CVCPItems.Count() / (double)PageSize);
                return totalpages;
            }
        }
        public int CurrentPage { get; set; }
       public class CertainValueCertainProductDiscountVM
       {
           public Guid id { get; set; }
           public Guid Product { get; set; }
           public string ProductName { get; set; }
           public decimal Value { get; set; }
           public int Quantity { get; set; }
           public DateTime EffectiveDate { get; set; }
           public DateTime EndDate { get; set; }
           public Guid LineItemId { get; set; }
           public bool IsActive { get; set; }
       }
    }
}
