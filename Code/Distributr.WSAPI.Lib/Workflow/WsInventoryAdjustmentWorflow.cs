using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.Commands.CommandPackage;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.Domain.Transactional;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Workflow;
using Distributr.WSAPI.Lib.Services.Bus;
using Distributr.WSAPI.Lib.Services.CommandAudit;
using Newtonsoft.Json;

/* ----  May2015_Notes -----------
    Inventory import, see newintegrations controler
 */
namespace Distributr.WSAPI.Lib.Workflow
{
    public class WsInventoryAdjustmentWorflow : IWsInventoryAdjustmentWorflow
    {
        private IBusPublisher _busPublisher;
        private ICommandEnvelopeProcessingAuditRepository _commandEnvelopeProcessingAuditRepository;

        public WsInventoryAdjustmentWorflow(ICommandEnvelopeProcessingAuditRepository commandEnvelopeProcessingAuditRepository, IBusPublisher busPublisher)
        {
            _commandEnvelopeProcessingAuditRepository = commandEnvelopeProcessingAuditRepository;
            _busPublisher = busPublisher;
        }


        public void Submit(InventoryAdjustmentNote note)
        {
            int sequence = 0;
            var envelope = new CommandEnvelope();
            envelope.Initialize(note);
            List<DocumentCommand> commandsToExecute = note.GetDocumentCommandsToExecute();
            var createCommand = commandsToExecute.OfType<CreateCommand>().First();
           
            if (createCommand != null)
            {
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, createCommand));
            }
            var lineItemCommands = commandsToExecute.OfType<AfterCreateCommand>();
            foreach (var _item in lineItemCommands)
            {
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, _item));
            }
            var co = commandsToExecute.OfType<ConfirmCommand>().First();
            if (co != null)
            {
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, co));
            }
            
            AddToMongoDB(envelope);
        }

        public void Submit(InventoryTransferNote note)
        {
            int sequence = 0;
            var envelope = new CommandEnvelope();
            envelope.Initialize(note);
            List<DocumentCommand> commandsToExecute = note.GetDocumentCommandsToExecute();
            var createCommand = commandsToExecute.OfType<CreateCommand>().FirstOrDefault();
            if (createCommand != null)
            {
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, createCommand));
            }
            var lineItemCommands = commandsToExecute.OfType<AfterCreateCommand>();
            foreach (var _item in lineItemCommands)
            {
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, _item));
            }
            var co = commandsToExecute.OfType<ConfirmCommand>().First();
            if(co !=null)
            {
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, co));
            }
            AddToMongoDB(envelope);
        }


       
       private void AddToMongoDB(CommandEnvelope envelope)
        {
            envelope.EnvelopeArrivedAtServerTick = DateTime.Now.Ticks;

            var envelopeProcessingAudit = new CommandEnvelopeProcessingAudit
            {
                GeneratedByCostCentreApplicationId =Guid.Empty,

                DateInserted = DateTime.Now,
                Id = envelope.Id,
                JsonEnvelope = JsonConvert.SerializeObject(envelope),
                RetryCounter = 0,
                Status = EnvelopeProcessingStatus.OnQueue,
               
                DocumentId = envelope.DocumentId,
                ParentDocumentId = envelope.ParentDocumentId,
                DocumentType = (DocumentType)envelope.DocumentTypeId,
                EnvelopeGeneratedTick = envelope.EnvelopeGeneratedTick,
                EnvelopeArrivalAtServerTick = DateTime.Now.Ticks,
                EnvelopeProcessOnServerTick = 0,
                GeneratedByCostCentreId = envelope.GeneratedByCostCentreId,
                RecipientCostCentreId = envelope.RecipientCostCentreId,
                LastExecutedCommand = 0,
                NumberOfCommand = envelope.CommandsList.Count

            };
            envelopeProcessingAudit.DocumentTypeName = envelopeProcessingAudit.DocumentType.ToString();
            _commandEnvelopeProcessingAuditRepository.AddCommand(envelopeProcessingAudit);
            var message = new EnvelopeBusMessage
            {
                DocumentTypeId = envelope.DocumentTypeId,
                MessageId = envelope.Id,
                BodyJson = JsonConvert.SerializeObject(envelope),
                SendDateTime = DateTime.Now.ToString(),
                
            };
            _busPublisher.Publish(message);
        }
    }
}
