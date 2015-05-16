using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PaymentGateway.WSApi.Lib.Domain.Payments.SDP.Request
{
    public class SDPPaymentQueryRequest : ServerRequestBase
    {
        public string applicationId { get; set; }
        public string password { get; set; }
        public string internalTrxId { get; set; }
    }
}
