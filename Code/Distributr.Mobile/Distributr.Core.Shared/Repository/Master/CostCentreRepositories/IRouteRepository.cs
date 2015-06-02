using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.Util;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;

namespace Distributr.Core.Repository.Master.CostCentreRepositories
{
    public interface IRouteRepository : IRepositoryMaster<Route>
    {
      QueryResult<Route> Query(QueryStandard query);
    }
    
}
