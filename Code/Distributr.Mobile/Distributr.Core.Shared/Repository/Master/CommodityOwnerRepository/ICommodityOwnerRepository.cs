using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.CommodityEntity;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;

namespace Distributr.Core.Repository.Master.CommodityOwnerRepository
{
    public interface ICommodityOwnerRepository : IRepositoryMaster<CommodityOwner>
    {
        List<CommodityOwner> GetBySupplier(Guid supplierId);
          QueryResult<CommodityOwner> Query(QueryBase q);
    }
}
