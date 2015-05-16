using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.AssetEntities;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;

namespace Distributr.Core.Repository.Master.AssetRepositories
{
    public interface IAssetStatusRepository : IRepositoryMaster<AssetStatus>
    {
        QueryResult<AssetStatus> Query(QueryStandard q);
    }
}
