using System.Collections.Generic;
using Distributr.Core.Workflow;

namespace Distributr.WPF.Lib.Services.DocumentReports.Order
{
    public class OrderReportContainer
    {
        public OrderReportContainer()
        {
            LineItems = new List<OrderReportLineItem>();
            PaymentInfoList = new List<OrderReportPaymentInfo>();
        }
        public CompanyHeaderReport CompanyHeader { get; set; }
        public OrderHeader DocumentHeader { get; set; }
        public List<OrderReportLineItem> LineItems { get; set; }
        public List<OrderReportPaymentInfo> PaymentInfoList { get; set; }
    }
}
