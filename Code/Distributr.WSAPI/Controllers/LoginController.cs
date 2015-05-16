using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.WSAPI.Lib.CommandResults;
using Distributr.WSAPI.Lib.Services.CostCentreApplications;
using log4net;

namespace Distributr.WSAPI.Controllers
{
    public class LoginController : BaseController
    {
        ICostCentreApplicationService _costCentreApplicationService;
        private ILog _log = LogManager.GetLogger("Login Controller");
        public LoginController(ICostCentreApplicationService costCentreApplicationService)
        {
            _costCentreApplicationService = costCentreApplicationService;
        }

        public JsonResult Login(string Username,string Password,string usertype)
        {
            _log.InfoFormat("Login attempt for {0} - {1}", Username, usertype);
            CostCentreLoginResponse response = _costCentreApplicationService.CostCentreLogin(Username, Password, usertype);
            AuditCCHit(response.CostCentreId,"Login", "Login attempt for " + Username, response.ErrorInfo );
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        
    }
}
