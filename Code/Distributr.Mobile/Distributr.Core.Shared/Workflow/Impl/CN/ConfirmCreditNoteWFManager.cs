using System.Collections.Generic;
using System.Linq;
using Distributr.Core.ClientApp;
using Distributr.Core.Commands.CommandPackage;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.Commands.DocumentCommands.CreditNotes;
using Distributr.Core.Domain.Transactional;
using Distributr.Core.Domain.Transactional.DocumentEntities;

namespace Distributr.Core.Workflow.Impl.CN
{
    public class ConfirmCreditNoteWFManager : IConfirmCreditNoteWFManager
    {
        IOutgoingCommandEnvelopeRouter _commandEnvelopeRouter;
        
        public ConfirmCreditNoteWFManager(IOutgoingCommandEnvelopeRouter commandEnvelopeRouter)
        {
            _commandEnvelopeRouter = commandEnvelopeRouter;
        }

        public void SubmitChanges(CreditNote document, BasicConfig config)
        {
            int sequence = 0;
            var envelope = new CommandEnvelope();
            envelope.Initialize(document);
            List<DocumentCommand> commandsToExecute = document.GetDocumentCommandsToExecute();

            CreateCommand createCommand= commandsToExecute.OfType<CreateCommand>().FirstOrDefault();
            if (createCommand != null)
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, createCommand));
               // _commandRouter.RouteDocumentCommand(createCommand);
            List<AfterCreateCommand> lineItemCommands = commandsToExecute.OfType<AfterCreateCommand>().ToList();
            foreach (var item in lineItemCommands)
            {
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, item));
                //_commandRouter.RouteDocumentCommand(item);
                
            }

            ConfirmCreditNoteCommand confirmCreditNoteCommand = commandsToExecute.OfType<ConfirmCreditNoteCommand>().FirstOrDefault();
            if (confirmCreditNoteCommand != null)
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, confirmCreditNoteCommand));

               // _commandRouter.RouteDocumentCommand(confirmCreditNoteCommand);
            _commandEnvelopeRouter.RouteCommandEnvelope(envelope);
           
        }
    }
}
