using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Distributr.Core.Validation;
using Distributr.HQ.Lib.Paging;
using Distributr.HQ.Lib.Validation;
using Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.CostCentreViewModelBuilders;
using Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.SourcingContainerViewModelBuilders;
using Distributr.HQ.Lib.ViewModels.Agrimanagr;
using Distributr.HQ.Lib.ViewModels.Agrimanagr.EquipmentViewModel;
using log4net;

namespace Agrimanagr.HQ.Areas.Agrimanagr.Controllers
{
    public class SourcingContainerController  : Controller
    {
        private const int defaultPageSize = 10;
        private ISourcingContainerViewModelBuilder _containerViewModelBuilder;
        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public ActionResult Index()
        {
            return View();
        }

        public SourcingContainerController(ISourcingContainerViewModelBuilder containerViewModelBuilder)
        {
            _containerViewModelBuilder = containerViewModelBuilder;
        }

        public ActionResult ListContainers(Boolean? showInactive, int? page)
        {
            try
            {
                int pageSize = 10;
                bool showinactive = false;
                if (showInactive != null)
                    showinactive = (bool)showInactive;

                ViewBag.showInactive = showinactive;
                if (TempData["msg"] != null)
                {
                    ViewBag.msg = TempData["msg"].ToString();
                    TempData["msg"] = null;
                }
                var ls = _containerViewModelBuilder.GetAll(showinactive);
                int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
                return View(ls.ToPagedList(currentPageIndex, pageSize));
            }
            catch (Exception ex)
            {
                _log.Debug("Failed to list hubs " + ex.Message);
                _log.Error("Failed to list hubs" + ex.ToString());
                return View();
            }
        }
        public ActionResult Deactivate(Guid id)
        {
            try
            {
                _containerViewModelBuilder.SetInactive(id);
                TempData["msg"] = "Container Successfully Deactivated";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to deactivate Equipment " + ex.Message);
                _log.Error("Failed to deactivate Equipment" + ex.ToString());
            }
            return RedirectToAction("ListContainers");
        }
        public ActionResult Activate(Guid id)
        {
            try
            {
                _containerViewModelBuilder.SetActive(id);
                TempData["msg"] = "Container Successfully activated";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to activate Equipment" + ex.Message);
                _log.Error("Failed to activate Equipment" + ex.ToString());

            }

            return RedirectToAction("ListContainers");
        }
        public ActionResult DetailsContainers(Guid id)
        {
            try
            {
                SourcingContainerViewModel vm = _containerViewModelBuilder.Get(id);
                return View(vm);
            }
            catch (Exception ex){
            
                ViewBag.msg = ex.Message;
                return View();
            }

        }
        public ActionResult EditContainer(Guid id)
        {
            ViewBag.CostCentreList = _containerViewModelBuilder.CostCentres();
            ViewBag.EquipmentTypeList = _containerViewModelBuilder.EquipmentTypes();
            ViewBag.CommodityList = _containerViewModelBuilder.Commodities();
            try
            {

                SourcingContainerViewModel hubViewModel = _containerViewModelBuilder.Get(id);
                return View(hubViewModel);
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }
        [HttpPost]
        public ActionResult EditContainer(SourcingContainerViewModel vm)
        {
            ViewBag.CostCentreList = _containerViewModelBuilder.CostCentres();
            ViewBag.EquipmentTypeList = _containerViewModelBuilder.EquipmentTypes();
            ViewBag.CommodityList = _containerViewModelBuilder.Commodities();
            try
            {
                _containerViewModelBuilder.Save(vm);
                TempData["msg"] = "Container Successfully Edited";
                return RedirectToAction("ListContainers");
            }
            catch (DomainValidationException ve)
            {
                ValidationSummary.DomainValidationErrors(ve, ModelState);
                _log.Debug("Failed to edit Equipment " + ve.Message);
                _log.Error("Failed to edit Equipment" + ve.ToString());

                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                _log.Debug("Failed to edit Commodity producer " + ex.Message);
                _log.Error("Failed to edit Commodity producer" + ex.ToString());
                return View();
            }
        }
        public ActionResult CreateContainer()
        {
            ViewBag.CostCentreList = _containerViewModelBuilder.CostCentres();
            ViewBag.EquipmentTypeList = _containerViewModelBuilder.EquipmentTypes();
            ViewBag.CommodityList = _containerViewModelBuilder.Commodities();


            return View(new SourcingContainerViewModel());
        }
        [HttpPost]
        public ActionResult CreateContainer(SourcingContainerViewModel vm)
        {
            ViewBag.CostCentreList = _containerViewModelBuilder.CostCentres();
            ViewBag.EquipmentTypeList = _containerViewModelBuilder.EquipmentTypes();
            ViewBag.CommodityList = _containerViewModelBuilder.Commodities();

            try
            {
                vm.Id = Guid.NewGuid();
                _containerViewModelBuilder.Save(vm);

                TempData["msg"] = "Container Successfully Created";

                return RedirectToAction("ListContainers");
            }
            catch (DomainValidationException ve)
            {
                ValidationSummary.DomainValidationErrors(ve, ModelState);
                _log.Debug("Failed to create Equipment " + ve.Message);
                _log.Error("Failed to create Equipment" + ve.ToString());

                return View(vm);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                _log.Debug("Failed to create Equipment " + ex.Message);
                _log.Error("Failed to create Equipment" + ex.ToString());

                return View(vm);
            }

        }
        public ActionResult DeleteContainer(Guid id)
        {
            try
            {
                _containerViewModelBuilder.SetAsDeleted(id);
                TempData["msg"] = "Container Successfully deleted";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to delete Equipment" + ex.Message);
                _log.Error("Failed to delete Equipment" + ex.ToString());


            }

            return RedirectToAction("ListContainers");
        }

        [HttpPost]
        public ActionResult ListContainers(bool? showInactive, int? page, string srch, string srchParam)
        {
            
            try
            {
                string command = srch;
                bool showinactive = false;
                if (showInactive != null)
                    showinactive = (bool)showInactive;
                ViewBag.showInactive = showinactive;
                if (TempData["msg"] != null)
                {
                    ViewBag.msg = TempData["msg"].ToString();
                    TempData["msg"] = null;
                }
                var ls = _containerViewModelBuilder.SearchContainers(srchParam, showinactive);
                int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
                if (command == "Search")
                {
                    return View(ls.ToPagedList(currentPageIndex, defaultPageSize));
                }
                else
                {
                    return RedirectToAction("ListContainers", new { showInactive = showInactive, srch = "Search", srchParam = ""});
                }

            }
            catch (Exception ex)
            {

                ViewBag.msg = ex.Message;
                return View();
            }

        }
        public ActionResult GetGrade(Guid commodityId){
            var item = _containerViewModelBuilder.GradeByCommodity(commodityId);
            
            return Json(item, JsonRequestBehavior.AllowGet);
        }

    }
}
