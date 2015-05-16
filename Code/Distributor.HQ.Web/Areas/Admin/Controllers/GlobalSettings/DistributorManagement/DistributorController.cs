using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.Core.Utility;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Admin.Distributors;
using Distributr.HQ.Lib.Validation;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Distributors;
using Distributr.HQ.Lib.Paging;
using log4net;
using System.Reflection;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Transaction;
using System.Configuration;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;

namespace Distributr.HQ.Web.Areas.Admin.Controllers.GlobalSettings.DistributorManagement
{
     //[Authorize]
    public class DistributorController : Controller
    { 
        IDistributorViewModelBuilder _DistributorViewModelBuilder;
        IAuditLogViewModelBuilder _auditLogViewModelBuilder;
        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public DistributorController(IDistributorViewModelBuilder distributorviewmodelbuilder, IAuditLogViewModelBuilder auditLogViewModelBuilder)
        {
            _DistributorViewModelBuilder = distributorviewmodelbuilder;
            _auditLogViewModelBuilder = auditLogViewModelBuilder;
        }
       
        public ActionResult Index()
        {
            return View();
        }
         [Authorize(Roles = "RoleViewCostCentre")]
        public ActionResult ListDistributors(bool showInactive = false, int page = 1, int itemsperpage = 10, string srchparam = "")
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
                if (Session["msg"] != null)
                {
                    ViewBag.msg = Session["msg"].ToString();
                    Session["msg"] = null;
                }
                ViewBag.searchParam = srchparam;
                ViewBag.items = itemsperpage;
              
