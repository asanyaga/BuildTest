using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PaymentGateway.WSApi.Lib.Domain.Payments.SDP.Request
{
    public class SDPPaymentInstrumentRequest : ServerRequestBase
    {
        public string applicationId { get; set; }
        public string password { get; set; }
        public string subscriberId { get; set; }
        public PaymentInstrumentType type { get; set; }
    }
}
