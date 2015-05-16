using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PaymentGateway.WSApi.Lib.Domain.Notifications
{
    public enum PgMessageType { SMSCN = 1, SMSCP = 2, BuyGoods = 3, PayBill = 4,Mpesa=5, Eazy247N = 6, Eazy247P=7}
    public class PgNotification
    {

        public string ReferenceNumber { set; get; }
        public decimal Amount { get; set; }
        public PgMessageType MessageType { set; get; }
        public string Payee { get; set; }
        public Guid ApplicationId { get; set; }
        public string TransactionId { get; set; }
    }

}
