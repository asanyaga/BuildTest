using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Distributr.Core;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Repository.Master.UserRepositories;
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
using Distributr.Core.Utility.Security;
using Distributr.Core.Utility.Validation;

namespace Agrimanagr.HQ.Areas.Agrimanagr.Controllers
{
   
    public class AccountController : Controller
    {
        private const int defaultPageSize = 10;
        private IUserRepository _userRepository;
        IAuditLogViewModelBuilder _auditLogViewModelBuilder;
        private IUserViewModelBuilder _userViewModelBuilder;
        private IApplicationSetupViewModelBuilder _applicationSetupViewModelBuilder;
        private FileInfo fi = null;
        public AccountController(IUserRepository userRepository, IUserViewModelBuilder userViewModelBuilder,IAuditLogViewModelBuilder auditLogViewModelBuilder, IApplicationSetupViewModelBuilder applicationSetupViewModelBuilder)
        {
            _userRepository = userRepository;
            _userViewModelBuilder = userViewModelBuilder;
            _auditLogViewModelBuilder = auditLogViewModelBuilder;
            _applicationSetupViewModelBuilder = applicationSetupViewModelBuilder;
        }

        public ActionResult Permission()
        {
            return View("Permission");
            
        }

        public ActionResult Login(string layout)
        {
            ApplicationSetupViewModel appSetupVm = _applicationSetupViewModelBuilder.Setup(new ApplicationSetupViewModel(), VirtualCityApp.Agrimanagr);
            if (!appSetupVm.DatabaseExists)
            {
                return View("RegisterCompany",appSetupVm);
            }
            if(!appSetupVm.CompanyIsSetup)
            {
                return View("RegisterCompany", appSetupVm);
            }

            if (layout != null)
            {
                ViewBag.layout = layout;
            }

            if (!Request.IsAuthenticated && Request.QueryString["ReturnUrl"] != null)
                return View();
            if(Request.IsAuthenticated && Request.QueryString["ReturnUrl"] != null)
                return View("Permission");

            return View();
        }

        [HttpPost]
        public ActionResult Login(string layout,LogOnModel model, string returnUrl)
        {
            if (layout != null)
            {
                ViewBag.layout = layout;
            }
            if (ModelState.IsValid)
            {
                User u = _userRepository.HqLogin(model.UserName,EncryptorMD5.GetMd5Hash(model.Password));
                if (u != null)
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    ViewBag.logged = model.UserName;
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
                model.userType = UserType.AgriHQAdmin;
                if (_applicationSetupViewModelBuilder.CreateCompanyAndSuperAdmin(model))
                    return RedirectToAction("Login");
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
            if (layout != null){
                ViewBag.Layout = layout;

                }
            FormsAuthentication.SignOut();
            Response.Cookies.Clear();
            Session.Clear();
            Response.Cache.SetNoStore();
          
           return  RedirectToAction("login",new {layout = layout});
        }

        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public  ActionResult ChangePassword(string oldPassword,string newPassword,string message)
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
                catch(DomainValidationException dve)
                {
                    ValidationSummary.DomainValidationErrors(dve, ModelState);
                    return View();
                }

            catch(Exception ex)
            {
                ViewBag.msg = ex.Message;
                var msg = ViewBag.msg;

           
                
               return View();
            }
   
        }

        public  ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        //[Authorize(Roles = "RoleViewUser")]
        public ActionResult ListAllUsers(bool? showInactive, int? page)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            ViewBag.UserList = _userViewModelBuilder.uts();
           
           
                            bool showinactive = false;
                if (showInactive != null)
                    showinactive = (bool)showInactive;
                ViewBag.showInactive = showinactive;
                var ls = _userViewModelBuilder.FilterAgrimanagrHqUsers(showinactive);
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
                ////_auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Timestamp", "Account Controller:" + elapsedTime, DateTime.Now);
                return View(ls.ToPagedList(currentPageIndex, defaultPageSize));
        }

