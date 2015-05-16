namespace Distributr.WPF.Lib.Services.DocumentReports.Receipt
{
    public class ReceiptReportLineItem
    {
        public int RowNumber { get; set; }
        public string PaymentMode { get; set; }
        public string PaymentReference { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
    }
}
