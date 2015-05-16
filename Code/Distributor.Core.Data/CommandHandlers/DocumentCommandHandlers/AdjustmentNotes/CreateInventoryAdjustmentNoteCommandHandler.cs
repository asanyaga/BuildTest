using System;
using System.Data;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.AdjustmentNotes;
using Distributr.Core.Commands.DocumentCommands.AdjustmentNotes;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using log4net;

namespace Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.AdjustmentNotes
{
    public class CreateInventoryAdjustmentNoteCommandHandler : BaseCommandHandler, ICreateInventoryAdjustmentNoteCommandHandler
    {
        private CokeDataContext _ctx;
        ILog _logger = LogManager.GetLogger("CreateInventoryAdjustmentNoteCommandHandler");
        public CreateInventoryAdjustmentNoteCommandHandler(
            CokeDataContext cokeDataContext
            )
            : base(cokeDataContext)
        {
            _ctx = cokeDataContext;
        }
        public void Execute(CreateInventoryAdjustmentNoteCommand command)
        {
            _logger.InfoFormat("Execute CreateInventoryAdjustmentNoteCommand - Command Id {0} ", command.CommandId);
            try
            {
                if (DocumentExists(command.DocumentId))
                    return;

                tblDocument doc = NewDocument(command, DocumentType.InventoryAdjustmentNote,
                                              command.DocumentRecipientCostCentreId);
                doc.OrderOrderTypeId = command.InventoryAdjustmentNoteTypeId;
                
                doc.VisitId = command.VisitId;
                _ctx.tblDocument.AddObject(doc);
                _ctx.SaveChanges();
            }
            catch (Exception ex)
            {
              var data=  _ctx.ObjectStateManager.GetObjectStateEntries(EntityState.Added);
                _logger.ErrorFormat("Error CreateInventoryAdjustmentNoteCommand - Command Id {0} ", command.CommandId);
                _logger.Error("CreateInventoryAdjustmentNoteCommandHandler exception", ex);
                throw;
            }
        }


    }
}
