using System;
using Distributr.Core.Domain.Master;
using SQLiteNetExtensions.Attributes;

namespace Distributr.Mobile.Core.OrderSale
{
    public enum LineItemStatus { New, Approved, Confirmed }

    public interface IProductLineItem
    {
        LineItemStatus LineItemStatus { get; set; }
        decimal Quantity { get; set; }
        decimal Price { get; set; }
        string Description { get;}
        decimal Available { get; set; }

        Guid ProductMasterId { get; set; }
        decimal Value { get; }
        decimal TotalLineItemValueInculudingVat { get; }
        decimal VatValue { get;}
        decimal VatRate { get; set; }
        decimal ProductDiscount { get; }
    }

    public abstract class BaseProductLineItem : MasterEntity, IProductLineItem
    {
        protected BaseProductLineItem()
            : base(default(Guid))
        {
        }

        protected BaseProductLineItem(Guid saleMasterId)
            : base(Guid.NewGuid())
        {
            SaleMasterId = saleMasterId;
        }

        [ForeignKey(typeof(Order))]
        public Guid SaleMasterId { get; set; }

        public LineItemStatus LineItemStatus { get; set; }
        public decimal Quantity { get; set; }
        public decimal ApprovedQuantity { get; set; }
        public decimal ConfirmedQuantity { get; set; }

        public decimal Price { get; set; }

        public decimal Available { get; set; }
        public decimal Value{ get { return SaleQuantity * Price; }}
        public decimal TotalLineItemValueInculudingVat{ get { return Value + VatValue; }}
        public decimal VatRate { get; set; }

        public decimal VatValue
        {
            get { return VatRate * Value; }
        }

        public virtual decimal SaleQuantity
        {
            get
            {
                return Quantity;
            }
            set
            {
                
            }
        }

        public abstract string Description { get; }
        public abstract Guid ProductMasterId { get; set; }
        public abstract  decimal ProductDiscount { get; }
    }
}