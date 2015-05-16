using System.Collections.Generic;

namespace Distributr.WSAPI.Lib.Services.CommandAudit
{
    public interface INotificationProcessingAuditRepository
    {
        void Add(NotificationProcessingAudit audit);
        void Add(NotificationProcessingAuditInfo audit);
        
        List<NotificationProcessingAudit> GetUnSent(int take = 100);
    }
}