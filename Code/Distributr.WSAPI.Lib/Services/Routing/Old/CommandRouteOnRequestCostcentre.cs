using System;

namespace Distributr.WSAPI.Lib.Services.Routing
{
    [Obsolete("Command Envelope Refactoring")]
    public class CommandRouteOnRequestCostcentre
    {
        public Guid Id { get; set; }
        public long CommandRouteOnRequestId { get; set; }
        public Guid CostCentreId { get; set; }
        public bool IsValid { get; set; }
        public DateTime DateAdded { get; set; }
        public bool IsRetired { get; set; }
        public string CommandType { get; set; }
    }

   
}