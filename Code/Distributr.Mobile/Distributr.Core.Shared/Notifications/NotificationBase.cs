using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.Utility;
using Distributr.Core.Utility.Serialization;
using Newtonsoft.Json;

namespace Distributr.Core.Notifications
{
    [JsonConverter(typeof(JsonNotificationConverter))]
    public abstract class NotificationBase
    {
        public string DocumentRef { get; set; }
        public Guid Id { get; set; }
        public abstract string TypeRef { get; }
    }
    public abstract class NotificationItemBase
    {
        public string ItemName { get; set; }
        public decimal Quantity { get; set; }
       
    }
    public class NotificationSMS
    {

        public Guid HubId { get; set; }
        public List<string> Recipitents { get; set; }
        [StringLength(150, ErrorMessage = "Invalid length for sms body.", MinimumLength = 5)]
        public string SmsBody { get; set; }


    }
}