using Distributr.Core.Workflow.Impl.AuditLogs;

namespace Distributr.Mobile.Core
{
    public class NoOpAuditLogWFManager : IAuditLogWFManager
    {
        public void AuditLogEntry(string module, string action)
        {
            //Do nothing on mobile
        }
    }
}