using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Distributr.Core.Domain.Master.UserEntities;

namespace Distributr.WSAPI.Lib.Services.WCFServices.DataContracts
{
    public class UserItem : MasterBaseItem
    {
        [DataMember]
        public string Username { get; set; }

        [DataMember]
        public string Password { get; set; }

        [DataMember]
        public Guid CostCenterID { get; set; }

        [DataMember]
        public string PIN { get; set; }

        [DataMember]
        public int UserType { get; set; }

        [DataMember]
        public string Mobile { get; set; }

        [DataMember]
        public List<UserRole> UserRoles { get; set; }

        [DataMember]
        public string TillNumber { get; set; }
    }
}
