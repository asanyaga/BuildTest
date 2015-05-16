using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributr.Core.Intergration
{
    public class ReturnInventoryExportDocument
    {
        public ReturnInventoryExportDocument()
        {
            LineItems = new List<ReturnInventoryExportDocumentItem>();
        }
        public Guid Id { get; set; }
        public string DocumentRef { get; set; }
        public string SalesmanName { get; set; }
        public string SalesmanCode { get; set; }
        public DateTime DocumentDateIssued { get; set; }
        public List<ReturnInventoryExportDocumentItem> LineItems { get; set; }


    }

    public class  ReturnInventoryExportDocumentItem
    {
        public Guid Id { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public decimal Quantity { get; set; }
        public int LossTypeId { get; set; }
    }
    
}
