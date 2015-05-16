using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Agrimanagr.HQ.Models;
using Distributr.Core;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Security;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.ApplicationSetupViewModelBuilders;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.UserModelBuilder;
using Distributr.HQ.Lib.Validation;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Admin.ApplicationSetupViewModel;
using Distributr.HQ.Lib.ViewModels.Admin.User;
using Distributr.HQ.Lib.Paging;
using System.Diagnostics;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Transaction;
using System.Data.OleDb;
using System.IO;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Security;
using Distributr.Core.Utility.Validation;
using log4net;
using StructureMap;

namespace Distributr.HQ.Web.Controllers
{

    public class AccountController : Controller
    {
        private const int defaultPageSize = 10;
        private IUserRepository _userRepository;
        private IAuditLogViewModelBuilder _auditLogViewModelBuilder;
        private IUserViewModelBuilder _userViewModelBuilder;
        private IApplicationSetupViewModelBuilder _applicationSetupViewModelBuilder;
        private FileInfo fi = null;
        private ISecurityService _securityService;
        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public AccountController(IUserRepository userRepository, IUserViewModelBuilder userViewModelBuilder,
                                 IAuditLogViewModelBuilder auditLogViewModelBuilder,
                                 IApplicationSetupViewModelBuilder applicationSetupViewModelBuilder, ISecurityService securityService)
        {
            _userRepository = userRepository;
            _userViewModelBuilder = userViewModelBuilder;
            _auditLogViewModelBuilder = auditLogViewModelBuilder;
            _applicationSetupViewModelBuilder = applicationSetupViewModelBuilder;
            _securityService = securityService;
        }

        public ActionResult Permission()
        {
            return View("Permission");

        }

        public ActionResult LogOn(string layout)
        {
            ApplicationSetupViewModel appSetupVm =
                _applicationSetupViewModelBuilder.Setup(new ApplicationSetupViewModel(), VirtualCityApp.Ditributr);
            if (!appSetupVm.DatabaseExists)
            {
                return View("RegisterCompany", appSetupVm);
            }
            if (!appSetupVm.CompanyIsSetup)
            {
                return View("RegisterCompany", appSetupVm);
            }

            if (layout != null)
            {
                ViewBag.layout = layout;
            }
            if (Request.QueryString["ReturnUrl"] == null)
                return View(); // user asked for login form
            else if (Request.IsAuthenticated)
                return View("Permission"); // insufficient rights
            else
                return View();

        }

        [HttpPost]
        public ActionResult LogOn(string layout, LogOnModel model, string returnUrl)
        {
            if (layout != null)
            {
                ViewBag.layout = layout;
            }
            if (ModelState.IsValid)
            {
              

                User u = _userRepository.HqLogin(model.UserName, EncryptorMD5.GetMd5Hash(model.Password));
                
                if (u != null)
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                 
                    if (Url.IsLocalUrl(returnUrl))
                    {

                        return Redirect(returnUrl);
                    }
                    else
                    {

                        return RedirectToAction("Index", "Home");
                    }
                }

                ModelState.AddModelError("", "The user name or password provided is incorrect.");

            }

            return View(model);
        }

        [HttpGet]
        public ActionResult RegisterCompany()
        {
            return View();
        }

    [HttpPost]
        public ActionResult RegisterCompany(ApplicationSetupViewModel model)
        {
            try
            {
                model.userType = UserType.HQAdmin;
                if (_applicationSetupViewModelBuilder.CreateCompanyAndSuperAdmin(model))
                    return RedirectToAction("Logon");
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                return View(model);
            }
            catch (Exception exx)
            {
                ViewBag.msg = exx.Message;
                return View(model);
            }
            return null;
        }

        public ActionResult Logoff(string layout)
        {
            if (layout != null)
            {
                ViewBag.Layout = layout;

            }
            string cachedKey = string.Format("cui_{0}", this.User.Identity.Name);
            System.Web.HttpContext.Current.Cache.Remove(cachedKey);
            //FormsAuthentication.SignOut();
            //Response.Cookies.Clear();
            //Session.Clear();
            //Response.Cache.SetNoStore();

            FormsAuthentication.SignOut();
            Session.Abandon();

            // clear authentication cookie
            HttpCookie cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            cookie1.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie1);

