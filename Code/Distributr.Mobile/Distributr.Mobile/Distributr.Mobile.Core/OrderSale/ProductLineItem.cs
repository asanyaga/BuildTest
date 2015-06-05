using System;
using Distributr.Core.Domain.Master.ProductEntities;
using SQLiteNetExtensions.Attributes;

namespace Distributr.Mobile.Core.OrderSale
{
    public class ProductLineItem : BaseProductLineItem
    {        
        public ProductLineItem()
        {
        }

        public ProductLineItem(Guid lineItemId, Guid saleMasterId) : base(lineItemId, saleMasterId)
        {
            SaleMasterId = saleMasterId;
        }

        [ForeignKey(typeof(SaleProduct))]
        public override Guid ProductMasterId { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public SaleProduct Product { get; set; }

        public bool SellReturnables { get; set; }

        public override decimal ProductDiscount
        {
            get { return 0; } //TODO discounts 
        }

        public override string Description
        {
            get { return Product.Description; }
        }
    }
}