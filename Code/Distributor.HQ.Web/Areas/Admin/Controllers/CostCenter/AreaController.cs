using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.CostCenter;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.CostCenter.Impl;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Admin.CostCenter;
using Distributr.HQ.Lib.Validation;
//using Distributr.HQ.Lib.ViewModels.Admin.CostCenter;

namespace Distributr.HQ.Web.Areas.Admin.Controllers.CostCenter
{
    [Authorize ]
    public class AreaController : Controller
    {
        //
        // GET: /Admin/Area/
        IAreaViewModelBuilder _areaViewModelBuilder;
        public AreaController(IAreaViewModelBuilder areaViewModelBuilder)
        {
            _areaViewModelBuilder = areaViewModelBuilder;
        }
        public ActionResult Index()
        {
            return View();
        }
         [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListAreas(Boolean? showInactive, int? itemsperpage)
        {
            bool showinactive = false;
            if (showInactive != null)
                showinactive = (bool)showInactive;
             
                if (itemsperpage != null)
                {
                    ViewModelBase.ItemsPerPage = itemsperpage.Value;
                }
            ViewBag.showInactive = showinactive;

            if (TempData["msg"] != null)
            {
                ViewBag.msg = TempData["msg"].ToString();
                TempData["msg"] = null;
            }
            IList<AreaViewModel> area = _areaViewModelBuilder.GetAll(showinactive);
            return View(area);
        }

         public ActionResult DetailsArea(Guid id)
        {
            AreaViewModel area = _areaViewModelBuilder.Get(id);
            return View(area);
        }


         [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult CreateArea()
        {
            ViewBag.RegionList = _areaViewModelBuilder.Region();
            return View("CreateArea", new AreaViewModel()); 
        } 

       

        [HttpPost]
        public ActionResult CreateArea(AreaViewModel avm)
        {

            try
            {
                ViewBag.RegionList = _areaViewModelBuilder.Region();
                avm.Id = Guid.NewGuid(); ;
                _areaViewModelBuilder.Save(avm);
                return RedirectToAction("ListAreas");
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

        [Authorize(Roles = "RoleModifyMasterData")]
        public ActionResult EditArea(Guid id)
        {
            AreaViewModel areaVM = _areaViewModelBuilder.Get(id);
            ViewBag.RegionList = _areaViewModelBuilder.Region();
            return View(areaVM);
        }


        [HttpPost]
        public ActionResult EditArea(AreaViewModel avm)
        {
            try
            {
                _areaViewModelBuilder.Save(avm);
                ViewBag.RegionList = _areaViewModelBuilder.Region();
                return RedirectToAction("ListAreas");
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
                _areaViewModelBuilder.SetInactive(id);
                TempData["msg"] = "Successfully Deactivated";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
            }
            return RedirectToAction("ListAreas");
        }
        public JsonResult Owner(string blogName)
        {
            IList<AreaViewModel> area = _areaViewModelBuilder.GetAll(true);
            return Json(area);
        }

        public ActionResult Activate(Guid id)
        {
            try
            {
                _areaViewModelBuilder.SetActive(id);
                TempData["msg"] = "Successfully Activated";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
            }
            return RedirectToAction("ListAreas");
        }

        public ActionResult Delete(Guid id)
        {
            try
            {
                _areaViewModelBuilder.SetAsDeleted(id);
                TempData["msg"] = "Successfully Deleted";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
            }
            return RedirectToAction("ListAreas");
        }
 
        
    }
}
