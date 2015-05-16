using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client;
using PaymentGateway.WSApi.Lib.Domain.SMS.Client;

namespace PaymentGateway.WSApi.Lib.MessageResults
{
    public abstract class ResponseBase
    {
        public string ErrorInfo { get; set; }
    }

    public class PGResponseBasic : ResponseBase
    {
        public string Result { get; set; }
    }

    public class PGBResponseBool : ResponseBase
    {
        public bool Success { get; set; }
    }


    public class PGBResponse : PGBResponseBool
    {
        public ClientRequestResponseBase Response { get; set; }
    }
}
