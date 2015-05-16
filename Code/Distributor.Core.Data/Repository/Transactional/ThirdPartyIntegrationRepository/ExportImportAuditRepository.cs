using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.ThirdPartyIntegrationEntities;
using Distributr.Core.Repository.Transactional.ThirdPartyIntegrationRepository;

namespace Distributr.Core.Data.Repository.Transactional.ThirdPartyIntegrationRepository
{
    public class ExportImportAuditRepository : IExportImportAuditRepository
    {
        private CokeDataContext _ctx;

        public ExportImportAuditRepository(CokeDataContext ctx)
        {
            _ctx = ctx;
        }
        
       
        public bool IsExported(string documentRef)
        {
            var exist =
                _ctx.tblExportImportAudit.FirstOrDefault(
                    p =>
                    (p.DocumentReference == documentRef ||
                     p.ExternalDocumentReference.ToLower() == documentRef.ToLower()) &&
                    p.DocumentAuditStatus == (int) DocumentAuditStatus.Exported);
            return exist != null;
        }
        

        public void Save(ExportImportAudit exportImport)
        {
            var exist =
                _ctx.tblExportImportAudit.FirstOrDefault(
                    p =>
                    p.DocumentId != Guid.Empty && p.DocumentId == exportImport.DocumentId ||
                    p.DocumentReference == exportImport.DocumentReference);
            if (exist == null)
            {
                exist = new tblExportImportAudit
                            {
                                DocumentAuditStatus = (int) exportImport.AuditStatus,
                                DocumentId = exportImport.DocumentId,
                                ExternalDocumentReference = exportImport.ExternalDocumentRef,
                                DocumentReference = exportImport.DocumentReference,
                                DateUploaded = DateTime.Now,
                                IntegrationModule = (int) exportImport.IntegrationModule
                            };
                _ctx.tblExportImportAudit.AddObject(exist);

            }
            exist.ExternalDocumentReference = exportImport.ExternalDocumentRef;
            exist.DocumentReference = exportImport.DocumentReference;
            exist.DocumentAuditStatus = (int) exportImport.AuditStatus;
            exist.DateUploaded = DateTime.Now;
            exist.IntegrationModule = (int) exportImport.IntegrationModule;
            exist.DocumentType = (int) exportImport.DocumentType;


            _ctx.SaveChanges();
        }

        
    

    }
}
