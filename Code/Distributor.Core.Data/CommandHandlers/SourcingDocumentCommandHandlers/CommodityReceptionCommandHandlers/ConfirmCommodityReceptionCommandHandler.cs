using System;
using Distributr.Core.CommandHandler.SourcingDocumentCommandHandlers.CommodityReceptionCommandHandler;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityReceptionCommands;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities.LineItems;
using Distributr.Core.Repository.Transactional.SourcingDocumentRepositories;
using log4net;
using System.Linq;

namespace Distributr.Core.Data.CommandHandlers.SourcingDocumentCommandHandlers.CommodityReceptionCommandHandlers
{
    public class ConfirmCommodityReceptionCommandHandler :BaseSourcingCommandHandler, IConfirmCommodityReceptionCommandHandler
    {
       
        ILog _log = LogManager.GetLogger("ConfirmCommodityReceptionCommandHandler");

        public ConfirmCommodityReceptionCommandHandler(CokeDataContext context) : base(context)
        {
        }

        public void Execute(ConfirmCommodityReceptionCommand command)
        {
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
            try
            {
                if (!DocumentExists(command.DocumentId))
                    return;
                ConfirmDocument(command.DocumentId);
                foreach (var item in _context.tblSourcingLineItem.Where(p=>p.DocumentId==command.DocumentId))
                {
                    item.LineItemStatusId = (int)SourcingLineItemStatus.Confirmed;
                }
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                _log.Error("ConfirmCommodityReceptionCommandHandler exception", ex);
                throw;
            }
        }
    }
}
