using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dependencies;
using System.Web.Http.Dispatcher;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Distributr.Core.Data.IOC;
using Distributr.CustomerSupport.Code.CCAudit.Impl;
using Distributr.CustomerSupport.Code.CCCommandProcessing;
using Distributr.CustomerSupport.Code.CCCommandProcessing.Impl;
using Distributr.CustomerSupport.Code.CCRouting;
using Distributr.CustomerSupport.Code.CCRouting.Impl;
using Distributr.CustomerSupport.Code.Summary;
using Distributr.CustomerSupport.Code.Tools;
using Distributr.MongoDB.CommandRouting;
using Distributr.WSAPI.Lib.IOC;
using Distributr.WSAPI.Lib.Services.Routing;
using Distributr.WSAPI.Server.IOC;
using StructureMap;
using Distributr.CustomerSupport.Code.CCAudit;
using IDependencyResolver = System.Web.Mvc.IDependencyResolver;

namespace Distributr.CustomerSupport
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            InitializeContainer();
           
        }

        public void InitializeContainer()
        {

            ObjectFactory.Initialize(x =>
            {
                x.AddRegistry<DataRegistry>();
                x.AddRegistry<WSAPIRegistry>();
                x.AddRegistry<BusRegistry>();
                x.AddRegistry<MongoRegistry>();
                x.For<ICCAuditViewModelBuilder>().Use<CCAuditViewModelBuilder>();
                x.For<ICCCommandProcessingViewModelBuilder>().Use<CCCommandProcessingViewModelBuilder>();
                x.For<ICCRoutingVMBuilder>().Use<CCRoutingVMBuilder>();
                x.For<ICCCommandAuditViewModelBuilder>().Use<CCCommandAuditViewModelBuilder>();
                x.For<IToolViewModelBuilder>().Use<ToolViewModelBuilder>();
                x.For<ICommandRoutingOnRequestRepository>().Use<CommandRoutingOnRequestMongoRepository>();
                x.For<ISummaryVMBuilder>().Use<SummaryVMBuilder>();

            });

            IContainer container = (IContainer)ObjectFactory.Container;
            DependencyResolver.SetResolver(new StructureMapDependencyResolver(container));
            GlobalConfiguration.Configuration.DependencyResolver = new StructureMapResolver(container);

        }
    }

    public class StructureMapDependencyScope : IDependencyScope
    {
        private IContainer container;

        public StructureMapDependencyScope(IContainer container)
        {
            if (container == null)
                throw new ArgumentNullException("container");

            this.container = container;
        }

        public object GetService(Type serviceType)
        {
            if (container == null)
                throw new ObjectDisposedException("this", "This scope has already been disposed.");

            return container.TryGetInstance(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            if (container == null)
                throw new ObjectDisposedException("this", "This scope has already been disposed.");

            return container.GetAllInstances(serviceType).Cast<object>();
        }

        public void Dispose()
        {
            if (container != null)
                container.Dispose();

            container = null;
        }
    }

    public class StructureMapResolver : StructureMapDependencyScope, System.Web.Http.Dependencies.IDependencyResolver, IHttpControllerActivator
    {
        private readonly IContainer container;

        public StructureMapResolver(IContainer container)
            : base(container)
        {
            if (container == null)
                throw new ArgumentNullException("container");

            this.container = container;

            this.container.Inject(typeof(IHttpControllerActivator), this);
        }

        public IDependencyScope BeginScope()
        {
            return new StructureMapDependencyScope(container.GetNestedContainer());
        }

        public IHttpController Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, Type controllerType)
        {
            return container.GetNestedContainer().GetInstance(controllerType) as IHttpController;
        }
    }
}