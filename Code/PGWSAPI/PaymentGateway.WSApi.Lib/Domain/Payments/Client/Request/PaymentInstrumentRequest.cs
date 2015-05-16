using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PaymentGateway.WSApi.Lib.Domain.Payments.Client.Request
{
    public class PaymentInstrumentRequest : ClientRequestResponseBase
    {
        public string SubscriberId { get; set; }
        public PaymentInstrumentType paymentInstrumentType { get; set; }
    }
}
