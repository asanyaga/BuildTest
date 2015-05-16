using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Integration.QuickBooks.Lib.EF.Entities;

namespace Integration.QuickBooks.Lib.EF
{
    public class AlidiLocalImportDatabaseContext : DbContext
    {
        public DbSet<TransactionImport> TransactionImports { get; set; }
        public DbSet<OrderImportLocal> OrderImportLocal { get; set; }
        public DbSet<OrderLineItemLocal> OrderLineItemLocal { get; set; }
        public DbSet<InvoiceImportLocal> InvoiceImportLocal { get; set; }
        public DbSet<InvoiceLineItemLocal> InvoiceLineItemLocal { get; set; }
        public DbSet<ReceiptImportLocal> ReceiptImportLocal { get; set; }
        public DbSet<ReceiptLineItemLocal> ReceiptLineItemLocal { get; set; }
    }
}
