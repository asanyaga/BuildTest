using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using Distributr.Core.Utility;
using Distributr.HQ.Lib.Security;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Admin.CostCenter;
using Distributr.HQ.Lib.Validation;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.CostCenter;
using MvcContrib.Pagination;
using log4net;
using System.Reflection;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Transaction;
using System.Security.Cryptography;
using System.Text;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.Paging;

namespace Distributr.HQ.Web.Areas.Admin.Controllers.GlobalSettings
{
      [CustomAuthorize]
    public class RegionController : Controller
    { 
        IRegionViewModelBuilder _regionViewModelBuilder;
        IProvinceViewModelBuilder _provinceViewModelBuilder;
        IDistrictViewModelBuilder _districtViewModelBuilder;
        IAuditLogViewModelBuilder _auditLogViewModelBuilder;
        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
          private IOutletMainOrderRepository _outletMainOrderRepository;
        public RegionController(IRegionViewModelBuilder regionViewModelBuilder, IProvinceViewModelBuilder provinceViewModelBuilder, IDistrictViewModelBuilder districtViewModelBuilder,IAuditLogViewModelBuilder auditLogViewModelBuilder, IOutletMainOrderRepository outletMainOrderRepository)
        {
            _regionViewModelBuilder = regionViewModelBuilder;
            _provinceViewModelBuilder = provinceViewModelBuilder;
            _districtViewModelBuilder = districtViewModelBuilder;
            _auditLogViewModelBuilder = auditLogViewModelBuilder;
            _outletMainOrderRepository = outletMainOrderRepository;
        }

