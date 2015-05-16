using System.Collections.Generic;
using System.Linq;
using Distributr.Core.ClientApp;
using Distributr.Core.Commands.CommandPackage;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityDeliveryCommands;
using Distributr.Core.Domain.Transactional;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;

namespace Distributr.Core.Workflow.Impl.CommodityDeliveryWorkflow
{
    public class ReceivedDeliveryWorkflow : IReceivedDeliveryWorkflow
    {
        private IOutgoingCommandEnvelopeRouter _commandEnvelopeRouter;
        public ReceivedDeliveryWorkflow(IOutgoingCommandEnvelopeRouter commandEnvelopeRouter)
        {
            _commandEnvelopeRouter = commandEnvelopeRouter;
        }

        public void SubmitChanges(ReceivedDeliveryNote document)
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
                //_commandRouter.RouteDocumentCommand(item);

            }
            List<ApproveCommand> stored = commandsToExecute.OfType<ApproveCommand>().ToList();
            foreach (var item in stored)
            {
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, item));

               // _commandRouter.RouteDocumentCommand(item);

            }
            ConfirmReceivedDeliveryCommand confirmReceivedDeliveryCommand = commandsToExecute.OfType<ConfirmReceivedDeliveryCommand>().FirstOrDefault();
            if (confirmReceivedDeliveryCommand != null)
                //_commandRouter.RouteDocumentCommand(confirmReceivedDeliveryCommand);
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, confirmReceivedDeliveryCommand));
            _commandEnvelopeRouter.RouteCommandEnvelope(envelope);

        }
    }
}
