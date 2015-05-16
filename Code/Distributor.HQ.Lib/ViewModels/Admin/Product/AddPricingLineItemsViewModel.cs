using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Distributr.HQ.Lib.Helper;

namespace Distributr.HQ.Lib.ViewModels.Admin.Product
{
    public class AddPricingLineItemsViewModel
    {
        public Guid id { get; set; }
        [Required(ErrorMessage = "Exfactory price is required")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Please enter a numeric value with up to two decimal places.")]
        [Range(0.1, double.MaxValue, ErrorMessage = "The Ex-Factory price must be above 0 (Zero) ")]
        public decimal CurrentExFactory { get; set; }
        [Required(ErrorMessage = "Selling price is required")]
        //[DisplayFormat(DataFormatString = "{0:n4}", ApplyFormatInEditMode = true)]
        //[RegularExpression(@"^\d+(\.\d{1,4})?$", ErrorMessage = "Maximum of Four decimal places.")]
      //  [Range(0.1, double.MaxValue, ErrorMessage = "The Selling Price must be above 0 (Zero) ")]
        public decimal CurrentSellingPrice { get; set; }
        [Required(ErrorMessage = "Effective date is required")]
        public DateTime CurrentEffectiveDate { get; set; }
    }
}
