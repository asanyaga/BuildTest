using System;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.InventoryTransferNotes;
using Distributr.Core.Commands.DocumentCommands.InventoryTransferNotes;
using Distributr.Core.Data.EF;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.InventoryRepositories;
using log4net;

namespace Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.InventoryTransferNotes
{
    public class AddInventoryTransferNoteLineItemCommandHandler : BaseCommandHandler, IAddInventoryTransferNoteLineItemCommandHandler
    {
        private CokeDataContext _cokeDataContext;
        IInventoryTransferNoteRepository _documentRepository;
        IProductRepository _productRepository;
        ILog _log = LogManager.GetLogger("AddInventoryTransferNoteLineItemCommandHandler");


        public AddInventoryTransferNoteLineItemCommandHandler(
            IInventoryTransferNoteRepository documentRepository, 
            IProductRepository productRepository,CokeDataContext cokeDataContext ) : base(cokeDataContext)
        {
            _documentRepository = documentRepository;
            _productRepository = productRepository;
            _cokeDataContext = cokeDataContext;
        }

        public void Execute(AddInventoryTransferNoteLineItemCommand command)
        {
             _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType());
             try
             {
                 if (!DocumentExists(command.DocumentId))
                 {
                     _log.InfoFormat("Cannot add line item. Document does not exist. Execute {1} - Command Id {0} ", command.CommandId, command.GetType());
                     return;
                 }

                 if (DocumentLineItemExists(command.CommandId))
                 {
                     _log.InfoFormat("Cannot add line item. Line item already exists. Execute {1} - Command Id {0} ", command.CommandId, command.GetType());
                     return;
                 }

                 tblDocument document = ExistingDocument(command.DocumentId);
                 tblLineItems lineItem = NewLineItem(command.CommandId, command.DocumentId, command.ProductId,
                                                     command.Description, command.Qty, command.LineItemSequenceNo);
                 document.tblLineItems.Add(lineItem);
                 _cokeDataContext.SaveChanges();
             }
             catch (Exception ex)
             {
                 _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                _log.Error("AddInventoryTransferNoteLineItemCommandHandler exception", ex);
                 throw ;
             }
        }


    }
}
