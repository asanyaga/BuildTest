using System.Diagnostics;
using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Distributr.WebApi.Common.App_Start;
using DistributrAgrimanagrFeatures.Helpers.DB;
using DistributrAgrimanagrFeatures.Helpers.TestTracing;
using Owin;

namespace DistributrAgrimanagrFeatures.Helpers.IOC
{
    public class StartupServerAutofac
    {
        public void Configuration(IAppBuilder app)
        {
            TI.trace("StartupServerAutofac", "Begin in memory server setup");
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var configuration = new HttpConfiguration();
            configuration.EnableSystemDiagnosticsTracing();
            //configuration.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //); 
            //add some temp route hacks
            configuration.Routes.MapHttpRoute(
                name: "Login1",
                routeTemplate: "Login/Login",
                defaults: new { controller = "Login", action = "LoginGet" });
             
            WebApiConfig.RegisterRoutes(configuration);
            WebApiConfig.RegisterIntegrationRoutes(configuration);
            DB_TestingHelper dbHelper = DefaultDbTestingHelper.GetDefaultDbTestingHelper();
            ContainerBuilder b = IOCHelper.InitializeServerWithAutofacContainer(dbHelper.Server_DistributrExmxConnection, dbHelper.MongoConnectionString, "win", dbHelper.MongoAuditingConnectionString);
            Assembly a = Assembly.GetAssembly(typeof(Distributr.WebApi.ApiControllers.CommandController));
            b.RegisterApiControllers(a);
            Autofac.IContainer c = b.Build();
            System.Web.Http.Dependencies.IDependencyResolver r = new AutofacWebApiDependencyResolver(c) ;
            configuration.DependencyResolver = r;
            app.UseAutofacMiddleware(c);
            app.UseAutofacWebApi(configuration);
            app.UseWebApi(configuration);
            sw.Stop();
            TI.trace("[StartupServerAutofac]",
                string.Format("Time taken to setup in memory server {0}", sw.ElapsedMilliseconds));

        }
    }
}