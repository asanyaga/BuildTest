using System;
using System.Reflection;
using System.Web.Mvc;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.Paging;
using Distributr.HQ.Lib.Validation;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Transaction;
using Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.CentreViewModelBuilders;
using Distributr.HQ.Lib.ViewModels.Agrimanagr.CentreViewModels;
using log4net;

namespace Agrimanagr.HQ.Areas.Agrimanagr.Controllers.CentreControllers
{
  
    public class CentreTypeController : Controller
    {
        private const int defaultPageSize = 10;
        ICentreTypeViewModelBuilder _centreTypeViewModelBuilder;
        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public CentreTypeController(ICentreTypeViewModelBuilder centreTypeViewModelBuilder)
        {
            _centreTypeViewModelBuilder = centreTypeViewModelBuilder;
        }

        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListCentreTypes(bool? showInactive,int page = 1, int itemsperpage = 10, string srchParam = "") 
        {
            try 
            {
                bool showinactive = false;
                if (showInactive != null)
                    showinactive = (bool)showInactive;
                ViewBag.showInactive = showinactive;
                if (TempData["msg"] != null)
                {
                    ViewBag.msg = TempData["msg"].ToString();
                    TempData["msg"] = null;
                }
                ViewBag.srchParam = srchParam;

                
                int currentPageIndex = page - 1 < 0 ? 0 : page - 1;
                int take = itemsperpage;
                int skip = currentPageIndex * take;
                
                var query = new QueryStandard {Name = srchParam, ShowInactive = showinactive, Skip = skip, Take = take};
                
                var ls = _centreTypeViewModelBuilder.Query(query);
                
                var data = ls.Data;
                var count = ls.Count;

                return View(data.ToPagedList(currentPageIndex, take, count));
            }
            catch(Exception ex)
            {
                _log.Debug("Failed to list centre type" + ex.Message);
                _log.Error("Failed to list centre type" + ex.ToString());
                ViewBag.msg = ex.Message;
                return View();
            }
        }

        [Authorize(Roles = "RoleUpdateMasterData")]
        public ActionResult EditCentreType(Guid id, string msg)
        {
            ViewBag.msg = msg;
            try
            {
                CentreTypeViewModel centreType = _centreTypeViewModelBuilder.Get(id);
          
               return View(centreType);
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }

        [HttpPost]
        public ActionResult EditCentreType(CentreTypeViewModel atvm)
          {
              try
              {
                  _centreTypeViewModelBuilder.Save(atvm);
                  TempData["msg"] = "Centre Type Successfully Edited";
                  return RedirectToAction("ListCentreTypes");
              }
              catch (DomainValidationException dve)
              {
                  ValidationSummary.DomainValidationErrors(dve, ModelState);
                  return View();
              }
              catch (Exception ex)
              {
                  _log.Debug("Failed to edit Centre type" + ex.Message);
                  _log.Error("Failed to edit Centre type" + ex.ToString());
                  return View();
              }
          }

        [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult CreateCentreType()
        {
            return View("CreateCentreType", new CentreTypeViewModel());
        }

        [HttpPost]
        public ActionResult CreateCentreType(CentreTypeViewModel atvm)
        {
            try
            {
                atvm.Id = Guid.NewGuid();
                _centreTypeViewModelBuilder.Save(atvm);
                TempData["msg"] = "Centre Type Successfully Created";
                return RedirectToAction("ListCentreTypes");
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
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
                _centreTypeViewModelBuilder.SetInactive(Id);
                TempData["msg"] = "Successfully Deactivated";
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                _log.Debug("Failed to deactivate Centre type" + dve.Message);
                _log.Error("Failed to deactivate Centre type" + dve.ToString());
                TempData["msg"] = dve.Message;
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to deactivate Centre type" + ex.Message);
                _log.Error("Failed to deactivate Centre type" + ex.ToString());
            }
            return RedirectToAction("ListCentreTypes");
        }


        [Authorize(Roles = "RoleDeleteMasterData")]
        public ActionResult Delete(Guid Id)
        {
            try
            {
                _centreTypeViewModelBuilder.SetDeleted(Id);
                TempData["msg"] = "Successfully Deleted";
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                _log.Debug("Failed to Delete Centre type" + dve.Message);
                _log.Error("Failed to Delete Centre type" + dve.ToString());
                TempData["msg"] = dve.Message;
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to Delete Centre type" + ex.Message);
                _log.Error("Failed to Delete Centre type" + ex.ToString());
            }
            return RedirectToAction("ListCentreTypes");
        }


        public ActionResult Activate(Guid Id)
        {
            try
            {
                _centreTypeViewModelBuilder.SetActive(Id);
                TempData["msg"] = "Successfully Activated";
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                _log.Debug("Failed to activate centre type" + dve.Message);
                _log.Error("Failed to activate centre type" + dve.ToString());
                TempData["msg"] = dve.Message;
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to activate asset type" + ex.Message);
                _log.Error("Failed to activate asset type" + ex.ToString());
            }
            return RedirectToAction("ListCentreTypes");
        }

       
    }
}
