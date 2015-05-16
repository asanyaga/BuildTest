using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Distributr.Core.Data.EF;
using Distributr.DatabaseSetup;
using DistributrAgrimanagrFeatures.Helpers.TestTracing;
using IContainer = StructureMap.IContainer;

namespace DistributrAgrimanagrFeatures.Helpers.DB
{
    public class DbSetupHelper
    {
        public static void SetupAllDatabases(DB_TestingHelper dbTestingHelper, Autofac.IContainer autofacContainer,
            StructureMap.IContainer structureMapContainer)
        {
            TI.trace("Begin database setup");

            SetupServerDb(dbTestingHelper, autofacContainer);

            SetupHubDistributr(dbTestingHelper, structureMapContainer);

            SetupHubLocal(dbTestingHelper);

           
            SetupMongo(dbTestingHelper);
        }

        public static void SetupServerDb(DB_TestingHelper dbTestingHelper, Autofac.IContainer autofacContainer)
        {
//server sql dist
            TI.trace("Remove all tables from Server distributr");
            dbTestingHelper.RemoveAllServerTables();
            IInsertTestData td = autofacContainer.Resolve<IInsertTestData>();
            CokeDataContext ctxServer = autofacContainer.Resolve<CokeDataContext>();
            TI.trace("Run scripts and add data to server");
            dbTestingHelper.SetupServerSql(true, ctxServer, td);
        }

        public static void SetupMongo(DB_TestingHelper dbTestingHelper)
        {
//setup mongo
            TI.trace("Drop and recreate mongo audit collections");
            dbTestingHelper.DropAndRecreateMongoAuditCollections();
            TI.trace("Drop and recreate mongo routing collections");
            dbTestingHelper.DropAndRecreateMongoRoutingCollections();
        }

        public static void SetupHubLocal(DB_TestingHelper dbTestingHelper)
        {
//hub sql local
            TI.trace("Remove all tables from Hub local db");
            dbTestingHelper.RemoveAllHubLocalTables();
            TI.trace("Setup hub local db");
            dbTestingHelper.SetupHubLocalSql();
        }

        public static void SetupHubDistributr(DB_TestingHelper dbTestingHelper, IContainer structureMapContainer)
        {
//hub sql dist
            CokeDataContext ctxHubDist = structureMapContainer.GetInstance<CokeDataContext>();
            TI.trace("Remove all tables from Hub distributr db");
            dbTestingHelper.RemoveAllHubDistTables();
            TI.trace("Setup Hub distributr db");
            dbTestingHelper.SetupHubDistSql(ctxHubDist);
        }
    }
}
