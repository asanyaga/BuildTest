using System.Collections.Generic;
using System.Linq;
using Distributr.Core.ClientApp;
using Distributr.Core.Commands.CommandPackage;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.Commands.DocumentCommands.InventoryReceivedNotes;
using Distributr.Core.Domain.Transactional;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Workflow.Impl.AuditLogs;

namespace Distributr.Core.Workflow.Impl.IRN
{
    public class ProducerIRNWFManager : IProducerIRNWFManager
    {
        IOutgoingCommandEnvelopeRouter _commandEnvelopeRouter;
        private IAuditLogWFManager _auditLogWFManager;

        public ProducerIRNWFManager( IAuditLogWFManager auditLogWFManager, IOutgoingCommandEnvelopeRouter commandEnvelopeRouter)
        {
            _auditLogWFManager = auditLogWFManager;
            _commandEnvelopeRouter = commandEnvelopeRouter;
        }

        public void SubmitChanges(InventoryReceivedNote irn,BasicConfig config)
        {
            int sequence = 0;
            var envelope = new CommandEnvelope();
            envelope.Initialize(irn);
            List<DocumentCommand> commandsToExecute = irn.GetDocumentCommandsToExecute();
            var createCommand = commandsToExecute.OfType<CreateCommand>().First();
            envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, createCommand));
            _auditLogWFManager.AuditLogEntry("Receive Inventory", string.Format("Created Goods Received Note: {0}; for Purchase Orders: {1}", irn.Id, irn.OrderReferences));
            
            var lineItemCommands = commandsToExecute.OfType<AfterCreateCommand>();
            foreach (var li in lineItemCommands)
            {
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, li));
                AddInventoryReceivedNoteLineItemCommand c = li as AddInventoryReceivedNoteLineItemCommand;
                _auditLogWFManager.AuditLogEntry("Receive Inventory", string.Format("Received Product: {0}; Quantity: {1}; for Goods Received Note: {2};", c.ProductId, c.Qty, irn.Id));
            }

            var co = commandsToExecute.OfType<ConfirmCommand>().First();
            envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, co));
            _commandEnvelopeRouter.RouteCommandEnvelope(envelope);
            _auditLogWFManager.AuditLogEntry("Receive Inventory", string.Format("Confirmed Goods Received Note: {0}; for Purchase Orders: {1}", irn.Id, irn.OrderReferences));
        }

    }
}
