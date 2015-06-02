using System;
using System.Collections.Generic;

namespace Distributr.Core.Intergration
{
    public class ReceiptExportDocument
    {
        public ReceiptExportDocument()
        {
            LineItems= new List<ReceiptExportDocumentItem>();
        }

        public Guid Id { get; set; }
        public string OrderExternalRef { get; set; }
        public string ReceiptRef { get; set; }
        public string InvoiceRef { get; set; }
        public string SalesmanCode { get; set; }
        public string OutletCode { get; set; }
        public DateTime PaymentDate { get; set; }
        public string RouteName { get; set; }
        public string ShipToAddress { get; set; }
        public  List<ReceiptExportDocumentItem> LineItems { get; set; }
        public decimal OrderTotalGross { get; set; }
        public decimal OrderNetGross { get; set; }
      


    }

    public class ReceiptExportDocumentItem
    {
       
        public decimal ReceiptAmount { get; set; }
        public string ModeOfPayment { get; set; }
        public string ChequeNumber { get; set; }
    }

}