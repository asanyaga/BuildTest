using System;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.Orders;
using Distributr.Core.Commands.DocumentCommands.Orders;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using log4net;

namespace Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.Orders
{
    //public class OrderPendingDispatchCommandHandler : IOrderPendingDispatchCommandHandler
    //{
    //    private IOrderRepository _documentRepository;
    //    ILog _log = LogManager.GetLogger("OrderPendingDispatchCommandHandler");

    //    public OrderPendingDispatchCommandHandler(IOrderRepository documentRepository)
    //    {
    //        _documentRepository = documentRepository;
    //    }

    //    public void Execute(OrderPendingDispatchCommand command)
    //    {
    //        _log.Info("OrderPendingDispatchCommand is null:"+(command==null));
    //        _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
    //        try
    //        {
    //            //check that order exists
    //            bool no_Order_Exist = _documentRepository.GetById(command.DocumentId) == null;
    //            if (no_Order_Exist)
    //            {
    //                _log.Info("no_Order_Exist");
    //                return;
    //            }
    //            else {
    //                _log.Info("Order_Exist");
    //            }
    //            //approve the order
    //            Order order = _documentRepository.GetById(command.DocumentId) as Order;
    //            order.PendingDispatch();
    //            _documentRepository.Save(order);
    //        }
    //        catch (Exception ex)
    //        {
    //            _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
    //            _log.Error("OrderPendingDispatchCommandHandler exception",  ex);
    //            throw ;
    //        }
    //    }
    //}
}
