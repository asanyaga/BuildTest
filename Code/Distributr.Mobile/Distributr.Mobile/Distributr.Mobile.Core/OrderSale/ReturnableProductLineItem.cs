using System;
using Distributr.Core.Domain.Master.ProductEntities;
using SQLiteNetExtensions.Attributes;

namespace Distributr.Mobile.Core.OrderSale
{
    public class ReturnableProductLineItem : BaseProductLineItem
    {
        public ReturnableProductLineItem()
            : base(default(Guid))
        {
        }

        public ReturnableProductLineItem(Guid saleMasterId)
            : base(Guid.NewGuid())
        {
            SaleMasterId = saleMasterId;
        }

        public override string Description
        {
            get { return Product.Description; }
        }

        public override decimal SaleQuantity { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public ReturnableProduct Product { get; set; }

        [ForeignKey(typeof(ReturnableProduct))]
        public override Guid ProductMasterId { get; set; }

        public override decimal ProductDiscount
        {
            get { return 0; }
        }
    }
}