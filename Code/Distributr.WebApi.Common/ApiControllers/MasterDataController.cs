using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Distributr.Core;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Repository.Master;
using Distributr.Core.Utility;
using Distributr.WSAPI.Lib.ResponseBuilders.MasterData;
using log4net;

namespace Distributr.WebApi.ApiControllers
{
    public class MasterDataController : BaseApiController
    {
        private IClientMasterDataTrackerRepository _clientMasterDataTrackerRepository;
        private IPullMasterDataResponseBuilder _masterDataResponseBuilder;
        private ILog _logger = LogManager.GetLogger("MasterDataController");
        public MasterDataController(IClientMasterDataTrackerRepository clientMasterDataTrackerRepository, IPullMasterDataResponseBuilder masterDataResponseBuilder)
        {
            _clientMasterDataTrackerRepository = clientMasterDataTrackerRepository;
            _masterDataResponseBuilder = masterDataResponseBuilder;
        }

        [HttpGet]
        public HttpResponseMessage DoesCostCentreNeedToSync(Guid costCentreApplicationId, VirtualCityApp vcAppId)
        {
            var response = new ResponseSyncRequired();
            _logger.InfoFormat("DoesCostCentreNeedToSync {0}", costCentreApplicationId);
            response.RequiresToSync = _clientMasterDataTrackerRepository.DoesCostCentreApplicationNeedToSync(costCentreApplicationId, vcAppId);
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpGet]
        public HttpResponseMessage ListMasterDataEntities()
        {
            var response = new ResponseCostCentreSyncTables();
            _logger.Info("ListMasterDataEntities");
            response.TablesToSync = _clientMasterDataTrackerRepository.RepositoryList();
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpGet]
        public HttpResponseMessage GetEntityMasterData(Guid costCentreApplicationId, string entityName)
        {
            HttpStatusCode returnCode = HttpStatusCode.OK;
            ResponseMasterDataInfo response = new ResponseMasterDataInfo();
            try
            {
                _logger.InfoFormat("Get Entity Master Data  {0} -- {1} ", entityName, costCentreApplicationId);
                 response = _masterDataResponseBuilder.GetTableContents(costCentreApplicationId, entityName);
               
            }
            catch(Exception ex)
            {
                response.ErrorInfo = ex.Message;
                returnCode=HttpStatusCode.ServiceUnavailable;
            }
            return Request.CreateResponse(returnCode, response);
        }

    }
}
