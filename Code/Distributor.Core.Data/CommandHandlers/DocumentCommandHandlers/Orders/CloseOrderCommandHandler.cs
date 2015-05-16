using System;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.Orders;
using Distributr.Core.Commands.DocumentCommands.Orders;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using log4net;

namespace Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.Orders
{
    public class CloseOrderCommandHandler : BaseCommandHandler, ICloseOrderCommandHandler
    {
        IOrderRepository _documentRepository;
        ILog _log = LogManager.GetLogger("CloseOrderCommandHandler");
        private CokeDataContext _cokeDataContext;

        public CloseOrderCommandHandler(IOrderRepository documentRepository, CokeDataContext cokeDataContext) : base(cokeDataContext)
        {
            _documentRepository = documentRepository;
            _cokeDataContext = cokeDataContext;
        }

        public void Execute(CloseOrderCommand command)
        {
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
            try
            {
                if (!DocumentExists(command.DocumentId))
                    return;
                CloseDocument(command.DocumentId);
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
               _log.Error("CloseOrderCommandHandler exception",ex);
                throw;
            }
        }
    }
}
