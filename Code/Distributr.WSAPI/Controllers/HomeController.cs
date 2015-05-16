using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.WSAPI.Lib.ResponseBuilders.MasterData;
using log4net;

namespace Distributr.WSAPI.Controllers
{
    public class HomeController : Controller
    {
        IPullMasterDataResponseBuilder _clientMasterDataManager;
        public HomeController(IPullMasterDataResponseBuilder clientStateManager)
        {
            _clientMasterDataManager = clientStateManager;
        }
        public ActionResult Index()
        {
            _clientMasterDataManager.RepositoryList();
            return View();
        }
        public ActionResult LogError(string message)
        {
            ILog logger = LogManager.GetLogger("ClientLogging");
            logger.Error("Client Error" + message);
            return Content("OK");
        }

    }
}
