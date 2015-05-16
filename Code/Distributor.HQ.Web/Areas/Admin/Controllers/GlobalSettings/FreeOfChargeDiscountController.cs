using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.Core.Security;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Products;
using Distributr.HQ.Lib.Validation;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Admin.Product;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Transaction;
using MvcContrib.Pagination;
using Distributr.HQ.Lib.Paging;

namespace Distributr.HQ.Web.Areas.Admin.Controllers.GlobalSettings
{
    public class FreeOfChargeDiscountController : Controller
    { 
        IFreeOfChargeDiscountViewModelBuilder _focViewModelBuilder;
        IAuditLogViewModelBuilder _auditLogViewModelBuilder;
        public FreeOfChargeDiscountController(IFreeOfChargeDiscountViewModelBuilder focViewModelBuilder,IAuditLogViewModelBuilder auditLogViewModelBuilder)
        {
            _focViewModelBuilder = focViewModelBuilder;
            _auditLogViewModelBuilder = auditLogViewModelBuilder;
        }
       
        [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult List(bool? showInactive, Guid? brandId, int page = 1, int itemsperpage = 10, string searchText = "")
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
                ViewBag.BrandList = _focViewModelBuilder.BrandList();
                ViewBag.searchParam = searchText;

                int currentPageIndex = page < 0 ? 0 : page - 1;
                int take = itemsperpage;
                int skip = currentPageIndex*take;

                var query = new QueryFOCDiscount()
                    {
                        Name = searchText,
                        Skip = skip,
                        Take = take,
                        SupplierId = supplierId,
                        BrandId = brandId
                    };

                var ls = _focViewModelBuilder.QueryResult(query);
                var data = ls.Data;
                var count = ls.Count;

                return View(data.ToPagedList(currentPageIndex,  ViewModelBase.ItemsPerPage, count));
              
            }
            catch (Exception ex)
            {
               return View();
            }
        }

        [HttpGet]
        public ActionResult CreateEdit(Guid? id)
        {
            var model = new FreeOfChargeDiscountViewModel();
            if (id.HasValue)
            {
                model = _focViewModelBuilder.Get(id.Value);
            }
            else
            {
                model.StartDate = DateTime.Today.ToShortDateString();
                model.EndDate = DateTime.Today.AddDays(1).ToShortDateString();
            }
            ViewBag.ProductList = _focViewModelBuilder.ProductList();
            return View(model);
        }

        public ActionResult CreateEdit(FreeOfChargeDiscountViewModel model)
        {
            try
            {

                bool isEdit;
                _focViewModelBuilder.Save(model, out isEdit);
                if (isEdit)
                {
                    TempData["msg"] = "Discount Successfully Edited";
                    _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Edit", "Free of charge Product", DateTime.Now);
                }

                else
                {
                    TempData["msg"] = "Discount Successfully Created";
                    _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Create", "Free of charge Product", DateTime.Now);
                }

                return RedirectToAction("List");
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                ViewBag.ProductList = _focViewModelBuilder.ProductList();
                return View();
            }
            catch (Exception exx)
            {
                TempData["msg"] = exx.Message;
                ViewBag.ProductList = _focViewModelBuilder.ProductList();
                return View();
            }
        }

        public ActionResult Delete(Guid id)
        {
            try
            {
                _focViewModelBuilder.SetDeleted(id);
                TempData["msg"] = "Successfully Deleted";
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                TempData["msg"] = dve.Message;
            }
            catch(Exception ex)
            {
                TempData["msg"] = ex.Message;
            }
            return RedirectToAction("List");
        }

    }
}
