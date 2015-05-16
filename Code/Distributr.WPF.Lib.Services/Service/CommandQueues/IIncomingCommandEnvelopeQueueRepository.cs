using System;
using System.Collections.Generic;

namespace Distributr.WPF.Lib.Services.Service.CommandQueues
{
    public interface IIncomingCommandEnvelopeQueueRepository 
    {
        List<InComingCommandEnvelopeQueueItemLocal> GetUnProcessedEnvelopes();
        InComingCommandEnvelopeQueueItemLocal GetByEnvelopeId(Guid envelopeId);
        void MarkAsProcessed(Guid envelopeId);

        void Add(InComingCommandEnvelopeQueueItemLocal itemToAdd);
       
    }
}