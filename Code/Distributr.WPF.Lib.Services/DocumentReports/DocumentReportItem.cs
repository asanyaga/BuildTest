using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributr.WPF.Lib.Services.DocumentReports
{
    public class DocumentReportItem
    {
        public DocumentReportItem()
        {
            LineItems = new List<DocumentLineItem>();
        }
        public DocumentHeader DocumentHeader { get; set; }
        public List<DocumentLineItem> LineItems { get; set; }
    }
}
