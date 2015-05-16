using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Distributr.WSAPI.Lib.ResponseBuilders.MasterData;
using Newtonsoft.Json;

namespace Distributr.Azure.WebApi.Controllers
{
    public class TestController : ApiController
    {
        IPullMasterDataResponseBuilder _clientMasterDataManager;

        public TestController(IPullMasterDataResponseBuilder clientMasterDataManager)
        {
            _clientMasterDataManager = clientMasterDataManager;
        }

        public HttpResponseMessage GetTestCostCentre()
        {
            //var json = JsonConvert.SerializeObject(_clientMasterDataManager.GetTestCostCentre());
            return Request.CreateResponse(HttpStatusCode.OK, _clientMasterDataManager.GetTestCostCentre());
        }

        public bool CheckIsOnline()
        {
            return true;
        }

    }
}
