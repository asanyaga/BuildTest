using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.ClientApp;
using Distributr.Core.Commands.CommandPackage;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.Commands.DocumentCommands.AdjustmentNotes;
using Distributr.Core.Domain.Transactional;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Factory.Documents;
using Distributr.Core.Repository.InventoryRepository;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.InventoryRepositories;
using Distributr.Core.Workflow.Impl.AuditLogs;
using Distributr.Core.Workflow.InventoryWorkflow;

namespace Distributr.Core.Workflow.Impl.AdjustmentNote
{
    public class InventoryAdjustmentNoteWfManager : IInventoryAdjustmentNoteWfManager
    {
         IOutgoingCommandEnvelopeRouter _commandEnvelopeRouter;
       
        private IAuditLogWFManager _auditLogWFManager;

        public InventoryAdjustmentNoteWfManager(IOutgoingCommandEnvelopeRouter commandEnvelopeRouter, IAuditLogWFManager auditLogWfManager)
        {
            _commandEnvelopeRouter = commandEnvelopeRouter;
            _auditLogWFManager = auditLogWfManager;
        }


        public void SubmitChanges(InventoryAdjustmentNote document, BasicConfig config)
        {
            int sequence = 0;
            var envelope = new CommandEnvelope();
            envelope.Initialize(document);
            envelope.OtherRecipientCostCentreList.Add(Guid.NewGuid());
            List<DocumentCommand> commandsToExecute = document.GetDocumentCommandsToExecute();
            
            var createCommand = commandsToExecute.OfType<CreateCommand>().First();
           // _commandRouter.RouteDocumentCommand(createCommand);
            envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence,createCommand));
            _auditLogWFManager.AuditLogEntry("Inventory Adjustment", string.Format("Created IAN document: {0};", document.Id));

            var lineItemCommands = commandsToExecute.OfType<AfterCreateCommand>();
            foreach (var _item in lineItemCommands)
            {
                var item = _item as AddInventoryAdjustmentNoteLineItemCommand;
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, item));
             
                _auditLogWFManager.AuditLogEntry("Inventory Adjustment", string.Format("Adjusted Product: {1}; quantity from: {2}; to: {3}; for IAN document: {0};", document.Id, item.ProductId, item.Actual, item.Actual));
            }
            
            var co = commandsToExecute.OfType<ConfirmCommand>().First();
            envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, co));
            _commandEnvelopeRouter.RouteCommandEnvelope(envelope);
            _auditLogWFManager.AuditLogEntry("Inventory Adjustment", string.Format("Confirmed IAN document: {0};", document.Id));
        }

        public void CreateAndConfirmIAN(Document document)
        {
        }

        //public void Save(InventoryAdjustmentNote ian)
        //{
        //    return;
        //    _inventoryAdjustmentNoteService.Save(ian);
        //    if (ian.InventoryAdjustmentNoteType != InventoryAdjustmentNoteType.StockTake)
        //    {
        //        foreach (var lineitem in ian.LineItem)
        //        {
        //            _inventoryWorkflow.InventoryAdjust(ian.DocumentIssuerCostCentre.Id,
        //                                               lineitem.Product.Id, (lineitem.Actual - lineitem.Qty),
        //                                               DocumentType.InventoryAdjustmentNote, ian.Id,
        //                                               ian.DocumentDateIssued, ian.InventoryAdjustmentNoteType);
        //        }
        //    }
        //}

        
    }
}
