using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PaymentGateway.WSApi.Lib.Domain.Payments.SDP.Request
{
    public class SDPPaymentRequest : ServerRequestBase
    {
        public string applicationId { get; set; }
        public string password { get; set; }
        public string externalTrxId { get; set; }
        public string subscriberId { get; set; }
        public string paymentInstrumentName { get; set; }
        public string accountId { get; set; }
        public string amount { get; set; }
        public string currency { get; set; }
        public string invoiceNo { get; set; }
        public string orderNo { get; set; }
        public string/*AllowPartialPayments*/ allowPartialPayments { get; set; }
        public string/*AllOverPayment*/ allowOverPayments { get; set; }
        public Dictionary<string, string> extra { get; set; }
        public string smsDescription { get; set; }//for Operator limit 40chars
    }

    public enum AllOverPayment
    {
        Allow = 1,
        Disallow = 2
    }

    public enum AllowPartialPayments
    {
        Allow = 1,
        Disallow = 2,
    }
}
