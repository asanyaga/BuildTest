using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Distributr.Core.Resources.Util;
using StructureMap;

namespace Agrimanagr.HQ.Models
{
    public static class AgriWebHelper
    {
        public static string GetVersion(this HtmlHelper htmlHelper)
        {

            return "Version : " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
    }
}