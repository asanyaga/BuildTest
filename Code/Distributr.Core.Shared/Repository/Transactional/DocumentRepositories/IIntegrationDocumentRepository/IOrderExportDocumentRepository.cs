using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Intergration;

namespace Distributr.Core.Repository.Transactional.DocumentRepositories.IIntegrationDocumentRepository
{
    public interface IOrderExportDocumentRepository
    {
        OrderExportDocument GetDocument(OrderType orderType, DocumentStatus status);
        bool MarkAsExported(string orderExternalRef);
    }
}