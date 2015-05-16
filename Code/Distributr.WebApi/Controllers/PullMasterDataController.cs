using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Helpers;
using Distributr.Core;
using Distributr.Core.Data.Utility.Caching;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Text;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.WSAPI.Lib.ResponseBuilders.MasterData;
using Distributr.Core.Utility;
using Distributr.WSAPI.Lib.Services.CostCentreApplications;

namespace Distributr.WSAPI.Controllers
{
    [Obsolete("No   longer used")]

    public class PullMasterDataController : BaseController
    {
        ILog _log = LogManager.GetLogger("PullMasterDataController");
        IPullMasterDataResponseBuilder _pullMasterDataResponseBuilder;
        private IMasterDataCachingInvalidator _masterDataCachingInvalidator;
        private ICostCentreApplicationService _centreApplicationService;
        public PullMasterDataController(IPullMasterDataResponseBuilder pullMasterDataResponseBuilder,
            IMasterDataCachingInvalidator masterDataCachingInvalidator,
            ICostCentreApplicationService costCentreApplicationService
            )
        {
            _centreApplicationService = costCentreApplicationService;
            _pullMasterDataResponseBuilder = pullMasterDataResponseBuilder;
            _masterDataCachingInvalidator = masterDataCachingInvalidator;
        }

        public JsonResult DoesCostCentreNeedToSync(Guid costCentreApplicationId, VirtualCityApp vcAppId)
        {
            Guid ccid = _centreApplicationService.GetCostCentreFromCostCentreApplicationId(costCentreApplicationId);
            _log.InfoFormat("DoesCostCentreNeedToSync with ccAppid", costCentreApplicationId);
            ResponseSyncRequired response = null;
            try
            {
                response = _pullMasterDataResponseBuilder.DoesCostCentreApplicationNeedToSync(costCentreApplicationId, vcAppId);
            }
            catch (Exception ex)
            {
                response = new ResponseSyncRequired { ErrorInfo = "DoesCostCentreNeedToSync failed" };
                _log.Error(ex);
            }
            AuditCCHit(ccid, "DoesCostCentreNeedToSync", "Requires sync " + response.RequiresToSync.ToString(), response.ErrorInfo);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListMasterDataEntities()
        {
            _log.Info("ListMasterDataEntities............");
            ResponseCostCentreSyncTables response = null;
            try
            {
                response = _pullMasterDataResponseBuilder.RepositoryList();
            }
            catch (Exception ex)
            {
                response = new ResponseCostCentreSyncTables { ErrorInfo = "ListMasterDataEntities failed" };
                _log.Error(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListMasterDataEntitiesToUpdate(Guid costCentreApplicationId)
        {
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetEntityMasterData(Guid costCentreApplicationId, string entityName)
        {
            Guid ccId = _centreApplicationService.GetCostCentreFromCostCentreApplicationId(costCentreApplicationId);
            _log.InfoFormat("GetEntityMasterData for ccAppId {0} and entity {1}", costCentreApplicationId, entityName);
            ContentResult result = new ContentResult();
            result.ContentType = "application/json";
            result.ContentEncoding = Encoding.UTF8;
            result.Content = null;
            try
            {
                var resp = _pullMasterDataResponseBuilder.GetTableContents(costCentreApplicationId, entityName);
                result.Content = JsonConvert.SerializeObject(resp);
                _log.InfoFormat("GetEntityMasterData - ccappid : {0} - entity name : {1} - result : {2} ", costCentreApplicationId, entityName, result.Content);
            }
            catch (Exception ex)
            {
                _log.InfoFormat("ERROR - GetEntityMasterData for ccAppId {0} and entity {1}", costCentreApplicationId, entityName);
                _log.Error(ex);
            }
            AuditCCHit(ccId, "GetEntityMasterData", "EntityName : " + entityName, "OK");
            return result;
        }
        public ActionResult GetInventory(Guid costCentreApplicationId)
        {
            _log.InfoFormat("GetInventory for ccAppId {0} and entity {1}", costCentreApplicationId, "Inventory");
            Guid ccid = _centreApplicationService.GetCostCentreFromCostCentreApplicationId(costCentreApplicationId);
            ContentResult result = new ContentResult();
            result.ContentType = "application/json";
            result.ContentEncoding = Encoding.UTF8;
            result.Content = null;
            try
            {
                var resp = _pullMasterDataResponseBuilder.GetInventory(costCentreApplicationId);
                result.Content =JsonConvert.SerializeObject(resp);
                _log.InfoFormat("GetInventory - ccappid : {0} - entity name : {1} - result : {2} ", costCentreApplicationId, "Inventory", result.Content);
            }
            catch (Exception ex)
            {
                _log.InfoFormat("ERROR - GetInventory for ccAppId {0} and entity {1}", costCentreApplicationId, "Inventory");
                _log.Error(ex);
            }
            AuditCCHit(ccid,"GetInventory", "","OK" );
            return result;
        }
        public ActionResult GetPayment(Guid costCentreApplicationId)
        {
            _log.InfoFormat("GetPayment for ccAppId {0} and entity {1}", costCentreApplicationId, "Payment");
            Guid ccid = _centreApplicationService.GetCostCentreFromCostCentreApplicationId(costCentreApplicationId);

            ContentResult result = new ContentResult();
            result.ContentType = "application/json";
            result.ContentEncoding = Encoding.UTF8;
            result.Content = null;
            try
            {
                var resp = _pullMasterDataResponseBuilder.GetPayments(costCentreApplicationId);
                result.Content = JsonConvert.SerializeObject(resp);
                _log.InfoFormat("GetPayment - ccappid : {0} - entity name : {1} - result : {2} ", costCentreApplicationId, "Payment", result.Content);
            }
            catch (Exception ex)
            {
                _log.InfoFormat("ERROR - GetPayment for ccAppId {0} and entity {1}", costCentreApplicationId, "Payment");
                _log.Error(ex);
            }
            AuditCCHit(ccid, "GetPayment", "","OK");
            return result;
        }
        public JsonResult CanConnect()
        {
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        public JsonResult InvalidateCache()
        {
            _log.Info("Invalidate cache called ++++++++++++++++++++++");
            _masterDataCachingInvalidator.InvalidateMasterDataCaching();
            return Json(new ResponseBasic() { ErrorInfo = "Success", Result = "Invalidated" }, JsonRequestBehavior.AllowGet);
        }
    }
}
