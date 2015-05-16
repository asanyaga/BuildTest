using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Contacts;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Admin.Contact;
using Distributr.HQ.Lib.Paging;
using Distributr.HQ.Lib.Validation;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Transaction;
using log4net;

namespace Distributr.HQ.Web.Areas.Admin.Controllers.Contact
{
    public class ContactTypeController : Controller
    { 
        IContactTypeViewModelBuilder _contactTypeViewModelBuilder;
        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        
        IAuditLogViewModelBuilder _auditLogViewModelBuilder;
        public ContactTypeController(IContactTypeViewModelBuilder contactTypeViewModelBuilder,IAuditLogViewModelBuilder auditLogViewModelBuilder)
        {
            _contactTypeViewModelBuilder = contactTypeViewModelBuilder;
            _auditLogViewModelBuilder = auditLogViewModelBuilder;
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListContactTypes(bool? showInactive, int page = 1, int itemsperpage = 10, string srchparam = "")
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
                ViewBag.srchParam = srchparam;
                
                int currentPageIndex = page < 0 ? 0 : page - 1;
                int take = itemsperpage;
                int skip = currentPageIndex * take;
                var query = new QueryStandard();
                query.ShowInactive = showinactive;
                query.Skip = skip;
                query.Take = take;
                query.Name = srchparam;

                var result = _contactTypeViewModelBuilder.Query(query);
                var count = result.Count;
                var data = result.Data.ToList();

                return View(data.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage, count));

            }
            catch (Exception ex)
            {
                _log.Debug("Failed to list contact types" + ex.Message);
                _log.Error("Failed to list contact types" + ex.ToString());
                return View();
            }
        }
        //
        // GET: /Admin/ContactType/Create

        public ActionResult AddContactType()
        {
            return View("AddContactType",new ContactTypeViewModel());
        } 

        //
        // POST: /Admin/ContactType/Create

        [HttpPost]
        public ActionResult AddContactType(ContactTypeViewModel contactTypeVM)
        {
            try
            {
                contactTypeVM.Id = Guid.NewGuid();
                _contactTypeViewModelBuilder.Save(contactTypeVM);
                TempData["msg"] = "ContactType Successfully Created";
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Create", "ContactType", DateTime.Now);

                return RedirectToAction("ListContactTypes");
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve,ModelState);
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }
        
        //
        // GET: /Admin/ContactType/Edit/5

        public ActionResult EditContactType(Guid id)
        {
            ContactTypeViewModel cTypeVM = _contactTypeViewModelBuilder.GetById(id);
            return View(cTypeVM);
        }

        //
        // POST: /Admin/ContactType/Edit/5

        [HttpPost]
        public ActionResult EditContactType(ContactTypeViewModel contactTypeVM)
        {
            try
            {
                _contactTypeViewModelBuilder.Save(contactTypeVM);
                TempData["msg"] = "ContactType Successfully Edited";
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Edit", "ContactType", DateTime.Now);
                return RedirectToAction("ListContactTypes");
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve,ModelState);
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }

        //
        // GET: /Admin/ContactType/Deactivate/5

        public ActionResult Deactivate(Guid id)
        {
            _contactTypeViewModelBuilder.SetInActive(id);
            TempData["msg"] = "ContactType Successfully Deactivated";
            _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Deactivate", "ContactType", DateTime.Now);
            return RedirectToAction("ListContactTypes");
        }

        public ActionResult Delete(Guid id)
        {
            _contactTypeViewModelBuilder.SetDeleted(id);
            TempData["msg"] = "ContactType Successfully Deleted";
            _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Delete", "ContactType", DateTime.Now);
            return RedirectToAction("ListContactTypes");
        }

		public ActionResult Activate(Guid id, string name)
		{
			_contactTypeViewModelBuilder.SetActive(id);
			TempData["msg"] = name + " Successfully Activated";
			_auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Activate", "ContactType", DateTime.Now);
			return RedirectToAction("ListContactTypes");
		}

    }
}
