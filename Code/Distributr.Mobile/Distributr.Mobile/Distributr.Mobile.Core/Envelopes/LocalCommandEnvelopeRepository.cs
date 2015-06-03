using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Mobile.Data;
using Distributr.Mobile.Envelopes;

namespace Distributr.Mobile.Core.Envelopes
{
    public class LocalCommandEnvelopeRepository : BaseRepository <LocalCommandEnvelope>, ILocalCommandEnvelopeRepository
    {

        private readonly Database database;

        public LocalCommandEnvelopeRepository(Database database) : base (database) 
        {
            this.database = database;
        }

        public List<LocalCommandEnvelope> GetNextOutgoingBatch()
        {
            return GetNextPendingBatch(RoutingDirection.Outgoing);
        }

        public List<LocalCommandEnvelope> GetNextIncomingBatch()
        {
            return GetNextPendingBatch(RoutingDirection.Incoming);
        }

        private List<LocalCommandEnvelope> GetNextPendingBatch(RoutingDirection routingDirection)
        {
            var last = database.Query<LocalCommandEnvelope>(
                    "SELECT * FROM LocalCommandEnvelope WHERE RoutingStatus = ? AND RoutingDirection = ? ORDER BY ROWID LIMIT 1",
                    RoutingStatus.Pending, routingDirection);

            if (last.Count == 0) return new List<LocalCommandEnvelope>();

            return database.Query<LocalCommandEnvelope>(
                "SELECT * FROM LocalCommandEnvelope WHERE ParentDoucmentGuid = ? ORDER BY ProcessingOrder",
                last[0].ParentDoucmentGuid);
        }

        public void Delete(LocalCommandEnvelope localCommandEnvelope)
        {
            database.Delete(localCommandEnvelope);
        }

        public void MarkAsFailed(Guid parentDocumentGuid, string errorMessage)
        {
            database.ExecuteScalar<int>(
                "UPDATE LocalCommandEnvelope SET ErrorMessage = ?, DateLastUpdated = ?, RoutingStatus = ? WHERE ParentDoucmentGuid = ?",
                errorMessage, DateTime.Now.Ticks, RoutingStatus.Error, parentDocumentGuid);
        }

        public void UpdateToPending(Guid parentDoucmentGuid)
        {
            database.ExecuteScalar<int>(
                "UPDATE LocalCommandEnvelope SET RoutingStatus = ?, DateLastUpdated = ? WHERE ParentDoucmentGuid = ?", 
                RoutingStatus.Pending, DateTime.Now.Ticks, parentDoucmentGuid);
        }

        public void UpdateAllEnvelopesWithErrorToPending()
        {
            database.ExecuteScalar<int>(
                "UPDATE LocalCommandEnvelope SET RoutingStatus = ?, DateLastUpdated = ? WHERE RoutingStatus = ?",
                RoutingStatus.Pending, DateTime.Now.Ticks, RoutingStatus.Error);
        }
    }
}
