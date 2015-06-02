using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.ClientApp.CommandResults;

namespace Distributr.Mobile.Core.Sync.Incoming
{
    public class DownloadEnvelopesResponse
    {
        public List<CommandEnvelopeWrapper> Envelopes { get; set; } 
        public string ErrorInfo { get; set; }        

        public bool WasSuccessful
        {
            get { return "Success".Equals(ErrorInfo); }
        }

        public bool HasMoreEnvelopes
        {
            get { return WasSuccessful; }
        }

        public Guid LastEnvelopeId()
        {
            return Envelopes.Last().Envelope.Id;
        }
    }
}
