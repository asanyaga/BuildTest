using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.ProductEntities;
using SQLite.Net.Attributes;

namespace Distributr.Mobile.Core.OrderSale
{
    [Table("OrderOrSale")]
    public class Sale : Order
    {
        public Sale()
        {
        }

        public Sale(Guid id, Outlet outlet) : base(id, outlet)
        {
        }

        public void AddItem(SaleProduct product, decimal quantity, decimal available, bool includeReturnables)
        {
            var item = AddItem(product, quantity);
            item.AvailableQauntity = available;
            item.SaleQuantity = item.Quantity;

            if (includeReturnables)
            {
                SellReturnablesForItem(item);
            }
        }

        public void SellReturnablesForItem(ProductLineItem item)
        {
            var returnableItem = ReturnableLineItems.Find(i => i.ProductMasterId == item.Product.ReturnableProductMasterId && i.SaleQuantity == 0);
            if (returnableItem != null)
            {
                returnableItem.SaleQuantity = item.SaleQuantity;
                returnableItem.LineItemStatus = LineItemStatus.Approved;
            }

            var caseReturnableItem = ReturnableLineItems.Find(i => i.ProductMasterId == item.Product.ReturnableContainerMasterId && i.SaleQuantity == 0);
            if (caseReturnableItem != null)
            {
                caseReturnableItem.SaleQuantity = CaseQuantityFor(item.Product, item.SaleQuantity);
                caseReturnableItem.LineItemStatus = LineItemStatus.Approved;
            }
            item.SellReturnables = true;
        }

        public void UnsellReturnablesForItem(ProductLineItem item)
        {
            var returnableItem = ReturnableLineItems.Find(i => i.ProductMasterId == item.Product.ReturnableProductMasterId && i.SaleQuantity == item.SaleQuantity);
            if (returnableItem != null)
            {
                returnableItem.SaleQuantity = 0;
                returnableItem.LineItemStatus = LineItemStatus.New;
            }

            var caseQuantity = CaseQuantityFor(item.Product, item.SaleQuantity);
            var caseReturnableItem = ReturnableLineItems.Find(i => i.ProductMasterId == item.Product.ReturnableContainerMasterId && i.SaleQuantity == caseQuantity);
            if (caseReturnableItem != null)
            {
                caseReturnableItem.SaleQuantity = 0;
                caseReturnableItem.LineItemStatus = LineItemStatus.New;
            }
            item.SellReturnables = false;
        }

        public List<ReturnableLineItem> SoldReturnables()
        {
            return ReturnableLineItems.Where(i => i.SaleQuantity > 0).ToList();
        }
    }
}
