using System;
using System.Collections.Generic;
using Distributr.Core.Domain.Master.UserEntities;

namespace Distributr.Core.MasterDataDTO.DataContracts
{
    public class UserItem : MasterBaseItem
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public int PasswordChanged { get; set; }

        public Guid CostCenterID { get; set; }

        public string PIN { get; set; }

        public int UserType { get; set; }

        public int SalesmanType { get; set; }

        public string Mobile { get; set; }

        public List<string> UserRoles { get; set; }

        public string TillNumber { get; set; }

        public string CostCentreCode { get; set; }
    }
}
