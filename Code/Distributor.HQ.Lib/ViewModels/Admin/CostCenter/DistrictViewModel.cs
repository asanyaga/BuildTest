using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.HQ.Lib.Helper;

namespace Distributr.HQ.Lib.ViewModels.Admin.CostCenter
{
   public class DistrictViewModel
    {
       [LocalizedRequired(ErrorMessage = "hq.vm.district.name")]
       [RegularExpression(@"^[a-zA-Z0-9'.\s]{1,40}$", ErrorMessage = "Special characters are not allowed")]
       public string DistrictName { get; set; }


       [LocalizedRequired(ErrorMessage = "hq.vm.district.province")]
       public Guid ProvinceId { get; set; }
       public string ProvinceName { get; set; }
       public bool isActive { get; set; }
       public Guid Id { get; set; }

       public Lazy<List<ProvinceViewModel>> LoadProvince { get; set; }
   }
}
