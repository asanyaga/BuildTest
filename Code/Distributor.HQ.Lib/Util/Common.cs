using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Configuration;
using Distributr.Core.Domain.Master.SettingsEntities;
using Distributr.Core.Repository.Master.SettingsRepositories;
using StructureMap;

namespace Distributr.HQ.Lib.Util
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
        public static MvcHtmlString MyResource(this HtmlHelper helper,string  resource)
        {
            string authority = helper.ViewContext.HttpContext.Request.Url.Authority;
            string scheme = helper.ViewContext.HttpContext.Request.Url.Scheme;
            var urlHelper = new UrlHelper(helper.ViewContext.RequestContext);
            string webroot = urlHelper.Content("~");
            string urlformat = string.Format("{0}://{1}{2}{3}", scheme, authority, webroot, resource);
            return MvcHtmlString.Create(urlformat);
        }
        public static MvcHtmlString ReportServerResource(this HtmlHelper helper, string resource)
        {
            string authority = ConfigurationManager.AppSettings["reportServerUrl"];
            string scheme = helper.ViewContext.HttpContext.Request.Url.Scheme;
            var urlHelper = new UrlHelper(helper.ViewContext.RequestContext);
            string webroot = urlHelper.Content("~");
            string urlformat = string.Format("{0}://{1}{2}{3}", scheme, authority, webroot, resource);
            return MvcHtmlString.Create(urlformat);
        }
        public static MvcHtmlString MapResource(this HtmlHelper helper)
        {
            string authority = helper.ViewContext.HttpContext.Request.Url.Authority;
          var urlSettings =  ObjectFactory.GetInstance<ISettingsRepository>().GetByKey(SettingsKeys.MapUri);
            if (urlSettings != null)
            {
                return MvcHtmlString.Create(urlSettings.Value );
            }
            return MvcHtmlString.Create("#");
        }
       
    }
}
