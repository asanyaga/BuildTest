using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.Paging;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.CostCenter;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Admin.CostCenter;
using Distributr.HQ.Lib.Validation;
using log4net;

namespace Distributr.HQ.Web.Areas.Admin.Controllers.CostCenter
{
    [Authorize ]
    public class TransporterController : Controller
    {

        ITransporterViewModelBuilder _transporterViewModelBuilder;
        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public TransporterController(ITransporterViewModelBuilder transporterViewModelBuilder)
        {
            _transporterViewModelBuilder = transporterViewModelBuilder;
        }
        public ActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListTransporter(Boolean? showInactive, int? page, int? itemsperpage)
        {
            bool showinactive = false;
                if (itemsperpage != null)
                {
                    ViewModelBase.ItemsPerPage = itemsperpage.Value;
                }
            if (showInactive != null)
                showinactive = (bool)showInactive;

            ViewBag.showInactive = showinactive;

            if (TempData["msg"] != null)
            {
                ViewBag.msg = TempData["msg"].ToString();
                TempData["msg"] = null;
            }
            var ls = _transporterViewModelBuilder.GetAll(showinactive);
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            return View(ls.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage));
        }

        public ActionResult DetailsTransporter(Guid id)
        {
            TransporterViewModel transporter = _transporterViewModelBuilder.Get(id);
            return View(transporter);
        }


         [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult CreateTransporter()
         {
            ViewBag.ParentCostCentreList = _transporterViewModelBuilder.ParentCostCentre();
            ViewBag.CostCentreTypeList = _transporterViewModelBuilder.CostCentreTypes();
            return View(new TransporterViewModel());
        } 

        //
        // POST: /Admin/Transporter/Create

        [HttpPost]
        public ActionResult CreateTransporter(TransporterViewModel tvm)
        {
            ViewBag.CostCentreTypeList = _transporterViewModelBuilder.CostCentreTypes();
            ViewBag.ParentCostCentreList = _transporterViewModelBuilder.ParentCostCentre();
            try
            {
                tvm.Id = Guid.Empty;
                _transporterViewModelBuilder.Save(tvm);
                
                return RedirectToAction("ListTransporter");
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
        
        //
        // GET: /Admin/Transporter/Edit/5
         [Authorize(Roles = "RoleModifyMasterData")]
        public ActionResult EditTransporter(Guid id)
        {
            ViewBag.CostCentreTypeList = _transporterViewModelBuilder.CostCentreTypes();
            ViewBag.ParentCostCentreList = _transporterViewModelBuilder.ParentCostCentre();
           TransporterViewModel transVM = _transporterViewModelBuilder.Get(id);
            return View(transVM);
        }

        //
        // POST: /Admin/Transporter/Edit/5

        [HttpPost]
        public ActionResult EditTransporter(TransporterViewModel transporter)
        {
            ViewBag.CostCentreTypeList = _transporterViewModelBuilder.CostCentreTypes();
            ViewBag.ParentCostCentreList = _transporterViewModelBuilder.ParentCostCentre();
            try
            {
                _transporterViewModelBuilder.Save(transporter);
                return RedirectToAction("ListTransporter");
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

        public ActionResult DeActivate(Guid id)
        {
            try
            {
                _transporterViewModelBuilder.SetInactive(id);
                TempData["msg"] = "Successfully Deactivated";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
            }
            return RedirectToAction("ListTransporter");
        }
        public JsonResult Owner(string blogName)
        {
            IList<TransporterViewModel> area = _transporterViewModelBuilder.GetAll(true);
            return Json(area);
        }
        public ActionResult DeleteTransporter(Guid id)
        {
            try
            {
                _transporterViewModelBuilder.SetAsDeleted(id);
                TempData["msg"] = "Transporter Successfully deleted";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to delete Transporter" + ex.Message);
                _log.Error("Failed to delete Transporter" + ex.ToString());


            }

            return RedirectToAction("ListTransporter");
        }
        public ActionResult Activate(Guid id)
        {
            try
            {
                _transporterViewModelBuilder.SetActive(id);
                TempData["msg"] = "Transporter Successfully activated";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to activate transporter" + ex.Message);
                _log.Error("Failed to activate transporter" + ex.ToString());

            }

            return RedirectToAction("ListTransporter");
        }
 
    }
}
