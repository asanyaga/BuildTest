using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.HQ.Lib.Helper;

namespace Distributr.HQ.Lib.ViewModels.Admin.Discounts
{
    public class ProductDiscountViewModel
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime EndDate { get; set; }
        [Required(ErrorMessage = "Quantity is required")]
        [Range(typeof(Decimal), "0", "100")]
        [RegularExpression("^[1-9][0-9]*$",ErrorMessage = "Discount Must be Greater than 0")]
       
        public decimal DiscountRate { get; set; }
        public decimal Rate { get; set; }
        [Required(ErrorMessage = "Tier is required")]
        public Guid TierId { get; set; }
        [Required(ErrorMessage = "Product is required")]
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public string TierName { get; set; }
        [RequiredIf("IsByQuantity",1)]
        public decimal Quantity { get; set; }
        public bool IsByQuantity { get; set; }

        public class ProductDiscountItemViewModel
        {
            [Range(typeof(Decimal), "0", "100")]
            public decimal DiscountRate { get; set; }
            public DateTime EffectiveDate { get; set; }
            public DateTime EndDate { get; set; }
            public Guid ProductDiscountId { get; set; }
            public Guid LineItemId { get; set; }
            public bool IsActive { get; set; }
            public decimal Quantity { get; set; }
            public bool IsByQuantity { get; set; }
           
        }
    }
}
