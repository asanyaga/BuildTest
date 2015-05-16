using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.Paging;
using Distributr.HQ.Lib.Validation;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Admin.User;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.UserModelBuilder;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Transaction;
using log4net;

namespace Distributr.HQ.Web.Controllers
{
    [Authorize]
    public class SupplierUsersController : Controller
    {
        //
        // GET: /SupplierUser/
        private IUserViewModelBuilder _userViewModelBuilder;
        private IAuditLogViewModelBuilder _auditLogViewModelBuilder;
        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public SupplierUsersController(IUserViewModelBuilder userViewModelBuilder, IAuditLogViewModelBuilder auditLogViewModelBuilder)
        {
            _userViewModelBuilder = userViewModelBuilder;
            _auditLogViewModelBuilder = auditLogViewModelBuilder;
        }

        public ActionResult Index()
        {
            return RedirectToAction("ListAllUsers");
        }

        [Authorize(Roles = "RoleViewUser")]
        public ActionResult ListAllUsers(bool? showInactive, string searchText, int itemsperpage = 10, int page = 1)
        {
            try
            {
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                if (itemsperpage != null)
                {
                    ViewModelBase.ItemsPerPage = itemsperpage;
                }
                bool showinactive = false;
                if (showInactive != null)
                    showinactive = (bool) showInactive;
                ViewBag.showInactive = showinactive;
                ViewBag.searchParam = searchText;

                int currentPageIndex = page < 0 ? 0 : page - 1;
                int take = itemsperpage;
                int skip = currentPageIndex*take;

                var query = new QueryUsers();
                query.ShowInactive = showinactive;
                query.Skip = skip;
                query.Take = take;
                query.Name = searchText;

                var result = _userViewModelBuilder.SupplierQuery(query);
                var count = result.Count;
                var data = result.Data;

                if (Session["msg"] != null)
                {
                    ViewBag.msg = Session["msg"].ToString();
                    Session["msg"] = null;
                }
                stopWatch.Stop();
                TimeSpan ts = stopWatch.Elapsed;


                // Format and display the TimeSpan value.
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    ts.Hours,
                    ts.Minutes,
                    ts.Seconds,
                    ts.TotalMilliseconds);

                stopWatch.Reset();
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Timestamp", "Supplier User Controller:" + elapsedTime, DateTime.Now);

                return View(data.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage, count));
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                return View();
            }
            catch (Exception ex)
            {
                _log.Debug("Failed to list supplier" + ex.Message);
                _log.Error("Failed to list supplier" + ex.ToString());
                return View();
            }
        }

        public ActionResult Register()
        {
            ViewBag.ProducerList = _userViewModelBuilder.Producer();
            ViewBag.SupplierList = _userViewModelBuilder.Suppliers();
            return View();
        }

        [HttpPost]
        public ActionResult Register(UserViewModel userViewModel)
        {
            ViewBag.ProducerList = _userViewModelBuilder.Producer();
            ViewBag.SupplierList = _userViewModelBuilder.Suppliers();
            if(userViewModel.SupplierId == Guid.Empty)
            {
                ModelState.AddModelError("", "Please select a supplier");
                return View();
            }
            try
            {
                if (userViewModel.Password.Length > 5)
                {
                    userViewModel.Id = Guid.NewGuid();
                    userViewModel.UserType = UserType.Supplier;
                    _userViewModelBuilder.Save(userViewModel);
                    _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Create", "Account", DateTime.Now);
                    Session["msg"] = "User Successfully Registered";
                    return RedirectToAction("ListAllUsers");
                }
                else
                {
                    ModelState.AddModelError("Password", "Password must be at least 6 characters");
                    return View();
                }
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

        public ActionResult EditSupplierUser(Guid Id)
        {
            UserViewModel userViewModel = _userViewModelBuilder.Get(Id);
            ViewBag.ProducerList = _userViewModelBuilder.Producer();
            ViewBag.SupplierList = _userViewModelBuilder.Suppliers();
            return View(userViewModel);
        }

        [HttpPost]
        public ActionResult EditSupplierUser(UserViewModel userViewModel)
        {
            ViewBag.ProducerList = _userViewModelBuilder.Producer();
            ViewBag.SupplierList = _userViewModelBuilder.Suppliers();
            if (userViewModel.SupplierId == Guid.Empty)
            {
                ModelState.AddModelError("", "Please select a supplier");
                return View();
            }
            try
            {
                _userViewModelBuilder.Save(userViewModel);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Edit", "Hq Users", DateTime.Now);
                Session["msg"] = "User Successfully Edited";
                return RedirectToAction("ListAllUsers");
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                return View();
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        public ActionResult Deactivate(Guid Id)
        {
            try
            {
                _userViewModelBuilder.SetInActive(Id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Deactivate", "Account", DateTime.Now);
                Session["msg"] = "User Successfully DeActivated";

            }
            catch (Exception ex)
            {
                Session["msg"] = ex.Message;

            }
            return RedirectToAction("ListAllUsers");
        }

        public ActionResult Delete(Guid Id)
        {
            try
            {
                if (Id != null)
                {
                    _userViewModelBuilder.Delete(Id);
                    _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Delete", "Account", DateTime.Now);
                    Session["msg"] = "User Successfully deleted";
                    RedirectToAction("ListAllUsers");
                }

            }
            catch (Exception ex)
            {
                Session["msg"] = ex.Message;

            }
            return RedirectToAction("ListAllUsers");
        }

        public ActionResult Activate(Guid Id)
        {
            try
            {
                if (Id != null)
                {
                    _userViewModelBuilder.Activate(Id);
                    _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Delete", "Account", DateTime.Now);
                    Session["msg"] = "User Successfully Activated";
                    RedirectToAction("ListAllUsers");
                }

            }
            catch (Exception ex)
            {
                Session["msg"] = ex.Message;

            }
            return RedirectToAction("ListAllUsers");
        }

        [Authorize(Roles = "RoleModifyUser")]
        public ActionResult AssignRoles(Guid Id)
        {
            UserViewModel uvm = _userViewModelBuilder.Get(Id);
            ViewBag.GroupList = _userViewModelBuilder.UserGroup();//.Where(k=>k.Value != "Admin");

            return View(uvm);
        }

        [HttpPost]
        public ActionResult AssignRoles(UserViewModel userViewModel)
        {
            ViewBag.GroupList = _userViewModelBuilder.UserGroup();
            try
            {
                UserViewModel uvm3 = _userViewModelBuilder.Get(userViewModel.Id);
                uvm3.Group = userViewModel.Group;
                _userViewModelBuilder.AssignRole(uvm3);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Assign", "User", DateTime.Now);
                TempData["msg"] = "Successfully Saved";
                return RedirectToAction("ListAllUsers");
            }
            catch (Exception ex)
            {
                ViewBag.lsUsers = ex.Message;
                return View();
            }
        }

        public ActionResult ResetPassword(Guid Id)
        {
            try
            {
                _userViewModelBuilder.ResetPassword(Id);
                Session["msg"] = "Password Successfully Reset";
                return RedirectToAction("ListAllUsers");
            }
            catch (Exception ex)
            {
                ViewBag.lsUsers = ex.Message;
                return RedirectToAction("ListAllUsers");
            }
        }
    }
}
