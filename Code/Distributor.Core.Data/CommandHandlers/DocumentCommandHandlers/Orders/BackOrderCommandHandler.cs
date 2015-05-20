using System;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.Orders;
using Distributr.Core.Commands.DocumentCommands.Orders;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using log4net;

namespace Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.Orders
{
    //public class BackOrderCommandHandler : IBackOrderCommandHandler
    //{
    //    private IOrderRepository _documentRepository;
    //    ILog _log = LogManager.GetLogger("BackOrderCommandHandler");

    //    public BackOrderCommandHandler(IOrderRepository documentRepository)
    //    {
    //        _documentRepository = documentRepository;
    //    }

    //    public void Execute(BackOrderCommand command)
    //    {
    //        _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
    //        try
    //        {
    //            //check that order exists
    //            bool no_Order_Exist = _documentRepository.GetById(command.DocumentId) == null;
    //            if (no_Order_Exist)
    //                return;
    //            //approve the order
    //            Order order = _documentRepository.GetById(command.DocumentId) as Order;
    //            order.AwaitingStock();
    //            _documentRepository.Save(order);

    //        }
    //        catch (Exception ex)
    //        {
    //            _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
    //            _log.Error("BackOrderCommandHandler exception", ex);
    //            throw ;
    //        }
    //    }
    //}
}
