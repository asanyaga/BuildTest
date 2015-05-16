using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.WSAPI.Lib.ResponseBuilders.MasterData;
using Newtonsoft.Json;
using log4net;

namespace Distributr.WSAPI.Controllers
{
    [Obsolete("No   longer used")]
    
    public class HomeController : Controller
    {
        IPullMasterDataResponseBuilder _clientMasterDataManager;
        public HomeController(IPullMasterDataResponseBuilder clientStateManager)
        {
            _clientMasterDataManager = clientStateManager;
        }
        public ActionResult Index()
        {
            string ssdsd=null;
            //string sdsd = "{\"DateOrderRequired\":\"2013-10-21T15:14:46\",\"IssuedOnBehalfOfCostCentreId\":\"4fd0b052-f221-483c-b39d-4800d6218123\",\"Note\":\"\",\"shipToAddress\":\"[HAPPY COW  AGROSHOP][][]\",\"ParentId\":\"5cdd0b88-fdc1-4b0e-9b41-3a6b316c7964\",\"OrderTypeId\":1,\"SaleDiscount\":0.0,\"OrderStatusId\":1,\"DocIssuerUserId\":\"d5b035be-f1a7-4caf-b5d9-6ec005594909\",\"DocumentDateIssued\":\"2013-10-21T15:14:49\",\"DocumentIssuerCostCentreId\":\"300b9846-98f5-4260-9dd8-6d7773c75db3\",\"DocumentRecipientCostCentreId\":\"bf9b69f7-0778-4f46-b9cb-37973b1b9f17\",\"DocumentReference\":\"Sale_bomet_CKO3388_20131021_151409_00260\",\"VersionNumber\":\"M-2.1.7\",\"CommandCreatedDateTime\":\"2013-10-21T15:14:49\",\"CommandGeneratedByCostCentreApplicationId\":\"1833cfc3-51f9-40e2-8a6f-186ba3ae9023\",\"CommandGeneratedByCostCentreId\":\"300b9846-98f5-4260-9dd8-6d7773c75db3\",\"CommandGeneratedByUserId\":\"d5b035be-f1a7-4caf-b5d9-6ec005594909\",\"CommandId\":\"868c3338-5c32-46a8-9144-dfed14d06fab\",\"SendDateTime\":\"2013-10-21T15:23:34\",\"CommandTypeRef\":\"CreateMainOrder\",\"PDCommandId\":\"5cdd0b88-fdc1-4b0e-9b41-3a6b316c7964\",\"DocumentId\":\"5cdd0b88-fdc1-4b0e-9b41-3a6b316c7964\",\"DiscountType\":0,\"Latitude\":-0.6158868,\"Longitude\":35.2027767,\"LineItemType\":0,\"CostCentreApplicationCommandSequenceId\":0,\"CommandSequence\":2560}";
            //DocumentCommand command = JsonConvert.DeserializeObject<DocumentCommand>(ssdsd.ToString());
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
