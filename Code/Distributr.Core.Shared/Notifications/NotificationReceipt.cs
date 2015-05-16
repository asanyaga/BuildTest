using System;
using System.Collections.Generic;

namespace Distributr.Core.Notifications
{
    public class NotificationReceipt : NotificationBase
    {
        public NotificationReceipt()
        {
            Items = new List<NotificationReceiptItem>();
        }

        public override string TypeRef
        {
            get { return NotificationType.Receipt.ToString(); }

        }
        public string OrderRef { get; set; }
        public string InvoiceRef { get; set; }
        public Guid DistributorId { get; set; }
        public Guid SalemanId { get; set; }
        public Guid OutletId { get; set; }
        public decimal TotalAmount { get; set; }
        public List<NotificationReceiptItem> Items { get; set; }
    }
    public class NotificationReceiptItem : NotificationItemBase
    {
        public string Reference { get; set; }
        public string Description { get; set; }

    }
}