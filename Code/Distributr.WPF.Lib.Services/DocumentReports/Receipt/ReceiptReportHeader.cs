using System;

namespace Distributr.WPF.Lib.Services.DocumentReports.Receipt
{
    public class ReceiptReportHeader
    {
        public string ReceiptNo { get; set; }
        public DateTime ReceiptDate { get; set; }
        public string InvoiceRef { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string ServedByUserName { get; set; }
        public DateTime DatePrinted { get; set; }
        public string ReceiptRecipientCompanyName { get; set; }
        public string CompanyName { get; set; }
        public string ChequePayableTo { get; set; }
        public string DocumentIssuerDetails { get; set; }

        public decimal TotalGross { get; set; }

        public decimal TotalNet { get; set; }

        public decimal TotalVat { get; set; }
    }
}
