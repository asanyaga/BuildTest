using System;
using System.Collections.Generic;
using Distributr.Core.Domain.Transactional.ThirdPartyIntegrationEntities;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IIntegrationDocumentRepository;

namespace Distributr.Integrations.Legacy.Integrations.Transactions.Impl
{
    public class SageTransactionsExportService : TransactionsExportAudit, ISageTransactionsExportService
   {
        public SageTransactionsExportService(IIntegrationDocumentRepository integrationDocumentRepository)
            : base(integrationDocumentRepository)
        {
        }

        public List<ShellOrderExportDto> GetShellOrderByRef(string orderRef)
       {
           return _integrationDocumentRepository.GetShellOrderByRef(orderRef);
       }

       public List<ShellOrderExportDto> GetShellOrdersPendingExport()
       {
           return _integrationDocumentRepository.GetShellOrdersPendingExport();
       }
       public override bool MarkAsExported(IEnumerable<string> orderReferences)
       {
           return _integrationDocumentRepository.MarkAsExported(orderReferences, IntegrationModule.Sage);

       }
      
   }
    public abstract class TransactionsExportAudit:ITransactionsExportAudit
    {
        protected readonly IIntegrationDocumentRepository _integrationDocumentRepository;

        protected TransactionsExportAudit(IIntegrationDocumentRepository integrationDocumentRepository)
        {
            _integrationDocumentRepository = integrationDocumentRepository;
        }

        public virtual bool MarkAsExported(IEnumerable<string> orderReferences)
        {
            throw new NotImplementedException();
        }
    }
}
