using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributr.WPF.Lib.Services.DocumentReports
{
    public class DocumentReportContainer
    {
        public DocumentReportContainer()
        {
            LineItems = new List<OrderReportLineItem>();
        }
        public CompanyHeaderReport CompanyHeader { get; set; }
        public DocumentHeader DocumentHeader { get; set; }
        public List<OrderReportLineItem> LineItems { get; set; }
    }
}
