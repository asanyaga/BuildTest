using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.Paging;
using Distributr.HQ.Lib.Validation;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Transaction;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.UserModelBuilder;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Admin.User;
using log4net;

namespace Distributr.HQ.Web.Controllers
{
    [Authorize]
    public class HQUsersController : Controller
    {
        private IUserViewModelBuilder _userViewModelBuilder;
        private AuditLogViewModelBuilder _auditLogViewModelBuilder;
        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public HQUsersController(UserViewModelBuilder userViewModelBuilder, AuditLogViewModelBuilder auditLogViewModelBuilder)
        {
            _userViewModelBuilder = userViewModelBuilder;
            _auditLogViewModelBuilder = auditLogViewModelBuilder;
        }

        public ActionResult Index()
        {
            return RedirectToAction("ListAllUsers");
        }

        public ActionResult RegisterUser()
        {
            ViewBag.ProducerList = _userViewModelBuilder.Producer();
            ViewBag.UserList = _userViewModelBuilder.uts().Where(k => k.Value == "SalesRep" || k.Value == "Surveyor" || k.Value == "HQAdmin" || k.Value == "ASM");
            return View();
        }
        [HttpPost]
        public ActionResult RegisterUser(UserViewModel uvm)
        {
            ViewBag.ProducerList = _userViewModelBuilder.Producer();
            ViewBag.UserList = _userViewModelBuilder.uts().Where(k => k.Value == "SalesRep" || k.Value == "Surveyor" || k.Value == "HQAdmin" || k.Value == "ASM");

            try
            {
                if (uvm.Password.Length > 5)
                {
                    uvm.Id = Guid.NewGuid();
                    _userViewModelBuilder.Save(uvm);
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
                    showinactive = (bool)showInactive;
                ViewBag.showInactive = showinactive;
                ViewBag.searchParam = searchText;

                int currentPageIndex = page < 0 ? 0 : page - 1;
                int take = itemsperpage;
                int skip = currentPageIndex * take;

                var query = new QueryUsers();
                query.ShowInactive = showinactive;
                query.Skip = skip;
                query.Take = take;
                query.Name = searchText;

                var result = _userViewModelBuilder.HQQuery(query);
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
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Timestamp",
                                                      "Supplier User Controller:" + elapsedTime, DateTime.Now);

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

        public ActionResult EditUser(Guid id)
        {
            UserViewModel userViewModel = _userViewModelBuilder.Get(id);
            ViewBag.ProducerList = _userViewModelBuilder.Producer();
            ViewBag.UserList = _userViewModelBuilder.uts().Where(k => k.Value == "SalesRep" || k.Value == "Surveyor" || k.Value == "HQAdmin" || k.Value == "ASM");
            return View(userViewModel);
        }
        [HttpPost]
        public ActionResult EditUser(UserViewModel userViewModel)
        {
            ViewBag.ProducerList = _userViewModelBuilder.Producer();
            ViewBag.UserList = _userViewModelBuilder.uts().Where(k => k.Value == "SalesRep" || k.Value == "Surveyor" || k.Value == "HQAdmin" || k.Value == "ASM");
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

        [Authorize(Roles = "RoleModifyUser")]
        public ActionResult AssignRoles(Guid Id)
        {
            UserViewModel uvm = _userViewModelBuilder.Get(Id);
            ViewBag.GroupList = _userViewModelBuilder.UserGroup();

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
                Session["msg"] = "Successfully Saved";
                return RedirectToAction("ListAllUsers");
            }
            catch (Exception ex)
            {
                ViewBag.lsUsers = ex.Message;
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
