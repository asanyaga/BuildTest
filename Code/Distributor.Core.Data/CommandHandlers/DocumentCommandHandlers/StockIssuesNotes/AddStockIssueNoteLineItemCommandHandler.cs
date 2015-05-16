using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Factory.Master;
using Distributr.Core.Repository.Transactional.DocumentRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Commands.DocumentCommands.StockIssueNotes;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;

namespace Distributr.Core.CommandHandler.DocumentCommandHandlers.StockIssueNotes.Impl
{
    public class AddStockIssueNoteLineItemCommandHandler : IAddStockIssueNoteLineItemCommandHandler
    {
        ICostCentreRepository _costCenterRepository;
        IUserRepository _userRepository;
        IDocumentFactory _documentFactory;
        IDocumentRepository _documentRepository;
        IProductRepository _productRepository;


        public AddStockIssueNoteLineItemCommandHandler(ICostCentreRepository costCenterRepository, IUserRepository userRepository,
            IDocumentRepository documentRepository, IDocumentFactory documentFactory, IProductRepository productRepository)
        {
            _costCenterRepository = costCenterRepository;
            _userRepository = userRepository;
            _documentFactory = documentFactory;
            _documentRepository = documentRepository;
            _productRepository = productRepository;
        }

        #region ICommandHandler<AddStockIssueNoteLineItemCommand> Members

        public void Execute(AddStockIssueNoteLineItemCommand command)
        {
            bool already_Exist = _documentRepository.GetById(command.DocumentId) != null;
            if (!already_Exist)
                return;
            StockIssueNote stockIssueNote = _documentRepository.GetById(command.DocumentId) as StockIssueNote;
            StockIssueNoteLineItem stockIssueNoteLineItem = new StockIssueNoteLineItem(command.CommandId);
            stockIssueNoteLineItem.LineItemSequenceNo = command.LineItemSequence;
            stockIssueNoteLineItem.Product = _productRepository.GetById(command.ProductId);
            stockIssueNoteLineItem.Qty = command.Quantity;
            stockIssueNote.AddLineItem(stockIssueNoteLineItem);
            _documentRepository.Save(stockIssueNote);
        }

        #endregion
    }
}
