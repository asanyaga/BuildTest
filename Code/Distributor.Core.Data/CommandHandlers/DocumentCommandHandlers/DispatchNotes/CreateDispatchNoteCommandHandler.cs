using System;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.DispatchNotes;
using Distributr.Core.Commands.DocumentCommands.DispatchNotes;
using Distributr.Core.Data.EF;
using Distributr.Core.Factory.Documents;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Repository.Transactional.DocumentRepositories.DispatchRepositories;
using log4net;

namespace Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.DispatchNotes
{
    public class CreateDispatchNoteCommandHandler : BaseCommandHandler, ICreateDispatchNoteCommandHandler
    {
      
        ICostCentreRepository _costCenterRepository;
        IUserRepository _userRepository;
        IDocumentFactory _documentFactory;
        IDispatchNoteRepository _documentRepository;
        ILog _log = LogManager.GetLogger("CreateDispatchNoteCommandHandler");
        private CokeDataContext _cokeDataContext;

        public CreateDispatchNoteCommandHandler(ICostCentreRepository costCenterRepository, 
            IUserRepository userRepository,
            IDispatchNoteRepository documentRepository, 
            IDocumentFactory documentFactory, CokeDataContext cokeDataContext) : base(cokeDataContext)
        {
            //_orderRepository = orderRepository;
            _costCenterRepository = costCenterRepository;
            _userRepository = userRepository;
            _documentFactory = documentFactory;
            _documentRepository = documentRepository;
            _cokeDataContext = cokeDataContext;
        }

        public void Execute(CreateDispatchNoteCommand command)
        {
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());

            try
            {
                if (DocumentExists(command.DocumentId))
                    return;

                tblDocument doc = NewDocument(command, DocumentType.DispatchNote, command.DispatchNoteRecipientCostCentreId);
                doc.OrderOrderTypeId = command.DispatchNoteType;
                doc.InvoiceOrderId = command.OrderId;

                _cokeDataContext.tblDocument.AddObject(doc);
                _cokeDataContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                _log.Error("CreateDispatchNoteCommandHandler exception",ex);
                throw;
            }
        }
        
    }
}
