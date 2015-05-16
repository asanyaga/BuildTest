using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using PaymentGateway.WSApi.Lib.Data.IOC;
using PaymentGateway.WSApi.Lib.IOC;
using PaymentGateway.WSApi.Lib.Security;
using StructureMap;

namespace PaymentGateway.WSApi
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("rpt");
            routes.IgnoreRoute("rpt/{*pathInfo}");
            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

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
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            InitializeContainer();
        }
        public void InitializeContainer()
        {

            ObjectFactory.Initialize(x =>
            {
                x.AddRegistry<PGWSAPIRegistry>();

            });
            IContainer container = (IContainer)ObjectFactory.Container;
            DependencyResolver.SetResolver(new StructureMapDependencyResolver(container));
        }
    }
}