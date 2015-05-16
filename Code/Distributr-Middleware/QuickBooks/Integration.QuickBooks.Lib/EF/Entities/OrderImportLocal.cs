using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integration.QuickBooks.Lib.EF.Entities
{
    public class OrderImportLocal
    {
        [Key]
        public string OrderExternalRef { get; set; }
        public string OutletCode { get; set; }
        public string OrderRef { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime OrderDueDate { get; set; }
        public string SalesmanCode { get; set; }
        public int OrderType { get; set; }

        public string OutletName { get; set; }
        public string Note { get; set; }
        public string RouteName { get; set; }
        public string ShipToAddress { get; set; }
        public decimal TotalNet { get; set; }
        public decimal TotalVat { get; set; }
        public decimal TotalGross { get; set; }
        public decimal TotalDiscount { get; set; }
        public DateTime DateOfImport { get; set; }
        public DateTime? DateOfExport { get; set; }

        public string QbOrderTransactionId { get; set; }

        public int ExportStatus { get; set; }
    }
}
