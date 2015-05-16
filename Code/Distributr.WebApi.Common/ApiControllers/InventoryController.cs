using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.WSAPI.Lib.ResponseBuilders.MasterData;
using Distributr.WSAPI.Lib.Services.Sync;
using log4net;

namespace Distributr.WebApi.ApiControllers
{
    public class InventoryController : BaseApiController
    {
        ILog _log = LogManager.GetLogger("InventoryController");
        private IPullMasterDataResponseBuilder _pullMasterDataResponseBuilder;
        private ISyncInventoryService _syncInventoryService;

        public InventoryController(IPullMasterDataResponseBuilder pullMasterDataResponseBuilder, ISyncInventoryService syncInventoryService)
        {
            _pullMasterDataResponseBuilder = pullMasterDataResponseBuilder;
            _syncInventoryService = syncInventoryService;
        }

        [HttpGet]
        public HttpResponseMessage GetInventory(Guid costCentreApplicationId)
        {
            ResponseMasterDataInfo  resp = _pullMasterDataResponseBuilder.GetInventory(costCentreApplicationId);
            AuditCCHit(costCentreApplicationId, "GetInventory", "", "OK");
            return Request.CreateResponse<ResponseMasterDataInfo>(HttpStatusCode.OK, resp);
        }
       
    }
}
