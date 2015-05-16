using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PaymentGateway.WSApi.Lib.Domain.Payments.SDP.Response
{
    public class SDPPaymentQueryResponse
    {
        public string internalTrxId { get; set; }
        public string statusCode { get; set; }
        public string statusText { get; set; }
    }
}
