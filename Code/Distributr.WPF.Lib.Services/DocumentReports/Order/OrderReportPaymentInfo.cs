using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributr.WPF.Lib.Services.DocumentReports.Order
{
    public class OrderReportPaymentInfo
    {
        public decimal Amount { get; set; }
        public string PaymentModeUsed { get; set; }
        public string PaymentRefId { get; set; }
        public string BankInfo { get; set; }
        public string ChequeDueDate { get; set; }
    }
}
