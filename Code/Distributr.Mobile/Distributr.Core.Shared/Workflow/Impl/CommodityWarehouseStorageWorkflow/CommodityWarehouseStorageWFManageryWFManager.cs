using System.Collections.Generic;
using System.Linq;
using Distributr.Core.ClientApp;
using Distributr.Core.Commands.CommandPackage;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityWarehouseStorageCommands;
using Distributr.Core.Domain.Transactional;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;

namespace Distributr.Core.Workflow.Impl.CommodityWarehouseStorageWorkflow
{
    public class CommodityWarehouseStorageWFManager : ICommodityWarehouseStorageWFManager
    {
        IOutgoingCommandEnvelopeRouter _commandEnvelopeRouter;

        public CommodityWarehouseStorageWFManager(IOutgoingCommandEnvelopeRouter commandEnvelopeRouter)
        {
            _commandEnvelopeRouter = commandEnvelopeRouter;
        }


        public void SubmitChanges(CommodityWarehouseStorageNote document)
        {
            int sequence = 0;
            var envelope = new CommandEnvelope();
            envelope.Initialize(document);
            List<DocumentCommand> commandsToExecute = document.GetDocumentCommandsToExecute();

            CreateCommand createCommand = commandsToExecute.OfType<CreateCommand>().FirstOrDefault();
             if (createCommand != null)
                 envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, createCommand));
            List<AfterCreateCommand> lineItemCommands = commandsToExecute.OfType<AfterCreateCommand>().ToList();
            foreach (var item in lineItemCommands)
            {
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, item));

            }

            List<AfterConfirmCommand> lineItemConfirmCommands = commandsToExecute.OfType<AfterConfirmCommand>().ToList();
            foreach (var item in lineItemConfirmCommands)
            {
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, item));


            }

            ConfirmCommodityWarehouseStorageCommand confirmCommodityWarehouseStorageCommand = commandsToExecute.OfType<ConfirmCommodityWarehouseStorageCommand>().FirstOrDefault();
            if (confirmCommodityWarehouseStorageCommand != null)
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, confirmCommodityWarehouseStorageCommand));


            ApproveCommodityWarehouseStorageCommand approveCommand = commandsToExecute.OfType<ApproveCommodityWarehouseStorageCommand>().FirstOrDefault();
            if (approveCommand != null)
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, approveCommand));


            StoreCommodityWarehouseStorageCommand storeCommand = commandsToExecute.OfType<StoreCommodityWarehouseStorageCommand>().FirstOrDefault();
            if (storeCommand != null)
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, storeCommand));

            _commandEnvelopeRouter.RouteCommandEnvelope(envelope);


        }
    }
}