        public ActionResult Index()
        {
            var query = new QueryOutletOrder();
            query.From = DateTime.Now.AddDays(-10);
             query.To =  query.From.AddDays(20);
             query.OutletId = new Guid("17C666DA-1238-45E3-988F-5A6D14042B5E");
             var ss = _outletMainOrderRepository.PendingApprovalQuery(query);
            return View();

        }
         [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListRegion(Boolean showInactive = false, int page = 1, int itemsperpage = 10, string searchParam = "")
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

                if (TempData["msg"] != null)
                {
                    ViewBag.msg = TempData["msg"].ToString();
                    TempData["msg"] = null;
                }
                ViewBag.searchParam = searchParam;
                var currentPageIndex = page - 1 < 0 ? 0 : page - 1;
                var take = itemsperpage;
                var skip = currentPageIndex * take;
                var query = new QueryStandard { Name = searchParam, ShowInactive = showinactive, Skip = skip, Take = take };
                var ls = _regionViewModelBuilder.Query(query);
                var total = ls.Count;
                var data = ls.Data;
                return View(data.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage,total));
            }
            catch (Exception ex)
            {
                _log.Debug("Failed to list region" + ex.Message);
                _log.Error("Failed to list region" + ex.ToString());
                return View();
            }
        }


        public ActionResult DetailsRegion(Guid id)
        {
            RegionViewModel reg = _regionViewModelBuilder.Get(id);
            return View(reg);
        }


          [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult CreateRegion(string url)
        {
            ViewBag.CountryList = _regionViewModelBuilder.Country();
            ViewBag.ProvinceList = _regionViewModelBuilder.Province();
            ViewBag.DistrictList = _regionViewModelBuilder.District();
            ViewBag.Url = url;
            //string xx =md5Encryptor GetMd5Hash("12345678");
            //  RegionViewModel r=new RegionViewModel();
            //  r.Name=xx;
            return View("CreateRegion",new RegionViewModel() );
        }

          

        [HttpPost]
        public ActionResult CreateRegion(string url,RegionViewModel rvm)
        {
            try
            {
                ViewBag.CountryList = _regionViewModelBuilder.Country();
                ViewBag.ProvinceList = _regionViewModelBuilder.Province();
                ViewBag.DistrictList = _regionViewModelBuilder.District();
                ViewBag.Url = url;
                //if (rvm.ProvinceId == 0 || rvm.ProvinceId.ToString() == "")
                //{
                //    ViewBag.msg2 = "Province is a required ";
                //    return View();
                //}
                //else if (rvm.DistrictId == 0 || rvm.DistrictId.ToString() == "")
                //{
                //    ViewBag.msg2 = "District is required";
                //    return View();
                //}
                //else
                //{
                rvm.Id = Guid.NewGuid();
                    _regionViewModelBuilder.Save(rvm);
                    _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Create", "Region", DateTime.Now);
                    TempData["msg"] = "Region Successfully Created";
                    return RedirectToAction("ListRegion");
                //}
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                _log.Debug("Failed to create region" + dve.Message);
                _log.Error("Failed to create region" + dve.ToString());
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                _log.Debug("Failed to create region" + ex.Message);
                _log.Error("Failed to create region" + ex.ToString());
                return View();
            }
        }


        [Authorize(Roles = "RoleModifyMasterData")]
        public ActionResult EditRegion(Guid id)
        {            ViewBag.CountryList = _regionViewModelBuilder.Country();
            ViewBag.ProvinceList = _regionViewModelBuilder.Province();
            ViewBag.DistrictList = _regionViewModelBuilder.District();
            try
            {
                RegionViewModel regVM = _regionViewModelBuilder.Get(id);

                return View(regVM);
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }



        [HttpPost]
        public ActionResult EditRegion(RegionViewModel rvm)
        {
            try
            {
                
                ViewBag.CountryList = _regionViewModelBuilder.Country();
                ViewBag.ProvinceList = _regionViewModelBuilder.Province();
                ViewBag.DistrictList = _regionViewModelBuilder.District();
                //if (rvm.ProvinceId == 0 || rvm.ProvinceId.ToString() == "")
                //{
                //    ViewBag.msg2 = "Province is a required ";
                //    return View();
                //}
                //else if (rvm.DistrictId == 0 || rvm.DistrictId.ToString() == "")
                //{
                //    ViewBag.msg2 = "District is required";
                //    return View();
                //}
                //else
                //{
                    _regionViewModelBuilder.Save(rvm);
                    _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Edit", "Region", DateTime.Now);
                    TempData["msg"] = "Region Successfully Edited";
                    return RedirectToAction("ListRegion");
                //}
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                _log.Debug("Failed to edit region" + dve.Message);
                _log.Error("Failed to edit region" + dve.ToString());
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                _log.Debug("Failed to edit region" + ex.Message);
                _log.Error("Failed to edit region" + ex.ToString());
                return View();
            }
        }
        public ActionResult DeActivate(Guid id)
        {
            try
            {
                _regionViewModelBuilder.SetInActive(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Deactivate", "Region", DateTime.Now);
                TempData["msg"] = "Successfully Deactivated";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to deactivate region" + ex.Message);
                _log.Error("Failed to deactivate region" + ex.ToString());
            }
            return RedirectToAction("ListRegion");
        }
          public ActionResult Activate(Guid id, string name)
          {
              try
              {
               _regionViewModelBuilder.SetActive(id);
                  _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name,"Activate","Region",DateTime.Now);
                  TempData["msg"] = name + " Succesfully Activated";
              }
              catch (Exception ex)
              {
                  TempData["msg"] = ex.Message;
                  _log.Debug("Failed to Activate region" + ex.Message);
                  _log.Error("Failed to Activate region" + ex.ToString());
                  }
              return RedirectToAction("ListRegion");
          }

          public ActionResult Delete(Guid id, string name)
          {
              try
              {
                  _regionViewModelBuilder.SetAsDeleted(id);
                  _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Delete", "Region", DateTime.Now);
                  TempData["msg"] = name + " Succesfully Deleted";
              }
              catch (Exception ex)
              {
                  TempData["msg"] = ex.Message;
                  _log.Debug("Failed to Delete region" + ex.Message);
                  _log.Error("Failed to Delete region" + ex.ToString());
              }
              return RedirectToAction("ListRegion");
          }



        public JsonResult Owner(string blogName)
        {
            IList<RegionViewModel> region = _regionViewModelBuilder.GetAll(true);
            return Json(region);
        }
        [HttpPost]
        public ActionResult Provinces(Guid countryId)
        {
            try
            {

                var provinces = _provinceViewModelBuilder.GetByCountry(countryId).ToList();
               
                return Json(new { ok = true, data = provinces, message = "ok" });
                
            }
            catch (Exception exx)
            {
                return Json(new { ok = false, message = exx.Message });
            }
        }
        [HttpPost]
        public ActionResult Districts(Guid provinceId)
        {
            try
            {

                var districts = _districtViewModelBuilder.GetByProvince(provinceId).ToList();

                return Json(new { ok = true, data = districts, message = "ok" });

            }
            catch (Exception exx)
            {
                return Json(new { ok = false, message = exx.Message });
            }
        }
    }
}
