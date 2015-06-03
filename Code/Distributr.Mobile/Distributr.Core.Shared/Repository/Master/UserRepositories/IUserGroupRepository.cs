using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;

namespace Distributr.Core.Repository.Master.UserRepositories
{
    public interface IUserGroupRepository : IRepositoryMaster<UserGroup>
    {
        QueryResult<UserGroup> Query(QueryStandard q);
    }
}
