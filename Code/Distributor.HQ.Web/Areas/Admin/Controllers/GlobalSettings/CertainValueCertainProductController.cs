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
    public class CertainValueCertainProductController : Controller
    { 
        ICertainValueCertainProductDiscountViewModelBuilder _certainValueCertainProductDiscountViewModelBuilder;
        IAuditLogViewModelBuilder _auditLogViewModelBuilder;
        public CertainValueCertainProductController(ICertainValueCertainProductDiscountViewModelBuilder certainValueCertainProductDiscountViewModelBuilder,IAuditLogViewModelBuilder auditLogViewModelBuilder)
        {
            _certainValueCertainProductDiscountViewModelBuilder=certainValueCertainProductDiscountViewModelBuilder;
            _auditLogViewModelBuilder = auditLogViewModelBuilder;
        }

        

        [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListCertainValueCertainProduct(int page = 1, int itemsperpage = 10, string srchParam = "")
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
                /*int currentPageIndex = page.HasValue ? page.Value - 1 : 0;*/

                ViewBag.srchParam = srchParam;
                int currentPageIndex = page - 1 < 0 ? 0 : page - 1;
                int take = itemsperpage;
                int skip = currentPageIndex * take;
                var query = new QueryStandard() { Skip = skip, Take = take, Name = srchParam, SupplierId = supplierId};

                var ls = _certainValueCertainProductDiscountViewModelBuilder.Query(query);
                var total = ls.Count;
                var data = ls.Data;

                return View(data.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage, total));
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }

        

        [Authorize(Roles = "RoleModifyMasterData")]
        public ActionResult CreateDiscount()
        {
            var viewModel = new CertainValueCertainProductDiscountViewModel();
            viewModel.EffectiveDate = DateTime.Today;
            viewModel.EndDate = DateTime.Today.AddDays(1);
            ViewBag.ProductList = _certainValueCertainProductDiscountViewModelBuilder.ProductList();
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult CreateDiscount(CertainValueCertainProductDiscountViewModel viewModel)
        {
            try
            {
                _certainValueCertainProductDiscountViewModelBuilder.ThrowIfExists(viewModel);
                _certainValueCertainProductDiscountViewModelBuilder.Save(viewModel);
                TempData["msg"] = "Discount Successfully Created";
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Create", "Certain Value Certain Product", DateTime.Now);
                return RedirectToAction("ListCertainValueCertainProduct");
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                ViewBag.ProductList = _certainValueCertainProductDiscountViewModelBuilder.ProductList();
                return View(viewModel);
            }
            catch (Exception exx)
            {
                ViewBag.msg = exx.Message;
                ViewBag.ProductList = _certainValueCertainProductDiscountViewModelBuilder.ProductList();
                return View(viewModel);
            }
        }

        [Authorize(Roles = "RoleModifyMasterData")]
        public ActionResult EditDiscount(Guid id)
        {
            var viewModel= new CertainValueCertainProductDiscountViewModel();
            viewModel = _certainValueCertainProductDiscountViewModelBuilder.Get(Guid.Parse(id.ToString()));
            ViewBag.ProductList = _certainValueCertainProductDiscountViewModelBuilder.ProductList();
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult EditDiscount(CertainValueCertainProductDiscountViewModel model)
        {
            try
            {
                _certainValueCertainProductDiscountViewModelBuilder.Save(model);
                TempData["msg"] = "Discount Successfully Edited";
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Edit", "Certain Value Certain Product", DateTime.Now);
              
                return RedirectToAction("ListCertainValueCertainProduct");
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                ViewBag.ProductList = _certainValueCertainProductDiscountViewModelBuilder.ProductList();
                return View(model);
            }
            catch (Exception exx)
            {
                ViewBag.msg = exx.Message;
                ViewBag.ProductList = _certainValueCertainProductDiscountViewModelBuilder.ProductList();
                return View(model);
            }
        }

        [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListCertainValueCertainProductItem(Guid id, int? page, int? itemsperpage)
        {
            try
            {
                if (itemsperpage != null)
                {
                    ViewModelBase.ItemsPerPage = itemsperpage.Value;
                }
                CertainValueCertainProductDiscountViewModel cvc = _certainValueCertainProductDiscountViewModelBuilder.Get(id);
                cvc.CurrentPage = 1;
                ViewBag.cvcpId = id;
                if (page.HasValue)
                    cvc.CurrentPage = page.Value;
                return View(cvc);
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }
        /*
        [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult CreateCertainValueCertainProductItem(Guid id)
        {
            ViewBag.ProductList = _certainValueCertainProductDiscountViewModelBuilder.ProductList();
            try
            {
               var cvc = new CertainValueCertainProductDiscountViewModel
                {
                    id = id
                };
                return View(cvc);
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }

        [HttpPost]
        public ActionResult CreateCertainValueCertainProductItem(CertainValueCertainProductDiscountViewModel cvcp)
        {
            try
            {
             _certainValueCertainProductDiscountViewModelBuilder.AddFreeOfChargeDiscount(cvcp.id,cvcp.Quantity,cvcp.Product,cvcp.InitialValue, DateTime.Parse(cvcp.EffectiveDate),DateTime.Parse(cvcp.EndDate));
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Create", "Certain Value Certain Product Item", DateTime.Now);
                return RedirectToAction("ListCertainValueCertainProductItem", new { @id=cvcp.id});
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
        }*/
        public ActionResult Deactivate(Guid id) 
        {
            try
            {
                _certainValueCertainProductDiscountViewModelBuilder.SetInactive(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Deactivate", "Certain Value Certain Product", DateTime.Now);
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
            return RedirectToAction("ListCertainValueCertainProduct");
        } 
        public ActionResult Delete(Guid id)
        {

            try
            {
                _certainValueCertainProductDiscountViewModelBuilder.SetDeleted(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Delete", "Certain Value Certain Product", DateTime.Now);
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
            return RedirectToAction("ListCertainValueCertainProduct");
        }
        public ActionResult Activate(Guid id) {
            try
            {
                _certainValueCertainProductDiscountViewModelBuilder.SetActive(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Activate", "Certain Value Certain Product", DateTime.Now);
                TempData["msg"] = "Successfully Activated";

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
            return RedirectToAction("ListCertainValueCertainProduct");
        }


        public ActionResult DeleteLineItem(Guid id, Guid cvcpId)
        {
            try
            {
                _certainValueCertainProductDiscountViewModelBuilder.DeacivateLineItem(id);
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;

            }
            return RedirectToAction("ListCertainValueCertainProductItem", new { id = cvcpId });
        }

       
    }
}
