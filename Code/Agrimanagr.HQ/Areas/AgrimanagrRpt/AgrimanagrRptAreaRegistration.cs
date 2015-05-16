using System.Web.Mvc;

namespace Agrimanagr.HQ.Areas.AgrimanagrRpt
{
    public class AgrimanagrRptAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "AgrimanagrRpt";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "AgrimanagrRpt_default",
                "AgrimanagrRpt/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
