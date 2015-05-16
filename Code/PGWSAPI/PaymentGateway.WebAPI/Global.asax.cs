using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Principal;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dependencies;
using System.Web.Http.Dispatcher;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using PaymentGateway.WSApi.Lib.Data.IOC;
using PaymentGateway.WSApi.Lib.IOC;
using PaymentGateway.WSApi.Lib.Security;
using PaymentGateway.WSApi.Lib.Services.DistributrWSProxy;
using StructureMap;
using IDependencyResolver = System.Web.Http.Dependencies.IDependencyResolver;

namespace PaymentGateway.WebAPI
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            InitializeContainer();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

            if (this.User != null)
            {
                ISecurityService _securityService = ObjectFactory.GetInstance<ISecurityService>();
                IPrincipal cp = _securityService.ConstructCustomPrincipal(this.User.Identity);
                this.Context.User = cp;


            }
        }

        public void InitializeContainer()
        {

            ObjectFactory.Initialize(x =>
            {
                x.AddRegistry<PGWSAPIRegistry>();

            });
            IDistributorWebApiProxy test1 = ObjectFactory.GetInstance<IDistributorWebApiProxy>();
            IContainer container = (IContainer)ObjectFactory.Container;
            DependencyResolver.SetResolver(new StructureMapDependencyResolver(container));
            GlobalConfiguration.Configuration.DependencyResolver = new StructureMapResolver(container);
        }

        public class StructureMapResolver : StructureMapDependencyScope, IDependencyResolver, IHttpControllerActivator
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
    }
}