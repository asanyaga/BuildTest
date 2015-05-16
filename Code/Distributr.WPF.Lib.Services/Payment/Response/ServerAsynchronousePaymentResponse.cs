using System;
using System.Collections.Generic;

namespace Distributr.WPF.Lib.Services.Payment.Response
{
    public class AsynchronousPaymentNotificationResponse : ClientRequestResponseBase
    {
        public string SDPTransactionRefId { get; set; }
        public string SDPReferenceId { get; set; }
        public string Currency { get; set; }
        public string StatusCode { get; set; }
        public string StatusDetail { get; set; }
        public string SubscriberId { get; set; }
        public string AccountId { get; set; }
        public List<PaymentNotificationList> PaymentNotificationDetails { get; set; }
    }
    public class PaymentNotificationList
    {
        public Guid Id { get; set; }
        public double PaidAmount { get; set; }
        public double BalanceDue { get; set; }
        public double TotalAmount { get; set; }//accuml partial or overpayments made by subscriber for the same referenceId
        public DateTime TimeStamp { get; set; }
        public string Status { get; set; }
    }
}
