using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Distributr.BusSubscriber.Service;
using Distributr.Core.Data.Script;
using Distributr.HQ.Lib.Helper;
using Distributr.HQ.Lib.ViewModels.Admin.Orders;
using Distributr.HQ.Web.IOC;
using Distributr.WSAPI.Lib.IOC;
using Distributr.WSAPI.Server.IOC;
using EasyNetQ;
using log4net;
using StructureMap;
using IDependencyResolver = System.Web.Mvc.IDependencyResolver;
using Distributr.HQ.Lib.IOC;
using Distributr.Core.Data.IOC;
using StructureMapDependencyResolver = Distributr.HQ.Lib.IOC.StructureMapDependencyResolver;
using Distributr.Core.Security;
using System.Security.Principal;

namespace Distributr.HQ.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            //for css compression
            routes.IgnoreRoute("Content/{resource}.axd/{*pathInfo}");
            //for js compression
            routes.IgnoreRoute("Scripts/{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("reports");
            //routes.IgnoreRoute("reports/{*pathInfo}");
            //routes.MapRoute("ovm",// Route name   

            //     "Orders/OrderedList/{page}",                           // URL with params        
            //     new { controller = "Orders", action = "Orders" } // Param defaults  
            //     );

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Account", action = "LogOn", id = UrlParameter.Optional } // Parameter defaults
            );

            
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            Bootstrap.Resolver();
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());
            log4net.Config.XmlConfigurator.Configure();
            LogManager.GetLogger(this.GetType());
            ModelBinders.Binders.Add(typeof(decimal), new DecimalModelBinder());
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(RequiredIfAttribute), typeof(RequiredAttributeAdapter));
            DistributrDataHelper.Migrate();
            CultureInfo ci = CultureInfo.CreateSpecificCulture("en-GB");
            ci.DateTimeFormat.ShortDatePattern = "dd-MMM-yyyy";
            Thread.CurrentThread.CurrentCulture = ci;
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            //bool isPrincipalSet = (this.User is CustomPrincipal);
            
            if (this.User != null )
            {
                string cachedKey = string.Format("cui_{0}", this.User.Identity.Name);
                CustomPrincipal cp = HttpContext.Current.Cache[cachedKey] as CustomPrincipal;
                if (cp == null)
                {
                    ISecurityService _securityService = ObjectFactory.GetInstance<ISecurityService>();
                    cp = _securityService.ConstructCustomPrincipal(this.User.Identity);
                    HttpContext.Current.Cache.Insert(cachedKey, cp);
                }
               
                this.Context.User = cp;
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            _log.Fatal("An uncaught exception occurred", this.Server.GetLastError());
        }

     

    }
}