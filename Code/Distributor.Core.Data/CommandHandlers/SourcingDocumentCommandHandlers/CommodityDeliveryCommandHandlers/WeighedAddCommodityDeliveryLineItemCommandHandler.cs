using System;
using Distributr.Core.CommandHandler.SourcingDocumentCommandHandlers.CommodityDeliveryCommandHandler;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityDeliveryCommands;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities.LineItems;
using log4net;
using System.Linq;

namespace Distributr.Core.Data.CommandHandlers.SourcingDocumentCommandHandlers.CommodityDeliveryCommandHandlers
{
    public class WeighedCommodityDeliveryLineItemCommandHandler :BaseSourcingCommandHandler, IWeighedCommodityDeliveryLineItemCommandHandler
    {
        ILog _log = LogManager.GetLogger("WeighedCommodityDeliveryLineItemCommandHandler");
        public WeighedCommodityDeliveryLineItemCommandHandler(CokeDataContext context)
            : base(context)
        {
        }

        public void Execute(WeighedCommodityDeliveryLineItemCommand command)
        {
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
            try
            {
                if (!DocumentExists(command.DocumentId))
                {
                    _log.InfoFormat("Cannot mark as weighed line item. Document does not exist  Execute {1} - Command Id {0} ", command.CommandId, command.GetType());
                    return;
                }

                if (DocumentLineItemExists(command.DocumentLineItemId))
                {
                    tblSourcingLineItem lineItem =_context.tblSourcingLineItem.First(s => s.Id == command.DocumentLineItemId);
                    lineItem.LineItemStatusId = (int) SourcingLineItemStatus.Weighed;
                }
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                _log.Error("WeighedCommodityDeliveryLineItemCommandHandler exception", ex);
                throw;
            }
        }
    }
}
