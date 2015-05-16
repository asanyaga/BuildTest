using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PaymentGateway.WSApi.Lib.Report.Domain
{
    public class ServiceProviderReport:ReportBase
    {

        public string SdpAppId { set; get; }
        public string SdpPassword { set; get; }
        public string SubscriberId { set; get; }
        public string Currency { set; get; }
        public bool AllowPartialPayment { set; get; }
        public bool AllowOverPayment { set; get; }
        public string Name { set; get; }
        public string Code { set; get; }
        public string Sid { set; get; }
    }
}
