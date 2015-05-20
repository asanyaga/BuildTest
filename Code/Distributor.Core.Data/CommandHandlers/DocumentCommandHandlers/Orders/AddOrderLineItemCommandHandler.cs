using System;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.Orders;
using Distributr.Core.Commands.DocumentCommands.Orders;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using log4net;

namespace Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.Orders
{
    //public class AddOrderLineItemCommandHandler : BaseCommandHandler, IAddOrderLineItemCommandHandler
    //{

    //    ILog _log = LogManager.GetLogger("AddOrderLineItemCommandHandler");
    //    private CokeDataContext _cokeDataContext;


    //    public AddOrderLineItemCommandHandler(CokeDataContext cokeDataContext)
    //        : base(cokeDataContext)
    //    {
    //        _cokeDataContext = cokeDataContext;
    //    }

    //    public void Execute(AddOrderLineItemCommand command)
    //    {
    //        _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
    //        try
    //        {
    //            if (!DocumentExists(command.DocumentId))
    //            {
    //                _log.InfoFormat("Cannot add line item. Document does not exist  Execute {1} - Command Id {0} ", command.CommandId, command.GetType());
    //                return;
    //            }
    //            if (DocumentLineItemExists(command.CommandId))
    //            {
    //                _log.InfoFormat("Cannot add line item {0}. Line item already exists", command.CommandId);
    //                return;
    //            }

    //            tblDocument doc = ExistingDocument(command.DocumentId);
    //            tblLineItems lineItem = NewLineItem(command.CommandId, command.DocumentId, command.ProductId,
    //                                               command.Description, command.Qty, command.LineItemSequenceNo);
    //            lineItem.Value = command.ValueLineItem;
    //            lineItem.Vat = command.LineItemVatValue;
    //            lineItem.DiscountLineItemTypeId = command.DiscountType;
    //            lineItem.ProductDiscount = command.ProductDiscount;
    //            lineItem.OrderLineItemType = command.LineItemType;

    //            doc.tblLineItems.Add(lineItem);
    //            _cokeDataContext.SaveChanges();
    //        }
    //        catch (Exception ex)
    //        {
    //            _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
    //            _log.Error("AddOrderLineItemCommandHandler exception", ex);
    //            throw ;
    //        }
    //    }

    //}
    public class AddMainOrderLineItemCommandHandler : BaseCommandHandler, IAddMainOrderLineItemCommandHandler
    {

        ILog _log = LogManager.GetLogger("AddMainOrderLineItemCommandHandler");
        private CokeDataContext _cokeDataContext;


        public AddMainOrderLineItemCommandHandler(CokeDataContext cokeDataContext)
            : base(cokeDataContext)
        {
            _cokeDataContext = cokeDataContext;
        }

        public void Execute(AddMainOrderLineItemCommand command)
        {
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
            try
            {
                if (!DocumentExists(command.DocumentId))
                {
                    _log.InfoFormat("Cannot add line item. Document does not exist  Execute {1} - Command Id {0} ", command.CommandId, command.GetType());
                    return;
                }
                if (DocumentLineItemExists(command.CommandId))
                {
                    _log.InfoFormat("Cannot add line item {0}. Line item already exists", command.CommandId);
                    return;
                }

                tblDocument doc = ExistingDocument(command.DocumentId);
                tblLineItems lineItem = NewLineItem(command.CommandId, command.DocumentId, command.ProductId,
                                                   command.Description, command.Qty, command.LineItemSequenceNo);
                lineItem.Value = command.ValueLineItem;
                lineItem.Vat = command.LineItemVatValue;
                lineItem.DiscountLineItemTypeId = command.DiscountType;
                lineItem.ProductDiscount = command.ProductDiscount;
                lineItem.OrderLineItemType = command.LineItemType;
                lineItem.LostSaleQuantity = 0;
                lineItem.BackOrderQuantity = 0;
                lineItem.ApprovedQuantity = 0;
                lineItem.DispatchedQuantity = 0;
                lineItem.InitialQuantity = command.Qty;
                lineItem.LineItemStatusId =(int) MainOrderLineItemStatus.New;
                doc.tblLineItems.Add(lineItem);
                _cokeDataContext.SaveChanges();
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
