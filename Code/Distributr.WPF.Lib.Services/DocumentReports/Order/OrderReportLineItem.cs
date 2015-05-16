namespace Distributr.WPF.Lib.Services.DocumentReports.Order
{
    public class OrderReportLineItem
    {
        public int RowNumber { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public decimal Qty { get; set; }
        public decimal ApprovedQuantity { get; set; }
        public decimal BackOrderQuantity { get; set; }
        public decimal LostSaleQuantity { get; set; }
        public decimal DispachedQuantity { get; set; }
        public string LineItemTypeStr { get; set; }
        public string DiscountType { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal UnitDiscount { get; set; }
        public decimal TotalNet { get; set; }
        public decimal UnitVat { get; set; }
        public decimal TotalVat { get; set; }
        public decimal GrossAmount { get; set; }
    }
}
