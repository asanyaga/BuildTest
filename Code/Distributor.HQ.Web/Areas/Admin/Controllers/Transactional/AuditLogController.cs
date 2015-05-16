
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Distributr.HQ.Lib.Paging;
using System.Web.Mvc;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Transaction;
using Distributr.HQ.Lib.ViewModels;

namespace Distributr.HQ.Web.Areas.Admin.Controllers.Transactional
{
    public class AuditLogController : Controller
    { 
        IAuditLogViewModelBuilder _auditLogViewModelBuilder;
        public AuditLogController(IAuditLogViewModelBuilder auditLogViewModelBuilder)
        {
            _auditLogViewModelBuilder = auditLogViewModelBuilder;
        }
        public ActionResult Index()
        {
            return View();
        }
       
        public ActionResult ListAuditLogs(bool? showInactive, int? page, int? itemsperpage)
        {
            try
            {
                if (itemsperpage != null)
                {
                    ViewModelBase.ItemsPerPage = itemsperpage.Value;
                }
                ViewBag.ActionList = _auditLogViewModelBuilder.GetAction();
                bool showinactive = false;
                if (showInactive != null)
                    showinactive = (bool)showInactive;
                ViewBag.showInactive = showinactive;
                var ls = _auditLogViewModelBuilder.GetAll();

                ViewBag.Users = ls.Select(s => new { s.UserId, s.UserName }).Distinct()
                    .ToDictionary(d => d.UserId, d => d.UserName).OrderBy(d => d.Value)
                    .ToDictionary(x => x.Key,x => x.Value); 
                ViewBag.Actions = ls.Select(n => n.actionName).Distinct();
                int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
                return View(ls.ToPagedList(currentPageIndex,  ViewModelBase.ItemsPerPage)); 
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }
        [HttpPost]
        public ActionResult ListAuditLogs(bool? showInactive, int? page,string action,string srch, Guid? user,DateTime StartDate,DateTime EndDate, int? itemsperpage)
        {
            try
            {
                if (itemsperpage != null)
                {
                    ViewModelBase.ItemsPerPage = itemsperpage.Value;
                }
                ViewBag.ActionList = _auditLogViewModelBuilder.GetAction();
                bool showinactive = false;
                string command = srch;
                if (showInactive != null)
                    showinactive = (bool)showInactive;
                ViewBag.showInactive = showinactive;

                    DateTime sDate = Convert.ToDateTime(StartDate);
                    DateTime eDate = Convert.ToDateTime(EndDate).AddDays(1);
                    var ls = _auditLogViewModelBuilder.FilterByUser(user,sDate,eDate);

                    ViewBag.Users = ls.Select(s => new { s.UserId, s.UserName }).Distinct()
                        .ToDictionary(d => d.UserId, d => d.UserName)
                        .OrderBy(x => x.Value)
                        .ToDictionary(x => x.Key, x => x.Value);
                    int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
                    return View(ls.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage));
                
                         //
                }
                
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }
        [HttpPost]
       // [HttpGet]
        public JsonResult GetAuditLogs(bool? showInactive, int? page, int? itemsperpage)
        {
            if (itemsperpage != null)
            {
                ViewModelBase.ItemsPerPage = itemsperpage.Value;
            }
            bool showinactive = false;
            if (showInactive != null)
                showinactive = (bool)showInactive;
            ViewBag.showInactive = showinactive;
            var ls = _auditLogViewModelBuilder.GetAll();
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            return Json(ls.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage), JsonRequestBehavior.AllowGet);
        }
    }
}
