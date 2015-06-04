using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.ProductEntities;

namespace Distributr.Mobile.Core.OrderSale
{
    public class Sale : BaseOrder
    {
        public Sale()
        {
        }

        public Sale(Guid id, Outlet outlet) : base(id, outlet)
        {
        }

        public void AddItem(SaleProduct product, decimal eachQuantity, decimal caseQuantity, decimal available, bool includeReturnables)
        {
            var item = AddItem(product, eachQuantity, caseQuantity);
            item.AvailableQuantity = available;
            item.SaleQuantity = item.Quantity;

            if (includeReturnables)
            {
                SellReturnablesForItem(item);
                item.IncludeReturnables = true;
            }
        }

        public void SellReturnablesForItem(SaleProductLineItem item)
        {
            var returnableItem = ReturnableLineItems.Find(i => i.ProductMasterId == item.Product.ReturnableProductMasterId && i.SaleQuantity == 0);
            if (returnableItem != null)
            {
                returnableItem.SaleQuantity = item.SaleQuantity;
            }

            var caseReturnableItem = ReturnableLineItems.Find(i => i.ProductMasterId == item.Product.ReturnableContainerMasterId && i.SaleQuantity == 0);
            if (caseReturnableItem != null)
            {
                caseReturnableItem.SaleQuantity = CaseQuantityFor(item.Product, item.SaleQuantity);
            }
        }

        public void UnsellReturnablesForItem(SaleProductLineItem item)
        {
            var returnableItem = ReturnableLineItems.Find(i => i.ProductMasterId == item.Product.ReturnableProductMasterId && i.SaleQuantity == item.SaleQuantity);
            if (returnableItem != null) returnableItem.SaleQuantity = 0;

            var caseQuantity = CaseQuantityFor(item.Product, item.SaleQuantity);
            var caseReturnableItem = ReturnableLineItems.Find(i => i.ProductMasterId == item.Product.ReturnableContainerMasterId && i.SaleQuantity == caseQuantity);
            if (caseReturnableItem != null) caseReturnableItem.SaleQuantity = 0;
        }

        public List<ReturnableLineItem> SoldReturnables()
        {
            return ReturnableLineItems.Where(i => i.SaleQuantity > 0).ToList();
        }
    }
}
