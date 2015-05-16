using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Agrimanagr.HQ.Models;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Master.SettingsEntities;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Master.SettingsRepositories;
using Distributr.Core.Security;
using Distributr.Core.Utility;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Products;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Admin.Product;
using Distributr.HQ.Lib.Validation;
using MvcContrib.Pagination;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.CostCenter;
using log4net;
using System.Reflection;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Transaction;
using System.Configuration;
using System.Diagnostics;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.Paging;

namespace Distributr.HQ.Web.Areas.Admin.Controllers
{
    //[Authorize ]
    public class ProductPricingController : Controller
    { 
       IProductPricingViewModelBuilder _ppViewModelBuilder;
       IEditProductPricingViewModelBuilder _editProductPricingViewModelBuilder;
       IAuditLogViewModelBuilder _auditLogViewModelBuilder;
       IAddPricingLineItemsViewModelBuilder _addPricingLineItemViewModelBuilder;
       protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private IProductRepository _productRepository;
        private ISettingsRepository _settingsRepository;
       public ProductPricingController(IProductRepository productRepository,IProductPricingViewModelBuilder productPricingViewModelBuilder, IEditProductPricingViewModelBuilder editProductPricingViewModelBuilder, IAddPricingLineItemsViewModelBuilder addPricingLineItemViewModelBuilder,IAuditLogViewModelBuilder auditLogViewModelBuilder, ISettingsRepository settingsRepository)
       {
           _ppViewModelBuilder = productPricingViewModelBuilder;
           _addPricingLineItemViewModelBuilder = addPricingLineItemViewModelBuilder;
           _editProductPricingViewModelBuilder = editProductPricingViewModelBuilder;
           _auditLogViewModelBuilder=auditLogViewModelBuilder;
           _settingsRepository = settingsRepository;
           ViewBag.productList = _ppViewModelBuilder.ProductList();
           ViewBag.tierList = _ppViewModelBuilder.TierList();
           _productRepository = productRepository;
       }
        public ActionResult Index()
        {
            return View();
        }



        public ActionResult Details(Guid id)
        {
            ProductPricingViewModel model = _ppViewModelBuilder.Get(id);
            return View(model);
        }


