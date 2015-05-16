using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.Core.Security;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.Paging;
using Distributr.HQ.Lib.Validation;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Discounts;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Transaction;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Admin.Discounts;

namespace Distributr.HQ.Web.Areas.Admin.Controllers.Discounts
{
    [Authorize]
    public class ProductDiscountController : Controller
    { 
        IProductDiscountViewModelBuilder _productDiscountViewModelBuilder;
        IAuditLogViewModelBuilder _auditLogViewModelBuilder;
        public ProductDiscountController(IProductDiscountViewModelBuilder productDiscountViewModelBuilder, IAuditLogViewModelBuilder auditLogViewModelBuilder)
        {
            _productDiscountViewModelBuilder = productDiscountViewModelBuilder;
            _auditLogViewModelBuilder = auditLogViewModelBuilder;
        }

        [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListDiscounts(bool? showInactive, string srchParam = "", int page = 1, int itemsperpage = 10)
        {
            try
            {
                var user = (CustomIdentity)this.User.Identity;
                Guid? supplierId = user.SupplierId != null ? user.SupplierId : (Guid?) null;
                
                if (itemsperpage != null)
                {
                    ViewModelBase.ItemsPerPage = itemsperpage;
                }
                bool showinactive = false;
                if (showInactive != null)
                    showinactive = (bool)showInactive;
                ViewBag.showInactive = showinactive;
                ViewBag.msg = null;
                if (TempData["msg"] != null)
                {
                    ViewBag.msg = TempData["msg"].ToString();
                    TempData["msg"] = null;
                }
                ViewBag.srchText = srchParam;

                int currentPageIndex = page < 0 ? 0 : page - 1;
                int take = itemsperpage;
                int skip = currentPageIndex*take;

                var query = new QueryStandard() {ShowInactive = showinactive, Skip = skip, Take = take, Name = srchParam, SupplierId = supplierId};
                var ls = _productDiscountViewModelBuilder.Query(query);
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
        public ActionResult CreateDiscount()
        {
            ViewBag.DiscountId = null;
            var viewmodel = new ProductDiscountViewModel();
            viewmodel.EffectiveDate = DateTime.Today;
            viewmodel.EndDate = DateTime.Today.AddDays(1);
            ViewBag.ProductList = _productDiscountViewModelBuilder.ProductList()
                .OrderBy(x => x.Value)
                .ToDictionary(x => x.Key, x => x.Value); 
            ViewBag.TierList = _productDiscountViewModelBuilder.TierList()
                .OrderBy(x => x.Value)
                .ToDictionary(x => x.Key, x => x.Value); 
            return View("CreateDiscount", viewmodel);
        }

        [HttpPost]
        public ActionResult CreateDiscount(ProductDiscountViewModel pdvm)
        {
            ViewBag.ProductList = _productDiscountViewModelBuilder.ProductList();
            ViewBag.TierList = _productDiscountViewModelBuilder.TierList();
            try
            {
                if (pdvm != null && pdvm.DiscountRate > 0)
                {
                  
                

                _productDiscountViewModelBuilder.ThrowIfExists(pdvm);
                pdvm.DiscountRate = pdvm.DiscountRate/100;
                _productDiscountViewModelBuilder.Save(pdvm);
                TempData["msg"] = "Discount Successfully Created";
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Create", "Product Discount", DateTime.Now);
                }
                else
                {
                   
                    return View(pdvm); 
                }
                return RedirectToAction("ListDiscounts");
               
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                return View(pdvm);
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View(pdvm);
            }
        }

        public ActionResult EditDiscount(Guid id)
        {
            ViewBag.DiscountId = null;
            var viewmodel = new ProductDiscountViewModel();
            var discountId = id;
            viewmodel = _productDiscountViewModelBuilder.Get(discountId);
            ViewBag.DiscountId = discountId;
            viewmodel.DiscountRate = viewmodel.DiscountRate * 100;
            ViewBag.ProductList = _productDiscountViewModelBuilder.ProductList()
                .OrderBy(x => x.Value)
                .ToDictionary(x => x.Key, x => x.Value);
            ViewBag.TierList = _productDiscountViewModelBuilder.TierList()
                .OrderBy(x => x.Value)
                .ToDictionary(x => x.Key, x => x.Value);
            return View("EditDiscount", viewmodel);
        }

        [HttpPost]
        public ActionResult EditDiscount(ProductDiscountViewModel pdvm)
        {
            ViewBag.ProductList = _productDiscountViewModelBuilder.ProductList();
            ViewBag.TierList = _productDiscountViewModelBuilder.TierList();
            try
            {
                pdvm.DiscountRate = pdvm.DiscountRate / 100;
                _productDiscountViewModelBuilder.Save(pdvm);
                TempData["msg"] = "Discount Successfully Edited";
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Edit", "Product Discount", DateTime.Now);
                return RedirectToAction("ListDiscounts");
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                return View(pdvm);
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View(pdvm);
            }
        }

        [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListDiscountItems(Guid id, int? page, int? itemsperpage)
        {
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            try
            {
                if (itemsperpage != null)
                {
                    ViewModelBase.ItemsPerPage = itemsperpage.Value;
                }
                List<ProductDiscountViewModel.ProductDiscountItemViewModel> pdvm = _productDiscountViewModelBuilder.GetProductDiscountItem(id).ToList();
                ViewBag.ProductDiscountId = id;
                DisplayProductName(id);
                ViewBag.msg = null;
                if (TempData["msg"] != null)
                {
                    ViewBag.msg = TempData["msg"].ToString();
                    TempData["msg"] = null;
                }
                return View(pdvm.ToPagedList(currentPageIndex,  ViewModelBase.ItemsPerPage));
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
                ViewBag.ProductDiscountId = id;
                DisplayProductName(id);
                ProductDiscountViewModel.ProductDiscountItemViewModel pd = new ProductDiscountViewModel.ProductDiscountItemViewModel { ProductDiscountId = id };
                return View(pd);
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }

        private void DisplayProductName(Guid id)
        {
            ViewBag.ProductName = _productDiscountViewModelBuilder.GetProductName(id);
        }

        [HttpPost]
        public ActionResult CreateDiscountItems(ProductDiscountViewModel.ProductDiscountItemViewModel pdvm)
        {
            try
            {
                Guid id = pdvm.ProductDiscountId;
                ViewBag.ProductDiscountId = id;
                decimal rate = pdvm.DiscountRate / 100;
                DateTime effectiveDate = pdvm.EffectiveDate;
                DateTime endDate = pdvm.EndDate;
                _productDiscountViewModelBuilder.AddDiscountItem(id, rate, effectiveDate, endDate,pdvm.IsByQuantity,pdvm.Quantity);
                TempData["msg"] = "Discount lineitem Successfully Created";
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Create", "Product Discount Item", DateTime.Now);
                return RedirectToAction("ListDiscountItems", new { @id = id });
            }
            catch (DomainValidationException dve)
            {
                DisplayProductName(pdvm.ProductDiscountId);
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                return View();
            }
            catch (Exception ex)
            {
                DisplayProductName(pdvm.ProductDiscountId);
                ViewBag.msg = ex.Message;
                return View();
            }
        }

        public ActionResult Delete(Guid id)
        {
            try
            {
                _productDiscountViewModelBuilder.SetDeleted(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Delete", "Product Discount", DateTime.Now);
                TempData["msg"] = "Successfully Deleted";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;

            }
            return RedirectToAction("ListDiscounts");
        }

        public ActionResult DeactivateLineItem(Guid id, Guid pdId)
        {
            try
            {
                _productDiscountViewModelBuilder.DeacivateLineItem(id);
                TempData["msg"] = "Successfully Deactivated";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;

            }
            return RedirectToAction("ListDiscountItems", new {id = pdId});
        }

    }
}
