using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.Paging;
using Distributr.HQ.Lib.Validation;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Transaction;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.UserModelBuilder;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Admin.User;

namespace Agrimanagr.HQ.Areas.Agrimanagr.Controllers
{
    public class AgriUserGroupController : Controller
    {
        private IUserGroupVeiwModelBuilder _userGroupVeiwModelBuilder;
        IAuditLogViewModelBuilder _auditLogViewModelBuilder;
        private IUserGroupRoleVeiwModelBuilder _userGroupRoleVeiwModelBuilder;
        public AgriUserGroupController(IUserGroupVeiwModelBuilder userGroupVeiwModelBuilder, IUserGroupRoleVeiwModelBuilder userGroupRoleVeiwModelBuilder, IAuditLogViewModelBuilder auditLogViewModelBuilder)
        {
            _userGroupVeiwModelBuilder = userGroupVeiwModelBuilder;
            _userGroupRoleVeiwModelBuilder = userGroupRoleVeiwModelBuilder;
            _auditLogViewModelBuilder=auditLogViewModelBuilder;
        }
        [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult List(bool? showInactive, string srchParam = "", int page = 1, int itemsperpage = 10)
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
                ViewBag.SearchText = srchParam;
                
                int currentPageIndex = page < 1 ? 0 : page - 1;
                int take = itemsperpage;
                int skip = currentPageIndex*take;

                var query = new QueryStandard() {ShowInactive = showinactive, Skip = skip, Take = take, Name = srchParam};

                var ls = _userGroupVeiwModelBuilder.Query(query);
                var data = ls.Data;
                var count = ls.Count;

                return View(data.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage, count));
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }

        }
        
        [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult CreateGroup()
        {
           return View(new UserGroupVeiwModel());
        }
        [HttpPost]
        public ActionResult CreateGroup(UserGroupVeiwModel vw)
        {
            try
            {
                vw.Id = Guid.NewGuid();
                _userGroupVeiwModelBuilder.Save(vw);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Create", "User Group", DateTime.Now);
                TempData["msg"] = "User Group Successfully Created";
                return RedirectToAction("list");
            }
            catch (DomainValidationException dve)
            {

                //string msg = ValidationSummary.DomainValidationErrors(dve);
                //ViewBag.msg = msg;
                ValidationSummary.DomainValidationErrors(dve,ModelState);
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;

                return View();
            }
        }
        [Authorize(Roles = "RoleModifyMasterData")]
        public ActionResult Edit(Guid Id)
        {
            return View(_userGroupVeiwModelBuilder.Get(Id));
        }
        [HttpPost]
        public ActionResult Edit(UserGroupVeiwModel vw)
        {
            try
            {
                _userGroupVeiwModelBuilder.Save(vw);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Edit", "User Group", DateTime.Now);
                TempData["msg"] = "User Group Successfully Edited";
                return RedirectToAction("list");
            }
            catch (DomainValidationException dve)
            {

                ValidationSummary.DomainValidationErrors(dve, ModelState);
               // ViewBag.msg = msg;
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;

                return View();
            }
        }
        public ActionResult Deactivate(Guid Id)
        {
            try
            {
                _userGroupVeiwModelBuilder.SetInActive(Id);
                TempData["msg"] = "Deactivated Successfully";
            }
            catch(Exception ex)
            {
                TempData["msg"] = ex.Message;
            }
            return RedirectToAction("list");
        }

        [Authorize(Roles = "RoleDeleteMasterData")]
        public ActionResult Delete(Guid Id)
        {
            try
            {
                _userGroupVeiwModelBuilder.SetDeleted(Id);
                TempData["msg"] = "Deleted Successfully";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
            }
            return RedirectToAction("list");
        }

        public ActionResult Activate(Guid Id)
        {
            try
            {
                _userGroupVeiwModelBuilder.SetActive(Id);
                TempData["msg"] = "Activated Successfully";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
            }
            return RedirectToAction("list");
        }

        [Authorize(Roles = "RoleUpdateMasterData")]
        public ActionResult Assign(Guid Id)
        {
            
            return View( _userGroupVeiwModelBuilder.GetAgriUserRoles(Id));
          
        }
         [HttpPost]
         public ActionResult Assign(string[] cb, string groupId)
         {
             try
             {
                 Guid id = Guid.Parse(groupId);
                 _userGroupVeiwModelBuilder.SaveAgri(cb,id);

                 _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Assign", "User Group", DateTime.Now);
                 TempData["msg"] = "Saved Successfully";
                 return RedirectToAction("list");
                 //return RedirectToAction("Assign", new { @groupId = groupId });
             }
             catch (DomainValidationException ve)
             {
                 ValidationSummary.DomainValidationErrors(ve, ModelState);
                 return View();
             }
             catch (Exception ex)
             {
                 ModelState.AddModelError("", ex.Message);
                 return RedirectToAction("List");
             }
             //return View(packs);
         }
    }
}