using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.Domain.Master.CostCentreEntities;

namespace Distributr.Mobile.Core.OrderSale
{
    public class Delivery : BaseOrder
    {
        public Delivery()
        {
        }

        public Delivery(Guid id, Outlet outlet) : base(id, outlet)
        {
        }

        public void IncludeReturnablesForItem(SaleProductLineItem item)
        {
            
        }

        public void ExcludeReturnablesForItem(SaleProductLineItem item)
        {
            
        }

        public List<SaleProductLineItem> DeliverableLineItem()
        {
            return ProductLineItems.Where(i => i.SaleQuantity > 0).ToList();
        }
    }
}
