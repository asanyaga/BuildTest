using System;

namespace Distributr.WPF.Lib.Services.Service.CommandQueues
{
    public class OutGoingCommandEnvelopeQueueItemLocal : CommandEnvelopeQueueItemLocal
    {


        public bool IsEnvelopeSent { get; set; }
        public DateTime DateSent { get; set; }
    }
}