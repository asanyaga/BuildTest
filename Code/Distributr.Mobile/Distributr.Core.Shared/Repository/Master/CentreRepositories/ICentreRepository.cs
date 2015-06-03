using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.CentreEntity;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;

namespace Distributr.Core.Repository.Master.CentreRepositories
{
    public interface ICentreRepository : IRepositoryMaster<Centre>
    {
        List<Centre> GetByHubId(Guid hubId, bool includeDeactivated = false);
        QueryResult Query(QueryStandard query);
    }
}
