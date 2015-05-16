using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Distributr.Core.Domain.Transactional.DocumentEntities;

namespace Distributr.WPF.Lib.Impl.Model.QBIntegration
{
    public class IntegrationExportedDocument
    {
        public Guid DocumentId { get; set; }
        public string ExternalDocumentRef { get; set; }
        public DateTime DateUploaded { get; set; }
        public int DocumentType { get; set; }
        public int IntegrationModule { get; set; }
    }

    public enum IntegrationModule
    {
        PZCussons = 1,
        QuickBooks = 2
    }

}
