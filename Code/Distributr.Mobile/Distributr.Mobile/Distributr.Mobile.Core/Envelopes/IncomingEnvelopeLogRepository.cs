using System;
using System.Linq;
using Distributr.Mobile.Data;

namespace Distributr.Mobile.Core.Envelopes
{
    public class IncomingEnvelopeLogRepository
    {
        private readonly Database database;

        public IncomingEnvelopeLogRepository(Database database)
        {
            this.database = database;
        }

        public IncomingEnvelopeLog GetIncomingEnvelopeLog()
        {
            var log = database.GetAll<IncomingEnvelopeLog>().FirstOrDefault();
            if (log == null)
            {
                log = new IncomingEnvelopeLog();
                database.Insert(log);
            }
            return log;
        }

        public void UpdateLastEnvelopeId(Guid lastEnvelopeId)
        {
            var log = GetIncomingEnvelopeLog();
            log.LastEnvelopeId = lastEnvelopeId;

            database.Update(log);
        }
    }
}
