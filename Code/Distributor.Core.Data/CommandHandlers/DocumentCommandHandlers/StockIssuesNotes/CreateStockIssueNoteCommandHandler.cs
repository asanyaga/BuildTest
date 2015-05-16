using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Factory.Master;
using Distributr.Core.Repository.Transactional.DocumentRepositories;
using Distributr.Core.Commands.DocumentCommands.StockIssueNotes;
using Distributr.Core.Domain.Transactional.DocumentEntities;

namespace Distributr.Core.CommandHandler.DocumentCommandHandlers.StockIssueNotes.Impl
{
    public class CreateStockIssueNoteCommandHandler : ICreateStockIssueNoteCommandHandler
    {
        ICostCentreRepository _costCenterRepository;
        IUserRepository _userRepository;
        IDocumentFactory _documentFactory;
        IDocumentRepository _documentRepository;

        public CreateStockIssueNoteCommandHandler(ICostCentreRepository costCenterRepository,
            IUserRepository userRepository,
            IDocumentRepository documentRepository,
            IDocumentFactory documentFactory)
        {
            _costCenterRepository = costCenterRepository;
            _userRepository = userRepository;
            _documentFactory = documentFactory;
            _documentRepository = documentRepository;
        }

        #region ICommandHandler<CreateStockIssueNoteCommand> Members

        public void Execute(CreateStockIssueNoteCommand command)
        {
            bool already_Exist = _documentRepository.GetById(command.DocumentId) != null;
            if (already_Exist)
                return;
            StockIssueNote stockIssueNote = _documentFactory.CreateDocument(
                command.DocumentId, DocumentType.StockIssueNote,
                _costCenterRepository.GetById(command.DocumentIssuerCostCentreId),
                _costCenterRepository.GetById(command.DocumentIssuerCostCentreId),
                _userRepository.GetById(command.DocIssuerUserId),
                "") as StockIssueNote;
            _documentRepository.Save(stockIssueNote);
        }

        #endregion
    }
}
