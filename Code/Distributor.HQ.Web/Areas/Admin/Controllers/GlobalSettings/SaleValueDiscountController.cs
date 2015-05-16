using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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

namespace Distributr.HQ.Web.Areas.Admin.Controllers.ProductPricing
{
    [Authorize]
    public class SaleValueDiscountController : Controller
    {
        ISaleValueDiscountViewModelBuilder _saleValueDiscountViewModelBuilder;
        IAuditLogViewModelBuilder _auditLogViewModelBuilder;
        public SaleValueDiscountController(ISaleValueDiscountViewModelBuilder saleValueDiscountViewModelBuilder, IAuditLogViewModelBuilder auditLogViewModelBuilder)
        {
            _saleValueDiscountViewModelBuilder = saleValueDiscountViewModelBuilder;
            _auditLogViewModelBuilder = auditLogViewModelBuilder;
        }
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListSaleValueDiscounts(bool? showInactive, int page = 1, int itemsperpage = 10, string searchText = "")
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
                ViewBag.searchParam = searchText;
                
                
                ViewBag.msg = null;
                if (TempData["msg"] != null)
                {
                    ViewBag.msg = TempData["msg"].ToString();
                    TempData["msg"] = null;
                }
                int currentPageIndex = page < 0 ? 0 : page - 1;
                int take = itemsperpage;
                int skip = currentPageIndex*take;

                var query = new QueryStandard()
                    {
                        Name = searchText,
                        Skip = skip,
                        Take = take
                    };

                var ls = _saleValueDiscountViewModelBuilder.Query(query);
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
        public ActionResult CreateSaleValueDiscount(Guid? id)
        {
            ViewBag.DiscountId = null;
            var saleValueDiscountViewModel = new SaleValueDiscountViewModel();
            ViewBag.TierList = _saleValueDiscountViewModelBuilder.TierList()
                .OrderBy(x => x.Value)
                .ToDictionary(x => x.Key, x => x.Value);
            return View("CreateSaleValueDiscount", saleValueDiscountViewModel);
        }

