using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Agrimanagr.HQ.Areas.Agrimanagr.Controllers.FarmActivities;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.Mapping;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.Paging;
using Distributr.HQ.Lib.Validation;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Products;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Admin.Product;

namespace Agrimanagr.HQ.Areas.Agrimanagr.Controllers.Products
{
    public class ProductController : BaseController
    {
        private IProductRepository _productRepository;
        private IProductFlavourRepository _productFlavourRepository;
        private IProductPackagingRepository _productPackagingRepository;
        private IProductBrandRepository _productBrandRepository;
        private IProductPackagingTypeRepository _productPackagingTypeRepository;
        private IProductTypeRepository _productTypeRepository;
        private IVATClassRepository _vatClassRepository;

        private IProductViewModelBuilder _productViewModelBuilder;

        //
        // GET: /Agrimanagr/SaleProduct/

        public ProductController(IDTOToEntityMapping dtoToEntityMapping, IMasterDataToDTOMapping masterDataToDtoMapping, CokeDataContext context, IProductRepository productRepository, IProductFlavourRepository productFlavourRepository, IProductPackagingRepository productPackagingRepository, IProductBrandRepository productBrandRepository, IProductPackagingTypeRepository productPackagingTypeRepository, IProductTypeRepository productTypeRepository, IVATClassRepository vatClassRepository, IProductViewModelBuilder productViewModelBuilder) : base(dtoToEntityMapping, masterDataToDtoMapping, context)
        {
            _productRepository = productRepository;
            _productFlavourRepository = productFlavourRepository;
            _productPackagingRepository = productPackagingRepository;
            _productBrandRepository = productBrandRepository;
            _productPackagingTypeRepository = productPackagingTypeRepository;
            _productTypeRepository = productTypeRepository;
            _vatClassRepository = vatClassRepository;
            _productViewModelBuilder = productViewModelBuilder;
        }

