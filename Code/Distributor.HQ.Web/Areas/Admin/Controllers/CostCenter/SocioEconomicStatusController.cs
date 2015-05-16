using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.CostCenter;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Admin.CostCenter;
using Distributr.HQ.Lib.Validation;

namespace Distributr.HQ.Web.Areas.Admin.Controllers.CostCenter
{
    [Authorize ]
    public class SocioEconomicStatusController : Controller
    {
        ISocioEconomicStatusViewModelBuilder _socioEconomicStatusViewModelBuilder;
        public SocioEconomicStatusController(ISocioEconomicStatusViewModelBuilder socioEconomicStatusViewModelBuilder)
        {
            _socioEconomicStatusViewModelBuilder = socioEconomicStatusViewModelBuilder;
        }

        public ActionResult Index()
        {
            return View();
        }
         [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult List(bool? showInactive, int? itemsperpage)
        {
            bool showinactive = false;
            if (showInactive != null)
                showinactive = (bool)showInactive;
             
                if (itemsperpage != null)
                {
                    ViewModelBase.ItemsPerPage = itemsperpage.Value;
                }
            ViewBag.showInactive = showinactive;

            if (TempData["msg"] != null)
            {
                ViewBag.msg = TempData["msg"].ToString();
                TempData["msg"] = null;
            }
            IList<SocioEconomicStatusViewModel> seVM = _socioEconomicStatusViewModelBuilder.GetAll(showinactive);
            return View(seVM);
        }

         public ActionResult Details(Guid id)
        {
            SocioEconomicStatusViewModel seVM = _socioEconomicStatusViewModelBuilder.GetByID(id);
            return View(seVM);
        }
        [Authorize(Roles = "RoleModifyMasterData")]
        public ActionResult Edit(Guid id)
        {
            SocioEconomicStatusViewModel se = _socioEconomicStatusViewModelBuilder.GetByID(id);
            return View(se);
        }

        [HttpPost]
        public ActionResult Edit(SocioEconomicStatusViewModel vm)
        {
            try
            {
                _socioEconomicStatusViewModelBuilder.Save(vm);
                return RedirectToAction("List");
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
        [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult Create()
        {
            return View("Create", new SocioEconomicStatusViewModel());
        }

        [HttpPost]
        public ActionResult Create(SocioEconomicStatusViewModel socioEconomicStatusViewModel)
        {
            try
            {
                string Create = Request.Params["Create"];
                string process = Request.Params["process"];
                socioEconomicStatusViewModel.Id = Guid.NewGuid(); ;
                _socioEconomicStatusViewModelBuilder.Save(socioEconomicStatusViewModel);
                return RedirectToAction("List");
            }
            catch (DomainValidationException ve)
            {
                ValidationSummary.DomainValidationErrors(ve, ModelState);
                return View(socioEconomicStatusViewModel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(socioEconomicStatusViewModel);
            }

        }

        public ActionResult Deactivate(Guid id)
        {

            try
            {
                _socioEconomicStatusViewModelBuilder.SetInActive(id);
                TempData["msg"] = "Successfully Deactivated";



            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;

            }
            return RedirectToAction("List");
        }

        public JsonResult Owner(string bName)
        {
            IList<SocioEconomicStatusViewModel> tvm = _socioEconomicStatusViewModelBuilder.GetAll(true);
            return Json(tvm);
        }
    }
}
