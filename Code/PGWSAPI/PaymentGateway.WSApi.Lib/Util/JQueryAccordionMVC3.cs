using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;


namespace PaymentGateway.WSApi.Lib.Util
{
    public static class HtmlExtensions
    {
        public static MvcHtmlString MenuItem(this HtmlHelper helper, string linkText,
            string actionName, string controllerName, string areaName, string selectedClass)
        {
            return MenuItem(helper, linkText, actionName, controllerName, areaName, selectedClass, false);
        }

        public static MvcHtmlString MenuItem(this HtmlHelper helper, string linkText,
            string actionName, string controllerName, string areaName, string selectedClass, bool controllerDesplay)
        {
            string currentControllerName = (string)helper.ViewContext.RouteData.Values["controller"];
            string currentActionName = (string)helper.ViewContext.RouteData.Values["action"];
            string area = areaName;

            var builder = new TagBuilder("li");
            if (controllerDesplay)
            {
                if (currentControllerName.Equals(controllerName, StringComparison.CurrentCultureIgnoreCase))
                    builder.AddCssClass(selectedClass);
            }
            else
            {
                if (currentControllerName.Equals(controllerName, StringComparison.CurrentCultureIgnoreCase)
                    && currentActionName.Equals(actionName, StringComparison.CurrentCultureIgnoreCase))
                    builder.AddCssClass(selectedClass);
            }

            builder.InnerHtml = helper.ActionLink(linkText, actionName, controllerName, new { area = area }, null).ToHtmlString();
            return MvcHtmlString.Create(builder.ToString(TagRenderMode.Normal));
        }

        public static MvcHtmlString Controller(this HtmlHelper helper)
        {
            return MvcHtmlString.Create((string)helper.ViewContext.RouteData.Values["controller"]);
        }
        public static MvcHtmlString Area(this HtmlHelper helper)
        {
            string area = (string)helper.ViewContext.RouteData.DataTokens["area"];
            if (string.IsNullOrEmpty(area))
            {
                area = "Home";
            }
            return MvcHtmlString.Create(area);
        }
    }
}
