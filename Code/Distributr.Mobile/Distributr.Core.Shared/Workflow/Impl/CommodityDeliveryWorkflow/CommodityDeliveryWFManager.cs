using System.Collections.Generic;
using System.Linq;
using Distributr.Core.ClientApp;
using Distributr.Core.Commands.CommandPackage;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityDeliveryCommands;
using Distributr.Core.Domain.Transactional;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.Core.Workflow.Impl.AuditLogs;

namespace Distributr.Core.Workflow.Impl.CommodityDeliveryWorkflow
{
    public class CommodityDeliveryWFManager : ICommodityDeliveryWFManager
    {
        IOutgoingCommandEnvelopeRouter _commandEnvelopeRouter;
        private IAuditLogWFManager _auditLogWFManager;

        public CommodityDeliveryWFManager( IAuditLogWFManager auditLogWfManager, IOutgoingCommandEnvelopeRouter commandEnvelopeRouter)
        {
           
            _auditLogWFManager = auditLogWfManager;
            _commandEnvelopeRouter = commandEnvelopeRouter;
        }

        public void SubmitChanges(CommodityDeliveryNote document)
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
            List<WeighedCommodityDeliveryLineItemCommand> weighed = commandsToExecute.OfType<WeighedCommodityDeliveryLineItemCommand>().ToList();
            foreach (var item in weighed)
            {
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, item));
              //  _commandRouter.RouteDocumentCommand(item);

            }

            ConfirmCommodityDeliveryCommand confirmCommodityDeliveryCommand = commandsToExecute.OfType<ConfirmCommodityDeliveryCommand>().FirstOrDefault();
            if (confirmCommodityDeliveryCommand != null)
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, confirmCommodityDeliveryCommand));
               // _commandRouter.RouteDocumentCommand(confirmCommodityDeliveryCommand);

            ApproveDeliveryCommand receivedDeliveryCommand = commandsToExecute.OfType<ApproveDeliveryCommand>().FirstOrDefault();
            if (receivedDeliveryCommand != null)
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, receivedDeliveryCommand));
            
                //_commandRouter.RouteDocumentCommand(receivedDeliveryCommand);
            _commandEnvelopeRouter.RouteCommandEnvelope(envelope);
        }
    }
}
