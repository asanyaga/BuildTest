using System;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.ReturnsNotes;
using Distributr.Core.Data.EF;
using Distributr.Core.Factory.Documents;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.ReturnsRepositories;
using log4net;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Commands.DocumentCommands.ReturnsNotes;

namespace Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.ReturnsNotes
{
    public class CreateReturnsNoteCommandHandler :BaseCommandHandler, ICreateReturnsNoteCommandHandler
    {
        private CokeDataContext _cokeDataContext;
        ILog _log = LogManager.GetLogger("CreateReturnsNoteCommandHandler");
        public CreateReturnsNoteCommandHandler(CokeDataContext context) : base(context)
        {
            _cokeDataContext = context;
        }

        public void Execute(CreateReturnsNoteCommand command)
        {
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
            try
            {
                if (DocumentExists(command.DocumentId))
                    return;

                tblDocument doc = NewDocument(command, DocumentType.ReturnsNote,
                                              command.DocumentRecipientCostCentreId);
                doc.DocumentDateIssued = command.DateReturnsNoteCreated;
                doc.DocumentIssuerCostCentreApplicationId = command.CommandGeneratedByCostCentreApplicationId;
                doc.SendDateTime = command.SendDateTime;
                doc.OrderOrderTypeId = command.ReturnsNoteTypeId;
                
                _cokeDataContext.tblDocument.AddObject(doc);
                _cokeDataContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                _log.Error("CreateReturnsNoteCommandHandler exception", ex);
                throw;
            }
            
        }
    }
}
