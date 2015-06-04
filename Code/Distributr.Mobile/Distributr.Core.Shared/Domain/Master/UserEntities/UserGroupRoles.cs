using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.Domain.Master.UserEntities
{
#if !SILVERLIGHT
   [Serializable]
#endif
    public class UserGroupRoles:MasterEntity
    {
        public UserGroupRoles(Guid id) : base(id)
        {
        }

        public UserGroupRoles(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
            : base(id, dateCreated, dateLastUpdated, isActive)
        {
        }
        public int UserRole { set; get; }
        public UserGroup UserGroup { set; get; }
        public bool CanAccess { get; set; }
    }
}
