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
using Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.PrinterViewModelBuilders;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Agrimanagr;
using Distributr.HQ.Lib.ViewModels.Agrimanagr.EquipmentViewModel;
using log4net;

namespace Agrimanagr.HQ.Areas.Agrimanagr.Controllers
{
    public class PrinterController : Controller
    {
        private IPrinterViewModelBuilder _printerViewModelBuilder;
        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public PrinterController(IPrinterViewModelBuilder printerViewModelBuilder)
        {
            _printerViewModelBuilder = printerViewModelBuilder;
        }

        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListPrinters(Boolean? showInactive, int page=1, int itemsperpage=10, string srchParam="")
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
                //var ls = _printerViewModelBuilder.GetAll(showinactive);

                int take = itemsperpage;
                int currentPageIndex = page - 1 < 0 ? 0 : page - 1;
                int skip = take*currentPageIndex;
                
                var query = new QueryEquipment {Name = srchParam, ShowInactive = showinactive, Skip = skip, Take = take,EquipmentType =(int)EquipmentType.Printer};

             /*   var ls = _printerViewModelBuilder.Query(query);
                var data = ls.Data.OfType<Printer>().ToList();
                var results = _printerViewModelBuilder.QueryList(data);
                var total = ls.Count;
                //int currentPageIndex = page.HasValue ? page.Value - 1 : 0;*/

                var ls = _printerViewModelBuilder.Query(query);
                var total = ls.Count;
                var data = ls.Data;
                return View(data.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage, total));
            }
            catch (Exception ex)
            {
                _log.Debug("Failed to list printers " + ex.Message);
                _log.Error("Failed to list printers" + ex.ToString());
                return View();
            }
        }

        [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult CreatePrinter()
        {
            ViewBag.CostCentreList = _printerViewModelBuilder.CostCentres();
            ViewBag.EquipmentTypeList = _printerViewModelBuilder.EquipmentTypes();


            return View(new PrinterViewModel());
        }
        [HttpPost]
        public ActionResult CreatePrinter(PrinterViewModel vm)
        {
            ViewBag.CostCentreList = _printerViewModelBuilder.CostCentres();
            ViewBag.EquipmentTypeList = _printerViewModelBuilder.EquipmentTypes();

            try
            {
                vm.Id = Guid.NewGuid();
                _printerViewModelBuilder.Save(vm);

                TempData["msg"] = "Printer Successfully Created";

                return RedirectToAction("ListPrinters");
            }
            catch (DomainValidationException ve)
            {
                ValidationSummary.DomainValidationErrors(ve, ModelState);
                _log.Debug("Failed to create Printer " + ve.Message);
                _log.Error("Failed to create Printer" + ve.ToString());

                return View(vm);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                _log.Debug("Failed to create Printer " + ex.Message);
                _log.Error("Failed to create Printer" + ex.ToString());

                return View(vm);
            }

        }

        [Authorize(Roles = "RoleUpdateMasterData")]
        public ActionResult EditPrinter(Guid id)
        {
            ViewBag.CostCentreList = _printerViewModelBuilder.CostCentres();
            ViewBag.EquipmentTypeList = _printerViewModelBuilder.EquipmentTypes();
       
            try
            {
                PrinterViewModel printerViewModel = _printerViewModelBuilder.Get(id);
                return View(printerViewModel);
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }
        [HttpPost]
        public ActionResult EditPrinter(PrinterViewModel vm)
        {
            ViewBag.CostCentreList = _printerViewModelBuilder.CostCentres();
            ViewBag.EquipmentTypeList = _printerViewModelBuilder.EquipmentTypes();
          
            try
            {
                _printerViewModelBuilder.Save(vm);
                TempData["msg"] = "Printer Successfully Edited";
                return RedirectToAction("ListPrinters");
            }
            catch (DomainValidationException ve)
            {
                ValidationSummary.DomainValidationErrors(ve, ModelState);
                _log.Debug("Failed to edit Printer " + ve.Message);
                _log.Error("Failed to edit Printer" + ve.ToString());

                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                _log.Debug("Failed to edit Printer " + ex.Message);
                _log.Error("Failed to edit Printer" + ex.ToString());
                return View();
            }
        }

        public ActionResult Deactivate(Guid id)
        {
            try
            {
                _printerViewModelBuilder.SetInactive(id);
                TempData["msg"] = "Printer Successfully Deactivated";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to deactivate Printer " + ex.Message);
                _log.Error("Failed to deactivate Printer" + ex.ToString());
            }
            return RedirectToAction("Listprinters");
        }
        
        public ActionResult Activate(Guid id)
        {
            try
            {
                _printerViewModelBuilder.SetActive(id);
                TempData["msg"] = "Printer Successfully activated";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to activate Printer" + ex.Message);
                _log.Error("Failed to activate Printer" + ex.ToString());

            }

            return RedirectToAction("Listprinters");
        }

        [Authorize(Roles = "RoleDeleteMasterData")]
        public ActionResult Delete(Guid id)
        {
            try
            {
                _printerViewModelBuilder.SetAsDeleted(id);
                TempData["msg"] = "Printer Successfully deleted";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to delete Printer" + ex.Message);
                _log.Error("Failed to delete Printer" + ex.ToString());

            }

            return RedirectToAction("Listprinters");
        }
    }
}
