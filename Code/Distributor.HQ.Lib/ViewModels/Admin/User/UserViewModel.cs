using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Distributr.HQ.Lib.Helper;

namespace Distributr.HQ.Lib.ViewModels.Admin.User
{
    public class UserViewModel
    {
        public UserViewModel()
        {
            CurrentPage = 1;
            PageSize = 15;
            Items = new List<UserViewModelItem>();

        }
        
        public Guid Id { get; set; }
        
        public DateTime DateCreated { get; set; }
        
        [LocalizedRequired(ErrorMessage = "hq.vm.user.username")]
        public string Username { get; set; }
        
        [LocalizedRequired(ErrorMessage = "hq.vm.user.costcentre")]
        public Guid CostCentre { get; set; }

        [Display(Name="Company")]
        public CostCentre SelectedCompany { get; set; }
        
        public string CostCentreName { get; set; }
        
        [LocalizedRequired(ErrorMessage = "hq.vm.user.password")]
        //[StringLength(15, MinimumLength = 6)]
        //[RegularExpression(@"(\S)+", ErrorMessage = "White space is not allowed")]
        //[RegularExpression("[\\S]{6,}", ErrorMessage = "Password Must be at least 6 characters.")]
        [RegularExpression(@"(?!^[0-9]*$)(?!^[a-zA-Z]*$)^([a-zA-Z0-9]{6,20})$", ErrorMessage = "Password Must contain letters and numbers")]
        public string Password { get; set; }
       
        public string PIN { get; set; }
        
        [LocalizedRequired(ErrorMessage = "hq.vm.user.usertype")]
        public UserType UserType { get; set; }

        [RegularExpression(@"((\(\d{3,4}\)|\d{3,4}-)\d{4,9}(-\d{1,5}|\d{0}))|(\d{4,12})", ErrorMessage = "Entered phone format is not valid.")]
        public string Mobile { get; set; }
        
        public SelectList UserTypeList { get; set; }
        
        public bool isActive { get; set; }
        
        // public List<UserRole> userRole { get; set; }
        
        public Guid Group { get; set; }
        
        public string CostCentreCode { get; set; }

        public Guid SupplierId { get; set; }
        
        public string ErrorText { get; set; }
        
        public Guid UserCostCentreId { get; set; }
        
        public string Name { get; set; }
        
        public string TillNumber { get; set; }
        
        //Import
        public string GroupName { get; set; }
        
        public string userTypeName { get; set; }
        
        public SelectList UserList { get; set; }
        
        public SelectList CostCentreList { get; set; }

        public List<UserViewModelItem> Items { get; set; }
        
        public List<UserViewModelItem> CurrentPageItems
        {
            get
            {
                return Items.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
            }
        }

        public int PageSize { get; set; }
        public int NoPages
        {
            get
            {
                int totalpages = (int)Math.Ceiling((double)Items.Count() / (double)PageSize);
                return totalpages;
            }
        }

        public int CurrentPage { get; set; }

        public string SupplierName { get; set; }
        public class UserViewModelItem
        {
            public Guid Id { get; set; }
            public string Username { get; set; }
            public UserType UserType { get; set; }
            public bool isActive { get; set; }
            public Guid CostCentre { get; set; }
            public string CostCentreName { get; set; }
            public string CostCentreCode { get; set; }
            public string PIN { get; set; }
            public string Mobile { get; set; }
            public string TillNumber { get; set; }
        }

    }
}
