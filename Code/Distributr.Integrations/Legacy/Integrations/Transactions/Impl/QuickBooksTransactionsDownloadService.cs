using System.Collections.Generic;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.ThirdPartyIntegrationEntities;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IIntegrationDocumentRepository;

namespace Distributr.Integrations.Legacy.Integrations.Transactions.Impl
{
    public class QuickBooksTransactionsDownloadService :TransactionsExportAudit, IQuickBooksTransactionsDownloadService
    {
        public QuickBooksTransactionsDownloadService(IIntegrationDocumentRepository integrationDocumentRepository) : base(integrationDocumentRepository)
        {
        }

        public List<QuickBooksOrderDocumentDto> GetOrdersPendingExport(string orderref = "", bool includeInvoiceAndReceipts = false,DocumentStatus documentStatus=DocumentStatus.Closed)
        {
            return _integrationDocumentRepository.GetPendingExport(includeInvoiceAndReceipts, orderref,documentStatus);
        }

        //public List<QuickBooksOrderDocumentDto> GetOrdersNotExported(string orderref = "", bool includeInvoiceAndReceipts = false)
        //{
        //    return _integrationDocumentRepository.GetOrdersNotExported(includeInvoiceAndReceipts, orderref);
        //}


        public List<QuickBooksOrderDocumentDto> GetTransactionPendingExport(bool includeInvoiceAndReceipts = false, DocumentStatus documentStatus = DocumentStatus.Closed)
        {
            return _integrationDocumentRepository.GetTransactionPendingExport(includeInvoiceAndReceipts,documentStatus);
        }

        public List<QuickBooksReturnInventoryDocumentDto> GetReturnsPendingExport(string documentRef = "")
        {
            return _integrationDocumentRepository.GetReturnsPendingExport(documentRef);
        }

        public override bool MarkAsExported(IEnumerable<string> orderReferences)
        {
            return _integrationDocumentRepository.MarkAsExported(orderReferences, IntegrationModule.QuickBooks);

        }
       
    }
}
