using System;
using System.Collections.Generic;
using Distributr.Core.Utility;
using Newtonsoft.Json;

namespace Distributr.Core.Notifications
{
    
    public class NotificationPurchase : NotificationBase
    {
        public NotificationPurchase()
        {
            Items = new List<NotificationPurchaseSaleItem>();
        }

        public override string TypeRef
        {
            get { return NotificationType.CommodityPurchase.ToString(); }

        }
       
        public Guid HubId { get; set; }
        public Guid PurchaseClerkId { get; set; }
        public Guid FarmerId { get; set; }
        public string CenterName { get; set; }
        public string CummulativeWeightDetail { get; set; }
        public string ServedBy { get; set; }
        public List<NotificationPurchaseSaleItem> Items { get; set; }
    }

    public class NotificationPurchaseSaleItem : NotificationItemBase
    {
        public string Grade { get; set; }
       
    }

   
}