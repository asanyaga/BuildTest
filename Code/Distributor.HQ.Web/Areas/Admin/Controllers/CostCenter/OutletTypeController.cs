using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.CostCenter;
using Distributr.HQ.Lib.ViewModels.Admin.CostCenter;
using Distributr.Core.Validation;
using Distributr.HQ.Lib.Validation;


namespace Distributr.HQ.Web.Areas.Admin.Controllers
{
    public class OutletTypeController : Controller
    {
        //cn
        IOutletTypeViewModelBuilder _outletTypeViewModelBuilders;
        public OutletTypeController(IOutletTypeViewModelBuilder outletTypeViewModelBuilders)
        {
            _outletTypeViewModelBuilders = outletTypeViewModelBuilders;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListOutletTypes(Boolean? showInactive)
        {
            bool showinactive = false;
            if (showInactive != null)
                showinactive = (bool)showInactive;

            ViewBag.showInactive = showinactive;

            if (TempData["msg"] != null)
            {
                ViewBag.msg = TempData["msg"].ToString();
                TempData["msg"] = null;
            }
            IList<OutletTypeViewModel> outletTypes = _outletTypeViewModelBuilders.GetAll(showinactive);
            return View(outletTypes);
        }

        public ActionResult DetailsOutletType(int id)
        {
            OutletTypeViewModel outletTypeVM = _outletTypeViewModelBuilders.GetByID(id);
            return View(outletTypeVM);
        }

        public ActionResult EditOutletType(int id)
        {
            OutletTypeViewModel outletType = _outletTypeViewModelBuilders.GetByID(id);
            return View(outletType);
        }

        [HttpPost]
        public ActionResult EditOutletType(OutletTypeViewModel vm)
        {
            try
            {
                _outletTypeViewModelBuilders.Save(vm);
                return RedirectToAction("listoutlettypes");
            }
            catch (DomainValidationException ve)
            {
                ValidationSummary.DomainValidationErrors(ve, ModelState);
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }


        public ActionResult CreateOutletType()
        {
            return View("CreateOutletType", new OutletTypeViewModel());
        }

        [HttpPost]
        public ActionResult CreateOutletType(OutletTypeViewModel outletTypeViewModel)
        {
            try
            {
                string Create = Request.Params["Create"];
                string process = Request.Params["process"];
                _outletTypeViewModelBuilders.Save(outletTypeViewModel);
                return RedirectToAction("listoutlettypes");
            }
            catch (DomainValidationException ve)
            {
                ValidationSummary.DomainValidationErrors(ve, ModelState);
                return View(outletTypeViewModel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(outletTypeViewModel);
            }

        }

        public ActionResult Deactivate(int id)
        {

            try
            {
                _outletTypeViewModelBuilders.SetInactive(id);
                TempData["msg"] = "Successfully Deactivated";



            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;

            }
            return RedirectToAction("ListOutletTypes");
        }
        public JsonResult Owner(string bName)
        {
            IList<OutletTypeViewModel> tvm = _outletTypeViewModelBuilders.GetAll(true);
            return Json(tvm);
        }
    }
}
