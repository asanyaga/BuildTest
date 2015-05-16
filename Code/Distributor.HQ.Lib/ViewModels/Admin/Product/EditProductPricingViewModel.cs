using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Distributr.HQ.Lib.ViewModels.Admin.Product
{
   public class EditProductPricingViewModel
    {
       public EditProductPricingViewModel()
       {
           PItems = new List<PricingItemVM>();
       }
       public Guid Id { get; set; }
        [Required(ErrorMessage = "Product is required")]
       public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        [Required(ErrorMessage = "Tier is required")]
        public Guid TierId { get; set; }
        public string TierName { get; set; }
        public bool Active { get; set; }
        public string ErrorText { get; set; }
        public List<PricingItemVM> PItems { get; set; }

        public class PricingItemVM
        {
            [Required(ErrorMessage = "Exfactory price is required")]
            public decimal CurrentExFactory { get; set; }
            [Required(ErrorMessage = "Selling price is required")]
            public decimal CurrentSellingPrice { get; set; }
            [Required(ErrorMessage = "Effective date is required")]
            public DateTime CurrentEffectiveDate { get; set; }
            
        }
    }
}
