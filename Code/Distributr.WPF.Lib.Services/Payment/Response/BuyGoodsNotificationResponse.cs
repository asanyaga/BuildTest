using System;

namespace Distributr.WPF.Lib.Services.Payment.Response
{
    public class BuyGoodsNotificationResponse : ClientRequestResponseBase
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
