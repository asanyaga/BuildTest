using System;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Distributr.Core.Domain.Master.EquipmentEntities;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.Paging;
using Distributr.HQ.Lib.Validation;
using Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.EquipmentViewModelBuilders;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Agrimanagr;
using Distributr.HQ.Lib.ViewModels.Agrimanagr.EquipmentViewModel;
using log4net;

namespace Agrimanagr.HQ.Areas.Agrimanagr.Controllers.EquipmentControllers
{
    public class SourcingContainerController : Controller
    {
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

        [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListContainers(Boolean? showInactive, int page = 1, int itemsperpage= 10, string srchParam="")
        {
            try
            {
                if (itemsperpage != null)
                {
                    ViewModelBase.ItemsPerPage = itemsperpage;
                }
                bool showinactive = false;
                if (showInactive != null)
                    showinactive = (bool)showInactive;
                ViewBag.srchParam = srchParam;
                ViewBag.showInactive = showinactive;
                if (TempData["msg"] != null)
                {
                    ViewBag.msg = TempData["msg"].ToString();
                    TempData["msg"] = null;
                }

                int currentPageIndex = page - 1 < 0 ? 0 : page - 1;
                int take = itemsperpage;
                int skip = currentPageIndex * take;
                var query = new QueryEquipment{ Name = srchParam, ShowInactive = showinactive, Skip = skip, Take = take,EquipmentType = (int)EquipmentType.Container};
                
                var ls = _containerViewModelBuilder.Query(query);
                var data = ls.Data;

                var total = ls.Count;

                return View(data.ToPagedList(currentPageIndex, take, total));
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

        [Authorize(Roles = "RoleUpdateMasterData")]
        public ActionResult EditContainer(Guid id)
        {
            ViewBag.CostCentreList = _containerViewModelBuilder.CostCentres();
           /* ViewBag.EquipmentTypeList = _containerViewModelBuilder.EquipmentTypes();
            ViewBag.CommodityList = _containerViewModelBuilder.Commodities();*/
            ViewBag.ContainerTypeList = _containerViewModelBuilder.ContainerTypes();
            //ViewBag.CommodityGrade = _containerViewModelBuilder.CommodityGrade(id);
            try
            {

                SourcingContainerViewModel containerViewModel = _containerViewModelBuilder.Get(id);
                /*ViewBag.CommodityGrade = containerViewModel.CommodityGrade;*/
                return View(containerViewModel);
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
            /*ViewBag.EquipmentTypeList = _containerViewModelBuilder.EquipmentTypes();
            ViewBag.CommodityList = _containerViewModelBuilder.Commodities();*/
            ViewBag.ContainerTypeList = _containerViewModelBuilder.ContainerTypes();
           /* ViewBag.CommodityGrade = vm.CommodityGrade;*///_containerViewModelBuilder.CommodityGrade(vm.CommodityGrade);
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

        [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult CreateContainer()
        {
            ViewBag.CostCentreList = _containerViewModelBuilder.CostCentres();
           /* ViewBag.EquipmentTypeList = _containerViewModelBuilder.EquipmentTypes();*/
            //ViewBag.CommodityList = _containerViewModelBuilder.Commodities();
            ViewBag.ContainerTypeList = _containerViewModelBuilder.ContainerTypes();

            var view = new SourcingContainerViewModel();
           /* view.CommodityList = new SelectList (_containerViewModelBuilder.Commodities(), "Key", "Value");*/


            return View(view);
        }
        [HttpPost]
        public ActionResult CreateContainer(SourcingContainerViewModel vm)
        {
            ViewBag.CostCentreList = _containerViewModelBuilder.CostCentres();
            /*ViewBag.EquipmentTypeList = _containerViewModelBuilder.EquipmentTypes();
            ViewBag.CommodityList = _containerViewModelBuilder.Commodities();*/
            ViewBag.ContainerTypeList = _containerViewModelBuilder.ContainerTypes();

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

        [Authorize(Roles = "RoleDeleteMasterData")]
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


       /* public ActionResult GetGrade(Guid commodityId)
        {
            var item = _containerViewModelBuilder.GradeByCommodity(commodityId);
            
            return Json(item, JsonRequestBehavior.AllowGet);
        }*/

        /*public ActionResult EditGrade(Guid commodityId, string gradeId)
        {
            Guid grade = Guid.Parse(gradeId);
            var item = _containerViewModelBuilder.CommodityGrade(commodityId, grade);
            
            return Json(item, JsonRequestBehavior.AllowGet);
        }*/
    }
}
