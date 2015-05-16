using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PaymentGateway.WSApi.Lib.Domain.Payments.SDP.Request
{
    public class SDPBuyGoodsNotificationRequest : ServerRequestBase
    {
        public string internalTrxId { get; set; }
        public string subscriberName { get; set; }
        public string receiptNumber { get; set; }
        public string currency { get; set; }
        public string paidAmount { get; set; }
        public string merchantBalance { get; set; }
        public DateTime date { get; set; }
        public DateTime time { get; set; }
        public string statusCode { get; set; }
        public string statusDetail { get; set; }
    }
}
