using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Distributr.WPF.Lib.Services.Service.CommandQueues
{
    public class IncomingCommandQueueItemLocal : CommandQueueItemLocal
    {

        public DateTime DateReceived { get; set; }
        public bool Processed { get; set; }
        public int NoOfRetry { get; set; }
        public string Info { get; set; }


    }
}
