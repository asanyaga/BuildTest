using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributr.HQ.Lib.ViewModels.Agrimanagr.CommodityViewModel
{
    public class CommodityGradeViewModel
    {
            public Guid Id { get; set; }
            [Required(ErrorMessage = "Name is required")]
            public string Name { get; set; }
            [Required(ErrorMessage = "Commodity is required")]
            public Guid CommodityId { get; set; }
            public string CommodityName { get; set; }
            public int UsageTypeId { get; set; }
            [StringLength(15, MinimumLength = 1, ErrorMessage = "Code length should not exceed 15 charachers.")]

            [Required(ErrorMessage = "Code is required")]
            public string Code { get; set; }
            public string Description { get; set; }
            public string ErrorText { get; set; }
            public bool Active { get; set; }
        
    }
}
