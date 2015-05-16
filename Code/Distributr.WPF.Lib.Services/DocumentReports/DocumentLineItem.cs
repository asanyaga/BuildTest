namespace Distributr.WPF.Lib.Services.DocumentReports
{
    public class DocumentLineItem : ReportBase
    {
        public int RowNumber { get; set; }
        public string ProductName { get; set; }
        public decimal Qty { get; set; }
        public decimal ApprovedQuantity { get; set; }
        public decimal BackOrderQuantity { get; set; }
        public decimal LostSaleQuantity { get; set; }
        public decimal DispachedQuantity { get; set; }
        public string LineItemTypeStr { get; set; }
        public string DiscountType { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal UnitDiscount { get; set; }
        public decimal TotalNet { get; set; } //{  get { return Qty * Value; }  }
        public decimal UnitVat { get; set; }
        public decimal TotalVat { get; set; } //{ get { return Qty * VatValue; } }
        public decimal GrossAmount { get; set; } //{ get { return (TotalNet + TotalVat); } }
    }
}
