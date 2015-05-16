using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.Core.Utility;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.OutletVisitReasonsTypeViewModelBuilders;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Admin.CostCenter;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.CostCenter;
using Distributr.HQ.Lib.Validation;
using Distributr.HQ.Lib.ViewModels.Admin.OutletVisitReasonsTypeViewModels;
using MvcContrib.Pagination;
using log4net;
using System.Reflection;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Transaction;
using Distributr.HQ.Lib.Paging;


namespace Distributr.HQ.Web.Areas.Admin.Controllers.OutletVisitReasonsTypeControllers
{
    public class OutletVisitReasonsTypeController : Controller
    {
        //
        // GET: /Admin/OutletVisitReasonsType/


        private IOutletVisitReasonsTypeViewModelBuilder _outletVisitReasonsTypeViewModelBuilder;
        IAuditLogViewModelBuilder _auditLogViewModelBuilder;
        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        public OutletVisitReasonsTypeController(IOutletVisitReasonsTypeViewModelBuilder outletCategoryViewModelBuilder, IAuditLogViewModelBuilder auditLogViewModelBuilder)
        {
            _outletVisitReasonsTypeViewModelBuilder = outletCategoryViewModelBuilder;
            _auditLogViewModelBuilder = auditLogViewModelBuilder;
        }
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListOutletVisitReasonsTypes(bool? showInactive, int itemsperpage = 10, int page = 1, string srchparam = "")
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
                ViewBag.srchparam = srchparam;

                var currentPageIndex = page - 1 < 0 ? 0 : page - 1;
                var take = itemsperpage;
                var skip = currentPageIndex * take;
                var query = new QueryStandard { Name = srchparam, ShowInactive = showinactive, Skip = skip, Take = take };
                var ls = _outletVisitReasonsTypeViewModelBuilder.Query(query);
                var total = ls.Count;
                var data = ls.Data.ToList();
                return View(data.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage, total));
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }


        [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult CreateOutletVisitReasonsTypes()
        {
            ViewBag.OutletVisitReasonsTypeAction = _outletVisitReasonsTypeViewModelBuilder.OutletVisitReasonsTypeAction();
               
            return View("CreateOutletVisitReasonsTypes", new OutletVisitReasonsTypeViewModel());
        }
         [HttpPost]
        public ActionResult CreateOutletVisitReasonsTypes(OutletVisitReasonsTypeViewModel ovrtvm)
        { 

             try
             {
                 
                 ovrtvm.id = Guid.NewGuid();
                 _outletVisitReasonsTypeViewModelBuilder.Save(ovrtvm);
                 TempData["msg"] = "OutletVisitReasonsType Successfully Created";
                 return RedirectToAction("ListOutletVisitReasonsTypes");
             }
             catch (DomainValidationException dve)
             {

                 ValidationSummary.DomainValidationErrors(dve, ModelState);
                 _log.Debug("Failed to create OutletVisitReasonsType" + dve.Message);
                 _log.Error("Failed to create OutletVisitReasonsType" + dve.ToString());
                 return View(ovrtvm);
             }
             catch(Exception ex)
             {
                 ViewBag.msg = ex.Message;
                 _log.Debug("Failed to create OutletVisitReasonsType" + ex.Message);
                 _log.Error("Failed to create OutletVisitReasonsType" + ex.ToString());
                 return View();

             }
            
        }


         public ActionResult DeActivate(Guid id)
         {
             try
             {
                 _outletVisitReasonsTypeViewModelBuilder.SetInActive(id);
                 _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Deactivate", "OutletVisitReasonsType", DateTime.Now);
                 TempData["msg"] = "Successfully Deactivated";
             }
             catch (DomainValidationException dve)
             {
                 ValidationSummary.DomainValidationErrors(dve, ModelState);
                 _log.Debug("Failed to deactivate outletVisitReasonsType" + dve.Message);
                 _log.Error("Failed to deactivate outletVisitReasonsType" + dve.ToString());
                 TempData["msg"] = dve.Message;
             }
              catch (Exception ex)
             {
                 TempData["msg"] = ex.Message;
                 _log.Debug("Failed to deactivate outletVisitReasonsType" + ex.Message);
                 _log.Error("Failed to deactivate outletVisitReasonsType" + ex.ToString());
             }
             return RedirectToAction("ListOutletVisitReasonsTypes");
         }



         public ActionResult Activate(Guid id, string name)
         {
             try
             {
                 _outletVisitReasonsTypeViewModelBuilder.SetActive(id);
                 _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Activate", "outletVisitReasonsType", DateTime.Now);
                 TempData["msg"] = name + " Successfully Activated";
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
             return RedirectToAction("ListOutletVisitReasonsTypes");
         }


         public ActionResult Delete(Guid id, string name)
         {
             try
             {
                 _outletVisitReasonsTypeViewModelBuilder.SetAsDeleted(id);
                 _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Delete", "outletVisitReasonsType", DateTime.Now);
                 TempData["msg"] = name + " Successfully Deleted";
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
             return RedirectToAction("ListOutletVisitReasonsTypes");
         }



         [Authorize(Roles = "RoleModifyMasterData")]
         public ActionResult EditOutletVisitReasonsType(Guid Id)
         {
             ViewBag.OutletVisitReasonsTypeAction = _outletVisitReasonsTypeViewModelBuilder.OutletVisitReasonsTypeAction();
          try
             {
                 OutletVisitReasonsTypeViewModel outletVisitReasonsType = _outletVisitReasonsTypeViewModelBuilder.Get(Id);
                 return View(outletVisitReasonsType);
             }
             catch (Exception ex)
             {
                 ViewBag.msg = ex.Message;
                 return View();
             }
         }

         [HttpPost]
         public ActionResult EditOutletVisitReasonsType(OutletVisitReasonsTypeViewModel pvm)
         {
             ViewBag.OutletVisitReasonsTypeAction = _outletVisitReasonsTypeViewModelBuilder.OutletVisitReasonsTypeAction();
             try
             {
                 _outletVisitReasonsTypeViewModelBuilder.Save(pvm);
                 _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Edit", "OutletVisitReasonsType", DateTime.Now);
                 TempData["msg"] = "OutletVisitReasonsType Successfully Edited";
                 return RedirectToAction("ListOutletVisitReasonsTypes");
             }

             catch (DomainValidationException dve)
             {

                 ValidationSummary.DomainValidationErrors(dve, ModelState);
                 //ViewBag.msg = msg;
                 return View();
             }
             catch (Exception ex)
             {
                 ViewBag.msg = ex.Message;

                 return View();
             }
         }



         [HttpPost]
         public ActionResult EditOutletVisitReasonsTypess(OutletVisitReasonsTypeViewModel pvm)
         {
             ViewBag.OutletVisitReasonsTypeAction = _outletVisitReasonsTypeViewModelBuilder.OutletVisitReasonsTypeAction();
             try
             {
                 _outletVisitReasonsTypeViewModelBuilder.Save(pvm);
                 _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Edit", "OutletVisitReasonsType", DateTime.Now);
                 TempData["msg"] = "OutletVisitReasonsType Successfully Edited";
                 return RedirectToAction("ListOutletVisitReasonsTypes");
             }

             catch (DomainValidationException dve)
             {

                 ValidationSummary.DomainValidationErrors(dve, ModelState);
                 //ViewBag.msg = msg;
                 return View();
             }
             catch (Exception ex)
             {
                 ViewBag.msg = ex.Message;

                 return View();
             }
         }

    }
}
