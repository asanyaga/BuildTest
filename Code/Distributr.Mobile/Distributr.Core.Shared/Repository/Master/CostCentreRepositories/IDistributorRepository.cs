using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;

namespace Distributr.Core.Repository.Master.CostCentreRepositories
{
    public interface IDistributorRepository: ICostCentreRepository
    {
        Distributor GetDistributor();

        QueryResult<Distributor> Query(QueryStandard query);
    }
}
