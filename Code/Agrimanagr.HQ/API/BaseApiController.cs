using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;

namespace Agrimanagr.HQ.API
{
    public class BaseApiController : ApiController
    {
        protected T Using<T>() where T : class
        {
            return DependencyResolver.Current.GetService<T>();
        }
    }
}