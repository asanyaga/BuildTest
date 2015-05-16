using System.Collections.Generic;

namespace Distributr.WPF.Lib.Services.Payment.Response
{
    public class PaymentInstrumentResponse : ClientRequestResponseBase
    {
        public List<PaymentInstrument> PaymentInstrumentList { get; set; }
        public string StatusCode { get; set; }
        public string StatusDetail { get; set; }

       
    }
}
