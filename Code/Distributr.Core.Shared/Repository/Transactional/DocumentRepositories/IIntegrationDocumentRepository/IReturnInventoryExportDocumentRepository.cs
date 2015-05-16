using System;
using Distributr.Core.Intergration;

namespace Distributr.Core.Repository.Transactional.DocumentRepositories.IIntegrationDocumentRepository
{
    public interface IReturnInventoryExportDocumentRepository
    {
        
        ReturnInventoryExportDocument GetDocument();
        bool MarkAsExported(Guid id);
    }
}