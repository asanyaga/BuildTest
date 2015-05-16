using System;

namespace Distributr.WPF.Lib.Services.Payment.Response
{
    public class AsynchronousPaymentNotificationResponse : ClientRequestResponseBase
    {
        /// <summary>
        /// This class is used to generate response for both SDP and Client apps i.e Mobile or SL app
        /// </summary>
        public string SDPTransactionRefId { get; set; }
        public string SDPReferenceId { get; set; }
        public double PaidAmount { get; set; }
        public double BalanceDue { get; set; }
        public double TotalAmount { get; set; }//accuml partial or overpayments made by subscriber for the same referenceId
        public string Currency { get; set; }
        public string StatusCode { get; set; }
        public string StatusDetail { get; set; }
        public DateTime TimeStamp { get; set; }


       
    }
}
