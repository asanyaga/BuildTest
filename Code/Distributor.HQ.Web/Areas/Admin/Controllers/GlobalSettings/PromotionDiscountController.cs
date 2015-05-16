using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Distributr.Core.Security;
using Distributr.Core.Utility;
using Distributr.HQ.Lib.Paging;
using System.Web.Mvc;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Products;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Admin.Product;
using Distributr.HQ.Lib.Validation;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Transaction;

namespace Distributr.HQ.Web.Areas.Admin.Controllers.GlobalSettings
{
   // [Authorize ]
    public class PromotionDiscountController : Controller
    {
        IPromotionDiscountViewModelBuilder _focDiscountViewModeBuilder;
        IAuditLogViewModelBuilder _auditLogViewModelBuilder;
        public PromotionDiscountController(IPromotionDiscountViewModelBuilder focDiscountViewModeBuilder,IAuditLogViewModelBuilder auditLogViewModelBuilder)
        {
            _focDiscountViewModeBuilder = focDiscountViewModeBuilder;
            _auditLogViewModelBuilder = auditLogViewModelBuilder;
        }
        public ActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListFOCDiscounts(int page = 1, int itemsperpage = 10, string searchText = "")
        {
            try
            {
                var user = (CustomIdentity)this.User.Identity;
                Guid? supplierId = user.SupplierId != null ? user.SupplierId : (Guid?) null;
                if (itemsperpage != null)
                {
                    ViewModelBase.ItemsPerPage = itemsperpage;
                }
                ViewBag.msg = null;
                if (TempData["msg"] != null)
                {
                    ViewBag.msg = TempData["msg"].ToString();
                    TempData["msg"] = null;
                }
                ViewBag.searchParam = searchText;

                int currentPageIndex = page < 0 ? 0 : page - 1;
                int take = itemsperpage;
                int skip = currentPageIndex*itemsperpage;

                var query = new QueryStandard()
                {
                    Name = searchText,
                    Skip = skip,
                    Take = take,
                    SupplierId = supplierId
                };

                var ls = _focDiscountViewModeBuilder.Query(query);

                var data = ls.Data;
                var count = ls.Count;

                return View(data.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage, count));
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }
        
