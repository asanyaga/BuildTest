using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.WSAPI.Lib.ResponseBuilders.MasterData;
using log4net;

namespace Distributr.WebApi.ApiControllers
{
    public class PaymentsController : ApiController
    {
        ILog _log = LogManager.GetLogger("PaymentsController");
        private IPullMasterDataResponseBuilder _pullMasterDataResponseBuilder;

        public PaymentsController(IPullMasterDataResponseBuilder pullMasterDataResponseBuilder)
        {
            _pullMasterDataResponseBuilder = pullMasterDataResponseBuilder;
        }

        [HttpGet]
        public HttpResponseMessage GetPayments(Guid costCentreApplicationId)
        {
            ResponseMasterDataInfo resp = _pullMasterDataResponseBuilder.GetPayments(costCentreApplicationId);
            return Request.CreateResponse(HttpStatusCode.OK, resp);
        }
    }
}
