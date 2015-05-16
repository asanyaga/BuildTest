using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.CoolerEntities;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;

namespace Distributr.Core.Repository.Master.CoolerTypeRepositories
{
    public interface IAssetTypeRepository : IRepositoryMaster<AssetType>
    {
        QueryResult<AssetType> Query(QueryStandard query);
    }
}
