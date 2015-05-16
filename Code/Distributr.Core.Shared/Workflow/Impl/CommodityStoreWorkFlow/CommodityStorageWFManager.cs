using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.ClientApp;
using Distributr.Core.Commands.CommandPackage;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityStorageCommands;
using Distributr.Core.Domain.Transactional;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities.LineItems;

namespace Distributr.Core.Workflow.Impl.CommodityStoreWorkFlow
{
    public class CommodityStorageWFManager : ICommodityStorageWFManager
    {
      
        IOutgoingCommandEnvelopeRouter _commandEnvelopeRouter;
        private IAuditLogsWFManager _auditLogWFManager;

        public CommodityStorageWFManager(IAuditLogsWFManager auditLogWfManager, IOutgoingCommandEnvelopeRouter commandEnvelopeRouter)
        {
            _auditLogWFManager = auditLogWfManager;
            _commandEnvelopeRouter = commandEnvelopeRouter;
        }


        public void SubmitChanges(CommodityStorageNote document)
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
                //_commandRouter.RouteDocumentCommand(item);

            }

            ConfirmCommodityStorageCommand confirmCommand = commandsToExecute.OfType<ConfirmCommodityStorageCommand>().FirstOrDefault();
            if (confirmCommand != null)
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, confirmCommand));
            _commandEnvelopeRouter.RouteCommandEnvelope(envelope);

            _auditLogWFManager.AuditLogEntry("Commodity Storage", "Created and confirmed commodity storage note " + document.DocumentReference+ "; id "+document.Id + " with " +document.LineItems.Count+" line items");
        }

        public void SaveNew(CommodityStorageNote document)
        {
            throw new NotImplementedException();
        }

        public CommodityStorageNote PendingSourcingDocumentFactory(ISourcingDocumentFactoryParameters parameters)
        {
            throw new NotImplementedException();
        }

        public CommodityStorageLineItem PendingSourcingDocumentLineItemsFactory(CommodityStorageNote document,
                                                                                ISourcingDocumentLineItemFactoryParameters parameters)
        {
            throw new NotImplementedException();
        }
    }
}
