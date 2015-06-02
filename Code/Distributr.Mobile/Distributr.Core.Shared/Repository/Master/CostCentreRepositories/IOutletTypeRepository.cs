using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;

namespace Distributr.Core.Repository.Master.CostCentreRepositories
{
    public interface IOutletTypeRepository : IRepositoryMaster<OutletType>
    {
        OutletType GetByName(string name, bool includeDeactivated = false);
        QueryResult<OutletType> QueryResult(QueryStandard q);
    }
    
}
