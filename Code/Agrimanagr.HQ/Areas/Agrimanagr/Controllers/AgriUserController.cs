using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Mvc;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.Paging;
using Distributr.HQ.Lib.Validation;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Transaction;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.UserModelBuilder;
using Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.AgriUserViewModelBuilders;
using Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.CostCentreViewModelBuilders;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Admin.User;
using Distributr.HQ.Lib.ViewModels.Agrimanagr.CostCentreViewModel;
using log4net;

namespace Agrimanagr.HQ.Areas.Agrimanagr.Controllers
{
    [Authorize]
    public class AgriUserController : Controller
    {
        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        IAgriUserViewModelBuilder _agriUserViewModelBuilder;
        IUserViewModelBuilder _userViewModelBuilder;
        IAuditLogViewModelBuilder _auditLogViewModelBuilder;

        public AgriUserController(IAgriUserViewModelBuilder agriUserViewModelBuilder, IUserViewModelBuilder userViewModelBuilder, IAuditLogViewModelBuilder auditLogViewModelBuilder)
        {
            _agriUserViewModelBuilder = agriUserViewModelBuilder;
            _userViewModelBuilder = userViewModelBuilder;
            _auditLogViewModelBuilder = auditLogViewModelBuilder;
        }

        public ActionResult Index()
        {
            
            return View();
        }

        [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListAgriUsers(bool? showInactive, string userType, int itemsperpage = 10, int page = 1, string srchParam="")
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
                ViewBag.SearchParam = srchParam;

                int currentPageIndex = page - 1 < 0 ? 0 : page - 1;
                int take = itemsperpage;
                int skip = currentPageIndex*take;
               // var listOfUsers =new  List<int>();
                
                //listOfUsers.Add((int)UserType.HubManager);
                //listOfUsers.Add((int)UserType.Clerk);
                //listOfUsers.Add((int)UserType.PurchasingClerk);
                //listOfUsers.Add(());


                var query = new QueryUsers() { Name = srchParam, ShowInactive = showinactive, Skip = skip, Take = take };

                var ls = _agriUserViewModelBuilder.Query(query);
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

        [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult CreateAgriUser()
        {
            ViewBag.CostCentreList = _agriUserViewModelBuilder.CostCentre();
            ViewBag.UserList = _agriUserViewModelBuilder.uts();

            return View("CreateAgriUser", new AgriUserViewModel());
        }

        [HttpPost]
        public ActionResult CreateAgriUser(AgriUserViewModel uvm)
        {

            try
            {
                uvm.Id = Guid.NewGuid();
                uvm.Password = "12345678";
                _agriUserViewModelBuilder.Save(uvm);
                TempData["msg"] = "User Succesfully Created";

                ViewBag.showInactive = false;
                return RedirectToAction("ListAgriUsers");

            }
            catch (DomainValidationException dve)
            {

                ViewBag.UserList = _agriUserViewModelBuilder.uts();
                ViewBag.CostCentreList = _agriUserViewModelBuilder.CostCentre();
                ValidationSummary.DomainValidationErrors(dve, ModelState);

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.UserList = _agriUserViewModelBuilder.uts();
                ViewBag.CostCentreList = _agriUserViewModelBuilder.CostCentre();
                ViewBag.lsUsers = ex.Message;

                return View();
            }

        }

        [Authorize(Roles = "RoleUpdateMasterData")]
        public ActionResult EditAgriUser(Guid id)
        {

            AgriUserViewModel uvm = _agriUserViewModelBuilder.Get(id);
            ViewBag.UserList = _agriUserViewModelBuilder.uts();
            ViewBag.CostCentreList = _agriUserViewModelBuilder.CostCentre();
            return View(uvm);
        }

        [HttpPost]
        public ActionResult EditAgriUser(AgriUserViewModel uvm)
        {
            ViewBag.CostCentreList = _agriUserViewModelBuilder.CostCentre();
            ViewBag.UserList = _agriUserViewModelBuilder.uts();
            try
            {
                _agriUserViewModelBuilder.Save(uvm);
                TempData["msg"] = "User Succesfully Edited";
                return RedirectToAction("ListAgriUsers");
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

        [Authorize(Roles = "RoleUpdateMasterData")]
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
                return RedirectToAction("ListAgriUsers");
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
                _agriUserViewModelBuilder.SetInActive(id);
                TempData["msg"] = "Successfully deactivated";
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                TempData["msg"] = dve.Message;
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
            }
            return RedirectToAction("ListAgriUsers");
        }

        public ActionResult Activate(Guid id)
        {
            try
            {
                _agriUserViewModelBuilder.Activate(id);
                TempData["msg"] = "Successfully activated";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
            }
            return RedirectToAction("ListAgriUsers");
        }

        [Authorize(Roles = "RoleDeleteMasterData")]
        public ActionResult Delete(Guid id)
        {
            try
            {
                _agriUserViewModelBuilder.Delete(id);
                TempData["msg"] = "Successfully deleted";
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                TempData["msg"] = dve.Message;
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
            }
            return RedirectToAction("ListAgriUsers");
        }
    }
}
