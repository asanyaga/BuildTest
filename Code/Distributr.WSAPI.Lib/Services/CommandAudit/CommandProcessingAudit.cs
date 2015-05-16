using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.WSAPI.Lib.Services.CommandAudit
{
    public class CommandProcessingAudit
    {
       public Guid Id { get; set; }
       public int CostCentreCommandSequence { get; set; }
       public Guid CostCentreApplicationId { get; set; }
       public Guid DocumentId { get; set; }
       public Guid ParentDocumentId { get; set; }
       public string JsonCommand { get; set; }
       public string CommandType { get; set; }
       public CommandProcessingStatus Status {get;set;}
       public int RetryCounter { get; set; }
       public DateTime DateInserted { get; set; }
       public string SendDateTime { get; set; }
    }

    public enum CommandProcessingStatus
    {
        OnQueue=1,
        SubscriberProcessBegin=2,
        MarkedForRetry=3,
        Complete=4,
        Failed=5,
       ManualProcessing=6
    }
}
