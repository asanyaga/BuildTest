using System.Collections.Generic;
using System.Linq;
using Distributr.Core.ClientApp;
using Distributr.Core.Commands.CommandPackage;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityReceptionCommands;
using Distributr.Core.Domain.Transactional;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;

namespace Distributr.Core.Workflow.Impl.CommodityReceptionWorkFlow
{
    public class CommodityReceptionWFManager : ICommodityReceptionWFManager
    {
        IOutgoingCommandEnvelopeRouter _commandEnvelopeRouter;
        
        public CommodityReceptionWFManager( IOutgoingCommandEnvelopeRouter commandEnvelopeRouter)
        {
          
            _commandEnvelopeRouter = commandEnvelopeRouter;
        }

        
        public void SubmitChanges(CommodityReceptionNote document)
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
            List<StoredCommodityReceptionLineItemCommand> stored = commandsToExecute.OfType<StoredCommodityReceptionLineItemCommand>().ToList();
            foreach (var item in stored)
            {
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, item));

                //_commandRouter.RouteDocumentCommand(item);
            }
            ConfirmCommodityReceptionCommand confirmCommand = commandsToExecute.OfType<ConfirmCommodityReceptionCommand>().FirstOrDefault();
            if (confirmCommand != null)
                //_commandRouter.RouteDocumentCommand(confirmCommand);
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, confirmCommand));
            _commandEnvelopeRouter.RouteCommandEnvelope(envelope);


        }

       
    }
}
