using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace PaymentGateway.WSApi.Lib.Util
{
    public static class Common
    {
        public static MvcHtmlString MyHost(this HtmlHelper helper)
        {
            string authority = helper.ViewContext.HttpContext.Request.Url.Authority;
            string scheme = helper.ViewContext.HttpContext.Request.Url.Scheme;
            var urlHelper = new UrlHelper(helper.ViewContext.RequestContext);
            string webroot = urlHelper.Content("~");
            string urlformat = string.Format("{0}://{1}{2}", scheme, authority, webroot);
            return MvcHtmlString.Create(urlformat);
        }
    }
}
