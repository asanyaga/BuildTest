using System.Net;
using System.Net.Http;
using System.Web.Http;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.WebApi.ApiControllers;
using Distributr.WSAPI.Lib.Services.CostCentreApplications;
using log4net;

namespace Distributr.WebApi.Common.ApiControllers
{
    public class LoginController : BaseApiController
    {
        private ILog _log = LogManager.GetLogger("LoginController");
        private ICostCentreApplicationService _costCentreApplicationService;

        public LoginController(ICostCentreApplicationService costCentreApplicationService)
        {
            _costCentreApplicationService = costCentreApplicationService;
        }

        [HttpGet]
        public HttpResponseMessage LoginGet(string userName, string password, string userType)
        {
            _log.InfoFormat("Login attempt for {0} - {1}", userName, userType);
            CostCentreLoginResponse response = _costCentreApplicationService.CostCentreLogin(userName, password, userType);
            AuditCCHit(response.CostCentreId, "Login", "Login attempt for " + userName, response.ErrorInfo);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
    }
}
