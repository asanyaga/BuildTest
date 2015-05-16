using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Distributr.HQ.Lib.Helper;

namespace Distributr.HQ.Lib.ViewModels.Admin.SuppliersViewModel
{
   public class SupplierViewModel
   {
       public SupplierViewModel()
       {
           CurrentPage = 1;
           PageSize = 15;
           Items = new List<SupplierViewModelItem>();
       }
       public Guid Id { get; set; }
       [LocalizedRequired(ErrorMessage = "hq.vm.supplier.name")]
       public string Name { get; set; }
       //[RegularExpression(@"^[a-zA-Z0-9'.\s]{1,40}$", ErrorMessage = "Special characters are not allowed")]
       public string Code { get; set; }
       public string Description { get; set; }
       public bool isActive { get; set; }

       public List<SupplierViewModelItem> Items { get; set; }

       public List<SupplierViewModelItem> CurrentPageItems
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
       public class SupplierViewModelItem
       {
           public Guid Id { get; set; }
           public string Name { get; set; }
           public string Code { get; set; }
           public string Description { get; set; }
           public bool isActive { get; set; }
       }
    }
}
