using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.Paging;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.ReOrderLevelViewModelBuilders;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Admin.ReOrderLevelViewModel;
using Distributr.HQ.Lib.Validation;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Transaction;

namespace Distributr.HQ.Web.Areas.Admin.Controllers.GlobalSettings
{
   // [Authorize ]
    public class ReOrderLevelController : Controller
    {
        IReOrderLevelViewModelBuilder _reOrderLevelBuilder;
        IAuditLogViewModelBuilder _auditLogViewModelBuilder;
        public ReOrderLevelController(IReOrderLevelViewModelBuilder reOrderLevelBuilder,IAuditLogViewModelBuilder auditLogViewModelBuilder)
        {
            _reOrderLevelBuilder = reOrderLevelBuilder;
            _auditLogViewModelBuilder = auditLogViewModelBuilder;
        }
        public ActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListReOrderLevels(bool showInactive = false, int page=1 , int itemsperpage=10, string srchParam="")
        {
            try
            {
                if (itemsperpage != null)
                {
                    ViewModelBase.ItemsPerPage = itemsperpage;
                }
                if (TempData["msg"] != null)
                {
                    ViewBag.msg = TempData["msg"].ToString();
                    TempData["msg"] = null;
                }

                bool showinactive = false;
                if (showInactive != null)
                    showinactive = (bool)showInactive;
                ViewBag.showInactive = showinactive;
                ViewBag.srchParam = srchParam;

                var currentPageIndex = page - 1 < 0 ? 0 : page - 1;
                int take = itemsperpage;
                int skip = currentPageIndex * take;
                var query = new QueryStandard { Name = srchParam, ShowInactive = showinactive, Skip = skip, Take = take };
                var ls = _reOrderLevelBuilder.Query(query);
                var total = ls.Count;
                var data = ls.Data;


                return View(data .ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage,total));
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }
      
       [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult CreateReOrderLevel()
        {
            ViewBag.DistributorList = _reOrderLevelBuilder.GetDistributor();
            ViewBag.ProductList = _reOrderLevelBuilder.GetProducts();
            return View("CreateReOrderLevel",new ReOrderLevelViewModel());
        }
        [HttpPost]
        public ActionResult CreateReOrderLevel(ReOrderLevelViewModel rolvm)
        {
            try
            {
                rolvm.Id = Guid.NewGuid();
                ViewBag.DistributorList = _reOrderLevelBuilder.GetDistributor();
                ViewBag.ProductList = _reOrderLevelBuilder.GetProducts();
                _reOrderLevelBuilder.Save(rolvm);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Create", "ReOrder Level", DateTime.Now);
                TempData["msg"] = "Reorder Level Successfully Created";
                return RedirectToAction("ListReOrderLevels");
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }
        [Authorize(Roles = "RoleModifyMasterData")]
        public ActionResult EditReOrderLevel(Guid id)
        {
            ViewBag.DistributorList = _reOrderLevelBuilder.GetDistributor();
            ViewBag.ProductList = _reOrderLevelBuilder.GetProducts();
            try
            {
                ReOrderLevelViewModel rolvm = _reOrderLevelBuilder.GetById(id);
                return View(rolvm);
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }
        [HttpPost]
        public ActionResult EditReOrderLevel(ReOrderLevelViewModel rolvm)
        {
            try
            {
                _reOrderLevelBuilder.Save(rolvm);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Edit", "ReOrder Level", DateTime.Now);
                TempData["msg"] = "Reorder Level Successfully Edited";
                return RedirectToAction("ListReOrderLevels");
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve,ModelState);
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }

        public ActionResult DeActivate(Guid id)
        {
            try
            {
                _reOrderLevelBuilder.SetInactive(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Deactivate", "ReOrder Level", DateTime.Now);
                TempData["msg"] = "Successfully Deactivated";

            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;

            }

            return RedirectToAction("ListReOrderLevels");
        }

        public ActionResult Activate(Guid id)
        {
            try
            {
                _reOrderLevelBuilder.Activate(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Activate", "Reorder level", DateTime.Now);
                TempData["msg"] = "Successfully Activated";
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
            }

            return RedirectToAction("ListReOrderLevels");
        }

        public ActionResult Delete(Guid id)
        {
            try
            {
                _reOrderLevelBuilder.Delete(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Delete", "Reorder level", DateTime.Now);
                TempData["msg"] = "Successfully deleted";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
            }

            return RedirectToAction("ListReOrderLevels");
        }
    }
}
