using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace Distributr.WebApi.Models
{
    //moved to Distributr.WebApi.Common
    //public static class WebHelper
    //{
    //    public static void PagingParam(this ApiController api, out int take, out int skip)
    //    {

    //        int page = 0;
    //        var parameters = api.Request.RequestUri.ParseQueryString();
    //        Int32.TryParse(parameters["page"], out page);
    //        Int32.TryParse(parameters["pagesize"], out take);

    //        skip = (page - 1) * take;
    //    }

    //}
    public static class WebHelper
    {
        public static string GetVersion(this HtmlHelper htmlHelper)
        {

            return "Version : " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
    }
}
