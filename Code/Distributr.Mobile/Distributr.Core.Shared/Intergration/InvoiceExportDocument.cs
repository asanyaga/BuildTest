using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributr.Core.Intergration
{
    public class InvoiceExportDocument
    {
        public InvoiceExportDocument()
        {
            LineItems = new List<InvoiceExportDocumentItem>();
        }
        //doc
        public Guid Id { get; set; }
        public string OutletCode { get; set; }
        public string InvoiceDocumentRef{ get; set; }
        public string OrderExternalRef { get; set; }
        public string DocumentDateIssued { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime OrderDueDate { get; set; }
        public string SalesmanCode { get; set; }

        public string SalesmanName { get; set; }
        public string OutletName { get; set; }
        public decimal TotalNet { get; set; }
        public decimal TotalVat { get; set; }
        public decimal TotalGross { get; set; }
        public decimal TotalDiscount { get; set; }

        public List<InvoiceExportDocumentItem> LineItems { get; set; }


    }

    public class InvoiceExportDocumentItem
    {
        //lineitem
        public string ProductCode { get; set; }
        public decimal Quantity { get; set; }
        public string VatClass { get; set; }
        public decimal Price { get; set; }
        public decimal VatPerUnit { get; set; }

        public decimal LineItemTotalNet { get; set; }
        public decimal LineItemTotalVat { get; set; }
        public decimal LineItemTotalGross { get; set; }




    
    }
}
