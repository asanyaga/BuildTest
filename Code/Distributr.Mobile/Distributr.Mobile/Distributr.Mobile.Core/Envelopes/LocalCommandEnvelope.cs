using System;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Transactional.DocumentEntities;

namespace Distributr.Mobile.Envelopes
{
    public enum RoutingStatus  {Pending, Error}
    public enum RoutingDirection { Incoming, Outgoing}

    public class LocalCommandEnvelope : MasterEntity
    {
        public LocalCommandEnvelope() : base(default(Guid))
        {
        }

        public Guid ParentDoucmentGuid { get; set; }
        public DocumentType DocumentType { get; set; }
        public string Contents { get; set; }
        public RoutingStatus RoutingStatus { get; set; }
        public RoutingDirection RoutingDirection { get; set; }
        public int ProcessingOrder { get; set; }
        public string ErrorMessage { get; set; }
    }
}
