using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Commands.DocumentCommands.Orders;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Domain.Transactional.DocumentEntities.Exceptions;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Utility;

namespace Distributr.Core.Domain.Transactional.DocumentEntities
{
    public enum OrderType { DistributorPOS = 1, DistributorToProducer = 2, OutletToDistributor = 3, SalesmanToDistributor = 4}
   // public enum PaymentType { Cash = 1, MPESA = 2, Cheque = 3 }
    /// <summary>
    /// Need to store all line items from confirmation of order.
    /// Once confirmed then line items are cloned and marked as post confirmation
    /// </summary>
    public class Order : Document
    {
        public Order(Guid id)
            : base(id)
        {
            _lineItems = new List<OrderLineItem>();
        }

        public Order(Guid id, bool isNew, string documentReference,
            CostCentre issuedOnBehalfOf,
            DateTime dateRequired,
            CostCentre documentIssuerCostCentre,
            Guid documentIssuerCostCentreApplicationId,
            User documentIssuerUser,
            DateTime documentDateIssued,
            CostCentre documentRecipientCostCentre,
            DocumentStatus status,
            OrderType orderType,
            decimal saleDiscount,
            List<OrderLineItem> lineItems,
            string note = ""//cn
            )
            : base(id, documentReference, documentIssuerCostCentre, documentIssuerCostCentreApplicationId, documentIssuerUser,
            documentDateIssued,
            documentRecipientCostCentre, status)
        {
            OrderType = orderType;
            _lineItems = lineItems;
            IssuedOnBehalfOf = issuedOnBehalfOf;
            DateRequired = dateRequired;
            this.DocumentType = DocumentType.Order;
            SaleDiscount = saleDiscount;
            Note = note;
        }
        
        public Order(Guid id, bool isNew, string documentReference,
            CostCentre issuedOnBehalfOf,
            DateTime dateRequired,
            CostCentre documentIssuerCostCentre,
            Guid documentIssuerCostCentreApplicationId,
            User documentIssuerUser,
            DateTime documentDateIssued,
            CostCentre documentRecipientCostCentre,
            DocumentStatus status,
            OrderType orderType,
            decimal saleDiscount,
            string note = ""//cn
            )
            : base(id, documentReference, documentIssuerCostCentre, documentIssuerCostCentreApplicationId, documentIssuerUser,
            documentDateIssued,
            documentRecipientCostCentre, 
            status
            )
        {
            _lineItems = new List<OrderLineItem>();
            OrderType = orderType;          
            IssuedOnBehalfOf = issuedOnBehalfOf;
            DateRequired = dateRequired;
            this.DocumentType = DocumentType.Order;
            SaleDiscount = saleDiscount;
            Note = note;
        }

        public CostCentre IssuedOnBehalfOf { get; internal set; }
        public DateTime DateRequired { get; internal set; }
        public OrderType OrderType { get; internal set; }
        public decimal SaleDiscount { get; set; }
        public string Note { get; set; }
       // public int ProductBrand { get; set; }

        public void AddLineItem(OrderLineItem orderLineItem)
        {
            //cn: for discount line items
            if (orderLineItem.LineItemType == OrderLineItemType.Discount)
            {
                CreateLineItem(orderLineItem);
                _AddAddLineItemCommandToExecute(orderLineItem);
                return;
            }
            if (Status == DocumentStatus.New)
            {
                orderLineItem.IsNew = true;
                orderLineItem.LineItemType = OrderLineItemType.DuringConfirmation;
                _lineItems.Add(orderLineItem);

            }
            else if (OrderType == OrderType.DistributorToProducer && Status == DocumentStatus.Confirmed)
            {
                orderLineItem.LineItemType = OrderLineItemType.PostConfirmation;
                _lineItems.Add(orderLineItem);
            }
            else
            {
                orderLineItem.LineItemType = OrderLineItemType.PostConfirmation;
                    _lineItems.Add(orderLineItem);
            }
            _AddAddLineItemCommandToExecute(orderLineItem);
        }

        public void RemoveLineItem(Guid lineItemId)
        {
            //can only remove line items from an order in confirmed state
            bool execute = false;
            if (Status == DocumentStatus.New)
                execute = true;
            if (Status == DocumentStatus.Confirmed)
                execute = true;
            if (Status == DocumentStatus.Approved)
                execute = true;
            if (Status == DocumentStatus.OrderPendingDispatch) //cn: to remove back order items after they are processed.
                execute = true;
            if (Status == DocumentStatus.OrderDispatchedToPhone) //cn: ata ii
                execute = true;
            if (!execute)
                return;
            OrderLineItem li = LineItems.FirstOrDefault(n => n.Id == lineItemId);
            if (li == null) return;
            li.LineItemType = OrderLineItemType.Deleted;
        }