        [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult CreateDiscount()
        {
            var saleValueDiscountViewModel = new SaleValueDiscountViewModel();
            saleValueDiscountViewModel.EffectiveDate = DateTime.Today;
            saleValueDiscountViewModel.EndDate = DateTime.Today.AddDays(1);
            ViewBag.TierList = _saleValueDiscountViewModelBuilder.TierList()
                .OrderBy(x => x.Value)
                .ToDictionary(x => x.Key, x => x.Value);
            return View("CreateDiscount", saleValueDiscountViewModel);
        }

        [HttpPost]
        public ActionResult CreateDiscount(SaleValueDiscountViewModel svdvm)
        {
            ViewBag.TierList = _saleValueDiscountViewModelBuilder.TierList();
            try
            {
                _saleValueDiscountViewModelBuilder.ThrowIfExists(svdvm);
                svdvm.Id = Guid.NewGuid();
                svdvm.Rate = svdvm.Rate / 100;
                _saleValueDiscountViewModelBuilder.Save(svdvm);
                TempData["msg"] = "Discount Successfully Created";
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Create", "SaleValue Discount", DateTime.Now);
                return RedirectToAction("ListSaleValueDiscounts");
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                return View(svdvm);
            }
            catch (Exception exx)
            {
                ViewBag.msg = exx.Message;
                return View(svdvm);
            }
        }

        [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult EditDiscount(Guid id)
        {
            var discountId = id;
            var saleValueDiscountViewModel = new SaleValueDiscountViewModel();
            saleValueDiscountViewModel = _saleValueDiscountViewModelBuilder.Get(discountId);
            saleValueDiscountViewModel.Rate = saleValueDiscountViewModel.Rate * 100;
            ViewBag.TierList = _saleValueDiscountViewModelBuilder.TierList()
                .OrderBy(x => x.Value)
                .ToDictionary(x => x.Key, x => x.Value);
            return View("EditDiscount", saleValueDiscountViewModel);
        }

        [HttpPost]
        public ActionResult EditDiscount(SaleValueDiscountViewModel svdvm)
        {
            ViewBag.TierList = _saleValueDiscountViewModelBuilder.TierList();
            try
            {
                svdvm.Id = Guid.NewGuid();
                svdvm.Rate = svdvm.Rate / 100;
                _saleValueDiscountViewModelBuilder.Save(svdvm);
                TempData["msg"] = "Discount Successfully Edited";
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Edit", "SaleValue Discount", DateTime.Now);
                return RedirectToAction("ListSaleValueDiscounts");
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                return View(svdvm);
            }
            catch (Exception exx)
            {
                ViewBag.msg = exx.Message;
                return View(svdvm);
            }
        }

        [HttpPost]
        public ActionResult CreateSaleValueDiscount(SaleValueDiscountViewModel svdvm)
        {
            ViewBag.TierList = _saleValueDiscountViewModelBuilder.TierList();

            try
            {
                _saleValueDiscountViewModelBuilder.ThrowIfExists(svdvm);
                svdvm.Id = Guid.NewGuid();
                svdvm.Rate = svdvm.Rate / 100;
                TempData["msg"] = "Discount Successfully Created";
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Create", "SaleValue Discount", DateTime.Now);
                return RedirectToAction("ListSaleValueDiscounts");
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                return View();
            }
            catch (Exception exx)
            {
                ViewBag.msg = exx.Message;
                return View();
            }
        }

        [Authorize(Roles = "RoleModifyMasterData")]
        public ActionResult EditSaleValueDiscount(Guid id)
        {
            ViewBag.TierList = _saleValueDiscountViewModelBuilder.TierList();
            try
            {
                SaleValueDiscountViewModel svdvm = _saleValueDiscountViewModelBuilder.Get(id);

                return View(svdvm);
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }

        [HttpPost]
        public ActionResult EditSaleValueDiscount(SaleValueDiscountViewModel svdvm)
        {
            ViewBag.TierList = _saleValueDiscountViewModelBuilder.TierList();
            try
            {
                svdvm.Rate = svdvm.Rate / 100;
                //_saleValueDiscountViewModelBuilder.Save(svdvm, out TODO);
                TempData["msg"] = "Discount Successfully Edited";
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Edit", "SaleValue Discount", DateTime.Now);
                return RedirectToAction("ListSaleValueDiscounts");
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                return View();
            }
            catch (Exception exx)
            {
                ViewBag.msg = exx.Message;
                return View();
            }
        }

        [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult SaleValueDiscountItems(Guid id, int? page, int? itemsperpage)
        {
            try
            {
                if (itemsperpage != null)
                {
                    ViewModelBase.ItemsPerPage = itemsperpage.Value;
                }
                SaleValueDiscountViewModel svdvm = _saleValueDiscountViewModelBuilder.Get(id);
                ViewBag.svdId = id;
                var ls = svdvm.SaleValueDiscountItems;
                ViewBag.msg = null;
                if (TempData["msg"] != null)
                {
                    ViewBag.msg = TempData["msg"].ToString();
                    TempData["msg"] = null;
                }
                int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
                return View(ls.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage));
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }

        [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult CreateSaleValueDiscountItem(Guid id)
        {
            try
            {
                ViewBag.Id = id;
                SaleValueDiscountViewModel svdvm = new SaleValueDiscountViewModel { Id = id };
                return View(svdvm);
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }

        [HttpPost]
        public ActionResult CreateSaleValueDiscountItem(SaleValueDiscountViewModel svdvm)
        {
            try
            {
                ViewBag.Id = svdvm.Id;
                _saleValueDiscountViewModelBuilder.AddSaleValueDiscountItem(svdvm.Id, svdvm.Rate / 100, svdvm.SaleValue, svdvm.EffectiveDate, svdvm.EndDate);
                TempData["msg"] = "Discount lineitem Successfully Created";
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Create", "SaleValue Discount Items", DateTime.Now);
                return RedirectToAction("SaleValueDiscountItems", new { @id = svdvm.Id });
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                return View();
            }
            catch (Exception exx)
            {
                ViewBag.msg = exx.Message;
                return View();
            }
        }
        public ActionResult Activate(Guid id)
        {

            try
            {
                _saleValueDiscountViewModelBuilder.SetActive(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Activated", "SaleValue Discount", DateTime.Now);
                TempData["msg"] = "Successfully Activated";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;

            }
            return RedirectToAction("ListSaleValueDiscounts");
        }
        public ActionResult Deactivate(Guid id)
        {

            try
            {
                _saleValueDiscountViewModelBuilder.SetInactive(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Deactivate", "SaleValue Discount", DateTime.Now);
                TempData["msg"] = "Successfully Deactivated";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
            }
            return RedirectToAction("ListSaleValueDiscounts");
        }
        public ActionResult Delete(Guid id)
        {

            try
            {
                _saleValueDiscountViewModelBuilder.SetAsDeleted(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Delete", "SaleValue Discount", DateTime.Now);
                TempData["msg"] = "Successfully Deleted";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;

            }
            return RedirectToAction("ListSaleValueDiscounts");
        }

        public ActionResult DeleteLineItem(Guid id, Guid svdId)
        {
            try
            {
                _saleValueDiscountViewModelBuilder.DeacivateLineItem(id);
                TempData["msg"] = "Successfully Deleted";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;

            }
            return RedirectToAction("SaleValueDiscountItems", new { id = svdId });
        }
    }
}
