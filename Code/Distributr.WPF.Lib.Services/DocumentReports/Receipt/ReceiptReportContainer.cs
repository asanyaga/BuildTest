using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributr.WPF.Lib.Services.DocumentReports.Receipt
{
    public class ReceiptReportContainer
    {
        public ReceiptReportContainer()
        {
            LineItems = new List<ReceiptReportLineItem>();
        }
        public CompanyHeaderReport CompanyHeader { get; set; }
        public ReceiptReportHeader DocumentHeader { get; set; }
        public List<ReceiptReportLineItem> LineItems { get; set; }
    }
}
