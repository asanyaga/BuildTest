using System.Web.Mvc;

namespace PaymentGateway.WSApi.Areas.Console
{
    public class ConsoleAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Console";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Console_default",
                "Console/{controller}/{action}/{id}",
                new {controller="console", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
