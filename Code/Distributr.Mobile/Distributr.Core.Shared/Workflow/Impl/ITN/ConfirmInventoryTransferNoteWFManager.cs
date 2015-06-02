using System.Collections.Generic;
using System.Linq;
using Distributr.Core.ClientApp;
using Distributr.Core.Commands.CommandPackage;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.Commands.DocumentCommands.InventoryTransferNotes;
using Distributr.Core.Domain.Transactional;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Workflow.Impl.AuditLogs;

namespace Distributr.Core.Workflow.Impl.ITN
{
    public class ConfirmInventoryTransferNoteWFManager : IConfirmInventoryTransferNoteWFManager
    {
        
        private IAuditLogWFManager _auditLogWFManager;
        IOutgoingCommandEnvelopeRouter _commandEnvelopeRouter;

        public ConfirmInventoryTransferNoteWFManager(
            
            IAuditLogWFManager auditLogWFManager, IOutgoingCommandEnvelopeRouter commandEnvelopeRouter)
        {
            
            _auditLogWFManager = auditLogWFManager;
            _commandEnvelopeRouter = commandEnvelopeRouter;
        }

        public void SubmitChanges(InventoryTransferNote document, BasicConfig config)
        {

            int sequence = 0;
            var envelope = new CommandEnvelope();
            envelope.Initialize(document);
            List<DocumentCommand> commandsToExecute = document.GetDocumentCommandsToExecute();
            var createCommand = commandsToExecute.OfType<CreateCommand>().First();
            envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, createCommand));
            //_commandRouter.RouteDocumentCommand(createCommand);
            _auditLogWFManager.AuditLogEntry("Inventory Transfer", string.Format("Created Inventory Transfer: {0}; From: {1}; To: {2}", document.Id, document.DocumentIssuerCostCentre.Name, document.DocumentRecipientCostCentre.Name));

            var lineItemCommands = commandsToExecute.OfType<AfterCreateCommand>();
            foreach (var _item in lineItemCommands)
            {
                var item = _item as AddInventoryTransferNoteLineItemCommand;
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, item));
               // _commandRouter.RouteDocumentCommand(_item);
                _auditLogWFManager.AuditLogEntry("Inventory Transfer", string.Format("Transferred Product: {0}; Quantity: {1}; for Inventory Transfer: {2};", item.ProductId, item.Qty, document.Id));
            }
            var co = commandsToExecute.OfType<ConfirmCommand>().First();
            envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, co));
            //_commandRouter.RouteDocumentCommand(co);
            _commandEnvelopeRouter.RouteCommandEnvelope(envelope);
            _auditLogWFManager.AuditLogEntry("Inventory Transfer", string.Format("Confirmed Inventory Transfer: {0}; From: {1}; To: {2}", document.Id, document.DocumentIssuerCostCentre.Name, document.DocumentRecipientCostCentre.Name));
        }

        //REFACTOR - get rid of this
       
    }
}
