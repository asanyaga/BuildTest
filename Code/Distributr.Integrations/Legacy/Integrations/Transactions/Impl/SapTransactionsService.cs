using System.Collections.Generic;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.ThirdPartyIntegrationEntities;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IIntegrationDocumentRepository;

namespace Distributr.Integrations.Legacy.Integrations.Transactions.Impl
{
    public class SapTransactionsDownloadService : TransactionsExportAudit,ISapTransactionsDownloadService
    {
        public SapTransactionsDownloadService(IIntegrationDocumentRepository integrationDocumentRepository) : base(integrationDocumentRepository)
        {
        }

        public List<SapDocumentExportDto> GetOrdersPendingExport(string orderref = "",OrderType orderType=OrderType.OutletToDistributor)
        {
            return _integrationDocumentRepository.GetOrdersPendingExport(orderref, orderType);
        }
        public override  bool MarkAsExported(IEnumerable<string> orderReferences)
        {
            return _integrationDocumentRepository.MarkAsExported(orderReferences, IntegrationModule.SAP);
            
        }

    }
}
