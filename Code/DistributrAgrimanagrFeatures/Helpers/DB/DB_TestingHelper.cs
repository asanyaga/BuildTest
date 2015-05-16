using Distributr.Core.Data.EF;
using RebuildDB;
using System;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using Distributr.Core.Data.Script;
using Distributr.DatabaseSetup;
using Distributr.WPF.Lib.ViewModels.TestFrames;
using DistributrAgrimanagrFeatures.Helpers.TestTracing;
using MongoDB.Driver;

namespace DistributrAgrimanagrFeatures.Helpers.DB
{
    public class DB_TestingHelper
    {
        string _distributrEdmxTemplate = "metadata=res://*/EF.CokeData.csdl|res://*/EF.CokeData.ssdl|res://*/EF.CokeData.msl;provider=System.Data.SqlClient;provider connection string=\"{0}\"";
        string _hubRoutingEdmxTemplate = "metadata=res://*/EF.CokeCommandRouting.csdl|res://*/EF.CokeCommandRouting.ssdl|res://*/EF.CokeCommandRouting.msl;provider=System.Data.SqlClient;provider connection string=\"{0}\"";
        public DB_TestingHelper(string hub_DistributrConnectionString, string hub_SetupConnectionString,
            string server_distributrConnectionString, string mongo_ConnectionString,
            string mongo_AuditingConnectionString, string createTablesScriptLocation)
        {
            Hub_DistributrConnectionstring = hub_DistributrConnectionString;
            Hub_RoutingConnectionString = hub_SetupConnectionString;
            Server_DistributrConnectionString = server_distributrConnectionString;
            MongoConnectionString = mongo_ConnectionString;
            MongoAuditingConnectionString = mongo_AuditingConnectionString;
            CreateTablesScriptLocation = createTablesScriptLocation;
        }

        public string Hub_DistributrConnectionstring { get; set; }
        public string Hub_RoutingConnectionString { get; set; }
        public string Server_DistributrConnectionString { get; set; }
        public string MongoConnectionString { get; set; }
        public string MongoAuditingConnectionString { get; set; }
        public string CreateTablesScriptLocation { get; set; }

        public string Hub_DistributrEdmxConnection
        {
            get { return string.Format(_distributrEdmxTemplate, Hub_DistributrConnectionstring); }
        }

        public string Server_DistributrExmxConnection
        {
            get { return string.Format(_distributrEdmxTemplate, Server_DistributrConnectionString); }

        }

        public string Hub_RoutingEdmxConnection
        {
            get { return string.Format(_hubRoutingEdmxTemplate, Hub_RoutingConnectionString); }
        }



        public void RemoveAllServerTables()
        {
            RebuildDb.dropalltables(Server_DistributrConnectionString);
        }

        public void SetupServerSql(bool insertTestData, CokeDataContext ctx, IInsertTestData insertTestDataService)
        {
            //run script.sql
            RebuildDb.RecreateTables(Server_DistributrConnectionString, CreateTablesScriptLocation);
            //migrate scripts
            DistributrDataHelper.Migrate(ctx);
            //insert test data
            if (insertTestData)
                insertTestDataService.InsertTestMasterData();
        }

        public void RemoveAllHubDistTables()
        {
            RebuildDb.dropalltables(Hub_DistributrConnectionstring);
        }

        public void SetupHubDistSql(CokeDataContext ctx)
        {
            RebuildDb.RecreateTables(Hub_DistributrConnectionstring, CreateTablesScriptLocation);
            DistributrDataHelper.Migrate(ctx);
        }

        public void RemoveAllHubLocalTables()
        {
            RebuildDb.dropalltables(Hub_RoutingConnectionString);
            string sqlDropMigrations =
                @"IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE 
                    TABLE_NAME = '__MigrationHistory')) BEGIN DROP TABLE [dbo].[__MigrationHistory] END";
            using (SqlConnection conn = new SqlConnection(Hub_RoutingConnectionString))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand comm = new SqlCommand(sqlDropMigrations, conn))
                    {
                        comm.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public void SetupHubLocalSql()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<DistributrLocalContextTest, ConfigurationMigrationTest>());

        }

        public void DropAndRecreateMongoRoutingCollections()
        {
            MongoDatabase db = getMongoDatabase(MongoConnectionString);
            string[] collections = new[] { "CommandEnvelopeRouteOnRequestCostcentre", "CommandEnvelopeRoutingStatus", 
                "CommandEnvelopeProcessingAudit","NotificationProcessingAudit","NotificationProcessingAuditInfo"
            };
            DropAndCreateCollections(db, collections);
        }

        public void DropAndRecreateMongoAuditCollections()
        {
            MongoDatabase db = getMongoDatabase(MongoAuditingConnectionString);
            string[] collections = new[] {"audititem"};
            //drop
            DropAndCreateCollections(db, collections);
        }

        private static void DropAndCreateCollections(MongoDatabase db, string[] collections)
        {
            string[] names = Enumerable.ToArray<string>(db.GetCollectionNames());
            foreach (string name in names)
            {
                if (collections.Contains(name))
                {
                    db.DropCollection(name);
                }
            }

            foreach (string collection in collections)
            {
                db.CreateCollection(collection);
            }
        }

        private MongoDatabase getMongoDatabase(string connectionString)
        {
            var databaseName = MongoUrl.Create(connectionString).DatabaseName;
            var server = MongoServer.Create(connectionString);
            return server.GetDatabase(databaseName);
        }

        public bool CheckDBsExist()
        {
            TI.trace("CheckDBsExist begin");
            bool localHubDbOk = CheckDatabaseExists(Hub_RoutingConnectionString);
            bool hubDistributrOk = CheckDatabaseExists(Hub_DistributrConnectionstring);
            bool serverDistributrOk = CheckDatabaseExists(Server_DistributrConnectionString);
            TI.trace("CheckDBsExist end");
            return localHubDbOk && hubDistributrOk && serverDistributrOk;
        }

        public bool CanConnectToMongo()
        {
            TI.trace(string.Format("Attempting connection to Mongo {0}", MongoConnectionString) );
            try
            {
                MongoServer mongo = MongoServer.Create(MongoConnectionString);
                mongo.Connect();
                TI.trace("Mongo connection success");
                return true;
            }
            catch (Exception)
            {
                TI.trace("Mongo connection failed");
            }
            return false;
        }

        private bool CheckDatabaseExists(string connectionString)
        {
            TI.trace(string.Format("Attempting connection to the database with connection string {0} ", connectionString));
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("select 1", conn))
                    {
                        cmd.ExecuteScalar();
                    }
                }
                TI.trace("connection ok");
                return true;
            }
            catch (Exception ex)
            {
                TI.trace("connection failed");
            }
            return false;
        }



    }
}
