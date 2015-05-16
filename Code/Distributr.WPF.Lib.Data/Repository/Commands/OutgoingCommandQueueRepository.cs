using System;
using Distributr.Core.Commands.DocumentCommands;
using System.Collections.Generic;
using Distributr.WPF.Lib.Data.EF;
using System.Linq;
using Distributr.Core.Commands;
using Distributr.WPF.Lib.Services.Service.CommandQueues;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;


namespace Distributr.WPF.Lib.Data.Repository.Commands
{[Obsolete]
    public class OutgoingCommandQueueRepository : IOutgoingCommandQueueRepository
    {
        private DistributrLocalContext _ctx;
        


        public OutgoingCommandQueueRepository(DistributrLocalContext ctx)
        {
            _ctx = ctx;
        }

        public OutgoingCommandQueueItemLocal GetByIOD(int OID)
        {
            return _ctx.OutgoingCommandQueueItemLocals.FirstOrDefault(p => p.Id == OID);
        }

        public OutgoingCommandQueueItemLocal GetByCommandId(Guid commandId)
        {
            return _ctx.OutgoingCommandQueueItemLocals.FirstOrDefault(n => n.CommandId == commandId);
        }

        public List<OutgoingCommandQueueItemLocal> GetByDocumentId(Guid documentId)
        {
            return _ctx.OutgoingCommandQueueItemLocals.Where(n => n.DocumentId == documentId).ToList();
        }

        public void Add(OutgoingCommandQueueItemLocal itemToAdd)
        {
            OutgoingCommandQueueItemLocal existing = _ctx.OutgoingCommandQueueItemLocals.FirstOrDefault(p => p.Id == itemToAdd.Id);
            if (existing == null)
            {
                existing = new OutgoingCommandQueueItemLocal();
                _ctx.OutgoingCommandQueueItemLocals.Add(existing);
            }
            existing.CommandId = itemToAdd.CommandId;
            existing.CommandType = itemToAdd.CommandType;
            existing.DateInserted = itemToAdd.DateInserted;
            existing.DateSent = itemToAdd.DateSent;
            existing.DocumentId = itemToAdd.DocumentId;
            existing.IsCommandSent = itemToAdd.IsCommandSent;
            existing.JsonCommand = itemToAdd.JsonCommand;
           
            _ctx.SaveChanges();
        }

        public void DropType()
        {
            //siaqodb.DropType<OutgoingCommandQueueItemLocal>();
            throw  new NotImplementedException();
        }

        public List<OutgoingCommandQueueItemLocal> GetAll()
        {
             return _ctx.OutgoingCommandQueueItemLocals.ToList();
           
        }
        public List<OutgoingCommandQueueItemLocal> GetUnSentCommands()
        {
            var all =_ctx.OutgoingCommandQueueItemLocals
                .Where(n => n.IsCommandSent == false).ToList();
            return all;
        }

        public void MarkCommandAsSent(int OID)
        {
            OutgoingCommandQueueItemLocal c = GetByIOD(OID);
           
            if (c == null) return;
            c.IsCommandSent = true;
            c.DateSent = DateTime.Now;
            Add(c);

        }

        public OutgoingCommandQueueItemLocal UpdateSerializedObjectWithOID(int commandOID)
        {
            OutgoingCommandQueueItemLocal commandItem = GetByIOD(commandOID);
            ICommand command = JsonConvert.DeserializeObject<DocumentCommand>(commandItem.JsonCommand);
            command.CostCentreApplicationCommandSequenceId = commandItem.Id;
            command.CommandSequence = commandItem.Id;

            commandItem.JsonCommand = JsonConvert.SerializeObject(command, new IsoDateTimeConverter());
            Add(commandItem);
            return GetByIOD(commandOID);
        }

        public OutgoingCommandQueueItemLocal GetFirstUnSentCommand()
        {
           return _ctx.OutgoingCommandQueueItemLocals.FirstOrDefault(n => n.IsCommandSent == false);
        }

        public void DeleteOldCommand(int OID)
        {
           //var cmd=  _ctx.OutgoingCommandQueueItemLocals.FirstOrDefault(p=>p.Id==)
            //throw new NotImplementedException();
        }
    }
}
