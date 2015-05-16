using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.AdjustmentNotes;
using Distributr.Core.Commands.DocumentCommands.AdjustmentNotes;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Repository.Transactional.DocumentRepositories.InventoryRepositories;
using log4net;
using Distributr.Core.Workflow.InventoryWorkflow;

namespace Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.AdjustmentNotes
{
    public class ConfirmInventoryAdjustmentNoteCommandHandler : BaseCommandHandler, IConfirmInventoryAdjustmentNoteCommandHandler
    {
        IInventoryAdjustmentNoteRepository _documentRepository;
        private IInventoryWorkflow _inventoryWorkflow;
        private CokeDataContext _cokeDataContext;
        ILog _log = LogManager.GetLogger("ConfirmInventoryAdjustmentNoteCommandHandler");
        public ConfirmInventoryAdjustmentNoteCommandHandler(
           IInventoryAdjustmentNoteRepository documentRepository, IInventoryWorkflow inventoryWorkflow, CokeDataContext cokeDataContext)
            :base(cokeDataContext)
        {
            _documentRepository = documentRepository;
            _inventoryWorkflow = inventoryWorkflow;
            _cokeDataContext = cokeDataContext;
        }

        public void Execute(ConfirmInventoryAdjustmentNoteCommand command)
        {
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
            try
            {
                if (!DocumentExists(command.DocumentId))
                {
                    _log.InfoFormat("Failed to  Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                    return;
                }
                if (DocumentIsConfirmed(command.DocumentId))
                {
                    _log.InfoFormat("Failed to  Execute {1} - Command Id {0} Document is already confirmed", command.CommandId, command.GetType().ToString());
                    return;
                }
                ConfirmDocument(command.DocumentId);
                
                InventoryAdjustmentNote adjustmentNote = _documentRepository.GetById(command.DocumentId);
                var nonInventoryDoc = new List<InventoryAdjustmentNoteType>
                                          {
                                              InventoryAdjustmentNoteType.StockTake,
                                              InventoryAdjustmentNoteType.OutletStockTake
                                          };
                if (!nonInventoryDoc.Contains(adjustmentNote.InventoryAdjustmentNoteType))
                {
                    List<Guid> aid =_cokeDataContext.tblLineItems
                        .Where(s => s.DocumentID == command.DocumentId)
                        .Select(s => s.ProductID.Value).Distinct().ToList();
                    int count =_cokeDataContext.tblProduct.Count(s => aid.Contains(s.id));
                    if(aid.Count!=count)
                    {
                        string liguilds = string.Join(",", aid.ToString());
                        int licount = aid.Count();
                        string message =
                            string.Format("The document contain invalid product ids {0} # line item count {1} - product count {2} ",liguilds, licount, count );
                        throw new Exception(message);
                    }
                    _log.InfoFormat("Document Id {0} no of lineItem count {1} ", command.DocumentId, adjustmentNote.LineItem.Count);
                 
                    foreach (var item in adjustmentNote.LineItem)
                    {
                        //adjust Distributor stock
                        _inventoryWorkflow.InventoryAdjust(adjustmentNote.DocumentIssuerCostCentre.Id, item.Product.Id,
                                                           (item.Actual - item.Qty),
                                                           DocumentType.InventoryAdjustmentNote, adjustmentNote.Id,
                                                           adjustmentNote.DocumentDateIssued,
                                                           adjustmentNote.InventoryAdjustmentNoteType);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                _log.Error("ConfirmInventoryAdjustmentNoteCommandHandler exception", ex);
                throw ex;
            }
        }
    }
}
