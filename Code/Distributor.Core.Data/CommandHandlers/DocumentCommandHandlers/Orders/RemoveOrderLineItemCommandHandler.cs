using System;
using System.Linq;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.Orders;
using Distributr.Core.Commands.DocumentCommands.Orders;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using log4net;

namespace Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.Orders
{
    public class RemoveOrderLineItemCommandHandler : IRemoveOrderLineItemCommandHandler
    {
        private IOrderRepository _orderRepository;
        private ILog _log = LogManager.GetLogger("RemoveOrderLineItemCommandHandler");

        public RemoveOrderLineItemCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public void Execute(RemoveOrderLineItemCommand command)
        {
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
            try
            {
                //check that order exists
                bool no_Order_Exist = _orderRepository.GetById(command.DocumentId) == null;
                if (no_Order_Exist)
                    return;
                //approve the order
                Order order = _orderRepository.GetById(command.DocumentId) as Order;
                order.RemoveLineItem(command.LineItemId);
                _orderRepository.Save(order);
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                _log.Error("RemoveOrderLineItemCommandHandler exception", ex);
                throw;
            }
        }
    }
    public class RemoveMainOrderLineItemCommandHandler : BaseCommandHandler, IRemoveMainOrderLineItemCommandHandler
    {

        ILog _log = LogManager.GetLogger("RemoveMainOrderLineItemCommandHandler");

        private CokeDataContext _cokeDataContext;
        public RemoveMainOrderLineItemCommandHandler(CokeDataContext context)
            : base(context)
        {
            _cokeDataContext = context;
        }

        public void Execute(RemoveMainOrderLineItemCommand command)
        {
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
            try
            {
                if (!DocumentExists(command.DocumentId))
                {
                    _log.InfoFormat("Cannot Edit line item. Document does not exist  Execute {1} - Command Id {0} ", command.CommandId, command.GetType());
                    return;
                }
                if (DocumentLineItemExists(command.LineItemId))
                {
                    tblDocument doc = ExistingDocument(command.DocumentId);
                    tblLineItems lineItem = doc.tblLineItems.First(s => s.id == command.LineItemId);
                    lineItem.LineItemStatusId = (int)MainOrderLineItemStatus.Removed; ;
                    _cokeDataContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                _log.Error("RemoveMainOrderLineItemCommandHandler exception", ex);
                throw;
            }
        }
    }
}
