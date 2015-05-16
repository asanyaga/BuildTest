using System;

namespace Distributr.WSAPI.Lib.Services.Routing
{
    public enum EnvelopeRoutePriority
    {
        Level1 = 1,
        Level2 = 2,
    }
    public class CommandEnvelopeRouteOnRequestCostcentre
    {
        public Guid Id { get; set; }
        public string DocumentType { get; set; }
        public Guid EnvelopeId { get; set; }
        public Guid DocumentId { get; set; }
        public Guid ParentDocumentId { get; set; }
        public long EnvelopeArrivalAtServerTick { get; set; }
        public Guid CostCentreId { get; set; }
        public bool IsValid { get; set; }
        public DateTime DateAdded { get; set; }
        public bool IsRetired { get; set; }
       
        public long EnvelopeRoutedAtServerTick { get; set; }
        public EnvelopeRoutePriority EnvelopeRoutePriority { get; set; }
        public Guid GeneratedByCostCentreApplicationId { get; set; }
    }
}