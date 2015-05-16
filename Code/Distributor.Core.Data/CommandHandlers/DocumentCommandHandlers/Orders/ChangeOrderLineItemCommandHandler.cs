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
    [Obsolete]
    public class ChangeOrderLineItemCommandHandler : IChangeOrderLineItemCommandHandler
    {
        private IOrderRepository _orderRepository;
        ILog _log = LogManager.GetLogger("ChangeOrderLineItemCommandHandler");

        public ChangeOrderLineItemCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public void Execute(ChangeOrderLineItemCommand command)
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
                order.ChangeLineItemQty(command.LineItemId, command.NewQuantity);
                _orderRepository.Save(order);

            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                _log.Error("ChangeOrderLineItemCommandHandler exception", ex);
                throw ;
            }
        }
    }
    public class ChangeMainOrderLineItemCommandHandler :BaseCommandHandler, IChangeMainOrderLineItemCommandHandler
    {
       
        ILog _log = LogManager.GetLogger("ChangeMainOrderLineItemCommandHandler");
        private CokeDataContext _cokeDataContext;

        public ChangeMainOrderLineItemCommandHandler(CokeDataContext context) : base(context)
        {
            _cokeDataContext = context;
        }


        public void Execute(ChangeMainOrderLineItemCommand command)
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
                    lineItem.Quantity = command.NewQuantity;
                    _cokeDataContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                _log.Error("AddOrderLineItemCommandHandler exception", ex);
                throw;
            }
        
        }
    }
}
