using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Admin.RouteViewModel;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.Validation;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.RouteViewBuilder;

namespace Distributr.HQ.Web.Areas.Admin.Controllers
{
    [Authorize ]
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
    
        public ActionResult ListRoutes(bool? showInactive, int? itemsperpage)
        {
            try
            {
                if (itemsperpage != null)
                {
                    ViewModelBase.ItemsPerPage = itemsperpage.Value;
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
                IList<AdminRouteViewModel> routes = _adminRouteViewModelBuilder.GetAll(showinactive);
                return View(routes);
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }

        public ActionResult DetailsRoutes(Guid id)
        {
            AdminRouteViewModel route = _adminRouteViewModelBuilder.Get(id);
            return View(route);
        }

        public ActionResult CreateRoute()
        {
            ViewBag.DistributorList = _adminRouteViewModelBuilder.Distributor();
            return View("CreateRoute", new AdminRouteViewModel());
        }

        [HttpPost]
        public ActionResult CreateRoute(AdminRouteViewModel adminRouteViewModel)
        {
            ViewBag.DistributorList = _adminRouteViewModelBuilder.Distributor();        


            try
            {
                adminRouteViewModel.Id = Guid.NewGuid();
                _adminRouteViewModelBuilder.Save(adminRouteViewModel);
             
                return RedirectToAction("listroutes");
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


        public ActionResult EditRoute(Guid id)
        {
            ViewBag.DistributorList = _adminRouteViewModelBuilder.Distributor();
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
                return View();
            }
        }




        public ActionResult Deactivate(Guid id)
        {

            try
            {
                _adminRouteViewModelBuilder.SetInactive(id);
                TempData["msg"] = "Successfully Deactivated";

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
                TempData["msg"] = "Successfully Activated";

            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;

            }
            return RedirectToAction("listroutes");
        }

        public ActionResult Delete(Guid id)
        {

            try
            {
                _adminRouteViewModelBuilder.SetAsDeleted(id);
                TempData["msg"] = "Successfully Deleted";

            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;

            }
            return RedirectToAction("listroutes");
        }


    }
}
