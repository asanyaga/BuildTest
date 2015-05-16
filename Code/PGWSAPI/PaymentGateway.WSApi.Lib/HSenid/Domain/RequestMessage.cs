using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PaymentGateway.WSApi.Lib.HSenid.Domain
{
    public enum StatusRequest : int { DeliveryRequestNotRequired = 0, DeliveryRequestRequired =1}
    public enum RequestEncoding : int { Text = 0, FlashSms = 240,BinarySms=245 }
    public class RequestMessage
    {
        public RequestMessage()
        {
            address = new List<String>();
        }

        public string message { set; get; }
        public string sourceAddress { set; get; }
        public string applicationID { set; get; }
        public string version { set; get; }
        public RequestEncoding encoding { set; get; }
        public decimal chargingAmount { set; get; }
        public List<String> address{ set; get; }
        public string binaryHeader { set; get; }
        public string password { set; get; }
        public StatusRequest statusRequest { set; get; }
       
    }
}
