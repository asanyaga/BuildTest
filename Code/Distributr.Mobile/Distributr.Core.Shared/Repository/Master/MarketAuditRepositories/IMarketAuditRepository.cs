using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.MarketAudit;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;

namespace Distributr.Core.Repository.Master.MarketAuditRepositories
{
    public interface IMarketAuditRepository : IRepositoryMaster<MarketAudit>
    {
        QueryResult<MarketAudit> Query(QueryStandard query);
    }
}
