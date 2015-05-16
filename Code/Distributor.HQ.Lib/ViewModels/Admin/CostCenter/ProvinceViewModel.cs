using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.CostCentreEntities;
using System.ComponentModel.DataAnnotations;
using Distributr.HQ.Lib.Helper;

namespace Distributr.HQ.Lib.ViewModels.Admin.CostCenter
{
   public class ProvinceViewModel
    {
       public Guid Id { get; set; }
       [LocalizedRequired(ErrorMessage = "hq.vm.province.name")]
       [RegularExpression(@"^[a-zA-Z0-9'.\s]{1,40}$", ErrorMessage = "Special characters are not allowed")]
        public string Name { get; set; }
       [RegularExpression(@"^[a-zA-Z0-9'.\s]{1,40}$", ErrorMessage = "Special characters are not allowed")]
        public string Description { get; set; }
        public string CountryName { get; set; }
       [LocalizedRequired(ErrorMessage = "hq.vm.province.code")]
        public Guid CountryId { get; set; }
        public bool IsActive { get; set; }
    }
}
