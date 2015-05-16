using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.Commands.CommandPackage;
using Distributr.WPF.Lib.Data.EF;
using Distributr.WPF.Lib.Services.Service.CommandQueues;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Distributr.WPF.Lib.Data.Repository.Commands
{
    public class OutgoingCommandEnvelopeQueueRepository : IOutgoingCommandEnvelopeQueueRepository
    {
        private DistributrLocalContext _ctx;

        public OutgoingCommandEnvelopeQueueRepository(DistributrLocalContext ctx)
        {
            _ctx = ctx;
        }

        public OutGoingCommandEnvelopeQueueItemLocal GetById(int id)
        {
            return _ctx.OutGoingCommandEnvelopeQueueItems.FirstOrDefault(s => s.Id == id);
        }

        public List<OutGoingCommandEnvelopeQueueItemLocal> GetByDocumentId(Guid documentId)
        {
            return _ctx.OutGoingCommandEnvelopeQueueItems.Where(n => n.DocumentId == documentId).ToList();
        }

        public List<OutGoingCommandEnvelopeQueueItemLocal> GetUnSentEnvelope()
        {
          return  _ctx.OutGoingCommandEnvelopeQueueItems.Where(s => !s.IsEnvelopeSent ).ToList();
        }

        public void MarkEnvelopeAsSent(int id)
        {
            var item = _ctx.OutGoingCommandEnvelopeQueueItems.FirstOrDefault(s => s.Id == id);
            if (item != null)
            {
                item.IsEnvelopeSent = true;
                _ctx.SaveChanges();
            }
        }

        public OutGoingCommandEnvelopeQueueItemLocal UpdateSerializedObject(int id)
        {
            OutGoingCommandEnvelopeQueueItemLocal envelopeQueueItem = GetById(id);
            CommandEnvelope envelope = JsonConvert.DeserializeObject<CommandEnvelope>(envelopeQueueItem.JsonEnvelope);
           // envelope. = commandItem.Id;
            //envelope.CommandSequence = commandItem.Id;

            envelopeQueueItem.JsonEnvelope = JsonConvert.SerializeObject(envelope, new IsoDateTimeConverter());
            Add(envelopeQueueItem);
            return GetById(id);
        }

        public OutGoingCommandEnvelopeQueueItemLocal GetFirstUnSentEnvelope()
        {
            return _ctx.OutGoingCommandEnvelopeQueueItems.FirstOrDefault(s => !s.IsEnvelopeSent);
        }

        public void Delete(int id)
        {
            //throw new NotImplementedException();
        }

        public void Add(OutGoingCommandEnvelopeQueueItemLocal itemToAdd)
        {
            OutGoingCommandEnvelopeQueueItemLocal existing = _ctx.OutGoingCommandEnvelopeQueueItems.FirstOrDefault(p => p.Id == itemToAdd.Id);
            if (existing == null)
            {
                existing = new OutGoingCommandEnvelopeQueueItemLocal();
                _ctx.OutGoingCommandEnvelopeQueueItems.Add(existing);
            }
            existing.DocumentId = itemToAdd.DocumentId;
            existing.DocumentType = itemToAdd.DocumentType;
            existing.DateInserted = itemToAdd.DateInserted;
            existing.DateSent = itemToAdd.DateSent;
            existing.DocumentId = itemToAdd.DocumentId;
            existing.IsEnvelopeSent = itemToAdd.IsEnvelopeSent;
            existing.JsonEnvelope = itemToAdd.JsonEnvelope;
            existing.EnvelopeId = itemToAdd.EnvelopeId;

            _ctx.SaveChanges();
        }

        public OutGoingCommandEnvelopeQueueItemLocal GetByEnvelopeId(Guid id)
        {
            return _ctx.OutGoingCommandEnvelopeQueueItems.FirstOrDefault(s => s.EnvelopeId == id);
        }
    }
}