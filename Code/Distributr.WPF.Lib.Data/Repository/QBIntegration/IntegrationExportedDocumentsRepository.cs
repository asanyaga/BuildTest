using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.WPF.Lib.Data.EF;
using Distributr.WPF.Lib.Impl.Model.QBIntegration;
using Distributr.WPF.Lib.Impl.Repository.QBIntegration;

namespace Distributr.WPF.Lib.Data.Repository.QBIntegration
{
    public class IntegrationExportedDocumentsRepository : IIntegrationExportedDocumentsRepository
    {
        private DistributrLocalContext _ctx;

        public IntegrationExportedDocumentsRepository(DistributrLocalContext ctx)
        {
            _ctx = ctx;
        }

        public void SaveExportedDocument(Guid documentId, string externalDocumentId, int documentType, IntegrationModule integrationModule)
        {
            IntegrationExportedDocument document = new IntegrationExportedDocument()
                                                       {
                                                           DocumentId = documentId,
                                                           ExternalDocumentRef = externalDocumentId,
                                                           DocumentType = documentType,
                                                           DateUploaded = DateTime.Now,
                                                           IntegrationModule = (int) integrationModule
                                                       };
            var existing = GetExportedDocumentById(documentId, integrationModule);
            if (existing == null)
            {
                _ctx.Set<IntegrationExportedDocument>().Add(document);

                _ctx.SaveChanges();
            }
        }

        public List<IntegrationExportedDocument> GetAllExportedDocuments(IntegrationModule integrationModule)
        {
            var items = _ctx.Set<IntegrationExportedDocument>().Where(n => n.IntegrationModule == (int)integrationModule).ToList();
            return items;
        }

        public IntegrationExportedDocument GetExportedDocumentById(Guid documentId, IntegrationModule integrationModule)
        {
            int moduleId = (int) integrationModule;
            var item = _ctx.Set<IntegrationExportedDocument>().Find(documentId, moduleId);
            return item;
        }

        public bool IsDocumentExported(Guid documentId, IntegrationModule integrationModule)
        {
            return GetExportedDocumentById(documentId, integrationModule) != null;
        }

        public bool OrderReferenceIsValid(string orderRef)
        {
            return _ctx.Set<IntegrationExportedDocument>().FirstOrDefault(n => n.ExternalDocumentRef == orderRef) == null;
        }

        public int GetCount(IntegrationModule integrationModule)
        {
            throw new NotImplementedException();
        }
    }
}
