using System;
using System.Reflection;
using System.Web.Mvc;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.Paging;
using Distributr.HQ.Lib.Validation;
using Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.CostCentreViewModelBuilders;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Agrimanagr.CostCentreViewModel;
using log4net;

namespace Agrimanagr.HQ.Areas.Agrimanagr.Controllers.CentreControllers
{
    public class StoreController : Controller
    {
        //
        // GET: /Agrimanagr/Store/
        private IStoreViewModelBuilder _storeViewModelBuilder;
        private const int defaultPageSize = 10;
        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public StoreController(IStoreViewModelBuilder storeViewModelBuilder)
        {
            _storeViewModelBuilder = storeViewModelBuilder;
        }

        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListStores(bool showinactive = false, int page = 1, int itemsperpage = 10, string srchParam = "")
        {
            try
            {

                /* bool showinactive = false;*/
                if (showinactive != null)
                    showinactive = (bool)showinactive;
                ViewBag.showInactive = showinactive;

                if (TempData["msg"] != null)
                {
                    ViewBag.msg = TempData["msg"].ToString();
                    TempData["msg"] = null;
                }
                ViewBag.srchParam = srchParam;
                int currentPageIndex = page - 1 < 0 ? 0 : page - 1;
                int take = itemsperpage;
                int skip = currentPageIndex * take;
                var query = new QueryStandard { Skip = skip, Take = take, Name = srchParam, ShowInactive = showinactive };

                var ls = _storeViewModelBuilder.Query(query);



                var data = ls.Data;
                var count = ls.Count;

                return View(data.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage, count));
            }
            catch (Exception ex)
            {
                _log.Debug("Failed to list stores " + ex.Message);
                _log.Error("Failed to list stores" + ex.ToString());
                return View();
            }
        }

        [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult CreateStore()
        {
            ViewBag.ParentCostCentreList = _storeViewModelBuilder.ParentCostCentre();
            ViewBag.CostCentreTypeList = _storeViewModelBuilder.CostCentreTypes();


            return View(new StoreViewModel());
        }
        [HttpPost]
        public ActionResult CreateStore(StoreViewModel vm)
        {
            ViewBag.ParentCostCentreList = _storeViewModelBuilder.ParentCostCentre();
            ViewBag.CostCentreTypeList = _storeViewModelBuilder.CostCentreTypes();

            try
            {
                vm.Id = Guid.NewGuid();
                _storeViewModelBuilder.Save(vm);

                TempData["msg"] = "Store Successfully Created";

                return RedirectToAction("ListStores");
            }
            catch (DomainValidationException ve)
            {
                ValidationSummary.DomainValidationErrors(ve, ModelState);
                _log.Debug("Failed to create stores " + ve.Message);
                _log.Error("Failed to create stores" + ve.ToString());

                return View(vm);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                _log.Debug("Failed to create stores " + ex.Message);
                _log.Error("Failed to create stores" + ex.ToString());

                return View(vm);
            }

        }

        [Authorize(Roles = "RoleUpdateMasterData")]
        public ActionResult EditStore(Guid id)
        {
            ViewBag.ParentCostCentreList = _storeViewModelBuilder.ParentCostCentre();
            ViewBag.CostCentreTypeList = _storeViewModelBuilder.CostCentreTypes();
            try
            {

                StoreViewModel storeViewModel = _storeViewModelBuilder.Get(id);
                return View(storeViewModel);
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }
        [HttpPost]
        public ActionResult EditStore(StoreViewModel vm)
        {
            ViewBag.ParentCostCentreList = _storeViewModelBuilder.ParentCostCentre();
            ViewBag.CostCentreTypeList = _storeViewModelBuilder.CostCentreTypes();
            try
            {
                _storeViewModelBuilder.Save(vm);
                TempData["msg"] = "Store Successfully Edited";
                return RedirectToAction("ListStores");
            }
            catch (DomainValidationException ve)
            {
                ValidationSummary.DomainValidationErrors(ve, ModelState);
                _log.Debug("Failed to edit Store " + ve.Message);
                _log.Error("Failed to edit Store" + ve.ToString());

                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                _log.Debug("Failed to edit Store " + ex.Message);
                _log.Error("Failed to edit Store" + ex.ToString());
                return View();
            }
        }
        public ActionResult Deactivate(Guid id)
        {
            try
            {
                _storeViewModelBuilder.SetInactive(id);
                TempData["msg"] = "Store Successfully Deactivated";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to deactivate store " + ex.Message);
                _log.Error("Failed to deactivate store" + ex.ToString());
            }
            return RedirectToAction("ListStores");
        }
        public ActionResult Activate(Guid id)
        {
            try
            {
                _storeViewModelBuilder.SetActive(id);
                TempData["msg"] = "Store Successfully activated";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to activate store" + ex.Message);
                _log.Error("Failed to activate store" + ex.ToString());

            }

            return RedirectToAction("ListStores");
        }

        [Authorize(Roles = "RoleDeleteMasterData")]
        public ActionResult DeleteStore(Guid id)
        {
            try
            {
                _storeViewModelBuilder.SetAsDeleted(id);
                TempData["msg"] = "Store Successfully deleted";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to delete Store" + ex.Message);
                _log.Error("Failed to delete Store" + ex.ToString());


            }

            return RedirectToAction("ListStores");
        }
    }
}