        public void ChangeLineItemQty(Guid lineItemId, decimal newQuantity, decimal newProductDiscount = 0)
        {
            bool execute = false;
            if (Status == DocumentStatus.New)
                execute = true;
            if (Status == DocumentStatus.Confirmed)
                execute = true;
            if (Status == DocumentStatus.Approved)
                execute = true;
            if (Status == DocumentStatus.OrderPendingDispatch) //cn:change item qty after they are processed.
                execute = true;
            if (Status == DocumentStatus.OrderDispatchedToPhone) //cn: ata ii
                execute = true;
            if (!execute)
                return;
            OrderLineItem li = LineItems.FirstOrDefault(n => n.Id == lineItemId);
            if (li == null) return;
            li.ProductDiscount = newProductDiscount;
            li.Qty = newQuantity;
        }

        private List<OrderLineItem> _lineItems;
        public List<OrderLineItem> LineItems
        {
            get
            {

                if (OrderType == OrderType.DistributorPOS)
                {
                    if (Status == DocumentStatus.New)
                        return _lineItems.Where(n => n.LineItemType == OrderLineItemType.DuringConfirmation ||
                                                     n.LineItemType == OrderLineItemType.Discount //cn
                            ).ToList();
                    
                    return _lineItems.Where(n => n.LineItemType == OrderLineItemType.PostConfirmation ||
                                                 n.LineItemType == OrderLineItemType.Discount //cn
                        ).ToList();
                }
                if (OrderType == OrderType.DistributorToProducer)
                    return _lineItems.Where(n => n.LineItemType == OrderLineItemType.DuringConfirmation).ToList();

                if (Status == DocumentStatus.New)
                    return _lineItems.Where(n => 
                        n.LineItemType == OrderLineItemType.DuringConfirmation || 
                        n.LineItemType == OrderLineItemType.Discount//cn
                        ).ToList();

                return _lineItems.Where(n => 
                    n.LineItemType == OrderLineItemType.PostConfirmation ||
                    n.LineItemType == OrderLineItemType.ProcessedBackOrder || //cn
                    n.LineItemType == OrderLineItemType.BackOrder ||  //cn
                    n.LineItemType == OrderLineItemType.LostSale || //cn
                    n.LineItemType == OrderLineItemType.Discount //cn
                    ).ToList();
            }
            //set { _lineItems = value; }
        }

        public List<OrderLineItem> OriginalOrderLineItems
        {
            get { return _lineItems.Where(n => n.LineItemType == OrderLineItemType.DuringConfirmation).ToList(); }

        }
        public List<OrderLineItem> PurchaseOrderLineItems
        {
            get
            {
                if (Status == DocumentStatus.New)
                    return _lineItems.Where(n => n.LineItemType == OrderLineItemType.DuringConfirmation).ToList();
                else
                    return _lineItems.Where(n => n.LineItemType == OrderLineItemType.DuringConfirmation).ToList();
            }

        }
        public decimal TotalNet { get
        {
            return
                LineItems.Where(
                    n => n.LineItemType != OrderLineItemType.BackOrder && n.LineItemType != OrderLineItemType.LostSale &&
                    n.LineItemType != OrderLineItemType.ProcessedBackOrder).
                    Sum(n => n.Value*n.Qty);
        } }
        public decimal TotalVat { get
        {
            return
                LineItems.Where(
                    n =>
                    n.LineItemType != OrderLineItemType.BackOrder && n.LineItemType != OrderLineItemType.LostSale &&
                    n.LineItemType != OrderLineItemType.ProcessedBackOrder).
                    Sum(n => n.LineItemVatTotal);
        } }
        public decimal TotalGross { get { return (TotalNet + TotalVat) - SaleDiscount; } }

        public override void Confirm()
        {
            if (Status != DocumentStatus.New)
                throw new InvalidDocumentOperationException("Cannot confirm an order that is not new");
            if (_lineItems.Count() == 0)
                throw new InvalidDocumentOperationException("Must add at least one lineitem to order before confirming");
            Status = DocumentStatus.Confirmed;
            List<OrderLineItem> unconfirmedLineItems = OriginalOrderLineItems;//.Where(n => n.LineItemType == OrderLineItemType.DuringConfirmation).ToList();
            foreach (var item in unconfirmedLineItems)
            {
                OrderLineItem confirmedOrderLineItem = new OrderLineItem(Guid.NewGuid())
                                                           {
                                                               Description = item.Id.ToString(),//cn:: keep track of the original line item necessary for tracking line items back order and/or lost sale
                                                               IsNew = true,
                                                               LineItemSequenceNo = item.LineItemSequenceNo,
                                                               LineItemType = OrderLineItemType.PostConfirmation,
                                                               LineItemVatValue = item.LineItemVatValue,
                                                               Product = item.Product,
                                                               Qty = item.Qty,
                                                               Value = item.Value,
                                                               DiscountType = item.DiscountType,
                                                               ProductDiscount = item.ProductDiscount
                                                           };
                AddLineItem(confirmedOrderLineItem);
            }
            _AddCreateCommandToExecute();
            _AddConfirmCommandToExecute();
        }

