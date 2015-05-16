using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.Core.Utility;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.UserModelBuilder;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Admin.User;
using Distributr.HQ.Lib.Validation;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.HQ.Lib.Helper;
using Distributr.HQ.Lib.Paging;
using log4net;
using System.Reflection;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Transaction;


namespace Distributr.HQ.Web.Areas.Admin.Controllers.user
{
    [Authorize ]
    [HandleError]
    public class UserController : Controller
    { 
        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        IUserViewModelBuilder _userViewModelBuilder;
        IAuditLogViewModelBuilder _auditLogViewModelBuilder;
        public UserController(IUserViewModelBuilder userViewModelBuilder,IAuditLogViewModelBuilder auditLogViewModelBuilder)
        {
            _userViewModelBuilder = userViewModelBuilder;
             _auditLogViewModelBuilder=auditLogViewModelBuilder;
        }

        public ActionResult Index()
        {
            
            return View();
        }
      //  [Authorize(Roles = "RoleViewUser")]
        public ActionResult ListUsers(bool? showInactive, string searchText, int? userType, int page = 1, int itemsperpage = 10)
        {
            ViewBag.UserList = _userViewModelBuilder.utsDistributor();
            //string filter = userType;
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
                ViewBag.searchParam = searchText;

                int currentPageIndex = page < 0 ? 0 : page - 1;
                int take = itemsperpage;
                int skip = currentPageIndex*take;

                var query = new QueryUsers()
                {
                    ShowInactive = showinactive,
                    Name = searchText,
                    Skip = skip,
                    Take = take,
                };
                List<int> userTypes;
               
                if(userType.HasValue)
                {
                    userTypes = new List<int>();
                    userTypes.Add(userType.Value);
                    query.UserTypes = userTypes;
                }

               

                var ls = _userViewModelBuilder.Query(query);
                var data = ls.Data;
                var count = ls.Count;

                return View(data.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage, count));
            }
            catch (Exception ex)
            {
                ViewBag.lsUsers = ex.ToString();
                return View();
            }
            
        }
        
        public ActionResult UserDetails(Guid id)
        {
            UserViewModel user = _userViewModelBuilder.Get(id);
            return View(user);
        }
        [Authorize(Roles = "RoleAddUser")]
        public ActionResult CreateUser()
        {
            ViewBag.CostCentreList = _userViewModelBuilder.CostCentre();
            ViewBag.UserList = _userViewModelBuilder.uts();

            return View("CreateUser", new UserViewModel());
        }

        [HttpPost]
        public ActionResult CreateUser(UserViewModel uvm)
        {

            try
            {
                if (uvm.Password.Length > 5)
                {
                    uvm.Id = Guid.NewGuid();
                    _userViewModelBuilder.Save(uvm);
                    _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Create", "User", DateTime.Now);
                    TempData["msg"] = "User Succesfully Created";
                    var latestUser =_userViewModelBuilder.Get(uvm.Id);
                           
                    Guid userId = latestUser.Id;
                    string userName = latestUser.Username;
                    return RedirectToAction("CreateContact", "Contact",
                                            new { CostCentre = latestUser.CostCentre, CostCentreName = userName, ContactFor = "User" });
                }
                else
                {
                    ViewBag.UserList = _userViewModelBuilder.uts();
                    ViewBag.CostCentreList = _userViewModelBuilder.CostCentre();
                    ModelState.AddModelError("Password", "Password must be at least 6 characters");
                    return View();
                }

            }
            catch (DomainValidationException dve)
            {

                ViewBag.UserList = _userViewModelBuilder.uts();
                ViewBag.CostCentreList = _userViewModelBuilder.CostCentre();
                ValidationSummary.DomainValidationErrors(dve, ModelState);

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.UserList = _userViewModelBuilder.uts();
                ViewBag.CostCentreList = _userViewModelBuilder.CostCentre();
                ViewBag.lsUsers = ex.Message;

                return View();
            }

        }

        [Authorize(Roles = "RoleModifyUser")]
        public ActionResult EditUser(Guid id)
        {
            
            UserViewModel uvm = _userViewModelBuilder.Get(id);
            ViewBag.UserList = _userViewModelBuilder.uts();
            ViewBag.CostCentreList = _userViewModelBuilder.CostCentre();
            return View(uvm);
        }

        [HttpPost]
        public ActionResult EditUser(UserViewModel uvm)
        {
            ViewBag.CostCentreList = _userViewModelBuilder.CostCentre();
            ViewBag.UserList = _userViewModelBuilder.uts();
            ViewBag.UserList = _userViewModelBuilder.utsDistributor();
            try
            {
                _userViewModelBuilder.Save(uvm);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Edit", "User", DateTime.Now);
                TempData["msg"] = "User Succesfully Edited";
                return RedirectToAction("listusers");
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.lsUsers = ex.Message;
                return View();
            }
        }

        public ActionResult DeActivate(Guid id)
        {
            try
            {
                _userViewModelBuilder.SetInActive(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Deactivate", "User", DateTime.Now);
                TempData["msg"] = "Successfully deactivated";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
            }
            return RedirectToAction("ListUsers");
        }

        public ActionResult Activate(Guid id)
        {
            try
            {
                _userViewModelBuilder.Activate(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Deactivate", "User", DateTime.Now);
                TempData["msg"] = "Successfully activated";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
            }
            return RedirectToAction("ListUsers");
        }

        public ActionResult Delete(Guid id)
        {
            try
            {
                _userViewModelBuilder.Delete(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Deactivate", "User", DateTime.Now);
                TempData["msg"] = "Successfully deleted";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
            }
            return RedirectToAction("ListUsers");
        }

        public JsonResult Owner(string blogName)
        {
            IList<UserViewModel> users = _userViewModelBuilder.GetAll(true);
            return Json(users);
        }
        [Authorize(Roles = "RoleModifyUser")]
        public ActionResult AssignRoles(Guid id)
        {
            //ViewBag.CostCenter = _userViewModelBuilder.GetAll().OrderBy(n => n.CostCentre).ToList();
            UserViewModel uvm = _userViewModelBuilder.Get(id);
            ViewBag.GroupList = _userViewModelBuilder.UserGroup();
          
            return View(uvm);
        }

        [HttpPost]
        public ActionResult AssignRoles(UserViewModel uvm)
        {
            ViewBag.GroupList = _userViewModelBuilder.UserGroup();
            try
            {
                UserViewModel uvm3 = _userViewModelBuilder.Get(uvm.Id);
                uvm3.Group = uvm.Group;
                _userViewModelBuilder.AssignRole(uvm3);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Assign", "User", DateTime.Now);
                TempData["msg"] = "Successfully Saved";
                //return RedirectToAction("ListAllUsers", "Account");
                return Redirect("~/Account/ListAllUsers");
               // return RedirectToAction("listusers");
            }
            catch (Exception ex)
            {
                ViewBag.lsUsers = ex.Message;
                return View();
            }
        }
    }
}
