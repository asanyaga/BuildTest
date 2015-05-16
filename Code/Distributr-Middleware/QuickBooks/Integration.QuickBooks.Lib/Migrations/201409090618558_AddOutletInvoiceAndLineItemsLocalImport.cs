namespace Integration.QuickBooks.Lib.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOutletInvoiceAndLineItemsLocalImport : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OrderImportLocals",
                c => new
                    {
                        OrderExternalRef = c.String(nullable: false, maxLength: 128),
                        OutletCode = c.String(),
                        OrderRef = c.String(),
                        OrderDate = c.DateTime(nullable: false),
                        OrderDueDate = c.DateTime(nullable: false),
                        SalesmanCode = c.String(),
                        OrderType = c.Int(nullable: false),
                        OutletName = c.String(),
                        Note = c.String(),
                        RouteName = c.String(),
                        ShipToAddress = c.String(),
                        TotalNet = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalVat = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalGross = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalDiscount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DateOfImport = c.DateTime(nullable: false),
                        DateOfExport = c.DateTime(),
                        ExportStatus = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.OrderExternalRef);
            
            CreateTable(
                "dbo.OrderLineItemLocals",
                c => new
                    {
                        LineItemId = c.Guid(nullable: false),
                        OrderExternalRef = c.String(),
                        ProductCode = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        VatClass = c.String(),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        VatPerUnit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LineItemTotalNet = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LineItemTotalVat = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LineItemTotalGross = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DateOfImport = c.DateTime(nullable: false),
                        DateOfExport = c.DateTime(),
                        ExportStatus = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.LineItemId);
            
            CreateTable(
                "dbo.InvoiceImportLocals",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        OutletCode = c.String(),
                        InvoiceDocumentRef = c.String(),
                        OrderExternalRef = c.String(),
                        DocumentDateIssued = c.String(),
                        OrderDate = c.DateTime(nullable: false),
                        OrderDueDate = c.DateTime(nullable: false),
                        SalesmanCode = c.String(),
                        SalesmanName = c.String(),
                        OutletName = c.String(),
                        TotalNet = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalVat = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalGross = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalDiscount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DateOfImport = c.DateTime(nullable: false),
                        DateOfExport = c.DateTime(),
                        ExportStatus = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.InvoiceLineItemLocals",
                c => new
                    {
                        LineItemId = c.Guid(nullable: false),
                        InvoiceId = c.Guid(nullable: false),
                        ProductCode = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        VatClass = c.String(),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        VatPerUnit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LineItemTotalNet = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LineItemTotalVat = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LineItemTotalGross = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DateOfImport = c.DateTime(nullable: false),
                        DateOfExport = c.DateTime(),
                        ExportStatus = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.LineItemId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.InvoiceLineItemLocals");
            DropTable("dbo.InvoiceImportLocals");
            DropTable("dbo.OrderLineItemLocals");
            DropTable("dbo.OrderImportLocals");
        }
    }
}
