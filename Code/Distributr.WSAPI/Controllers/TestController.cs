using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Distributr.WSAPI.Lib.ResponseBuilders.MasterData;

namespace Distributr.WSAPI.Controllers
{
    public class TestController: Controller
    {
        IPullMasterDataResponseBuilder _clientMasterDataManager;
        public TestController(IPullMasterDataResponseBuilder clientStateManager)
        {
            _clientMasterDataManager = clientStateManager;
        }

        public JsonResult GetTestCostCentre()
        {
            return Json(_clientMasterDataManager.GetTestCostCentre(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckIsOnline()
        {
            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}
