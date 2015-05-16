using System;
using Distributr.Core.Commands.CommandPackage;
using Distributr.Core.Domain.Transactional.DocumentEntities;

namespace Distributr.WSAPI.Lib.Services.CommandAudit
{
    public class CommandEnvelopeProcessingAudit
    {
       
        public Guid Id { get; set; }
        public Guid DocumentId { get; set; }
        public string DocumentTypeName { set; get; }
        public DocumentType DocumentType { set; get; }
        public long EnvelopeArrivalAtServerTick { get; set; }
        public int LastExecutedCommand { get; set; }
        public int NumberOfCommand { get; set; }
        public Guid GeneratedByCostCentreApplicationId { get; set; }
        public Guid GeneratedByCostCentreId { set; get; }
        public Guid RecipientCostCentreId { set; get; }
        public Guid ParentDocumentId { get; set; }
        public string JsonEnvelope { get; set; }
       
        public EnvelopeProcessingStatus Status { get; set; }
        public int RetryCounter { get; set; }
        public DateTime DateInserted { get; set; }
        public string SendDateTime { get; set; }
        
        public long EnvelopeProcessOnServerTick { get; set; }
        public long EnvelopeGeneratedTick { get; set; }
    }
}