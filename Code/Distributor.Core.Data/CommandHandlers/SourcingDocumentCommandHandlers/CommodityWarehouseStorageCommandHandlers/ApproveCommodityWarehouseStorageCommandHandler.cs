using System;
using System.Linq;
using Distributr.Core.CommandHandler.SourcingDocumentCommandHandlers.CommodityWarehouseStorageCommandHandler;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityWarehouseStorageCommands;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities.LineItems;
using log4net;

namespace Distributr.Core.Data.CommandHandlers.SourcingDocumentCommandHandlers.CommodityWarehouseStorageCommandHandlers
{
    public class ApproveCommodityWarehouseStorageCommandHandler : BaseSourcingCommandHandler, IApproveCommodityWarehouseStorageCommandHandler
    {
         ILog _log = LogManager.GetLogger("ApproveCommodityWarehouseStorageCommandHandler");
         public ApproveCommodityWarehouseStorageCommandHandler(CokeDataContext context)
             : base(context)
        {
        }

         public void Execute(ApproveCommodityWarehouseStorageCommand command)
        {
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
            try
            {
                if (!DocumentExists(command.DocumentId))
                    return;
                ApproveDocument(command.DocumentId);
                
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                _log.Error("ApproveCommodityWarehouseStorageCommandHandler exception", ex);
                throw;
            }
        }
    }
}
