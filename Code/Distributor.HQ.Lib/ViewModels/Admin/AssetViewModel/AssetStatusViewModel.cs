using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Distributr.HQ.Lib.ViewModels.Admin.AssetViewModel
{
    public class AssetStatusViewModel
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
        [Required(ErrorMessage = "Description is a Required Field!")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Name is a Required Field!")]
        public string Name { get; set; }
    }
}
