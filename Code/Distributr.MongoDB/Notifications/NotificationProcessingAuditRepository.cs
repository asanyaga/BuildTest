using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.MongoDB.Repository;
using Distributr.WSAPI.Lib.Services.CommandAudit;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;

namespace Distributr.MongoDB.Notifications
{
   public class NotificationProcessingAuditRepository :MongoBase, INotificationProcessingAuditRepository
    {
       private string _notificationProcessingAuditCollectionName = "NotificationProcessingAudit";
        private MongoCollection<NotificationProcessingAudit> _notificationProcessingAuditCollection;

        private string _notificationProcessingAuditInfoCollectionName = "NotificationProcessingAuditInfo";
        private MongoCollection<NotificationProcessingAuditInfo> _notificationProcessingAuditInfoCollection;
        
        public NotificationProcessingAuditRepository(string connectionStringNot)
            : base(connectionStringNot)
       {
           _notificationProcessingAuditCollection = CurrentMongoDB.GetCollection<NotificationProcessingAudit>(_notificationProcessingAuditCollectionName);
           _notificationProcessingAuditCollection.EnsureIndex(IndexKeys<NotificationProcessingAudit>.Ascending(n => n.Id), IndexOptions.SetUnique(true));
           _notificationProcessingAuditInfoCollection=CurrentMongoDB.GetCollection<NotificationProcessingAuditInfo>(_notificationProcessingAuditInfoCollectionName);
           _notificationProcessingAuditInfoCollection.EnsureIndex(IndexKeys<NotificationProcessingAuditInfo>.Ascending(n => n.Id), IndexOptions.SetUnique(true));
       }

       public void Add(NotificationProcessingAudit audit)
       {
           _notificationProcessingAuditCollection.Save(audit);
       }
       public void Add(NotificationProcessingAuditInfo audit)
       {
           _notificationProcessingAuditInfoCollection.Save(audit);
       }
       public List<NotificationProcessingAudit> GetUnSent(int take=100)
       {
           return
               _notificationProcessingAuditCollection.AsQueryable().Where(
                   s => s.Status == NotificationProcessingStatus.Pending).OrderByDescending(s=>s.DateInserted).Take(take).ToList();

       }
    }
}
