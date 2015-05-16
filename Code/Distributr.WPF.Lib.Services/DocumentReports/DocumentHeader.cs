using System;

namespace Distributr.WPF.Lib.Services.DocumentReports
{
    public class DocumentHeader
    {
        public string DocumentReference { get; set; }
        public string SalesmanName { get; set; }
        public string RouteName { get; set; }
        public string IssuedOnBehalfOfName { get; set; }
        public string DocumentIssuerUserName { get; set; }
        public string DocumentIssuerCCName { get; set; }
        public string DocumentRecipientCCName { get; set; }
        public string OrderTypeStr { get; set; }
        public string StatusStr { get; set; }
        public Guid ParentId { get; set; }
        public DateTime DateRequired { get; set; }
        public decimal SaleDiscount { get; set; }
        public string Comment { get; set; }
        public string ShipToAddress { get; set; }

        public decimal TotalNet { get; set; }
        public decimal TotalVat { get; set; }
        public decimal TotalGross { get; set; }
        public decimal TotalSaleDiscount { get; set; }
        public decimal TotalProductDiscount { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal OutstandingAmount { get; set; }

        public DateTime DatePrinted { get; set; }

        public string DocumentIssuerDetails { get; set; }

        //invoice
    }
}
