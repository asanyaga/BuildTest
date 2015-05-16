using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Intergration;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IIntegrationDocumentRepository;
using Integration.QuickBooks.Lib.EF.Entities;

namespace Integration.QuickBooks.Lib.Repository
{
    public interface IOrderImportRepository
    {
        List<Guid> SaveToLocal(OrderExportDocument document,OrderType orderType);
        void SaveToQuickBooks(OrderExportDocument orderExportDocument);
        void MarkExportedLocal(Guid id);
        void MarkExportedLocal(string externalDocRef,string QbOrderTransactionId);
        List<OrderExportDocument> LoadFromDB(OrderType type);
        List<QuickBooksOrderDocLineItem> GetLineItems(string externalReference);
    }
}
