using System;

namespace Distributr.WPF.Lib.Services.DocumentReports.Invoice
{
    public class InvoiceReportLineItem
    {
        public string InvoiceCreditNoteRefField { get; set; }
        
        public string InvoiceReceiptRefField { get; set; }
        
        public string InvoiceReceiptDateField { get; set; }

        public int RowNumber { get; set; }

        public string Description { get; set; }

        public decimal Qty { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal TotalNet { get; set; }

        public decimal UnitDiscount { get; set; }

        public string ReceiptRef { get; set; }

        public DateTime ReceiptDate { get; set; }

        public string CreditNoteRef { get; set; }
    }
}
