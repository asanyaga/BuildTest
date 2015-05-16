using System;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.AdjustmentNotes;
using Distributr.Core.Commands.DocumentCommands.AdjustmentNotes;
using Distributr.Core.Data.EF;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.InventoryRepositories;
using log4net;

namespace Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.AdjustmentNotes
{
    public class AddInventoryAdjustmentNoteLineItemCommandHandler : BaseCommandHandler, IAddInventoryAdjustmentNoteLineItemCommandHandler
    {
        ILog _log = LogManager.GetLogger("AddInventoryAdjustmentNoteLineItemCommandHandler");
        private CokeDataContext _cokeDataContext;

        public AddInventoryAdjustmentNoteLineItemCommandHandler(
            IInventoryAdjustmentNoteRepository documentRepository, IProductRepository productRepository, CokeDataContext cokeDataContext)
            : base(cokeDataContext)
        {
            _cokeDataContext = cokeDataContext;
        }

        public void Execute(AddInventoryAdjustmentNoteLineItemCommand command)
        {
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType());
            try
            {
                if (!DocumentExists(command.DocumentId))
                {
                    _log.InfoFormat("Cannot add line item. Document does not exist  Execute {1} - Command Id {0} ", command.CommandId, command.GetType());
                    return;
                }
                if (DocumentLineItemExists(command.CommandId))
                {
                    _log.InfoFormat("Cannot add line item {0}. Line item already exists", command.CommandId);
                    return;
                }

                tblDocument document = ExistingDocument(command.DocumentId);
                tblLineItems lineItem = NewLineItem(command.CommandId, command.DocumentId, command.ProductId,
                                                    command.Reason, command.Expected, command.LineItemSequence);
                lineItem.IAN_Actual = command.Actual;
                document.tblLineItems.Add(lineItem);
                _cokeDataContext.SaveChanges();
            }

            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                _log.Error("AddInventoryAdjustmentNoteLineItemCommandHandler exception", ex);
                throw ;
            }
        }
    }
}
