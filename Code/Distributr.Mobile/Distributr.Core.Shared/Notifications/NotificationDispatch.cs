using System;
using System.Collections.Generic;
using Distributr.Core.Utility;
using Newtonsoft.Json;

namespace Distributr.Core.Notifications
{
    
    public class NotificationDispatch : NotificationBase
    {
        public NotificationDispatch()
        {
            Items = new List<NotificationDispatchItem>();
        }

        public override string TypeRef
        {
            get { return NotificationType.OrderDispatch.ToString(); }

        }
       
        public Guid DistributorId { get; set; }
        public Guid SalemanId { get; set; }
        public Guid OutletId { get; set; }
        
        public List<NotificationDispatchItem> Items { get; set; }

        public string OrderRef{
            get; set; }
    }

    public class NotificationDispatchItem : NotificationItemBase
    {
       
    }
    public class NotificationDelivery : NotificationBase
    {
        public NotificationDelivery()
        {
            Items = new List<NotificationDeliveryItem>();
        }

        public override string TypeRef
        {
            get { return NotificationType.OrderDelivery.ToString(); }

        }
        public string OrderRef
        {
            get;
            set;
        }
        public Guid DistributorId { get; set; }
        public Guid SalemanId { get; set; }
        public Guid OutletId { get; set; }

        public List<NotificationDeliveryItem> Items { get; set; }
    }

    public class NotificationDeliveryItem : NotificationItemBase
    {
       
    }
    
   
}