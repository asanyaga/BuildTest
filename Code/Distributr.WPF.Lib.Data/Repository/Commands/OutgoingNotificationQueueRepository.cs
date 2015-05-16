using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Notifications;
using Distributr.WPF.Lib.Data.EF;
using Distributr.WPF.Lib.Services.Service.CommandQueues;
using Newtonsoft.Json;

namespace Distributr.WPF.Lib.Data.Repository.Commands
{
   public class OutgoingNotificationQueueRepository : IOutgoingNotificationQueueRepository
    {
       private DistributrLocalContext _ctx;

       public OutgoingNotificationQueueRepository(DistributrLocalContext ctx)
       {
           _ctx = ctx;
       }

       public List<OutGoingNotificationQueueItemLocal> GetUnSent()
       {
           return _ctx.OutGoingNotificationQueueItemLocals.OrderBy(d => d.DateInserted).Where(p => !p.IsSent).ToList();

       }

       public void MarkAsSent(int OID)
       {
           OutGoingNotificationQueueItemLocal existing = _ctx.OutGoingNotificationQueueItemLocals.OrderBy(d => d.DateInserted).FirstOrDefault(p => !p.IsSent);
           existing.IsSent = true;
           _ctx.SaveChanges();
       }

       public OutGoingNotificationQueueItemLocal GetFirstUnSent()
       {
           OutGoingNotificationQueueItemLocal existing = _ctx.OutGoingNotificationQueueItemLocals.OrderBy(d=>d.DateInserted).FirstOrDefault(p => !p.IsSent);
           return existing;
       }

       public void Add(NotificationBase notification)
       {
           
           if(notification ==null) return;
           var item = new OutGoingNotificationQueueItemLocal();
           item.NotificationId = Guid.NewGuid();
           item.DateInserted = DateTime.Now;
           item.DateSent = DateTime.Now;
           item.IsSent = false;
           if(notification is NotificationOrderSale)
           {
               var n = notification as NotificationOrderSale;
               item.Type = NotificationType.OrderSale;
               item.JsonDTO = JsonConvert.SerializeObject(n);
               AddItem(item);
           }
           if (notification is NotificationInvoice)
           {
               var n = notification as NotificationInvoice;
               item.Type = NotificationType.Invoice;
               item.JsonDTO = JsonConvert.SerializeObject(n);
               AddItem(item);
           }
           if (notification is NotificationReceipt)
           {
               var n = notification as NotificationReceipt;
               item.Type = NotificationType.Receipt;
               item.JsonDTO = JsonConvert.SerializeObject(n);
               AddItem(item);
           }
           if (notification is NotificationPurchase)
           {
               var n = notification as NotificationPurchase;
               item.Type = NotificationType.CommodityPurchase;
               item.JsonDTO = JsonConvert.SerializeObject(n);
               AddItem(item);
           }
           if (notification is NotificationDispatch)
           {
               var n = notification as NotificationDispatch;
               item.Type = NotificationType.OrderDispatch;
               item.JsonDTO = JsonConvert.SerializeObject(n);
               AddItem(item);
           }
          
       }


       private void AddItem(OutGoingNotificationQueueItemLocal itemToAdd)
       {
           OutGoingNotificationQueueItemLocal existing = _ctx.OutGoingNotificationQueueItemLocals.FirstOrDefault(p => p.Id == itemToAdd.Id);
           if (existing == null)
           {
               existing = new OutGoingNotificationQueueItemLocal();
               _ctx.OutGoingNotificationQueueItemLocals.Add(existing);
           }
           existing.NotificationId = itemToAdd.NotificationId;
           existing.Type = itemToAdd.Type;
           existing.DateInserted = itemToAdd.DateInserted;
           existing.DateSent = itemToAdd.DateSent;
           existing.IsSent = false;
           existing.JsonDTO = itemToAdd.JsonDTO;

           _ctx.SaveChanges();
       }
    }
}
