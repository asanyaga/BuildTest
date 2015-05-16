using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Intergration;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IIntegrationDocumentRepository;

namespace Integration.QuickBooks.Lib.Repository
{
    public interface IReceiptImportRepository
    {
        List<Guid> SaveToLocal(ReceiptExportDocument document);
        void SaveToQuickBooks(ReceiptExportDocument orderExportDocument);
        void MarkExportedLocal(Guid id);
        void MarkExportedLocal(string externalDocRef, string qbInvoiceTransactionId);
        List<ReceiptExportDocument> LoadFromDB();
        List<QuickBooksOrderDocLineItem> GetLineItems(string externalReference);
    }
}
