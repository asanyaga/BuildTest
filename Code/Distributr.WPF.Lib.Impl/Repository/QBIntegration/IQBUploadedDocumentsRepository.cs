using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.WPF.Lib.Impl.Model.QBIntegration;

namespace Distributr.WPF.Lib.Impl.Repository.QBIntegration
{
    public interface IQBUploadedDocumentsRepository
    {
        void SaveUploadedDocument(Guid documentId, string quickBooksId, int documentType);
        List<QBUploadedDocument> GetAllUploadedDocuments();
        QBUploadedDocument GetUploadedDocumentById(Guid documentId);
        bool IsDocumentUploaded(Guid documentId);
        int GetCount();
    }
}
