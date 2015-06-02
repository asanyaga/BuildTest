using System.Collections.Generic;
using Distributr.Core.ClientApp;
using Distributr.Core.Commands.CommandPackage;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.Commands.DocumentCommands.Invoices;
using Distributr.Core.Domain.Transactional;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Workflow.Impl.AuditLogs;

namespace Distributr.Core.Workflow.Impl.Invoice
{
    public class ConfirmInvoiceWorkFlowManager : BaseWFManager,  IConfirmInvoiceWorkFlowManager
    {
        IOutgoingCommandEnvelopeRouter _commandEnvelopeRouter;
        private IAuditLogWFManager _auditLogWFManager;


        public ConfirmInvoiceWorkFlowManager(IOutgoingCommandEnvelopeRouter commandEnvelopeRouter, IAuditLogWFManager auditLogWfManager)
        {
            _commandEnvelopeRouter = commandEnvelopeRouter;
            _auditLogWFManager = auditLogWfManager;
        }

        public void SubmitChanges(Domain.Transactional.DocumentEntities.Invoice document, BasicConfig config)
        {
            int sequence = 0;
            var envelope = new CommandEnvelope();
            envelope.Initialize(document);
            List<DocumentCommand> commandsToExecute = document.GetDocumentCommandsToExecute();
            
            CreateCommand createCommand; //= commandsToExecute.OfType<CreateCommand>().First();
            if (TryGetCreateCommand(commandsToExecute, out createCommand))
            {
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, createCommand));
                //_commandRouter.RouteDocumentCommand(createCommand);
                _auditLogWFManager.AuditLogEntry("Invoice",
                                                 string.Format("Created Invoice: {0}; for Order: {1}", document.Id,
                                                               document.OrderId));
            }
            List<AfterCreateCommand> lineItemCommands;
            if (TryGetAfterCreateCommands(commandsToExecute, out lineItemCommands))
            {
                foreach (var item in lineItemCommands)
                {
                    envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, item));
                  //  _commandRouter.RouteDocumentCommand(item);
                    var _item = item as AddInvoiceLineItemCommand;
                    _auditLogWFManager.AuditLogEntry("Invoice",
                                                     string.Format(
                                                         "Added Product: {0}; Quantity: {1}; for Invoice: {2};",
                                                         _item.ProductId, _item.Qty, document.Id));
                }
            }
            ConfirmCommand confirmCommand;
            if (TryGetConfirmCommand(commandsToExecute, out confirmCommand))
            {
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, confirmCommand));
               // _commandRouter.RouteDocumentCommand(confirmCommand);
                //send notification
              //  _notifyService.SubmitInvoiceNotification(document);
                _auditLogWFManager.AuditLogEntry("Invoice",
                                                 string.Format("Confirmed Invoice: {0}; for Order: {1}", document.Id,
                                                               document.OrderId));
            }
            _commandEnvelopeRouter.RouteCommandEnvelope(envelope);
            document.CallClearCommands();
        }

     
    }
}
