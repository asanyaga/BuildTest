using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.CommodityEntities;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;

namespace Distributr.Core.Repository.Master.CommodityRepositories
{
   public interface ICommodityTypeRepository: IRepositoryMaster<CommodityType>
    {
       QueryResult<CommodityType> Query(QueryStandard query);
    }
}
