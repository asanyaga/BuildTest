using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PaymentGateway.WSApi.Lib.Domain.Payments.SDP.Response
{
    public class SDPPaymentNotificationResponse : ServerResponseBase
    {
        public string statusCode { get; set; }
        public string statusDetail { get; set; }
    }
}
