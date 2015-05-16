using System;

namespace PaymentGateway.WSApi.Lib.Domain.Payments.Client.Request
{
    public class PaymentNotificationRequest : ClientRequestResponseBase
    {
        /// <summary>
        /// This class is used for request from SDP which comes as a Notification message from SDP that a payment has been made
        /// </summary>
        public string SDPTransactionRefId { get; set; }//SDP's internalTrxId
        public string SDPReferenceId { get; set; } //should match with the Asynch Payment Response SDPReferenceId
        public double SDPPaidAmount { get; set; } //amount sent from SDP as the amount paid by customer
        public double SDPTotalAmount { get; set; }//accuml partial or overpayments made by subscriber for the same referenceId
        public double SDPBalanceDue { get; set; }//bal that must be paid by the subscriber
        public string SDPCurrency { get; set; } //currence as receive from SPD from PG
        public string SDPStatusCode { get; set; }
        public string SDPStatusDetail { get; set; }
        public DateTime SDPTimeStamp { get; set; }//SDP's date and time of payment according to Equity Bank system
    }
}
