using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Distributr.Core.Domain.Transactional.DocumentEntities;

namespace Distributr.WPF.Lib.Impl.Model.QBIntegration
{
    public class QBUploadedDocument
    {
        [Key]
        public Guid DocumentId { get; set; }
        public string QuickBooksID { get; set; }
        public DateTime DateUploaded { get; set; }
        public int DocumentType { get; set; }
    }
}
