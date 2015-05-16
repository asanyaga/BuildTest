using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.CostCentreEntities;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using Distributr.HQ.Lib.Helper;
using MvcContrib.Pagination;

namespace Distributr.HQ.Lib.ViewModels.Admin.Outlets
{
 public   class OutletViewModel 
    {
     public OutletViewModel()
     {
         CurrentPage = 1;
         PageSize = 15;
         Items = new List<OutletViewModelItems>();
     }
     public Guid Id { get; set; }
     [LocalizedRequired(ErrorMessage = "hq.vm.outlet.name")]
     [RegularExpression(@"^[a-zA-Z0-9'.\s]{1,40}$", ErrorMessage = "Special characters are not allowed")]
     
        public string Name { get; set; }
     [LocalizedRequired(ErrorMessage = "hq.vm.outlet.route")]
     public Guid RouteId { get; set; }
     [LocalizedRequired(ErrorMessage = "hq.vm.outlet.outletcat")]
     public Guid OutletCategoryId { get; set; }
     [LocalizedRequired(ErrorMessage = "hq.vm.outlet.outlettype")]
     public Guid OutletTypeId { get; set; }
     public Guid SurveyorId { get; set; }
     public Guid SalesRepId { get; set; }
     public Guid ASMId { get; set; }
     public string RouteName { get; set; }
     public string OutletTypeName { get; set; }
     public string OutletCategoryName { get; set; }
     public string OutletTypeCode { get; set; }
     public string OutletCategoryCode { get; set; }
     public string SurveyorName { get; set; }
     public string SalesRepName { get; set; }
     public string ASM { get; set; }
     [LocalizedRequired(ErrorMessage = "hq.vm.outlet.code")]
     //[RegularExpression(@"^[a-zA-Z0-9'.\s]{1,40}$", ErrorMessage = "Special characters are not allowed")]
      public string OutLetCode { get; set; }
     [Required]
      public Guid distributor { get; set; }
      public string DistributorName { get; set; }
      public IPagination<OutletViewModel> outletsPagedList { get; set; }
      public SelectList RouteList { get; set; }
     public SelectList OutletCategoryList { get; set; }
      public SelectList OutletTypeList { get; set; }
      public SelectList VatClassList { get; set; }
      public SelectList PricingTiersList { get; set; }
      public SelectList DiscountGroupList { get; set; }
      public SelectList Distributr { get; set; }
        public bool IsActive { get; set; }
        public Guid? DiscountGroup { get; set; }
        public string DiscountGroupName { get; set; }
        public string Code { get; set; }
    
        public Guid vatClassId { get; set; }
        public string vatClassName { get; set; }
        [Required]
        public Guid pricingTierId { get; set; }
        public string pricingTierName { get; set; }
        public string pricingTierCode { get; set; }
        public string ErrorText { get; set; }
        public DateTime DateCreated { get; set; }

        public int PageSize { get; set;  }
        public List<OutletViewModelItems> Items { get; set; }
        public List<OutletViewModelItems> CurrentPageItems
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
        public int CurrentPage { get; set;  }
        public class OutletViewModelItems
        {
            public string ErrorText { get; set; }
            public Guid Id { get; set; }
            public Guid DiscountGroup { get; set; }
            public string DiscountGroupName { get; set; }
            public string Code { get; set; }
            public Guid distributor { get; set; }
            public string DistributorName { get; set; }
            public Guid pricingTierId { get; set; }
            public string pricingTierName { get; set; }
            public bool IsActive { get; set; }
            public string Name { get; set; }
            public string OutLetCode { get; set; }
        }
     
     public class ShipToAddressItem
          {
         public Guid Id { get; set; }
              public string Name { get; set; }
              public string Description { get; set; }
              public string PostalAddress { get; set; }
              public string PhysicalAdress { get; set; }
              public decimal Longitude { get; set; }
              public decimal Latitude { get; set; }
         public bool IsActive { get; set; }
          }
     public Guid UserId { get; set; }
     [LocalizedRequired(ErrorMessage = "hq.vm.outlet.username")]
     public string Username { get; set; }
     public int Usertype { get; set; }
     public string UserCode { get; set; }
     [LocalizedRequired(ErrorMessage = "hq.vm.outlet.password")]
     public string Password { get; set; }
     public string Pin { get; set; }
     [LocalizedRequired(ErrorMessage = "hq.vm.outlet.mobile")]
     public string Mobile { get; set; }
    }
}
