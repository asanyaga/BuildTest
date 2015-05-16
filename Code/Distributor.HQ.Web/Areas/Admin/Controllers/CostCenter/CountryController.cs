using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.CostCenter;
using Distributr.HQ.Lib.ViewModels.Admin.CostCenter;
using Distributr.Core.Validation;
using Distributr.HQ.Lib.Validation;
using Distributr.HQ.Lib.ViewModels.Admin.CostCenter;

namespace Distributr.HQ.Web.Areas.Admin.Controllers.CostCenter
{
    public class CountryController : Controller
    {
        //
        // GET: /Admin/Country/

        ICountryViewModelBuilder _countryViewModelBuilder;
        public CountryController(ICountryViewModelBuilder countryViewModelBuilder)
        {
            _countryViewModelBuilder = countryViewModelBuilder;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ListCountry(Boolean? showInactive)
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
            IList<CountryViewModel> country = _countryViewModelBuilder.GetAll(showinactive);
            return View(country);
        }

        public ActionResult DetailsCountry(int id)
        {
            CountryViewModel countr = _countryViewModelBuilder.Get(id);
            return View(countr);
        }
        public ActionResult EditCountry(int id)
        {
            CountryViewModel countr = _countryViewModelBuilder.Get(id);
            return View(countr);
        }
        [HttpPost]
        public ActionResult EditCountry(CountryViewModel cvm)
        {
            try
            {
                _countryViewModelBuilder.Save(cvm);
                return RedirectToAction("ListCountry");
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
        public ActionResult CreateCountry()
        {
            return View("EditCountry", new CountryViewModel());
        }
        [HttpPost]
        public ActionResult CreateCountry(CountryViewModel cvm)
        {
            try
            {
                _countryViewModelBuilder.Save(cvm);
                return RedirectToAction("ListCountry");
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
                _countryViewModelBuilder.SetInActive(id);
                TempData["msg"] = "Successfully Deactivated";
            }
            catch(Exception ex)
            {
                TempData["msg"] = ex.Message;
            }
            return RedirectToAction("ListCountry");
        }
        public JsonResult Owner(string blogName)
        {
            IList<CountryViewModel> country = _countryViewModelBuilder.GetAll(true);
            return Json(country);
        }

        }
}
