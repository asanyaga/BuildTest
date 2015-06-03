using System;
using Distributr.Core.Domain.Master.ProductEntities;
using SQLiteNetExtensions.Attributes;

namespace Distributr.Mobile.Core.OrderSale
{
    public class ProductLineItem : BaseProductLineItem
    {
        
        public ProductLineItem() : base(default(Guid))
        {
        }

        public ProductLineItem(Guid id, Guid saleMasterId)
            : base(id)
        {
            SaleMasterId = saleMasterId;
        }

        public override string Description
        {
            get { return Product.Description; }
        }

        public decimal EachQuantity { get; set; }
        public decimal CaseQuantity { get; set; }

        [ForeignKey(typeof(ReturnableProductLineItem))]
        public Guid ItemReturnableMasterId { get; set; }

        [OneToOne("ItemReturnableMasterId", CascadeOperations = CascadeOperation.All)]
        public ReturnableProductLineItem ItemReturnable { get; set; }

        [ForeignKey(typeof(ReturnableProductLineItem))]
        public Guid ContainerReturnableMasterId { get; set; }

        [OneToOne("ContainerReturnableMasterId", CascadeOperations = CascadeOperation.All)]
        public ReturnableProductLineItem ContainerReturnable { get; set; }


        [ForeignKey(typeof(SaleProduct))]
        public override Guid ProductMasterId { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public SaleProduct Product { get; set; }
       
        public override decimal ProductDiscount
        {
            get { return 0; } //TODO discounts 
        }        
    }
}