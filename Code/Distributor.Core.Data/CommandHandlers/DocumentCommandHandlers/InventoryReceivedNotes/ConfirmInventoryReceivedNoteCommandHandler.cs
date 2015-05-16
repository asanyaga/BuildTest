using System;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.InventoryReceivedNotes;
using Distributr.Core.Commands.DocumentCommands.InventoryReceivedNotes;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Repository.Transactional.DocumentRepositories.InventoryRepositories;
using Distributr.Core.Workflow.InventoryWorkflow;
using log4net;

namespace Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.InventoryReceivedNotes
{
    public class ConfirmInventoryReceivedNoteCommandHandler :BaseCommandHandler, IConfirmInventoryReceivedNoteCommandHandler
    {
        private CokeDataContext _cokeDataContext;
        IInventoryReceivedNoteRepository _documentRepository;
        private IInventoryWorkflow _inventoryWorkflow;
        ILog _log = LogManager.GetLogger("ConfirmInventoryReceivedNoteCommandHandler");

        public ConfirmInventoryReceivedNoteCommandHandler(IInventoryReceivedNoteRepository documentRepository, IInventoryWorkflow inventoryWorkflow,
            CokeDataContext cokeDataContext) : base(cokeDataContext)
        {
            _cokeDataContext = cokeDataContext;
            _documentRepository = documentRepository;
            _inventoryWorkflow = inventoryWorkflow;
        }

        public void Execute(ConfirmInventoryReceivedNoteCommand command)
        {
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
            try
            {
                if (!DocumentExists(command.DocumentId))
                    return;
                ConfirmDocument(command.DocumentId);
                InventoryReceivedNote inventoryReceivedNote = _documentRepository.GetById(command.DocumentId);

                foreach (var item in inventoryReceivedNote.LineItems)
                {
                    _inventoryWorkflow.InventoryAdjust(inventoryReceivedNote.DocumentIssuerCostCentre.Id, item.Product.Id, item.Qty, DocumentType.InventoryReceivedNote
                                                       , inventoryReceivedNote.Id, inventoryReceivedNote.DocumentDateIssued, InventoryAdjustmentNoteType.Available);
                }
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                _log.Error("ConfirmInventoryReceivedNoteCommandHandler exception", ex);
                throw ;
            }
        }

    }
}
