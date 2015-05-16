using System;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using MongoDB.Bson.Serialization.Attributes;

namespace Distributr.WSAPI.Lib.Services.Routing
{
    public class CommandEnvelopeRoutingTracker
    {
        public Guid Id { get; set; }
        public long EnvelopeArrivalAtServerTick { get; set; }
        public Guid EnvelopeId { get; set; }
    }
    public class CommandEnvelopeRoutingStatus
    {
        public Guid Id { get; set; }
        public string DocumentType { set; get; }
        public long EnvelopeDeliveredAtServerTick { get; set; }
        public bool IsReconcile { get; set; }
        public long EnvelopeArrivalAtServerTick { get; set; }
        public Guid EnvelopeId { get; set; }
        public Guid DestinationCostCentreApplicationId { get; set; }
        public bool Delivered { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime DateDelivered { get; set; }

        public bool Executed { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime DateExecuted { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime DateAdded { get; set; }
        public Guid CostCentreId { get; set; }


    }
}