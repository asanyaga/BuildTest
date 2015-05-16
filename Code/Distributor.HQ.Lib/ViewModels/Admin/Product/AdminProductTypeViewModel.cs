using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Distributr.HQ.Lib.Helper;
using MvcContrib.Pagination;

namespace Distributr.HQ.Lib.ViewModels.Admin.Product
{
    public class AdminProductTypeViewModel
    {
        public AdminProductTypeViewModel()
       {
           CurrentPage = 1;
           PageSize = 10;
           Items = new List<AdminProductTypeViewModelItem>();
       }
        public Guid Id { get; set; }
        [LocalizedRequired(ErrorMessage = "hq.vm.product.typename")]
        //[RegularExpression(@"^[a-zA-Z0-9'.\s]{1,40}$", ErrorMessage = "Special characters are not allowed")]
        public string Name { get; set; }
        //[RegularExpression(@"^[a-zA-Z0-9'.\s]{1,200}$", ErrorMessage = "Special characters are not allowed")]
        public string Description { get; set; }
        public string Code { get; set; }
        public bool isActive { get; set; }
        public IPagination<AdminProductTypeViewModel> prodTypePagedList { get; set; }
        public List<AdminProductTypeViewModelItem> Items { get; set; }

        public List<AdminProductTypeViewModelItem> CurrentPageItems
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
        public int CurrentPage { get; set; }
        public class AdminProductTypeViewModelItem
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public string Code { get; set; }
            public string Description { get; set; }
            public bool isActive { get; set; }
        }
    }
}