        [HttpPost]
        public ActionResult ListAllUsers(string user, string srch, bool? showInactive, int? page, UserType? UserType)
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
                     var ls = _userViewModelBuilder.SearchHqUsers(Distributr.Core.Domain.Master.UserEntities.UserType.AgriHQAdmin, user, showinactive); 
                     int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
                     return View(ls.ToPagedList(currentPageIndex, defaultPageSize));
                 }
                 else
                 {
                     return RedirectToAction("ListAllUsers", new { user = "", showinactive = showinactive, srch = false });
                 }
        }

        [Authorize(Roles = "RoleAddMasterData")]
        public  ActionResult Register()
        {
            ViewBag.UserList = _userViewModelBuilder.HqUserTypes();
            ViewBag.ProducerList = _userViewModelBuilder.Producer();
            RegisterModel model = new RegisterModel();
            return View(model);
        }

        [HttpPost]
       public  ActionResult Register(UserViewModel uvm)
        {
            if(this.User.Identity.Name == "")
            {
                return RedirectToAction("Login", new { ReturnUrl = "/Agrimanagr/Account/Register" });
            }
            ViewBag.ProducerList = _userViewModelBuilder.Producer();
            ViewBag.UserList = _userViewModelBuilder.HqUserTypes();
            try
            {
                uvm.Password = "12345678";
                uvm.Id = Guid.NewGuid();
                uvm.UserType = UserType.AgriHQAdmin;
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Create", "Account", DateTime.Now);
                _userViewModelBuilder.Save(uvm);
                Session["msg"] = "User Successfully Registered";
                var latestUser = _userViewModelBuilder.Get(uvm.Id);
                Guid userId = latestUser.Id;
                string userName = latestUser.Username;
                return RedirectToAction("CreateContact", "Contact",
                                        new
                                            {
                                                CostCentre = latestUser.CostCentre,
                                                CostCentreName = userName,
                                                ContactFor = "User"
                                            });
            }

            catch(DomainValidationException dve)
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

        //[Authorize(Roles = "RoleModifyUser")]
        public ActionResult ResetHqUserPassword(bool? showInactive, int? page)
        {
            bool showinactive = false;
            if (showInactive != null)
                showinactive = (bool)showInactive;
            ViewBag.showInactive = showinactive;
            ViewBag.costCentre = TempData["costCentre"];
            if (TempData["msgHqPass"] != null)
            {
                ViewBag.msg = TempData["msgHqPass"].ToString();
            }
            var ls = _userViewModelBuilder.GetHqUsers(showinactive);
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            return View(ls.ToPagedList(currentPageIndex, defaultPageSize));
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
                var ls = _userViewModelBuilder.SearchHqUsers(Distributr.Core.Domain.Master.UserEntities.UserType.AgriHQAdmin, username, showinactive);
                int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
                return View(ls.ToPagedList(currentPageIndex, defaultPageSize));
            }
            else
            {
              return  RedirectToAction("ResetHqUserPassword",new{showinactive=showInactive,srch="Search",username=""});
            }
            
        }

        //[Authorize(Roles = "RoleModifyUser")]
        public ActionResult ResetDistributorUserPassword(bool? showInactive, int? page,string username="")
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
                var ls = _userViewModelBuilder.SearchUsers(username,showinactive);
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

        //[Authorize(Roles = "RoleModifyUser")]
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

        //[Authorize(Roles = "RoleModifyUser")]
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

        [Authorize(Roles = "RoleDeleteMasterData")]
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

        [Authorize(Roles = "RoleUpdateMasterData")]
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
                uvm.UserType = UserType.AgriHQAdmin;
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
        return View ("ImportUsers",new UserViewModel());
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
                            

                            if (hasDuplicateName )
                            { }
                            else
                            {

                                pdvm.Username = username;
                                pdvm.Password=password;
                                pdvm.Mobile=mobile;
                                pdvm.PIN = pin;
                                pdvm.userTypeName = userType;
                                pdvm.GroupName = userGroup;
                                _userViewModelBuilder.ImportUsers(this.User.Identity.Name,pdvm);
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
