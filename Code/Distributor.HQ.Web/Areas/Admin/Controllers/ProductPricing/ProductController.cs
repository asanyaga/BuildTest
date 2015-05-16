using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Agrimanagr.HQ.Models;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Security;
using Distributr.Core.Utility;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Products;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Admin.Product;
using Distributr.HQ.Lib.Validation;
using log4net;
using System.Reflection;

using Distributr.HQ.Lib.ViewModelBuilders.Admin.Transaction;
using System.Web;

using System.IO;
using System.Data.OleDb;
using System.Diagnostics;
using System.Configuration;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.Paging;

namespace Distributr.HQ.Web.Areas.Admin.Controllers
{
    //[Authorize ]
    [HandleError]
    public class ProductController : Controller
    {
        IProductViewModelBuilder _productViewModelBuilder;
        IProductFlavoursViewModelBuilder _productFlavourViewModelBuilder;
        IAuditLogViewModelBuilder _auditLogViewModelBuilder;
        private IProductRepository _productRepository;
        private FileInfo fi = null;
        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ProductController(IProductViewModelBuilder productViewModelBuilder, IProductFlavoursViewModelBuilder productFlavourViewModelBuilder, IAuditLogViewModelBuilder auditLogViewModelBuilder, IProductRepository productRepository)
        {
            _productViewModelBuilder = productViewModelBuilder;
            _productFlavourViewModelBuilder = productFlavourViewModelBuilder;
            _auditLogViewModelBuilder = auditLogViewModelBuilder;
            _productRepository = productRepository;
        }
      
       
          [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult Index(int page = 1, bool showInactive = false, string srchParam = "")
        {
           
            if (TempData["msg"] != null)
            {
                ViewBag.msg = TempData["msg"].ToString();
                TempData["msg"] = null;
            }
            try
            {
                var user = (CustomIdentity)this.User.Identity;
                Guid? supplerid = user != null ? user.SupplierId : (Guid?)null;
                ViewBag.srchParam = srchParam;
                ViewBag.showInactive = showInactive;
                ViewBag.SearchText = srchParam;
                int currentPageIndex = page - 1 < 0 ? 0 : page - 1;
                int take = DistributorWebHelper.GetItemPerPage();
                int skip = currentPageIndex * take;
                var query = new QueryStandard { Skip = skip, Take = take, Name = srchParam, ShowInactive = showInactive, SupplierId = supplerid };
                
                var result = _productRepository.Query(query);
                var item = MapToViewModel(result.Data.Cast<Product>());
                int total = result.Count;
                var data = item.ToPagedList(currentPageIndex, take, total);
                return View(data);
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Error loading products \nDetails:" + ex.Message;
            }
            return View();
        }

        private IEnumerable<ListProductViewModel> MapToViewModel(IEnumerable<Product> productList)
        {
            var products = new List<ListProductViewModel>();
            foreach (var product in productList)
            {
                products.Add(new ListProductViewModel()
                {
                    Brand = product.Brand.Name,
                    Flavour = product.GetType() == typeof(SaleProduct) ? ((SaleProduct)product).Flavour == null ? "" : ((SaleProduct)product).Flavour.Name : "",
                    RetProdFalvour = product.GetType() == typeof(ReturnableProduct) ? ((ReturnableProduct)product).Flavour == null ? "" : ((ReturnableProduct)product).Flavour.Name : "",
                    Code = product.ProductCode,
                    Description = product.Description,
                    PackagingName = product.Packaging == null ? "" : product.Packaging.Name,
                    VatClass = product.VATClass == null ? "" : product.VATClass.VatClass,
                    ProductId = product.Id,
                    isActive = product._Status == EntityStatus.Active ? true : false,
                    ProductTypeName = product.GetType().ToString().Split('.').Last(),
                    ProductType = product.GetType() == typeof(SaleProduct) ? ((SaleProduct)product).ProductType == null ? "" : ((SaleProduct)product).ProductType.Name : "",
                });
            }
            return products;
        }


        [Authorize(Roles = "RoleModifyMasterData")]
        public ActionResult Edit(Guid? id, string prodType)
        {
            //switch to the right view.
            switch(prodType){
                case "SaleProduct":
                    return EditSaleProduct(id);
                case "ConsolidatedProduct":
                    return EditConsolidatedProduct(id);
                case "ReturnableProduct":
                    return EditConsolidatedProduct(id);
                default:
                    return null;
            }
        
        }

        [HttpPost]
        public ActionResult Edit(EditSaleProductViewModelIn vm)
        {
            try
            {
                return View(_productViewModelBuilder.CreateEditSaleProductViewModel(vm.Id));
            }
            catch (Exception ex)
            {

                return View("Index");
            }

        }
        [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult CreateSaleProduct()
        {
            ViewBag.BrandsList = _productViewModelBuilder.GetBrands();
            ViewBag.FlavoursList = _productViewModelBuilder.GetFlavours();
            ViewBag.PackagingsList = _productViewModelBuilder.GetPackaging();
            ViewBag.PackTypeList = _productViewModelBuilder.GetPackagingType();
           ViewBag.VatClassList = _productViewModelBuilder.VatClass();
           ViewBag.ProductTypeList = _productViewModelBuilder.GetProductType();
           ViewBag.ReturnableProductList = _productViewModelBuilder.GetReturnableProducts();
            return View("CreateSaleProduct", new SaleProductViewModel());
        }
        [HttpPost]
        public ActionResult CreateSaleProduct(SaleProductViewModel vm)
        { 
            
         try
         {
             ViewBag.msg = TempData["msg"] = null;
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                vm.Id = Guid.NewGuid();
                _productViewModelBuilder.SaveSaleProduct(vm);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Create", "Sale Product", DateTime.Now);
                stopWatch.Stop();
                TimeSpan ts = stopWatch.Elapsed;


                // Format and display the TimeSpan value.
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    ts.Hours,
                    ts.Minutes,
                    ts.Seconds,
                    ts.TotalMilliseconds);


                stopWatch.Reset();


                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Timestamp", "Product Controller:Creat Sale Product:" + elapsedTime, DateTime.Now);
                TempData["msg"] = "Sale Product Successfully created";
                return RedirectToAction("Index");
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                //ViewBag.msg = dve.Message;
                ViewBag.BrandsList = _productViewModelBuilder.GetBrands();
                ViewBag.FlavoursList = _productViewModelBuilder.GetFlavours();
                ViewBag.PackagingsList = _productViewModelBuilder.GetPackaging();
                ViewBag.PackTypeList = _productViewModelBuilder.GetPackagingType();
                ViewBag.ProductTypeList = _productViewModelBuilder.GetProductType();
                ViewBag.VatClassList = _productViewModelBuilder.VatClass();
                ViewBag.ReturnableProductList = _productViewModelBuilder.GetReturnableProducts();
                _log.ErrorFormat("Error in creating product " + dve.Message);
                _log.InfoFormat("Error in creating product " + dve.Message);
                return View("CreateSaleProduct");
            }
            catch (Exception exx)
            {
                ViewBag.msg = exx.Message;

                ViewBag.BrandsList = _productViewModelBuilder.GetBrands();
                ViewBag.FlavoursList = _productViewModelBuilder.GetFlavours();
                ViewBag.PackagingsList = _productViewModelBuilder.GetPackaging();
                ViewBag.PackTypeList = _productViewModelBuilder.GetPackagingType();
                ViewBag.ProductTypeList = _productViewModelBuilder.GetProductType();
                ViewBag.VatClassList = _productViewModelBuilder.VatClass();
                ViewBag.ReturnableProductList = _productViewModelBuilder.GetReturnableProducts();
                _log.ErrorFormat("Error in creating product " + exx.Message);
                _log.InfoFormat("Error in creating product " + exx.Message);
               
                return View(vm);
            }
        }
        [Authorize(Roles = "RoleModifyMasterData")]
        public ActionResult EditSaleProduct(Guid? id = null)
        {
            ViewBag.ReturnableProductList = _productViewModelBuilder.GetReturnableProducts();
            ViewBag.BrandsList = _productViewModelBuilder.GetBrands();
            ViewBag.FlavoursList = _productViewModelBuilder.GetFlavours();
            ViewBag.PackagingsList = _productViewModelBuilder.GetPackaging();
            ViewBag.PackTypeList = _productViewModelBuilder.GetPackagingType();
            ViewBag.ProductTypeList = _productViewModelBuilder.GetProductType();
            ViewBag.VatClassList = _productViewModelBuilder.VatClass();
            try
            {
                ViewBag.ReturnableProductList = _productViewModelBuilder.GetReturnableProducts();
                return View(_productViewModelBuilder.CreateEditSaleProductViewModel(id.Value));
            }
            catch (Exception exx)
            {
                    return View();
            }
        }

        [HttpPost]
        public ActionResult EditSaleProduct(EditSaleProductViewModelIn vm)
        {
          
            try
            {
                _productViewModelBuilder.Save(vm);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Edit", "Sale Product", DateTime.Now);
                TempData["msg"] = "Sale Product Successfully Edited";
                return RedirectToAction("Index");
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                ViewBag.ReturnableProductList = _productViewModelBuilder.GetReturnableProducts();
                ViewBag.BrandsList = _productViewModelBuilder.GetBrands();
                ViewBag.FlavoursList = _productViewModelBuilder.GetFlavours();
                ViewBag.PackagingsList = _productViewModelBuilder.GetPackaging();
                ViewBag.PackTypeList = _productViewModelBuilder.GetPackagingType();
                ViewBag.ProductTypeList = _productViewModelBuilder.GetProductType();
                ViewBag.VatClassList = _productViewModelBuilder.VatClass();
                _log.ErrorFormat("Error in editing sale product " + dve.Message);
                _log.InfoFormat("Error in editing sale product " + dve.Message);
                ViewBag.Title = "Add Product";
                ViewBag.msg = dve.Message;
                return View();
            }
            catch (Exception exx)
            {
                ViewBag.msg = exx.Message;
                ViewBag.ReturnableProductList = _productViewModelBuilder.GetReturnableProducts();
                ViewBag.BrandsList = _productViewModelBuilder.GetBrands();
                ViewBag.FlavoursList = _productViewModelBuilder.GetFlavours();
                ViewBag.PackagingsList = _productViewModelBuilder.GetPackaging();
                ViewBag.PackTypeList = _productViewModelBuilder.GetPackagingType();
                ViewBag.ProductTypeList = _productViewModelBuilder.GetProductType();
                ViewBag.VatClassList = _productViewModelBuilder.VatClass();
                _log.ErrorFormat("Error in editing sale product " + exx.Message);
                _log.InfoFormat("Error in editing sale product " + exx.Message);
                ViewBag.Title = "Add Product";
                
                return View();
            }
        }
        [Authorize(Roles="RoleAddMasterData")]
        public ActionResult CreateReturnableProduct(int? id)
        {
            ViewBag.BrandsList = _productViewModelBuilder.GetBrands();
            ViewBag.PackagingsList = _productViewModelBuilder.GetPackaging();
            ViewBag.PackTypeList = _productViewModelBuilder.GetPackagingType();
            ViewBag.ReturnableTypeList = _productViewModelBuilder.GetReturnableType();
            ViewBag.FlavoursList = _productViewModelBuilder.GetFlavours();
            ViewBag.VatClassList = _productViewModelBuilder.VatClass();
            ViewBag.RetReturnableProductList = _productViewModelBuilder.GetRetReturnableProducts();
            ViewBag.Title = "Edit Returnable Product";
            try
            {
                return View("CreateReturnableProduct",new ReturnableProductViewModel());
            }
            catch (Exception exx)
            {
                _log.ErrorFormat("Error in editing returnable product " + exx.Message);
                _log.InfoFormat("Error in editing returnable product " + exx.Message);
               
                return View("Index");
            }
        }

        [HttpPost]
        public ActionResult CreateReturnableProduct(ReturnableProductViewModel vm)
        {
           
            try
            {
                ViewBag.msg = null;
                vm.Id = Guid.NewGuid();
                _productViewModelBuilder.CreateReturnableProduct(vm);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Edit", "Returnable Product", DateTime.Now);
                TempData["msg"] = "Returnable Product Successfully Created";

                return RedirectToAction("Index");
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                ViewBag.BrandsList = _productViewModelBuilder.GetBrands();
                ViewBag.PackagingsList = _productViewModelBuilder.GetPackaging();
                ViewBag.PackTypeList = _productViewModelBuilder.GetPackagingType();
                ViewBag.ReturnableTypeList = _productViewModelBuilder.GetReturnableType();
                ViewBag.FlavoursList = _productViewModelBuilder.GetFlavours();
                ViewBag.VatClassList = _productViewModelBuilder.VatClass();
                ViewBag.RetReturnableProductList = _productViewModelBuilder.GetRetReturnableProducts();
                _log.ErrorFormat("Error in editing returnable product " + dve.Message);
                _log.InfoFormat("Error in editing returnable product " + dve.Message);
                ViewBag.Title = "Edit Returnable Product";
                
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                ViewBag.msg = ex.ToString();
                ViewBag.BrandsList = _productViewModelBuilder.GetBrands();
                ViewBag.PackagingsList = _productViewModelBuilder.GetPackaging();
                ViewBag.PackTypeList = _productViewModelBuilder.GetPackagingType();
                ViewBag.ReturnableTypeList = _productViewModelBuilder.GetReturnableType();
                ViewBag.FlavoursList = _productViewModelBuilder.GetFlavours();
                ViewBag.VatClassList = _productViewModelBuilder.VatClass();
                ViewBag.RetReturnableProductList = _productViewModelBuilder.GetRetReturnableProducts();
                ViewBag.Title = "Edit Returnable Product";
                try
                {
                    HQMailerViewModelBuilder hqm = new HQMailerViewModelBuilder(ConfigurationSettings.AppSettings["ServerIP"], ConfigurationSettings.AppSettings["UserName"], ConfigurationSettings.AppSettings["Password"]);


                    hqm.Send(ConfigurationSettings.AppSettings["ServerEmail"], ConfigurationSettings.AppSettings["MailGroup"], "Test", "editing sale product error:" + ex.Message);
                }
                catch (Exception exx)
                { }
                return View(vm);
            }
            
        }

         [Authorize(Roles = "RoleModifyMasterData")]
        public ActionResult EditReturnableProduct(Guid? id= null)
        {
            ViewBag.BrandsList = _productViewModelBuilder.GetBrands();
            ViewBag.PackagingsList = _productViewModelBuilder.GetPackaging();
            ViewBag.PackTypeList = _productViewModelBuilder.GetPackagingType();
            ViewBag.ReturnableTypeList = _productViewModelBuilder.GetReturnableType();
            ViewBag.FlavoursList = _productViewModelBuilder.GetFlavours();
            ViewBag.VatClassList = _productViewModelBuilder.VatClass();
            ViewBag.RetReturnableProductList = _productViewModelBuilder.GetRetReturnableProducts();
            //var ReturnableProductList = _productViewModelBuilder.GetRetReturnableProducts();
            //ReturnableProductList.Add(Guid.Empty, "----Select ReturnablProduct----");
            //ViewBag.RetReturnableProductList = ReturnableProductList;
            ViewBag.Title = "Edit Returnable Product";
            try
            {
                return View(_productViewModelBuilder.CreateEditReturnableProductViewModel(id.Value));
            }
            catch (Exception exx)
            {
                _log.ErrorFormat("Error in editing returnable product " + exx.Message);
                _log.InfoFormat("Error in editing returnable product " + exx.Message);
               
                return View();
            }
        }

        [HttpPost]
        public ActionResult EditReturnableProduct(ReturnableProductViewModel vm)
        {
            
            try
            {
                _productViewModelBuilder.CreateReturnableProduct(vm);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Edit", "Returnable Product", DateTime.Now);
                TempData["msg"] = "Returnable Product Successfully Edited";
                return RedirectToAction("Index");
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                ViewBag.BrandsList = _productViewModelBuilder.GetBrands();
                ViewBag.PackagingsList = _productViewModelBuilder.GetPackaging();
                ViewBag.PackTypeList = _productViewModelBuilder.GetPackagingType();
                ViewBag.ReturnableTypeList = _productViewModelBuilder.GetReturnableType();                
                ViewBag.FlavoursList = _productViewModelBuilder.GetFlavours();
                ViewBag.VatClassList = _productViewModelBuilder.VatClass();
                ViewBag.RetReturnableProductList = _productViewModelBuilder.GetRetReturnableProducts();                
                _log.ErrorFormat("Error in editing returnable product " + dve.Message);
                _log.InfoFormat("Error in editing returnable product " + dve.Message);
                ViewBag.Title = "Edit Returnable Product";
                ViewBag.msg = dve.Message;
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                ViewBag.msg = ex.ToString();
                ViewBag.BrandsList = _productViewModelBuilder.GetBrands();
                ViewBag.PackagingsList = _productViewModelBuilder.GetPackaging();
                ViewBag.PackTypeList = _productViewModelBuilder.GetPackagingType();
                ViewBag.ReturnableTypeList = _productViewModelBuilder.GetReturnableType();
                ViewBag.FlavoursList = _productViewModelBuilder.GetFlavours();
                ViewBag.VatClassList = _productViewModelBuilder.VatClass();
                ViewBag.RetReturnableProductList = _productViewModelBuilder.GetRetReturnableProducts();
                ViewBag.Title = "Edit Returnable Product";
                
                return View();
            }
            //return View(_productViewModelBuilder.CreateEditReturnableProductViewModel(vm.Id));
        }
         [Authorize(Roles = "RoleModifyMasterData")]
        public ActionResult EditConsolidatedProduct(Guid? id = null)
        {
            ViewBag.BrandsList = _productViewModelBuilder.GetBrands();
           
            ViewBag.PackagingsList = _productViewModelBuilder.GetPackaging();
            ViewBag.PackTypeList = _productViewModelBuilder.GetPackagingType();
            ViewBag.ProductTypeList = _productViewModelBuilder.GetProductType();
            try
            {
                return View(_productViewModelBuilder.CreateEditConsolidatedProductViewModel(id.Value));
            }
            catch (Exception exx)
            {
               
                return View();
            }
        }

        [HttpPost]
         public ActionResult EditConsolidatedProduct(EditConsolidatedProductOut vm, Guid? thisId)
        {
            
            ViewBag.BrandsList = _productViewModelBuilder.GetBrands();
            ViewBag.FlavoursList = _productViewModelBuilder.GetFlavours();
            ViewBag.PackagingsList = _productViewModelBuilder.GetPackaging();
            ViewBag.PackTypeList = _productViewModelBuilder.GetPackagingType();
            ViewBag.ProductTypeList = _productViewModelBuilder.GetProductType();
            //vm.Id = thisId.Value;
            try
            {
                //vm.Id = thisId;
                _productViewModelBuilder.Save(vm);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Edit", "Consolidated Product", DateTime.Now);
                TempData["msg"] = "Consolidated Product Successfully Edited";
                return RedirectToAction("Index");
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                _log.ErrorFormat("Error in consolidated product " + dve.Message);
                _log.InfoFormat("Error in consolidated product " + dve.Message);
                ViewBag.msg = dve.Message;
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                _log.ErrorFormat("Error in consolidated product pricing" + ex.Message);
                _log.InfoFormat("Error in consolidated product pricing" + ex.Message);
                try
                {
                    HQMailerViewModelBuilder hqm = new HQMailerViewModelBuilder(ConfigurationSettings.AppSettings["ServerIP"], ConfigurationSettings.AppSettings["UserName"], ConfigurationSettings.AppSettings["Password"]);


                    hqm.Send(ConfigurationSettings.AppSettings["ServerEmail"], ConfigurationSettings.AppSettings["MailGroup"], "Test", "editing sale product error:" + ex.Message);
                }
                catch (Exception exx)
                { }
                return View();
            }
        }
         [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult CreateConsolidatedProduct(Guid? id, bool isNew = false)
        {
            ViewBag.BrandsList = _productViewModelBuilder.GetBrands();
         
            ViewBag.PackagingsList = _productViewModelBuilder.GetPackaging();
            ViewBag.PackTypeList = _productViewModelBuilder.GetPackagingType();

            Guid _id = Guid.Empty;
            if (isNew)
                _id = _productViewModelBuilder.Save(id.Value, 1);
            //return View("CreateConsolidatedProduct", _productViewModelBuilder.CreateEditConsolidatedProductViewModel(_id));
           return View("CreateConsolidatedProduct",new EditConsolidatedProductOut());
        }

        [HttpPost]
         public ActionResult CreateConsolidatedProduct(EditConsolidatedProductOut spvm, Guid? thisId)
        {
            ViewBag.BrandsList = _productViewModelBuilder.GetBrands();
           
            ViewBag.PackagingsList = _productViewModelBuilder.GetPackaging();
            ViewBag.PackTypeList = _productViewModelBuilder.GetPackagingType();
           
            try
            {
                if (spvm.Description == null)
                {
                    ModelState.AddModelError("Consolidated Product", "Consolidated product is required");
                   //return View();
                   TempData["msg"] = "Product Name Is Required";
                   ViewBag.msg = TempData["msg"].ToString();
                    return RedirectToAction("CreateConsolidatedProduct", new { @thisId = 0 });
                }
                else
                {
                    spvm.Id = thisId.Value;
                    spvm.Id = Guid.NewGuid();
                    _productViewModelBuilder.Save(spvm);
                    _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Create", "Consolidated Product", DateTime.Now);
                    _log.Info("Saving Consolidated Product:" + spvm);
                    TempData["msg"] = "Consolidated Product Successfully Created";
                    return RedirectToAction("Index");
                }
                
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error in creating consolidated product " + ex.Message);
                _log.InfoFormat("Error in creating consolidated product " + ex.Message);
                try
                {
                    HQMailerViewModelBuilder hqm = new HQMailerViewModelBuilder(ConfigurationSettings.AppSettings["ServerIP"], ConfigurationSettings.AppSettings["UserName"], ConfigurationSettings.AppSettings["Password"]);


                    hqm.Send(ConfigurationSettings.AppSettings["ServerEmail"], ConfigurationSettings.AppSettings["MailGroup"], "Test", "editing sale product error:" + ex.Message);
                }
                catch (Exception exx)
                { }
                return View();
            }
        }

        [Authorize(Roles = "RoleModifyMasterData")]
        public ActionResult AddItem(Guid id)
        {
            ViewBag.BrandsList = _productViewModelBuilder.GetBrands();

            ViewBag.PackagingsList = _productViewModelBuilder.GetPackaging();
            ViewBag.PackTypeList = _productViewModelBuilder.GetPackagingType();
            try
            {
                return View("CreateConsolidatedProduct", _productViewModelBuilder.CreateEditConsolidatedProductViewModel(id));
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
               
                return View("Index");
            }
        }

        [HttpPost]
        public ActionResult AddItem(EditConsolidatedProductOut spvm, Guid parentId)
        {
            ViewBag.BrandsList = _productViewModelBuilder.GetBrands();

            ViewBag.PackagingsList = _productViewModelBuilder.GetPackaging();
            ViewBag.PackTypeList = _productViewModelBuilder.GetPackagingType();
            try
            {
                spvm.Id = parentId;
                _productViewModelBuilder.AddItemToConsolidatedProduct(spvm);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Create", "Consolidated Product Item", DateTime.Now);
                return RedirectToAction("AddItem", new { id = spvm.Id });
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                try
                {
                    HQMailerViewModelBuilder hqm = new HQMailerViewModelBuilder(ConfigurationSettings.AppSettings["ServerIP"], ConfigurationSettings.AppSettings["UserName"], ConfigurationSettings.AppSettings["Password"]);


                    hqm.Send(ConfigurationSettings.AppSettings["ServerEmail"], ConfigurationSettings.AppSettings["MailGroup"], "Test", "editing sale product error:" + ex.Message);
                }
                catch (Exception exx)
                { }
                return RedirectToAction("AddItem", new { id = spvm.Id });
            }
        }
        [Authorize(Roles = "RoleModifyMasterData")]
        public ActionResult RemoveItem(Guid id)
        {
            return View("CreateConsolidatedProduct", _productViewModelBuilder.CreateEditConsolidatedProductViewModel(id));
        }

        [HttpPost]
        public ActionResult RemoveItem(Guid parentId, Guid itemId)
        {
            try
            {
                _productViewModelBuilder.RemoveItemFromConsolidatedProduct(parentId, itemId);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Remove", "Consolidated Product Item", DateTime.Now);
                return RedirectToAction("AddItem", new { id = parentId });
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
      
                return View("index");
            }
        }

        public ActionResult Deactivate(Guid id)
        {
            try
            {
                _productViewModelBuilder.SetInActive(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Deactivate", "Product", DateTime.Now);
                TempData["msg"] = "Successfully Deactivated";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                ViewBag.msg = ex.Message;
                _log.ErrorFormat("Error in deactvating product " + ex.Message);
                _log.InfoFormat("Error in deactivatng product " + ex.Message);
                try
                {
                    HQMailerViewModelBuilder hqm = new HQMailerViewModelBuilder(ConfigurationSettings.AppSettings["ServerIP"], ConfigurationSettings.AppSettings["UserName"], ConfigurationSettings.AppSettings["Password"]);


                    hqm.Send(ConfigurationSettings.AppSettings["ServerEmail"], ConfigurationSettings.AppSettings["MailGroup"], "Test", "editing sale product error:" + ex.Message);
                }
                catch (Exception exx)
                { }
            }
            return RedirectToAction("Index");
        }


        public ActionResult Delete(Guid id) {
            try
            {
                _productViewModelBuilder.SetAsDeleted(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Delete", "Product", DateTime.Now);
                TempData["msg"] = "Successfully Deleted";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                ViewBag.msg = ex.Message;
                _log.ErrorFormat("Error in deleting product " + ex.Message);
                _log.InfoFormat("Error in deleting product " + ex.Message);
                try
                {
                    HQMailerViewModelBuilder hqm = new HQMailerViewModelBuilder(ConfigurationSettings.AppSettings["ServerIP"], ConfigurationSettings.AppSettings["UserName"], ConfigurationSettings.AppSettings["Password"]);


                    hqm.Send(ConfigurationSettings.AppSettings["ServerEmail"], ConfigurationSettings.AppSettings["MailGroup"], "Test", "editing sale product error:" + ex.Message);
                }
                catch (Exception exx)
                { }
            }
            return RedirectToAction("Index");
            
        }

        public ActionResult Activate(Guid id, string name)
        {
            try
            {
                _productViewModelBuilder.SetActive(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Activate","Product",DateTime.Now);
                TempData["msg"] =  " Successfully Activated";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                ViewBag.msg = ex.Message;
                _log.ErrorFormat("Error in Activating product " + ex.Message);
                _log.InfoFormat("Error in Activating product " + ex.Message);

            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult SubBrands(Guid brandId)
        {
            try
            {
                var brands = _productFlavourViewModelBuilder.GetByBrand(brandId);
               
                return Json(new { ok = true, data = brands, message = "ok" });
            }
            catch (Exception exx)
            {
                _log.ErrorFormat("Error in getting sub brand as per brand " + exx.Message+"Brand Id="+brandId);
                _log.InfoFormat("Error in getting sub brand as per brand " + exx.Message + "Brand Id=" + brandId);
                try
                {
                    HQMailerViewModelBuilder hqm = new HQMailerViewModelBuilder(ConfigurationSettings.AppSettings["ServerIP"], ConfigurationSettings.AppSettings["UserName"], ConfigurationSettings.AppSettings["Password"]);


                    hqm.Send(ConfigurationSettings.AppSettings["ServerEmail"], ConfigurationSettings.AppSettings["MailGroup"], "Test", "editing sale product error:" + exx.Message);
                }
                catch (Exception ex)
                { }
                return Json(new { ok=false,message=exx.Message});
            }
        }
        public ActionResult ImportProduct()
        {
            return View("ImportProduct", new SaleProductViewModel());
        }
        //[HttpPost]
        //public ActionResult ImportProducts(HttpPostedFileBase file)
        //{

        //    try
        //    {
        //        app = new Application();

        //        // extract only the fielname
        //        var fileName = Path.GetFileName(file.FileName);
        //        // store the file inside ~/App_Data/uploads folder
        //        var path = Path.Combine(Server.MapPath("~/Uploads"), fileName);
        //        file.SaveAs(path);

        //        string fileExtension = Path.GetExtension(fileName);
        //        if (fileExtension == ".xlsx")
        //        {
        //            ViewBag.msg = "Please wait. Upload in progress";
        //            Workbook workBook = app.Workbooks.Open(path, 0, true, 5, "", "", true, XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
        //            Worksheet workSheet = (Worksheet)workBook.ActiveSheet;
        //            int index = 0;
        //            object rowIndex = 2;
        //            object colIndex1 = 1;
        //            object colIndex2 = 2;
        //            object colIndex3 = 3;
        //            object colIndex4 = 4;
        //            object colIndex5 = 5;
        //            object colIndex6 = 6;
        //            object colIndex7 = 7;
        //            object colIndex8 = 8;
        //            object colIndex9 =9;
        //            object colIndex10 = 10;

        //            while (((Range)workSheet.Cells[rowIndex, colIndex1]).Value2 != null)
        //            {

        //                string product = ((Range)workSheet.Cells[rowIndex, colIndex1]).Value2.ToString();
        //                string brandCode = ((Range)workSheet.Cells[rowIndex, colIndex2]).Value2.ToString();
        //                string subBrandCode = ((Range)workSheet.Cells[rowIndex, colIndex3]).Value2.ToString();
        //                string packagingTypeCode = ((Range)workSheet.Cells[rowIndex, colIndex4]).Value2.ToString();
        //                string packagingCode = ((Range)workSheet.Cells[rowIndex, colIndex5]).Value2.ToString();
        //                string productTypeCode = ((Range)workSheet.Cells[rowIndex, colIndex6]).Value2.ToString();
        //                string vatClass = ((Range)workSheet.Cells[rowIndex, colIndex7]).Value2.ToString();
        //                string productCode = ((Range)workSheet.Cells[rowIndex, colIndex8]).Value2.ToString();
        //                string description = ((Range)workSheet.Cells[rowIndex, colIndex9]).Value2.ToString();
        //                string returnableType = ((Range)workSheet.Cells[rowIndex, colIndex10]).Value2.ToString();
        //                //bool hasDuplicateName = _productViewModelBuilder.GetProductList()
        //                //    .Any(p => p.Name == Name);
        //                //bool hasDuplicateCode = _productViewModelBuilder.GetAll()
        //                //    .Any(p => p.Code == code);

        //                //if (hasDuplicateName || hasDuplicateCode)
        //                //{ }
        //                //else
        //                //{
        //                if (product == "Sale Product")
        //                {
        //                    SaleProductViewModel pdvm = new SaleProductViewModel();
        //                    pdvm.brandCode = brandCode;
        //                    pdvm.subBrandCode = subBrandCode;
        //                    pdvm.packTypeCode =packagingTypeCode;
        //                    pdvm.packCode = packagingCode;
        //                    pdvm.productTypeCode = productTypeCode;
        //                    pdvm.vatClass = vatClass;
        //                    pdvm.ProductCode = productCode;
        //                    pdvm.Description = description;
                            
        //                    _productViewModelBuilder.SaveSaleProduct(pdvm);
        //                }
        //                else if (product== "Returnable Product")
        //                {
        //                    EditReturnableProductViewModelIn pdvm = new EditReturnableProductViewModelIn();
        //                    pdvm.brandCode = brandCode;
        //                    pdvm.subBrandCode = subBrandCode;
        //                    pdvm.packTypeCode = packagingTypeCode;
        //                    pdvm.packCode = packagingCode;
        //                    pdvm.productTypeCode = productTypeCode;
        //                    pdvm.vatClass = vatClass;
        //                    pdvm.ProductCode = productCode;
        //                    pdvm.Description = description;
        //                    pdvm.RetunableTypeName = returnableType;
        //                    _productViewModelBuilder.Save(pdvm);
        //                }
        //                else if (product == "Consolidated Product")
        //                {
        //                    EditConsolidatedProductOut vm = new EditConsolidatedProductOut();
        //                    vm.brandCode = brandCode;
        //                    vm.packagingTypeCode =packagingTypeCode;
        //                    vm.packagingCode = subBrandCode;
        //                    vm.ProductCode = productTypeCode;
        //                    vm.Description = vatClass;
        //                    _productViewModelBuilder.Save(vm);
        //                }
        //                //}
        //                index++;
        //                rowIndex = 2 + index;

        //            }
        //            fi = new FileInfo(path);

        //            fi.Delete();
        //            _auditLogViewModelBuilder.addAuditLog(this.User.Identity.Name, "Import", "Product Brand", DateTime.Now);
        //            ViewBag.msg = "Upload Successful";
        //            return RedirectToAction("List");
        //        }
        //        else
        //        {
        //            ViewBag.msg = "Please upload excel file with extension .xlsx";
        //            return View();
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        ViewBag.msg = ex.ToString();
        //        return View();
        //    }


        //}
        [HttpPost]
        public ActionResult ImportProduct(HttpPostedFileBase file)
        {

            try
            {

                var fileName = Path.GetFileName(file.FileName);


                var directory = Server.MapPath("~/Uploads");
                if (Directory.Exists(directory) == false)
                {
                    Directory.CreateDirectory(directory);
                }
                var path = Server.MapPath("~/Uploads") + "\\" + fileName;


                file.SaveAs(path);


                string fileExtension = Path.GetExtension(fileName);
                if (fileExtension == ".xlsx")
                {
                    ViewBag.msg = "Please wait. Upload in progress";

                    string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties='Excel 12.0;HDR=YES;'";

                    OleDbConnection conn = new OleDbConnection(connectionString);
                    try
                    {
                        conn.Open();
                        string product = "";
                        OleDbCommand commProduct = new OleDbCommand("SELECT product FROM [Sheet1$]", conn);
                        OleDbDataReader readProduct = commProduct.ExecuteReader();
                        while (readProduct.Read())
                        {
                            product = readProduct["product"].ToString();
                        }
                        if (product == "Sale Product")
                        {
                            OleDbCommand command = new OleDbCommand("SELECT brandCode,subBrandCode,packagingtypeCode,packagingCode,productTypeCode,vatClass,code,description FROM [Sheet1$]", conn);
                            OleDbDataReader reader = command.ExecuteReader();
                            SaleProductViewModel pdvm = new SaleProductViewModel();
                            while (reader.Read())
                            {

                                string brandCode = reader["brandCode"].ToString();
                                string subBrandCode = reader["subBrandCode"].ToString();
                                string packagingTypeCode = reader["packagingTypeCode"].ToString();
                                string packagingCode = reader["packagingCode"].ToString();
                                string productTypeCode = reader["productTypeCode"].ToString();
                                string vatClass = reader["vatClass"].ToString();
                                string productCode = reader["code"].ToString();
                                string description = reader["description"].ToString();

                                pdvm.brandCode = brandCode;
                                pdvm.subBrandCode = subBrandCode;
                                pdvm.packTypeCode = packagingTypeCode;
                                pdvm.packCode = packagingCode;
                                pdvm.productTypeCode = productTypeCode;
                                pdvm.vatClass = vatClass;
                                pdvm.ProductCode = productCode;
                                pdvm.Description = description;
                                _productViewModelBuilder.SaveSaleProduct(pdvm);

                            }
                        }
                        else if (product == "Returnable Product")
                        {
                            OleDbCommand command = new OleDbCommand("SELECT Brand,SubBrand,PackagingType,Packaging,ProductType,VatClass,ProductCode,Description,returnableType FROM [Sheet1$]", conn);
                            OleDbDataReader reader = command.ExecuteReader();
                            EditReturnableProductViewModelIn pdvm = new EditReturnableProductViewModelIn();
                            while (reader.Read())
                            {

                                string brandCode = reader["brand"].ToString();
                                string subBrandCode = reader["subBrand"].ToString();
                                string packagingTypeCode = reader["packagingType"].ToString();
                                string packagingCode = reader["packaging"].ToString();
                                string productTypeCode = reader["productType"].ToString();
                                string vatClass = reader["vatClass"].ToString();
                                string productCode = reader["productcode"].ToString();
                                string description = reader["description"].ToString();
                                string returnableType = reader["returnableType"].ToString();

                                pdvm.brandCode = brandCode;
                                pdvm.subBrandCode = subBrandCode;
                                pdvm.packTypeCode = packagingTypeCode;
                                pdvm.packCode = packagingCode;
                                pdvm.productTypeCode = productTypeCode;
                                pdvm.vatClass = vatClass;
                                pdvm.ProductCode = productCode;
                                pdvm.Description = description;
                                pdvm.RetunableTypeName = returnableType;
                                _productViewModelBuilder.Save(pdvm);

                            }
                        }
                        else if (product == "Consolidated Product")
                        {
                            OleDbCommand command = new OleDbCommand("SELECT brandCode,packagingtypeCode,packagingCode,code,description FROM [Sheet1$]", conn);
                            OleDbDataReader reader = command.ExecuteReader();
                            EditConsolidatedProductOut vm = new EditConsolidatedProductOut();

                            string brandCode = reader["brandCode"].ToString();
                            string packagingTypeCode = reader["packagingTypeCode"].ToString();
                            string packagingCode = reader["packagingCode"].ToString();
                            string code = reader["Code"].ToString();
                            string description = reader["description"].ToString();

                            vm.brandCode = brandCode;
                            vm.packagingTypeCode = packagingTypeCode;
                            vm.packagingCode = packagingCode;
                            vm.ProductCode = code;
                            vm.Description = description;
                            _productViewModelBuilder.Save(vm);
                        }
                    }
                    catch (OleDbException ex)
                    {
                        ViewBag.msg = ex.ToString();
                        try
                        {
                            HQMailerViewModelBuilder hqm = new HQMailerViewModelBuilder(ConfigurationSettings.AppSettings["ServerIP"], ConfigurationSettings.AppSettings["UserName"], ConfigurationSettings.AppSettings["Password"]);


                            hqm.Send(ConfigurationSettings.AppSettings["ServerEmail"], ConfigurationSettings.AppSettings["MailGroup"], "Test", "editing sale product error:" + ex.Message);
                        }
                        catch (Exception exx)
                        { }
                        return View();
                    }

                    finally
                    {
                        conn.Close();

                    }

                    fi = new FileInfo(path);

                    fi.Delete();
                    _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Import", "Product", DateTime.Now);
                    ViewBag.msg = "Upload Successful";
                    return RedirectToAction("Index");
                }

                else
                {
                    fi = new FileInfo(path);

                    fi.Delete();
                    ViewBag.msg = "Please upload excel file with extension .xlsx";
                    return View();
                }
            }
            catch (Exception ex)
            {

                ViewBag.msg = ex.ToString();
                try
                {
                    HQMailerViewModelBuilder hqm = new HQMailerViewModelBuilder(ConfigurationSettings.AppSettings["ServerIP"], ConfigurationSettings.AppSettings["UserName"], ConfigurationSettings.AppSettings["Password"]);


                    hqm.Send(ConfigurationSettings.AppSettings["ServerEmail"], ConfigurationSettings.AppSettings["MailGroup"], "Test", "editing sale product error:" + ex.Message);
                }
                catch (Exception exx)
                { }
                return View();
            }


        }
    }
}
