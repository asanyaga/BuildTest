using System;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Intergration;

namespace Distributr.Core.Repository.Transactional.DocumentRepositories.IIntegrationDocumentRepository
{
    public interface IReceiptExportDocumentRepository
    {
        ReceiptExportDocument GetPayment();
        bool MarkAsExported(Guid receiptId);
    }
}