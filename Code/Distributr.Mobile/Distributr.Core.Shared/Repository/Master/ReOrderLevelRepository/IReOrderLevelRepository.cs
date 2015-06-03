using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.ReOrdeLevelEntities;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;

namespace Distributr.Core.Repository.Master.ReOrderLevelRepository
{
   public interface IReOrderLevelRepository:IRepositoryMaster<ReOrderLevel>
   {

       QueryResult<ReOrderLevel> Query(QueryStandard query);

   }
}
