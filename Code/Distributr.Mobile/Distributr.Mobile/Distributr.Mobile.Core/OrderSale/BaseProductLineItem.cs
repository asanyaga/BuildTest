using System;
using Distributr.Core.Domain.Master;
using SQLiteNetExtensions.Attributes;

namespace Distributr.Mobile.Core.OrderSale
{
    public enum LineItemStatus {New, Approved, Confirmed}

    public abstract class BaseProductLineItem : MasterEntity
    {
        protected BaseProductLineItem() : base(default(Guid))
        {
        }

        protected BaseProductLineItem(Guid lineItemId, Guid saleMasterId) : base(lineItemId)
        {
            SaleMasterId = saleMasterId;
        }

        [ForeignKey(typeof(Order))]
        public Guid SaleMasterId { get; set; }

        public decimal Quantity { get; set; }
        public decimal SaleQuantity { get; set; }
        public decimal AvailableQauntity { get; set; }

        public decimal Price { get; set; }
        public decimal Value{ get { return Quantity * Price; }}

        public decimal TotalLineItemValueInculudingVat{ get { return Value + VatValue; }}
        public decimal VatRate { get; set; }

        public decimal VatValue
        {
            get { return VatRate * Value; }
        }

        public LineItemStatus LineItemStatus { get; set; }

        public abstract Guid ProductMasterId { get; set; }
        public abstract decimal ProductDiscount { get; }
        public abstract string Description { get; }
    }
}