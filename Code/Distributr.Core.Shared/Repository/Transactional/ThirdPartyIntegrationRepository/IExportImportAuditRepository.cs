using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.ThirdPartyIntegrationEntities;

namespace Distributr.Core.Repository.Transactional.ThirdPartyIntegrationRepository
{
    public interface IExportImportAuditRepository
    {
      
       
        bool IsExported(string documentRef);
        void Save(ExportImportAudit exportImport);
        
    }
}
