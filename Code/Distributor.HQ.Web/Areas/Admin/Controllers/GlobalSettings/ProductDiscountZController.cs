using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Distributr.HQ.Lib.Paging;
using System.Web.Mvc;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Products;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Admin.Product;
using Distributr.HQ.Lib.Validation;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Transaction;

namespace Distributr.HQ.Web.Areas.Admin.Controllers.GlobalSettings
{
    [Authorize ]
    public class ProductDiscountZController : Controller
    { 
        ZIProductDiscountViewModelBuilder _productDiscountViewModelBuilder;
        IAuditLogViewModelBuilder _auditLogViewModelBuilder;
        public ProductDiscountZController(ZIProductDiscountViewModelBuilder productDiscountViewModelBuilder,IAuditLogViewModelBuilder auditLogViewModelBuilder)
        {
            _productDiscountViewModelBuilder = productDiscountViewModelBuilder;
            _auditLogViewModelBuilder = auditLogViewModelBuilder;
        }
        public ActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListDiscounts(bool? showInactive, int? page, int? itemsperpage)
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
                var ls = _productDiscountViewModelBuilder.GetAll(showinactive);
                int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
                return View(ls.ToPagedList(currentPageIndex,  ViewModelBase.ItemsPerPage));
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }
        [HttpPost]
        public ActionResult ListDiscounts(bool? showInactive, int? page,string srch,string discount, int? itemsperpage)
        {
            try
            {
                if (itemsperpage != null)
                {
                    ViewModelBase.ItemsPerPage = itemsperpage.Value;
                }
                bool showinactive = false;
                string command = srch;
                if (showInactive != null)
                    showinactive = (bool)showInactive;
                ViewBag.showInactive = showinactive;
                var ls = _productDiscountViewModelBuilder.Search(discount, showinactive);
                int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
                if (command == "Search")
                {
                    return View(ls.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage));
                }
                else
                {
                    return RedirectToAction("ListDiscounts", new { srch = "Search", showinactive = showInactive, discount = "" });
                }
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }

        }
        [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListDiscountItems(Guid id, int? page, int? itemsperpage)
        {
            try
            {
                if (itemsperpage != null)
                {
                    ViewModelBase.ItemsPerPage = itemsperpage.Value;
                }
                ProductDiscountViewModel pdvm = _productDiscountViewModelBuilder.Get(id);
                pdvm.CurrentPage = 1;
                if (page.HasValue)
                    pdvm.CurrentPage = page.Value;
                return View(pdvm);
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }
        [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult CreateDiscountItems(Guid id)
        {
            try
            {

                ProductDiscountViewModel pd = new ProductDiscountViewModel { Id = id };
                return View(pd);
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }
        [HttpPost]
        public ActionResult CreateDiscountItems(ProductDiscountViewModel pdvm)
        { 
            try
            {
                Guid id = pdvm.Id;
                decimal rate = pdvm.DiscountRate/100;
                DateTime effectiveDate = pdvm.EffectiveDate;
                DateTime endDate = pdvm.EndDate;
                _productDiscountViewModelBuilder.AddDiscountItem(id, rate, effectiveDate, endDate);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Create", "Product Discount Item", DateTime.Now);
                return RedirectToAction("ListDiscountItems", new { @id = pdvm.Id });
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
        [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult CreateDiscounts()
        {
            ViewBag.ProductList = _productDiscountViewModelBuilder.ProductList();
            ViewBag.TierList = _productDiscountViewModelBuilder.TierList();
            return View("CreateDiscounts", new ProductDiscountViewModel());
        }
        [HttpPost]
        public ActionResult CreateDiscounts(ProductDiscountViewModel pdvm)
        {
            ViewBag.ProductList = _productDiscountViewModelBuilder.ProductList();
            ViewBag.TierList = _productDiscountViewModelBuilder.TierList();
            try
            {
                pdvm.Id = Guid.NewGuid();
                pdvm.DiscountRate = pdvm.DiscountRate/100;
                _productDiscountViewModelBuilder.Save(pdvm);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Create", "Product Discount", DateTime.Now);
                return RedirectToAction("ListDiscounts");
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
        public ActionResult EditDiscounts(Guid id)
        {
            ViewBag.ProductList = _productDiscountViewModelBuilder.ProductList();
            ViewBag.TierList = _productDiscountViewModelBuilder.TierList();
            try
            {
                ProductDiscountViewModel pdvm = _productDiscountViewModelBuilder.Get(id);
                return View(pdvm);
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }
        [HttpPost]
        public ActionResult EditDiscounts(ProductDiscountViewModel pdvm)
        { 
            try
            {
                pdvm.DiscountRate = pdvm.DiscountRate/100;
                _productDiscountViewModelBuilder.Save(pdvm);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Edit", "Product Discount", DateTime.Now);
                return RedirectToAction("ListDiscounts");
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
        public ActionResult Deactivate(Guid id)
        {

            try
            {
                _productDiscountViewModelBuilder.SetInactive(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Deactivate", "Product Discount", DateTime.Now);
                TempData["msg"] = "Successfully Deactivated";



            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;

            }
            return RedirectToAction("ListDiscounts");
        }
    }
}
