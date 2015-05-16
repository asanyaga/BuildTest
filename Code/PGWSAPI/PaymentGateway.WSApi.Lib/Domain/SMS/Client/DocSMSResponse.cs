using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client;
using PaymentGateway.WSApi.Lib.Domain.SMS.SDP;

namespace PaymentGateway.WSApi.Lib.Domain.SMS.Client
{
    public class DocSMSResponse : ClientRequestResponseBase
    {
        public SmsStatuses SmsStatus { get; set; }
        public string SdpRequestId { get; set; }
        public string SdpResponseCode { get; set; }
        public string SdpResponseStatus { get; set; }
        public List<DestinationResponse> SdpDestinationResponses { get; set; }
        public string SdpVersion { get; set; }
    }

    public enum SmsStatuses
    {
        Pending = 0,
        Sent = 1,
        Received = 2
    }
}
