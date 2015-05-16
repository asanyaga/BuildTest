using System;
using System.Collections.Generic;
using Distributr.Core.Commands.CommandPackage;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using MongoDB.Bson.Serialization.Attributes;

namespace Distributr.WSAPI.Lib.Services.Routing
{
    
    [Obsolete("Command Envelope Refactoring")]
    public class CommandRouteOnRequest
    {
        public long Id { get; set; }
        public Guid CommandId { get; set; }
        public Guid DocumentId { get; set; }
        public DateTime DateCommandInserted { get; set; }
        public Guid CommandGeneratedByCostCentreApplicationId { get; set; }
        public Guid CommandGeneratedByUserId { get; set; }
        public string CommandType { get; set; }
        public string JsonCommand { get; set; }
        public Guid DocumentParentId { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime DateAdded { get; set; }

        public bool IsRetired { get; set; }
    }

    public class CommandRouteOnRequestDTO
{
        public CommandRouteOnRequestDTO()
        {
            CommandRouteCentres = new List<CommandRouteOnRequestCostcentre>();
            RouteOnRequest = new CommandRouteOnRequest();
        }
        public CommandRouteOnRequest RouteOnRequest{get;set;}
        public List<CommandRouteOnRequestCostcentre> CommandRouteCentres { get; set; } 
}
}