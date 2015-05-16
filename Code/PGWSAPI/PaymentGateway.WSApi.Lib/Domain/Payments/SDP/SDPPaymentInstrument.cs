using System;

namespace PaymentGateway.WSApi.Lib.Domain.Payments.SDP
{
    public class SDPPaymentInstrument
    {
        public string name { get; set; }
        public string type { get; set; }
        public string accountId { get; set; }
    }
}
