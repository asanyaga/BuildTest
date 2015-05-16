using System;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.InventoryTransferNotes;
using Distributr.Core.Commands.DocumentCommands.InventoryTransferNotes;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Repository.Transactional.DocumentRepositories.InventoryRepositories;
using log4net;
using Distributr.Core.Workflow.InventoryWorkflow;

namespace Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.InventoryTransferNotes
{
    public class ConfirmInventoryTransferNoteCommandHandler :BaseCommandHandler, IConfirmInventoryTransferNoteCommandHandler
    {
        ILog _log = LogManager.GetLogger("ConfirmInventoryTransferNoteCommandHandler");
        private CokeDataContext _cokeDataContext;
        IInventoryTransferNoteRepository _documentRepository;
        private IInventoryWorkflow _inventoryWorkflow;
        public ConfirmInventoryTransferNoteCommandHandler(
            IInventoryTransferNoteRepository documentRepository, IInventoryWorkflow inventoryWorkflow, CokeDataContext cokeDataContext): base(cokeDataContext)
        {
            _cokeDataContext = cokeDataContext;
            _documentRepository = documentRepository;
            _inventoryWorkflow = inventoryWorkflow;
        }

        public void Execute(ConfirmInventoryTransferNoteCommand command)
        {
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
            try
            {
                if(!DocumentExists(command.DocumentId))
                    return;
                
                ConfirmDocument(command.DocumentId);

                InventoryTransferNote inventoryTransferNote = _documentRepository.GetById(command.DocumentId) ;
                
                foreach (var item in inventoryTransferNote.LineItems)
                {
                    //adjust the stock Distributor
                    _inventoryWorkflow.InventoryAdjust(inventoryTransferNote.DocumentIssuerCostCentre.Id, item.Product.Id, -(item.Qty), DocumentType.InventoryTransferNote
                                                       , inventoryTransferNote.Id, inventoryTransferNote.DocumentDateIssued, InventoryAdjustmentNoteType.Available);
                    //adjust the stock Sales Man
                    _inventoryWorkflow.InventoryAdjust(inventoryTransferNote.DocumentRecipientCostCentre.Id, item.Product.Id, item.Qty, DocumentType.InventoryTransferNote
                                                       , inventoryTransferNote.Id, inventoryTransferNote.DocumentDateIssued, InventoryAdjustmentNoteType.Available);
                }
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                _log.Error ("ConfirmInventoryTransferNoteCommandHandler exception", ex);
                throw ;
            }
        }

    }
}
