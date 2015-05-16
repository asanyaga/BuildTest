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
    public class QBUploadedDocumentsRepository : IQBUploadedDocumentsRepository
    {
        private DistributrLocalContext _ctx;

        public QBUploadedDocumentsRepository(DistributrLocalContext ctx)
        {
            _ctx = ctx;
        }

        public void SaveUploadedDocument(Guid documentId, string quickBooksId, int documentType)
        {
            QBUploadedDocument document = new QBUploadedDocument()
                                         {
                                             DocumentId = documentId,
                                             QuickBooksID = quickBooksId,
                                             DocumentType = documentType,
                                             DateUploaded = DateTime.Now,
                                         };
            var existing = _ctx.Set<QBUploadedDocument>().Find(document.DocumentId);
            if (existing == null)
            {
                _ctx.Set<QBUploadedDocument>().Add(document);

                _ctx.SaveChanges();
            }
        }

        public List<QBUploadedDocument> GetAllUploadedDocuments()
        {
            var items = _ctx.Set<QBUploadedDocument>().ToList();
            return items;
        }

        public QBUploadedDocument GetUploadedDocumentById(Guid documentId)
        {
            var item = _ctx.Set<QBUploadedDocument>().Find(documentId);
            return item;
        }

        public bool IsDocumentUploaded(Guid documentId)
        {
            return _ctx.Set<QBUploadedDocument>().Any(n => n.DocumentId == documentId);
        }

        public int GetCount()
        {
            throw new NotImplementedException();
        }
    }
}
