using System;
using System.Configuration;
using System.Runtime.Remoting.Contexts;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Distributr.BusSubscriber.Service;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.IOC;
using Distributr.Core.Data.Script;
using Distributr.Core.Resources.Util;
using Distributr.Core.Utility.Mapping.impl;
using Distributr.WSAPI.Lib.IOC;
using Distributr.WSAPI.Lib.ResponseBuilders.MasterData;
using Distributr.WSAPI.Lib.Services;
using Distributr.WSAPI.Server.IOC;
using Distributr.WSAPI.Utility;
using EasyNetQ;
using StructureMap;
using log4net;
using Distributr.WebApi.Common.App_Start;

namespace Distributr.WebApi
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
         
            WebApiConfig.RegisterRoutes(GlobalConfiguration.Configuration);
            WebApiConfig.RegisterIntegrationRoutes(GlobalConfiguration.Configuration);

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            GlobalConfiguration.Configuration.EnsureInitialized();
            log4net.Config.XmlConfigurator.Configure();
            InitializeContainer();
            DTOToEntityMapping.SetupAutomapperMappings();
            MasterDataToDTOMapping.SetupMapper();
           // new QUtility().SetupTimer();
         
            DistributrDataHelper.Migrate();

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
                x.AddRegistry<BusRegistry>();
                x.AddRegistry<MongoRegistry>();
                x.AddRegistry<IntegrationsRegistry>();
                x.AddRegistry<IntegrationServiceRegistry>();
                x.For<IAdvancedBus>().Singleton().Use(BusBuilder.CreateMessageBus);
                x.For<IMessageSourceAccessor>().Use(MessageSourceAccessor.GetInstance("web"));

                x.For<IMessageSourceAccessor>().Use(MessageSourceAccessor.GetInstance("web"));

            });
            IContainer container = (IContainer)ObjectFactory.Container;
            DependencyResolver.SetResolver(new StructureMapDependencyResolver(container));
            GlobalConfiguration.Configuration.DependencyResolver = new StructureMapResolver(container);
        }
    }
}