        [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult Create()
        {
            var settings = _settingsRepository.GetByKey(SettingsKeys.NumberOfDecimalPlaces);
            var decimalPlaces = settings != null ? settings.Value : "4";
            ViewBag.DecimalPlaces = decimalPlaces;

            return View(new ProductPricingViewModel());
        }
        [HttpPost]
        public ActionResult GetExfactoryPrice(Guid productId)
        {
            Product p = _productRepository.GetById(productId);
            decimal price = 0;
            if (p != null)
                price = p.ExFactoryPrice;
            return Json(price);
        }


        [HttpPost]
        public ActionResult Create(ProductPricingViewModel productPricingViewModel)
        {
            try
            {
                var settings = _settingsRepository.GetByKey(SettingsKeys.NumberOfDecimalPlaces);
                var decimalPlaces = settings != null ? settings.Value : "4";
                ViewBag.DecimalPlaces = decimalPlaces;

                if (productPricingViewModel.CurrentExFactory > productPricingViewModel.CurrentSellingPrice)
                {
                    ModelState.AddModelError("","Exfactory should be less than wholesale price");
                    return View(productPricingViewModel);
                }
                else
                {
                    productPricingViewModel.Id = Guid.NewGuid();
                    _ppViewModelBuilder.Save(productPricingViewModel);
                    _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Create", "Product Pricing", DateTime.Now);
                    TempData["msg"]="Pricing Successfully Created";
                    return RedirectToAction("List");
                }
            }
            catch (DomainValidationException ve)
            {
                ValidationSummary.DomainValidationErrors(ve, ModelState);
                //ViewBag.msg = ve.Message;
                _log.ErrorFormat("Error in creating product pricing"+ve.Message);
                _log.InfoFormat("Error in creating product pricing" + ve.Message);
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewBag.msg = ex.Message;
                _log.ErrorFormat("Error in creating product pricing" + ex.Message);
                _log.InfoFormat("Error in creating product pricing" + ex.Message);
                return View();
            }
        }
        [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListPricingItems(Guid id, int? itemsperpage)
        {
            try
            {
                if (itemsperpage != null)
                {
                    ViewModelBase.ItemsPerPage = itemsperpage.Value;
                }
                EditProductPricingViewModel al = _editProductPricingViewModelBuilder.Get(id);
                return View(al);
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.ToString();
                string exception = ex.Message;
                try
                {
                    HQMailerViewModelBuilder hqm = new HQMailerViewModelBuilder(ConfigurationSettings.AppSettings["ServerIP"], ConfigurationSettings.AppSettings["UserName"], ConfigurationSettings.AppSettings["Password"]);


                    hqm.Send(ConfigurationSettings.AppSettings["ServerEmail"], ConfigurationSettings.AppSettings["MailGroup"], "Listing Product Pricing", ex.ToString());
                }
                catch { }
                    ProductPricingViewModel productPricingVM = new ProductPricingViewModel();

                productPricingVM.ErrorText = exception;
                return View(productPricingVM);
            }
        }
         [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult CreatePricingItems(Guid id)
        {
            //return View("CreatePricingItems", new AddPricingLineItemsViewModel());
             var settings = _settingsRepository.GetByKey(SettingsKeys.NumberOfDecimalPlaces);
             var decimalPlaces = settings != null ? settings.Value : "4";
             ViewBag.DecimalPlaces = decimalPlaces;
             var decimalCharacter = string.Format("N{0}", decimalPlaces);


            try
            {
                ProductPricingViewModel model = _ppViewModelBuilder.Get(id);
                var p = _productRepository.GetById(model.ProductId);
                AddPricingLineItemsViewModel alvm = new AddPricingLineItemsViewModel
                {
                    id = id,
                    CurrentExFactory =p!=null?p.ExFactoryPrice:0,
                };
                
                return View(alvm);
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }
        [HttpPost]
        public ActionResult CreatePricingItems(AddPricingLineItemsViewModel aplvm)
        {
            try
            {
                var settings = _settingsRepository.GetByKey(SettingsKeys.NumberOfDecimalPlaces);
                var decimalPlaces = settings != null ? settings.Value : "4";
                ViewBag.DecimalPlaces = decimalPlaces;

                if (aplvm.CurrentExFactory > aplvm.CurrentSellingPrice)
                {
                    ModelState.AddModelError("", "Exfactory should be less than selling price");
                    return View(aplvm);
                }
                //if (Decimal.Round(aplvm.CurrentSellingPrice, 4) != aplvm.CurrentSellingPrice)
                //{
                   
                //    return View(aplvm);
                    
                //}
                if (Decimal.Round(aplvm.CurrentSellingPrice, Convert.ToInt32(decimalPlaces)) != aplvm.CurrentSellingPrice)
                {

                    return View(aplvm);

                }
                else
                {
                    
                
                Guid id = aplvm.id;
                DateTime dt = aplvm.CurrentEffectiveDate;
                decimal currSellingPrice = aplvm.CurrentSellingPrice;
                decimal exFactory = aplvm.CurrentExFactory;
                _addPricingLineItemViewModelBuilder.AddPricingLineItem(id, dt, currSellingPrice, exFactory);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Create", "Product Pricing Items", DateTime.Now);
                TempData["msg"] = "Pricing Item Successfully Created";
                return RedirectToAction("EditProductPricing", new { @id = aplvm.id });
           }
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                ViewBag.msg = dve.Message;
                var modelId = new AddPricingLineItemsViewModel();
                modelId.id = aplvm.id;
                _log.ErrorFormat("Error in creating product pricing" + dve.Message);
                _log.InfoFormat("Error in creating product pricing" + dve.Message);
                return View(aplvm);
            }
            catch (Exception exx)
            {
               ViewBag.msg= exx.Message;
               _log.ErrorFormat("Error in creating product pricing" + exx.Message);
               _log.InfoFormat("Error in creating product pricing" + exx.Message);
               return View(aplvm);
            }
        }
        [Authorize(Roles = "RoleModifyMasterData")]
        public ActionResult Edit(Guid id)
        {
            try
            {
                ProductPricingViewModel model = _ppViewModelBuilder.Get(id);
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }

      

        [HttpPost]
        public ActionResult Edit(ProductPricingViewModel productPricingViewModel)
        {
            try
            {

                _ppViewModelBuilder.Save(productPricingViewModel);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Edit", "Product Pricing", DateTime.Now);
                //TempData["msg"] = "Pricing Successfully Edited";
                return RedirectToAction("List");
            }
            catch (DomainValidationException ve)
            {
                ValidationSummary.DomainValidationErrors(ve, ModelState);
                _log.ErrorFormat("Error in editing product pricing" + ve.Message);
                _log.InfoFormat("Error in editing product pricing" + ve.Message);
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                _log.ErrorFormat("Error in editing product pricing" + ex.Message);
                _log.InfoFormat("Error in editing product pricing" + ex.Message);
                return View();
            }
        }
         [Authorize(Roles = "RoleModifyMasterData")]
        public ActionResult EditProductPricing(Guid id)
        {
            try
            {
                if (TempData["msg"] != null)
                {
                    ViewBag.msg = TempData["msg"].ToString();
                    TempData["msg"] = null;
                }
                EditProductPricingViewModel epv = _editProductPricingViewModelBuilder.Get(id);

                var settings = _settingsRepository.GetByKey(SettingsKeys.NumberOfDecimalPlaces);
                var decimalPlaces = settings != null ? settings.Value : "4";
                ViewBag.DecimalFormat = string.Format("N{0}", decimalPlaces);

                return View(epv);
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }
        [HttpPost]
         public ActionResult createPricingItem(Guid id, decimal exFactory, decimal sellingPrice, DateTime effectiveDate)
        {
            try
            {
                _editProductPricingViewModelBuilder.AddPricingItem(id, exFactory, sellingPrice, effectiveDate);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Create", "Product Pricing Items", DateTime.Now);
                TempData["msg"] = "Pricing Successfully Created";
                return RedirectToAction("EditProductPricing", new { id = id });
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }
         [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult List(bool? showInactive, string searchText,int page = 1)
        {
            try
            {
                var user = (CustomIdentity)this.User.Identity;
                Guid? supplerid = user != null ? user.SupplierId : (Guid?)null; 
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                int pageSize = 10;
                bool showinactive = false;
                if (showInactive != null)
                    showinactive = (bool)showInactive;
                ViewBag.showInactive = showinactive;
                //if (itemsperpage != null)
                //{
                //    ViewModelBase.ItemsPerPage = itemsperpage;
                //}
                if (TempData["msg"] != null)
                {
                    ViewBag.msg = TempData["msg"].ToString();
                } 
                stopWatch.Stop();
                TimeSpan ts = stopWatch.Elapsed;


                // Format and display the TimeSpan value.
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    ts.Hours,
                    ts.Minutes,
                    ts.Seconds,
                    ts.TotalMilliseconds);


                stopWatch.Reset();
                ViewBag.msg = "";
                _log.InfoFormat("Product Pricing\tTime taken to get all product pricing" + elapsedTime);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Timestamp", "Product Pricing Controller:" + elapsedTime, DateTime.Now);

                int currentPageIndex = page < 0 ? 0 : page - 1;
                int take = DistributorWebHelper.GetItemPerPage();
                int skip = currentPageIndex*take;


                var query = new QueryStandard()
                {
                    ShowInactive = showinactive,
                    Skip = skip,
                    Take = take,
                    Name = searchText,
                    SupplierId = supplerid
                };
                
                ViewBag.searchParam = searchText;
                var ls = _ppViewModelBuilder.Query(query);
                var data = ls.Data;
                
                var count = ls.Count;


                var settings = _settingsRepository.GetByKey(SettingsKeys.NumberOfDecimalPlaces);
                var decimalPlaces = settings != null ? settings.Value : "4";
                ViewBag.DecimalFormat = string.Format("N{0}", decimalPlaces);

                return View(data.ToPagedList(currentPageIndex, take,count));
                
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error in listing product pricing" + ex.Message);
                _log.InfoFormat("Error in listing product pricing" + ex.Message);
                ViewBag.msg = ex.ToString();
                string exception = ex.Message;
                try
                {
                    HQMailerViewModelBuilder hqm = new HQMailerViewModelBuilder(ConfigurationSettings.AppSettings["ServerIP"], ConfigurationSettings.AppSettings["UserName"], ConfigurationSettings.AppSettings["Password"]);


                    hqm.Send(ConfigurationSettings.AppSettings["ServerEmail"], ConfigurationSettings.AppSettings["MailGroup"], "Listing Product Pricing", ex.ToString());
                }
                catch (Exception exx) { }
                    ProductPricingViewModel productPricingVM = new ProductPricingViewModel();
                productPricingVM.ErrorText = exception;
                return View(productPricingVM);
            }
        
        }

         public ActionResult Deactivate(Guid id)
        {
            try

            {

                _ppViewModelBuilder.SetInactive(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Deactivate", "Product Pricing", DateTime.Now);
                TempData["msg"] = "Successfully Deactivated";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.ErrorFormat("Error in deactivating product pricing" + ex.Message);
                _log.InfoFormat("Error in deactivating product pricing" + ex.Message);

            }
            return RedirectToAction("List");
        }

         public ActionResult Delete(Guid id)
         {
             try
             {
                 _ppViewModelBuilder.SetAsDeleted(id);
                 _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Deleted", "Product Pricing", DateTime.Now);
                 TempData["msg"] = "Successfully Deleted";
             }
             catch (Exception ex)
             {
                 TempData["msg"] = ex.Message;
                 _log.ErrorFormat("Error in deleting product pricing" + ex.Message);
                 _log.InfoFormat("Error in deleting product pricing" + ex.Message);
             }
             return RedirectToAction("List");
         }

        public ActionResult Activate(Guid id, string name)
        {
            try
            {
                _ppViewModelBuilder.SetActive(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Activate", "Product Pricing", DateTime.Now);
                TempData["msg"] = name + " Successfully Activated";
            }
            catch (Exception ex)
            {

                TempData["msg"] = ex.Message;
                _log.ErrorFormat("Error in activating product pricing" + ex.Message);
                _log.InfoFormat("Error in activating product pricng" + ex.Message);
            }
            return RedirectToAction("List");
        }

    }
}
