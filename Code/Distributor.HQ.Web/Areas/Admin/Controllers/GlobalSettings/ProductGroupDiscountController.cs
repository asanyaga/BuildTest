using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.Paging;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Products;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Admin.Product;
using Distributr.HQ.Lib.Validation;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Transaction;

namespace Distributr.HQ.Web.Areas.Admin.Controllers.GlobalSettings
{
    public class ProductGroupDiscountController : Controller
    { 
        IProductGroupDiscountViewModelBuilder _productGroupDiscountViewModelBuilder;
        IAuditLogViewModelBuilder _auditLogViewModelBuilder;
        public ProductGroupDiscountController(IProductGroupDiscountViewModelBuilder productGroupDiscountViewModelBuilder,IAuditLogViewModelBuilder auditLogViewModelBuilder)
        {
            _productGroupDiscountViewModelBuilder = productGroupDiscountViewModelBuilder;
            _auditLogViewModelBuilder = auditLogViewModelBuilder;
        }
        [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListProductGroupDiscounts(bool? showInactive, int? page, Guid? discountGroup, string discountGroupName, int? itemsperpage)
        {
            try
            {
                if (itemsperpage != null)
                {
                    ViewModelBase.ItemsPerPage = itemsperpage.Value;
                }
                if (discountGroup.HasValue)
                    ViewBag.discountGroup = discountGroup.Value;
                ViewBag.discountGroupName = discountGroupName;
                bool showinactive = false;
                if (showInactive != null)
                    showinactive = (bool)showInactive;
                ViewBag.showInactive = showinactive;
                if (TempData["msg"] != null)
                {
                    ViewBag.msg = TempData["msg"].ToString();
                    TempData["msg"] = null;
                }

                var ls = _productGroupDiscountViewModelBuilder.GetByDiscountGroup(discountGroup.Value, showinactive);
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
        public ActionResult ListProductGroupDiscounts(bool? showInactive, int? page, string srch, string prdGroupDiscount, Guid? discountGroup, int? itemsperpage)
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
                var ls = _productGroupDiscountViewModelBuilder.Search(prdGroupDiscount, showinactive);
                int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
                if (command == "Search")
                {
                    return View(ls.ToPagedList(currentPageIndex,  ViewModelBase.ItemsPerPage));
                }
                else
                {
                    return RedirectToAction("ListProductGroupDiscounts", new { srch = "Search", showinactive = showInactive, prdGroupDiscount = "", discountGroup = discountGroup });
                }
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }
        [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult CreateDiscount(Guid discountGroup, string discountGroupName)
        {
            ViewBag.DiscountId = null;
            ViewBag.ProductList = _productGroupDiscountViewModelBuilder.ProductListWithoutReturnables()
                .OrderBy(x => x.Value)
                .ToDictionary(x => x.Key, x => x.Value);
            ViewBag.DiscountGroupList = _productGroupDiscountViewModelBuilder.DiscountGroupList();
            var pgdvm = new ProductDiscountGroupViewModel
            {
                DiscountGroup = discountGroup,
                DiscountGroupName = discountGroupName
            };
            pgdvm.EffectiveDate = DateTime.Today;
            pgdvm.EndDate = DateTime.Today.AddDays(1);
            return View(pgdvm);
        }

        [HttpPost]
        public ActionResult CreateDiscount(ProductDiscountGroupViewModel pgdvm)
        {
            ViewBag.ProductList = _productGroupDiscountViewModelBuilder.ProductList();
            ViewBag.DiscountGroupList = _productGroupDiscountViewModelBuilder.DiscountGroupList();
            try
            {
                _productGroupDiscountViewModelBuilder.ThrowIfExists(pgdvm);
                pgdvm.discountRate = pgdvm.discountRate/100;
              
                _productGroupDiscountViewModelBuilder.Save(pgdvm);
                TempData["msg"] = "Discount Successfully Created";
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Create", "Promotion Group Discount", DateTime.Now);
                return RedirectToAction("ListProductGroupDiscounts", new { discountGroup=pgdvm.DiscountGroup, discountGroupName = pgdvm.DiscountGroupName});
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                pgdvm = new ProductDiscountGroupViewModel
                {
                    DiscountGroup = pgdvm.DiscountGroup,
                    DiscountGroupName=pgdvm.DiscountGroupName
                };
                return View(pgdvm);
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                pgdvm = new ProductDiscountGroupViewModel
                {
                    DiscountGroup = pgdvm.DiscountGroup,
                    DiscountGroupName=pgdvm.DiscountGroupName
                };
                return View(pgdvm);
            }
        }

        [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult EditDiscount(Guid id, Guid discountGroup, string discountGroupName)
        {
            ViewBag.DiscountId = null;
            var productList = _productGroupDiscountViewModelBuilder.ProductListWithoutReturnables()
                .OrderBy(x => x.Value)
                .ToDictionary(x => x.Key, x => x.Value);
            ViewBag.ProductList = productList;
            ViewBag.DiscountGroupList = _productGroupDiscountViewModelBuilder.DiscountGroupList();
           
            var discountId = id;
            var groupDiscountItem = _productGroupDiscountViewModelBuilder.Get(discountId);
            if (groupDiscountItem == null)
           {
                groupDiscountItem = new ProductDiscountGroupViewModel
               {
                   DiscountGroup = discountGroup,
                   DiscountGroupName = discountGroupName
               };
               
             
            }
            groupDiscountItem.discountRate = groupDiscountItem.discountRate * 100;
            groupDiscountItem.IsByQuantity = groupDiscountItem.Quantity>0;
            ViewBag.DiscountId = discountId;

            return View(groupDiscountItem);
        }

        [HttpPost]
        public ActionResult EditDiscount(ProductDiscountGroupViewModel pgdvm)
        {
            ViewBag.ProductList = _productGroupDiscountViewModelBuilder.ProductList();
            ViewBag.DiscountGroupList = _productGroupDiscountViewModelBuilder.DiscountGroupList();
            try
            {
                pgdvm.discountRate = pgdvm.discountRate / 100;
                _productGroupDiscountViewModelBuilder.Save(pgdvm);
                TempData["msg"] = "Discount Successfully Edited";
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Edit", "Promotion Group Discount", DateTime.Now);
                return RedirectToAction("ListProductGroupDiscounts", new { discountGroup = pgdvm.DiscountGroup, discountGroupName = pgdvm.DiscountGroupName });
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                pgdvm = new ProductDiscountGroupViewModel
                {
                    DiscountGroup = pgdvm.DiscountGroup,
                    DiscountGroupName = pgdvm.DiscountGroupName
                };
                return View(pgdvm);
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                pgdvm = new ProductDiscountGroupViewModel
                {
                    DiscountGroup = pgdvm.DiscountGroup,
                    DiscountGroupName = pgdvm.DiscountGroupName
                };
                return View(pgdvm);
            }
        }

        [Authorize(Roles = "RoleModifyMasterData")]
        public ActionResult EditProductGroupDiscount(Guid id, int? discountGroup, string discountGroupName)
        {
            try
            {
                if (discountGroup.HasValue)
                    ViewBag.discountGroup = discountGroup.Value;
                ViewBag.discountGroupName = discountGroupName;

                ViewBag.ProductList = _productGroupDiscountViewModelBuilder.ProductList();
                ViewBag.DiscountGroupList = _productGroupDiscountViewModelBuilder.DiscountGroupList();
                ProductDiscountGroupViewModel pgdvm = _productGroupDiscountViewModelBuilder.Get(id);
                return View(pgdvm);
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }
        [HttpPost]
        public ActionResult EditProductGroupDiscount(ProductDiscountGroupViewModel pgdvm)
        {
            ViewBag.ProductList = _productGroupDiscountViewModelBuilder.ProductList();
            ViewBag.DiscountGroupList = _productGroupDiscountViewModelBuilder.DiscountGroupList();
            try
            {
                if (pgdvm.Id == null) throw new Exception("FAIL");
                pgdvm.discountRate = pgdvm.discountRate/100;
               // _productGroupDiscountViewModelBuilder.Save(pgdvm, out TODO);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Edit", "Promotion Group Discount", DateTime.Now);
                TempData["msg"] = "Successfully Edited";
                return RedirectToAction("ListProductGroupDiscounts", new { discountGroup = pgdvm.DiscountGroup });
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
        public ActionResult ListProductGroupDiscountItems(Guid id, int? page, int? itemsperpage)
        {
            
                if (itemsperpage != null)
                {
                    ViewModelBase.ItemsPerPage = itemsperpage.Value;
                }
           ProductDiscountGroupViewModel pdvm = _productGroupDiscountViewModelBuilder.Get(id);
           ViewBag.id = pdvm.Id;
           ViewBag.discountGroup = pdvm.DiscountGroup;
           ViewBag.discountGroupName = pdvm.DiscountGroupName;
           ViewBag.productName = pdvm.ProductName;
           ViewBag.productId = pdvm.Product;
           var ls = pdvm.productGroupDiscountItems;
           ViewBag.msg = null;
           if (TempData["msg"] != null)
           {
               ViewBag.msg = TempData["msg"].ToString();
               TempData["msg"] = null;
           }
           int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
           return View(ls.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage));
        }
        [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult CreateProductGroupDiscountItems(Guid id, string discountGroupName, string productName, Guid productId)
        {
            ViewBag.ProductList = _productGroupDiscountViewModelBuilder.ProductList();
            ViewBag.DiscountGroupName = discountGroupName;
            ViewBag.ProductName = productName;
            ViewBag.Id = id;
            ProductDiscountGroupViewModel pd = new ProductDiscountGroupViewModel { Id = id,DiscountGroupName=discountGroupName, ProductName = productName, Product = productId };
            return View(pd);
        }
        [HttpPost]
        public ActionResult CreateProductGroupDiscountItems(ProductDiscountGroupViewModel pgdvm)
        {
            ViewBag.ProductList = _productGroupDiscountViewModelBuilder.ProductList();
            try
            {
                Guid id = pgdvm.Id;
                decimal rate = pgdvm.discountRate/100;
                Guid productId = pgdvm.Product;
                DateTime effectiveDate = pgdvm.EffectiveDate;
                DateTime endDate = pgdvm.EndDate;
                ViewBag.ProductList = _productGroupDiscountViewModelBuilder.ProductList();
                ViewBag.DiscountGroupName = pgdvm.DiscountGroupName;
                ViewBag.ProductName = pgdvm.ProductName;
                ViewBag.Id = id;
                if (effectiveDate.Date < DateTime.Now.Date)
                    throw new Exception("Invalid effective date, must be current or future date. Failed to validate product group discount");
                //vri.Results.Add(new ValidationResult("Invalid effective date, must be current or future date."));
                _productGroupDiscountViewModelBuilder.AddProductGroupDiscount(id, productId, rate, effectiveDate, endDate);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Create", "Promotion Group Discount Items", DateTime.Now);
                TempData["msg"] = "Successfully Created";
                return RedirectToAction("ListProductGroupDiscountItems", new { @id = pgdvm.Id });
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
        public ActionResult Delete(Guid id, Guid discountGroup, string discountGroupName)
        {
            ViewBag.discountGroup = discountGroup;
            ViewBag.discountGroupName = discountGroupName;
            try
            {
                _productGroupDiscountViewModelBuilder.SetInactive(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Delete", "Promotion Group Discount", DateTime.Now);
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
            return RedirectToAction("ListProductGroupDiscounts", new {discountGroup=discountGroup,discountGroupName=discountGroupName });
        }
        public ActionResult DeleteLineItem(Guid id)
        {

            try
            {
                _productGroupDiscountViewModelBuilder.SetLineItemsInactive(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Delete", "Promotion Group Discount Item", DateTime.Now);
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
            return RedirectToAction("ListProductGroupDiscountItems", new { @id=id});
        }
    }
}
