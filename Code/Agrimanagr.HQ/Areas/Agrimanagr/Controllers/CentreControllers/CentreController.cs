using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Distributr.Core.Utility;
using Distributr.HQ.Lib.Paging;
using Distributr.HQ.Lib.Validation;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Transaction;
using Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.CentreViewModelBuilders;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Agrimanagr.CentreViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using log4net;
using System.Reflection;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;

namespace Agrimanagr.HQ.Areas.Agrimanagr.Controllers.CentreControllers
{
    public class CentreController : Controller
    {
        ICentreViewModelBuilder _centreViewModelBuilder;
        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public CentreController(ICentreViewModelBuilder centreViewModelBuilder)
        {
            _centreViewModelBuilder = centreViewModelBuilder;
        }

        public ActionResult Index()
        {
            return View();
        }

        public IEnumerable<SelectListItem> ItemsPerPageList
        {
            get
            {
                return new SelectList(new[]
                                          {
                                              new SelectListItem {Value = "10", Text = "10", Selected = true},
                                              new SelectListItem {Value = "15", Text = "15"},
                                              new SelectListItem {Value = "20", Text = "20"},
                                              new SelectListItem {Value = "25", Text = "25"},
                                              new SelectListItem {Value = "30", Text = "30"}
                                          });
            }
        }

        [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListCentres(bool showInactive = false, int page = 1, int itemsperpage = 10, string srchparam = "")
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
                ViewBag.srchParam = srchparam;
                ViewBag.showInactive = showinactive;

                if (TempData["msg"] != null)
                {
                    ViewBag.msg = TempData["msg"].ToString();
                    TempData["msg"] = null;
                }
              
               // var query=new QueryStandard();
               // query.ShowInactive = showinactive;

                //var ls = _centreViewModelBuilder.Query(query);
                //var ls = _centreViewModelBuilder.GetAll(showinactive);

                int currentPageIndex = page - 1 < 0 ? 0 : page - 1;
                int take = itemsperpage;
                int skip = currentPageIndex * take;
                var query = new QueryStandard { Skip = skip, Take = take, Name = srchparam, ShowInactive = showInactive };
                var ls = _centreViewModelBuilder.Query(query);
                var total = ls.Count;
                var data = ls.Data.ToList();

                return View(data.ToPagedList(currentPageIndex, take, total));
            }
            catch (Exception ex)
            {
                _log.Debug("Failed to list centres " + ex.Message);
           
                return View();
            }
        }

        [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult CreateCentre()
        {
            CentreViewModel vm = new CentreViewModel();
            vm = _centreViewModelBuilder.Setup(vm);
            return View("CreateCentre", vm);
        }

        [HttpPost]
        public ActionResult CreateCentre(CentreViewModel acvm)
        {
            acvm = _centreViewModelBuilder.Setup(acvm);
            acvm.RouteList = _centreViewModelBuilder.GetRouteList(acvm.SelectedHubId);
            try
            {
                acvm.Id = Guid.NewGuid();
                _centreViewModelBuilder.Save(acvm);
                TempData["msg"] = "Centre Successfully Created";

                return RedirectToAction("ListCentres");
            }
            catch (DomainValidationException ve)
            {
                ValidationSummary.DomainValidationErrors(ve, ModelState);
                _log.Debug("Failed to create centres " + ve.Message);
                acvm = _centreViewModelBuilder.Setup(acvm);
                acvm.RouteList = _centreViewModelBuilder.GetRouteList(acvm.SelectedHubId);
                return View(acvm);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                _log.Debug("Failed to create centres " + ex.Message);
                acvm = _centreViewModelBuilder.Setup(acvm);
                acvm.RouteList = _centreViewModelBuilder.GetRouteList(acvm.SelectedHubId);
                return View(acvm);
            }

        }

        [Authorize(Roles = "RoleUpdateMasterData")]
        public ActionResult EditCentre(Guid id, string msg)
        {
            ViewBag.msg = msg;
            try
            {
                CentreViewModel centre = _centreViewModelBuilder.Get(id);
                centre = _centreViewModelBuilder.Setup(centre);
                centre.RouteList = _centreViewModelBuilder.GetRouteList(centre.SelectedHubId);

                return View(centre);
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }

        [HttpPost]
        public ActionResult EditCentre(CentreViewModel vm)
        {
            try
            {
                _centreViewModelBuilder.Save(vm);
                TempData["msg"] = "Centre Successfully Edited";
                return RedirectToAction("ListCentres");
            }
            catch (DomainValidationException ve)
            {
                ValidationSummary.DomainValidationErrors(ve, ModelState);
                _log.Debug("Failed to edit Centre " + ve.Message);

                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                _log.Debug("Failed to edit Centre " + ex.Message);

                return View();
            }
        }

        public ActionResult Activate(Guid id)
        {
            try
            {
                _centreViewModelBuilder.SetActive(id);
                TempData["msg"] = "Centre Successfully activated";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to activate centre" + ex.Message);
                _log.Error("Failed to activate centre" + ex.ToString());


            }

            return RedirectToAction("ListCentres");
        }

        public ActionResult DeActivate(Guid id)
        {
            try
            {
                _centreViewModelBuilder.SetInactive(id);
                TempData["msg"] = "Centre Successfully deactivated";
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                _log.Debug("Failed to deactivate Centre " + dve.Message);
                _log.Error("Failed to deactivate Centre " + dve.ToString());
                TempData["msg"] = dve.Message;
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to DeActivate centre" + ex.Message);
                _log.Error("Failed to DeActivate centre" + ex.ToString());


            }

            return RedirectToAction("ListCentres");
        }

        [Authorize(Roles = "RoleDeleteMasterData")]
        public ActionResult Delete(Guid id)
        {
            try
            {
                _centreViewModelBuilder.SetAsDeleted(id);
                TempData["msg"] = "Centre Successfully deleted";
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                _log.Debug("Failed to delete Centre " + dve.Message);
                _log.Error("Failed to delete Centre " + dve.ToString());
                TempData["msg"] = dve.Message;
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to Delete centre" + ex.Message);
                _log.Error("Failed to Delete centre" + ex.ToString());


            }

            return RedirectToAction("ListCentres");
        }

        public ActionResult GetRegionRoutes(Guid hubId, Guid centreId)
        {
            var items = _centreViewModelBuilder.GetRouteList(hubId);
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListItems()
        {
            return View("ListCentres");
        }
    }
}
