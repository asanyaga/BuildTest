using System;
using System.Collections.Generic;
using Distributr.WPF.Lib.Data.EF;
using System.Linq;
using Distributr.WPF.Lib.Services.Service.CommandQueues;

namespace Distributr.WPF.Lib.Data.Repository.Commands
{

    public class IncomingCommandEnvelopeQueueRepository : IIncomingCommandEnvelopeQueueRepository
    {
        private DistributrLocalContext _ctx;

        public IncomingCommandEnvelopeQueueRepository(DistributrLocalContext ctx)
        {
            _ctx = ctx;
        }

       

        public List<InComingCommandEnvelopeQueueItemLocal> GetUnProcessedEnvelopes()
        {
            throw new NotImplementedException();
        }

        public InComingCommandEnvelopeQueueItemLocal GetByEnvelopeId(Guid envelopeId)
        {
            var existing = _ctx.InComingCommandEnvelopeQueueItems.FirstOrDefault(p => p.EnvelopeId == envelopeId);
            return existing;
        }

        public void MarkAsProcessed(Guid envelopeId)
        {
            var existing = _ctx.InComingCommandEnvelopeQueueItems.FirstOrDefault(p => p.EnvelopeId == envelopeId);
            if (existing != null)
            {
                existing.Processed = true;
            }
        }

        public void Add(InComingCommandEnvelopeQueueItemLocal itemToAdd)
        {
            InComingCommandEnvelopeQueueItemLocal existing = null;

            existing = _ctx.InComingCommandEnvelopeQueueItems.FirstOrDefault(p => p.EnvelopeId == itemToAdd.EnvelopeId);
            if (existing == null)
            {
                existing = new InComingCommandEnvelopeQueueItemLocal();
                _ctx.InComingCommandEnvelopeQueueItems.Add(itemToAdd);
                existing.NoOfRetry = 0;

            }
            existing.EnvelopeId = itemToAdd.EnvelopeId;
            existing.DocumentType = itemToAdd.DocumentType;
            existing.DateInserted = itemToAdd.DateInserted;
            existing.Info = itemToAdd.Info;
            existing.DocumentId = itemToAdd.DocumentId;
            existing.Processed = itemToAdd.Processed;
            existing.NoOfRetry = itemToAdd.NoOfRetry;
            _ctx.SaveChanges();

        }
    }

    [Obsolete("Command Envelope Refactoring")]
    public class IncomingCommandQueueRepository : IIncomingCommandQueueRepository
    {
        private DistributrLocalContext _ctx;

        public IncomingCommandQueueRepository(DistributrLocalContext ctx)
        {
            _ctx = ctx;
        }
        public List<IncomingCommandQueueItemLocal> GetUnProcessedCommands()
        {
            return _ctx.IncomingCommandQueueItemLocals.Where(p => p.Processed == false && p.NoOfRetry<5 ).OrderByDescending(s=>s.Id).ToList();
        }

        public IncomingCommandQueueItemLocal GetByIOD(int OID)
        {
            return _ctx.IncomingCommandQueueItemLocals.FirstOrDefault(p => p.Id == OID);

        }

        public IncomingCommandQueueItemLocal GetByCommandId(Guid commandId)
        {
            return _ctx.IncomingCommandQueueItemLocals.FirstOrDefault(n => n.CommandId == commandId);
        }

        public List<IncomingCommandQueueItemLocal> GetByDocumentId(Guid documentId)
        {
            return _ctx.IncomingCommandQueueItemLocals.Where(n => n.DocumentId == documentId).ToList();
        }

        public void Add(IncomingCommandQueueItemLocal itemToAdd)
        {
            IncomingCommandQueueItemLocal existing = null;

            existing = _ctx.IncomingCommandQueueItemLocals.FirstOrDefault(p => p.CommandId == itemToAdd.CommandId);
            if (existing == null)
            {
                existing = new IncomingCommandQueueItemLocal();
                _ctx.IncomingCommandQueueItemLocals.Add(itemToAdd);
                existing.NoOfRetry = 0;

            }
            existing.CommandId = itemToAdd.CommandId;
            existing.CommandType = itemToAdd.CommandType;
            existing.DateInserted = itemToAdd.DateInserted;
            existing.DateReceived = itemToAdd.DateReceived;
            existing.DocumentId = itemToAdd.DocumentId;
            existing.Processed = itemToAdd.Processed;
            existing.NoOfRetry = itemToAdd.NoOfRetry;
            _ctx.SaveChanges();


        }



        public void DeleteOldCommand()
        {
            DateTime date = DateTime.Now.AddDays(-2);
            var all = _ctx.IncomingCommandQueueItemLocals.Where(s => s.Processed && s.DateInserted < date).ToList();

            foreach (IncomingCommandQueueItemLocal icl in all)
            {
                _ctx.IncomingCommandQueueItemLocals.Remove(icl);

            }
            _ctx.SaveChanges();

        }

        public void MarkAsProcessed(Guid commandId)
        {

            IncomingCommandQueueItemLocal existing = GetByCommandId(commandId);
            if (existing != null)
            {
                existing.Processed = true;
                Add(existing);
            }
        }

        public void IncrimentRetryCounter(Guid commandId)
        {
            IncomingCommandQueueItemLocal existing = GetByCommandId(commandId);
            if (existing != null)
            {
                existing.NoOfRetry = existing.NoOfRetry+1;
                Add(existing);
            }
        }

        public bool ExitOldCommand()
        {
            DateTime date = DateTime.Now.AddDays(-2);
            return _ctx.IncomingCommandQueueItemLocals.Any(s => s.Processed && s.DateInserted < date);

        }


        public void DropType()
        {
            //siaqodb.DropType<IncomingCommandQueueItemLocal>();
            throw new NotImplementedException();
        }

        public List<IncomingCommandQueueItemLocal> GetAll()
        {
            return _ctx.IncomingCommandQueueItemLocals.ToList();
        }



    }
}
