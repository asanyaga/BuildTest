using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.UserEntities;

namespace Distributr.Core.Repository.Master.UserRepositories
{
    public interface IUserGroupRolesRepository : IRepositoryMaster<UserGroupRoles>
    {
        List<UserGroupRoles> GetByGroup(Guid GroupId);
        List<AgriUserGroupRoles> GetByAgriGroup(Guid goupid);
        Guid SaveAgriRole(AgriUserGroupRoles entity, bool? isSync = null);
    }
}
