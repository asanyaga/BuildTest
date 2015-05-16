using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributr.WPF.Lib.Services.DocumentReports.GRN
{
    public class GRNReportContainer
    {
        public GRNReportContainer()
        {
            LineItems = new List<GRNReportLineItem>();
        }

        public CompanyHeaderReport CompanyHeader { get; set; }
        public GRNHeader DocumentHeader { get; set; }
        public List<GRNReportLineItem> LineItems { get; set; }
    }
}
