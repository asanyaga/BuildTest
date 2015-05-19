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
    //public class ConfirmOrderCommandHandler : BaseCommandHandler, IConfirmOrderCommandHandler
    //{
    //    private IOrderRepository _documentRepository;
    //    private CokeDataContext _cokeDataContext;
    //    private ILog _log = LogManager.GetLogger("ConfirmOrderCommandHandler");

    //    public ConfirmOrderCommandHandler(
    //        IOrderRepository documentRepository, CokeDataContext cokeDataContext)
    //        : base(cokeDataContext)
    //    {
    //        _documentRepository = documentRepository;
    //        _cokeDataContext = cokeDataContext;
    //    }

    //    public void Execute(ConfirmOrderCommand command)
    //    {
    //        _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
    //        try
    //        {
    //            //CN: todo straighten out the differences later
    //            Order order = _documentRepository.GetById(command.DocumentId) as Order;
    //            if (order == null)
    //                return;
    //            if (order.OrderType == OrderType.DistributorPOS || order.OrderType == OrderType.OutletToDistributor)
    //            {
    //                if (!DocumentExists(command.DocumentId))
    //                    return;
    //                ConfirmDocument(command.DocumentId);
    //                return;
    //            }
    //            order.Confirm();
    //            _documentRepository.Save(order);
    //        }
    //        catch (Exception ex)
    //        {
    //            _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
    //            _log.Error("ConfirmOrderCommandHandler exception ", ex);
    //            throw;
    //        }
    //    }


    //}
    public class ConfirmMainOrderCommandHandler : BaseCommandHandler, IConfirmMainOrderCommandHandler
    {

       
        private CokeDataContext _cokeDataContext;
        ILog _log = LogManager.GetLogger("ConfirmMainOrderCommandHandler");

        public ConfirmMainOrderCommandHandler(
             CokeDataContext cokeDataContext)
            : base(cokeDataContext)
        {
           
            _cokeDataContext = cokeDataContext;
        }

        public void Execute(ConfirmMainOrderCommand command)
        {
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
            try
            {
               
               
                    if (!DocumentExists(command.DocumentId))
                        return;
                     ConfirmDocument(command.DocumentId);
                var doc = _cokeDataContext.tblDocument.FirstOrDefault(s => s.Id == command.DocumentId);
                foreach (var tblLine in doc.tblLineItems)
                {
                    tblLine.LineItemStatusId =(int) MainOrderLineItemStatus.Confirmed;

                }
                doc.OrderStatusId =(int) OrderStatus.Inprogress;
                _cokeDataContext.SaveChanges();

            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                _log.Error("ConfirmOrderCommandHandler exception ", ex);
                throw;
            }
        }

    }
}
