using System;
using System.Linq;
using Distributr.Core.CommandHandler.SourcingDocumentCommandHandlers.CommodityWarehouseStorageCommandHandler;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityWarehouseStorageCommands;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities.LineItems;
using log4net;

namespace Distributr.Core.Data.CommandHandlers.SourcingDocumentCommandHandlers.CommodityWarehouseStorageCommandHandlers
{
    public class ConfirmCommodityWarehouseStorageCommandHandler : BaseSourcingCommandHandler, IConfirmCommodityWarehouseStorageCommandHandler
    {
         ILog _log = LogManager.GetLogger("ConfirmCommodityWarehouseStorageCommandHandler");
         public ConfirmCommodityWarehouseStorageCommandHandler(CokeDataContext context)
             : base(context)
        {
        }

         public void Execute(ConfirmCommodityWarehouseStorageCommand command)
        {
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
            try
            {
                if (!DocumentExists(command.DocumentId))
                    return;
                ConfirmDocument(command.DocumentId);
                foreach(var item in _context.tblSourcingLineItem.Where(s=>s.DocumentId==command.DocumentId))
                {
                    item.LineItemStatusId =(int) SourcingLineItemStatus.Confirmed;
                }
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                _log.Error("ConfirmCommodityWarehouseStorageCommandHandler exception", ex);
                throw;
            }
        }
    }
}
