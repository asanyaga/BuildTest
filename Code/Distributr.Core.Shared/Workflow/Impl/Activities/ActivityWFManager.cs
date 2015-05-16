using System.Collections.Generic;
using System.Linq;
using Distributr.Core.ClientApp;
using Distributr.Core.Commands.CommandPackage;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.Domain.Transactional;
using Distributr.Core.Domain.Transactional.ActivityDocumentEntities;

namespace Distributr.Core.Workflow.Impl.Activities
{
    public interface IActivityWFManager: IActivityDocumentWFManager<ActivityDocument>
    {
        
    }
    public class ActivityWFManager : IActivityWFManager
    {
        IOutgoingCommandEnvelopeRouter _commandEnvelopeRouter;
        private IAuditLogsWFManager _auditLogWFManager;

        public ActivityWFManager( IAuditLogsWFManager auditLogWfManager, IOutgoingCommandEnvelopeRouter commandEnvelopeRouter)
        {
            
            _auditLogWFManager = auditLogWfManager;
            _commandEnvelopeRouter = commandEnvelopeRouter;
        }

        public void SubmitChanges(ActivityDocument document)
        {
            int sequence = 0;
            var envelope = new CommandEnvelope();
            envelope.Initialize(document);
            List<DocumentCommand> commandsToExecute = document.GetDocumentCommandsToExecute();

            CreateCommand createCommand = commandsToExecute.OfType<CreateCommand>().FirstOrDefault();
            if (createCommand != null)
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, createCommand));
               // _commandRouter.RouteDocumentCommand(createCommand);
            List<AfterCreateCommand> lineItemCommands = commandsToExecute.OfType<AfterCreateCommand>().ToList();
            foreach (var item in lineItemCommands)
            {
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, item));
               // _commandRouter.RouteDocumentCommand(item);

            }
            var confirmCommand = commandsToExecute.OfType<ConfirmCommand>().FirstOrDefault();
            if (confirmCommand != null)
            {
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, confirmCommand));

               // _commandRouter.RouteDocumentCommand(confirmCommand);
               
            }
            _commandEnvelopeRouter.RouteCommandEnvelope(envelope);
            _auditLogWFManager.AuditLogEntry("Activity", "hahahaha " + document.DocumentReference + "; id " + document.Id + " with ");

        }
    }
}
