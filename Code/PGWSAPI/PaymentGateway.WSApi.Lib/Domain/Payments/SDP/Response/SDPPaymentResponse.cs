using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PaymentGateway.WSApi.Lib.Domain.Payments.SDP.Response
{
    public class SDPPaymentResponse : ServerResponseBase
    {
        public string externalTrxId { get; set; }
        public string internalTrxId { get; set; }
        public long referenceId { get; set; }
        public string businessNumber { get; set; }
        public string amountDue { get; set; }
        public DateTime timeStamp { get; set; }
        public string statusCode { get; set; }
        public string statusDetail { get; set; }
        public string longDescription { get; set; }
        public string shortDescription { get; set; }
    }
}
