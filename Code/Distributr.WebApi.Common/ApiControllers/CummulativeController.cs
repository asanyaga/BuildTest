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
    public class CummulativeController : ApiController
    {
        ILog _log = LogManager.GetLogger("PaymentsController");
        private IPullMasterDataResponseBuilder _pullMasterDataResponseBuilder;

        public CummulativeController(IPullMasterDataResponseBuilder pullMasterDataResponseBuilder)
        {
            _pullMasterDataResponseBuilder = pullMasterDataResponseBuilder;
        }

        [HttpGet]
        public HttpResponseMessage FamersCummulative(Guid costCentreApplicationId)
        {
            ResponseFarmerCummulativeDataInfo resp =new ResponseFarmerCummulativeDataInfo();
            try
            {
                resp = _pullMasterDataResponseBuilder.FamersCummulative(costCentreApplicationId);
            }
            catch (Exception ex)
            {

                resp.ErrorInfo = "Error"+ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.OK, resp);
        }
    }
}
