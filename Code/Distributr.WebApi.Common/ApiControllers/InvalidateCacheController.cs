using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Data.Utility.Caching;

namespace Distributr.WebApi.ApiControllers
{
    public class InvalidateCacheController : ApiController
    {
        private IMasterDataCachingInvalidator _masterDataCachingInvalidator;

        public InvalidateCacheController(IMasterDataCachingInvalidator masterDataCachingInvalidator)
        {
            _masterDataCachingInvalidator = masterDataCachingInvalidator;
        }

        public HttpResponseMessage GetInvalidateCache()
        {
            var response = new ResponseBool();
            _masterDataCachingInvalidator.InvalidateMasterDataCaching();
            response.Success = true;
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
    }
}
