using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Distributr.Core.Domain.Master.EquipmentEntities;
using Distributr.Core.Repository.Master.EquipmentRepository;
using Distributr.Core.Utility;
using Distributr.HQ.Lib.Paging;
using System.Web.Mvc;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.Validation;
using Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.EquipmentViewModelBuilders;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Agrimanagr.EquipmentViewModel;
using log4net;

namespace Agrimanagr.HQ.Areas.Agrimanagr.Controllers.EquipmentControllers
{
    public class ContainerTypeController : Controller
    {
        private IContainerTypeViewModelBuilder _containerTypeViewModelBuilder;
        private IContainerTypeRepository _containerTypeRepository;
        protected static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ContainerTypeController(IContainerTypeViewModelBuilder containerTypeViewModelBuilder, IContainerTypeRepository containerTypeRepository)
        {
            _containerTypeViewModelBuilder = containerTypeViewModelBuilder;
            _containerTypeRepository = containerTypeRepository;
        }

        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListContainerTypes(Boolean? showInactive,int page = 1, int itemsperpage= 10, string srchParam="" )
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

                ViewBag.showInactive = showinactive;
                if (TempData["msg"] != null)
                {
                    ViewBag.msg = TempData["msg"].ToString();
                    TempData["msg"] = null;
                }
                ViewBag.SearchText = srchParam;

                int currentPageIndex = page - 1 < 0 ? 0 : page - 1;
                int take = itemsperpage;
                int skip = currentPageIndex * take;
                var query = new QueryStandard { Name = srchParam, ShowInactive = showinactive,Skip = skip,Take = take};


                var ls = _containerTypeViewModelBuilder.Query(query);
                var data = ls.Data;
                var count = ls.Count;

                return View(data.ToPagedList(currentPageIndex, take, count));
            }
            catch (Exception ex)
            {
                Log.Debug("Failed to list ContainerTypes " + ex.Message);
                Log.Error("Failed to list ContainerTypes" + ex);
                return View();
            }
        }

        [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult CreateContainerType()
        {
            ViewBag.CommodityList = _containerTypeViewModelBuilder.Commodities();
            ViewBag.ContainerTypeList = _containerTypeViewModelBuilder.ContainerUserTypes();

            var view = new ContainerTypeViewModel();
            view.CommodityList = new SelectList(_containerTypeViewModelBuilder.Commodities(), "Key", "Value");
            view.CommodityGradesList = _containerTypeViewModelBuilder.GradeByCommodity(Guid.Empty);

            return View(view);
        }
        [HttpPost]
        public ActionResult CreateContainerType(ContainerTypeViewModel vm)
        {
            ViewBag.CommodityList = _containerTypeViewModelBuilder.Commodities();
            ViewBag.ContainerTypeList = _containerTypeViewModelBuilder.ContainerUserTypes();

            try
            {
                vm.Id = Guid.NewGuid();
                _containerTypeViewModelBuilder.Save(vm);

                TempData["msg"] = "Container type Successfully Created";

                return RedirectToAction("ListContainerTypes");
            }
            catch (DomainValidationException ve)
            {
                ValidationSummary.DomainValidationErrors(ve, ModelState);
                Log.Debug("Failed to create Container type " + ve.Message);

                return View(vm);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                Log.Debug("Failed to create Container type " + ex.Message);

                return View(vm);
            }

        }

        [Authorize(Roles = "RoleUpdateMasterData")]
        public ActionResult EditContainerType(Guid id)
        {

            ViewBag.CommodityList = _containerTypeViewModelBuilder.Commodities();
            ViewBag.ContainerTypeList = _containerTypeViewModelBuilder.ContainerUserTypes();
           
            try
            {
                ContainerTypeViewModel containerViewModel = _containerTypeViewModelBuilder.Get(id);
                containerViewModel.CommodityGradesList = _containerTypeViewModelBuilder.GradeByCommodity(containerViewModel.SelectedCommodityId);
                return View(containerViewModel);
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }
        [HttpPost]
        public ActionResult EditContainerType(ContainerTypeViewModel vm)
        {

            ViewBag.CommodityList = _containerTypeViewModelBuilder.Commodities();
            ViewBag.ContainerTypeList = _containerTypeViewModelBuilder.ContainerUserTypes();
            ViewBag.CommodityGrade = vm.SelectedCommodityGradeId;
            try
            {
                _containerTypeViewModelBuilder.Save(vm);
                TempData["msg"] = "Container type Successfully Edited";
                return RedirectToAction("ListContainerTypes");
            }
            catch (DomainValidationException ve)
            {
                ValidationSummary.DomainValidationErrors(ve, ModelState);
                Log.Debug("Failed to edit Container type " + ve.Message);

                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                Log.Debug("Failed to edit Container type " + ex.Message);
                return View();
            }
        }
   
        public ActionResult Deactivate(Guid id)
        {
            try
            {
                _containerTypeViewModelBuilder.SetInactive(id);
                TempData["msg"] = "Container Type Successfully Deactivated";
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                Log.Debug("Failed to deactivate Container type" + dve.Message);
                Log.Error("Failed to deactivate Container type" + dve.ToString());
                TempData["msg"] = dve.Message;
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                Log.Debug("Failed to deactivate container type " + ex.Message);
              
            }
            return RedirectToAction("ListContainerTypes");
        }

        public ActionResult Activate(Guid id)
        {
            try
            {
                _containerTypeViewModelBuilder.SetActive(id);
                TempData["msg"] = "Container type Successfully activated";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                Log.Debug("Failed to activate Container type" + ex.Message);

            }

            return RedirectToAction("ListContainerTypes");
        }

        [Authorize(Roles = "RoleDeleteMasterData")]
        public ActionResult Delete(Guid id)
        {
            try
            {
                _containerTypeViewModelBuilder.SetAsDeleted(id);
                TempData["msg"] = "Container Type Successfully deleted";
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                Log.Debug("Failed to delete Container type" + dve.Message);
                Log.Error("Failed to delete Container type" + dve.ToString());
                TempData["msg"] = dve.Message;
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                Log.Debug("Failed to delete Container Type " + ex.Message);


            }

            return RedirectToAction("ListContainerTypes");
        }
        
        public ActionResult GetGrade(Guid commodityId)
        {
            var item = _containerTypeViewModelBuilder.GradeByCommodity(commodityId);

            return Json(item, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditGrade(Guid commodityId, string gradeId)
        {
            Guid grade = Guid.Parse(gradeId);
            var item = _containerTypeViewModelBuilder.CommodityGrade(commodityId, grade);

            return Json(item, JsonRequestBehavior.AllowGet);
        }


    }
}
