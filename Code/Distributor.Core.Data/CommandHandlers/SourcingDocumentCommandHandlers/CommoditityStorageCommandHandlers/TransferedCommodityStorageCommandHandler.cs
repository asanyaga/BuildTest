using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.CommandHandler.SourcingDocumentCommandHandlers.CommodityStorageCommandHandler;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityStorageCommands;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities.LineItems;
using log4net;

namespace Distributr.Core.Data.CommandHandlers.SourcingDocumentCommandHandlers.CommoditityStorageCommandHandlers
{
    public class TransferedCommodityStorageCommandHandler : BaseSourcingCommandHandler, ITransferedCommodityStorageCommandHandler
    {
        ILog _log = LogManager.GetLogger("TransferedCommodityStorageCommandHandler");

        public TransferedCommodityStorageCommandHandler(CokeDataContext context) : base(context)
        {
        }

        public void Execute(TransferedCommodityStorageLineItemCommand command)
        {
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
            try
            {
                
                if (!DocumentExists(command.DocumentId))
                {
                    _log.InfoFormat("Cannot mark as Transfered line item. Document does not exist  Execute {1} - Command Id {0} ", command.CommandId, command.GetType());
                    return;
                }
                if (DocumentLineItemExists(command.DocumentLineItemId))
                {
                    tblSourcingLineItem lineItem = _context.tblSourcingLineItem.First(s => s.Id == command.DocumentLineItemId);
                    lineItem.LineItemStatusId = (int)SourcingLineItemStatus.Transfered;
                }
                _context.SaveChanges();
                
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                _log.Error("TransferedCommodityStorageCommandHandler exception", ex);
                throw;
            }
        }
    }
}
