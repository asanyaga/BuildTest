using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.DistributorTargetEntities;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;

namespace Distributr.Core.Repository.Master.DistributorTargetRepositories
{
   public interface ITargetPeriodRepository:IRepositoryMaster<TargetPeriod >
   {
       QueryResult<TargetPeriod> Query(QueryStandard query);
   }
}
