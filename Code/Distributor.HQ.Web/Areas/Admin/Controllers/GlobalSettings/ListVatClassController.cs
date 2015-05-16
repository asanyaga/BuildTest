using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.CostCenter;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Admin.CostCenter;
using Distributr.HQ.Lib.Validation;
using MvcContrib.Pagination;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Transaction;
using Distributr.HQ.Lib.Paging;

namespace Distributr.HQ.Web.Areas.Admin.Controllers.GlobalSettings
{
    [Authorize ]
    public class ListVatClassController : Controller
    { 
        IListVatClassViewModelBuilder _listVatClassModelBuilder;
        IEditVatClassViewModelBuilder _editClassViewModelBuilder;
        IVATClassViewModelBuilder _addVatClassViewModelBuilder;
        IAuditLogViewModelBuilder _auditLogViewModelBuilder;
        IVatClassLineItemViewModelBuilder _vatClassLineItemViewModelBuilder;
        public ListVatClassController(IListVatClassViewModelBuilder listVatClassModelBuilder, IEditVatClassViewModelBuilder editClassViewModelBuilder, IVATClassViewModelBuilder addVatClassViewModelBuilder, IVatClassLineItemViewModelBuilder vatClassLineItemViewModelBuilder,IAuditLogViewModelBuilder auditLogViewModelBuilder)
        {
            _listVatClassModelBuilder = listVatClassModelBuilder;
            _editClassViewModelBuilder = editClassViewModelBuilder;
            _addVatClassViewModelBuilder = addVatClassViewModelBuilder;
            _vatClassLineItemViewModelBuilder = vatClassLineItemViewModelBuilder;
            _auditLogViewModelBuilder = auditLogViewModelBuilder;
        }
        public ActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListVat(Boolean showInactive = false,int page=1, int itemsperpage=10, string srchParam="")
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
                if (TempData["msg"] != null)
                {
                    ViewBag.msg = TempData["msg"].ToString();
                    TempData["msg"] = null;
                }
            
                ViewBag.showInactive = showInactive;
                ViewBag.srchParam = srchParam;
                int currentPageIndex = page - 1 < 0 ? 0 : page - 1;
                int take = itemsperpage;
                int skip = currentPageIndex * take;
                var query = new QueryStandard() { Skip = skip, Take = take, Name = srchParam, ShowInactive = showInactive };

                var ls = _listVatClassModelBuilder.Query(query);
                var total = ls.Count;
                var data = ls.Data;

                return View(data.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage, total));



                //return View(ls.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage));
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }
        [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult CreateVat()
        {

            return View("CreateVat", new VATClassViewModel());
        }
        //
        //Post
        [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult AddVatClassLineItems(Guid id)
        {
            try
            {
                VatClassLineItemViewModel vcl = new VatClassLineItemViewModel
                {
                    id = id
                };
                return View(vcl);
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }
        [HttpPost]
        public ActionResult AddVatClassLineItems(VatClassLineItemViewModel vcl)
        { 
         try{
             Guid id = vcl.id;
             decimal addrate=vcl.Rate;
             DateTime? addeffectivedate=vcl.effectiveDate;

             _vatClassLineItemViewModelBuilder.AddVatClassItem(id, addrate, addeffectivedate.Value);
             TempData["msg"] = "VAT line-item Successfully Created";
             _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Create", "Vat Class Items", DateTime.Now);
             return RedirectToAction("ListVatClassLineItems", new {@id=vcl.id });
         }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                ViewBag.msg = dve.Message;
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }
        //
        //Edit VatClass LineItem
        [Authorize(Roles = "RoleModifyMasterData")]
        public ActionResult EditVatClassLineItems(Guid id)
        {
            try
            {
                VatClassLineItemViewModel vm = _vatClassLineItemViewModelBuilder.Get(id);
                return View(vm);
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
        //
        //Add VatClass LineItem
        [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult AddVatLineItem(Guid id)
        {
            try
            {
                EditVatClassViewModel vm = _editClassViewModelBuilder.Get(id);
                return View(vm);
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
        //
        //VatClass Lineitems
        [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListVatClassLineItems(Guid id, int? itemsperpage)
        {
            try 
            {
                if (itemsperpage != null)
                {
                    ViewModelBase.ItemsPerPage = itemsperpage.Value;
                }
                if (TempData["msg"] != null)
                {
                    ViewBag.msg = TempData["msg"].ToString();
                    TempData["msg"] = null;
                }
                EditVatClassViewModel vam = _editClassViewModelBuilder.Get(id);
                return View(vam);
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
        //
        // POST: /Admin/VatClass/Edit/5

        [HttpPost]
        public ActionResult CreateVat(VATClassViewModel vcm)
        {
            try
            {
                ViewBag.msg = "";
                vcm.Id = Guid.NewGuid();
                _addVatClassViewModelBuilder.Save(vcm); 
                TempData["msg"] = "VAT Successfully Created";
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Create", "Vat Class", DateTime.Now);
                return RedirectToAction("ListVat");
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
        public ActionResult EditVat1(Guid id)
        {
            try
            {
                EditVatClassViewModel vm = _editClassViewModelBuilder.Get(id);
                return View(vm);
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
      [HttpPost]
        public ActionResult EditVat1(VATClassViewModel vatCl)
        {
            try
            {
                _addVatClassViewModelBuilder.Save(vatCl);
                TempData["msg"] = "VAT Successfully Edited";
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Edit", "Vat Class", DateTime.Now);
                return RedirectToAction("ListVat");
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
        [HttpPost]
      public ActionResult CreateVatLineItem(Guid id, string addrate, DateTime addeffectivedate)
        {
            try
            {
                if (addrate == null || addrate == "")
                {
                    ModelState.AddModelError("addrate", "Rate is required");
                    ViewData["addrate"] = "Rate is Required";
                    TempData["errMsg"] = "Rate is required";
                    ViewBag.err = TempData["errMsg"];

                }
                if (!ModelState.IsValid)
                {
                    return RedirectToAction("ListVatClassLineItems", new { id = id, ViewBag.err });

                }
                else
                {
                    decimal rate = Convert.ToDecimal(addrate);
                    _editClassViewModelBuilder.AddVatClassItem(id, rate, addeffectivedate);
                    _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Create", "Vat Class Items", DateTime.Now);
                    return RedirectToAction("ListVatClassLineItems", new { id = id });
                }
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
                _listVatClassModelBuilder.SetInactive(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Deactivate", "Vat Class", DateTime.Now);
                TempData["msg"] = "Successfully deactivated";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;

            }
            return RedirectToAction("ListVat");
        }
        public ActionResult Delete(Guid id)
        {

            try
            {
                _listVatClassModelBuilder.SetAsDeleted(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Delete", "Vat Class", DateTime.Now);
                TempData["msg"] = "Successfully Deleted";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;

            }
            return RedirectToAction("ListVat");
        }

        public ActionResult Activate(Guid id, string name)
        {

            try
            {
                _listVatClassModelBuilder.SetActive(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Deactivate", "Vat Class", DateTime.Now);
                TempData["msg"] = name + " Successfully activated";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;

            }
            return RedirectToAction("ListVat");
        }
    }
}
