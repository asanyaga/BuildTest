using System;
using System.Collections.Generic;
using Distributr.Core.Utility;
using Newtonsoft.Json;

namespace Distributr.Core.Notifications
{
    
    public class NotificationOrderSale : NotificationBase
    {
        public NotificationOrderSale()
        {
            Items= new List<NotificationOrderSaleItem>();
        }

        public override string TypeRef
        {
            get { return NotificationType.OrderSale.ToString(); }

        }
       
        public Guid DistributorId { get; set; }
        public Guid SalemanId { get; set; }
        public Guid OutletId { get; set; }
        public decimal SalevalueDiscount { get; set; }
        public decimal TotalNet { get; set; }
        public decimal TotalGross { get; set; }
        public decimal TotalVat { get; set; }
        public List<NotificationOrderSaleItem> Items { get; set; }
    }

    public class NotificationOrderSaleItem : NotificationItemBase
    {
        public decimal UnitPrice { get; set; }
        public decimal TotalVat { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalGross { get; set; }
        public decimal TotalNet { get; set; }
    }

   
}