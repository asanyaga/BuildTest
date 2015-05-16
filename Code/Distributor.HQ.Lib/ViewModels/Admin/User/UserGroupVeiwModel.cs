using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Distributr.Core.Repository.Master.UserRepositories;

namespace Distributr.HQ.Lib.ViewModels.Admin.User
{
    public class UserGroupVeiwModel
    {
        public Guid Id { get; set; }
         [Required(ErrorMessage = "Group Name is required")]
        public string Name { get; set; }
         [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}
