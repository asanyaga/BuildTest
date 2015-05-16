using System.Collections.Generic;
using Distributr.Core.ClientApp;
using Distributr.Core.Commands.CommandPackage;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.Commands.DocumentCommands.DispatchNotes;
using Distributr.Core.Domain.Transactional;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Notifications;
using Distributr.Core.Workflow.Impl.AuditLogs;

namespace Distributr.Core.Workflow.Impl.DN
{
    public class ConfirmDispatchNoteWFManager : BaseWFManager, IConfirmDispatchNoteWFManager
    {
        IOutgoingCommandEnvelopeRouter _commandEnvelopeRouter;
        private IAuditLogWFManager _auditLogWFManager;

        public ConfirmDispatchNoteWFManager(IOutgoingCommandEnvelopeRouter commandEnvelopeRouter, IAuditLogWFManager auditLogWfManager)
        {
            _commandEnvelopeRouter = commandEnvelopeRouter;
            _auditLogWFManager = auditLogWfManager;
        }


        public void SubmitChanges(DispatchNote document, BasicConfig config)
        {
            //document.Confirm();
            int sequence = 0;
            var envelope = new CommandEnvelope();
            envelope.Initialize(document);
            
            List<DocumentCommand> commandsToExecute = document.GetDocumentCommandsToExecute();
            
            CreateCommand createCommand;
            if (TryGetCreateCommand(commandsToExecute, out createCommand))
            {
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, createCommand));
               // _commandRouter.RouteDocumentCommand(createCommand);
                _auditLogWFManager.AuditLogEntry("Dispatch Note", 
                    string.Format("Dispatch Note Document No: {0}; to: {1}; Created", document.Id, document.DocumentRecipientCostCentre.Name));
            }

            List<AfterCreateCommand> lineItemCommands;
            if (TryGetAfterCreateCommands(commandsToExecute, out lineItemCommands))
            {
                foreach (var item in lineItemCommands)
                {
                    envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, item));
                    //_commandRouter.RouteDocumentCommand(item);
                    var _item = item as AddDispatchNoteLineItemCommand;
                    _auditLogWFManager.AuditLogEntry("Dispatch Note", 
                        string.Format("Added Product: {1}; Quantity: {2}; Value: {3}; to Dispatch Note document: {0}", document.Id, item.Description, _item.Qty, _item.Value));
                }
            }
            ConfirmCommand confirmCommand;
            if (TryGetConfirmCommand(commandsToExecute, out confirmCommand))
            {
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, confirmCommand));

                //_commandRouter.RouteDocumentCommand(confirmCommand);
                _auditLogWFManager.AuditLogEntry("Dispatch Note", string.Format("Confirmed Dispatch Note document: {0}", document.Id));
               // _notifyService.SubmitDispatch(document);
            }
            _commandEnvelopeRouter.RouteCommandEnvelope(envelope);
            document.CallClearCommands();
            
        }

        
       
    }
}
