using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Distributr.HQ.Lib.ViewModels.Admin
{
    public class AdminProductBrandViewModel
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
         [Required(ErrorMessage = "Product Name Is Required")]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
