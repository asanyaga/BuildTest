using System;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.Paging;
using Distributr.HQ.Lib.Validation;
using Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.CostCentreViewModelBuilders;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Agrimanagr.CostCentreViewModel;
using log4net;

namespace Agrimanagr.HQ.Areas.Agrimanagr.Controllers
{
    public class HubController : Controller
    {
        private IHubViewModelBuilder _hubViewModelBuilder;
        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ActionResult Index()
        {
            return View();
        }

        public HubController(IHubViewModelBuilder hubViewModelBuilder)
        {
            _hubViewModelBuilder = hubViewModelBuilder;
        }

        [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListHubs(Boolean? showInactive, int page = 1, int itemsperpage=10, string srchParam = "")
        {
            try
            {
                if (itemsperpage != null)
                {
                    ViewModelBase.ItemsPerPage = itemsperpage;
                }
                bool showinactive = false;
                if (showInactive != null)
                    showinactive = (bool) showInactive;

                ViewBag.showInactive = showinactive;
                ViewBag.srchParam = srchParam;
                if (TempData["msg"] != null)
                {
                    ViewBag.msg = TempData["msg"].ToString();
                    TempData["msg"] = null;
                }

               // var ls = _hubViewModelBuilder.GetAll(showinactive);
                int currentPageIndex = page - 1 < 0 ? 0 : page - 1;
                int take = itemsperpage ;
                int skip = take*currentPageIndex;
                var query = new QueryStandard()
                                {   ShowInactive = showinactive,
                                    Skip = skip,
                                    Take = take,
                                    Name = srchParam
                                };
                

                var ls = _hubViewModelBuilder.Query(query);
                var total = ls.Count;
                var data = ls.Data;

                return View(data.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage, total));
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
                _hubViewModelBuilder.SetInactive(id);
                TempData["msg"] = "Hub Successfully Deactivated";
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                TempData["msg"] = dve.Message;
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to deactivate hub " + ex.Message);
                _log.Error("Failed to deactivate hub" + ex.ToString());
            }
            return RedirectToAction("ListHubs");
        }

        public ActionResult Activate(Guid id)
        {
            try
            {
                _hubViewModelBuilder.SetActive(id);
                TempData["msg"] = "Hub Successfully activated";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to activate hub" + ex.Message);
                _log.Error("Failed to activate hub" + ex.ToString());

            }

            return RedirectToAction("ListHubs");
        }

        [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult DetailsHub(Guid id)
        {
            try
            {
                HubViewModel hubVM = _hubViewModelBuilder.Get(id);
                return View(hubVM);
            }
            catch (Exception ex)
            {

                ViewBag.msg = ex.Message;
                return View();
            }

        }

        [Authorize(Roles = "RoleUpdateMasterData")]
        public ActionResult EditHub(Guid id)
        {
            ViewBag.RegionList = _hubViewModelBuilder.Region();
            ViewBag.ContactList = _hubViewModelBuilder.Contact();
            ViewBag.ParentCostCentreList = _hubViewModelBuilder.ParentCostCentre();
            try
            {

                HubViewModel hubViewModel = _hubViewModelBuilder.Get(id);
                return View(hubViewModel);
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }

        [HttpPost]
        public ActionResult EditHub(HubViewModel vm)
        {
            ViewBag.RegionList = _hubViewModelBuilder.Region();
            ViewBag.ContactList = _hubViewModelBuilder.Contact();
            ViewBag.ParentCostCentreList = _hubViewModelBuilder.ParentCostCentre();
            try
            {
                _hubViewModelBuilder.Save(vm);
                TempData["msg"] = "Hub Successfully Edited";
                return RedirectToAction("ListHubs");
            }
            catch (DomainValidationException ve)
            {
                ValidationSummary.DomainValidationErrors(ve, ModelState);
                _log.Debug("Failed to edit Commodity producer " + ve.Message);
                _log.Error("Failed to edit Commodity producer" + ve.ToString());

                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                _log.Debug("Failed to edit Commodity producer " + ex.Message);
                _log.Error("Failed to edit Commodity producer" + ex.ToString());
                return View();
            }
        }

        [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult CreateHub()
        {
            ViewBag.RegionList = _hubViewModelBuilder.Region();
            ViewBag.ContactList = _hubViewModelBuilder.Contact();
            ViewBag.ParentCostCentreList = _hubViewModelBuilder.ParentCostCentre();


            return View(new HubViewModel());
        }

        [HttpPost]
        public ActionResult CreateHub(HubViewModel vm)
        {
            ViewBag.RegionList = _hubViewModelBuilder.Region();
            ViewBag.ContactList = _hubViewModelBuilder.Contact();
            ViewBag.ParentCostCentreList = _hubViewModelBuilder.ParentCostCentre();

            try
            {
                vm.Id = Guid.NewGuid();
                _hubViewModelBuilder.Save(vm);

                TempData["msg"] = "Hub Successfully Created";

                return RedirectToAction("ListHubs");
            }
            catch (DomainValidationException ve)
            {
                TempData["msg"] = ve.Message;
                ValidationSummary.DomainValidationErrors(ve, ModelState);
                _log.Debug("Failed to create commodity producers " + ve.Message);
                _log.Error("Failed to create commodity producers" + ve.ToString());

                return View(vm);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                _log.Debug("Failed to create Commodity producers " + ex.Message);
                _log.Error("Failed to create Commodity producers" + ex.ToString());

                return View(vm);
            }

        }

        [Authorize(Roles = "RoleDeleteMasterData")]
        public ActionResult DeleteHub(Guid id)
        {
            try
            {
                _hubViewModelBuilder.SetAsDeleted(id);
                TempData["msg"] = "Hub Successfully deleted";
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                TempData["msg"] = dve.Message;
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to delete Hub" + ex.Message);
                _log.Error("Failed to delete Hub" + ex.ToString());


            }

            return RedirectToAction("ListHubs");
        }
    }
}
