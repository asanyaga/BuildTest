using System.Web.Mvc;

namespace Distributr.HQ.Web.Areas.Rpt
{
    public class RptAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Rpt";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Rpt_default",
                "Rpt/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
