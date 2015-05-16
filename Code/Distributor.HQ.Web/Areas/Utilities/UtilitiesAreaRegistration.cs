using System.Web.Mvc;

namespace Distributr.HQ.Web.Areas.Utilities
{
    public class UtilitiesAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Utilities";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Utilities_default",
                "Utilities/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
