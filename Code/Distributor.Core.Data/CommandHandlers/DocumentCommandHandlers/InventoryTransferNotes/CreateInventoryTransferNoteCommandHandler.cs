using System;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.InventoryTransferNotes;
using Distributr.Core.Commands.DocumentCommands.InventoryTransferNotes;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using log4net;

namespace Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.InventoryTransferNotes
{
    public class CreateInventoryTransferNoteCommandHandler : BaseCommandHandler,  ICreateInventoryTransferNoteCommandHandler
    {
        private CokeDataContext _cokeDataContext;
        ILog _log = LogManager.GetLogger("CreateInventoryTransferNoteCommandHandler");

        public CreateInventoryTransferNoteCommandHandler(
             CokeDataContext cokeDataContext):base(cokeDataContext)
        {
            _cokeDataContext = cokeDataContext;
        }

        public void Execute(CreateInventoryTransferNoteCommand command)
        {
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
            try
            {
                if( DocumentExists(command.DocumentId))
                    return;

                tblDocument doc = NewDocument(command, DocumentType.InventoryTransferNote,
                                              command.DocumentRecipientCostCentreId);
                doc.OrderIssuedOnBehalfOfCC = command.IssuedOnBehalfOfCostCentreId;
                _cokeDataContext.tblDocument.AddObject(doc);
                _cokeDataContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                _log.Error("CreateInventoryTransferNoteCommandHandler exception", ex);
                throw ;
            }
        }


    }
}
