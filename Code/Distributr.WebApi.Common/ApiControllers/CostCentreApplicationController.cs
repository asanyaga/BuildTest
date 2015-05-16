using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.WSAPI.Lib.Services.CostCentreApplications;
using log4net;

namespace Distributr.WebApi.ApiControllers
{
    public class CostCentreApplicationController : BaseApiController
    {
        ILog _log = LogManager.GetLogger("CostCentreApplicationController");

        ICostCentreApplicationService _costCentreApplicationService;

        public CostCentreApplicationController(ICostCentreApplicationService costCentreApplicationService)
        {
            _costCentreApplicationService = costCentreApplicationService;
        }

        public HttpResponseMessage GetCreateCostCentreApplication(Guid costCentreId, string applicationDescription)
        {
            var response = new CreateCostCentreApplicationResponse();
            response.CostCentreApplicationId = Guid.Empty;
            _log.InfoFormat("CreateCostCentreApplication from CCID : {0} with applicationdescription : {1}", costCentreId, applicationDescription);

            try
            {
                response= _costCentreApplicationService.CreateCostCentreApplication(costCentreId, applicationDescription);
            }
            catch (Exception ex)
            {
                string msg = string.Format("ERROR - CreateCostCentreApplication from CCID : {0} with applicationdescription : {1}", costCentreId, applicationDescription);
                _log.Error(msg, ex);
            }
            AuditCCHit(costCentreId, "CreateCostCentreApplication", "APPID:" + response.CostCentreApplicationId.ToString(), "OK");

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
    }
}
