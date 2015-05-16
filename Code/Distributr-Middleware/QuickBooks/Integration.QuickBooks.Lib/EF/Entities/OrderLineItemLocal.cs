using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integration.QuickBooks.Lib.EF.Entities
{
    public class OrderLineItemLocal
    {
        [Key]
        public Guid LineItemId { get; set; }
        public string OrderExternalRef { get; set; }
        public string ProductCode { get; set; }
        public decimal Quantity { get; set; }
        public string VatClass { get; set; }
        public decimal Price { get; set; }
        public decimal VatPerUnit { get; set; }

        public decimal LineItemTotalNet { get; set; }
        public decimal LineItemTotalVat { get; set; }
        public decimal LineItemTotalGross { get; set; }

        public DateTime DateOfImport { get; set; }
        public DateTime? DateOfExport { get; set; }
        public int ExportStatus { get; set; }
    }
}
