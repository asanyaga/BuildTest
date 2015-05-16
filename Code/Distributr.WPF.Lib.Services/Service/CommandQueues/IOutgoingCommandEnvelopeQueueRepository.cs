using System;
using System.Collections.Generic;

namespace Distributr.WPF.Lib.Services.Service.CommandQueues
{
    public interface IOutgoingCommandEnvelopeQueueRepository 
    {
        OutGoingCommandEnvelopeQueueItemLocal GetById(int id);
        List<OutGoingCommandEnvelopeQueueItemLocal> GetByDocumentId(Guid documentId);
        List<OutGoingCommandEnvelopeQueueItemLocal> GetUnSentEnvelope();
        void MarkEnvelopeAsSent(int id);
        OutGoingCommandEnvelopeQueueItemLocal UpdateSerializedObject(int id);
        OutGoingCommandEnvelopeQueueItemLocal GetFirstUnSentEnvelope();
        void Delete(int id);
        void Add(OutGoingCommandEnvelopeQueueItemLocal itemToAdd);
        OutGoingCommandEnvelopeQueueItemLocal GetByEnvelopeId(Guid id);

    }
}