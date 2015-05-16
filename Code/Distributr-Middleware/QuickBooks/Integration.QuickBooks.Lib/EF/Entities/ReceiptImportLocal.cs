using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integration.QuickBooks.Lib.EF.Entities
{
    public class ReceiptImportLocal
    {
        [Key]
        public Guid Id { get; set; }
        public string OrderExternalRef { get; set; }
        public string ReceiptRef { get; set; }
        public string InvoiceRef { get; set; }
        public string SalesmanCode { get; set; }
        public string OutletCode { get; set; }
        public decimal OrderTotalGross { get; set; }
        public decimal OrderNetGross { get; set; }

        public DateTime DateOfImport { get; set; }
        public DateTime? DateOfExport { get; set; }
        public int ExportStatus { get; set; }
    }
}
