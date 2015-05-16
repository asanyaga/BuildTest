using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Distributr.HQ.Lib.ViewModels.Admin.User
{
    public class UserTypeViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "User Type Name is required")]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
