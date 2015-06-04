using System;
using System.Collections.Generic;
using Distributr.Core.Domain.Master;
using SQLite.Net.Attributes;

namespace Distributr.Mobile.Core.Envelopes
{
    public class IncomingEnvelopeLog : MasterEntity
    {
        public Guid? LastEnvelopeId { get; set; }

        public IncomingEnvelopeLog()
            : base(Guid.NewGuid())
        {
        }

        [Ignore]
        public List<Guid> LastEnvelopeIds
        {
            get
            {
                if (LastEnvelopeId == null)
                {
                    return new List<Guid>();
                }
                return new List<Guid>(){LastEnvelopeId.Value};
            }
        }
    }
}
