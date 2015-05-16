using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.Domain.Master.UserEntities;
using System.Web.Mvc;

namespace Distributr.HQ.Lib.ViewModels.Admin.CostCenter
{
   public class OutletSalesmanViewModel
    {
       public int Id { get; set; }
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Cost Center is required")]
        public int CostCentre { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        public string PIN { get; set; }

        [Required(ErrorMessage = "User type is required")]
        public UserType UserType { get; set; }

        public string Mobile { get; set; }
        public SelectList UserTypeList { get; set; }
        public bool isActive { get; set; }
    }
}
