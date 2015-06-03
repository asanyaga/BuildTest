using System;
using Distributr.Core.Domain.Transactional.DocumentEntities;

namespace Distributr.Core.Domain.Transactional.ThirdPartyIntegrationEntities
{
   public class ExportImportAudit
    {
       public ExportImportAudit(){}
       public ExportImportAudit(IntegrationModule module, DocumentAuditStatus auditStatus, Guid documentId, string docRef, string externalDocRef)
       {
           DocumentId = documentId;
           IntegrationModule = module;
           AuditStatus = auditStatus;
           DocumentReference = docRef;
           ExternalDocumentRef = externalDocRef;
       }
       public Guid DocumentId { get; internal set; }
       public string DocumentReference { get; internal set; }
       public string ExternalDocumentRef { get; set; }
       public DocumentAuditStatus AuditStatus { get; set; }
       public DocumentType DocumentType { get; internal set; }
       public IntegrationModule IntegrationModule { get; internal set; }
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
       FCL = 3,
       Sage=4,
       SAP=5
   }
}
