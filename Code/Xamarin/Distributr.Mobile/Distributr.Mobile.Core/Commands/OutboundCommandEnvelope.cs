using System;
using Distributr.Core.Domain.Master;

namespace Distributr.Mobile.Core.Commands
{
    public enum RoutingStatus  {Pending, Sent}

    public class OutboundCommandEnvelope : MasterEntity
    {
        public OutboundCommandEnvelope() : base(default(Guid))
        {
        }

        public string Contents { get; set; }
        public RoutingStatus Status { get; set; }
    }
}
