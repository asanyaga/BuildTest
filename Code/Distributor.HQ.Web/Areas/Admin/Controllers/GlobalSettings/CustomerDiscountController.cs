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
     [Authorize]
    public class CustomerDiscountController : Controller
    { 
        IAuditLogViewModelBuilder _auditLogViewModelBuilder;
        ICustomerDiscountViewModelBuilder _customerDiscountViewModelBuilder;
        public CustomerDiscountController(ICustomerDiscountViewModelBuilder customerDiscountViewModelBuilder,IAuditLogViewModelBuilder auditLogViewModelBuilder)
        {
            _customerDiscountViewModelBuilder = customerDiscountViewModelBuilder;
            _auditLogViewModelBuilder = auditLogViewModelBuilder;
        }
        public ActionResult Index()
        {
            return View();
        }
         [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListCustomerDiscounts(bool? showInactive, int? page, int? itemsperpage)
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
                var ls = _customerDiscountViewModelBuilder.GetAll(showinactive);
                int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
                return View(ls.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage));
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }
        [HttpPost]
        public ActionResult ListCustomerDiscounts(bool? showInactive, int? page, string srch,string discount, int? itemsperpage)
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
                var ls = _customerDiscountViewModelBuilder.Search(discount, showinactive);
                int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
                if (command == "Search")
                {
                    return View(ls.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage));
                }
                else
                {
                    return RedirectToAction("ListCustomerDiscounts", new { srch = "Search", showinactive = showInactive, discount = "" });
                }
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
            ViewBag.ProductList = _customerDiscountViewModelBuilder.ProductList();
            ViewBag.OutletList = _customerDiscountViewModelBuilder.OutletList();
            return View("CreateDiscounts",new CustomerDiscountViewModel());
        }
        [HttpPost]
        public ActionResult CreateDiscounts(CustomerDiscountViewModel cdvm)
        {
            ViewBag.ProductList = _customerDiscountViewModelBuilder.ProductList();
            ViewBag.OutletList = _customerDiscountViewModelBuilder.OutletList();
            try
            {
                cdvm.id = Guid.NewGuid();
                cdvm.discountRate = cdvm.discountRate/100;
                _customerDiscountViewModelBuilder.Save(cdvm);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Create", "Customer Discount", DateTime.Now);
                return RedirectToAction("ListCustomerDiscounts");
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
            ViewBag.ProductList = _customerDiscountViewModelBuilder.ProductList();
            ViewBag.OutletList = _customerDiscountViewModelBuilder.OutletList();
            try
            {
                CustomerDiscountViewModel cdvm = _customerDiscountViewModelBuilder.Get(id);
                return View(cdvm);
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }
        [HttpPost]
        public ActionResult EditDiscounts(CustomerDiscountViewModel cdvm)
        {
            ViewBag.ProductList = _customerDiscountViewModelBuilder.ProductList();
            ViewBag.OutletList = _customerDiscountViewModelBuilder.OutletList();
        try
        {
            cdvm.discountRate = cdvm.discountRate/100;
            _customerDiscountViewModelBuilder.Save(cdvm);
            _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Edit", "Customer Discount", DateTime.Now);
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
         [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListDiscountItems(Guid id, int? itemsperpage)
        {
                if (itemsperpage != null)
                {
                    ViewModelBase.ItemsPerPage = itemsperpage.Value;
                }
            CustomerDiscountViewModel pdvm = _customerDiscountViewModelBuilder.Get(id);
            return View(pdvm);
        }
         [Authorize(Roles = "RoleAddMasterData")]
         public ActionResult CreateDiscountItems(Guid id)
        {
            ViewBag.ProductList = _customerDiscountViewModelBuilder.ProductList();
            CustomerDiscountViewModel pd = new CustomerDiscountViewModel { id = id };
            return View(pd);
        }
        [HttpPost]
        public ActionResult CreateDiscountItems(CustomerDiscountViewModel cdvm)
        {
          
            try
            {
                Guid id = cdvm.id;
                decimal rate = cdvm.discountRate/100;
                Guid productId = cdvm.ProductId;
                DateTime dt = cdvm.effectiveDate;
                _customerDiscountViewModelBuilder.AddCutomerDiscount(id, rate,dt);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Create", "Customer Discount Items", DateTime.Now);
                return RedirectToAction("ListDiscountItems", new { @id = cdvm.id });
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
        public ActionResult Deactivate(Guid id)
        {

            try
            {
                _customerDiscountViewModelBuilder.SetInactive(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Deactivate", "Customer Discount", DateTime.Now);
                TempData["msg"] = "Successfully Deactivated";

            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                TempData["msg"] = dve.Message;

            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                ViewBag.msg = ex.Message;

            }
            return RedirectToAction("ListCustomerDiscounts");
        }
    }
}
