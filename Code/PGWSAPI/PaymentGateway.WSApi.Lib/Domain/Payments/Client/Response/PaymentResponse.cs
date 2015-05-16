using System;

namespace PaymentGateway.WSApi.Lib.Domain.Payments.Client.Response
{
    public class PaymentResponse : ClientRequestResponseBase
    {
        public string SDPTransactionRefId { get; set; }
        public string SDPReferenceId { get; set; }//SDP referenceID Number Generated in PGW must be 8 digits 
        public string BusinessNumber { get; set; }
        public double AmountDue { get; set; } 
        public DateTime TimeStamp { get; set; }
        public string StatusCode { get; set; }
        public string StatusDetail { get; set; }
        public string LongDescription { get; set; }
        public string ShortDescription { get; set; }
        public string SubscriberId { get; set; }
    }
}
