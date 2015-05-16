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
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Products;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Admin.Product;
using Distributr.HQ.Lib.Validation;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Transaction;

namespace Distributr.HQ.Web.Areas.Admin.Controllers.GlobalSettings
{
   // [Authorize]
    public class DiscountGroupController : Controller
    { 
        IDiscountGroupViewModelBuilder _discountGroupViewModelBuilder;
        IProductGroupDiscountViewModelBuilder _productGroupDiscountViewModelBuilder;
        IAuditLogViewModelBuilder _auditLogViewModelBuilder;

        public DiscountGroupController(IDiscountGroupViewModelBuilder discountGroupViewModelBuilder, IProductGroupDiscountViewModelBuilder productGroupDiscountViewModelBuilder,IAuditLogViewModelBuilder auditLogViewModelBuilder)
        {
            _discountGroupViewModelBuilder = discountGroupViewModelBuilder;
            _productGroupDiscountViewModelBuilder = productGroupDiscountViewModelBuilder;
            _auditLogViewModelBuilder = auditLogViewModelBuilder;
        }
        public ActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListDiscountGroups(bool? showInactive, int page = 1, int itemsperpage = 10, string searchText = "")
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
                if (TempData["msg"] != null)
                {
                    ViewBag.msg = TempData["msg"].ToString();
                    TempData["msg"] = null;
                }

                ViewBag.searchParam = searchText;
                
                int currentPageIndex = page < 0 ? 0 : page - 1;
                int take = itemsperpage;
                int skip = currentPageIndex*take;

                var query = new QueryStandard()
                    {
                        Name = searchText,
                        Skip = skip,
                        Take = take,
                        SupplierId = supplierId
                    };

                var ls = _discountGroupViewModelBuilder.Query(query);
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
        [HttpPost]
        public ActionResult ListDiscountGroups(bool? showInactive, int? page,string srch,string searchText, int? itemsperpage)
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
                int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
                if (command == "Search")
                {
                    ViewBag.searchParam = searchText;
                    var ls = _discountGroupViewModelBuilder.Search(searchText, showinactive);
                    return View(ls.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage));
                }
                else
                {
                    return RedirectToAction("ListDiscountGroups", new { srch = "Search", showinactive = showInactive, group = "" });
                }
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }

        }
        [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult CreateDiscountGroup()
        {
            return View("CreateDiscountGroup",new DiscountGroupViewModel());
        }
        [HttpPost]
        public ActionResult CreateDiscountGroup(DiscountGroupViewModel dgvm)
        {
            try
            {
                dgvm.id = Guid.NewGuid();
                _discountGroupViewModelBuilder.Save(dgvm);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Create", "Discount Group", DateTime.Now);
                TempData["msg"] = "Discount Group Successfully Created";
                return RedirectToAction("ListDiscountGroups");
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
        public ActionResult EditDiscountGroup(Guid id)
        {
            DiscountGroupViewModel dgvm = _discountGroupViewModelBuilder.Get(id);
            return View(dgvm);
        }
        [HttpPost]
        public ActionResult EditDiscountGroup(DiscountGroupViewModel dgvm)
        {
            try
            {
                _discountGroupViewModelBuilder.Save(dgvm);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Edit", "Discount Group", DateTime.Now);
                TempData["msg"] = "Discount Group Successfully Edited";
                return RedirectToAction("ListDiscountGroups");
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
        public ActionResult Delete(Guid id)
        {
            try
            {
                _discountGroupViewModelBuilder.SetInactive(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Delete", "Discount Group", DateTime.Now);
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
            return RedirectToAction("ListDiscountGroups");
        }
        //public ActionResult ListProductGroupDiscounts(bool? showInactive, int? page,int Id)
        //{
        //    bool showinactive = false;
        //    if (showInactive != null)
        //        showinactive = (bool)showInactive;
        //    ViewBag.showInactive = showinactive;
        //    var ls = _productGroupDiscountViewModelBuilder.GetByDiscountGroup(Id,showinactive);
        //    int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
        //    return View(ls.ToPagedList(currentPageIndex, defaultPageSize,Id));

        //}
        //public ActionResult ListProductGroupDiscountItems(int id, int? page)
        //{
        //    ProductDiscountGroupViewModel pdvm = _productGroupDiscountViewModelBuilder.Get(id);
        //    pdvm.CurrentPage = 1;
        //    if (page.HasValue)
        //        pdvm.CurrentPage = page.Value;
            
        //    return View(pdvm);
        //}
        //public ActionResult CreateProductGroupDiscount(int Id)
        //{
        //    //ViewBag.ProductList = _productGroupDiscountViewModelBuilder.ProductList();
        //    //ViewBag.DiscountGroupList = _productGroupDiscountViewModelBuilder.DiscountGroupList();
        //    //return View("CreateProductGroupDiscount", new ProductDiscountGroupViewModel());
        //    ViewBag.ProductList = _productGroupDiscountViewModelBuilder.ProductList();
        //    ProductDiscountGroupViewModel pd = new ProductDiscountGroupViewModel { DiscountGroup = Id };
        //    return View(pd);
        //}
        //[HttpPost]
        //public ActionResult CreateProductGroupDiscount(ProductDiscountGroupViewModel pgdvm)
        //{
        //    ViewBag.ProductList = _productGroupDiscountViewModelBuilder.ProductList();
        //    ViewBag.DiscountGroupList = _productGroupDiscountViewModelBuilder.DiscountGroupList();
        //    try
        //    {
        //        _productGroupDiscountViewModelBuilder.Save(pgdvm);
        //        return RedirectToAction("ListProductGroupDiscounts");
        //    }
        //    catch (DomainValidationException dve)
        //    {
        //        ValidationSummary.DomainValidationErrors(dve, ModelState);
        //        return View();
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.msg = ex.Message;
        //        return View();
        //    }
        //}
        [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult CreateProductGroupDiscountItems(Guid id, string discountGroup)
        {
            ViewBag.ProductList = _productGroupDiscountViewModelBuilder.ProductList();
            try
            {
                ProductDiscountGroupViewModel pd = new ProductDiscountGroupViewModel { Id = id };
                return View(pd);
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }
        [HttpPost]
        public ActionResult CreateProductGroupDiscountItems(ProductDiscountGroupViewModel pgdvm)
        {
            ViewBag.ProductList = _productGroupDiscountViewModelBuilder.ProductList();
            try
            {
                Guid id = pgdvm.Id;
                decimal rate = pgdvm.discountRate;
                Guid productId = pgdvm.Product;
                DateTime effectiveDate = pgdvm.EffectiveDate;
                DateTime endDate = pgdvm.EndDate;
                _productGroupDiscountViewModelBuilder.AddProductGroupDiscount(id, productId, rate, effectiveDate, endDate);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Create", "Discount Group Items", DateTime.Now);
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
    }
}
