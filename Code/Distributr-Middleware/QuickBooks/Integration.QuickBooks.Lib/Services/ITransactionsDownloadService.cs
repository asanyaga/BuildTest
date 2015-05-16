using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IIntegrationDocumentRepository;
using Distributr.WSAPI.Lib.Integrations;

namespace Integration.QuickBooks.Lib.Services
{
   public interface ITransactionsDownloadService
   {
       TransactionResponse DownloadAllAsync(string orderref = "", bool includeInvoiceAndReceipts = true, DocumentStatus documentStatus = DocumentStatus.Closed);
       TransactionResponse DownloadTransactionAsync(string orderref = "", bool includeInvoiceAndReceipts = true, DocumentStatus documentStatus = DocumentStatus.Closed);
       TransactionResponse DownloadReturnsAsync(string orderref = "");
       Task<List<QuickBooksOrderDocumentDto>> GetOrdersPendingExport(string orderref = "", bool includeInvoiceAndReceipts = false);
       List<QuickBooksOrderDocumentDto> GetTransactions();

        Task<TransactionExportResponse> GetNextOrder(OrderType orderType = OrderType.OutletToDistributor,DocumentStatus documentStatus = DocumentStatus.Closed);
        Task<TransactionExportResponse> GetNextInvoice();
        Task<TransactionExportResponse> GetNextReceipt();
        Task<TransactionExportResponse> MarkAsExported(string orderRef);
        Task<TransactionExportResponse> MarkInvoiceAsExported(Guid id);
        Task<TransactionExportResponse> MarkReceiptAsExported(Guid id);
   }
}
