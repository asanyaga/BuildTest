using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributr.HQ.Lib.ViewModels.Agrimanagr.CostCentreViewModel
{
    public class CommodityOwnerTypeViewModel
    {

        public Guid Id { get; set; }
        [Required(ErrorMessage = "Name is a Required Field!")]
        public string Name { get; set; }
        [StringLength(15, MinimumLength = 1, ErrorMessage = "Code length should not exceed 15 charachers.")]
        public string Code { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}
