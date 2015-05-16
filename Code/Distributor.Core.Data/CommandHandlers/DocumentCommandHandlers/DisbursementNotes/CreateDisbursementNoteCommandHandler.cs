using System;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.DisbursementNotes;
using Distributr.Core.Commands.DocumentCommands.DisbursementNotes;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Factory.Documents;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.DisbursementRepositories;
using log4net;

namespace Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.DisbursementNotes
{
   public class CreateDisbursementNoteCommandHandler : ICreateDisbursementNoteCommandHandler
    {
       ICostCentreRepository _costCenterRepository;
        IUserRepository _userRepository;
        IDocumentFactory _documentFactory;
        IDisbursementNoteRepository _documentRepository;
        ILog _log = LogManager.GetLogger("CreateDisbursementNoteCommandHandler");

        public CreateDisbursementNoteCommandHandler(ICostCentreRepository costCenterRepository, 
            IUserRepository userRepository,
            IDisbursementNoteRepository documentRepository, 
            IDocumentFactory documentFactory)
        {
           
            _costCenterRepository = costCenterRepository;
            _userRepository = userRepository;
            _documentFactory = documentFactory;
            _documentRepository = documentRepository;
        }
        public void Execute(CreateDisbursementNoteCommand command)
        {
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());

            try
            {
                bool already_Exist = _documentRepository.GetById(command.DocumentId) != null;

                if (already_Exist)
                    return;
                DisbursementNote disbursementNote = _documentFactory.CreateDocument
                    (
                    command.DocumentId,
                    DocumentType.DisbursementNote,
                    _costCenterRepository.GetById(command.DocumentIssuerCostCentreId),
                    _costCenterRepository.GetById(command.DocumentRecipientCostCentreId),
                    _userRepository.GetById(command.DocIssuerUserId)
                    ,
                    "") as DisbursementNote;
                disbursementNote.SendDateTime = command.SendDateTime;
                disbursementNote.DocumentDateIssued = command.CommandCreatedDateTime;
                _documentRepository.Save(disbursementNote);
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                _log.Error("CreateDisbursementNoteCommandHandler exception", ex);
                throw ;
            }
        }

     
    }
}