        [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult CreatFOCDiscount()
        {
            ViewBag.ProductList = _focDiscountViewModeBuilder.ProductList()
                .OrderBy(x => x.Value)
                .ToDictionary(x => x.Key, x => x.Value); 
            return View("CreatFOCDiscount",new PromotionDiscountViewModel());
        }
        [HttpPost]
        public ActionResult CreatFOCDiscount(PromotionDiscountViewModel focvm)
        {
            ViewBag.ProductList = _focDiscountViewModeBuilder.ProductList();
            try
            {
                focvm.Id = Guid.NewGuid();
                focvm.DiscountRate = focvm.DiscountRate/100;
                //_focDiscountViewModeBuilder.Save(focvm, out TODO);
                TempData["msg"] = "Discount Successfully Created";
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Create", "Promotion Discount", DateTime.Now);
                return RedirectToAction("ListFOCDiscounts");
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                return View();
            }
            catch (Exception exx)
            {
                ValidationSummary.DomainValidationErrors(exx.Message, ModelState);
                return View(focvm);
            }
        }

        [Authorize(Roles = "RoleModifyMasterData")]
        public ActionResult CreateDiscount()
        {
            var viewModel = new PromotionDiscountViewModel();
            viewModel.EffectiveDate = DateTime.Today;
            viewModel.EndDate = DateTime.Today.AddDays(1);
            ViewBag.ProductList = _focDiscountViewModeBuilder.ProductList()
                .OrderBy(x => x.Value)
                .ToDictionary(x => x.Key, x => x.Value);
            return View("CreateDiscount", viewModel);
        }
        [HttpPost]
        public ActionResult CreateDiscount(PromotionDiscountViewModel focvm)
        {
            ViewBag.ProductList = _focDiscountViewModeBuilder.ProductList();
            try
            {
                _focDiscountViewModeBuilder.ThrowIfExists(focvm);
                focvm.DiscountRate = focvm.DiscountRate / 100;
                _focDiscountViewModeBuilder.Save(focvm);
                TempData["msg"] = "Discount Successfully Created";
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Create", "Promotion Discount", DateTime.Now);
                return RedirectToAction("ListFOCDiscounts");
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                return View(focvm);
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View(focvm);
            }
        }

        [Authorize(Roles = "RoleModifyMasterData")]
        public ActionResult EditDiscount(Guid id)
        {
            ViewBag.DiscountId = null;
            var viewModel = new PromotionDiscountViewModel();
            var discountId = id;
            viewModel = _focDiscountViewModeBuilder.Get(discountId);
            ViewBag.ProductList = _focDiscountViewModeBuilder.ProductList()
                .OrderBy(x => x.Value)
                .ToDictionary(x => x.Key, x => x.Value);
            return View("EditDiscount", viewModel);
        }
        [HttpPost]
        public ActionResult EditDiscount(PromotionDiscountViewModel focvm)
        {
            ViewBag.ProductList = _focDiscountViewModeBuilder.ProductList();
            try
            {
                focvm.DiscountRate = focvm.DiscountRate / 100;
                _focDiscountViewModeBuilder.Save(focvm);
                TempData["msg"] = "Discount Successfully Edited";
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Edit", "Promotion Discount", DateTime.Now);
                return RedirectToAction("ListFOCDiscounts");
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                return View(focvm);
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View(focvm);
            }
        }

        [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult CreatFOCDiscountItem(Guid id, string ProductName)
        {
            ViewBag.ProductName = ProductName;
            bindlineitem(); 
            try
            {
                PromotionDiscountViewModel fovm = new PromotionDiscountViewModel
                {
                    Id = id
                };
                return View(fovm);
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }

        private void bindlineitem()
        {
            ViewBag.ProductList = _focDiscountViewModeBuilder.ProductList()
                .OrderBy(x => x.Value)
                .ToDictionary(x => x.Key, x => x.Value);
        }

        [HttpPost]
        public ActionResult CreatFOCDiscountItem(PromotionDiscountViewModel focvm)
        {
            try
            {
                Guid id = focvm.Id;
                Guid freeProduct = focvm.FreeProduct ?? Guid.Empty; //: focvm.FreeProduct;
                int freeQuantity = focvm.FreeOfChargeProductQuantity==null ? 0: focvm.FreeOfChargeProductQuantity.Value;
                int parentProductQuantity = focvm.ParentProductQuantity;
                decimal discountRate = focvm.DiscountRate;
                DateTime effectiveDate = focvm.EffectiveDate;
                DateTime endDate = focvm.EndDate;
                _focDiscountViewModeBuilder.AddFreeOfChargeDiscount(id, parentProductQuantity, freeProduct, freeQuantity, effectiveDate,discountRate, endDate);
                TempData["msg"] = "Discount lineitem Successfully Created";
                return RedirectToAction("ListFOCDiscountItems", new { @id = focvm.Id });
            }
            catch (DomainValidationException dve)
            {
                bindlineitem(); 
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                return View(focvm);
            }
            catch (Exception ex)
            {
                bindlineitem(); 
                ViewBag.msg = ex.Message;
                ValidationSummary.DomainValidationErrors(ex.Message, ModelState);
                return View(focvm);
            }
        }
        [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListFOCDiscountItems(Guid id, string ProductName, int? page, int? itemsperpage)
        {
                if (itemsperpage != null)
                {
                    ViewModelBase.ItemsPerPage = itemsperpage.Value;
                }
            ViewBag.ProductName = ProductName;
            ViewBag.pdid = id;
            PromotionDiscountViewModel fovm = _focDiscountViewModeBuilder.Get(id);
            var ls = fovm.freeOfChargeDiscountItems;
            ViewBag.msg = null;
            if (TempData["msg"] != null)
            {
                ViewBag.msg = TempData["msg"].ToString();
                TempData["msg"] = null;
            }
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            return View(ls.ToPagedList(currentPageIndex,  ViewModelBase.ItemsPerPage));
        }
        public ActionResult Delete(Guid id)
        {

            try
            {
                _focDiscountViewModeBuilder.SetDeleted(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Delete", "Promotion Discount", DateTime.Now);
                TempData["msg"] = "Successfully Deleted";
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
            return RedirectToAction("ListFOCDiscounts");
        }
        public ActionResult DeleteLineItem(Guid id, Guid pdId, string ProductName)
        {
            try
            {
                _focDiscountViewModeBuilder.DeacivateLineItem(id);
                TempData["msg"] = "Successfully Deleted";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;

            }
            return RedirectToAction("ListFOCDiscountItems", new { id = pdId, ProductName = ProductName});
        }

    }
}
