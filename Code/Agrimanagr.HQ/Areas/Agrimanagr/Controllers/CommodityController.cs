using System;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Distributr.Core.Domain.Master.CommodityEntities;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.Paging;
using Distributr.HQ.Lib.Validation;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Transaction;
using Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.CommodityViewModelBuilders;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Agrimanagr.CommodityViewModel;
using log4net;

namespace Agrimanagr.HQ.Areas.Agrimanagr.Controllers
{

    public class CommodityController : Controller
    { 
        ICommodityViewModelBuilder _commodityViewModelBuilder;
        protected static readonly ILog Log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public CommodityController(ICommodityViewModelBuilder commodityViewModelBuilder)
        {
            _commodityViewModelBuilder = commodityViewModelBuilder;
            ViewBag.CommodityTypeList = _commodityViewModelBuilder.CommodityTypeList();

        }

        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult Details(Guid id)
        {
            CommodityViewModel model = _commodityViewModelBuilder.Get(id);
            return View(model);
        }

        [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult Create()
        {
            return View(new CommodityViewModel());
        }

        [HttpPost]
        public ActionResult Create(CommodityViewModel commodityViewModel)
        {
            try
            {

                commodityViewModel.Id = Guid.NewGuid();
                _commodityViewModelBuilder.Save(commodityViewModel);
                TempData["msg"] = "Commodity Successfully Created";
                return RedirectToAction("List");

            }
            catch (DomainValidationException ve)
            {
                ValidationSummary.DomainValidationErrors(ve, ModelState);
                //ViewBag.msg = ve.Message;
                Log.ErrorFormat("Error in creating commodity" + ve.Message);
                Log.InfoFormat("Error in creating commodity" + ve.Message);
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewBag.msg = ex.Message;
                Log.ErrorFormat("Error in creating commodity" + ex.Message);
                Log.InfoFormat("Error in creating commodity" + ex.Message);
                return View();
            }
        }

        [Authorize(Roles = "RoleUpdateMasterData")]
        public ActionResult Edit(Guid id)
        {
            try
            {
                CommodityViewModel model = _commodityViewModelBuilder.Get(id);
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }

        [HttpPost]
        public ActionResult Edit(CommodityViewModel commodityViewModel)
        {
            try
            {

                _commodityViewModelBuilder.Save(commodityViewModel);
                TempData["msg"] = "Commodity Successfully Edited";
                return RedirectToAction("List");
            }
            catch (DomainValidationException ve)
            {
                ValidationSummary.DomainValidationErrors(ve, ModelState);
                Log.ErrorFormat("Error in editing Commodity" + ve.Message);
                Log.InfoFormat("Error in editing Commodity" + ve.Message);
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                Log.ErrorFormat("Error in editing Commodity" + ex.Message);
                Log.InfoFormat("Error in editing Commodity" + ex.Message);
                return View();
            }
        }

        [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult List(bool? showInactive, int page=1, int itemsperpage=10, string srchParam="")
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

                ViewBag.SearchText = srchParam;

                var currentPageIndex = page-1 < 0 ? 0 : page-1;
                var take = itemsperpage;
                var skip = currentPageIndex*take;

                var query = new QueryStandard
                    {Name = srchParam, ShowInactive = showinactive, Take = take, Skip = skip};
                
                var ls = _commodityViewModelBuilder.Query(query);
                
                var data = ls.Data;
                var total = ls.Count;

                return View(data.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage, total));

            }
            catch (Exception ex)
            {

                Log.ErrorFormat("Error in listing Commodity" + ex.Message);
                Log.InfoFormat("Error in listing Commodity" + ex.Message);
                ViewBag.msg = ex.ToString();
                string exception = ex.Message;
                try
                {
                    HQMailerViewModelBuilder hqm = new HQMailerViewModelBuilder(ConfigurationManager.AppSettings["ServerIP"], ConfigurationManager.AppSettings["UserName"], ConfigurationManager.AppSettings["Password"]);


                    hqm.Send(ConfigurationManager.AppSettings["ServerEmail"], ConfigurationManager.AppSettings["MailGroup"], "Listing Product Pricing", ex.ToString());
                }
                catch (Exception exx) { }
                bool showinactive = false;
                if (showInactive != null)
                    showinactive = (bool)showInactive;
                ViewBag.showInactive = showinactive;
                var ls = _commodityViewModelBuilder.Search("", showinactive);
                //int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
                var currentPageIndex = page < 0 ? 0 : page;
                return View(ls.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage));
            }
        }

