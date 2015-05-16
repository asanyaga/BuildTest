using System;

namespace Distributr.WPF.Lib.Services.Service.CommandQueues
{
    public class OutgoingCommandQueueItemLocal : CommandQueueItemLocal
    {
        
        public DateTime DateSent { get; set; }
        public bool IsCommandSent { get; set; }
    }
}
