using System;
using Distributr.Core.CommandHandler.SourcingDocumentCommandHandlers.CommodityReceptionCommandHandler;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityDeliveryCommands;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityReceptionCommands;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities.LineItems;
using log4net;
using System.Linq;

namespace Distributr.Core.Data.CommandHandlers.SourcingDocumentCommandHandlers.CommodityReceptionCommandHandlers
{
    public class StoredCommodityReceptionLineItemCommandHandler : BaseSourcingCommandHandler, IStoredCommodityReceptionLineItemCommandHandler
    {
        ILog _log = LogManager.GetLogger("StoredCommodityReceptionLineItemCommandHandler");
        public StoredCommodityReceptionLineItemCommandHandler(CokeDataContext context)
            : base(context)
        {
        }

        public void Execute(StoredCommodityReceptionLineItemCommand command)
        {
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
            try
            {
                if (!DocumentExists(command.DocumentId))
                {
                    _log.InfoFormat("Cannot mark as stored line item. Document does not exist  Execute {1} - Command Id {0} ", command.CommandId, command.GetType());
                    return;
                }

                if (DocumentLineItemExists(command.DocumentLineItemId))
                {
                    tblSourcingLineItem lineItem =_context.tblSourcingLineItem.First(s => s.Id == command.DocumentLineItemId);
                    lineItem.LineItemStatusId = (int) SourcingLineItemStatus.Stored;
                }
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                _log.Error("StoredCommodityReceptionLineItemCommandHandler exception", ex);
                throw;
            }
        }
    }
}
