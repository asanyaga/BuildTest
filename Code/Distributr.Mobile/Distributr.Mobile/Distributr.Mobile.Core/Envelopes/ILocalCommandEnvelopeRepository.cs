using System;
using System.Collections.Generic;
using Distributr.Core.Repository;
using Distributr.Mobile.Envelopes;

namespace Distributr.Mobile.Core.Envelopes
{
    public interface ILocalCommandEnvelopeRepository : IRepositoryMaster<LocalCommandEnvelope>
    {
        List<LocalCommandEnvelope> GetNextIncomingBatch();
        List<LocalCommandEnvelope> GetNextOutgoingBatch();
        void Delete(LocalCommandEnvelope localCommandEnvelope);
        void MarkAsFailed(Guid parentDocumentGuid, string errorMessage);
        void UpdateToPending(Guid parentDoucmentGuid);
        void UpdateAllEnvelopesWithErrorToPending();
    }
}