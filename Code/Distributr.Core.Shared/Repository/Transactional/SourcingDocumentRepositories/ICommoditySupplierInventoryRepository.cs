using System.Collections.Generic;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;

namespace Distributr.Core.Repository.Transactional.SourcingDocumentRepositories
{
    public interface ICommoditySupplierInventoryRepository 
    {
        QueryResult<CommoditySupplierInventoryLevel> Query(QueryCommoditySupplierInventory query);
    }
}