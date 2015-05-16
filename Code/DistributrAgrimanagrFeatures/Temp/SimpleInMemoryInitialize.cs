using System;
using System.Net.Http;
using Autofac;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.WPF.Lib.Services.Service.Utility;
using DistributrAgrimanagrFeatures.Helpers.DB;
using DistributrAgrimanagrFeatures.Helpers.Initialise;
using DistributrAgrimanagrFeatures.Helpers.IOC;
using DistributrAgrimanagrFeatures.Helpers.TestTracing;
using Microsoft.Owin.Hosting;
using NUnit.Framework;
using StructureMap;
using IContainer = Autofac.IContainer;

namespace DistributrAgrimanagrFeatures.Initialise
{
   
    /// <summary>
    /// Used to test the testing bits
    /// </summary>
    [TestFixture]
    public class SimpleInMemoryInitialize
    {
        //[Test, Ignore]
        //public void IocNinjectWireup()
        //{
        //    DB_TestingHelper cs = TestingIOC.GetDefaultDbTestingHelper();
        //    TestingIOC.InitializeServerWithNinjectContainer(cs.Server_DistributrExmxConnection, cs.MongoConnectionString, "win", cs.MongoAuditingConnectionString);
        //    IProductDiscountRepository pdr = IOCHelperOld.NinjectKernel.Get<IProductDiscountRepository>();
        //}

        [Test][Ignore]
        public void IocAutofacWireup()
        {
            DB_TestingHelper cs = DefaultDbTestingHelper.GetDefaultDbTestingHelper();
            IContainer c = IOCHelper.InitializeServerWithAutofacContainer(cs.Server_DistributrExmxConnection, cs.MongoConnectionString, "win", cs.MongoAuditingConnectionString).Build();
            //IProductDiscountRepository pdr = c.Resolve<IProductDiscountRepository>();
            ICostCentreRepository cc = c.Resolve<ICostCentreRepository>();
            var test = cc.GetAll();
          
        }

        [Test]
        [Ignore]
        public void IocStructuremapWireup()
        {
            DB_TestingHelper cs = DefaultDbTestingHelper.GetDefaultDbTestingHelper();
            IOCHelper.InitialiseHubSliceWithStructurmapContainer(cs.Hub_DistributrEdmxConnection, cs.Hub_RoutingConnectionString, "");
            //IProductDiscountRepository pdr = ObjectFactory.GetInstance<IProductDiscountRepository>();
           
            ObjectFactory.GetInstance<InitializationHelper>().AppIdSetup();
        }

        [Test]
        [Ignore]
        public void SetupAllDatabases()
        {
            DB_TestingHelper dbh = DefaultDbTestingHelper.GetDefaultDbTestingHelper();
            IContainer c = IOCHelper.InitializeServerWithAutofacContainer(dbh.Server_DistributrExmxConnection, dbh.MongoConnectionString, "win", dbh.MongoAuditingConnectionString).Build();
            IOCHelper.InitialiseHubSliceWithStructurmapContainer(dbh.Hub_DistributrEdmxConnection, dbh.Hub_RoutingConnectionString, "");
            DbSetupHelper.SetupAllDatabases(dbh, c, ObjectFactory.Container);
            ObjectFactory.Container.Dispose();

        }

        [Test]
        [Ignore]
        public void RunThruSetup()
        {
            DB_TestingHelper dbh = DefaultDbTestingHelper.GetDefaultDbTestingHelper();
            IOCHelper.InitialiseHubSliceWithStructurmapContainer(dbh.Hub_DistributrEdmxConnection, dbh.Hub_RoutingConnectionString, "");
            string wsurl = "http://localhost:9443/";

            InitializationHelper helper = ObjectFactory.GetInstance<InitializationHelper>();
            ClientApplication app = helper.AppIdSetup();
            helper.SetConfigWebserviceUrl(wsurl);
            TI.trace("RunThruSetup", "#1");
            using (var webApp = WebApp.Start<StartupServerAutofac>("http://localhost:9443/"))
            {
                TI.trace("RunThruSetup", "#1a");

                CostCentreLoginResponse r = helper.InitialLogin("kameme", "12345678", UserType.WarehouseManager);
                Guid ccappid = helper.GetCostCentreApplicationIdAndAppInitialise().Result;
                bool canSync = helper.CanSync();
                //bool masterDateSync = helper.SyncMasterData().Result;
            }

            TI.trace("RunThruSetup", "#1b");

        }



        [Test]
        [Ignore]
        public void SimpleCall()
        {
            using (var webApp = WebApp.Start<StartupServerAutofac>("http://localhost:9443/"))
            {
                using (var httpClient = new HttpClient())
                {
                    // Execute test against the web API.
                    var requestUri = new Uri("http://localhost:9443/api2/ping3/getisalive");
                    var r = httpClient.GetAsync(requestUri).Result;

                }
                webApp.Dispose();
            }

            //using (var server = TestServer.Create<StartupServerAutofac>())
            //{
            //    using (var httpClient = new HttpClient())
            //     {
            //         //var httpClient = new HttpClient(new AddHeaderHttpHandler(server.Handler));
            //         //var response = await httpClient.GetAsync("http://localhost:1234/");

            //         var requestUri = new Uri("http://localhost/api2/ping2/getisalive");
            //         httpClient.BaseAddress = new Uri("http://localhost");
            //         var r = httpClient.GetAsync("/api2/ping2/getisalive").Result;

            //     }


            //    // Execute test against the web API.
            //    var result = server.HttpClient.GetAsync("/api2/ping2/getisalive").Result;
            //}
        }
    }

    //public partial class Startup
    //{
    //    public void Configuration(IAppBuilder app)
    //    {
    //        var configuration = new HttpConfiguration();
    //        //configuration.MapHttpAttributeRoutes();
    //        configuration.EnableSystemDiagnosticsTracing();
    //        //configuration.Routes.MapHttpRoute(
    //        //    name: "DefaultApi",
    //        //    routeTemplate: "api/{controller}/{id}",
    //        //    defaults: new { id = RouteParameter.Optional }
    //        //); 
    //        WebApiConfig.RegisterRoutes(configuration);
    //        WebApiConfig.RegisterIntegrationRoutes(configuration);
    //        app.UseWebApi(configuration);
    //        TestingIOC.InitialiseContainer(configuration);

    //    }
    //}

    /*public partial class StartupServerNinject
    {
        public void Configuration(IAppBuilder app)
        {
            var configuration = new HttpConfiguration();
            //configuration.MapHttpAttributeRoutes();
            configuration.EnableSystemDiagnosticsTracing();
            //configuration.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //); 
            WebApiConfig.RegisterRoutes(configuration);
            WebApiConfig.RegisterIntegrationRoutes(configuration);
            DB_TestingHelper dbHelper = TestingIOC.GetDbTestingHelper();
            IKernel k = TestingIOC.InitializeServerWithNinjectContainer(dbHelper.Server_DistributrExmxConnection, dbHelper.MongoConnectionString, "win", dbHelper.MongoAuditingConnectionString);
            app.UseNinjectMiddleware(() => k).UseNinjectWebApi(configuration);
        }


    }*/
}
