using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.HQ.Lib.ViewModels.Admin.CostCenter;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.CostCenter;
using Distributr.Core.Validation;
using Distributr.HQ.Lib.Validation;

namespace Distributr.HQ.Web.Areas.Admin.Controllers
{
    public class OutletCategoryController : Controller
    {
        //cn
        IOutletCategoryViewModelBuilder _outletCategoryViewModelBuilder;
        public OutletCategoryController(IOutletCategoryViewModelBuilder outletCategoryViewModelBuilder)
        {
            _outletCategoryViewModelBuilder = outletCategoryViewModelBuilder;
        }
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult ListOutletCategories(Boolean? showInactive)
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
            IList<OutletCategoryViewModel> outletCategories = _outletCategoryViewModelBuilder.GetAll(showinactive);
            return View(outletCategories);
        }

        public ActionResult DetailsOutletCategory(int id)
        {
            OutletCategoryViewModel ocVM = _outletCategoryViewModelBuilder.GetByID(id);
            return View(ocVM);
        }

        public ActionResult EditOutletCategory(int id)
        {
            OutletCategoryViewModel outletCategory = _outletCategoryViewModelBuilder.GetByID(id);
            return View(outletCategory);
        }

        [HttpPost]
        public ActionResult EditOutletCategory(OutletCategoryViewModel vm)
        {
            try
            {
                _outletCategoryViewModelBuilder.Save(vm);
                return RedirectToAction("listoutletcategories");
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

        public ActionResult CreateOutletCategory()
        {
            return View("CreateOutletCategory", new OutletCategoryViewModel());
        } 

        [HttpPost]
        public ActionResult CreateOutletCategory(OutletCategoryViewModel outletCategoryViewModel)
        {
            try
            {
                string Create = Request.Params["Create"];
                string process = Request.Params["process"];
                _outletCategoryViewModelBuilder.Save(outletCategoryViewModel);
                return RedirectToAction("listoutletcategories");
            }
            catch (DomainValidationException ve)
            {
                ValidationSummary.DomainValidationErrors(ve, ModelState);
                return View(outletCategoryViewModel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(outletCategoryViewModel);
            }

        }
        public ActionResult Deactivate(int id)
        {

            try
            {
                _outletCategoryViewModelBuilder.SetInactive(id);
                TempData["msg"] = "Successfully Deactivated";



            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;

            }
            return RedirectToAction("ListOutletCategories");
        }
        public JsonResult Owner(string bName)
        {
            IList<OutletCategoryViewModel> tvm = _outletCategoryViewModelBuilder.GetAll(true);
            return Json(tvm);
        }
    }
}
