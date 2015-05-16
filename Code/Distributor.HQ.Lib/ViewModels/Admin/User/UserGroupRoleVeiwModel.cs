using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.HQ.Lib.ViewModels.Admin.User
{
    public class UserGroupRoleVeiwModel
    {
        public Guid Id { get; set; }
        public int RoleId { get; set; }
        public Guid GroupId { get; set; }
        public string GroupName { get; set; }
        public string RoleName { get; set; }
        public bool IsActive { get; set; }
    }
}
