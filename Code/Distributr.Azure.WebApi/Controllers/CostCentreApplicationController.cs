using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using Distributr.WSAPI.Lib.Services.CostCentreApplications;
using Distributr.WebApi.ApiControllers;
using log4net;

namespace Distributr.Azure.WebApi.Controllers
{
   /* public class CostCentreApplicationController : BaseApiController
    {
        ILog _log = LogManager.GetLogger("CostCentreApplicationController");
        ICostCentreApplicationService _costCentreApplicationService;
        public CostCentreApplicationController(ICostCentreApplicationService costCentreApplicationService)
        {
            _costCentreApplicationService = costCentreApplicationService;
        }

        public HttpResponseMessage CreateCostCentreApplication(Guid costCentreId, string applicationDescription)
        {
            _log.InfoFormat("CreateCostCentreApplication from CCID : {0} with applicationdescription : {1}", costCentreId, applicationDescription);

            CreateCostCentreApplicationResponse result = null;
            try
            {
                result = _costCentreApplicationService.CreateCostCentreApplication(costCentreId, applicationDescription);
            }
            catch (Exception ex)
            {
                result = new CreateCostCentreApplicationResponse { ErrorInfo = "Failed to CreateCostCentreApplication" };
                _log.InfoFormat("ERROR - CreateCostCentreApplication from CCID : {0} with applicationdescription : {1}", costCentreId, applicationDescription);
                _log.Error(ex);
            }
            AuditCCHit(costCentreId, "CreateCostCentreApplication", "APPID:" + result.CostCentreApplicationId.ToString(), "OK");
            return Request.CreateResponse(HttpStatusCode.OK, result);
            //return Json(result, JsonRequestBehavior.AllowGet);
        }
    }*/
}
