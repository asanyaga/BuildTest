using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.CoolerEntities;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;

namespace Distributr.Core.Repository.Master.AssetRepositories
{
   public interface IAssetRepository:IRepositoryMaster<Asset>
   {
       QueryResult<Asset> Query(QueryStandard query);
   }
}
