using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.WSAPI.Lib.Services.CostCentreApplications;
using log4net;

namespace Distributr.WSAPI.Controllers
{
    [Obsolete("No   longer used")]

    public class CostCentreApplicationController : BaseController
    {
        ILog _log = LogManager.GetLogger("CostCentreApplicationController");

        ICostCentreApplicationService _costCentreApplicationService;
        public CostCentreApplicationController(ICostCentreApplicationService costCentreApplicationService)
        {
            _costCentreApplicationService = costCentreApplicationService;
        }

        public JsonResult CreateCostCentreApplication(Guid costCentreId, string applicationDescription)
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
            AuditCCHit(costCentreId, "CreateCostCentreApplication", "APPID:" + result.CostCentreApplicationId.ToString(),"OK");
            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}
