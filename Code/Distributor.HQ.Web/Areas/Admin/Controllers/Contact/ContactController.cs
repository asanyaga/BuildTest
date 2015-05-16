using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Utility;
using Distributr.HQ.Lib.Paging;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Admin.Contact;
using Distributr.HQ.Lib.Validation;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Contacts;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Transaction;
using StructureMap.Query;
using System.Configuration;
using log4net;
using System.Reflection;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.UserModelBuilder;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Distributors;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Outlets;

namespace Distributr.HQ.Web.Areas.Admin.Controllers.Contact
{
    [Authorize]
    public class ContactController : Controller
    {
        IContactViewModelBuilder _contactModelViewBuilder;
        IUserViewModelBuilder _userViewModelBuilder;
        IDistributorViewModelBuilder _distributorViewModelBuilder;
        IOutletViewModelBuilder _outletViewModelBuilder;
        IAuditLogViewModelBuilder _auditLogViewModelBuilder;
        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        // ICostCentreViewModelBuilder _costCentreViewModelBuider;
        public ContactController(IContactViewModelBuilder contactModelViewBuilder, IAuditLogViewModelBuilder auditLogViewModelBuilder, IUserViewModelBuilder userViewModelBuilder, IDistributorViewModelBuilder distributorViewModelBuilder, IOutletViewModelBuilder outletViewModelBuilder)
        {
            _contactModelViewBuilder = contactModelViewBuilder;
            _auditLogViewModelBuilder = auditLogViewModelBuilder;
            _distributorViewModelBuilder = distributorViewModelBuilder;
            _outletViewModelBuilder = outletViewModelBuilder;
            _userViewModelBuilder = userViewModelBuilder;
        }
        public ActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListContacts(bool? showInactive, Guid? CostCentre, int? ContactOwner, int itemsperpage = 10, int page = 1, string srchparam = "")
        {
            ViewBag.contactOwnerList = AcceptedContactOwnerTypes();// _contactModelViewBuilder.ContactOwner();
            ViewBag.contactOwner = null;
            try
            {
                if (itemsperpage != null)
                {
                    ViewModelBase.ItemsPerPage = itemsperpage;
                }
                ViewBag.CostCentre = CostCentre;
                bool showinactive = false;
                if (showInactive != null)
                    showinactive = (bool)showInactive;

                ViewBag.showInactive = showinactive;
                ViewBag.ContactsFor = "All";
                if (TempData["msg"] != null)
                {
                    ViewBag.msg = TempData["msg"].ToString();
                    TempData["msg"] = null;
                }
                ViewBag.SearchText = srchparam;
                
                var currentPageIndex = page - 1 < 0 ? 0 : page - 1;
                var take = itemsperpage;
                var skip = currentPageIndex * take;
                var query = new QueryStandard { Name = srchparam, ShowInactive = showinactive, Skip = skip, Take = take };
                var ls = _contactModelViewBuilder.Query(query);
                var total = ls.Count;
                var data = ls.Data.ToList();


                return View(data.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage, total));
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                var errorContactList = new ContactViewModel();
                errorContactList.ErrorText = ex.Message;
               // return View(errorContactList);
                return View();
            }
        }
        [HttpPost]
        public ActionResult ListContacts(bool? showInactive, Guid? CostCentre, int? page, string srcParam, string srch, string CostCentreID, string ContactOwner, int? itemsperpage)
        {
            ViewBag.contactOwnerList = AcceptedContactOwnerTypes();// _contactModelViewBuilder.ContactOwner();
            string command = srch;
            Guid ccId = Guid.Empty;
            int ccType = 0;
            if (ContactOwner != "")
            {
                int.TryParse(ContactOwner, out ccType);
            }
            if (CostCentreID != "")
            {
                Guid.TryParse(CostCentreID, out ccId);
                //ccId = Guid.Parse(CostCentreID.ToString());
            }
            try
            {
                if (itemsperpage != null)
                {
                    ViewModelBase.ItemsPerPage = itemsperpage.Value;
                }
                ViewBag.CostCentre = CostCentre;
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
                    var ls = _contactModelViewBuilder.SearchContacts(srcParam, showinactive);
                    int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
                    return View(ls.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage));
                }
                if (command == "Filter")
                {
                    var ls = _contactModelViewBuilder.GetContactsByCostCentre(ccId, showinactive);
                    string cont = GetCostCentreUser(ccType, ccId);
                    ViewBag.ContactsFor = GetCostCentreUser(ccType, ccId);
                    int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
                    return View(ls.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage));
                }
                else
                    return RedirectToAction("ListContacts", new { srcParam = "", showinactive = showInactive, srch = "Search" });
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                var errorContactList = new ContactViewModel();
                errorContactList.ErrorText = ex.Message;
                return View(errorContactList);
            }
        }
        public ActionResult ContactDetails(Guid Id)
        {
            ContactViewModel cont = _contactModelViewBuilder.Get(Id);
            return View(cont);
        }
        [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult CreateContact(Guid? CostCentre, string CostCentreName, string ContactFor)
        {
            ViewBag.CostCentreList = _contactModelViewBuilder.CostCentre();
            ViewBag.MaritalStatusList = _contactModelViewBuilder.GetMarialStatus();
            ViewBag.ContactClassificationList = _contactModelViewBuilder.ContactClassification();
            ViewBag.contactTypeList = _contactModelViewBuilder.GetContactTypes();
            ViewBag.contactOwnerList = AcceptedContactOwnerTypes();// _contactModelViewBuilder.ContactOwner();
            try
            {
                if (CostCentre.HasValue)
                {
                    ContactViewModel cvm = new ContactViewModel
                    {
                        CostCentre = CostCentre.Value,
                        CostCentreName = CostCentreName,
                        ContactFor = ContactFor
                    };
                    //return View("CreateContact",_contactModelViewBuilder.Get(0));
                    return View(cvm);
                }
                else
                {
                    ContactViewModel cvm2 = new ContactViewModel
                   {
                       CostCentre = Guid.NewGuid(),
                       CostCentreName = "",
                       ContactFor = ""
                   };
                    return View("CreateContact", cvm2);
                }
            }
            catch (Exception ex)
            {
                return View(ex.Message);
            }
        }
        [HttpPost]
        public ActionResult CreateContact(ContactViewModel cvm)
        {
            ViewBag.CostCentreList = _contactModelViewBuilder.CostCentre();
            ViewBag.MaritalStatusList = _contactModelViewBuilder.GetMarialStatus();
            ViewBag.ContactClassificationList = _contactModelViewBuilder.ContactClassification();
            ViewBag.contactTypeList = _contactModelViewBuilder.GetContactTypes();
            ViewBag.contactOwnerList = AcceptedContactOwnerTypes();// _contactModelViewBuilder.ContactOwner();
            try
            {
                if (cvm.Firstname == null)
                {
                    ModelState.AddModelError("Contact", "Firstname is required");
                    ContactViewModel cvm2 = new ContactViewModel
                    {
                        CostCentre = Guid.NewGuid(),
                        CostCentreName = "",
                        ContactFor = ""
                    };
                    return View(cvm2);
                }
                else
                {
                    cvm.Id = Guid.NewGuid();
                    _contactModelViewBuilder.save(cvm);
                    ViewBag.CostCentreList = _contactModelViewBuilder.CostCentre();
                    _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Create", "Contact", DateTime.Now);
                    TempData["msg"] = "Contact Successfully Created";

                    return RedirectToAction("ListContacts", new { CostCentre = cvm.CostCentre, CostCentreName = cvm.CostCentreName, ContactFor = cvm.ContactFor });
                }
            }
            catch (DomainValidationException dve)
            {

                ValidationSummary.DomainValidationErrors(dve, ModelState);
                ContactViewModel cvm2 = new ContactViewModel
                {
                    CostCentre = Guid.NewGuid(),
                    CostCentreName = "",
                    ContactFor = ""
                };
                return View(cvm2);
            }
            catch (Exception exx)
            {
                ViewBag.msg = exx.Message;
                ContactViewModel cvm2 = new ContactViewModel
                {
                    CostCentre = Guid.NewGuid(),
                    CostCentreName = "",
                    ContactFor = ""
                };
                return View(cvm2);
            }
        }
        [Authorize(Roles = "RoleModifyMasterData")]
        public ActionResult EditContact(Guid Id)
        {
            ViewBag.ContactClassificationList = _contactModelViewBuilder.ContactClassification();
            ViewBag.MaritalStatusList = _contactModelViewBuilder.GetMarialStatus();
            ViewBag.CostCentreList = _contactModelViewBuilder.CostCentre();
            ViewBag.contactTypeList = _contactModelViewBuilder.GetContactTypes();
            ViewBag.contactOwnerList = AcceptedContactOwnerTypes();// _contactModelViewBuilder.ContactOwner();
            try
            {
                ContactViewModel cvm = _contactModelViewBuilder.Get(Id);

                return View(cvm);
            }
            catch (Exception ex)
            {


                ViewBag.msg = ex.Message;
                return View();
            }
        }
        [HttpPost]
        public ActionResult EditContact(ContactViewModel cvm)
        {
            ViewBag.ContactClassificationList = _contactModelViewBuilder.ContactClassification();
            ViewBag.MaritalStatusList = _contactModelViewBuilder.GetMarialStatus();
            ViewBag.CostCentreList = _contactModelViewBuilder.CostCentre();
            ViewBag.contactTypeList = _contactModelViewBuilder.GetContactTypes();
            try
            {
                _contactModelViewBuilder.save(cvm);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Edit", "Contact", DateTime.Now);
                TempData["msg"] = "Contact Successfully Edited";
                return RedirectToAction("ListContacts", new { CostCentre = cvm.CostCentre });
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                ViewBag.msg = dve.Message;
                return View();
            }
            catch (Exception exx)
            {
                ViewBag.msg = exx.Message;
                return View();
            }
        }
        public ActionResult Deactivate(Guid Id, Guid CostCentre)
        {
            try
            {
                _contactModelViewBuilder.SetInactive( Id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Deactivate", "Contact", DateTime.Now);
                TempData["msg"] = "Contact sucessfully Deactivated";

            }
            catch (Exception exx)
            {
                TempData["msg"] = exx.Message;
            }
            return RedirectToAction("ListContacts", new { CostCentre = CostCentre });
        }
        public JsonResult Owner(string bName)
        {
            IList<ContactViewModel> cvm = _contactModelViewBuilder.GetAll(true);
            return Json(cvm);
        }
        [HttpPost]
        public ActionResult LoadCostCentreAndUsers(int CostCentreID)
        {
            bool showinactive = false;
            try
            {
                if (CostCentreID == 1)
                {
                    var qryResult = _distributorViewModelBuilder.GetAll();
                    // _contactModelViewBuilder.GetAll();// _productFlavourViewModelBuilder.GetByBrand(CostCentreID);

                    return Json(new { ok = true, data = qryResult, message = "ok" });
                }
                else if (CostCentreID == 2)
                {
                    var qryResult = _userViewModelBuilder.GetContactUsers(showinactive);
                    //_userViewModelBuilder.GetAll() ;// _contactModelViewBuilder.GetAll();// _productFlavourViewModelBuilder.GetByBrand(CostCentreID);

                    return Json(new { ok = true, data = qryResult, message = "ok" });
                }
                else if (CostCentreID == 3)
                {
                    var qryResult = _outletViewModelBuilder.GetAll();
                    // _contactModelViewBuilder.GetAll();// _productFlavourViewModelBuilder.GetByBrand(CostCentreID);

                    return Json(new { ok = true, data = qryResult, message = "ok" });
                }
                else
                {
                    return Json(new { ok = true, data = "", message = "ok" });
                }

            }
            catch (Exception exx)
            {
                _log.ErrorFormat("Error in getting sub brand as per brand " + exx.Message + "Brand Id=" + CostCentreID);
                _log.InfoFormat("Error in getting sub brand as per brand " + exx.Message + "Brand Id=" + CostCentreID);
                try
                {
                    HQMailerViewModelBuilder hqm =
                        new HQMailerViewModelBuilder(ConfigurationSettings.AppSettings["ServerIP"],
                                                     ConfigurationSettings.AppSettings["UserName"],
                                                     ConfigurationSettings.AppSettings["Password"]);


                    hqm.Send(ConfigurationSettings.AppSettings["ServerEmail"],
                             ConfigurationSettings.AppSettings["MailGroup"], "Test",
                             "editing sale product error:" + exx.Message);
                }
                catch (Exception ex)
                {
                }
                return Json(new { ok = false, message = exx.Message });
            }
        }

        public String GetCostCentreUser(int CostCentreType, Guid CostCentreMasterId)
        {
            String contactsFor = "All";
            if (CostCentreType == 1)
            {
                var qryResult = _distributorViewModelBuilder.GetAll();
                // _contactModelViewBuilder.GetAll();// _productFlavourViewModelBuilder.GetByBrand(CostCentreID);
                var result = qryResult.FirstOrDefault(n => n.Id == CostCentreMasterId);
                return contactsFor = result.Name + " (distributor)";
            }
            else if (CostCentreType == 2)
            {
                var qryResult = _userViewModelBuilder.GetContactUsers();//_userViewModelBuilder.GetAll() ;// _contactModelViewBuilder.GetAll();// _productFlavourViewModelBuilder.GetByBrand(CostCentreID);
                var result = qryResult.FirstOrDefault(n => n.Id == CostCentreMasterId);
                return contactsFor = result.Name + " (user)";
            }
            else if (CostCentreType == 3)
            {
                var qryResult = _outletViewModelBuilder.GetAll();// _contactModelViewBuilder.GetAll();// _productFlavourViewModelBuilder.GetByBrand(CostCentreID);
                var result = qryResult.FirstOrDefault(n => n.Id == CostCentreMasterId);
                return contactsFor = result.Name + " (outlet)";
            }
            else
            {
                return contactsFor;
            }
        }

        public ActionResult Activate(Guid Id, Guid CostCentre)
        {
            try
            {
                _contactModelViewBuilder.SetActive(Id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Activate", "Contact", DateTime.Now);
                TempData["msg"] = "Contact sucessfully Activated";

            }
            catch (Exception exx)
            {
                TempData["msg"] = exx.Message;
            }
            return RedirectToAction("ListContacts", new { CostCentre = CostCentre });
        }

        public ActionResult Delete(Guid Id, Guid CostCentre)
        {
            try
            {
                _contactModelViewBuilder.SetAsDeleted(Id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Delete", "Contact", DateTime.Now);
                TempData["msg"] = "Contact sucessfully Deleted";

            }
            catch (Exception exx)
            {
                TempData["msg"] = exx.Message;
            }
            return RedirectToAction("ListContacts", new { CostCentre = CostCentre });
        }

        private Dictionary<int, string> AcceptedContactOwnerTypes()
        {
            var rawContactOwnerList = _contactModelViewBuilder.ContactOwner();
            var keysToRemove = new List<ContactOwnerType>() { ContactOwnerType.CommoditySupplier };

            foreach (var key in keysToRemove)
            {
                rawContactOwnerList.Remove((int)key);
            }
            return rawContactOwnerList;
        }
    }
}
