using System;
using Distributr.Core.Domain.Transactional.DocumentEntities;

namespace Distributr.WPF.Lib.Impl.Model.ThirdPartyIntegration
{
   public class ExportImportAudit
    {
       public Guid DocumentId { get; set; }
       public string DocumentReference { get; set; }
       public string ExternalDocumentReference { get; set; }
       public DocumentAuditStatus AuditStatus { get; set; }
       public DocumentType DocumentType { get; set; }
       public IntegrationModule IntegrationModule { get; set; }
       public DateTime DateUploaded { get; set; }
    }

   public enum DocumentAuditStatus
    {
       Exported=2,
       Imported=1,
       New=0
    }

   public enum IntegrationModule
   {
       Other = 0,
       PZCussons = 1,
       QuickBooks = 2,
       FCL = 3
   }
}
