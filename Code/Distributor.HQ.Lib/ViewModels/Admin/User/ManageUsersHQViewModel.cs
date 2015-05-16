using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.UserEntities;

namespace Distributr.HQ.Lib.ViewModels.Admin.User
{
    public class ManageUsersHQViewModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public UserType UserType { get; set; }
        public int CostCentre { get; set; }
    }
}
