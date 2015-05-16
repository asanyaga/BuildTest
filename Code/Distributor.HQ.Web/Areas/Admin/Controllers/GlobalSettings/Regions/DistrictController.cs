using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.Core.Utility;
using Distributr.HQ.Lib.Paging;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.CostCenter;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Admin.CostCenter;
using Distributr.HQ.Lib.Validation;
using log4net;
using System.Reflection;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Transaction;

namespace Distributr.HQ.Web.Areas.Admin.Controllers.GlobalSettings.Regions
{
     [Authorize]
    public class DistrictController : Controller
    { 
        IDistrictViewModelBuilder _districtViewModelBuilder;
        IProvinceViewModelBuilder _provinceViewModelBuilder;
        ICountryViewModelBuilder _countryViewModelBuilder;
        IAuditLogViewModelBuilder _auditLogViewModelBuilder;
        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public DistrictController(IDistrictViewModelBuilder districtViewModelBuilder, IProvinceViewModelBuilder provinceViewModelBuilder, ICountryViewModelBuilder countryViewModelBuilder,IAuditLogViewModelBuilder auditLogViewModelBuilder)
        {
            _provinceViewModelBuilder = provinceViewModelBuilder;
            _districtViewModelBuilder = districtViewModelBuilder;
            _countryViewModelBuilder = countryViewModelBuilder;
            _auditLogViewModelBuilder = auditLogViewModelBuilder;
        }
        public ActionResult Index()
        {
            return View();
        }
         [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListDistricts(bool? showInactive, int page = 1, int itemsperpage = 10, string srchParam = "")
        {
            try
            {
                if (itemsperpage != null)
                {
                    ViewModelBase.ItemsPerPage = itemsperpage;
                }
                bool showinactive = false;
                if (TempData["msg"] != null)
                {
                    ViewBag.msg = TempData["msg"].ToString();
                    TempData["msg"] = null;
                }
                if (showInactive != null)
                    showinactive = (bool)showInactive;
                ViewBag.showInactive = showinactive;
                ViewBag.srchText = srchParam;
                
                int currentPageIndex = page < 0 ? 0 : page- 1;
                int take = itemsperpage;
                int skip = currentPageIndex*take;

                var query = new QueryStandard() { Name = srchParam, ShowInactive = showinactive, Skip = skip, Take = take };

                var ls = _districtViewModelBuilder.Query(query);
                var data = ls.Data;
                var count = ls.Count;

                return View(data.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage, count));
            }
            catch (Exception ex)
            {
                _log.Debug("Failed to list district" + ex.Message);
                _log.Error("Failed to list district" + ex.ToString());
                return View();
            }

        }
         [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult CreateDistrict()
        {
            ViewBag.Country = _districtViewModelBuilder.GetCountry();
            ViewBag.Province = _districtViewModelBuilder.GetProvince();
            return View("CreateDistrict", new DistrictViewModel());
        }
        [HttpPost]
        public ActionResult CreateDistrict(DistrictViewModel dvm)
       // public ActionResult CreateDistrict(int countryId, int provinceId, string distName, DistrictViewModel dvm)
        {
            ViewBag.Province = _districtViewModelBuilder.GetProvince();
            try
            {
                if (dvm.ProvinceId == Guid.Empty || dvm.ProvinceId.ToString() == "")
                {
                    ViewBag.Country = _districtViewModelBuilder.GetCountry();

                    ViewBag.msg2 = "Select Province\n Or create province under this country";
                   
                    return View();
                }
                else
                {
                    dvm.Id = Guid.NewGuid();
                    _districtViewModelBuilder.Save(dvm);
                    TempData["msg"] = "District Successfully Created";
                    _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Create", "District", DateTime.Now);
                }
                return RedirectToAction("ListDistricts");
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                _log.Debug("Failed to create district" + dve.Message);
                _log.Error("Failed to create district" + dve.ToString());
                return View();
            }
            catch (Exception exx)
            {
                ViewBag.msg = exx.Message;
                _log.Debug("Failed to create district" + exx.Message);
                _log.Error("Failed to create district" + exx.ToString());
                return View();
            }
            
        }
         [Authorize(Roles = "RoleModifyMasterData")]
        public ActionResult EditDistrict(Guid id)
        {
            ViewBag.Country = _districtViewModelBuilder.GetCountry();
            ViewBag.Province = _districtViewModelBuilder.GetProvince();
            try
            {
                DistrictViewModel dist = _districtViewModelBuilder.GetById(id);
                return View(dist);
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }

        }
        [HttpPost]
        public ActionResult EditDistrict(DistrictViewModel dvm)
        {
            ViewBag.Province = _districtViewModelBuilder.GetProvince();
            try
            {
            _districtViewModelBuilder.Save(dvm);
            _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Edit", "District", DateTime.Now);
            TempData["msg"] = "District Successfully Edited";
            return RedirectToAction("ListDistricts");
             }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                _log.Debug("Failed to edit district" + dve.Message);
                _log.Error("Failed to edit district" + dve.ToString());
                return View();
            }
            catch (Exception exx)
            {
                ViewBag.msg = exx.Message;
                _log.Debug("Failed to edit district" + exx.Message);
                _log.Error("Failed to edit district" + exx.ToString());
                return View();
            }
        }
        public ActionResult DeActivate(Guid id)
        {
            try
            {
                _districtViewModelBuilder.SetInactive(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Deactivate", "District", DateTime.Now);
                TempData["msg"] = "District Successfully Deactivated";

            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to deactivate district" + ex.Message);
                _log.Error("Failed to deactivate district" + ex.ToString());
            }
            
            return RedirectToAction("ListDistricts");
        }
         public ActionResult Activate(Guid id, string name)
         {
             try
             {
                 _districtViewModelBuilder.SetActive(id);
                 _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name,"Activate","District",DateTime.Now);
                 TempData["msg"] = name + " District Succesfully Activated";
             }
             catch (DomainValidationException dve)
             {
                 ValidationSummary.DomainValidationErrors(dve, ModelState);
                 _log.InfoFormat("Failed to Activate District" + dve.Message);
                 _log.Error("Failed to Activate District" + dve.ToString());
                 TempData["msg"] = dve.Message;
             }
             catch (Exception ex)
             {
                 TempData["msg"] = ex.Message;
                 _log.Debug("Failed to Activate District" + ex.Message);
                 _log.Error("Failed to Activate District" + ex.ToString());
                 
                 }
             return RedirectToAction("ListDistricts");
         }

