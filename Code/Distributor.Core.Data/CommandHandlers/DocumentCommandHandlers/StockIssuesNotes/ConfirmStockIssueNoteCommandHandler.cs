using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Repository.Transactional.DocumentRepositories;
using Distributr.Core.Commands.DocumentCommands.StockIssueNotes;
using Distributr.Core.Domain.Transactional.DocumentEntities;

namespace Distributr.Core.CommandHandler.DocumentCommandHandlers.StockIssueNotes.Impl
{
    public class ConfirmStockIssueNoteCommandHandler : IConfirmStockIssueNoteCommandHandler
    {
        IDocumentRepository _documentRepository;
        public ConfirmStockIssueNoteCommandHandler(
            IDocumentRepository documentRepository)
        {
            _documentRepository = documentRepository;
        }

        
        #region ICommandHandler<ConfirmStockIssueNoteCommand> Members

        public void Execute(ConfirmStockIssueNoteCommand command)
        {
            bool already_Exist = _documentRepository.GetById(command.DocumentId) != null;
            if (!already_Exist)
                return;
            StockIssueNote stockIssueNote = _documentRepository.GetById(command.DocumentId) as StockIssueNote;
            stockIssueNote.Confirm();
            _documentRepository.Save(stockIssueNote);
        }

        #endregion
    }
}
