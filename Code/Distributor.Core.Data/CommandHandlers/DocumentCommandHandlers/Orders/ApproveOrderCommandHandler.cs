using System;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.Orders;
using Distributr.Core.Commands.DocumentCommands.Orders;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using log4net;
using System.Linq;

namespace Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.Orders
{
    //public class ApproveOrderCommandHandler : IApproveOrderCommandHandler
    //{
    //    IOrderRepository _orderRepository;
    //    IDistributorRepository _distributorReopsitory;
    //    ILog _log = LogManager.GetLogger("ApproveOrderCommandHandler");

    //    public ApproveOrderCommandHandler(IOrderRepository orderRepository, IDistributorRepository distributorReopsitory)
    //    {
    //        _orderRepository = orderRepository;
    //        _distributorReopsitory = distributorReopsitory;
    //    }

    //    public void Execute(ApproveOrderCommand command)
    //    {
    //        _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
    //        try
    //        {
    //            //check that order exists
    //            bool no_Order_Exist = _orderRepository.GetById(command.DocumentId) == null;
    //            if (no_Order_Exist)
    //                return;
    //            //approve the order
    //            Order order = _orderRepository.GetById(command.DocumentId) as Order;
    //            order.Approve();
    //            _orderRepository.Save(order);
    //        }
    //        catch (Exception ex)
    //        {
    //            _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
    //            _log.Error("ApproveOrderCommandHandler exception", ex);
    //            throw ;
    //        }
    //    }

    //}

    public class ApproveMainOrderCommandHandler :BaseCommandHandler, IApproveMainOrderCommandHandler
    {

        ILog _log = LogManager.GetLogger("ApproveMainOrderCommandHandler");
        private CokeDataContext _cokeDataContext;


        public ApproveMainOrderCommandHandler(CokeDataContext context, CokeDataContext cokeDataContext) : base(context)
        {
            _cokeDataContext = cokeDataContext;
        }

        public void Execute(ApproveMainOrderCommand command)
        {
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
            try
            {

                if (!DocumentExists(command.DocumentId))
                    return;
                ApproveDocument(command.DocumentId);
               
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                _log.Error("ApproveMainOrderCommandHandler exception", ex);
                throw;
            }
        }

    }
}
