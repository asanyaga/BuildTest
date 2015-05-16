using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Distributr.Core.Utility;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Admin.CostCenter;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.CostCenter;
using Distributr.HQ.Lib.Validation;
using log4net;
using System.Reflection;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Transaction;
using Distributr.HQ.Lib.Paging;

namespace Agrimanagr.HQ.Areas.Agrimanagr.Controllers
{

    public class CountryController : Controller
    {
        ICountryViewModelBuilder _countryViewModelBuilder;
        IAuditLogViewModelBuilder _auditLogViewModelBuilder;
        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public CountryController(ICountryViewModelBuilder countryViewModelBuilder, IAuditLogViewModelBuilder auditLogViewModelBuilder)
        {
            _countryViewModelBuilder = countryViewModelBuilder;
            _auditLogViewModelBuilder = auditLogViewModelBuilder;
        }
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListCountry(int page = 1, int itemsperpage = 10, bool showinactive = false, string srchparam = "")
        {

            try
            {
                if (itemsperpage != null)
                {
                    ViewModelBase.ItemsPerPage = itemsperpage;
                }
             /*   bool showinactive = false;*/
                if (showinactive != null)
                    showinactive = (bool)showinactive;
                ViewBag.srchparam = srchparam;
                ViewBag.showInactive = showinactive;

                if (TempData["msg"] != null)
                {
                    ViewBag.msg = TempData["msg"].ToString();
                    TempData["msg"] = null;
                }
                //IList<CountryViewModel> country = _countryViewModelBuilder.GetAll(showinactive);
                //var countries = _countryViewModelBuilder.GetAll(showinactive);
                //var coutriesPagedList = countries.AsPagination(page ?? 1, pageSize);
                //var countriesListContainer = new CountryViewModel
                //{
                //    countriesPagedList = coutriesPagedList
                //};

                //return View(countriesListContainer);

                //var ls = _countryViewModelBuilder.GetAll(showinactive);
                //int currentPageIndex = page.HasValue ? page.Value - 1 : 0;

                int currentPageIndex = page - 1 < 0 ? 0 : page - 1;
                int take = itemsperpage;
                int skip = currentPageIndex * take;
                var query = new QueryStandard();
                query.ShowInactive = showinactive;
                query.Skip = skip;
                query.Take = take;
                query.Name = srchparam;

                var ls = _countryViewModelBuilder.Query(query);
                var total = ls.Count;
                var data = ls.Data;
                return View(data.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage, total));
            }
            catch (Exception ex)
            {
                _log.Debug("Failed to list country" + ex.Message);
                _log.Error("Failed to list country" + ex.ToString());
                return View();
            }
        }

        [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult DetailsCountry(Guid id)
        {
            CountryViewModel countr = _countryViewModelBuilder.Get(id);
            return View(countr);
        }

        [Authorize(Roles = "RoleUpdateMasterData")]
        public ActionResult EditCountry(Guid id)
        {
            try
            {
                CountryViewModel countr = _countryViewModelBuilder.Get(id);
                return View(countr);
            }
            catch (Exception ex)
            {
                _log.Debug("Failed to edit country" + ex.Message);
                _log.Error("Failed to edit country" + ex.ToString());
                return View();
            }
        }
        [HttpPost]
        public ActionResult EditCountry(CountryViewModel cvm)
        {
            try
            {
                _countryViewModelBuilder.Save(cvm);
                TempData["msg"] = "Country Successfully Edited";
                return RedirectToAction("ListCountry");
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                _log.Debug("Failed to edit country" + dve.Message);
                _log.Error("Failed to edit country" + dve.ToString());
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                _log.Debug("Failed to edit country" + ex.Message);
                _log.Error("Failed to edit country" + ex.ToString());
                return View();
            }
        }

        [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult CreateCountry()
        {
            return View(new CountryViewModel());
        }
        [HttpPost]
        public ActionResult CreateCountry(CountryViewModel cvm)
        {
            try
            {
                cvm.id = Guid.NewGuid();
                _countryViewModelBuilder.Save(cvm);
                TempData["msg"] = "Country Successfully Created";
                return RedirectToAction("ListCountry");
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                _log.Debug("Failed to create country" + dve.Message);
                _log.Error("Failed to create country" + dve.ToString());
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                _log.Debug("Failed to create country" + ex.Message);
                _log.Error("Failed to create country" + ex.ToString());
                return View();
            }
        }

        public ActionResult DeActivate(Guid id)
        {
            try
            {
                _countryViewModelBuilder.SetInActive(id);
                TempData["msg"] = "Successfully Deactivated";
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                _log.Debug("Failed to deactivate country" + dve.Message);
                _log.Error("Failed to deactivate country" + dve.ToString());
                TempData["msg"] = dve.Message;
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to deactivate country" + ex.Message);
                _log.Error("Failed to deactivate country" + ex.ToString());
            }
            return RedirectToAction("ListCountry");
        }

        [Authorize(Roles = "RoleDeleteMasterData")]
        public ActionResult Delete(Guid id)
        {
            try
            {
                _countryViewModelBuilder.SetAsDeleted(id);
                TempData["msg"] = "Successfully Deleted";
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                _log.Debug("Failed to delete country" + dve.Message);
                _log.Error("Failed to delete country" + dve.ToString());
                TempData["msg"] = dve.Message;
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to delete country" + ex.Message);
                _log.Error("Failed to delete country" + ex.ToString());
            }
            return RedirectToAction("ListCountry");
        }
        public ActionResult Activate(Guid id, string name)
        {
            try
            {
                _countryViewModelBuilder.SetActive(id);
                TempData["msg"] = name + " Successfully Activated";

            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                _log.InfoFormat("Failed to Activate Country" + dve.Message);
                _log.Error("Failed to Activate Country" + dve.ToString());
                TempData["msg"] = dve.Message;
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to Activate Country" + ex.Message);
                _log.Error("Failed to Activated Country" + ex.ToString());
            }
            return RedirectToAction("ListCountry");
        }

        public JsonResult Owner(string blogName)
        {
            IList<CountryViewModel> country = _countryViewModelBuilder.GetAll(true);
            return Json(country);
        }


    }
}
