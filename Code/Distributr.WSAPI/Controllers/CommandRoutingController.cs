using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.Utility;
using Distributr.WSAPI.Lib.CommandResults;
using Distributr.WSAPI.Lib.Services.CostCentreApplications;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Text;
using Distributr.WSAPI.Lib.ResponseBuilders.CommandRouter;

namespace Distributr.WSAPI.Controllers
{
    public class CommandRoutingController : BaseController
    {
        ILog _log = LogManager.GetLogger("CommandRoutingController");
        ICommandRouterResponseBuilder _commandRouterResponseBuilder;
        private ICostCentreApplicationService _costCentreApplicationService;
        public CommandRoutingController(ICostCentreApplicationService costCentreApplicationService,ICommandRouterResponseBuilder commandRouterResponseBuilder)
        {
            _commandRouterResponseBuilder = commandRouterResponseBuilder;
            _costCentreApplicationService = costCentreApplicationService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="costCentreApplicationId"></param>
        /// <param name="lastDeliveredCommandRouteItemId">-1 if never requested before</param>
        /// <returns></returns>
        public ActionResult GetNextDocumentCommand(Guid costCentreApplicationId, long lastDeliveredCommandRouteItemId)
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
            catch (Exception ex )
            {
                response = new DocumentCommandRoutingResponse { ErrorInfo = "Failed to get next document command" };
                _log.InfoFormat("ERROR GetNextDocumentCommand  from CCAPPId : {0} with lastDeliveredCommandRouteItemId : {1}", costCentreApplicationId, lastDeliveredCommandRouteItemId);
                _log.Error(ex);
            }
            ContentResult result = new ContentResult
                                       {
                                           Content = JsonConvert.SerializeObject(response),
                                           ContentType = "application/json",
                                           ContentEncoding = Encoding.UTF8
                                       };
            AuditCCHit(ccid, "GetNextDocumentCommand", result.Content, "OK" );
            return result;
        }
        [HttpPost]
        [JsonBatchFilter]
        public ActionResult GetNextBatchDocumentCommand(Guid costCentreApplicationId, long lastDeliveredCommandRouteItemId, int batchSize, string  batchIdsJson)
        {
            _log.InfoFormat("GetNextBatchDocumentCommand of Batch Size {2} from CCAPPId : {0} with lastDeliveredCommandRouteItemId : {1}", costCentreApplicationId, lastDeliveredCommandRouteItemId, batchSize);
            Guid ccid = _costCentreApplicationService.GetCostCentreFromCostCentreApplicationId(costCentreApplicationId);
            BatchDocumentCommandRoutingResponse response = null;
            List<long> LastBatchIds = JsonConvert.DeserializeObject<List<long>> (batchIdsJson);
            try
            {
                if (_costCentreApplicationService.IsCostCentreActive(costCentreApplicationId))
                {
                    response = _commandRouterResponseBuilder.GetNextBatcDocumentCommand(costCentreApplicationId,ccid,
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
                _log.InfoFormat("ERROR GetNextBatchDocumentCommand of Batch Size {2}  from CCAPPId : {0} with lastDeliveredCommandRouteItemId : {1}", costCentreApplicationId, lastDeliveredCommandRouteItemId,batchSize);
                _log.Error(ex);
            }
            ContentResult result = new ContentResult
                                       {
                                           Content = JsonConvert.SerializeObject(response),
                                           ContentType = "application/json",
                                           ContentEncoding = Encoding.UTF8
                                       };
            AuditCCHit(ccid, "GetNextBatchDocumentCommand", result.Content, "OK");

            return result;
        }

    }
    public class JsonBatchFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var incomingData = new StreamReader(filterContext.HttpContext.Request.InputStream).ReadToEnd();
            filterContext.ActionParameters["batchIdsJson"] = incomingData.Replace("batchIdsJson=", "");
            base.OnActionExecuting(filterContext);
        }
    }
}
