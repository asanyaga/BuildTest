using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.Security;
using Distributr.Core.Utility;
using Distributr.HQ.Lib.Paging;
using System.Web;
using System.Web.Mvc;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.SuppliersViewModelBuilders;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Transaction;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Admin.SuppliersViewModel;
using Distributr.HQ.Lib.Validation;
using System.Diagnostics;
using log4net;
using System.Reflection;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;

namespace Distributr.HQ.Web.Areas.Admin.Controllers.GlobalSettings.Suppliers
{
    public class SupplierController : Controller
    { 
        ISupplierViewModelBuilder _supplierViewModelBuilder;
        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        IAuditLogViewModelBuilder _auditLogViewModelBuilder;
        public SupplierController(ISupplierViewModelBuilder supplierViewModelBuilder, IAuditLogViewModelBuilder auditLogViewModelBuilder)
        {
            _supplierViewModelBuilder = supplierViewModelBuilder;
            _auditLogViewModelBuilder = auditLogViewModelBuilder;
        }
        public ActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListSuppliers(bool showInactive = false, int page = 1, int itemsperpage = 10, string srchParam = "")
        {

            try
            {
                if (itemsperpage != null)
                {
                    ViewModelBase.ItemsPerPage = itemsperpage;
                }
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                bool showinactive = false;
                if (showInactive != null)
                    showinactive = (bool)showInactive;
                ViewBag.showInactive = showinactive;
                if (TempData["msg"] != null)
                {
                    ViewBag.msg = TempData["msg"].ToString();
                    TempData["msg"] = null;
                }
                ViewBag.srchParam = srchParam;

                stopWatch.Stop();
                TimeSpan ts = stopWatch.Elapsed;

                // Format and display the TimeSpan value.
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    ts.Hours,
                    ts.Minutes,
                    ts.Seconds,
                    ts.TotalMilliseconds);


                stopWatch.Reset();

                _log.InfoFormat("Product Brand\tTime taken to get all product brands" + elapsedTime);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Timestamp", "Product Brand Controller:" + elapsedTime, DateTime.Now);

                var user = (CustomIdentity) this.User.Identity;
                Guid? supplierId = user.SupplierId != null ? user.SupplierId : (Guid?) null;

                ViewBag.srchParam = srchParam;
                var currentPageIndex = page - 1 < 0 ? 0 : page - 1;
                var take = itemsperpage;
                var skip = currentPageIndex * take;
                var query = new QueryStandard { Name = srchParam, ShowInactive = showinactive, Skip = skip, Take = take, SupplierId = supplierId};
                var ls = _supplierViewModelBuilder.Query(query);
                var total = ls.Count;
                var data = ls.Data;
                return View(data.ToPagedList(currentPageIndex,  ViewModelBase.ItemsPerPage,total));
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }
        [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult CreateSupplier()
        {
            return View("CreateSupplier", new SupplierViewModel());
        }
        [HttpPost]
        public ActionResult CreateSupplier(SupplierViewModel supplierVM)
        {
            try
            {
                supplierVM.Id = Guid.NewGuid();
                _supplierViewModelBuilder.Save(supplierVM);
                TempData["msg"] = "Supplier Successfully Created";
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Create", "Add Supplier", DateTime.Now);
                
                return RedirectToAction("ListSuppliers");
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
        public ActionResult EditSupplier(Guid id)
        {
            SupplierViewModel supplierVM = _supplierViewModelBuilder.GetById(id);
            return View(supplierVM);
        }
        [HttpPost]
        public ActionResult EditSupplier(SupplierViewModel supplierVM)
        {
            try
            {
                _supplierViewModelBuilder.Save(supplierVM);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Edit", "Supplier", DateTime.Now);
                TempData["msg"] = "Supplier Successfully Edited";
                return RedirectToAction("ListSuppliers");
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
        public ActionResult Deactivate(Guid id)
        {
            try
            {
                _supplierViewModelBuilder.SetInactive(id);
                _log.InfoFormat("Deactivating Supplier id:" + id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Deactivate", "Supplier", DateTime.Now);
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
                _log.InfoFormat("Deactivating Supplier=" + ex.Message);
                ViewBag.msg = ex.Message;
            }
            return RedirectToAction("ListSuppliers");
        }

        public ActionResult Delete(Guid id)
        {

            try
            {
                _supplierViewModelBuilder.SetDelete(id);
                _log.InfoFormat("Deleting Supplier id:" + id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Delete", "Supplier", DateTime.Now);
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
                _log.InfoFormat("Deleting Supplier=" + ex.Message);
                ViewBag.msg = ex.Message;
            }
            return RedirectToAction("ListSuppliers");
        }

		public ActionResult Activate(Guid id, string name)
		{
			try
			{
				_supplierViewModelBuilder.SetActive(id);
				_auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Activate", "Supplier", DateTime.Now);
				TempData["msg"] = name + " Successfully Activated";
			}
			catch (Exception ex)
			{
				TempData["msg"] = ex.Message;
				_log.InfoFormat("Error Activating Supplier = "+ex.Message);
				ViewBag.msg = ex.Message;
			}
			return RedirectToAction("ListSuppliers");
		}

    
    }
}