                var currentPageIndex = page - 1 < 0 ? 0 : page - 1;
                var take = itemsperpage;
                var skip = currentPageIndex * take;
                var query = new QueryStandard { Name = srchparam, ShowInactive = showinactive, Skip = skip, Take = take };
                var ls = _DistributorViewModelBuilder.Query(query);
                var total = ls.Count;
                var data = ls.Data.ToList();
                return View(data.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage, total)); 
            }
            catch (Exception ex)
            {
                _log.Debug("Failed to list distributor" + ex.Message);
                _log.Error("Failed to list distributor" + ex.ToString());
                ViewBag.msg = ex.Message;
                HQMailerViewModelBuilder hqm = new HQMailerViewModelBuilder(ConfigurationSettings.AppSettings["ServerIP"], ConfigurationSettings.AppSettings["UserName"], ConfigurationSettings.AppSettings["Password"]);


                hqm.Send(ConfigurationSettings.AppSettings["ServerEmail"], ConfigurationSettings.AppSettings["MailGroup"], "Test", "Distributor listing error:" + ex.Message);

                return View();

            }

        }

         public ActionResult DistributorDetails(Guid id)
        {
            DistributorViewModel distributor = _DistributorViewModelBuilder.Get(id);
            return View(distributor);
        }
         [Authorize(Roles = "RoleAddCostCentre")]
        public ActionResult CreateDistributor()
        {
            ViewBag.RegionList = _DistributorViewModelBuilder.Region();
            ViewBag.ASMList = _DistributorViewModelBuilder.ASM();
            ViewBag.SalesRepList = _DistributorViewModelBuilder.SalesRep();
            ViewBag.SurveyorList = _DistributorViewModelBuilder.Surveyor();
           // ViewBag.TierList = _DistributorViewModelBuilder.PricingTier();
            ViewBag.Title = "Create Distribtuor";
            return View("CreateDistributor", new DistributorViewModel());
        }

        [HttpPost]
        public ActionResult CreateDistributor(DistributorViewModel dvm)
        {
            ViewBag.RegionList = _DistributorViewModelBuilder.Region();
            ViewBag.ASMList = _DistributorViewModelBuilder.ASM();
            ViewBag.SalesRepList = _DistributorViewModelBuilder.SalesRep();
            ViewBag.SurveyorList = _DistributorViewModelBuilder.Surveyor();
            //ViewBag.TierList = _DistributorViewModelBuilder.PricingTier();
            try
            {
                if (dvm.RegionId == Guid.Empty)
                {
                    ModelState.AddModelError("Distributor", "Region is required");
                    return View();
                }
                else
                {
                    dvm.Id = Guid.NewGuid();
                    _DistributorViewModelBuilder.Save(dvm);
                    _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Create", "Distributor", DateTime.Now);
                    Session["msg"] = "Distributor Successfully Created";
                    var latestDistributr = _DistributorViewModelBuilder.Get(dvm.Id);//.Where(n => n.DateCreated < DateTime.Now).OrderByDescending(n => n.DateCreated).ThenByDescending(n => n.id).ToList()[0];//;
                    //  DistributorViewModel  latestDistributr = _DistributorViewModelBuilder.GetAll().FirstOrDefault(n=>n.DateCreated<DateTime.Now);

                    Guid distributrId = latestDistributr.Id;
                    string distributorName = latestDistributr.Name;
                    // return RedirectToAction("listDistributors");
                    return RedirectToAction("CreateContact", "Contact", new { CostCentre = distributrId, CostCentreName = distributorName, ContactFor = "Distributor" });
                }
            }
            catch (DomainValidationException dve)
            {

                ValidationSummary.DomainValidationErrors(dve, ModelState);
                _log.Debug("Failed to create distributor" + dve.Message);
                _log.Error("Failed to create distributor" + dve.ToString());
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                _log.Debug("Failed to create distributor" + ex.Message);
                _log.Error("Failed to create distributor" + ex.ToString()); HQMailerViewModelBuilder hqm = new HQMailerViewModelBuilder(ConfigurationSettings.AppSettings["ServerIP"], ConfigurationSettings.AppSettings["UserName"], ConfigurationSettings.AppSettings["Password"]);


                hqm.Send(ConfigurationSettings.AppSettings["ServerEmail"], ConfigurationSettings.AppSettings["MailGroup"], "Test", "Create Distributor error:" + ex.Message);

                return View();
            }

        }
        [Authorize(Roles = "RoleModifyMasterData")]
        public ActionResult EditDistributor(Guid id)
        {
            DistributorViewModel dvm = _DistributorViewModelBuilder.Get(id);
            ViewBag.RegionList = _DistributorViewModelBuilder.Region();
            var ASMList = _DistributorViewModelBuilder.ASM();
            var SalesRepList = _DistributorViewModelBuilder.SalesRep();
            var SurveyorList = _DistributorViewModelBuilder.Surveyor(); 

            ViewBag.ASMList = ASMList;
            ViewBag.SalesRepList = SalesRepList;
            ViewBag.SurveyorList = SurveyorList;
            ViewBag.Title = "Edit Distributor";
            return View(dvm);
        }

        [HttpPost]
        public ActionResult EditDistributor(DistributorViewModel dvm)
        {
            //ViewBag.RegionList = _DistributorViewModelBuilder.Region();
            //ViewBag.ASMList = _DistributorViewModelBuilder.ASM();
            //ViewBag.SalesRepList = _DistributorViewModelBuilder.SalesRep();
            //ViewBag.SurveyorList = _DistributorViewModelBuilder.Surveyor();
            try
            {
                _DistributorViewModelBuilder.Save(dvm);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Edit", "Distributor", DateTime.Now);
                Session["msg"] = "Distributor Successfully Edited";
                return RedirectToAction("listDistributors");
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve,ModelState);
                _log.Debug("Failed to edit distributor" + dve.Message);
                _log.Error("Failed to edit distributor" + dve.ToString());
                return View();
            }
            catch (Exception ex)
            {
                _log.Debug("Failed to edit distributor" + ex.Message);
                _log.Error("Failed to edit distributor" + ex.ToString());
                HQMailerViewModelBuilder hqm = new HQMailerViewModelBuilder(ConfigurationSettings.AppSettings["ServerIP"], ConfigurationSettings.AppSettings["UserName"], ConfigurationSettings.AppSettings["Password"]);


                hqm.Send(ConfigurationSettings.AppSettings["ServerEmail"], ConfigurationSettings.AppSettings["MailGroup"], "Test", "Edit Distributor error:" + ex.Message);

                return View();
            }

        }

        [Authorize(Roles = "RoleModifyMasterData")]
        public ActionResult Deactivate(Guid id)
        {
            try
            {
                _DistributorViewModelBuilder.SetInactive(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Deactivate", "Distributor", DateTime.Now);
                Session["msg"] = "Successfully DeActivated";
            }
            catch (Exception ex)
            {
                Session["msg"] = ex.Message;
                _log.Debug("Failed to deactivate distributor" + ex.Message);
                _log.Error("Failed to deactivate distributor" + ex.ToString());
            }
            return RedirectToAction("listDistributors");
        }

        [Authorize(Roles = "RoleModifyMasterData")]
        public ActionResult Activate(Guid id)
        {
            try
            {
                _DistributorViewModelBuilder.Activate(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Activate", "Distributor", DateTime.Now);
                Session["msg"] = "Successfully activated";
            }
            catch (Exception ex)
            {
                Session["msg"] = ex.Message;
                _log.Debug("Failed to activate distributor" + ex.Message);
                _log.Error("Failed to activate distributor" + ex.ToString());
            }
            return RedirectToAction("listDistributors");
        }

        [Authorize(Roles = "RoleModifyMasterData")]
        public ActionResult Delete(Guid id)
        {
            try
            {
                _DistributorViewModelBuilder.Delete(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Delete", "Distributor", DateTime.Now);
                Session["msg"] = "Successfully DeActivated";
            }
            catch (Exception ex)
            {
                Session["msg"] = ex.Message;
                _log.Debug("Failed to delete distributor" + ex.Message);
                _log.Error("Failed to delete distributor" + ex.ToString());
            }
            return RedirectToAction("listDistributors");
        }

      
    }
}
