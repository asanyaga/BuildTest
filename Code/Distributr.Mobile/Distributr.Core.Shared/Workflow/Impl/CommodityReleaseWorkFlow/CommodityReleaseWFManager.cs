using System.Linq;
using Distributr.Core.ClientApp;
using Distributr.Core.Commands.CommandPackage;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityReleaseCommands;
using Distributr.Core.Domain.Transactional;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;

namespace Distributr.Core.Workflow.Impl.CommodityReleaseWorkFlow
{
    public class CommodityReleaseWFManager : ICommodityReleaseWFManager
    {
        IOutgoingCommandEnvelopeRouter _commandEnvelopeRouter;

        public CommodityReleaseWFManager(IOutgoingCommandEnvelopeRouter commandEnvelopeRouter)
        {
            _commandEnvelopeRouter = commandEnvelopeRouter;
        }


        public void SubmitChanges(CommodityReleaseNote document)
        {
            int sequence = 0;
            var envelope = new CommandEnvelope();
            envelope.Initialize(document);
            var commandsToExecute = document.GetDocumentCommandsToExecute();

            var createCommand = commandsToExecute.OfType<CreateCommand>().FirstOrDefault();
            if (createCommand != null)
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, createCommand));
               // _commandRouter.RouteDocumentCommand(createCommand);
            var lineItemCommands = commandsToExecute.OfType<AfterCreateCommand>().ToList();
            foreach (var item in lineItemCommands)
            {
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, item));
               // _commandRouter.RouteDocumentCommand(item);
            }

            var confirmCommodityReleaseCommand = commandsToExecute.OfType<ConfirmCommodityReleaseNoteCommand>().FirstOrDefault();
            if (confirmCommodityReleaseCommand != null)
               // _commandRouter.RouteDocumentCommand(confirmCommodityReleaseCommand);
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, confirmCommodityReleaseCommand));
            _commandEnvelopeRouter.RouteCommandEnvelope(envelope);

        }
    }
}