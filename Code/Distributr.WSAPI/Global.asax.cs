using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Distributr.BusSubscriber.Service;
using Distributr.Reports.IOC;
using Distributr.WSAPI.Server.IOC;
using EasyNetQ;
using Newtonsoft.Json.Linq;
using StructureMap;
using Distributr.Core.Data.IOC;
using Distributr.WSAPI.Lib.IOC;
using log4net;
using System.Configuration;
using Newtonsoft.Json;
using Distributr.WSAPI.Utility;
namespace Distributr.WSAPI
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            _log.InfoFormat("Registering Routes");
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            _log.Info("Application starting");
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            log4net.Config.XmlConfigurator.Configure();
            InitializeContainer();  
            new QUtility().SetupTimer();

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            _log.Error("An uncaught exception occurred", this.Server.GetLastError());
        }

        public void InitializeContainer()
        {
            string test = ConfigurationManager.AppSettings["MongoConnectionString"];
            ObjectFactory.Initialize(x =>
            {
                x.AddRegistry<DataRegistry>();
                x.AddRegistry<ReportRegistry>();
                x.AddRegistry<WSAPIRegistry>();
                x.For<IAdvancedBus>().Singleton().Use(BusBuilder.CreateMessageBus);
                
            });


            IContainer container = (IContainer)ObjectFactory.Container;
            DependencyResolver.SetResolver(new StructureMapDependencyResolver(container));
        
        }
    }
}

