using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributr.WPF.Lib.Services.DocumentReports.Invoice
{
    public class InvoiceReportHeader
    {
        public string InvoiceRecipientCompanyName { get; set; }
        public decimal InvoiceBalance { get; set; }
        public decimal InvoiceSubBalance { get; set; }
        public decimal TotalAmountPaid { get; set; }
        public decimal TotalDeductions { get; set; }

        public decimal TotalNet { get; set; }

        public decimal TotalVat { get; set; }

        public decimal TotalGross { get; set; }

        public decimal SaleDiscount { get; set; }

        public decimal TotalProductDiscount { get; set; }

        public string InvoiceRef { get; set; }

        public DateTime InvoiceDate { get; set; }

        public string CreditTerms { get; set; }

        public object PreparedByUserName { get; set; }

        public string PreparedByJobTitle { get; set; }

        public string DocumentIssuerDetails { get; set; }

        public DateTime DatePrinted { get; set; }

        public string DocumentIssuerUserName { get; set; }

        public string DocumentIssuerCCName { get; set; }
    }
}
