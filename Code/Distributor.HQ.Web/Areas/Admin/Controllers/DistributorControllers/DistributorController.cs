using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Distributors;
using System.Web.Mvc;
using Distributr.HQ.Lib.ViewModels.Admin.Distributors;
using Distributr.Core.Validation;
using Distributr.HQ.Lib.Validation;
using Distributr.Core.Domain.Master.CostCentreEntities;

namespace Distributr.HQ.Web.Areas.Admin.Controllers.DistributorControllers
{
    public class DistributorController:Controller
    {
        IDistributorViewModelBuilder _DistributorViewModelBuilder;

        public DistributorController(IDistributorViewModelBuilder distributorviewmodelbuilder)
        {
            _DistributorViewModelBuilder = distributorviewmodelbuilder;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListDistributors(bool? showInactive)
        {
            bool showinactive = false;
            if (showInactive != null)
                showinactive = (bool)showInactive;

            ViewBag.showInactive = showinactive;

            if (Session["msg"] != null)
            {
                ViewBag.msg = Session["msg"].ToString();
                Session["msg"] = null;
            }
            IList<DistributorViewModel> distributors = _DistributorViewModelBuilder.GetAll(showinactive);
            return View(distributors);
        }

        public ActionResult DistributorDetails(Guid id)
        {
            DistributorViewModel distributor = _DistributorViewModelBuilder.Get(id);
            return View(distributor);
        }

        public ActionResult CreateDistributor()
        {
            ViewBag.RegionList = _DistributorViewModelBuilder.Region();
            return View("CreateDistributor",new DistributorViewModel());
        }

        [HttpPost]
        public ActionResult CreateDistributor(DistributorViewModel dvm)
        {
            try
            {
                ViewBag.RegionList = _DistributorViewModelBuilder.Region();
                _DistributorViewModelBuilder.Save(dvm);
                return RedirectToAction("listDistributors");
            }
            catch (DomainValidationException dve)
            {

                string msg = ValidationSummary.DomainValidationErrors(dve);
                ViewBag.msg = msg;
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;

                return View();
            }

        }

        public ActionResult EditDistributor(Guid  id)
        {          
            DistributorViewModel dvm = _DistributorViewModelBuilder.Get(id);
            ViewBag.RegionList = _DistributorViewModelBuilder.Region();
            return View(dvm);
        }

        [HttpPost]
        public ActionResult EditDistributor(DistributorViewModel dvm)
        {
            _DistributorViewModelBuilder.Save(dvm);
            ViewBag.RegionList = _DistributorViewModelBuilder.Region();
            return RedirectToAction("listDistributors");
        }

        public ActionResult Deactivate(Guid id)
        {
            try
            {
                _DistributorViewModelBuilder.SetInactive(id);
                Session["msg"] = "Successfully DeActivated";
            }
            catch (Exception ex)
            {
                Session["msg"] = ex.Message;
            }
            return RedirectToAction("listDistributors");
        }
        
    }
}