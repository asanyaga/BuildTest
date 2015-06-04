using System.Collections.Generic;
using System.Linq;
using Distributr.Core.ClientApp;
using Distributr.Core.Commands.CommandPackage;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityPurchaseCommands;
using Distributr.Core.Domain.Transactional;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;

namespace Distributr.Core.Workflow.Impl.CommodityPurchaseWorkFlow
{
    public class CommodityPurchaseWFManager : ICommodityPurchaseWFManager
    {
       
        IOutgoingCommandEnvelopeRouter _commandEnvelopeRouter;
        private IAuditLogsWFManager _auditLogWFManager;

        public CommodityPurchaseWFManager(IOutgoingCommandEnvelopeRouter commandEnvelopeRouter, IAuditLogsWFManager auditLogWfManager)
        {
            _commandEnvelopeRouter = commandEnvelopeRouter;
            _auditLogWFManager = auditLogWfManager;
        }


        public void SubmitChanges(CommodityPurchaseNote document)
        {
            int sequence = 0;
            var envelope = new CommandEnvelope();
            envelope.Initialize(document);
            
            List<DocumentCommand> commandsToExecute = document.GetDocumentCommandsToExecute();

            CreateCommand createCommand = commandsToExecute.OfType<CreateCommand>().FirstOrDefault();
            if (createCommand != null)
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, createCommand));
                //_commandRouter.RouteDocumentCommand(createCommand);
            List<AfterCreateCommand> lineItemCommands = commandsToExecute.OfType<AfterCreateCommand>().ToList();
            foreach (var item in lineItemCommands)
            {
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, item));
              //  _commandRouter.RouteDocumentCommand(item);

            }
            ConfirmCommodityPurchaseCommand confirmCommand = commandsToExecute.OfType<ConfirmCommodityPurchaseCommand>().FirstOrDefault();
            if (confirmCommand != null)
            {
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, confirmCommand));

               // _commandRouter.RouteDocumentCommand(confirmCommand);
                //_notifyService.SubmitCommodityPurchase(document);
            }
            _commandEnvelopeRouter.RouteCommandEnvelope(envelope);
            _auditLogWFManager.AuditLogEntry("Commodity Purchase", "Created and confirmed commodity purchase note " + document.DocumentReference + "; id " + document.Id + " with " + document.LineItems.Count + " line items");

        }
    }
}
