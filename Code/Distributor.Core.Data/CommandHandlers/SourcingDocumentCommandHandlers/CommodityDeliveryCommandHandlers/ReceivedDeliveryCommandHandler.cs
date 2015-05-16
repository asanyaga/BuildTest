using System;
using Distributr.Core.CommandHandler.SourcingDocumentCommandHandlers.CommodityDeliveryCommandHandler;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityDeliveryCommands;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities.LineItems;
using log4net;
using System.Linq;
namespace Distributr.Core.Data.CommandHandlers.SourcingDocumentCommandHandlers.CommodityDeliveryCommandHandlers
{
    public class ApproveDeliveryCommandHandler : BaseSourcingCommandHandler, IApproveDeliveryCommandHandler
    {
        ILog _log = LogManager.GetLogger("ApproveDeliveryCommandHandler");
        public ApproveDeliveryCommandHandler(CokeDataContext context) : base(context)
        {
        }

        public void Execute(ApproveDeliveryCommand command)
        {
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
            try
            {
                if (!DocumentExists(command.DocumentId))
                    return;
                ReceiveDocument(command.DocumentId);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                _log.Error("ApproveDeliveryCommandHandler exception", ex);
                throw;
            }
        }
    }
}
