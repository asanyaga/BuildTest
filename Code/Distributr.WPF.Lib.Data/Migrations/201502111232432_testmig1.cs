namespace Distributr.WPF.Lib.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    /// <summary>
    /// Sample migration file to ensure that migrations work
    /// </summary>
    public partial class testmig1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SampleMig",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    EnvelopeId = c.Guid(nullable: false),
                    EnvelopeArrivedAtServerTick = c.Long(nullable: false),
                })
                .PrimaryKey(t => t.Id);
        }

        public override void Down()
        {
            DropTable("dbo.SampleMig");
        }
    }
}
