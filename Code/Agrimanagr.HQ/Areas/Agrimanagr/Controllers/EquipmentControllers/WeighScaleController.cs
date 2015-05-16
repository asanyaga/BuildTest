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
    public class WeighScaleController : Controller
    {
        private IWeighScaleViewModelBuilder _scaleViewModelBuilder;
        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public ActionResult Index()
        {
            return View();
        }

        public WeighScaleController(IWeighScaleViewModelBuilder scaleViewModelBuilder)
        {
            _scaleViewModelBuilder = scaleViewModelBuilder;
        }

        [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListWeighScales(Boolean? showInactive, int page = 1, int itemsperpage = 10, string srchParam = "")
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

                var currentPageIndex = page-1 < 0 ? 0 : page-1;
                var take = itemsperpage;
                var skip = take*currentPageIndex;

                var query = new QueryEquipment
                    {
                        Name = srchParam,
                        ShowInactive = showinactive,
                        Skip = skip,
                        Take = take,
                        EquipmentType = (int) EquipmentType.WeighingScale
                    };

                var ls = _scaleViewModelBuilder.Query(query);
                
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
                _scaleViewModelBuilder.SetInactive(id);
                TempData["msg"] = "Weigh Scale Successfully Deactivated";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to deactivate Equipment " + ex.Message);
                _log.Error("Failed to deactivate Equipment" + ex.ToString());
            }
            return RedirectToAction("ListWeighScales");
        }
        public ActionResult Activate(Guid id)
        {
            try
            {
                _scaleViewModelBuilder.SetActive(id);
                TempData["msg"] = "Weigh Scale Successfully activated";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to activate Equipment" + ex.Message);
                _log.Error("Failed to activate Equipment" + ex.ToString());

            }

            return RedirectToAction("ListWeighScales");
        }

        [Authorize(Roles = "RoleUpdateMasterData")]
        public ActionResult EditWeighScale(Guid id)
        {
            ViewBag.CostCentreList = _scaleViewModelBuilder.CostCentres();
            ViewBag.EquipmentTypeList = _scaleViewModelBuilder.EquipmentTypes();
            try
            {

                WeighScaleViewModel weighScaleViewModel = _scaleViewModelBuilder.Get(id);
                return View(weighScaleViewModel);
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }
        [HttpPost]
        public ActionResult EditWeighScale(WeighScaleViewModel vm)
        {
            ViewBag.CostCentreList = _scaleViewModelBuilder.CostCentres();
            ViewBag.EquipmentTypeList = _scaleViewModelBuilder.EquipmentTypes();
            try
            {
                _scaleViewModelBuilder.Save(vm);
                TempData["msg"] = "Weigh scale Successfully Edited";
                return RedirectToAction("ListWeighScales");
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
                _log.Debug("Failed to edit Weigh Scale " + ex.Message);
                _log.Error("Failed to edit Weigh Scale" + ex.ToString());
                return View();
            }
        }

        [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult CreateWeighScale()
        {
            ViewBag.CostCentreList = _scaleViewModelBuilder.CostCentres();
            ViewBag.EquipmentTypeList = _scaleViewModelBuilder.EquipmentTypes();


            return View(new WeighScaleViewModel());
        }
        [HttpPost]
        public ActionResult CreateWeighScale(WeighScaleViewModel vm)
        {
            ViewBag.CostCentreList = _scaleViewModelBuilder.CostCentres();
            ViewBag.EquipmentTypeList = _scaleViewModelBuilder.EquipmentTypes();

            try
            {
                vm.Id = Guid.NewGuid();
                _scaleViewModelBuilder.Save(vm);

                TempData["msg"] = "Weigh Scale Successfully Created";

                return RedirectToAction("ListWeighScales");
            }
            catch (DomainValidationException ve)
            {
                ValidationSummary.DomainValidationErrors(ve, ModelState);
                _log.Debug("Failed to create Weigh Scale " + ve.Message);
                _log.Error("Failed to create Weigh Scale" + ve.ToString());

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
        public ActionResult DeleteWeighScale(Guid id)
        {
            try
            {
                _scaleViewModelBuilder.SetAsDeleted(id);
                TempData["msg"] = "Weigh Scale Successfully deleted";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to delete Equipment" + ex.Message);
                _log.Error("Failed to delete Equipment" + ex.ToString());


            }

            return RedirectToAction("ListWeighScales");
        }

    }
}
