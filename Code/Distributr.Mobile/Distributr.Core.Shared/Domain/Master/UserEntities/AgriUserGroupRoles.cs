using System;

namespace Distributr.Core.Domain.Master.UserEntities
{
    public class AgriUserGroupRoles:MasterEntity
    {
        public AgriUserGroupRoles(Guid id) : base(id)
        {
        }

        public AgriUserGroupRoles(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
            : base(id, dateCreated, dateLastUpdated, isActive)
        {
        }
        public AgriUserRole UserRole { set; get; }
        public UserGroup UserGroup { set; get; }
        public bool CanAccess { get; set; }
    }
}