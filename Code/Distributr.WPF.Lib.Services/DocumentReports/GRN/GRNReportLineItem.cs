using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributr.WPF.Lib.Services.DocumentReports.GRN
{
    public class GRNReportLineItem
    {
        public int RowNumber { get; set; }
        public string ProductName { get; set; }
        public decimal ReceivedQty { get; set; }
        public decimal ExpectedQty { get; set; }
        public decimal UnitCost { get; set; }
        public decimal TotalCost { get; set; }
        public string LineItemInfo { get; set; }
    }
}
