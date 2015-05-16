using System;

namespace Distributr.WSAPI.Lib.Services.CommandAudit
{
    public class CommandProcessingAuditGroup
    {
        public Guid DocumentId { get; set; }
        public string DocumentType { get; set; }
        public int NoOfCommands { get; set; }
        public DateTime DateInserted { get; set; }
        public DateTime DateDelivered { get; set; }
    }
}
