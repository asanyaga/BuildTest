namespace Distributr.WPF.Lib.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeUnexecutedCommandLocals : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.UnExecutedCommandLocals", "Command", c => c.String(maxLength: 1000));
            AlterColumn("dbo.UnExecutedCommandLocals", "Reason", c => c.String(maxLength: 1000));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.UnExecutedCommandLocals", "Reason", c => c.String());
            AlterColumn("dbo.UnExecutedCommandLocals", "Command", c => c.String());
        }
    }
}
