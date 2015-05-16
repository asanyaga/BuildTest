using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Distributr.BusSubscriber.Service;
using Distributr.Core.Data.IOC;
using Distributr.Core.Data.Script;
using Distributr.Core.Security;
using Distributr.Core.Utility.Mapping.impl;
using Distributr.HQ.Lib.IOC;
using Distributr.WSAPI.Server.IOC;
using EasyNetQ;
using StructureMap;

namespace Agrimanagr.HQ
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        protected void Application_Start()
        {
            //EnableOptimization
            AreaRegistration.RegisterAllAreas();
            InitializeContainer();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            DTOToEntityMapping.SetupAutomapperMappings();
            MasterDataToDTOMapping.SetupMapper();
            DistributrDataHelper.Migrate();

           
        }
        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            if (this.User != null)
            {
                ISecurityService _securityService = ObjectFactory.GetInstance<ISecurityService>();
                IPrincipal cp = _securityService.ConstructAgriCustomPrincipal(this.User.Identity);
                this.Context.User = cp;

            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            _log.Fatal("An uncaught exception occurred", this.Server.GetLastError());
        }

        public void InitializeContainer()
        {

            ObjectFactory.Initialize(x =>
            {
                x.AddRegistry<DataRegistry>();
                x.AddRegistry<ViewModelBuilderRegistry>();
                x.AddRegistry<WSAPIRegistry>();
                x.AddRegistry<BusRegistry>();
                x.AddRegistry<MongoRegistry>();
                x.For<IAdvancedBus>().Singleton().Use(BusBuilder.CreateMessageBus);
            });
            IContainer container = (IContainer)ObjectFactory.Container;
            DependencyResolver.SetResolver(new StructureMapDependencyResolver(container));
        }
    }
}