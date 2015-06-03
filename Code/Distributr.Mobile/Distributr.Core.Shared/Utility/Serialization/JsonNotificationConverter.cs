using System;
using Distributr.Core.Notifications;
using Newtonsoft.Json.Linq;

namespace Distributr.Core.Utility.Serialization
{
    public class JsonNotificationConverter : JsonCreationConverter<NotificationBase>
    {
        protected override NotificationBase Create(Type objectType, JObject jObject)
        {
            NotificationBase returnType = null;
            string TypeRef = jObject.Value<string>("TypeRef");

            NotificationType notificationType = (NotificationType)Enum.Parse(typeof(NotificationType), TypeRef);
            switch (notificationType)
            {

                case NotificationType.OrderSale:
                    returnType = new NotificationOrderSale();
                    break;
                case NotificationType.Invoice:
                    returnType = new NotificationInvoice();
                    break;
                case NotificationType.Receipt:
                    returnType = new NotificationReceipt();
                    break;
                case NotificationType.CommodityPurchase:
                    returnType = new NotificationPurchase();
                    break;
                case NotificationType.OrderDispatch:
                    returnType = new NotificationDispatch();
                    break;
                case NotificationType.OrderDelivery:
                    returnType = new NotificationDelivery();
                    break;


            }
            return returnType;
        }
    }
}