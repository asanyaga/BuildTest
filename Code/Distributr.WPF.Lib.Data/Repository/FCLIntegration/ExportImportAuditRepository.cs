using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Transactional.ThirdPartyIntegrationEntities;
using Distributr.WPF.Lib.Data.EF;
using Distributr.WPF.Lib.Impl.Repository.FCLIntegration;

namespace Distributr.WPF.Lib.Data.Repository.FCLIntegration
{
    public class ExportImportAuditRepository : IExportImportAuditRepository
    {
        private DistributrLocalContext _ctx;

        public ExportImportAuditRepository(DistributrLocalContext ctx)
        {
            _ctx = ctx;
        }

        public void MarkAsExported(Guid documentId)
        {
            var exist = _ctx.ExportImportAudits.FirstOrDefault(p => p.DocumentId == documentId && p.IntegrationModule == IntegrationModule.FCL && p.AuditStatus==DocumentAuditStatus.New);
            if (exist != null)
            {
                exist.AuditStatus = DocumentAuditStatus.Exported;
                _ctx.SaveChanges();
            }
        }

        public void MarkAsImported(Guid documentId)
        {
            var exist = _ctx.ExportImportAudits.FirstOrDefault(p => p.DocumentId == documentId && p.IntegrationModule == IntegrationModule.FCL && p.AuditStatus == DocumentAuditStatus.Exported);
           if(exist !=null)
           {
               exist.AuditStatus=DocumentAuditStatus.Imported;
               _ctx.SaveChanges();
           }
        }

        public bool IsImported(string documentRef)
        {
            var exist = _ctx.ExportImportAudits.FirstOrDefault(p => p.DocumentReference == documentRef && p.IntegrationModule == IntegrationModule.FCL && p.AuditStatus == DocumentAuditStatus.Imported);
            return exist != null;
        }

        public bool IsExported(string documentRef)
        {
            var exist = _ctx.ExportImportAudits.FirstOrDefault(p => p.DocumentReference == documentRef && p.IntegrationModule == IntegrationModule.FCL && p.AuditStatus == DocumentAuditStatus.Exported);
            return exist != null;
        }

        public void Save(ExportImportAudit exportImport)
        {
            var exist = _ctx.ExportImportAudits.FirstOrDefault(p=>p.DocumentId==exportImport.DocumentId);
            if(exist==null)
            {
                exist = new ExportImportAudit(IntegrationModule.FCL, DocumentAuditStatus.New, exportImport.DocumentId, exportImport.DocumentReference, exportImport.ExternalDocumentRef);
                exist.DateUploaded = DateTime.Now;
                _ctx.ExportImportAudits.Add(exist);
                
            }
            else
            {
               exist.ExternalDocumentRef = exportImport.ExternalDocumentRef;
            }
            _ctx.SaveChanges();
        }
    }
}
