using System.Web;
using System.Web.Http.Filters;
using System.Web.Mvc;
using Distributr.Azure.WebApi.Code;

namespace Distributr.Azure.WebApi
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //filters.Add(new HandleErrorAttribute());
            filters.Add(new ExceptionHandlingAttribute());
        }
    }


}