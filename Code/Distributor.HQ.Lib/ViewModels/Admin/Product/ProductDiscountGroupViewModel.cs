using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Distributr.HQ.Lib.ViewModels.Admin.Product
{
    public class ProductDiscountGroupViewModel
    {
        public ProductDiscountGroupViewModel()
        {
            CurrentPage = 1;
            PageSize = 10;
            productGroupDiscountItems = new List<ProductGroupDiscountVM>();
        }
        public Guid Id { get; set; }
        public bool isActive { get; set; }
        public string ProductName { get; set; }
        public Guid Product { get; set; }
         [Range(typeof(Decimal), "0", "100")]
        public decimal discountRate { get; set; }
        public Guid DiscountGroup { get; set; }
        public string DiscountGroupName { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Quantity { get; set; }
        public bool IsByQuantity { get; set; }

        public List<ProductGroupDiscountVM> productGroupDiscountItems { get; set; }
        public List<ProductGroupDiscountVM> CurrentPageItems {
            get {
                return productGroupDiscountItems.Skip( (CurrentPage -1) * PageSize).Take(PageSize).ToList();
        }}

        public int PageSize { get; set; }
        public int NoPages { get { 
            int totalpages = (int)Math.Ceiling((double)productGroupDiscountItems.Count() / (double)PageSize);
            return totalpages;
        } }
        public int CurrentPage { get; set; }

        public class ProductGroupDiscountVM
        {
            public Guid Id { get; set; }
            public bool isActive { get; set; }
            public string ProductName { get; set; }
            public Guid Product { get; set; }
            public DateTime EffectiveDate { get; set; }
            public DateTime EndDate { get; set; }
             [Range(typeof(Decimal), "0", "100")]
            public decimal discountRate { get; set; }
        }
    }
}
