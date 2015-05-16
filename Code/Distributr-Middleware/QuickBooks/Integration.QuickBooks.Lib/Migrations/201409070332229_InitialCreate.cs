namespace Integration.QuickBooks.Lib.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TransactionImports",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        OutletName = c.String(),
                        OutletCode = c.String(),
                        GenericRef = c.String(),
                        ExternalRef = c.String(),
                        TransactionIssueDate = c.DateTime(nullable: false),
                        TransactionDueDate = c.DateTime(nullable: false),
                        SalemanName = c.String(),
                        SalesmanCode = c.String(),
                        ProductCode = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalNet = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalDiscount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LineItemValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GrossValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalVat = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PaymentType = c.String(),
                        PaymentRef = c.String(),
                        VatClass = c.String(),
                        TransactionType = c.Int(nullable: false),
                        ExportStatus = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TransactionImports");
        }
    }
}
