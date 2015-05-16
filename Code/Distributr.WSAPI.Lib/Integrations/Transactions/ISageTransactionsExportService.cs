using System.Collections.Generic;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IIntegrationDocumentRepository;

namespace Distributr.WSAPI.Lib.Integrations.Transactions
{
    public interface ISageTransactionsExportService :ITransactionsExportAudit
    {
        List<ShellOrderExportDto> GetShellOrderByRef(string orderRef);
        List<ShellOrderExportDto> GetShellOrdersPendingExport();

       
    }
    public interface ISapTransactionsDownloadService : ITransactionsExportAudit
    {
        List<SapDocumentExportDto> GetOrdersPendingExport(string orderref = "", OrderType orderType = OrderType.OutletToDistributor);
        
    }
    public interface IQuickBooksTransactionsDownloadService : ITransactionsExportAudit
    {
        List<QuickBooksOrderDocumentDto> GetOrdersPendingExport(string orderref = "", bool includeInvoiceAndReceipts = false,DocumentStatus documentStatus=DocumentStatus.Closed);
        //List<QuickBooksOrderDocumentDto> GetOrdersNotExported(string orderref = "", bool includeInvoiceAndReceipts = false);
        List<QuickBooksReturnInventoryDocumentDto> GetReturnsPendingExport(string documentRef="");
    }

    public interface ITransactionsExportAudit
    {
        bool MarkAsExported(IEnumerable<string> orderReferences); 
    }
}
