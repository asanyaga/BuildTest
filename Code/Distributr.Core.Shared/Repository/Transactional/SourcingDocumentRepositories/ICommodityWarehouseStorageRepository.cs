using System.Collections.Generic;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;

namespace Distributr.Core.Repository.Transactional.SourcingDocumentRepositories
{
    public interface ICommodityWarehouseStorageRepository : ISourcingDocumentRepository
    {
        List<CommodityWarehouseStorageNote> GetAllByStatus(DocumentSourcingStatus status);
        //List<SourcingDocument> Query(QueryDocument query);
        QueryResult<CommodityWarehouseStorageNote> Query(QueryDocument query);
    }
}