using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.WSAPI.Lib.Services.CommandAudit;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using log4net;

namespace Distributr.Azure.Lib.CommandProcessing.Notification
{
    public class AzureNotificationProcessingAuditRepository : INotificationProcessingAuditRepository
    {
        private ILog _logger = LogManager.GetLogger("AzureNotificationProcessingAuditRepository");
        public AzureNotificationProcessingAuditRepository(string storageConnectionString)
        {
            
        }
        public void Add(NotificationProcessingAudit audit)
        {
            _logger.Info("Failed to add audit --> " + JsonConvert.SerializeObject(audit) );
            //throw new NotImplementedException();
        }

        public List<NotificationProcessingAudit> GetUnSent(int take = 100)
        {
            _logger.Info("Stubbed NotificationProcessing Audit List");
            return new List<NotificationProcessingAudit>();
        }


        public void Add(NotificationProcessingAuditInfo audit)
        {
           // throw new NotImplementedException();
        }
    }
}
