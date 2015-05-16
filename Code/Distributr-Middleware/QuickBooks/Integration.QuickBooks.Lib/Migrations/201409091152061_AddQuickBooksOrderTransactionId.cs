namespace Integration.QuickBooks.Lib.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddQuickBooksOrderTransactionId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderImportLocals", "QbOrderTransactionId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderImportLocals", "QbOrderTransactionId");
        }
    }
}
