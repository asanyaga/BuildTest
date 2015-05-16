using System;
using Sqo;

namespace Distributr.WPF.Lib.Impl.Model.Utility
{
    [Obsolete("Command Envelope Refactoring")]
    public class ReceivedCommandLocal 
    {
        public int Id { get; set; }
        public long LastDeliveredCommandRouteItemId { get; set; }
       
    }
    public class ReceivedCommandEnvelopeId
    {

        public Guid EnvelopeId { get; set; }
        public int Id { get; set; }
        public long EnvelopeArrivedAtServerTick  { get; set; }
      

    }
}
