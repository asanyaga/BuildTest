using System;
using Distributr.Core.CommandHandler.SourcingDocumentCommandHandlers.ReceivedDeliveryCommandHandler;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityDeliveryCommands;
using Distributr.Core.Data.EF;
using log4net;

namespace Distributr.Core.Data.CommandHandlers.SourcingDocumentCommandHandlers.ReceivedDeliveryCommandHandlers
{
    public class ConfirmReceivedDeliveryCommandHandler : BaseSourcingCommandHandler, IConfirmReceivedDeliveryCommandHandler
    {
         ILog _log = LogManager.GetLogger("ConfirmReceivedDeliveryCommandHandler");
         public ConfirmReceivedDeliveryCommandHandler(CokeDataContext context)
             : base(context)
        {
        }

         public void Execute(ConfirmReceivedDeliveryCommand command)
         {
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
                _log.Error("ConfirmReceivedDeliveryCommandHandler exception", ex);
                throw;
            }
        }
    }
}
