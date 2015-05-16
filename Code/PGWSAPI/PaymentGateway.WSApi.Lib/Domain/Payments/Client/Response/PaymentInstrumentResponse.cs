using System.Collections.Generic;
using PaymentGateway.WSApi.Lib.Domain.Payments.SDP;

namespace PaymentGateway.WSApi.Lib.Domain.Payments.Client.Response
{
    public class PaymentInstrumentResponse : ClientRequestResponseBase
    {
        public List<SDPPaymentInstrument> PaymentInstrumentList { get; set; }
        public string StatusCode { get; set; }
        public string StatusDetail { get; set; }
    }
}
