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
    public class OrderDispatchedToPhoneCommandHandler : IOrderDispatchedToPhoneCommandHandler
    {
        private IOrderRepository _documentRepository;
        ILog _log = LogManager.GetLogger("OrderDispatchedToPhoneCommandHandler");

        public OrderDispatchedToPhoneCommandHandler(IOrderRepository documentRepository)
        {
            _documentRepository = documentRepository;
        }

        public void Execute(Commands.DocumentCommands.Orders.DispatchToPhoneCommand command)
        {
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
            try
            {
                //check that order exists
                bool no_Order_Exist = _documentRepository.GetById(command.DocumentId) == null;
                if (no_Order_Exist)
                    return;
                //approve the order
                Order order = _documentRepository.GetById(command.DocumentId) as Order;
                order.DispatchedToPhone();
                _documentRepository.Save(order);
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                _log.Error("OrderDispatchedToPhoneCommandHandler exception", ex);
                throw ;
            }
        }
    }
    public class OrderDispatchApprovedLineItemsCommandHandler :BaseCommandHandler, IOrderDispatchApprovedLineItemsCommandHandler
    {
        
        ILog _log = LogManager.GetLogger("OrderDispatchedToPhoneCommandHandler");
        private CokeDataContext _cokeDataContext;


        public OrderDispatchApprovedLineItemsCommandHandler(CokeDataContext context) : base(context)
        {
            _cokeDataContext = context;

        }

        public void Execute(OrderDispatchApprovedLineItemsCommand command)
        {
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
            try
            {

                if (!DocumentExists(command.DocumentId))
                    return;
               
                var doc = _cokeDataContext.tblDocument.FirstOrDefault(s => s.Id == command.DocumentId);
                foreach (var tblLine in doc.tblLineItems.Where(s=>s.LineItemStatusId==(int)MainOrderLineItemStatus.Approved))
                {
                    tblLine.LineItemStatusId = (int)MainOrderLineItemStatus.Dispatched;
                    tblLine.DispatchedQuantity = tblLine.ApprovedQuantity;

                }
                //doc.OrderStatusId = (int)OrderStatus.Inprogress;
                _cokeDataContext.SaveChanges();
                
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                _log.Error("OrderDispatchedToPhoneCommandHandler exception", ex);
                throw ;
            }
        }
    }
    
}
