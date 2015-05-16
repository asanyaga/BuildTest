
using System.Configuration;
using System.Data.Entity.Migrations;
using System.Linq;
using Distributr.WPF.Lib.Data.EF;

namespace Distributr.WPF.Lib.Data.Migrations
{
    public class DistributrLocalContextAuto : DistributrLocalContext
    {
        public DistributrLocalContextAuto()
            : base(ConfigurationManager.ConnectionStrings["DistributrLocalContext"].ConnectionString)
        {
                
        }
    }

    public sealed class ConfigurationMigritation : DbMigrationsConfiguration<DistributrLocalContextAuto>
    {
       
       public ConfigurationMigritation()
       {
           
            DbMigrator migrator = new DbMigrator(this);
           AutomaticMigrationsEnabled = false;
           AutomaticMigrationDataLossAllowed = true;
           //migrator.Configuration.AutomaticMigrationsEnabled
            if (migrator.GetPendingMigrations().Any())
            {
                //_pendingMigrations = true;
                migrator.Update();
            }
          
        }

       protected override void Seed(DistributrLocalContextAuto context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}
