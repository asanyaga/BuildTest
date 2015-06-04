using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.ProductEntities;
using SQLiteNetExtensions.Attributes;

namespace Distributr.Mobile.Core.OrderSale
{
    public class BaseOrder : MasterEntity
    {
        public BaseOrder() : base(default(Guid))
        {
        }

        public BaseOrder(Guid id, Outlet outlet) : base(id)
        {
            ReturnableLineItems = new List<ReturnableLineItem>();
            ProductLineItems = new List<SaleProductLineItem>();
            Outlet = outlet;
        }

        [ForeignKey(typeof(Outlet))]
        public Guid OutletMasterId { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public Outlet Outlet { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public List<ReturnableLineItem> ReturnableLineItems { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public List<SaleProductLineItem> ProductLineItems { get; set; }

        //This method is called when constructing the order from commands as part of a DB rebuild or when the Hub modified the order
        public SaleProductLineItem AddItem(Guid lineItemId, SaleProduct product, Guid saleProductId, decimal quantity, decimal price, decimal vatRate)
        {
            var item = new SaleProductLineItem(lineItemId, Id)
            {
                Quantity = quantity,
                Product = product,
                ProductMasterId = product.Id,
                Price = price,
                VatRate = vatRate
            };

            ProductLineItems.Add(item);

            return item;
        }

        //This method is used when the user adds an item via the UI
        public SaleProductLineItem AddItem(SaleProduct product, decimal eachQuantity, decimal caseQuantity)
        {
            var quantity = eachQuantity + (caseQuantity * product.ContainerCapacity);

            var item = AddItem(Guid.NewGuid(), product, product.Id, quantity, PriceFor(product), VatRateFor(product));

            if (product.ReturnableProduct != null)
            {
                ReturnableLineItems.Add(
                    AddReturnableItem(Guid.NewGuid(), product.ReturnableProduct, product.ReturnableProduct.Id, quantity));
            }

            if (product.ReturnableContainer != null && caseQuantity > 0)
            {
                ReturnableLineItems.Add(
                    AddReturnableItem(Guid.NewGuid(), product.ReturnableContainer, product.ReturnableContainer.Id, caseQuantity));
            }

            return item;
        }

        public void RemoveItem(Guid lineItemId, bool removeReturnables = true)
        {
            var item = ProductLineItems.Find(i => i.Id == lineItemId);

            ProductLineItems.Remove(item);

            if (removeReturnables) RemoveReturnables(item);
        }

        public ReturnableLineItem AddReturnableItem(Guid lineItemId, ReturnableProduct product, Guid returnableProductId, decimal quantity)
        {
            return new ReturnableLineItem(lineItemId, Id)
            {
                Quantity = quantity,
                Product = product,
                ProductMasterId = returnableProductId
            };
        }

        protected void RemoveReturnables(SaleProductLineItem item)
        {
            var eachReturnable = ReturnableLineItems
                .First(i => i.ProductMasterId == item.Product.ReturnableProductMasterId && i.Quantity == item.Quantity);

            ReturnableLineItems.Remove(eachReturnable);

            var caseQuantity = CaseQuantityFor(item.Product, item.Quantity);
            if (caseQuantity > 0)
            {
                var caseReturnable = ReturnableLineItems
                    .First(i => i.ProductMasterId == item.Product.ReturnableContainerMasterId && i.Quantity == caseQuantity);
                ReturnableLineItems.Remove(caseReturnable);
            }
        }

        public decimal CaseQuantityFor(SaleProduct product, decimal eachQuantity)
        {
            return Math.Floor(eachQuantity / product.ContainerCapacity);
        }

        protected decimal VatRateFor(Product product)
        {
            return product.VATClass.CurrentRate;
        }

        protected decimal PriceFor(Product product)
        {
            return product.ProductPrice(Outlet.OutletProductPricingTier);
        }
    }

    public abstract class BaseLineItem : MasterEntity
    {
        protected BaseLineItem() : base(default(Guid))
        {
        }

        protected BaseLineItem(Guid lineItemId, Guid orderMasterId) : base(lineItemId)
        {
            OrderMasterId = orderMasterId;
        }

        public Guid OrderMasterId { get; set; }

        public decimal Quantity { get; set; }
        public decimal SaleQuantity { get; set; }
        public decimal AvailableQuantity { get; set; }

        public decimal Price { get; set; }
        public decimal VatRate { get; set; }

        public abstract Guid ProductMasterId { get; set; }
    }

    public class SaleProductLineItem : BaseLineItem
    {
        public SaleProductLineItem() : base(default(Guid), default(Guid))
        {            
        }

        public SaleProductLineItem(Guid lineItemId, Guid orderGuid): base(lineItemId, orderGuid)
        {
        }

        public bool IncludeReturnables { get; set; }

        public override Guid ProductMasterId { get; set; }
        public SaleProduct Product { get; set; }
    }

    public class ReturnableLineItem : BaseLineItem
    {
        public ReturnableLineItem() : base(default(Guid), default(Guid))
        {
        }

        public ReturnableLineItem(Guid lineItemId, Guid orderGuid) : base(lineItemId, orderGuid)
        {
        }

        public override Guid ProductMasterId { get; set; }
        public ReturnableProduct Product { get; set; }
    }
}
