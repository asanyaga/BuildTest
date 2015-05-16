using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.CostCenter;
using Distributr.HQ.Lib.ViewModels.Admin.CostCenter;
using Distributr.HQ.Lib.Validation;
using Distributr.Core.Validation;


namespace Distributr.HQ.Web.Areas.Admin.Controllers.CostCenter
{
    public class RegionsController : Controller
    {
        //
        // GET: /Admin/Region/
        IRegionViewModelBuilder _regionViewModelBuilder;
        public RegionsController(IRegionViewModelBuilder regionViewModelBuilder)
        {
            _regionViewModelBuilder = regionViewModelBuilder;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListRegion(Boolean? showInactive)
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
            IList<RegionViewModel> region = _regionViewModelBuilder.GetAll(showinactive);
            return View(region);
        }

        public ActionResult DetailsRegion(int id)
        {
            RegionViewModel reg = _regionViewModelBuilder.Get(id);
            return View(reg);
        }

        

        public ActionResult CreateRegion()
        {
            ViewBag.CountryList = _regionViewModelBuilder.Country();
            return View("CreateRegion",new RegionViewModel()); 
        } 

        

        [HttpPost]
        public ActionResult CreateRegion(RegionViewModel rvm)
        {
            try
            {
                ViewBag.CountryList = _regionViewModelBuilder.Country();
                _regionViewModelBuilder.Save(rvm);
                return RedirectToAction("ListRegion");
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve,ModelState);
                return View();
            }
            catch(Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }
        
       
 
        public ActionResult EditRegion(int id)
        {
            RegionViewModel regVM = _regionViewModelBuilder.Get(id);
            ViewBag.CountryList = _regionViewModelBuilder.Country();
            return View(regVM);
        }

      

        [HttpPost]
        public ActionResult EditRegion(RegionViewModel rvm)
        {
            try
            {
                _regionViewModelBuilder.Save(rvm);
                ViewBag.CountryList = _regionViewModelBuilder.Country();
                return RedirectToAction("ListRegion");
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
        public ActionResult DeActivate(int id)
        {
            try
            {
                _regionViewModelBuilder.SetInActive(id);
                TempData["msg"] = "Successfully Deactivated";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
            }
            return RedirectToAction("ListRegion");
        }
        public JsonResult Owner(string blogName)
        {
            IList<RegionViewModel> region = _regionViewModelBuilder.GetAll(true);
            return Json(region);
        }
     
    }
}
