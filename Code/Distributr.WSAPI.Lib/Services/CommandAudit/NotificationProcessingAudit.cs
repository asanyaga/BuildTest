using System;

namespace Distributr.WSAPI.Lib.Services.CommandAudit
{
    public class NotificationProcessingAudit
    {
        public Guid Id { get; set; }
        public string JsonNotification { get; set; }
        public string Type { get; set; }
        public NotificationProcessingStatus Status { get; set; }
       
        public DateTime DateInserted { get; set; }
        public string Info { get; set; }
      
    }
    public class NotificationProcessingAuditInfo
    {
        public string Id { get; set; }
        public Guid NotificationId { get; set; }
        public string Type { get; set; }
        public DateTime DateInserted { get; set; }
        public string Info { get; set; }
        public string Contact { get; set; }

    }
    public enum NotificationProcessingStatus
    {
        Error = 0,
        Pending = 1,
        InProgress = 2,
        Sent = 3,
    }
}