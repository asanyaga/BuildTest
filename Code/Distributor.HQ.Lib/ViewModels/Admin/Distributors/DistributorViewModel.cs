using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.Domain.Master.CostCentreEntities;
using System.Web.Mvc;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.HQ.Lib.Helper;

namespace Distributr.HQ.Lib.ViewModels.Admin.Distributors
{
    public class DistributorViewModel
    {
        public Guid Id { get; set; }
        [LocalizedRequired(ErrorMessage = "hq.vm.dist.owner")]
        public string Owner { get; set; }
        [LocalizedRequired(ErrorMessage = "hq.vm.dist.pin")]
        public string PIN { get; set; }
        [LocalizedRequired(ErrorMessage = "hq.vm.dist.account")]
        public string AccountNo { get; set; }
        [LocalizedRequired(ErrorMessage = "hq.vm.dist.region")]
        public string  Region {get;set;}
       // [Required(ErrorMessage = "Area Sales Manager is required")]
        public Guid ASM { get; set; }
        public Guid SalesRep { get; set; }
        public Guid Surveyor { get; set; }
        public string ASMName { get; set; }
        public string SurveyorName { get; set; }
        public string SalesRepName { get; set; }
        [LocalizedRequired(ErrorMessage = "hq.vm.dist.isactive")]
        public bool IsActive { get; set; }
        [LocalizedRequired(ErrorMessage = "hq.vm.dist.name")]
        public string Name { get; set; }
        [LocalizedRequired(ErrorMessage = "hq.vm.dist.vatregno")]
        public string VatRegistrationNo { get; set; }
        [LocalizedRequired(ErrorMessage = "hq.vm.dist.region")]
        public Guid RegionId { get; set; }
        public string RegionName { get; set; }
        public Guid PricingTierId { get; set; }
        [LocalizedRequired(ErrorMessage = "hq.vm.dist.code")]
        public string CostCentreCode { get; set; }
        public DateTime DateCreated { get; set; }
        public string ErrorText { get; set; }
        [RegularExpression(@"^[a-zA-Z0-9]{6}$", ErrorMessage = "The paybill number must be six characters long.")]
        public string PayBillNumber { get; set; }
        [RegularExpression(@"^[a-zA-Z0-9]{6}$", ErrorMessage = "The Merchant Number must be six characters long.")]
        public string MerchantNumber { get; set; }
        public bool CanEditRegion { get; set; }
    }
}
