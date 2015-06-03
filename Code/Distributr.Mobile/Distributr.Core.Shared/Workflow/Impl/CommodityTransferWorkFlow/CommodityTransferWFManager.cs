using System.Linq;
using Distributr.Core.ClientApp;
using Distributr.Core.Commands.CommandPackage;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityStorageCommands;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityTransferCommands;
using Distributr.Core.Domain.Transactional;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;

namespace Distributr.Core.Workflow.Impl.CommodityTransferWorkFlow
{
    public class CommodityTransferWFManager : ICommodityTransferWFManager
    {
        IOutgoingCommandEnvelopeRouter _commandEnvelopeRouter;

        public CommodityTransferWFManager(IOutgoingCommandEnvelopeRouter commandEnvelopeRouter)
        {
            _commandEnvelopeRouter = commandEnvelopeRouter;
        }


        public void SubmitChanges(CommodityTransferNote document)
        {
            int sequence = 0;
            var envelope = new CommandEnvelope();
            envelope.Initialize(document);
            var commandsToExecute = document.GetDocumentCommandsToExecute();

            var createCommand = commandsToExecute.OfType<CreateCommand>().FirstOrDefault();
            if (createCommand != null)
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, createCommand));
            var lineItemCommands = commandsToExecute.OfType<AfterCreateCommand>().ToList();
            foreach (var item in lineItemCommands)
            {
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, item));
            }

            var confirmCommodityDeliveryCommand = commandsToExecute.OfType<ConfirmCommodityTransferCommand>().FirstOrDefault();
            if (confirmCommodityDeliveryCommand != null)
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, confirmCommodityDeliveryCommand));
            var approveCommodityTransferCommand = commandsToExecute.OfType<ApproveCommodityTransferCommand>().FirstOrDefault();
            if (approveCommodityTransferCommand != null)
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, approveCommodityTransferCommand));


            var transferedCommodityStorageCommand = commandsToExecute.OfType<TransferedCommodityStorageLineItemCommand>().FirstOrDefault();
            if (transferedCommodityStorageCommand != null)
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, transferedCommodityStorageCommand));
            _commandEnvelopeRouter.RouteCommandEnvelope(envelope);

        }
    }
}
