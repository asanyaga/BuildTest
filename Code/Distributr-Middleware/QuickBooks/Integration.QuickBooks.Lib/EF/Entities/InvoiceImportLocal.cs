using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integration.QuickBooks.Lib.EF.Entities
{
    public class InvoiceImportLocal
    {
        public Guid Id { get; set; }
        public string OutletCode { get; set; }
        public string InvoiceDocumentRef { get; set; }
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

        public DateTime DateOfImport { get; set; }
        public DateTime? DateOfExport { get; set; }
        public int ExportStatus { get; set; }
    }
}
