using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Distributr.Core.Domain.Master.EquipmentEntities;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.Paging;
using Distributr.HQ.Lib.Validation;
using Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.EquipmentViewModelBuilders;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Agrimanagr.EquipmentViewModel;
using log4net;

namespace Agrimanagr.HQ.Areas.Agrimanagr.Controllers.EquipmentControllers
{
    public class VehicleController : Controller
    {
        private readonly IVehicleViewModelBuilder _vehicleViewModelBuilder;
        protected static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public VehicleController(IVehicleViewModelBuilder vehicleViewModelBuilder)
        {
            _vehicleViewModelBuilder = vehicleViewModelBuilder;
        }

        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListVehicles(Boolean? showInactive,bool showinactive = false, int page=1, int itemsperpage=10, string srchParam="")
        {
            try
            {
                if (itemsperpage != null)
                {
                    ViewModelBase.ItemsPerPage = itemsperpage;
                }
                
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
                var query = new QueryEquipment { Name = srchParam, ShowInactive = showinactive, Skip = skip, Take = take, EquipmentType = (int)EquipmentType.Vehicle };
                
                var ls = _vehicleViewModelBuilder.Query(query);
                var data = ls.Data;
                var total = ls.Count;

                return View(data.ToPagedList(currentPageIndex, take, total));
            }
            catch (Exception ex)
            {
                Log.Debug("Failed to list ContainerTypes " + ex.Message);
                Log.Error("Failed to list ContainerTypes" + ex);
                return View();
            }
        }

        [Authorize(Roles = "RoleAddMasterData")]
        [HttpGet]
        public ActionResult CreateVehicle(Guid? id)
        {
            var viewmodel = new VehicleViewModel();
            if (id.HasValue)
            {
                viewmodel = _vehicleViewModelBuilder.Get((Guid) id.Value);
                ViewBag.Action = "Edit";
            }

            ViewBag.HubsList = _vehicleViewModelBuilder.Hubs();
            return View(viewmodel);
        }

        [HttpPost]
        public ActionResult CreateVehicle(VehicleViewModel vm)
        {
            try
            {
                 _vehicleViewModelBuilder.Save(vm);

                TempData["msg"] = "vehicle Successfully Saved";

                return RedirectToAction("ListVehicles");
            }
            catch (DomainValidationException ve)
            {
                ValidationSummary.DomainValidationErrors(ve, ModelState);
                Log.Debug("Failed to create Container type " + ve.Message);
                ViewBag.HubsList = _vehicleViewModelBuilder.Hubs();
                return View(vm);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                Log.Debug("Failed to create Container type " + ex.Message);
                ViewBag.HubsList = _vehicleViewModelBuilder.Hubs();
                return View(vm);
            }

        }

        [Authorize(Roles = "RoleUpdateMasterData")]
        public ActionResult EditVehicle(Guid id)
        {
            return RedirectToAction("CreateVehicle", new {Id = id});
        }

        public ActionResult Deactivate(Guid id)
        {
            try
            {
                _vehicleViewModelBuilder.SetInactive(id);
                TempData["msg"] = "Vehicle Successfully Deactivated";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                Log.Debug("Failed to deactivate vehicle " + ex.Message);

            }
            return RedirectToAction("ListVehicles");
        }

        public ActionResult Activate(Guid id)
        {
            try
            {
                _vehicleViewModelBuilder.SetActive(id);
                TempData["msg"] = "Vehicle Successfully activated";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                Log.Debug("Failed to activate vehicle" + ex.Message);

            }

            return RedirectToAction("ListVehicles");
        }

        [Authorize(Roles = "RoleDeleteMasterData")]
        public ActionResult Delete(Guid id)
        {
            try
            {
                _vehicleViewModelBuilder.SetAsDeleted(id);
                TempData["msg"] = "Vehicle Successfully deleted";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                Log.Debug("Failed to delete Vehicle " + ex.Message);


            }

            return RedirectToAction("ListVehicles");
        }
    }
}