         public ActionResult Delete(Guid id)
         {
             try
             {
                 _districtViewModelBuilder.SetAsDeleted(id);
                 _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Delete", "District", DateTime.Now);
                 TempData["msg"] = "District Successfully Deleted";

             }
             catch (Exception ex)
             {
                 TempData["msg"] = ex.Message;
                 _log.Debug("Failed to deleted district" + ex.Message);
                 _log.Error("Failed to deleted district" + ex.ToString());
             }

             return RedirectToAction("ListDistricts");
         }

        [HttpPost]
        public ActionResult ListDistricts(bool? showInactive, int? page, string srch, string distName, int? itemsperpage)
        {
            try
            {
                if (itemsperpage != null)
                {
                    ViewModelBase.ItemsPerPage = itemsperpage.Value;
                }
                string command = srch;
                bool showinactive = false;
                if (showInactive != null)
                    showinactive = (bool)showInactive;
                ViewBag.showInactive = showinactive;
                var ls = _districtViewModelBuilder.Search(distName, showinactive);
                int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
                if (command == "Search")
                {
                    return View(ls.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage));
                }
                else
                {
                    return RedirectToAction("ListDistricts", new { showinactive = showInactive, srch = "Search", distName = "" });
                }
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }
        [HttpPost]
        public ActionResult Provinces(Guid countryId)
        {
            try
            {
            
                var provinces = _provinceViewModelBuilder.GetByCountry(countryId).ToList();
                //if (provinces.Count() < 1)
                //{
                //    return Json(new { ok=true,data="",message="NoData"});
                //}
                //else
                //{
                    return Json(new { ok = true, data = provinces, message = "ok" });
                //}
            }
            catch(Exception exx)
            { 
            return Json(new { ok = false, message = exx.Message });
            }
        }
        [HttpPost]
        public ActionResult CountryList()
        {
            try
            {
               
                var countries = _countryViewModelBuilder.GetAll().ToList();
              
                return Json(new { ok = true, data = countries, message = "ok" });
            }
            catch (Exception exx)
            {
                return Json(new { ok = false, message = exx.Message });
            }
        }
 
    }
}
