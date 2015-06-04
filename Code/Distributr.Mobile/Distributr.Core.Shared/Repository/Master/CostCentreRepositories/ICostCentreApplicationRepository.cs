using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.CostCentreEntities;

namespace Distributr.Core.Repository.Master.CostCentreRepositories
{
    public interface ICostCentreApplicationRepository : IRepositoryMaster<CostCentreApplication>
    {
        IEnumerable<CostCentreApplication> GetByCostCentreId(Guid id);

        //return 0 if is first ccappid for the application
        //int[] GetSeedCostCentreApplicationIds(int costCentreId);
    }
}
