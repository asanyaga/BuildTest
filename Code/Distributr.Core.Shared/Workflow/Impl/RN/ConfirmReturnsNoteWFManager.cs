using System.Collections.Generic;
using System.Linq;
using Distributr.Core.ClientApp;
using Distributr.Core.Commands.CommandPackage;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.Commands.DocumentCommands.InventoryTransferNotes;
using Distributr.Core.Commands.DocumentCommands.ReturnsNotes;
using Distributr.Core.Domain.Transactional;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Workflow.Impl.AuditLogs;

namespace Distributr.Core.Workflow.Impl.RN
{
    public class ConfirmReturnsNoteWFManager : IConfirmReturnsNoteWFManager
    {
        
        IOutgoingCommandEnvelopeRouter _commandEnvelopeRouter;
       
      
        private IAuditLogWFManager _auditLogWFManager;

        public ConfirmReturnsNoteWFManager(IOutgoingCommandEnvelopeRouter commandEnvelopeRouter, IAuditLogWFManager auditLogWfManager)
        {
            _commandEnvelopeRouter = commandEnvelopeRouter;
            _auditLogWFManager = auditLogWfManager;
        }


        public void SubmitChanges(ReturnsNote document,BasicConfig config)
        {
            int sequence = 0;
            var envelope = new CommandEnvelope();
            envelope.Initialize(document);
            List<DocumentCommand> commandsToExecute = document.GetDocumentCommandsToExecute();
            var createCommand = commandsToExecute.OfType<CreateCommand>().FirstOrDefault();
            if (createCommand != null)
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, createCommand));
            //_commandRouter.RouteDocumentCommand(createCommand);
            _auditLogWFManager.AuditLogEntry("Return Note", string.Format("Created Return Note: {0}; From: {1}; To: {2}", document.Id, document.DocumentIssuerCostCentre.Name, document.DocumentRecipientCostCentre.Name));

            var lineItemCommands = commandsToExecute.OfType<AfterCreateCommand>();
            foreach (var _item in lineItemCommands)
            {
                var item = _item as AddInventoryTransferNoteLineItemCommand;
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, item));
                // _commandRouter.RouteDocumentCommand(_item);
                _auditLogWFManager.AuditLogEntry("Inventory Transfer", string.Format("Return Note: {0}; Quantity: {1}; for Return Note: {2};", item.ProductId, item.Qty, document.Id));
            }
            var co = commandsToExecute.OfType<ConfirmCommand>().FirstOrDefault();
            if (co != null)
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, co));
            //_commandRouter.RouteDocumentCommand(co);
            var clo = commandsToExecute.OfType<CloseReturnsNoteCommand>().FirstOrDefault();
            if (clo != null)
            {
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, clo));
            }
            _commandEnvelopeRouter.RouteCommandEnvelope(envelope);
            _auditLogWFManager.AuditLogEntry("Return Note", string.Format("Confirmed IReturn Note: {0}; From: {1}; To: {2}", document.Id, document.DocumentIssuerCostCentre.Name, document.DocumentRecipientCostCentre.Name));
     
        }

       
    }
}
