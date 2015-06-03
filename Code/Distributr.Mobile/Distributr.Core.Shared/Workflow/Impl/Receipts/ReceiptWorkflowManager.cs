using System.Collections.Generic;
using Distributr.Core.ClientApp;
using Distributr.Core.Commands.CommandPackage;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.Commands.DocumentCommands.Receipts;
using Distributr.Core.Domain.Transactional;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Workflow.Impl.AuditLogs;

namespace Distributr.Core.Workflow.Impl.Receipts
{
    public class ReceiptWorkflowManager : BaseWFManager, IReceiptWorkFlowManager
    {
        IOutgoingCommandEnvelopeRouter _commandEnvelopeRouter;
        private IAuditLogWFManager _auditLogWFManager;

        public ReceiptWorkflowManager(IOutgoingCommandEnvelopeRouter commandEnvelopeRouter, IAuditLogWFManager auditLogWfManager)
        {
            _commandEnvelopeRouter = commandEnvelopeRouter;
            _auditLogWFManager = auditLogWfManager;
        }

        public void SubmitChanges(Receipt document,BasicConfig config)
        {
            int sequence = 0;
            var envelope = new CommandEnvelope();
            envelope.Initialize(document);
            List<DocumentCommand> commandsToExecute = document.GetDocumentCommandsToExecute();

            CreateCommand createCommand; //= commandsToExecute.OfType<CreateCommand>().First();
            if (TryGetCreateCommand(commandsToExecute, out createCommand))
            {
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, createCommand));
               // _commandRouter.RouteDocumentCommand(createCommand);
                _auditLogWFManager.AuditLogEntry("Receipt",
                                                 string.Format("Created Receipt: {0}; for invoice: {1}", document.Id,
                                                               document.InvoiceId));
            }
            List<AfterCreateCommand> lineItemCommands;
            if (TryGetAfterCreateCommands(commandsToExecute, out lineItemCommands))
            {
                foreach (var item in lineItemCommands)
                {
                    envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, item));
                  //  _commandRouter.RouteDocumentCommand(item);
                    var _item = item as AddReceiptLineItemCommand;
                    _auditLogWFManager.AuditLogEntry("Receipt",
                                                     string.Format(
                                                         "Added Product type: {0}; Quantity: {1}; for Invoice: {2};",
                                                         _item.LineItemType, _item.Value, document.InvoiceId));
                }
            }
            ConfirmCommand confirmCommand;
            if (TryGetConfirmCommand(commandsToExecute, out confirmCommand))
            {
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, confirmCommand));
               // _commandRouter.RouteDocumentCommand(confirmCommand);
                _auditLogWFManager.AuditLogEntry("Receipt",
                                                 string.Format("Confirmed Receipt: {0}; for Invoice: {1}", document.Id,
                                                               document.InvoiceId));
               // _notifyService.SubmitRecieptNotification(document);

            }
            _commandEnvelopeRouter.RouteCommandEnvelope(envelope);
            document.CallClearCommands();
        }
    }
}
