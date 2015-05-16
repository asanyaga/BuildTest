using System;
using Distributr.Core.CommandHandler.SourcingDocumentCommandHandlers.CommodityPurchaseCommandHandler;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityPurchaseCommands;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.Core.Repository.Transactional.SourcingDocumentRepositories;
using log4net;

namespace Distributr.Core.Data.CommandHandlers.SourcingDocumentCommandHandlers.CommoditityPurchaseCommandHandlers
{
    public class ConfirmCommodityPurchaseCommandHandler :BaseSourcingCommandHandler, IConfirmCommodityPurchaseCommandHandler
    {
       
        ILog _log = LogManager.GetLogger("ConfirmCommodityPurchaseCommandHandler");


        public ConfirmCommodityPurchaseCommandHandler(CokeDataContext context) : base(context)
        {
        }

        public void Execute(ConfirmCommodityPurchaseCommand command)
        {
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
            try
            {
                if (!DocumentExists(command.DocumentId))
                    return;
                ConfirmDocument(command.DocumentId);
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
