using System;
using System.ComponentModel.DataAnnotations;

namespace Distributr.HQ.Lib.ViewModels.Agrimanagr.CommodityViewModel
{
    public class CommodityTypeViewModel
    {   
        public Guid Id { get;  set; }
        [Required(ErrorMessage = "Name is a Required Field!")]
        public string Name { get;  set; }
        [Required(ErrorMessage = "Code is a Required Field!")]
        [StringLength(15, MinimumLength = 1, ErrorMessage = "Code length should not exceed 15 charachers.")]
        public string Code { get; set; }
        public string Description { get; set; }
        public int IsActive { get; set; }

    }
}
