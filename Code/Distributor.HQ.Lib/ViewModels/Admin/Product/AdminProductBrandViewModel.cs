using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Distributor.HQ.Lib.ViewModels.Admin.Product
{
    public class AdminProductBrandViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Code Is Required")]
        public string Code { get; set; }
        [Required(ErrorMessage = "Name  Is Required")]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
