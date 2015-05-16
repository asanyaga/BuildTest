using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Distributr.WSAPI.Lib.Services.Routing
{
    [Obsolete("Command Envelope Refactoring")]
    public class CommandRoutingStatus
    {
        public Guid Id { get; set; }
        public long CommandRouteOnRequestId { get; set; }
        public Guid CommandId { get; set; }
        public Guid DestinationCostCentreApplicationId { get; set; }
        public bool Delivered { get; set; }
        
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime DateDelivered { get; set; }

        public bool Executed { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime DateExecuted { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime DateAdded { get; set; }


    }
}