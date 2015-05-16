using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Commands;
using Distributr.Core.Commands.CommandPackage;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.Commands.DocumentCommands.Orders;
using Distributr.Core.Domain.Transactional;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Domain.Transactional.DocumentEntities.OrderDocumentEntities;
using Distributr.Core.Workflow;
using Distributr.WSAPI.Lib.Services.Bus;
using Distributr.WSAPI.Lib.Services.CommandAudit;
using Newtonsoft.Json;

namespace Distributr.HQ.Lib.Workflows.PurchaseOrder
{
    public class PurchaseOrderHqWorkflow : IPurchaseOrderWorkflow
    {
        private IBusPublisher _busPublisher;
        private ICommandEnvelopeProcessingAuditRepository _commandProcessingAuditRepository;

        public PurchaseOrderHqWorkflow(IBusPublisher busPublisher, ICommandEnvelopeProcessingAuditRepository commandProcessingAuditRepository)
        {
            _busPublisher = busPublisher;
            _commandProcessingAuditRepository = commandProcessingAuditRepository;
        }

        public void Submit(MainOrder order)
        {
            int sequence = 0;
            var envelope = new CommandEnvelope();
            envelope.GeneratedByCostCentreApplicationId = Guid.Empty;
            
            envelope.Initialize(order);
            List<DocumentCommand> commandsToExecute = order.GetSubOrderCommandsToExecute();
            var createCommand = commandsToExecute.OfType<CreateCommand>().FirstOrDefault();
            if (createCommand != null)
            {
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, createCommand));
                //AddToMongoDB(createCommand);
                
            }

            var lineItemCommands = commandsToExecute.OfType<AfterCreateCommand>();
            foreach (var _item in lineItemCommands)
            {
                  envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, _item));
                //AddToMongoDB(_item);
                //_busPublisher.WrapAndPublish(_item, (CommandType)Enum.Parse(typeof(CommandType), _item.CommandTypeRef));
              
               
            }
            var editlineItemCommands = commandsToExecute.OfType<ChangeMainOrderLineItemCommand>();
            foreach (var _editeditem in editlineItemCommands)
            {
                                  envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, _editeditem));

              
            }
            var removedLineitem = commandsToExecute.OfType<RemoveMainOrderLineItemCommand>();
            foreach (var _removeditem in removedLineitem)
            {
                   envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, _removeditem));

                
            }
            var co = commandsToExecute.OfType<ConfirmCommand>().FirstOrDefault();
            if (co != null)
            {
                 envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, co));

            }

            var rco = commandsToExecute.OfType<RejectMainOrderCommand>().FirstOrDefault();
            if (rco != null)
            {
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, rco));

            }

            var approvedlineItemCommands = order.GetSubOrderCommandsToExecute().OfType<ApproveOrderLineItemCommand>();
            foreach (var _item in approvedlineItemCommands)
            {
                  envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, _item));

            }
            var aop = commandsToExecute.OfType<ApproveCommand>().FirstOrDefault();
            if (aop != null)
            {
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, aop));
     
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
            _commandProcessingAuditRepository.AddCommand(envelopeProcessingAudit);
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
