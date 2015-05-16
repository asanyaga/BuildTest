using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Web.Mvc;
using log4net;

namespace Distributr.Azure.WebApi.Code
{
    public class ExceptionHandlingAttribute : HandleErrorAttribute
    {
        private ILog _logger = null;
        public ExceptionHandlingAttribute()
        {
            _logger = LogManager.GetLogger("GLOBAL ERROR");
        }

        public override void OnException(ExceptionContext filterContext)
        {
            _logger.Error("On Exception ", filterContext.Exception);

            //Log Critical errors
            Debug.WriteLine(filterContext.Exception);

            //throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
            //{
            //    Content = new StringContent("An error occurred, please try again or contact the administrator."),
            //    ReasonPhrase = "Critical Exception"
            //});
            base.OnException(filterContext);
        }

       
    }
}