        public  void Approve()
        {
            DocumentStatus[] validStatus = new DocumentStatus[] { DocumentStatus.Confirmed, DocumentStatus.OrderBackOrder };

            if (!validStatus.Contains(Status))
                throw new InvalidDocumentOperationException("Cannot Approve an order that is not Confirmed");
            if (LineItems.Count() == 0)
                throw new InvalidDocumentOperationException("Must add at least one lineitem to order before approving");
            Status = DocumentStatus.Approved;
        }

        public  void Reject()
        {
            if (Status != DocumentStatus.Confirmed)
                throw new InvalidDocumentOperationException("Order needs to be Confirmed");
            Status = DocumentStatus.Rejected;
        }

        public  void Close()
        {
            //if (Status != DocumentStatus.Approved || Status != DocumentStatus.OrderDispatchedToPhone || Status != DocumentStatus.Rejected)
            //{
            //    //throw new InvalidDocumentOperationException("Order needs to be Approved");
            //}
            //else
            Status = DocumentStatus.Closed;
        }

        protected override void _AddCreateCommandToExecute(bool isHybrid = false)
        {
            var coc = new CreateOrderCommand(
                Guid.NewGuid(),
                Id,
                DocumentIssuerUser.Id,
                DocumentIssuerCostCentre.Id,
                0,
                DocumentIssuerCostCentreApplicationId,
                DocumentReference,
                DocumentDateIssued,
                DateRequired,
                IssuedOnBehalfOf.Id,
                DocumentIssuerCostCentre.Id,
                DocumentRecipientCostCentre.Id,
                DocumentIssuerUser.Id,
                (int)OrderType,
                "",
                SaleDiscount
                );
            _AddCommand(coc);
        }

        protected override void _AddAddLineItemCommandToExecute<T>(T lineItem, bool isHybrid = false)
        {
            var item = lineItem as OrderLineItem;
            var ali = new AddOrderLineItemCommand(
                    item.Id,
                    Id,
                    DocumentIssuerUser.Id,
                    DocumentIssuerCostCentre.Id,
                    0,
                    DocumentIssuerCostCentreApplicationId,
                    item.LineItemSequenceNo,
                    item.Value,
                    item.Product.Id,
                    item.Qty,
                    item.LineItemVatValue,
                    item.ProductDiscount,
                    item.Description,
                    (int)item.LineItemType,
                    (int)item.DiscountType
                    );
            _AddCommand(ali);
        }

        protected override void _AddConfirmCommandToExecute(bool isHybrid = false)
        {
            var co = new ConfirmOrderCommand(Guid.NewGuid(), 
                Id,
                DocumentIssuerUser.Id,
                DocumentIssuerCostCentre.Id,
                0,
                DocumentIssuerCostCentreApplicationId, 
                DocumentParentId
                );
            _AddCommand(co);
        }

        public List<OrderLineItem> _allLineItems()
        {
            return _lineItems;
        }

        public void _SetLineItems(List<OrderLineItem> items)
        {
            _lineItems = items;
        }

        public void ClearLineItems()
        {
            _lineItems.Clear();
        }

        public void AwaitingStock()
        {
            if (Status != DocumentStatus.Confirmed)
                throw new InvalidDocumentOperationException("Only a confirmed order can be set to Awaiting Stock status");
            Status = DocumentStatus.OrderBackOrder;
        }

        public void PendingDispatch()
        {
            if (Status != DocumentStatus.Approved)
                throw new InvalidDocumentOperationException("Only an approved order can be set to pending dispatch status");
            Status = DocumentStatus.OrderPendingDispatch;
        }

        public void DispatchedToPhone()
        {
            if (Status != DocumentStatus.OrderPendingDispatch)
                throw new InvalidDocumentOperationException("Only an order with order pending dispatch can be set to dispatched to phone status");
            Status = DocumentStatus.OrderDispatchedToPhone;
        }

        public void CreateLineItem(OrderLineItem orderLineItem)
        {
            _lineItems.Add(orderLineItem);
        }

        ////cn
        //public void RemoveBackOrderLineItem(Guid lineItemId)
        //{
        //    if (Status != DocumentStatus.OrderPendingDispatch)
        //        return;
        //    OrderLineItem li = LineItems.FirstOrDefault(n => n.Id == lineItemId);
        //    if (li == null) return;
        //    li.LineItemType = OrderLineItemType.Deleted;
        //}

        //////cn: 
        ////public static bool operator ==(Order a, Order b)
        ////{
        ////    try
        ////    {
        ////        if (a.Id == Guid.Empty ||  b.Id == Guid.Empty)
        ////        {
        ////            if (object.ReferenceEquals(a, b))
        ////            {
        ////                return true;
        ////            }
        ////            return false;
        ////        }
        ////        else
        ////        {
        ////            if (a.Id == b.Id)
        ////            {
        ////                return true;
        ////            }
        ////            return false;
        ////        }
        ////    }
        ////    catch { return false; }
        ////}

        ////public static bool operator !=(Order a, Order b)
        ////{
        ////    if (a == b)
        ////        return false;
        ////    return true;
        ////}
    }
}
