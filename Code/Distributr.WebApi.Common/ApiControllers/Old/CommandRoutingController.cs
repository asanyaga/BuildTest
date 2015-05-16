using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.WSAPI.Lib.ResponseBuilders.CommandRouter;
using Distributr.WSAPI.Lib.Services.CostCentreApplications;
using Newtonsoft.Json;
using log4net;

namespace Distributr.WebApi.ApiControllers
{
    public class CommandRoutingController : BaseApiController
    {
        private ILog _log = LogManager.GetLogger("CommandRoutingAPIController");
        private ICostCentreApplicationService _costCentreApplicationService;
        ICommandRouterResponseBuilder _commandRouterResponseBuilder;

        public CommandRoutingController(ICostCentreApplicationService costCentreApplicationService, ICommandRouterResponseBuilder commandRouterResponseBuilder)
        {
            _costCentreApplicationService = costCentreApplicationService;
            _commandRouterResponseBuilder = commandRouterResponseBuilder;
        }

        public HttpResponseMessage GetNextDocumentCommand(Guid costCentreApplicationId, long lastDeliveredCommandRouteItemId)
        {
            _log.InfoFormat("GetNextDocumentCommand from CCAPPId : {0} with lastDeliveredCommandRouteItemId : {1}", costCentreApplicationId, lastDeliveredCommandRouteItemId);
            Guid ccid = _costCentreApplicationService.GetCostCentreFromCostCentreApplicationId(costCentreApplicationId);

            DocumentCommandRoutingResponse response = null;
            try
            {
                if (_costCentreApplicationService.IsCostCentreActive(costCentreApplicationId))
                    response = _commandRouterResponseBuilder.GetNextDocumentCommand(costCentreApplicationId, ccid, lastDeliveredCommandRouteItemId);
                else
                    response = new DocumentCommandRoutingResponse { ErrorInfo = "Inactive CostCentre Application Id" };
            }
            catch (Exception ex)
            {
                response = new DocumentCommandRoutingResponse { ErrorInfo = "Failed to get next document command" };
                _log.InfoFormat("ERROR GetNextDocumentCommand  from CCAPPId : {0} with lastDeliveredCommandRouteItemId : {1}", costCentreApplicationId, lastDeliveredCommandRouteItemId);
                _log.Error(ex);
            }
           
            AuditCCHit(ccid, "GetNextDocumentCommand", JsonConvert.SerializeObject(response), "OK");
            
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        public HttpResponseMessage GetNextDocumentCommand(Guid costCentreApplicationId,
                                                          long lastDeliveredCommandRouteItemId, int batchSize, string batchIdsJson)
        {
            _log.InfoFormat("GetNextBatchDocumentCommand of Batch Size {2} from CCAPPId : {0} with lastDeliveredCommandRouteItemId : {1}", costCentreApplicationId, lastDeliveredCommandRouteItemId, batchSize);
            Guid ccid = _costCentreApplicationService.GetCostCentreFromCostCentreApplicationId(costCentreApplicationId);
            BatchDocumentCommandRoutingResponse response = null;
            List<long> LastBatchIds = JsonConvert.DeserializeObject<List<long>>(batchIdsJson);
            try
            {
                if (_costCentreApplicationService.IsCostCentreActive(costCentreApplicationId))
                {
                    response = _commandRouterResponseBuilder.GetNextBatcDocumentCommand(costCentreApplicationId, ccid,
                                                                                LastBatchIds, batchSize);
                }
                else
                {
                    response = new BatchDocumentCommandRoutingResponse { ErrorInfo = "Inactive CostCentre Application Id" };
                }

            }
            catch (Exception ex)
            {
                response = new BatchDocumentCommandRoutingResponse { ErrorInfo = "Failed to get next document command" };
                _log.InfoFormat("ERROR GetNextBatchDocumentCommand of Batch Size {2}  from CCAPPId : {0} with lastDeliveredCommandRouteItemId : {1}", costCentreApplicationId, lastDeliveredCommandRouteItemId, batchSize);
                _log.Error(ex);
            }
            
            AuditCCHit(ccid, "GetNextBatchDocumentCommand", JsonConvert.SerializeObject(response), "OK");

            
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
    }
}