        [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult Index(int page = 1, int itemsperpage = 10, bool showInactive = false, string srchParam = "")
        {
            if (TempData["msg"] != null)
            {
                ViewBag.msg = TempData["msg"].ToString();
                TempData["msg"] = null;
            }
            try
            {
                ViewBag.srchParam = srchParam;
                ViewBag.showInactive = showInactive;
                int currentPageIndex = page - 1 < 0 ? 0 : page - 1;
                int take = itemsperpage;
                int skip = currentPageIndex * take;
                var query = new QueryStandard { Skip = skip, Take = take, Name = srchParam, ShowInactive = showInactive };

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
            foreach(var product in productList)
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

        [Authorize(Roles = "RoleUpdateMasterData")]
        public ActionResult Edit(Guid? id, string prodType)
        {
            //switch to the right view.
            switch (prodType)
            {
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
                return View();
            }

        }

        //public ActionResult Edit(Guid? id)
        //{
        //    return RedirectToAction("Index");
        //}

        public ActionResult Deactivate(Guid id)
        {
            try
            {
                _productRepository.SetInactive(_productRepository.GetById(id));
                TempData["msg"] = "Product Deactivated Successfully";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                Log.Debug("Failed to deactivate product " + ex.Message);

            }
            return RedirectToAction("Index");
        }

        public ActionResult Activate(Guid id)
        {
            try
            {
                _productRepository.SetActive(_productRepository.GetById(id));
                TempData["msg"] = "Product Activated Successfully";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                Log.Debug("Failed to activate Product" + ex.Message);

            }

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "RoleDeleteMasterData")]
        public ActionResult Delete(Guid id)
        {
            try
            {
                _productRepository.SetAsDeleted(_productRepository.GetById(id));
                TempData["msg"] = "Product Deleted Successfully";
            }
            catch (DomainValidationException ex)
            {
                TempData["msg"] = ex.ValidationResults.Results.FirstOrDefault().ToString();
                Log.Debug("Failed to delete Product " + ex.Message);


            }

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult CreateSaleProduct()
        {
            ViewBag.BrandsList = _productBrandRepository.GetAll().Select(n=>new{n.Id,n.Name}).ToDictionary(s=>s.Id,s=>s.Name);
            ViewBag.FlavoursList =_productFlavourRepository.GetAll().Select(n=>new{n.Id,n.Description}).ToDictionary(s=>s.Id,s=>s.Description);
            ViewBag.PackagingsList = _productPackagingRepository.GetAll().Select(n=>new{n.Id,n.Description}).ToDictionary(s=>s.Id,s=>s.Description);
            ViewBag.PackTypeList = _productPackagingTypeRepository.GetAll().Select(n=>new{n.Id,n.Description}).ToDictionary(s=>s.Id,s=>s.Description);
            ViewBag.VatClassList = _vatClassRepository.GetAll().Select(n=>new{n.Id,n.VatClass}).ToDictionary(s=>s.Id,s=>s.VatClass);
            ViewBag.ProductTypeList = _productTypeRepository.GetAll().Select(n=>new{n.Id,n.Name}).ToDictionary(s=>s.Id,s=>s.Name);
            ViewBag.ReturnableProductList =_productRepository.GetAll().OfType<ReturnableProduct>().Select(n => new {n.Id, n.Description}).ToDictionary(s => s.Id, s => s.Description);
            
            return View("CreateSaleProduct", new SaleProductViewModel());
        }

        [HttpPost]
        public ActionResult CreateSaleProduct(SaleProductViewModel vm)
        {

            try
            {
                ViewBag.msg = TempData["msg"] = null;
               
                vm.Id = Guid.NewGuid();

                _productViewModelBuilder.SaveSaleProduct(vm);
                TempData["msg"] = "Sale Product Successfully created";
                return RedirectToAction("Index");
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                //ViewBag.msg = dve.Message;
                ViewBag.BrandsList = _productBrandRepository.GetAll().Select(n => new { n.Id, n.Name }).ToDictionary(s => s.Id, s => s.Name);
                ViewBag.FlavoursList = _productFlavourRepository.GetAll().Select(n => new { n.Id, n.Description }).ToDictionary(s => s.Id, s => s.Description);
                ViewBag.PackagingsList = _productPackagingRepository.GetAll().Select(n => new { n.Id, n.Description }).ToDictionary(s => s.Id, s => s.Description);
                ViewBag.PackTypeList = _productPackagingTypeRepository.GetAll().Select(n => new { n.Id, n.Description }).ToDictionary(s => s.Id, s => s.Description);
                ViewBag.VatClassList = _vatClassRepository.GetAll().Select(n => new { n.Id, n.VatClass }).ToDictionary(s => s.Id, s => s.VatClass);
                ViewBag.ProductTypeList = _productTypeRepository.GetAll().Select(n => new { n.Id, n.Name }).ToDictionary(s => s.Id, s => s.Name);
                ViewBag.ReturnableProductList = _productRepository.GetAll().OfType<ReturnableProduct>().Select(n => new { n.Id, n.Description }).ToDictionary(s => s.Id, s => s.Description);
                
                return View("CreateSaleProduct");
            }
            catch (Exception exx)
            {
                ViewBag.msg = exx.Message;

                ViewBag.BrandsList = _productBrandRepository.GetAll().Select(n => new { n.Id, n.Name }).ToDictionary(s => s.Id, s => s.Name);
                ViewBag.FlavoursList = _productFlavourRepository.GetAll().Select(n => new { n.Id, n.Description }).ToDictionary(s => s.Id, s => s.Description);
                ViewBag.PackagingsList = _productPackagingRepository.GetAll().Select(n => new { n.Id, n.Description }).ToDictionary(s => s.Id, s => s.Description);
                ViewBag.PackTypeList = _productPackagingTypeRepository.GetAll().Select(n => new { n.Id, n.Description }).ToDictionary(s => s.Id, s => s.Description);
                ViewBag.VatClassList = _vatClassRepository.GetAll().Select(n => new { n.Id, n.VatClass }).ToDictionary(s => s.Id, s => s.VatClass);
                ViewBag.ProductTypeList = _productTypeRepository.GetAll().Select(n => new { n.Id, n.Name }).ToDictionary(s => s.Id, s => s.Name);
                ViewBag.ReturnableProductList = _productRepository.GetAll().OfType<ReturnableProduct>().Select(n => new { n.Id, n.Description }).ToDictionary(s => s.Id, s => s.Description);
              
                return View();
            }
        }

        [Authorize(Roles = "RoleUpdateMasterData")]
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
                ViewBag.msg = exx.Message;
                return View("EditSaleProduct");
            }
        }

        [HttpPost]
        public ActionResult EditSaleProduct(EditSaleProductViewModelIn vm)
        {

            try
            {
                _productViewModelBuilder.Save(vm);
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
                ViewBag.Title = "Add Product";
               
                return View();
            }
        }

        [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult CreateReturnableProduct(int? id)
        {
            ViewBag.BrandsList = _productBrandRepository.GetAll().Select(n => new { n.Id, n.Name }).ToDictionary(s => s.Id, s => s.Name);
            ViewBag.PackagingsList = _productViewModelBuilder.GetPackaging();
            ViewBag.PackTypeList = _productViewModelBuilder.GetPackagingType();
            ViewBag.ReturnableTypeList = _productViewModelBuilder.GetReturnableType();
            ViewBag.FlavoursList = _productViewModelBuilder.GetFlavours();
            ViewBag.VatClassList = _productViewModelBuilder.VatClass();
            ViewBag.RetReturnableProductList = _productViewModelBuilder.GetRetReturnableProducts();
            ViewBag.Title = "Edit Returnable Product";
            try
            {
                return View("CreateReturnableProduct", new ReturnableProductViewModel());
            }
            catch (Exception exx)
            {
              
                return View();
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
              
                return View();
            }
            
        }

        [Authorize(Roles = "RoleUpdateMasterData")]
        public ActionResult EditReturnableProduct(Guid? id = null)
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
                return View(_productViewModelBuilder.CreateEditReturnableProductViewModel(id.Value));
            }
            catch (Exception exx)
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult EditReturnableProduct(ReturnableProductViewModel vm)
        {

            try
            {
                _productViewModelBuilder.CreateReturnableProduct(vm);
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
           
        }

        [Authorize(Roles = "RoleUpdateMasterData")]
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
                TempData["msg"] = "Consolidated Product Successfully Edited";
                return RedirectToAction("Index");
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                ViewBag.msg = dve.Message;
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
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
            return View("CreateConsolidatedProduct", new EditConsolidatedProductOut());
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
                    TempData["msg"] = "Consolidated Product Successfully Created";
                    return RedirectToAction("Index");
                }

            }
            catch (Exception ex)
            {
               
                return View();
            }
        }


        public JsonResult SubBrands(Guid brandId)
        {
            try
            {
                var flavours = _productFlavourRepository.GetByBrandId(brandId).Select(n=>new SelectListItem(){Value=n.Key.ToString(),Text=n.Value.ToString()});

                return Json(flavours,JsonRequestBehavior.AllowGet);
            }
            catch (Exception exx)
            {
                return Json(new SelectListItem { Text = "Select Branch", Value = Guid.Empty.ToString() },
                            JsonRequestBehavior.AllowGet);
            }
        }




    }
}
