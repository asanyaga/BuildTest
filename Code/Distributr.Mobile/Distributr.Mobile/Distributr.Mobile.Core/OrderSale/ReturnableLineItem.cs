using System;
using Distributr.Core.Domain.Master.ProductEntities;
using SQLiteNetExtensions.Attributes;

namespace Distributr.Mobile.Core.OrderSale
{
    public class ReturnableLineItem : BaseProductLineItem
    {
        public ReturnableLineItem()            
        {
        }

        public ReturnableLineItem(Guid lineItemId, Guid saleMasterId) : base(lineItemId, saleMasterId)
        {
            SaleMasterId = saleMasterId;
        }

        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public ReturnableProduct Product { get; set; }

        [ForeignKey(typeof(ReturnableProduct))]
        public override Guid ProductMasterId { get; set; }

        public override decimal ProductDiscount
        {
            get { return 0; }
        }

        public override string Description
        {
            get { return Product.Description; }
        }
    }
}