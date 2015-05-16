using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integration.QuickBooks.Lib.EF.Entities
{
    public enum ExportStatus
    {
        New = 1,
        Exported = 2
    }
    public enum TransactionType
    {
        DistributorPOS=1,
        Invoice=2,
        Receipt=3,
        OutletToDistributor = 4,
        Unknown=5

    }
    public class TransactionImport
    {
        [Key]
        public Guid Id { get; set; }
        public string OutletName { get; set; }
        public string OutletCode { get; set; }
        public string GenericRef { get; set; }
        public string ExternalRef { get; set; }
        public DateTime TransactionIssueDate { get; set; }
        public DateTime TransactionDueDate { get; set; }

        public string SalemanName { get; set; }
        public string SalesmanCode { get; set; }

        //lineitem
        public string ProductCode { get; set; }
        public decimal Quantity { get; set; }
        public decimal TotalNet { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal LineItemValue { get; set; }
        public decimal GrossValue { get; set; }
        public decimal TotalVat { get; set; }
        public string PaymentType { get; set; }
        public string PaymentRef { get; set; }
        public string VatClass { get; set; }
        
        public int TransactionType { get; set; }
        public int ExportStatus { get; set; }
    }
}
