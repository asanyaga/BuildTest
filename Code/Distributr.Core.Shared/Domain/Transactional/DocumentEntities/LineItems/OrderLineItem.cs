using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.Domain.Transactional.DocumentEntities.LineItems
{
    /// <summary>
    /// Used to record when the line item was inserted. Used to extract lost sales
    /// </summary>
    [Obsolete("To be removed")]
    public enum OrderLineItemType 
    { 
        DuringConfirmation = 1,
        PostConfirmation = 2,
        Deleted = 3,
        ProcessedBackOrder = 4,
        BackOrder = 5,
        LostSale = 6,
        Discount = 7
    }
    public enum MainOrderLineItemStatus
    {
        None=0,
        New = 1,
        Confirmed = 2,
        Approved = 3,
        Dispatched = 4,
        Removed=5,
        Rejected = 6,
    }
    public enum MainOrderLineItemType
    {
        Sale=1,
        Discount= 2,

        Returned=3
    }
    public class OrderLineItem : ProductLineItem
    {
        public OrderLineItem(Guid id)
            : base(id)
        {

        }

        public OrderLineItemType LineItemType { get; set; }

        public decimal LineItemVatValue { get; set; }
        public decimal LineItemVatTotal
        {
            get { return Qty * LineItemVatValue; }
        }

        public virtual decimal LineItemTotal
        {
            get
            {
                return LineItemVatTotal + (Qty * Value);
            }
        }
    }
    public class SubOrderLineItem : ProductLineItem
    {
        public SubOrderLineItem(Guid id)
            : base(id)
        {

        }

        public MainOrderLineItemType LineItemType { get; set; }
        public MainOrderLineItemStatus LineItemStatus { get; set; }
        public decimal ApprovedQuantity { get; set; }
        public decimal LostSaleQuantity { get; set; }
        public decimal BackOrderQuantity { get; set; }
        public decimal DispatchedQuantity { get; set; }
        public decimal InitialQuantity { get; set; }
        
        public decimal LineItemVatValue { get; set; }
        public decimal LineItemVatTotal
        {
            get { return Qty * LineItemVatValue; }
        }

        public virtual decimal LineItemTotal
        {
            get
            {
                return LineItemVatTotal + (Qty * Value);
            }
        }

        public virtual decimal TotalNetPrice    
        {
            get { return (Qty*Value); }
            
        }

        public decimal TotalDiscount    
        {
            get { return (Qty * ProductDiscount); }
        }
    }
}
