using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.WPF.Lib.Impl.Model.QBIntegration;

namespace Distributr.WPF.Lib.Impl.Repository.QBIntegration
{
    public interface IIntegrationExportedDocumentsRepository
    {
        void SaveExportedDocument(Guid documentId, string externalDocumentId, int documentType, IntegrationModule integrationModule);
        List<IntegrationExportedDocument> GetAllExportedDocuments (IntegrationModule integrationModule);
        IntegrationExportedDocument GetExportedDocumentById(Guid documentId, IntegrationModule integrationModule);
        bool IsDocumentExported(Guid documentId, IntegrationModule integrationModule);
        bool OrderReferenceIsValid(string orderRef);
        int GetCount(IntegrationModule integrationModule);
    }
}
