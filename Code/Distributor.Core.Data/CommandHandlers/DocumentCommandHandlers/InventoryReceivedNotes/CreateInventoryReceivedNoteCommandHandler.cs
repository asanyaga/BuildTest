using System;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.InventoryReceivedNotes;
using Distributr.Core.Commands.DocumentCommands.InventoryReceivedNotes;
using Distributr.Core.Data.EF;
using Distributr.Core.Factory.Documents;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Repository.Transactional.DocumentRepositories.InventoryRepositories;
using log4net;

namespace Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.InventoryReceivedNotes
{
    public class CreateInventoryReceivedNoteCommandHandler : BaseCommandHandler ,ICreateInventoryReceivedNoteCommandHandler
    {
        private CokeDataContext _cokeDataContext;
        ILog _log = LogManager.GetLogger("CreateInventoryReceivedNoteCommandHandler");

        public CreateInventoryReceivedNoteCommandHandler( CokeDataContext cokeDataContext) : base(cokeDataContext)
        {
            _cokeDataContext = cokeDataContext;
        }

        public void Execute(CreateInventoryReceivedNoteCommand command)
        {
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
            try
            {
                if (DocumentExists(command.DocumentId))
                    return;

                tblDocument doc = NewDocument(command, DocumentType.InventoryReceivedNote,
                                              command.CommandGeneratedByCostCentreId);
                doc.IRNOrderReferences = command.OrderReferences;
                doc.IRNLoadNo = command.LoadNo;
                doc.IRNGoodsReceivedFromCC = command.InventoryReceivedFromCostCentreId;

                _cokeDataContext.tblDocument.AddObject(doc);
                _cokeDataContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                _log.Error("CreateInventoryReceivedNoteCommandHandler exception", ex);
                throw ;
            }
        }

    }
}
