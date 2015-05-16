using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Distributr.Core.Utility;
using Distributr.HQ.Lib.Paging;
using System.Web.Mvc;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.CompetitorViewModelBuilders;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Admin.CompetitorViewModel;
using Distributr.HQ.Lib.Validation;
using log4net;
using System.Reflection;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Products;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Transaction;

namespace Distributr.HQ.Web.Areas.Admin.Controllers.GlobalSettings.CompetitorManagement
{
     [Authorize]
    public class CompetitorProductController : Controller
    { 
        IAuditLogViewModelBuilder _auditLogViewModelBuilder;
        IProductFlavoursViewModelBuilder _productFlavourViewModelBuilder;
        ICompetitorProductsViewModelBuilder _competitorProductsViewModel;
        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public CompetitorProductController(ICompetitorProductsViewModelBuilder competitorProductsViewModel, IProductFlavoursViewModelBuilder productFlavourViewModelBuilder, IAuditLogViewModelBuilder auditLogViewModelBuilder)
        {
            _competitorProductsViewModel = competitorProductsViewModel;
            _productFlavourViewModelBuilder = productFlavourViewModelBuilder;
            _auditLogViewModelBuilder = auditLogViewModelBuilder;
        }
        public ActionResult Index()
        {
            return View();
        }
          [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListCompetitorProducts(bool showInactive = false, int page = 1, int itemsperpage = 10, string srchParam= "")
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
             /*   var ls = _competitorProductsViewModel.GetAll(showinactive);
                int currentPageIndex = page.HasValue ? page.Value - 1 : 0;*/
                ViewBag.srchParam = srchParam;
                
                var currentPageIndex = page - 1 < 0 ? 0 : page - 1;
                var take = itemsperpage;
                var skip = currentPageIndex * take;
                var query = new QueryStandard { Name = srchParam, ShowInactive = showinactive, Skip = skip, Take = take };
                var ls = _competitorProductsViewModel.Query(query);
                var total = ls.Count;
                var data = ls.Data.ToList();
                return View(data.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage,total));
            }
            catch (Exception ex)
            {
                _log.Debug("Failed to list competitor products" + ex.Message);
                _log.Error("Failed to list competitor products" + ex.ToString());
                ViewBag.msg = ex.Message;
                return View();
            }
        }
     
          [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult CreateProduct()
        {
            ViewBag.Brands = _competitorProductsViewModel.GetBrand();
            ViewBag.Competitors = _competitorProductsViewModel.GetCompetitor();
            ViewBag.Flavours = _competitorProductsViewModel.GetFlavour();
            ViewBag.PackTypes = _competitorProductsViewModel.GetPackType();
            ViewBag.Packs = _competitorProductsViewModel.GetPackaging();
            ViewBag.Types = _competitorProductsViewModel.GetProdType();
            return View("CreateProduct",new CompetitorProductsViewModel());
        }
        [HttpPost]
        public ActionResult CreateProduct(CompetitorProductsViewModel cpvm)
        {
            ViewBag.Brands = _competitorProductsViewModel.GetBrand();
            ViewBag.Competitors = _competitorProductsViewModel.GetCompetitor();
            ViewBag.Flavours = _competitorProductsViewModel.GetFlavour();
            ViewBag.PackTypes = _competitorProductsViewModel.GetPackType();
            ViewBag.Packs = _competitorProductsViewModel.GetPackaging();
            ViewBag.Types = _competitorProductsViewModel.GetProdType();
            try
            {
                cpvm.Id = Guid.NewGuid();
                _competitorProductsViewModel.Save(cpvm);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Create", "Competitor Products", DateTime.Now);
                TempData["msg"] = "Competitor Product Successfully Created";
                return RedirectToAction("ListCompetitorProducts");
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                          
                _log.Debug("Failed to create competitor products" + dve.Message);
                _log.Error("Failed to creat competitor products" + dve.ToString());
                return View();
            }
            catch (Exception ex)
            {
               // ViewBag.msg = ex.Message;
                            
                _log.Debug("Failed to create competitor products" + ex.Message);
                _log.Error("Failed to create competitor products" + ex.ToString());
                return View();
            }
        }
          [Authorize(Roles = "RoleModifyMasterData")]
        public ActionResult EditProduct(Guid id)
        {
            ViewBag.Brands = _competitorProductsViewModel.GetBrand();
            ViewBag.Competitors = _competitorProductsViewModel.GetCompetitor();
            ViewBag.Flavours = _competitorProductsViewModel.GetFlavour();
            ViewBag.PackTypes = _competitorProductsViewModel.GetPackType();
            ViewBag.Packs = _competitorProductsViewModel.GetPackaging();
            ViewBag.Types = _competitorProductsViewModel.GetProdType();
            try
            {
                CompetitorProductsViewModel cmp = _competitorProductsViewModel.GetById(id);
                return View(cmp);
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }
        [HttpPost]
        public ActionResult EditProduct(CompetitorProductsViewModel cpvm)
        {
            ViewBag.Brands = _competitorProductsViewModel.GetBrand();
            ViewBag.Competitors = _competitorProductsViewModel.GetCompetitor();
            ViewBag.Flavours = _competitorProductsViewModel.GetFlavour();
            ViewBag.PackTypes = _competitorProductsViewModel.GetPackType();
            ViewBag.Packs = _competitorProductsViewModel.GetPackaging();
            ViewBag.Types = _competitorProductsViewModel.GetProdType();
            try
            {
            _competitorProductsViewModel.Save(cpvm);
            _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Edit", "Competitor Products", DateTime.Now);
            TempData["msg"] = "Competitor Product Successfully Edited";

                return RedirectToAction("ListCompetitorProducts");
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve,ModelState);
                return View();
            }
            catch(Exception ex)
            {
                      
                _log.Debug("Failed to edit competitor products" + ex.Message);
                _log.Error("Failed to edit competitor products" + ex.ToString());
                return View();
            }
        }
        public ActionResult DeActivate(Guid id)
        {
            try
            {
                _competitorProductsViewModel.SetInactive(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Deactivate", "Competitor Products", DateTime.Now);
                TempData["msg"] = "Successfully Deactivated";

            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                         
                _log.Debug("Failed to deactivate competitor products" + ex.Message);
                _log.Error("Failed to deactivate competitor products" + ex.ToString());
                
            }

            return RedirectToAction("ListCompetitorProducts");
        }
        [HttpPost]
        public ActionResult SubBrands(Guid brand)
        {
            try
            {
                var brands = _productFlavourViewModelBuilder.GetByBrand(brand);
                return Json(new { ok = true, data = brands, message = "ok" });
            }
            catch (Exception exx)
            {
                _log.ErrorFormat("Error in getting sub brand as per brand " + exx.Message + "Brand Id=" + brand);
                _log.InfoFormat("Error in getting sub brand as per brand " + exx.Message + "Brand Id=" + brand);
                return Json(new { ok = false, message = exx.Message });
            }
        }

        public ActionResult Activate(Guid id)
        {
            try
            {
                _competitorProductsViewModel.SetActive(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Activate", "Competitor Products", DateTime.Now);
                TempData["msg"] = "Successfully Activated";

            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;

                _log.Debug("Failed to activate competitor products" + ex.Message);
                _log.Error("Failed to activate competitor products" + ex.ToString());

            }

            return RedirectToAction("ListCompetitorProducts");
        }
        public ActionResult Delete(Guid id)
        {
            try
            {
                _competitorProductsViewModel.SetAsDeleted(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Delete", "Competitor Products", DateTime.Now);
                TempData["msg"] = "Successfully Deleted";

            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;

                _log.Debug("Failed to delete competitor products" + ex.Message);
                _log.Error("Failed to delete competitor products" + ex.ToString());

            }

            return RedirectToAction("ListCompetitorProducts");
        }

       }

}
