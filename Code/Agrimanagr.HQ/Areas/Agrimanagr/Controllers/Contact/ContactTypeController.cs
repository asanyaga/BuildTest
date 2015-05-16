using System.Linq;
using Distributr.Core.Utility;
using Distributr.HQ.Lib.Paging;
using Distributr.HQ.Lib.Validation;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Contacts;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Transaction;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Admin.Contact;
using System;
using System.Web.Mvc;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;

namespace Agrimanagr.HQ.Areas.Agrimanagr.Controllers.Contact
{
    public class ContactTypeController : Controller
    {
        private const int defaultPageSize = 10;
        //
        // GET: /Admin/ContactType/
        IContactTypeViewModelBuilder _contactTypeViewModelBuilder;
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

        [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListContactTypes(bool showInactive = false, int page = 1, int itemsperpage = 10, string srchparam = "")
        {
            try
            {
                bool showinactive = false;
                if (showInactive != null)
                    showinactive = (bool)showInactive;

                ViewBag.showInactive = showinactive;
                ViewBag.srchparam = srchparam;
                if (TempData["msg"] != null)
                {
                    ViewBag.msg = TempData["msg"].ToString();
                    TempData["msg"] = null;
                }
                //ViewBag.showInactive = showInactive;
                int currentPageIndex = page - 1 < 0 ? 0 : page - 1;
                int take = itemsperpage;
                int skip = currentPageIndex * take;
                var query = new QueryStandard { Skip = skip, Take = take, Name = srchparam, ShowInactive = showInactive };

                var ls = _contactTypeViewModelBuilder.Query(query);
                var total = ls.Count;
                var data = ls.Data.ToList();

                return View(data.ToPagedList(currentPageIndex, take, total));
           
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                var errorContactList = new ContactTypeViewModel();
                errorContactList.ErrorText = ex.Message;
                return View();
            }
        }

        [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult AddContactType()
        {
            return View("AddContactType",new ContactTypeViewModel());
        } 


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

        [Authorize(Roles = "RoleUpdateMasterData")]
        public ActionResult EditContactType(Guid id)
        {
            ContactTypeViewModel cTypeVM = _contactTypeViewModelBuilder.GetById(id);
            return View(cTypeVM);
        }


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


        public ActionResult Deactivate(Guid id)
        {
            _contactTypeViewModelBuilder.SetInActive(id);
            TempData["msg"] = "ContactType Successfully Deactivated";
            _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Deactivate", "ContactType", DateTime.Now);
            return RedirectToAction("ListContactTypes");
        }

        [Authorize(Roles = "RoleDeleteMasterData")]
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

    	[HttpPost]
        public ActionResult ListContactTypes(bool? showInactive, int? page,string srch,string cType)
        {
            string command = srch;
                bool showinactive = false;
                if (showInactive != null)
                    showinactive = (bool)showInactive;

                ViewBag.showInactive = showinactive;

                if (TempData["msg"] != null)
                {
                    ViewBag.msg = TempData["msg"].ToString();
                    TempData["msg"] = null;
                }
                if (command == "Search")
                {
                    var ls = _contactTypeViewModelBuilder.Search(cType, showinactive);
                    int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
                    return View(ls.ToPagedList(currentPageIndex, defaultPageSize));
                }
                else
                    return RedirectToAction("ListContactTypes", new {srch="Search",showinactive=showInactive,cType="" });
            
        }
    }
}
