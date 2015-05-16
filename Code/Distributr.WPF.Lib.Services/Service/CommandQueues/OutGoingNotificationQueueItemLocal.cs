using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Distributr.Core.Notifications;

namespace Distributr.WPF.Lib.Services.Service.CommandQueues
{
    public class OutGoingNotificationQueueItemLocal
    {
        public NotificationType Type { get; set; }
        [Column(TypeName = "nvarchar(MAX)")]
        public string JsonDTO { get; set; }
        public bool IsSent { get; set; }
        public DateTime DateSent { get; set; }
        public int Id { get; set; }
        public Guid NotificationId { get; set; }

        public DateTime DateInserted
        { get; set; }
    }
}