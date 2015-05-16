using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PaymentGateway.WSApi.Lib.Domain.Payments.SDP.Request
{
    public class SDPPaymentNotificationRequest : ServerRequestBase
    {
        public string externalTrxId { get; set; }
        public string internalTrxId { get; set; }
        public string referenceId { get; set; }
        public string paidAmount { get; set; }
        public string currency { get; set; }
        public string totalAmount { get; set; }//accuml partial or overpayments made by subscriber for the same referenceId
        public string balanceDue { get; set; }//bal that must be paid by the subscriber
        public string statusCode { get; set; }
        public string statusDetail { get; set; }
        public DateTime timestamp { get; set; }
    }
}
