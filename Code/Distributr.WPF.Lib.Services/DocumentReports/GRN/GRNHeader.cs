using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributr.WPF.Lib.Services.DocumentReports.GRN
{
    public class GRNHeader
    {
        public string DocumentReference { get; set; }
        public string OrderReference { get; set; }
        public string LoadNumber { get; set; }
        public string DocumentIssuerUserName { get; set; }
        public string DocumentIssuerCCName { get; set; }
        public string DocumentRecipientCCName { get; set; }
        public DateTime DateReceived { get; set; }
        public decimal TotalCost { get; set; }
        public DateTime DatePrinted { get; set; }

        public string DocumentIssuerDetails { get; set; }
    }
}
