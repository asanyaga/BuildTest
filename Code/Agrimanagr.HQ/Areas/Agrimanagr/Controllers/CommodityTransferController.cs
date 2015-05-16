using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.Paging;
using Distributr.HQ.Lib.Validation;
using Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.CommodityTransferViewModelBuilders;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Agrimanagr.CommodityTransferViewModel;
using log4net;

namespace Agrimanagr.HQ.Areas.Agrimanagr.Controllers
{
    public class CommodityTransferController : Controller
    {
        //
        // GET: /Agrimanagr/CommodityTransfer/
        private readonly ICommodityTransferViewModelBuilder _commodityTransferViewModelBuilder;
        private readonly ICommodityTransferStoreAssignmentViewModelBuilder _assignmentViewModelBuilder;
        protected static readonly ILog Log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public CommodityTransferController(ICommodityTransferViewModelBuilder commodityTransferViewModelBuilder, ICommodityTransferStoreAssignmentViewModelBuilder assignmentViewModelBuilder)
        {
            _commodityTransferViewModelBuilder = commodityTransferViewModelBuilder;
            _assignmentViewModelBuilder = assignmentViewModelBuilder;
            ViewBag.CommodityTypeList = _assignmentViewModelBuilder.StoreList();
        }

        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult List(bool? showInactive, int? page, int? itemsperpage)
        {
            var ls = _commodityTransferViewModelBuilder.GetAll();
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            return View(ls.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage));
        }

        [Authorize(Roles = "RoleUpdateMasterData")]
        public ActionResult MoveToStore(Guid noteId)
        {
            try
            {
                var model = _assignmentViewModelBuilder.Get(noteId);
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }
        [HttpPost]
        public ActionResult MoveToStore(CommodityTransferStoreAssignmentViewModel model)
        {
            try
            {
                _commodityTransferViewModelBuilder.Approve(model.Id, model.StoreId);
                TempData["msg"] = "Commodity Successfully Edited";
                return RedirectToAction("List");
            }
            catch (DomainValidationException ve)
            {
                ValidationSummary.DomainValidationErrors(ve, ModelState);
                Log.ErrorFormat("Error in editing Commodity" + ve.Message);
                Log.InfoFormat("Error in editing Commodity" + ve.Message);
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                Log.ErrorFormat("Error in editing Commodity" + ex.Message);
                Log.InfoFormat("Error in editing Commodity" + ex.Message);
                return View();
                
            }
        }

        [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult Approve(Guid noteId, Guid storeId)
        {
            _commodityTransferViewModelBuilder.Approve(noteId, storeId);
            return RedirectToAction("List");
        }

        public ActionResult Release()
        {
            return View();
        }

    }
}
