using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PaymentGateway.WSApi.Lib.Domain.Payments.Client.Request
{
    public class BuyGoodsNotificationRequest : ClientRequestResponseBase
    {
        public string SDPTransactionRefId { get; set; }
        public string SubscriberName { get; set; }
        public string ReceiptNumber { get; set; }
        public string Currency { get; set; }
        public double PaidAmount { get; set; }
        public double MerchantBalance { get; set; }
        public DateTime Date { get; set; }
        public DateTime Time { get; set; }
        public string StatusCode { get; set; }
        public string StatusDetail { get; set; }
    }
}
