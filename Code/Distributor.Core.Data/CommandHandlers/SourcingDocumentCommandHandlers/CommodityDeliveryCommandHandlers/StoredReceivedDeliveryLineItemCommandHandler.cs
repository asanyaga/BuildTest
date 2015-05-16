using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.CommandHandler.SourcingDocumentCommandHandlers.CommodityDeliveryCommandHandler;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityDeliveryCommands;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities.LineItems;
using log4net;

namespace Distributr.Core.Data.CommandHandlers.SourcingDocumentCommandHandlers.CommodityDeliveryCommandHandlers
{
   public class StoredReceivedDeliveryLineItemCommandHandler:BaseSourcingCommandHandler, IStoredReceivedDeliveryLineItemCommandHandler
    {
       ILog _log = LogManager.GetLogger("StoredReceivedDeliveryLineItemCommandHandler");
        public StoredReceivedDeliveryLineItemCommandHandler(CokeDataContext context)
            : base(context)
        {
        }

        public void Execute(StoredReceivedDeliveryLineItemCommand command)
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
                    lineItem.LineItemStatusId = (int) SourcingLineItemStatus.Stored;
                }
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                _log.Error("StoredReceivedDeliveryLineItemCommandHandler exception", ex);
                throw;
            }
        }
    }
}
