using System.Web.Mvc;

namespace Distributr.HQ.Web.Areas.OutletManager
{
    public class OutletManagerAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "OutletManager";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "OutletManager_default",
                "OutletManager/{controller}/{action}/{id}",
                new { controller = "OutletHome", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
