using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.CostCenter;
using Distributr.HQ.Lib.ViewModels.Admin.CostCenter;
using Distributr.Core.Validation;
using Distributr.HQ.Lib.Validation;

namespace Distributr.HQ.Web.Areas.Admin.Controllers.CostCenter
{
    public class TerritoryController : Controller
    {
        //
        // GET: /Admin/Territory/
        ITerritoryViewModelBuilder _territoryViewModelBuilder;
        public TerritoryController(ITerritoryViewModelBuilder territoryViewModelBuilder)
        {
            _territoryViewModelBuilder = territoryViewModelBuilder;
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListTerritory(Boolean? showInactive)
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
            IList<TerritoryViewModel> territory = _territoryViewModelBuilder.GetAll(showinactive);
            return View(territory);
        }

        public ActionResult DetailsTerritory(int id)
        {
            TerritoryViewModel territory = _territoryViewModelBuilder.Get(id);
            return View(territory);
        }

       
        //  /Admin/Territory/Create

        public ActionResult CreateTerritory()
        {
            return View("CreateTerritory",new TerritoryViewModel());
        } 

    
        // POST: /Admin/Territory/Create

        [HttpPost]
        public ActionResult CreateTerritory(TerritoryViewModel territory)
        {
            try
            {
                _territoryViewModelBuilder.Save(territory);

                return RedirectToAction("ListTerritory");
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                return View();
            }
            catch(Exception exx)
            {
                ViewBag.msg = exx.Message;
                return View();
            }
        }
        
        //
        // GET: /Admin/Territory/Edit/5
 
        public ActionResult EditTerritory(int id)
        {
            TerritoryViewModel territory = _territoryViewModelBuilder.Get(id);
            return View(territory);
        }

        //
        // POST: /Admin/Territory/Edit/5

        [HttpPost]
        public ActionResult EditTerritory(TerritoryViewModel territory)
        {
            try
            {
                _territoryViewModelBuilder.Save(territory);
 
                return RedirectToAction("ListTerritory");
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                return View();
            }
            catch(Exception exx)
            {
                ViewBag.msg = exx.Message;
                return View();
            }
        }
        public JsonResult Owner(string bName)
        {
            IList<TerritoryViewModel> tvm = _territoryViewModelBuilder.GetAll(true);
            return Json(tvm);
        }
       
    }
}
