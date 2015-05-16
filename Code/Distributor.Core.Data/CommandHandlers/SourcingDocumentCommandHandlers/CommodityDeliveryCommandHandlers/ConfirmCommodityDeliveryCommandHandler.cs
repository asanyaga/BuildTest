using System;
using Distributr.Core.CommandHandler.SourcingDocumentCommandHandlers.CommodityDeliveryCommandHandler;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityDeliveryCommands;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities.LineItems;
using log4net;
using System.Linq;
namespace Distributr.Core.Data.CommandHandlers.SourcingDocumentCommandHandlers.CommodityDeliveryCommandHandlers
{
    public class ConfirmCommodityDeliveryCommandHandler :BaseSourcingCommandHandler, IConfirmCommodityDeliveryCommandHandler
    {
         ILog _log = LogManager.GetLogger("ConfirmCommodityDeliveryCommandHandler");
         public ConfirmCommodityDeliveryCommandHandler(CokeDataContext context)
             : base(context)
        {
        }

        public void Execute(ConfirmCommodityDeliveryCommand command)
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
                _log.Error("ConfirmCommodityDeliveryCommandHandler exception", ex);
                throw;
            }
        }
    }
}
