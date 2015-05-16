using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Distributr.HQ.Lib.Helper;
using MvcContrib.Pagination;

namespace Distributr.HQ.Lib.ViewModels.Admin.Product
{
    //public class ListProductBrandsViewModel
    //{
    //    public ListProductBrandsViewModel()
    //    {
    //        Brands = new List<ProductBrandViewModel>();
    //    }
    //    public string SearchTerm { get; set; }
    //    public List<ProductBrandViewModel> Brands { get; set; }
    //}

    public class ProductBrandViewModel
    {
        public ProductBrandViewModel()
        {
            CurrentPage = 1;
            PageSize = 15;
            Items = new List<ProductBrandViewModelItem>();
        }
        public Guid Id { get; set; }
        [LocalizedRequired(ErrorMessage = "hq.vm.brand.name")]
        //[RegularExpression(@"^[a-zA-Z0-9'.\s]{1,40}$",ErrorMessage="Special characters are not allowed")]
        public string Name { get; set; }
        [LocalizedRequired(ErrorMessage = "hq.vm.brand.code")]
        //[RegularExpression(@"^[a-zA-Z0-9'.\s]{1,40}$", ErrorMessage = "Special characters are not allowed")]
        public string Code { get; set; }
        //[RegularExpression(@"^[a-zA-Z0-9'.\s]{1,200}$", ErrorMessage = "Special characters are not allowed")]
        public string Description { get; set; }
        public bool isActive { get; set; }
        [LocalizedRequired(ErrorMessage = "hq.vm.brand.supplier")]
        public Guid SupplierId { get; set; }
        public string SupplierName { get; set; }
        public IPagination<ProductBrandViewModel> brandPagedList { get; set; }
        public string ErrorText { get; set; }

        public int PageSize { get; set; }
        public int NoPages 
        {
            get 
            {
                int totalPages = (int)Math.Ceiling((double)Items.Count() / (double)PageSize);
                return totalPages;
            }
        }

        public List<ProductBrandViewModelItem> CurrentPageItems
        {
            get
            {
                return Items.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
            }    
        }
        public List<ProductBrandViewModelItem> Items { get; set; }

        public int CurrentPage { get; set; }
        public class ProductBrandViewModelItem
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public string Code { get; set; }
            public string Description { get; set; }
            public bool isActive { get; set; }
            public Guid SupplierId { get; set; }
            public string SupplierName { get; set; }
            public string ErrorText { get; set; }
        }
    }
}
