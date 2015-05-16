using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Distributr.Azure.Lib.Audit;
using Distributr.Azure.Lib.Bus;
using Distributr.Azure.Lib.CommandProcessing.Notification;
using Distributr.Azure.Lib.CommandProcessing.Routing;
using Distributr.Azure.Lib.IOC;
using Distributr.BusSubscriber.Service;
using Distributr.Core.Data.IOC;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Utility.Mapping.impl;
using Distributr.WSAPI.Lib.IOC;
using Distributr.WSAPI.Lib.Services.Bus;
using Distributr.WSAPI.Lib.Services.Bus.Impl;
using Distributr.WSAPI.Lib.Services.CommandAudit;
using Distributr.WSAPI.Lib.Services.Routing;
using Distributr.WSAPI.Lib.System.Utility;
using Distributr.WSAPI.Server.IOC;
using Distributr.WebApi;
using Distributr.WebApi.Common.App_Start;
using Microsoft.WindowsAzure;
using StructureMap;
using TestAzureTables.Impl;
using log4net;

namespace Distributr.Azure.WebApi
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class WebApiApplication : System.Web.HttpApplication
    {
        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected void Application_Start()
        {
            GlobalConfiguration.Configuration.EnableSystemDiagnosticsTracing();

            RegisterAzureRoutes(GlobalConfiguration.Configuration);
            WebApiConfig.RegisterRoutes(GlobalConfiguration.Configuration);


            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            log4net.Config.XmlConfigurator.Configure();



            InitializeContainer();
            DTOToEntityMapping.SetupAutomapperMappings();

            IProductRepository pr = ObjectFactory.GetInstance<IProductRepository>();

        }

        public static void RegisterAzureRoutes(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(name: "CostCentreApplicationCreate",
                  routeTemplate: "CostCentreApplication/createcostcentreapplication",
                       defaults: new { controller = "CostCentreApplication", action = "GetCreateCostCentreApplication" });

            config.Routes.MapHttpRoute(name: "AzureCommandController",
                routeTemplate: "api/command/run",
                     defaults: new { controller = "AzureCommand", action = "Run" });

            config.Routes.MapHttpRoute(name: "AzureTestController",
                routeTemplate: "Test/gettestcostcentre",
                     defaults: new { controller = "Test", action = "GetTestCostCentre" });

            config.Routes.MapHttpRoute(name: "LoginController",
               routeTemplate: "Login/Login",
                    defaults: new { controller = "Login", action = "LoginGet" });
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            _log.Error("An uncaught exception occurred", this.Server.GetLastError());
        }

        public void InitializeContainer()
        {
            ObjectFactory.Initialize(x =>
            {
                x.AddRegistry<DataRegistry>();
                x.AddRegistry<SyncRegistry>();
                x.AddRegistry<WSAPIRegistry>();
                //x.AddRegistry<BusRegistry>();
                //x.AddRegistry<MongoRegistry>();
                x.AddRegistry<IntegrationsRegistry>();
                //x.For<IAdvancedBus>().Singleton().Use(BusBuilder.CreateMessageBus);

                //azure 
                string storageConnectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");

                x.For<ICommandProcessingAuditRepository>()
              .Use<AzureCommandProcessingAuditRepository>()
              .Ctor<string>("storageConnectionString")
              .Is(storageConnectionString);

                x.For<ICCAuditRepository>()
                    .Use<AzureCCAuditRepository>()
                    .Ctor<string>("storageConnectionString")
                 .Is(storageConnectionString);

                x.For<INotificationProcessingAuditRepository>()
                  .Use<AzureNotificationProcessingAuditRepository>()
                  .Ctor<string>("storageConnectionString")
               .Is(storageConnectionString);

                x.For<ICommandRoutingOnRequestRepository>()
                  .Use<AzureCommandRoutingOnRequestRepository>()
                  .Ctor<string>("storageConnectionString")
               .Is(storageConnectionString)
               .Ctor<string>("sqlConnectionString")
               .EqualToAppSetting("cokeconnectionstring");

            });
            ObjectFactory.Configure(n => n.For<IBusPublisher>().Use<AzureBusPublisher>());
            ObjectFactory.Configure(n => n.For<IControllerBusPublisher>().Use<AzureBusPublisher>());
            ObjectFactory.Configure(n => n.For<IBusSubscriber>().Use<MainBusSubscriber>());


            IContainer container = (IContainer)ObjectFactory.Container;
            DependencyResolver.SetResolver(new StructureMapDependencyResolver(container));
            GlobalConfiguration.Configuration.DependencyResolver = new StructureMapResolver(container);
        }
    }
}