        public ActionResult Deactivate(Guid id)
        {
            try
            {

                _commodityViewModelBuilder.SetInactive(id);
                TempData["msg"] = "Successfully Deactivated";
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                TempData["msg"] = dve.Message;
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                Log.ErrorFormat("Error in deactivating commodity pricing" + ex.Message);
                Log.InfoFormat("Error in deactivating commodity pricing" + ex.Message);

            }
            return RedirectToAction("List");
        }

        public ActionResult Activate(Guid id)
        {
            try
            {
                _commodityViewModelBuilder.SetActive(id);
                TempData["msg"] = "Successfully Activated";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                Log.ErrorFormat("Error in activating commodity pricing" + ex.Message);
                Log.InfoFormat("Error in activating commodity pricing" + ex.Message);

            }
            return RedirectToAction("List");
        }

        [Authorize(Roles = "RoleDeleteMasterData")]
        public ActionResult Delete(Guid id)
        {
            try
            {
                _commodityViewModelBuilder.SetAsDeleted(id);
                TempData["msg"] = "Successfully Deleted";
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                TempData["msg"] = dve.Message;
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                Log.ErrorFormat("Error in deactivating commodity pricing" + ex.Message);
                Log.InfoFormat("Error in deactivating Commodity pricing" + ex.Message);

            }
            return RedirectToAction("List");
        }

