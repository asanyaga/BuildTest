using System;
using System.Data.Entity.Migrations;

namespace Integration.QuickBooks.Lib.Migrations
{
    public partial class AddReceiptLocalItemandReceiptLineItemLocal : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ReceiptImportLocals",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        OrderExternalRef = c.String(),
                        ReceiptRef = c.String(),
                        InvoiceRef = c.String(),
                        SalesmanCode = c.String(),
                        OutletCode = c.String(),
                        OrderTotalGross = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrderNetGross = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DateOfImport = c.DateTime(nullable: false),
                        DateOfExport = c.DateTime(),
                        ExportStatus = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ReceiptLineItemLocals",
                c => new
                    {
                        LineItemId = c.Guid(nullable: false),
                        ReceiptId = c.Guid(nullable: false),
                        ReceiptAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ModeOfPayment = c.String(),
                        ChequeNumber = c.String(),
                        DateOfImport = c.DateTime(nullable: false),
                        DateOfExport = c.DateTime(),
                        ExportStatus = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.LineItemId);
            
            AddColumn("dbo.InvoiceLineItemLocals", "InvoiceExternalRef", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.InvoiceLineItemLocals", "InvoiceExternalRef");
            DropTable("dbo.ReceiptLineItemLocals");
            DropTable("dbo.ReceiptImportLocals");
        }
    }
}
