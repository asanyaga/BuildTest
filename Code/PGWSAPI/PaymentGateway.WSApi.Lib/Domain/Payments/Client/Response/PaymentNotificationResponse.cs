using System;
using System.Collections.Generic;

namespace PaymentGateway.WSApi.Lib.Domain.Payments.Client.Response
{
    public class PaymentNotificationResponse : ClientRequestResponseBase
    {
        /// <summary>
        /// This class is used to generate response for both SDP and Client apps i.e Mobile or SL app
        /// </summary>
        public  PaymentNotificationResponse()
        {
           PaymentNotificationDetails= new List<PaymentNotificationListItem>();
        }
       
        public string SDPTransactionRefId { get; set; }
        public string SDPReferenceId { get; set; }
        public string Currency { get; set; }
        public string StatusCode { get; set; }
        public string StatusDetail { get; set; }
        public string SubscriberId { get; set; }
        public string AccountId { get; set; }

        public double PaidAmount { get; set; }
        public double BalanceDue { get; set; }
        public double TotalAmount { get; set; }//accuml partial or overpayments made by subscriber for the same referenceId
        public DateTime TimeStamp { get; set; }
        public List<PaymentNotificationListItem> Items { get; set; }
        public  List<PaymentNotificationListItem> PaymentNotificationDetails { get; set; }
    }
    public class PaymentNotificationListItem
    {
        public Guid Id { get; set; }
        public double PaidAmount { get; set; }
        public double BalanceDue { get; set; }
        public double TotalAmount { get; set; }//accuml partial or overpayments made by subscriber for the same referenceId
        public DateTime TimeStamp { get; set; }
        public string Status { get; set; }
        public Guid ResponseId { get; set; }
        public bool IsUsed { get; set; }
        public PaymentNotificationResponse PaymentNotificationResponse { get; set; }
    }
}
