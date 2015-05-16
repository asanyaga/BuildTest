using System.Reflection;
using System.Web.Mvc;

namespace PaymentGateway.WebAPI.Models
{
    public static class PGHelper
    {
        public static string GetVersion(this HtmlHelper htmlHelper)
        {

            return "Version : " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
    }
}