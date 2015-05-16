using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributr.HQ.Lib.ViewModels.Agrimanagr.CentreViewModels
{
    public class CentreTypeViewModel
    {
        public Guid Id { get; set; }
        [StringLength(15, MinimumLength = 1, ErrorMessage = "Code length should not exceed 15 charachers.")]
        public string Code { get; set; }
        [Required(ErrorMessage = "Name is Required")]
        //[RegularExpression(@"(\S)+", ErrorMessage = "White space is not allowed")]
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}
