using System.Linq;
using System.Web.Mvc;
using Distributr.Core.Domain.Master;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Products;
using Distributr.HQ.Lib.ViewModels.Admin.Product;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Factory.Master;
using Distributr.Core.Repository.Master.ProductRepositories;
using System.Collections.Generic;
using System;
using Distributr.HQ.Lib.Helper;
using System.Reflection;
using log4net;
using System.Diagnostics;
using Distributr.Core.Repository.Transactional.AuditLogRepositories;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.Products.Impl
{
    public class ProductViewModelBuilder : IProductViewModelBuilder
    {
        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        IProductRepository _productRepository;
        IProductFactory _productFactory;
        IProductBrandRepository _productBrandRepository;
        IProductPackagingRepository _productPackagingRepository;
        IProductPackagingTypeRepository _productPackagingTypeRepository;
        IProductTypeRepository _productTypeRepository;
        IProductFlavourRepository _productFlavourRepository;
        IProductPricingFactory _productPricingFactory;
        IProductPricingRepository _productPricingRepository;
        IProductPricingTierRepository _productPricingTier;
        IVATClassRepository _vatClassRepository;
        IAuditLogRepository _auditLogRepository;
        //IReturnablesRepository _returnablesRepository;
        public ProductViewModelBuilder( IProductRepository productRepository,
                                        IProductFactory productFactory,
                                        IProductPackagingRepository productPackagingRepository, 
                                        IProductPackagingTypeRepository productPackagingTypeRepository,
                                        IProductBrandRepository productBrandRepository,
                                        IProductTypeRepository productTypeRepository,
                                        IProductFlavourRepository productFlavourFactory,
            IProductPricingTierRepository productPricingTier,
            IProductPricingFactory productPricingFactory,
        IProductPricingRepository productPricingRepository,
            IVATClassRepository vatClassRepository,
            IAuditLogRepository auditLogRepository
            //IReturnablesRepository returnablesRepository
            )
        {
            _productRepository = productRepository;
            _productFactory = productFactory;
            _productBrandRepository = productBrandRepository;
            _productPackagingRepository = productPackagingRepository;
            _productPackagingTypeRepository = productPackagingTypeRepository;
            _productTypeRepository = productTypeRepository;
            _productFlavourRepository = productFlavourFactory;
            _productPricingFactory = productPricingFactory;
            _productPricingRepository = productPricingRepository;
            _productPricingTier = productPricingTier;
            _vatClassRepository = vatClassRepository;
            _auditLogRepository = auditLogRepository;
            //_returnablesRepository = returnablesRepository;
        }

        public void SetInActive(Guid id)
        {
            _log.InfoFormat("Setting Product Inactive.Product id="+id);
            Product p = _productRepository.GetById(id);
            _productRepository.SetInactive(p);
        }

        public void SetActive(Guid id)
        {
            _log.InfoFormat("Setting Product Inactive.Product id="+id);
            Product p = _productRepository.GetById(id);
            _productRepository.SetActive(p);
        }

        public void SetAsDeleted(Guid id) {
            _log.InfoFormat("Deleting Product.Product id =" +id);
            Product p = _productRepository.GetById(id);
            _productRepository.SetAsDeleted(p);
        }
        public EditSaleProductViewModelOut CreateEditSaleProductViewModel(Guid productid)
        {
            //Product p      = _productRepository.GetById(productid);
            SaleProduct sp = (SaleProduct)_productRepository.GetById(productid);
            var pb         = _productBrandRepository.GetAll().OrderBy(d=>d.Name).ToDictionary(d => d.Id, d => d.Name);
            var pp         = _productPackagingRepository.GetAll().OrderBy(d=>d.Name).ToDictionary(d => d.Id, d => d.Name);
            var ppt        = _productPackagingTypeRepository.GetAll().OrderBy(d=>d.Name).ToDictionary(d => d.Id, d => d.Name);
            var pt         = _productTypeRepository.GetAll().OrderBy(d=>d.Name).ToDictionary(d => d.Id, d => d.Name);
            var pf         = _productFlavourRepository.GetAll().OrderBy(d=>d.Name).ToDictionary(d => d.Id, d => d.Name);
            var retProduct = _productRepository.GetAll().OrderBy(n => n.Description).ToList().ToDictionary(n => n.Id, n => n.Description);
            var vatClass = _vatClassRepository.GetAll().OrderBy(n => n.VatClass).ToList().ToDictionary(n=>n.Id,n=>n.VatClass);
            
            if (productid !=Guid.Empty)
            {
                var vm = new EditSaleProductViewModelOut
                {
                    Title           = "Edit Sale Product",
                    Id              = sp.Id,
                    Description     = sp.Description,
                    BrandId         = sp.Brand.Id,
                    PackagingId = sp.Packaging == null ? Guid .Empty: sp.Packaging.Id,
                    PackagingTypeId = sp.PackagingType.Id,
                    ProductCode     = sp.ProductCode,
                    ProductTypeID   = sp.ProductType.Id,
                    FlavourID = sp.Flavour == null ? Guid.Empty : sp.Flavour.Id,
                    VatClassId = sp.VATClass == null ? Guid.Empty : sp.VATClass.Id,
                    ReturnableProductId = sp.ReturnableProduct == null ? Guid.Empty : sp.ReturnableProduct.Id,
                    ExFactoryPrice=sp.ExFactoryPrice,
                    ProductPackagings = new SelectList(pp, "Key", "Value", sp.Packaging == null ? Guid.Empty : sp.Packaging.Id),
                    ProductPackagingTypes = new SelectList(ppt, "Key", "Value", sp.PackagingType.Id),
                    ProductBrands         = new SelectList(pb, "Key", "Value", sp.Brand.Id),
                    Flavours = new SelectList(pf, "Key", "Value", sp.Flavour == null ? Guid.Empty : sp.Flavour.Id),
                    ProductTypes          = new SelectList(pt, "Key", "Value", sp.ProductType),
                    VatClass = new SelectList(vatClass, "Key", "Value", sp.VATClass == null ? Guid.Empty : sp.VATClass.Id),
                    ReturnableProduct = new SelectList(retProduct, "Key", "Value", sp.ReturnableProduct == null ? Guid.Empty : sp.ReturnableProduct.Id),
                    
                };
                return vm;
            }
            else
            {
                var vm = new EditSaleProductViewModelOut
                {
                    Title                 = "Add Product",
                    ProductPackagings     = new SelectList(pp, "Key", "Value"),
                    ProductPackagingTypes = new SelectList(ppt, "Key", "Value"),
                    ProductBrands         = new SelectList(pb, "Key", "Value"),
                    Flavours              = new SelectList(pf, "Key", "Value"),
                    ProductTypes          = new SelectList(pt, "Key", "Value"),
                   
                };
                return vm;
            }
        }

        public EditReturnableProductViewModelOut CreateEditReturnableProductViewModel(Guid productid)
        {
            ReturnableProduct p = (ReturnableProduct)_productRepository.GetById(productid);
            var exceptionlist = _productRepository.GetAll().OfType<ReturnableProduct>().Where(n => n.ReturnAbleProduct == null).ToList();
            var exceptionlist2 = _productRepository.GetAll().OfType<ReturnableProduct>().Where(n => n.ReturnAbleProduct != null).Where(n => n.ReturnAbleProduct.Id != productid).ToList();
            var list = exceptionlist;
            list.AddRange(exceptionlist2);
            var pb = _productBrandRepository.GetAll().OrderBy(d => d.Name).ToDictionary(d => d.Id, d => d.Name);
            var pp = _productPackagingRepository.GetAll().OrderBy(d => d.Name).ToDictionary(d => d.Id, d => d.Name);
            var ppt = _productPackagingTypeRepository.GetAll().OrderBy(d => d.Name).ToDictionary(d => d.Id, d => d.Name);
            var rt    = _productRepository.GetAll().OrderBy(d=>d.ReturnableType).ToDictionary(d=>d.Id,d=>d.ReturnableType );
            var retProduct = list.OrderBy(n=>n.Description).Where(n=>n.Id!=productid).ToDictionary(n => n.Id, n => n.Description); // _productRepository.GetAll().OrderBy(d => d.Description).ToList().Where(n => n.Id != productid).Except(exceptionlist).ToDictionary(n => n.Id, n => n.Description);
            var vatClass = _vatClassRepository.GetAll().OrderBy(d => d.VatClass).ToList().ToDictionary(n => n.Id, n => n.VatClass);
            var subBrand = _productFlavourRepository.GetAll().OrderBy(d => d.Name).ToList().ToDictionary(n => n.Id, n => n.Name);
            if (productid !=Guid .Empty)
            {
                var vm = new EditReturnableProductViewModelOut
                {
                    Title                 = "Edit Returnable Product",
                    Id                    = p.Id,
                    Description           = p.Description,
                    BrandId               = p.Brand.Id,
                    PackagingId = p.Packaging == null ? Guid.Empty : p.Packaging.Id,
                    PackagingTypeId       = p.PackagingType.Id,
                    ProductCode           = p.ProductCode,
                    ReturnableProductId = p.ReturnAbleProduct == null ? Guid.Empty : p.ReturnAbleProduct.Id,
                    FlavourID = p.Flavour == null ? Guid.Empty : p.Flavour.Id,
                    VatClassId = p.VATClass == null ? Guid.Empty : p.VATClass.Id,
                      Capacity              =p.Capacity,
                      ExFactoryPrice=p.ExFactoryPrice,
                    ReturnableType=(int)p.ReturnableType,
                    ReturnableTypes=new SelectList (rt,"Key","Value"),
                    ProductPackagings = new SelectList(pp, "Key", "Value", p.Packaging == null ? Guid.Empty : p.Packaging.Id),
                    ProductPackagingTypes = new SelectList(ppt, "Key", "Value", p.PackagingType.Id),
                    ProductBrands         = new SelectList(pb, "Key", "Value", p.Brand.Id),
                    ReturnableProduct = new SelectList(retProduct, "Key", "Value", p.ReturnAbleProduct == null ? Guid.Empty : p.ReturnAbleProduct.Id),
                    VatClass = new SelectList(vatClass, "Key", "Value", p.VATClass == null ? Guid.Empty : p.VATClass.Id),
                    SubBrand = new SelectList(subBrand, "Key", "Value", p.Flavour == null ? Guid.Empty : p.Flavour.Id),


                };
                return vm;
            }
            else
            {
                var vm = new EditReturnableProductViewModelOut
                {
                    Title                 = "Create Returnable Product",
                    ProductPackagings     = new SelectList(pp, "Key", "Value"),
                    ProductPackagingTypes = new SelectList(ppt, "Key", "Value"),
                    ProductBrands         = new SelectList(pb, "Key", "Value"),
                    ReturnableTypes       = new SelectList(rt, "Key", "Value"),
                };
                return vm;
            }
        }

        public EditConsolidatedProductOut CreateEditConsolidatedProductViewModel(Guid productid)
        {
            ConsolidatedProduct p = _productRepository.GetById(productid) as ConsolidatedProduct;

            var pb  = _productBrandRepository.GetAll().ToDictionary(d => d.Id, d => d.Name);
            var pp  = _productPackagingRepository.GetAll().ToDictionary(d => d.Id, d => d.Name);
            var ppt = _productPackagingTypeRepository.GetAll().ToDictionary(d => d.Id, d => d.Name);
            var pL  = _productRepository.GetAll(false).ToDictionary(d => d.Id, d => d.Description);

            List<EditConsolidatedProductOut.ProductDetailViewModel> cppd = null;
            cppd = p.ProductDetails.Select(n => new EditConsolidatedProductOut.ProductDetailViewModel
                {
                    Brand         = n.Product.Brand.Name,
                    Descritpion   = n.Product.Description,
                    ProductCode   = n.Product.ProductCode,
                    Packaging     = n.Product.Packaging==null ?"": n.Product.Packaging.Name,
                    PackagingType = n.Product.PackagingType.Name,
                    ProductId     = n.Product.Id,
                    Qty           = n.QuantityPerConsolidatedProduct,
                }).ToList();

            //cn : filter to remove already existing products in Consolidated product and itself.
            pL = pL.Where(_p => (!cppd.Select(x => x.ProductId).Contains(_p.Key)) && _p.Key != p.Id).ToDictionary(d => d.Key, d => d.Value);
            var vm = new EditConsolidatedProductOut
            {
                Title                 = "Edit Consolidated Product",
                Id                    = p.Id,
                Description           = p.Description,
                BrandId               = p.Brand.Id,
                PackagingId = p.Packaging == null ? Guid.Empty : p.Packaging.Id,
                PackagingTypeId       = p.PackagingType.Id,
                ProductCode           = p.ProductCode,
                ProductDetails        = cppd,
                ExFactoryPrice=p.ExFactoryPrice,
                ProductPackagings = new SelectList(pp, "Key", "Value", p.Packaging == null ? Guid.Empty : p.Packaging.Id),
                ProductPackagingTypes = new SelectList(ppt, "Key", "Value", p.PackagingType.Id),
                ProductBrands         = new SelectList(pb, "Key", "Value", p.Brand.Id),
                ProductList           = new SelectList(pL, "Key", "Value"),
            };
            return vm;

           
        }

        public ListProductViewModel GetProductList(bool inactive = false)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            //ListProductViewModel vm = new ListProductViewModel();
            //vm.Title = "";
            //var items = _productRepository.GetAll();
            //foreach (var item in items)
            //{
            //    ListProductViewModel.ListProductViewModelItem vit = new ListProductViewModel.ListProductViewModelItem();
            //    if (vit.Brand != null)
            //    {
            //        vit.Brand = item.Brand.Name;
            //        vit.Code = item.ProductCode;
            //        vit.Description = item.Description;
            //        vit.Packaging = item.Packaging.Name;
            //        vit.ProductId = item.Id;
            //        vit.ProductType = item.GetType().ToString().Split('.').Last();
            //        vm.Items.Add(vit);
            //    }

            //}
            ListProductViewModel vm = new ListProductViewModel();
            vm.Title = "List Products";
            foreach (var n in _productRepository.GetAll(inactive))
            {
                vm.Items.Add(new ListProductViewModel.ListProductViewModelItem
                                   {
                                       Brand = n.Brand.Name,
                                       //Flavour=((SaleProduct )n).Flavour==null?"":((SaleProduct )n).Flavour.Name,
                                       Flavour = n.GetType() == typeof(SaleProduct) ? ((SaleProduct)n).Flavour == null ? "" : ((SaleProduct)n).Flavour.Name : "",
                                       RetProdFalvour = n.GetType() == typeof(ReturnableProduct) ? ((ReturnableProduct)n).Flavour == null ? "" : ((ReturnableProduct)n).Flavour.Name : "",
                                       Code = n.ProductCode,
                                       Description = n.Description,
                                       Packaging = n.Packaging == null ? "" : n.Packaging.Name,
                                       VatClassId = n.VATClass == null ? Guid.Empty : n.VATClass.Id,
                                       VatClass = n.VATClass == null ? "" : _vatClassRepository.GetById(n.VATClass.Id).VatClass,
                                       ProductId = n.Id,
                                       isActive = n._Status == EntityStatus.Active ? true : false,
                                       ProductType = n.GetType().ToString().Split('.').Last()
                                   });
            }    

            _log.Debug("Product list" + vm.Brands);
            _log.Debug("Product list" + vm.Packaging);
            _log.Debug("Product list" + vm.Brands);
            _log.Error("Product list" + vm);
            _log.InfoFormat("Product list"+vm);
            stopWatch.Stop();


            TimeSpan ts = stopWatch.Elapsed;


            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Console.WriteLine("RunTime " + elapsedTime);

            stopWatch.Reset();
            
            _log.InfoFormat("Product View Model Builder\tTime taken to get all products"+elapsedTime);
            
            return vm;                    
        }


        public IList<ListProductViewModel> GetAll(bool inactive = false)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();            
            ListProductViewModel vm = new ListProductViewModel();
            vm.Title = "List Products";
            var items = _productRepository.GetAll(inactive).Select(s => new ListProductViewModel
            {
                    Brand = s.Brand.Name,
                    Flavour = s.GetType() == typeof(SaleProduct) ? ((SaleProduct)s).Flavour == null ? "" : ((SaleProduct)s).Flavour.Name : "",
                    RetProdFalvour = s.GetType() == typeof(ReturnableProduct) ? ((ReturnableProduct)s).Flavour == null ? "" : ((ReturnableProduct)s).Flavour.Name : "",
                    Code = s.ProductCode,
                    Description = s.Description,
                    PackagingName = s.Packaging == null ? "" : s.Packaging.Name,
                    VatClassId = s.VATClass == null ? Guid.Empty : s.VATClass.Id,
                    VatClass = s.VATClass == null ? "" : _vatClassRepository.GetById(s.VATClass.Id).VatClass,
                    ProductId = s.Id,
                    isActive = s._Status == EntityStatus.Active ? true : false,
                    ProductTypeName = s.GetType().ToString().Split('.').Last(),
                    ProductType = s.GetType() == typeof(SaleProduct) ? ((SaleProduct)s).ProductType == null ? "" : ((SaleProduct)s).ProductType.Name : "",
                }).ToList();
            
            return items;
        }

        public IList<ListProductViewModel> GetAll(string searchText, int pageIndex, int pageSize, out int count, bool includeDeactivated = false)
        {
            var vm = new ListProductViewModel();
            vm.Title = "List Products";
            var items = _productRepository.GetAll(searchText, pageIndex, pageSize, out count, includeDeactivated).Select(s => new ListProductViewModel
            {
                Brand = s.Brand.Name,
                Flavour = s.GetType() == typeof(SaleProduct) ? ((SaleProduct)s).Flavour == null ? "" : ((SaleProduct)s).Flavour.Name : "",
                RetProdFalvour = s.GetType() == typeof(ReturnableProduct) ? ((ReturnableProduct)s).Flavour == null ? "" : ((ReturnableProduct)s).Flavour.Name : "",
                Code = s.ProductCode,
                Description = s.Description,
                PackagingName = s.Packaging == null ? "" : s.Packaging.Name,
                VatClassId = s.VATClass == null ? Guid.Empty : s.VATClass.Id,
                VatClass = s.VATClass == null ? "" : _vatClassRepository.GetById(s.VATClass.Id).VatClass,
                ProductId = s.Id,
                isActive = s._Status == EntityStatus.Active ? true : false,
                ProductTypeName = s.GetType().ToString().Split('.').Last(),
                ProductType = s.GetType() == typeof(SaleProduct) ? ((SaleProduct)s).ProductType == null ? "" : ((SaleProduct)s).ProductType.Name : "",
            }).ToList();

            return items;
        }


        public void Save(EditSaleProductViewModelIn vm)
        {
            _log.InfoFormat("Saving Sale Product");
            SaleProduct sp = _productRepository.GetById(vm.Id) as SaleProduct;
            if (sp == null)
                sp = _productFactory.CreateSaleProduct(sp.Id);
            sp.Brand         = _productBrandRepository.GetById(vm.BrandId);
            sp.Description   = vm.Description;
            sp.Packaging     = _productPackagingRepository.GetById(vm.PackagingId);
            sp.PackagingType = _productPackagingTypeRepository.GetById(vm.PackagingTypeId);
            sp.ProductCode   = vm.ProductCode;
            sp.Flavour       = _productFlavourRepository.GetById(vm.FlavourID);
            sp.ProductType   = _productTypeRepository.GetById(vm.ProductTypeID);
            sp.ExFactoryPrice = vm.ExFactoryPrice;
            if (vm.VatClassId != Guid.Empty)
                sp.VATClass = _vatClassRepository.GetById(vm.VatClassId);
            if (vm.ReturnableProductId != null)
                sp.ReturnableProduct = _productRepository.GetById(vm.ReturnableProductId.Value) as ReturnableProduct;
            _productRepository.Save(sp);
        }

        public void Save(EditReturnableProductViewModelIn vm)
        {
            if (vm.brandCode != null && vm.subBrandCode != null && vm.productTypeCode != null && vm.packTypeCode != null && vm.packCode != null && vm.vatClass != null)
            {
                ProductBrand pd = _productBrandRepository.GetAll().FirstOrDefault(n => n.Code == vm.brandCode);
                ProductFlavour pf = _productFlavourRepository.GetAll().FirstOrDefault(n => n.Code == vm.subBrandCode);
                ProductType pt = _productTypeRepository.GetAll().FirstOrDefault(n => n.Code == vm.productTypeCode);
                ProductPackaging ppg = _productPackagingRepository.GetAll().FirstOrDefault(n => n.Code == vm.packCode);
                ProductPackagingType ppgT = _productPackagingTypeRepository.GetAll().FirstOrDefault(n => n.Code == vm.packTypeCode);
                VATClass vc = _vatClassRepository.GetAll().FirstOrDefault(n => n.VatClass == vm.vatClass);
                ReturnableType rt = (ReturnableType)Enum.Parse(typeof(ReturnableType), vm.RetunableTypeName);

                ReturnableProduct rp = _productRepository.GetById(vm.Id) as ReturnableProduct;
                if (rp==null)
                    rp = _productFactory.CreateReturnableProduct(rp.Id);
                  
                rp.Brand = _productBrandRepository.GetById(pd.Id);
                rp.Description = vm.Description;
                rp.ExFactoryPrice = vm.ExFactoryPrice;
                rp.Packaging = _productPackagingRepository.GetById(ppg.Id);
                rp.PackagingType = _productPackagingTypeRepository.GetById(ppgT.Id);
                rp.ProductCode = vm.ProductCode;
                rp.Flavour = _productFlavourRepository.GetById(pf.Id);
                // = _productTypeRepository.GetById(pt.Id);
                rp.VATClass = _vatClassRepository.GetById(vc.Id);
                rp.ReturnableType = (ReturnableType)rt;
                rp.Capacity = vm.Capacity;
                _productRepository.Save(rp);
                if (vm.Id == Guid.Empty)
                {
                    addDefaultPriceForReturnable();
                }
            }
            else
            {
                ReturnableProduct rp = _productRepository.GetById(vm.Id) as ReturnableProduct;
                if (rp==null)
                    rp = _productFactory.CreateReturnableProduct(rp.Id);

              
                   
                rp.Brand = _productBrandRepository.GetById(vm.BrandId);
                rp.Flavour = _productFlavourRepository.GetById(vm.FlavourID);
                rp.Description = vm.Description;
                rp.Packaging = _productPackagingRepository.GetById(vm.PackagingId);
                rp.PackagingType = _productPackagingTypeRepository.GetById(vm.PackagingTypeId);
                rp.ProductCode = vm.ProductCode;
                rp.ReturnableType = (ReturnableType)vm.ReturnableType;
                rp.VATClass = _vatClassRepository.GetById(vm.VatClassId);
                rp.Capacity = vm.Capacity;
                if(rp.ReturnAbleProduct!=null)
                rp.ReturnAbleProduct = _productRepository.GetById(vm.ReturnableProductId.Value)as ReturnableProduct;

                _productRepository.Save(rp);
                if (vm.Id == Guid.Empty)
                {
                    addDefaultPriceForReturnable();
                }
            }
        }

        public void Save(EditConsolidatedProductOut vm)
        {
            if (vm.brandCode != null || vm.packagingCode != null || vm.packagingTypeCode != null)
            {
                ProductBrand pd = _productBrandRepository.GetAll().FirstOrDefault(n => n.Code == vm.brandCode);
                ProductPackaging ppg = _productPackagingRepository.GetAll().FirstOrDefault(n => n.Code == vm.packagingCode);
                ProductPackagingType ppgT = _productPackagingTypeRepository.GetAll().FirstOrDefault(n => n.Code == vm.packagingTypeCode);


                ConsolidatedProduct _cp = _productRepository.GetById(vm.Id) as ConsolidatedProduct;
                if (_cp==null)
                    _cp = _productFactory.SaveConsolidatedProduct(vm.Id);
                
                   
                _cp.Description = vm.Description;
                _cp.Brand = _productBrandRepository.GetById(pd.Id);
                _cp.Packaging = _productPackagingRepository.GetById(ppg.Id);
                _cp.PackagingType = _productPackagingTypeRepository.GetById(ppgT.Id);
                _cp.ProductCode = vm.ProductCode;
                _cp.ExFactoryPrice = vm.ExFactoryPrice;

                _productRepository.Save(_cp);
            }
            else
            {
                ConsolidatedProduct _cp = _productRepository.GetById(vm.Id) as ConsolidatedProduct;
                if (_cp==null)
                    _cp = _productFactory.SaveConsolidatedProduct(vm.Id);
                  
                _cp.Description = vm.Description;
                _cp.Brand = _productBrandRepository.GetById(vm.BrandId);
                _cp.Packaging = _productPackagingRepository.GetById(vm.PackagingId);
                _cp.PackagingType = _productPackagingTypeRepository.GetById(vm.PackagingTypeId);
                _cp.ProductCode = vm.ProductCode;
                _cp.ExFactoryPrice = vm.ExFactoryPrice;

                _productRepository.Save(_cp);
            }
            
            
            //Get Consolidated Product
           // var cp = _productRepository.GetAll().ToList().OfType<ConsolidatedProduct>().OrderByDescending(n => n._DateCreated).FirstOrDefault();
            //Save(cp.Id,1);
        }

        public Guid Save(Guid productID, int Qty)
        {
            Product _p = _productRepository.GetById(productID);
            ConsolidatedProduct newCP = _productFactory.CreateConsolidatedProduct(Guid.NewGuid(),_p, Qty);

            newCP.Brand         = _p.Brand;
            newCP.Description   = _p.Description;
            newCP.Packaging     = _p.Packaging;
            newCP.PackagingType = _p.PackagingType;
            newCP.ProductCode   = _p.ProductCode;

            return _productRepository.Save(newCP);
        }

        public void AddItemToConsolidatedProduct(EditConsolidatedProductOut vm)
        {
            ConsolidatedProduct _cp = _productRepository.GetById(vm.Id) as ConsolidatedProduct;
            Product toAdd = _productRepository.GetById(vm.ProductID);
            if (_cp.CanAddProductToConsolidatedProduct(toAdd,_cp))
                _cp.ProductDetails.Add(new ConsolidatedProduct.ProductDetail { Product = toAdd, QuantityPerConsolidatedProduct = vm.Quantity });
            _productRepository.Save(_cp);
        }

        public void RemoveItemFromConsolidatedProduct(Guid cProductID, Guid itemID)
        {
            //fetch consolidated product
            ConsolidatedProduct _cp = _productRepository.GetById(cProductID) as ConsolidatedProduct;
            //remove the product from the ProductDetails
            //Product toRemove = _productRepository.GetById(itemID);
            var pDetail = (from _dP in _cp.ProductDetails
                          where _dP.Product.Id == itemID
                          select _dP).First();
            _cp.ProductDetails.Remove(pDetail);
            //save the consolidated product.
            _productRepository.Save(_cp);
        }


        public List<ConsolidatedProduct.ProductDetail> AddProductDetails(Guid productID, int qty)
        {
            throw new NotImplementedException();
        }

        public ListProductViewModel SearchProductList(string srchParam,bool inactive = false)
        {
            ListProductViewModel vm = new ListProductViewModel
            {  Title = "List Products",
                Items = _productRepository.GetAll(inactive).ToList().Where(n => (n.ProductCode.ToLower().StartsWith(srchParam.ToLower()))||(n.Description.ToLower().StartsWith(srchParam.ToLower())) ||  (n.Brand.Name.ToLower().StartsWith(srchParam.ToLower()))
               ).ToList()
                .Select(n => new ListProductViewModel.ListProductViewModelItem
                {
                    Brand = n.Brand.Name,
                    //Flavour=((SaleProduct )n).Flavour==null?"":((SaleProduct )n).Flavour.Name,
                    Flavour = n.GetType() == typeof(SaleProduct) ? ((SaleProduct)n).Flavour == null ? "" : ((SaleProduct)n).Flavour.Name : "",
                    RetProdFalvour = n.GetType() == typeof(ReturnableProduct) ? ((ReturnableProduct)n).Flavour == null ? "" : ((ReturnableProduct)n).Flavour.Name : "",
                    Code = n.ProductCode,
                    Description = n.Description,
                    Packaging = n.Packaging == null ? "" : n.Packaging.Name,
                    VatClassId = n.VATClass == null ? Guid.Empty : n.VATClass.Id,
                    VatClass = n.VATClass == null ? "" : _vatClassRepository.GetById(n.VATClass.Id).VatClass,
                    ProductId = n.Id,


                    isActive = n._Status == EntityStatus.Active ? true : false,


                    ProductType = n.GetType().ToString().Split('.').Last()
                })
                .ToList()
            };
            return vm;
        }

        public IList<ListProductViewModel> Search(string srchParam, bool inactive = false)
        {
            //ListProductViewModel vm = new ListProductViewModel
            //{
                //Title = "List Products",
            var items = _productRepository.GetAll(inactive).ToList().Where(n => (n.ProductCode.ToLower().Contains(srchParam.ToLower())) || (n.Description.ToLower().Contains(srchParam.ToLower())) || (n.Brand.Name.ToLower().Contains(srchParam.ToLower()))).ToList();
                return items.Select(s => new ListProductViewModel
            {
                    Brand = s.Brand.Name,
                    Flavour = s.GetType() == typeof(SaleProduct) ? ((SaleProduct)s).Flavour == null ? "" : ((SaleProduct)s).Flavour.Name : "",
                    RetProdFalvour = s.GetType() == typeof(ReturnableProduct) ? ((ReturnableProduct)s).Flavour == null ? "" : ((ReturnableProduct)s).Flavour.Name : "",
                    Code = s.ProductCode,
                    Description = s.Description,
                    PackagingName = s.Packaging == null ? "" : s.Packaging.Name,
                    VatClassId = s.VATClass == null ? Guid.Empty : s.VATClass.Id,
                    VatClass = s.VATClass == null ? "" : _vatClassRepository.GetById(s.VATClass.Id).VatClass,
                    ProductId = s.Id,
                    isActive = s._Status == EntityStatus.Active ? true : false,
                    ProductTypeName = s.GetType().ToString().Split('.').Last(),
                    ProductType = s.GetType() == typeof(SaleProduct) ? ((SaleProduct)s).ProductType == null ? "" : ((SaleProduct)s).ProductType.Name : "",
                }).ToList();
        }
        public Dictionary<Guid, string> GetBrands()
        {
            return _productBrandRepository.GetAll().ToList().Select(n => new { n.Id, n.Name }).OrderBy(n=>n.Name).ToDictionary(n=>n.Id,n=>n.Name);
        }

        public Dictionary<Guid, string> GetFlavours()
        {
            return _productFlavourRepository.GetAll().ToList().Select(n => new { n.Id, n.Name }).OrderBy(n => n.Name).ToDictionary(n => n.Id, n => n.Name);
        }

        public Dictionary<Guid, string> GetPackaging()
        {
            return _productPackagingRepository.GetAll().ToList().Select(n => new { n.Id, n.Name }).OrderBy(n => n.Name).ToDictionary(n => n.Id, n => n.Name);
        }

        public Dictionary<Guid, string> GetPackagingType()
        {
            return _productPackagingTypeRepository.GetAll().ToList().Select(n => new { n.Id, n.Name }).OrderBy(n => n.Name).ToDictionary(n => n.Id, n => n.Name);
        }

        public Dictionary<Guid, string> GetProductType()
        {
            return _productTypeRepository.GetAll().ToList().Select(n => new { n.Id, n.Name }).OrderBy(n => n.Name).ToDictionary(n => n.Id, n => n.Name);
        }
        public void SaveSaleProduct(SaleProductViewModel vm)
        {
            if (vm.brandCode != null && vm.subBrandCode != null && vm.productTypeCode != null && vm.packTypeCode != null && vm.packCode != null && vm.vatClass != null)
            {
                ProductBrand pd = _productBrandRepository.GetAll().FirstOrDefault(n => n.Code == vm.brandCode);
                ProductFlavour pf = _productFlavourRepository.GetAll().FirstOrDefault(n=>n.Code==vm.subBrandCode);
                ProductType pt = _productTypeRepository.GetAll().FirstOrDefault(n=>n.Code==vm.productTypeCode);
                ProductPackaging ppg = _productPackagingRepository.GetAll().FirstOrDefault(n=>n.Code==vm.packCode);
                ProductPackagingType ppgT = _productPackagingTypeRepository.GetAll().FirstOrDefault(n=>n.Code==vm.packTypeCode);
                VATClass vc = _vatClassRepository.GetAll().FirstOrDefault(n=>n.VatClass==vm.vatClass);

                SaleProduct sp = _productRepository.GetById(vm.Id) as SaleProduct;
                if (sp == null)
                    sp = _productFactory.CreateSaleProduct(vm.Id);
                sp.Brand = _productBrandRepository.GetById(pd.Id);
                sp.Description = vm.Description;
                sp.ExFactoryPrice = vm.ExFactoryPrice;
                sp.Packaging = _productPackagingRepository.GetById(ppg.Id);
                sp.PackagingType = _productPackagingTypeRepository.GetById(ppgT.Id);
                sp.ProductCode = vm.ProductCode;
                sp.Flavour = _productFlavourRepository.GetById(pf.Id);
                sp.ProductType = _productTypeRepository.GetById(pt.Id);
                sp.VATClass = _vatClassRepository.GetById(vc.Id);

                _productRepository.Save(sp);
                if (vm.Id == Guid.Empty)
                {
                    addDefaultPrice();
                }
            }
            else
            {

                SaleProduct sp = sp = _productRepository.GetById(vm.Id) as SaleProduct; ;
                if (sp== null)
                    sp = _productFactory.CreateSaleProduct(vm.Id);


                sp.ExFactoryPrice = vm.ExFactoryPrice;
                sp.Brand = _productBrandRepository.GetById(vm.Brands);
                sp.Description = vm.Description;
                sp.Packaging = _productPackagingRepository.GetById(vm.Packaging);
                sp.PackagingType = _productPackagingTypeRepository.GetById(vm.PackageType);
                sp.ProductCode = vm.ProductCode;
                sp.Flavour = _productFlavourRepository.GetById(vm.FlavourID);
                sp.ProductType = _productTypeRepository.GetById(vm.ProductTypeID);
                sp.VATClass = _vatClassRepository.GetById(vm.VatClassId);
                if (vm.ReturnableProductId != null)
                    sp.ReturnableProduct = _productRepository.GetById(vm.ReturnableProductId.Value) as ReturnableProduct;

                _productRepository.Save(sp);
                if (vm.Id == Guid.Empty)
                {
                    addDefaultPrice();
                }
            }
        }

        public void CreateReturnableProduct(ReturnableProductViewModel retunableVM)
        {
            ReturnableProduct rp = _productRepository.GetById(retunableVM.Id) as ReturnableProduct;
            if (rp==null)
                rp = _productFactory.CreateReturnableProduct(retunableVM.Id);
            rp.Brand = _productBrandRepository.GetById(retunableVM.BrandId);
            rp.ExFactoryPrice = retunableVM.ExFactoryPrice;
            rp.Description = retunableVM.Description;
            rp.Packaging = _productPackagingRepository.GetById(retunableVM.PackagingId);
            rp.PackagingType = _productPackagingTypeRepository.GetById(retunableVM.PackagingTypeId);
            rp.ProductCode = retunableVM.ProductCode;
            rp.Flavour = _productFlavourRepository.GetById(retunableVM.FlavourID);
            // = _productTypeRepository.GetById(pt.Id);
            rp.VATClass = _vatClassRepository.GetById(retunableVM.VatClassId);
            rp.ReturnableType = (ReturnableType)retunableVM.ReturnableType;
            rp.Capacity = retunableVM.Capacity;
            if (retunableVM.ReturnableProductId != null)
                rp.ReturnAbleProduct = _productRepository.GetById(retunableVM.ReturnableProductId.Value) as ReturnableProduct;
            else rp.ReturnAbleProduct = null;
            _productRepository.Save(rp);
            if (retunableVM.Id == Guid.Empty)
            {
                addDefaultPriceForReturnable();
            }
        }


        Dictionary<int, string> IProductViewModelBuilder.GetReturnableType()
        {
            return EnumHelper.EnumToList<ReturnableType>()
                            .ToDictionary(n => (int)n, n => n.ToString());
        }
        public void addDefaultPrice()
        {
            //Get Latest Product
            SaleProduct spLatest = _productRepository.GetAll().OrderByDescending(n => n._DateCreated).First() as SaleProduct;
            //Get top most tier
            ProductPricingTier topTier = _productPricingTier.GetAll().FirstOrDefault();
            ProductPricing price = _productPricingFactory.CreateProductPricing(spLatest.Id, topTier.Id, 0, 0, DateTime.Now);
            _productPricingRepository.Save(price);
        }
        public void addDefaultPriceForReturnable()
        {
            //Get Latest Product
            ReturnableProduct spLatest = _productRepository.GetAll().OrderByDescending(n => n._DateCreated).First() as ReturnableProduct;
            //Get top most tier
            ProductPricingTier topTier = _productPricingTier.GetAll().FirstOrDefault();
            ProductPricing price = _productPricingFactory.CreateProductPricing(spLatest.Id, topTier.Id, 0, 0, DateTime.Now);
            _productPricingRepository.Save(price);
        }

        public Dictionary<Guid, string> VatClass()
        {
            return _vatClassRepository.GetAll().ToList().Select(n => new { n.Id, n.VatClass }).OrderBy(n => n.VatClass).ToDictionary(n => n.Id, n => n.VatClass);
        }


        public Dictionary<Guid, string> GetReturnableProducts()
        {
            return _productRepository.GetAll().ToList().OfType<ReturnableProduct>()
                .Select(n => new { n.Id, n.Description }).OrderBy(n => n.Description).ToDictionary(n => n.Id, n => n.Description);
        }


        public Dictionary<Guid, string> GetRetReturnableProducts()
        {
            return _productRepository.GetAll().ToList().OfType<ReturnableProduct>()
                .Where(n=>n.Capacity > 1)
               // .Where(n=>n.Id!=productId.Value)
                .OrderBy(n=>n.Description)
                .Select(n => new { n.Id, n.Description }).ToDictionary(n => n.Id, n => n.Description);
        }

       
    }
}
