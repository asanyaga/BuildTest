using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Transactional.ThirdPartyIntegrationEntities;

namespace Distributr.WPF.Lib.Impl.Repository.FCLIntegration
{
   public interface IExportImportAuditRepository
   {
      void MarkAsExported(Guid documentId);
       void MarkAsImported(Guid documentId);
       bool IsImported(string documentRef);
       bool IsExported(string documentRef);
       void Save(ExportImportAudit exportImport);
   }
}
