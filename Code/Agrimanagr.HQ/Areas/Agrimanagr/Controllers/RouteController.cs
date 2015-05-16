using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.Paging;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Admin.RouteViewModel;
using Distributr.HQ.Lib.Validation;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.RouteViewBuilder;

namespace Agrimanagr.HQ.Areas.Agrimanagr.Controllers
{
   /* [Authorize ]*/
    public class RouteController : Controller
    {

        IAdminRouteViewModelBuilder _adminRouteViewModelBuilder;
        
        public RouteController(IAdminRouteViewModelBuilder adminRouteViewModelBuilder) {

            _adminRouteViewModelBuilder = adminRouteViewModelBuilder;
        
        } 
        
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListRoutes(bool? showInactive,string srchparam = "", int page=1, int itemsperpage=10)
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
                ViewBag.srchparam = srchparam;
                if (TempData["msg"] != null)
                {
                    ViewBag.msg = TempData["msg"].ToString();
                    TempData["msg"] = null;
                }
              
                int currentPageIndex = page - 1 < 0 ? 0 : page - 1;
                int take = itemsperpage;
                int skip= currentPageIndex*take;
                var query = new QueryStandard(){Name = srchparam , ShowInactive = showinactive ,Skip = skip ,Take =take};

                var ls = _adminRouteViewModelBuilder.Query(query);

                var total = ls.Count;
                var data = ls.Data;
                return View(data.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage,total ));
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }

        [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult DetailsRoutes(Guid id)
        {
            AdminRouteViewModel route = _adminRouteViewModelBuilder.Get(id);
            return View(route);
        }

        [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult CreateRoute()
        {
            ViewBag.DistributorList = _adminRouteViewModelBuilder.Distributor();
            ViewBag.RegionList = _adminRouteViewModelBuilder.Regions();
            return View("CreateRoute", new AdminRouteViewModel());
        }

        [HttpPost]
        public ActionResult CreateRoute(AdminRouteViewModel adminRouteViewModel)
        {
         
            try
            {
                adminRouteViewModel.Id = Guid.NewGuid();
                _adminRouteViewModelBuilder.Save(adminRouteViewModel);
                TempData["msg"] = "Route Successfully Created";
                return RedirectToAction("listroutes");
            }
            catch (DomainValidationException ve)
            {
                ValidationSummary.DomainValidationErrors(ve, ModelState);
                ViewBag.DistributorList = _adminRouteViewModelBuilder.Distributor();
                ViewBag.RegionList = _adminRouteViewModelBuilder.Regions();
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.DistributorList = _adminRouteViewModelBuilder.Distributor();
                ViewBag.RegionList = _adminRouteViewModelBuilder.Regions();
                ModelState.AddModelError("", ex.Message);
               
            }
            return View();
        }

        [Authorize(Roles = "RoleUpdateMasterData")]
        public ActionResult EditRoute(Guid id)
        {
            ViewBag.DistributorList = _adminRouteViewModelBuilder.Distributor();
            ViewBag.RegionList = _adminRouteViewModelBuilder.Regions();
            try
            {
                AdminRouteViewModel route = _adminRouteViewModelBuilder.Get(id);
                return View(route);
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }
        
        [HttpPost]
        public ActionResult EditRoute(AdminRouteViewModel vm)
        {
            ViewBag.DistributorList = _adminRouteViewModelBuilder.Distributor();

            try
            {
                _adminRouteViewModelBuilder.Save(vm);
                TempData["msg"] = "Route Successfully Edited";
                return RedirectToAction("listroutes");
            }
            catch (DomainValidationException ve)
            {

                ValidationSummary.DomainValidationErrors(ve, ModelState);
                return View();

            }
            catch (Exception ex)
            {
               //Session["msg"] = ex.Message;
                ModelState.AddModelError("", ex.Message);
            }
            ViewBag.DistributorList = _adminRouteViewModelBuilder.Distributor();
            ViewBag.RegionList = _adminRouteViewModelBuilder.Regions();
            return View();
        }

        public ActionResult Deactivate(Guid id)
        {

            try
            {
                _adminRouteViewModelBuilder.SetInactive(id);
                TempData["msg"] = "Route Successfully Deactivated";

            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                TempData["msg"] = dve.Message;
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;

            }
            return RedirectToAction("listroutes");
        }

        public JsonResult Owner(string blogName)
        {
            IList<AdminRouteViewModel> route = _adminRouteViewModelBuilder.GetAll(true);
            return Json(route);
        }

        public ActionResult Activate(Guid id)
        {

            try
            {
                _adminRouteViewModelBuilder.SetActive(id);
                TempData["msg"] = "Route Successfully Activated";

            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;

            }
            return RedirectToAction("listroutes");
        }

        [Authorize(Roles = "RoleDeleteMasterData")]
        public ActionResult Delete(Guid id)
        {

            try
            {
                _adminRouteViewModelBuilder.SetAsDeleted(id);
                TempData["msg"] = "Route Successfully Deleted";

            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                TempData["msg"] = dve.Message;
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;

            }
            return RedirectToAction("listroutes");
        }


    }
}
