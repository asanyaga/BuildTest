using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.CostCentreEntities;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.HQ.Lib.Helper;
using MvcContrib.Pagination;

namespace Distributr.HQ.Lib.ViewModels.Admin.Product
{
   public class FreeOfChargeDiscountViewModel
    {
       public Guid Id { get; set; }
       [Required(ErrorMessage = "Product is required")]
       public Guid ProductId { get; set; }
       public bool IsActive { get; set; }
       public string ProductDescription { get; set; }
       [Required(ErrorMessage = "Discount start date is required", AllowEmptyStrings = false)]
       public string StartDate { get; set; }
       [Required(ErrorMessage = "Discount end date is required",AllowEmptyStrings = false)]
       public string EndDate { get; set; }
    }
}
