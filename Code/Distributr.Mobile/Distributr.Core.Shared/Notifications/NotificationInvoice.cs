using System;
using System.Collections.Generic;

namespace Distributr.Core.Notifications
{
    public class NotificationInvoice : NotificationBase
    {
        public NotificationInvoice()
        {
            Items = new List<NotificationInvoiceItem>();
        }

        public override string TypeRef
        {
            get { return NotificationType.Invoice.ToString(); }

        }
        public string OrderRef { get; set; }
        public Guid DistributorId { get; set; }
        public Guid SalemanId { get; set; }
        public Guid OutletId { get; set; }
        public decimal SalevalueDiscount { get; set; }
        public decimal TotalNet { get; set; }
        public decimal TotalGross { get; set; }
        public decimal TotalVat { get; set; }
        public List<NotificationInvoiceItem> Items { get; set; }
    }
    public class NotificationInvoiceItem : NotificationItemBase
    {
        public decimal UnitPrice { get; set; }
        public decimal TotalVat { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalGross { get; set; }
        public decimal TotalNet { get; set; }
    }

  
}