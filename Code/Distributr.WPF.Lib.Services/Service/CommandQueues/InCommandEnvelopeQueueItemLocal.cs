namespace Distributr.WPF.Lib.Services.Service.CommandQueues
{
    public class InComingCommandEnvelopeQueueItemLocal : CommandEnvelopeQueueItemLocal
    {
        public bool Processed { get; set; }
        public int NoOfRetry { get; set; }
        public string Info { get; set; }




    }
}