using System.Web.Mvc;

namespace Agrimanagr.HQ.Areas.Agrimanagr
{
    public class AgrimanagrAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Agrimanagr";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Agrimanagr_default",
                "Agrimanagr/{controller}/{action}/{id}",
                new {area="agrimanagr", controller="Agrimain", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
