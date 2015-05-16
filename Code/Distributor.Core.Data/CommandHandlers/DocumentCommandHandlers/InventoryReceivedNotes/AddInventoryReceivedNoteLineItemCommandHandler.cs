using System;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.InventoryReceivedNotes;
using Distributr.Core.Commands.DocumentCommands.InventoryReceivedNotes;
using Distributr.Core.Data.EF;
using Distributr.Core.Factory.Documents;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Repository.Transactional.DocumentRepositories.InventoryRepositories;
using log4net;

namespace Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.InventoryReceivedNotes
{
    public class AddInventoryReceivedNoteLineItemCommandHandler : BaseCommandHandler, IAddInventoryReceivedNoteLineItemCommandHandler
    {
        private CokeDataContext _cokeDataContext;
        ICostCentreRepository _costCenterRepository;
        IUserRepository _userRepository;
        IDocumentFactory _documentFactory;
        IInventoryReceivedNoteRepository _documentRepository;
        IProductRepository _productRepository;
        ILog _log = LogManager.GetLogger("AddInventoryReceivedNoteLineItemCommandHandler");


        public AddInventoryReceivedNoteLineItemCommandHandler(ICostCentreRepository costCenterRepository, IUserRepository userRepository,
            IInventoryReceivedNoteRepository documentRepository, IDocumentFactory documentFactory, IProductRepository productRepository,
            CokeDataContext cokeDataContext) : base(cokeDataContext)
        {
            _cokeDataContext = cokeDataContext;
            _costCenterRepository = costCenterRepository;
            _userRepository = userRepository;
            _documentFactory = documentFactory;
            _documentRepository = documentRepository;
            _productRepository = productRepository;
        }

        public void Execute(AddInventoryReceivedNoteLineItemCommand command)
        {
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
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
                                                    command.Description, command.Qty, command.LineItemSequenceNo);
                lineItem.Value = command.Value;
                lineItem.ApprovedQuantity = command.Expected;//todo=>GO temporary fix=>to hold original approved quantity from HQ

                document.tblLineItems.Add(lineItem);
                _cokeDataContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                _log.Error("AddInventoryReceivedNoteLineItemCommandHandler", ex);
                throw;
            }
        }

    }
}
