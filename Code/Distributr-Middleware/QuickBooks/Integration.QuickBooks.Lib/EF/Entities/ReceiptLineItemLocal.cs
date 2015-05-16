using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integration.QuickBooks.Lib.EF.Entities
{
    public class ReceiptLineItemLocal
    {
        [Key]
        public Guid LineItemId { get; set; }
        public Guid ReceiptId { get; set; }
        public decimal ReceiptAmount { get; set; }
        public string ModeOfPayment { get; set; }
        public string ChequeNumber { get; set; }

        public DateTime DateOfImport { get; set; }
        public DateTime? DateOfExport { get; set; }
        public int ExportStatus { get; set; }
    }
}
