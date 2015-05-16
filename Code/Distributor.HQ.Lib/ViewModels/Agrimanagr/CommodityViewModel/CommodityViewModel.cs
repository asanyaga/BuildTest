using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master.CommodityEntities;

namespace Distributr.HQ.Lib.ViewModels.Agrimanagr.CommodityViewModel
{
    public class CommodityViewModel
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Name is a Required Field!")]
        public string Name { get; set; }
        [Required(ErrorMessage = "CommodityType is a Required Field!")]
        public Guid CommodityTypeId { get; set; }
        public string CommodityTypeName { get; set; }
        [StringLength(15, MinimumLength = 1, ErrorMessage = "Code length should not exceed 15 charachers.")]

        [Required(ErrorMessage = "Code is a Required Field!")]
        public string Code { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }

    }
    public class CommodityGradeViewModelItems
    {
        public Guid Id { get; set; }
        public string CommodityGradeName { get; set; }
        public Guid CommodityId { get; set; }
        public int UsageTypeId { get; set; }
        public string CommodityGradeCode { get; set; }
        public string CommodityGradeDescription { get; set; }
        public string ErrorText { get; set; }
        public bool Active { get; set; }
    }
}
