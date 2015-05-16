using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PaymentGateway.WSApi.Lib.Domain.Payments.Client.Response
{
    public class PaymentNotification : ClientRequestResponseBase
    {
        /// <summary>
        /// use base class TransactionRefId for this requsts internalTransactionId
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
