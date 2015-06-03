using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;

namespace Distributr.Core.Repository.Master.CostCentreRepositories
{
    public interface IOutletCategoryRepository: IRepositoryMaster<OutletCategory>
    {
        OutletCategory GetByName(string code, bool includeDeactivated = false);

        QueryResult<OutletCategory> Query(QueryStandard q);
    }
}
