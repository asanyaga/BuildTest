using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Agrimanagr.HQ.Models
{
    public static class AgriWebHelper
    {
        public static string GetVersion(this HtmlHelper htmlHelper)
        {

            return "Version : " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
      
       
    }
    public static class DistributorWebHelper
    {
      
        public static int GetItemPerPage()
        {
            string cachedKey;
            cachedKey = string.Format("itemperpage_{0}", HttpContext.Current.User.Identity.Name);
            int cp = 10;
            try
            {
                int.TryParse(HttpContext.Current.Cache[cachedKey].ToString(), out cp);
            }
            catch { }
            return cp;
        }
        public static void SetItemPerPage(int pagesize)
        {
            string cachedKey = string.Format("itemperpage_{0}", HttpContext.Current.User.Identity.Name);
            HttpContext.Current.Cache.Insert(key: cachedKey, value: pagesize);
        }
    }
}