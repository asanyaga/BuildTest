using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Distributr.HQ.Lib.ViewModels.Admin.Contact
{
    public class ContactTypeViewModel
    {
        [Required(ErrorMessage = "Name is required!")]
        public string Name { get; set; }

        public Guid Id { get; set; }
        public bool isActive { get; set; }

        //[Required(ErrorMessage = "Description is required!")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Code is required!")]
        [StringLength(15, MinimumLength = 1, ErrorMessage = "Code length should not exceed 15 charachers.")]
        public string Code { get; set; }

        public string ErrorText { get; set; }
    }
}
