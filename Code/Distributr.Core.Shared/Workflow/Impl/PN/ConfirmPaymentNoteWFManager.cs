using System.Collections.Generic;
using System.Linq;
using Distributr.Core.ClientApp;
using Distributr.Core.Commands.CommandPackage;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.Domain.Transactional;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Workflow.Impl.AuditLogs;

namespace Distributr.Core.Workflow.Impl.PN
{
    public class ConfirmPaymentNoteWFManager : IConfirmPaymentNoteWFManager
    {
        private IOutgoingCommandEnvelopeRouter _commandEnvelopeRouter;
      
        

        private IAuditLogWFManager _auditLogWFManager;

        public ConfirmPaymentNoteWFManager(IOutgoingCommandEnvelopeRouter commandEnvelopeRouter,  IAuditLogWFManager auditLogWfManager)
        {
            _commandEnvelopeRouter = commandEnvelopeRouter;
          
            _auditLogWFManager = auditLogWfManager;
        }


        public void SubmitChanges(PaymentNote document, BasicConfig config)
        {
            int sequence = 0;
            var envelope = new CommandEnvelope();
            envelope.Initialize(document);
            List<DocumentCommand> commandsToExecute = document.GetDocumentCommandsToExecute();
            var createCommand = commandsToExecute.OfType<CreateCommand>().FirstOrDefault();
            if (createCommand != null)
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, createCommand));
            _auditLogWFManager.AuditLogEntry("PaymentNote", string.Format("Created PaymentNote: {0}; From: {1}; To: {2}", document.Id, document.DocumentIssuerCostCentre.Name, document.DocumentRecipientCostCentre.Name));

            var lineItemCommands = commandsToExecute.OfType<AfterCreateCommand>();
            foreach (var _item in lineItemCommands)
            {

                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, _item));
              
            }
            var co = commandsToExecute.OfType<ConfirmCommand>().FirstOrDefault();
            if (co != null)
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, co));
          
            _commandEnvelopeRouter.RouteCommandEnvelope(envelope);
            _auditLogWFManager.AuditLogEntry("PaymentNote", string.Format("Confirmed PaymentNote: {0}; From: {1}; To: {2}", document.Id, document.DocumentIssuerCostCentre.Name, document.DocumentRecipientCostCentre.Name));
      
        }
    }
}