            // clear session cookie (not necessary for your current problem but i would recommend you do it anyway)
            HttpCookie cookie2 = new HttpCookie("ASP.NET_SessionId", "");
            cookie2.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie2);

            FormsAuthentication.RedirectToLoginPage();


            return RedirectToAction("logon", new { layout = layout });
        }

        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(string oldPassword, string newPassword, string message)
        {

            try
            {
                if (oldPassword == newPassword)
                {
                    ViewBag.msg = "New password should not be equal to old password";
                    return View();
                }
                else
                {

                    _userViewModelBuilder.ChangePassword(oldPassword, newPassword, this.User.Identity.Name);
                    _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Edit", "Change Password", DateTime.Now);
                    return RedirectToAction("ChangePasswordSuccess");
                }
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                return View();
            }

            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                var msg = ViewBag.msg;



                return View();
            }

        }

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        [Authorize(Roles = "RoleViewUser")]
        public ActionResult ListAllUsers(bool? showInactive, int? page, string searchText)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            ViewBag.UserList = _userViewModelBuilder.uts();


            bool showinactive = false;
            if (showInactive != null)
                showinactive = (bool)showInactive;
            ViewBag.showInactive = showinactive;
            //Important Commented Code PLiz dont delete//////

            //var ls = _userViewModelBuilder.GetAll(showinactive);
            //var ls = _userViewModelBuilder.FilterHqUsersSkipTake(showinactive);
            //int currentPageIndex = page.HasValue ? page.Value - 1 : 0;

            ///end////
            var ls = _userViewModelBuilder.FilterDistributrHqUsers(showinactive);
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;

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
            if (showinactive == false)
            {
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Timestamp", "Account Controller:" + elapsedTime, DateTime.Now);
            }
           
            return View(ls.ToPagedList(currentPageIndex, defaultPageSize));
        }

        [HttpPost]
        public ActionResult ListAllUsers(string searchText, string srch, bool? showInactive, int? page, UserType? UserType)
        {
            string command = srch;
            bool showinactive = false;
            if (showInactive != null)
                showinactive = (bool)showInactive;
            ViewBag.UserList = _userViewModelBuilder.uts();
            ViewBag.showInactive = showinactive;
            ViewBag.costCentre = TempData["costCentre"];
            if (command == "Search")
            {
                ViewBag.searchParam = searchText;
                var ls = _userViewModelBuilder.SearchHqUsers(Distributr.Core.Domain.Master.UserEntities.UserType.HQAdmin, searchText, showinactive);
                int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
                return View(ls.ToPagedList(currentPageIndex, defaultPageSize));
            }
            else
            {
                return RedirectToAction("ListAllUsers", new { user = "", showinactive = showinactive, srch = false });
            }
        }

        public ActionResult Register()
        {
            ViewBag.UserList = _userViewModelBuilder.HqUserTypes();
            ViewBag.ProducerList = _userViewModelBuilder.Producer();
            return View();
        }

        [HttpPost]
        public ActionResult Register(UserViewModel uvm)
        {
            ViewBag.ProducerList = _userViewModelBuilder.Producer();
            ViewBag.UserList = _userViewModelBuilder.HqUserTypes();
            try
            {
                if (uvm.Password.Length > 5)
                {
                    uvm.Id = Guid.NewGuid();
                    _userViewModelBuilder.Save(uvm);
                    _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Create", "Account", DateTime.Now);
                    Session["msg"] = "User Successfully Registered";
                    var latestUser = _userViewModelBuilder.Get(uvm.Id);
                    Guid userId = latestUser.Id;
                    string userName = latestUser.Username;
                    return RedirectToAction("CreateContact", "Admin/Contact", new { CostCentre = latestUser.CostCentre, CostCentreName = userName, ContactFor = "User" });
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

        [Authorize(Roles = "RoleModifyUser")]
        public ActionResult ResetHqUserPassword(Boolean? showInactive, string searchParam = "", int page = 1)
        {

          
                bool showinactive = false;
                if (showInactive != null)
                    showinactive = (bool) showInactive;
                ViewBag.showInactive = showinactive;
                ViewBag.costCentre = TempData["costCentre"];
                if (TempData["msgHqPass"] != null)
                {
                    ViewBag.msg = TempData["msgHqPass"].ToString();
                }
                // var ls = _userViewModelBuilder.GetHqUsers(showinactive);
                int currentPageIndex = page - 1 < 0 ? 0 : page - 1;
                var take = DistributorWebHelper.GetItemPerPage();
                var skip = currentPageIndex*take;
                var query = new QueryUsers {Name = searchParam, ShowInactive = showinactive, Skip = skip, Take = take};
                var ls = _userViewModelBuilder.Query(query);
                var total = ls.Count;
                var data = ls.Data;
               // var data = item.ToPagedList(currentPageIndex, take, total);


                return View(data.ToPagedList(currentPageIndex, take, total));

        }
    


        [HttpPost]
        public ActionResult ResetHqUserPassword(bool? showInactive, int? page, string srch, string username)
        {
            bool showinactive = false;
            string command = srch;
            if (showInactive != null)
                showinactive = (bool)showInactive;
            ViewBag.showInactive = showinactive;
            ViewBag.costCentre = TempData["costCentre"];
            if (TempData["msgHqPass"] != null)
            {
                ViewBag.msg = TempData["msgHqPass"].ToString();

            }

            if (command == "Search")
            {
                var ls = _userViewModelBuilder.SearchHqUsers(Distributr.Core.Domain.Master.UserEntities.UserType.HQAdmin, username, showinactive);
                int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
                return View(ls.ToPagedList(currentPageIndex, defaultPageSize));
            }
            else
            {
                return RedirectToAction("ResetHqUserPassword", new { showinactive = showInactive, srch = "Search", username = "" });
            }

        }

        [Authorize(Roles = "RoleModifyUser")]
        public ActionResult ResetDistributorUserPassword(bool? showInactive, int? page, string username = "")
        {
            ViewBag.costCentre = _userViewModelBuilder.Distributor();
            bool showinactive = false;
            if (showInactive != null)
                showinactive = (bool)showInactive;
            ViewBag.showInactive = showinactive;
            if (TempData["msgDistPass"] != null)
            {
                ViewBag.msg = TempData["msgDistPass"].ToString();
            }
            var ls = _userViewModelBuilder.GetAllDistributorUsers(Guid.Empty, showinactive);
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            return View(ls.ToPagedList(currentPageIndex, defaultPageSize));
        }

        [HttpPost]
        public ActionResult ResetDistributorUserPassword(bool? showInactive, int? page, string srch, string username, Guid? costCentre)
        {
            bool showinactive = false;
            string command = srch;
            if (showInactive != null)
                showinactive = (bool)showInactive;
            ViewBag.showInactive = showinactive;
            ViewBag.costCentre = _userViewModelBuilder.Distributor();
            if (TempData["msgDistPass"] != null)
            {
                ViewBag.msg = TempData["msgDistPass"].ToString();
                TempData["msgDistPass"] = null;
            }

            if (command == "Search")
            {
                costCentre = costCentre ?? Guid.Empty;
                var ls = _userViewModelBuilder.SearchDistributorUsers(username, costCentre, showinactive);
                int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
                return View(ls.ToPagedList(currentPageIndex, defaultPageSize));
            }
            else if (command == "Load Users")
            {
                if (costCentre != null)
                {
                    var ls = _userViewModelBuilder.GetAllDistributorUsers(costCentre.Value, showinactive);
                    int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
                    return View(ls.ToPagedList(currentPageIndex, defaultPageSize));
                }
                else
                {
                    var ls = _userViewModelBuilder.GetAllDistributorUsers(Guid.Empty, showinactive);
                    int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
                    return View(ls.ToPagedList(currentPageIndex, defaultPageSize));
                }
            }
            else
            {
                return RedirectToAction("ResetDistributorUserPassword", new { showinactive = showInactive, srch = "Load Users", username = "", costCentre = costCentre });
            }

        }

        [Authorize(Roles = "RoleModifyUser")]
        public ActionResult ResetPassword(Guid id)
        {

            try
            {
                string userName = _userViewModelBuilder.Get(id).Username;
                _userViewModelBuilder.ResetPassword(id);
                TempData["msgHqPass"] = "Password for " + userName + " successfully reset";
            }
            catch (Exception exx)
            {
                ViewBag.msg = exx.Message;
                TempData["msgHqPass"] = "Password reset unsuccessfully";
            }
            return RedirectToAction("ResetHqUserPassword");
        }

        [Authorize(Roles = "RoleModifyUser")]
        public ActionResult ResetDistUserPassword(Guid id)
        {

            try
            {
                string userName = _userViewModelBuilder.Get(id).Username;
                _userViewModelBuilder.ResetPassword(id);
                TempData["msgDistPass"] = "Password for " + userName + "  successfully reset";
            }
            catch (Exception exx)
            {
                ViewBag.msg = exx.Message;
                TempData["msgDistPass"] = "Password reset unsuccessfully";
            }
            return RedirectToAction("ResetDistributorUserPassword");
        }

        public ActionResult DeActivate(Guid id)
        {

            try
            {
                _userViewModelBuilder.SetInActive(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Deactivate", "Account", DateTime.Now);
                Session["msg"] = "User Successfully DeActivated";

            }
            catch (Exception ex)
            {
                Session["msg"] = ex.Message;

            }
            return RedirectToAction("ListAllUsers");
        }

        public ActionResult Activate(Guid id)
        {
            try
            {
                _userViewModelBuilder.Activate(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Activate", "Account", DateTime.Now);
                Session["msg"] = "User Successfully activated";

            }
            catch (Exception ex)
            {
                Session["msg"] = ex.Message;

            }
            return RedirectToAction("ListAllUsers");
        }

        public ActionResult Delete(Guid id)
        {
            try
            {
                _userViewModelBuilder.Delete(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Delete", "Account", DateTime.Now);
                Session["msg"] = "User Successfully deleted";

            }
            catch (Exception ex)
            {
                Session["msg"] = ex.Message;

            }
            return RedirectToAction("ListAllUsers");
        }

        [Authorize(Roles = "RoleModifyUser")]
        public ActionResult EditHqUsers(Guid id)
        {

            UserViewModel uvm = _userViewModelBuilder.Get(id);

            ViewBag.UserList = _userViewModelBuilder.uts();
            ViewBag.ProducerList = _userViewModelBuilder.Producer();
            ViewBag.CostCentreList = _userViewModelBuilder.CostCentre();
            return View(uvm);
        }

        [HttpPost]
        public ActionResult EditHqUsers(UserViewModel uvm)
        {
            //Debugger.Break();
            ViewBag.CostCentreList = _userViewModelBuilder.CostCentre();
            ViewBag.ProducerList = _userViewModelBuilder.Producer();
            ViewBag.UserList = _userViewModelBuilder.uts();
            try
            {
                _userViewModelBuilder.Save(uvm);
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

        public ActionResult ImportUsers()
        {
            return View("ImportUsers", new UserViewModel());
        }

        [HttpPost]
        public ActionResult ImportUsers(HttpPostedFileBase file)
        {

            try
            {

                // extract only the fielname
                var fileName = Path.GetFileName(file.FileName);

                // var pathh = Path.Combine(Server.MapPath("./Uploads"), fileName);
                var directory = Server.MapPath("~/Uploads");

                if (Directory.Exists(directory) == false)
                {
                    Directory.CreateDirectory(directory);
                }
                var path = Server.MapPath("~/Uploads") + "\\" + fileName;

                file.SaveAs(path);

                string fileExtension = Path.GetExtension(fileName);
                if (fileExtension == ".xlsx")
                {
                    ViewBag.msg = "Please wait. Upload in progress";

                    string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties='Excel 12.0;HDR=YES;'";

                    OleDbConnection conn = new OleDbConnection(connectionString);
                    try
                    {
                        conn.Open();
                        OleDbCommand command = new OleDbCommand("SELECT username,password,pin,mobile,usertype,usergroup FROM [Sheet1$]", conn);
                        OleDbDataReader reader = command.ExecuteReader();
                        UserViewModel pdvm = new UserViewModel();
                        while (reader.Read())
                        {
                            string username = reader["username"].ToString().ToLower();
                            string password = reader["password"].ToString().ToLower();

                            string pin = reader["pin"].ToString();
                            string mobile = reader["mobile"].ToString();
                            string userType = reader["usertype"].ToString();
                            string userGroup = reader["usergroup"].ToString();
                            bool hasDuplicateName = _userViewModelBuilder.GetAll()
                .Any(p => p.Username == username);


                            if (hasDuplicateName)
                            { }
                            else
                            {

                                pdvm.Username = username;
                                pdvm.Password = password;
                                pdvm.Mobile = mobile;
                                pdvm.PIN = pin;
                                pdvm.userTypeName = userType;
                                pdvm.GroupName = userGroup;
                                _userViewModelBuilder.ImportUsers(this.User.Identity.Name, pdvm);
                            }
                        }
                    }
                    catch (OleDbException ex)
                    {
                        ViewBag.msg = ex.ToString();
                        return View();
                    }

                    finally
                    {
                        conn.Close();

                    }

                    fi = new FileInfo(path);

                    fi.Delete();
                    _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Import", "Users", DateTime.Now);
                    ViewBag.msg = "Upload Successful";
                    return RedirectToAction("ListAllUsers");
                }

                else
                {
                    fi = new FileInfo(path);

                    fi.Delete();
                    ViewBag.msg = "Please upload excel file with extension .xlsx";
                    return View();
                }
            }
            catch (Exception ex)
            {

                ViewBag.msg = ex.ToString();
                return View();
            }


        }
    }
}