        [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListGrades(bool? showInactive, Guid? commodity, int page=1, int itemsperpage=10,string srchParam="")
        {
            Guid commodityId = new Guid();

            if (commodity.Value != null)
            {
                ViewBag.CommodityId = commodity.Value;
                commodityId = commodity.Value;
            }

            if (TempData["msg"] != null)
            {
                ViewBag.msg = TempData["msg"].ToString();
                TempData["msg"] = null;
            }

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
                ViewBag.srchParam = srchParam;
                int currentPageIndex = page - 1 < 0 ? 0 : page - 1;
                int take = itemsperpage;
                int skip = currentPageIndex*take;

                var query = new QueryGrade
                    {
                        Name = srchParam,
                        CommodityId = commodity.Value,
                        ShowInactive = showinactive,
                        Take = take,
                        Skip = skip
                    };

                var ls = _commodityViewModelBuilder.Search(commodityId, srchParam, showinactive);
                //var ls = _commodityViewModelBuilder.QueryGrade(query);

                //int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
                return View(ls.ToPagedList(currentPageIndex,  ViewModelBase.ItemsPerPage));
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }

        [HttpPost]
        public ActionResult ListGrades(string grade, int? page, bool? showInactive, Guid? commodity, string srch, int? itemsperpage)
        {

            string command = srch;
            Guid commodityId = new Guid();
            if (commodity.HasValue)
            {
                ViewBag.CommodityId = commodity.Value;
                commodityId = commodity.Value;

            }

            try
            {
                if (itemsperpage != null)
                {
                    ViewModelBase.ItemsPerPage = itemsperpage.Value;
                }
                bool showinactive = false;
                if (showInactive != null)
                    showinactive = (bool)showInactive;
                ViewBag.showInactive = showinactive;

                if (command == "Search")
                {
                    var ls = _commodityViewModelBuilder.Search(commodityId, grade, showinactive);
                    int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
                    return View(ls.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage));
                }
                else
                {
                    return RedirectToAction("ListGrades", new
                    {
                        showinactive = showInactive,
                        commodity = commodityId,
                        srch = "Search",
                        srcParam = ""
                    });
                }
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }

        [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult AddCommodityGrade(Guid? commodity)
        {

            if (commodity.HasValue)
            {
                CommodityGradeViewModel cvm = new CommodityGradeViewModel
                {
                    CommodityId = commodity.Value
                };

                return View(cvm);
            }
            else
            {
                CommodityGradeViewModel cvm2 = new CommodityGradeViewModel
                {
                    CommodityId = Guid.NewGuid()
                };
                return View("AddCommodityGrade", cvm2);
            }

        }
        [HttpPost]
        public ActionResult AddCommodityGrade(CommodityGradeViewModel aplvm)
        {
            try
            {
                Guid commodityId = aplvm.CommodityId;
                string commodityGradeName = aplvm.Name;
                int usageTypeId = aplvm.UsageTypeId;
                string commodityGradeCode = aplvm.Code;
                string commodityGradeDescription = aplvm.Description;

                _commodityViewModelBuilder.AddCommodityGrades
                    (commodityId, Guid.NewGuid(), commodityGradeName, usageTypeId, commodityGradeCode, commodityGradeDescription);

                return RedirectToAction("ListGrades", new { commodity = commodityId });

            }
            catch (DomainValidationException ve)
            {
                ValidationSummary.DomainValidationErrors(ve, ModelState);
                ViewBag.msg2 = ve.Message;
                Log.ErrorFormat("Error in creating commodity grade" + ve.Message);
                Log.InfoFormat("Error in creating commodity grade" + ve.Message);
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewBag.msg2 = ex.Message;
                Log.ErrorFormat("Error in creating commodity grade" + ex.Message);
                Log.InfoFormat("Error in creating commodity grade" + ex.Message);
                return View();
            }
        }

        [Authorize(Roles = "RoleUpdateMasterData")]
        public ActionResult EditGrade(Guid commodity, Guid gradeId, string name, string code, string description, int usageTypeId)
        {

            CommodityGradeViewModel cvm = new CommodityGradeViewModel
            {
                CommodityId = commodity,
                Id = gradeId,
                Name = name,
                Code = code,
                Description = description,
                UsageTypeId = usageTypeId

            };

            return View(cvm);


        }
        [HttpPost]
        public ActionResult EditGrade(CommodityGradeViewModel aplvm)
        {
            try
            {
                Guid gradeId = aplvm.Id;
                Guid commodityId = aplvm.CommodityId;
                string commodityGradeName = aplvm.Name;
                int usageTypeId = aplvm.UsageTypeId;
                string commodityGradeCode = aplvm.Code;
                string commodityGradeDescription = aplvm.Description;

                _commodityViewModelBuilder.AddCommodityGrades
                    (commodityId, gradeId, commodityGradeName, usageTypeId, commodityGradeCode, commodityGradeDescription);

                return RedirectToAction("ListGrades", new { commodity = commodityId });

            }
            catch (DomainValidationException ve)
            {
                ValidationSummary.DomainValidationErrors(ve, ModelState);
                ViewBag.msg2 = ve.Message;
                Log.ErrorFormat("Error in editing commodity grade" + ve.Message);
                Log.InfoFormat("Error in editing commodity grade" + ve.Message);
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewBag.msg2 = ex.Message;
                Log.ErrorFormat("Error in editing commodity grade" + ex.Message);
                Log.InfoFormat("Error in editing commodity grade" + ex.Message);
                return View();
            }
        }

        public ActionResult DeactivateGrade(Guid commodity, Guid gradeId)
        {
            try
            {
                _commodityViewModelBuilder.SetGradeInactive(commodity, gradeId);
                TempData["msg"] = "Grade Successfully Deactivated";
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                TempData["msg"] = dve.Message;
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                Log.ErrorFormat("Error in deactivating commodity grade" + ex.Message);
                Log.InfoFormat("Error in deactivating commodity grade" + ex.Message);

            }
            return RedirectToAction("ListGrades", new { commodity });
        }


        public ActionResult ActivateGrade(Guid commodity, Guid gradeId)
        {
            try
            {
                _commodityViewModelBuilder.SetGradeActive(commodity, gradeId);
                TempData["msg"] = "Grade Successfully activated";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                Log.ErrorFormat("Error in activating commodity grade" + ex.Message);
                Log.InfoFormat("Error in activating commodity grade" + ex.Message);

            }
            return RedirectToAction("ListGrades", new { commodity });
        }

        [Authorize(Roles = "RoleDeleteMasterData")]
        public ActionResult DeleteGrade(Guid commodity, Guid gradeId)
        {
            try
            {
                _commodityViewModelBuilder.SetGradeAsDeleted(commodity, gradeId);
                TempData["msg"] = "Grade Successfully deleted";
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                TempData["msg"] = dve.Message;
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                Log.ErrorFormat("Error in deleting commodity grade" + ex.Message);
                Log.InfoFormat("Error in deleting commodity grade" + ex.Message);

            }
            return RedirectToAction("ListGrades", new { commodity });
        }
    }
}
