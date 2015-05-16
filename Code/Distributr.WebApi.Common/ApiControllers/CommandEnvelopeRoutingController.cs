using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.WSAPI.Lib.ResponseBuilders.CommandRouter;
using Distributr.WSAPI.Lib.Services.CostCentreApplications;
using Newtonsoft.Json;
using log4net;

namespace Distributr.WebApi.ApiControllers
{
    [RoutePrefix("api/commandenveloperouting")]
    public class CommandEnvelopeRoutingController : BaseApiController
    {
        private ILog _log = LogManager.GetLogger("CommandEnvelopeRoutingController");
        private ICostCentreApplicationService _costCentreApplicationService;
        ICommandEnvelopeRouterResponseBuilder _commandEnvelopeRouterResponseBuilder;

        public CommandEnvelopeRoutingController(ICostCentreApplicationService costCentreApplicationService, ICommandEnvelopeRouterResponseBuilder commandEnvelopeRouterResponseBuilder)
        {
            _costCentreApplicationService = costCentreApplicationService;
            _commandEnvelopeRouterResponseBuilder = commandEnvelopeRouterResponseBuilder;
         
        }


        [System.Web.Http.HttpPost]
         [Route("GetNextEnvelopes")]
        public HttpResponseMessage GetNextDocumentCommandEnvelopes(EnvelopeRoutingRequest request)
        {
            string pullType = "Inventory";
            _log.InfoFormat("GetNextDocumentCommandEnvelopes  from CCAPPId : {0} with batchSize : {1}", request.CostCentreApplicationId, request.BatchSize);
            Guid ccid = _costCentreApplicationService.GetCostCentreFromCostCentreApplicationId(request.CostCentreApplicationId);
            BatchDocumentCommandEnvelopeRoutingResponse response = null;
            List<Guid> lastBatchEnvelopesIds = request.DeliveredEnvelopeIds;
            try
            {
                if (_costCentreApplicationService.IsCostCentreActive(request.CostCentreApplicationId))
                {
                    response = _commandEnvelopeRouterResponseBuilder.GetNextInventoryCommandEnvelopes(request.CostCentreApplicationId, ccid,
                                                                                lastBatchEnvelopesIds, request.BatchSize);
                    if (response.Envelopes.Count == 0)
                    {
                        pullType = "Other";
                        response=_commandEnvelopeRouterResponseBuilder.GetNextCommandEnvelopes(request.CostCentreApplicationId, ccid,
                                                                                lastBatchEnvelopesIds, request.BatchSize);
                    }
                   
                }
                else
                {
                    response = new BatchDocumentCommandEnvelopeRoutingResponse { ErrorInfo = "Inactive CostCentre Application Id" };
                }

            }
            catch (Exception ex)
            {
                response = new BatchDocumentCommandEnvelopeRoutingResponse { ErrorInfo = "Failed to get next document command" };
                _log.InfoFormat("ERROR GetNextDocumentCommandEnvelopes   from CCAPPId : {0} with batchsize : {1}", request.CostCentreApplicationId, request.BatchSize);
                _log.Error(ex);
            }

            AuditCCHit(ccid, "GetNextDocumentCommandEnvelopes", JsonConvert.SerializeObject(response.Envelopes.Select(s=>new {s.DocumentType,s.Envelope.Id})), pullType + " OK " + JsonConvert.SerializeObject(request));

            
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
    }
}
