using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.CostCenter;
using Distributr.HQ.Lib.Paging;
using Distributr.HQ.Lib.Validation;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Admin.CostCenter;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Transaction;

namespace Distributr.HQ.Web.Areas.Admin.Controllers.CostCenter
{
    [Authorize ]
    public class ProvinceController : Controller
    { 
        IProvinceViewModelBuilder _ProvinceViewModelBuilder;
        IAuditLogViewModelBuilder _auditLogViewModelBuilder;
        public ProvinceController(IProvinceViewModelBuilder provinceViewModelBuilder, IAuditLogViewModelBuilder auditLogViewModelBuilder)
        {
            _ProvinceViewModelBuilder = provinceViewModelBuilder;
            _auditLogViewModelBuilder = auditLogViewModelBuilder;
        }
        public ActionResult Index()
        {
            return View();
        }
         [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListProvinces(bool? showInactive, int itemsperpage = 10, int page = 1, string srchparam = "")
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
                ViewBag.srchparam = srchparam;

                var currentPageIndex = page - 1 < 0 ? 0 : page - 1;
                var take = itemsperpage;
                var skip = currentPageIndex * take;
                var query = new QueryStandard { Name = srchparam, ShowInactive = showinactive, Skip = skip, Take = take };
                var ls = _ProvinceViewModelBuilder.Query(query);
                var total = ls.Count;
                var data = ls.Data.ToList();


                return View(data.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage, total));
           
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }

        }
         [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult CreateProvince()
        {
            ViewBag.CountryList = _ProvinceViewModelBuilder.Country();
            return View("CreateProvince", new ProvinceViewModel());
        }
         public ActionResult ProvinceDetails(Guid Id)
        {
            ProvinceViewModel province = _ProvinceViewModelBuilder.Get(Id);
            return View(province);
        }
         [Authorize(Roles = "RoleModifyMasterData")]
        public ActionResult EditProvince(Guid Id)
        {
            ViewBag.CountryList = _ProvinceViewModelBuilder.Country();
            try
            {
                ProvinceViewModel province = _ProvinceViewModelBuilder.Get(Id);
                return View(province);
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }
         public ActionResult DeActivate(Guid Id)
        {
            try
            {
                _ProvinceViewModelBuilder.SetInactive(Id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Deactivate", "Province", DateTime.Now);
                TempData["msg"] = "Successfully Deactivated";

            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve,ModelState);
                TempData["msg"] = dve.Message;
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;

            }
            return RedirectToAction("ListProvinces");
        }
        public ActionResult Activate(Guid id, string name)
        {
            try
            {
                _ProvinceViewModelBuilder.SetActive(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name,"Activate","Province",DateTime.Now);
                TempData["msg"] = name + " Successfully Activated";
            }
                catch(DomainValidationException dve)
                {
                    ValidationSummary.DomainValidationErrors(dve,ModelState);
                    TempData["msg"] = dve.Message;
                }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
            }
            return RedirectToAction("ListProvinces");
        }

        public ActionResult Delete(Guid id, string name)
        {
            try
            {
                _ProvinceViewModelBuilder.SetAsDeleted(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Delete", "Province", DateTime.Now);
                TempData["msg"] = name + " Successfully Deleted";
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
            return RedirectToAction("ListProvinces");
        }


        public JsonResult Owner(string blogName)
        {
            IList<ProvinceViewModel> province = _ProvinceViewModelBuilder.GetAll(true);
            return Json(province);
        }
        [HttpPost]
        public ActionResult CreateProvince(ProvinceViewModel pvm)
        {
            ViewBag.CountryList = _ProvinceViewModelBuilder.Country();
            try
            {
                pvm.Id = Guid.NewGuid();
                _ProvinceViewModelBuilder.Save(pvm);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Create", "Province", DateTime.Now);
                TempData["msg"] = "Province Successfully Created";
                return RedirectToAction("ListProvinces");
            }

            catch (DomainValidationException dve)
            {

               ValidationSummary.DomainValidationErrors(dve,ModelState );
               // ViewBag.msg = msg;
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;

                return View();
            }
        }
        [HttpPost]
        public ActionResult EditProvince(ProvinceViewModel pvm)
        {
            ViewBag.CountryList = _ProvinceViewModelBuilder.Country();
            try{
            _ProvinceViewModelBuilder.Save(pvm);
            _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Edit", "Province", DateTime.Now);
            TempData["msg"] = "Province Successfully Edited";
            return RedirectToAction("ListProvinces");
             }

            catch (DomainValidationException dve)
            {

              ValidationSummary.DomainValidationErrors(dve, ModelState);
                //ViewBag.msg = msg;
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;

                return View();
            }
        }
        
    }
}
