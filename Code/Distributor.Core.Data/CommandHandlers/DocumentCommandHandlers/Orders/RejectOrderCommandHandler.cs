using System;
using System.Linq;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.Orders;
using Distributr.Core.Commands.DocumentCommands.Orders;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using log4net;

namespace Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.Orders
{
    //[Obsolete]
    //public class RejectOrderCommandHandler : IRejectOrderCommandHandler
    //{
    //    IOrderRepository _orderRepository;
    //    IDistributorRepository _distributorRepository;
    //    ILog _log = LogManager.GetLogger("RejectOrderCommandHandler");

    //    public RejectOrderCommandHandler(IOrderRepository orderRepository, IDistributorRepository distributorReopsitory)
    //    {
    //        _orderRepository = orderRepository;
    //        _distributorRepository = distributorReopsitory;
    //    }

    //    public void Execute(RejectOrderCommand command)
    //    {
    //        _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
    //        try
    //        {
    //            //order exists
    //            bool no_Order_Exist = _orderRepository.GetById(command.DocumentId) == null;
    //            if (no_Order_Exist)
    //                return;
    //            //Reject an order
    //            Order order = _orderRepository.GetById(command.DocumentId) as Order;
    //            order.Reject();

    //            _orderRepository.Save(order);
    //        }
    //        catch (Exception ex)
    //        {
    //            _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
    //            _log.Error("RejectOrderCommandHandler exception", ex);
    //            throw ;
    //        }
    //    }

    //}

    public class RejectMainOrderCommandHandler :BaseCommandHandler, IRejectMainOrderCommandHandler
    {
        
        ILog _log = LogManager.GetLogger("RejectOrderCommandHandler");
        private CokeDataContext _ctx;

        public RejectMainOrderCommandHandler(CokeDataContext context) : base(context)
        {
            _ctx = context;
        }

        public void Execute(RejectMainOrderCommand command)
        {
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
           
            try
            {

                if (!DocumentExists(command.DocumentId))
                    return;
                RejectDocument(command.DocumentId);
                var doc = _ctx.tblDocument.FirstOrDefault(s => s.Id == command.DocumentId);
                foreach (var tblLine in doc.tblLineItems)
                {
                    tblLine.LineItemStatusId = (int)MainOrderLineItemStatus.Rejected;
                    tblLine.LostSaleQuantity = tblLine.Quantity;
                }
                _ctx.SaveChanges();



            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                _log.Error("RejectOrderCommandHandler exception", ex);
                throw;
            }
        }

    }
}
