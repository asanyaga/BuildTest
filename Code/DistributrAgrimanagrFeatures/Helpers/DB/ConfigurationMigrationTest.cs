using System.Configuration;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Reflection;
using Distributr.WPF.Lib.Data.EF;

namespace DistributrAgrimanagrFeatures.Helpers.DB
{
    public sealed class ConfigurationMigrationTest : DbMigrationsConfiguration<DistributrLocalContextTest>
    {
        public ConfigurationMigrationTest()
        {
            var config = new DbMigrationsConfiguration<DistributrLocalContextTest>();
            string connectionString = ConfigurationManager.AppSettings["Hub_RoutingConnectionString"];
            config.TargetDatabase = new DbConnectionInfo(connectionString, "System.Data.SqlClient");
            config.MigrationsAssembly = Assembly.GetAssembly(typeof(DistributrLocalContext));
            config.MigrationsNamespace = "Distributr.WPF.Lib.Data.Migrations";
            config.ContextType = typeof (DistributrLocalContextTest);

            //--------- old
            DbMigrator migrator = new DbMigrator(config);

            config.AutomaticMigrationsEnabled = true;
            migrator.Update();
            var test = migrator.GetLocalMigrations().ToList();
            //migrator.Configuration.AutomaticMigrationsEnabled
            //if (migrator.GetPendingMigrations().Any())
            //{
            //    //_pendingMigrations = true;
            //    migrator.Update();
            //}
        }
    }
}