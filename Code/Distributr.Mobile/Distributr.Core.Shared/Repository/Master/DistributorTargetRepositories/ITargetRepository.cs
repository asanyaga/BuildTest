using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.DistributorTargetEntities;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;

namespace Distributr.Core.Repository.Master.DistributorTargetRepositories
{
  public interface ITargetRepository:IRepositoryMaster<Target>
    {
      QueryResult<Target> Query(QueryStandard query);
    }
}
