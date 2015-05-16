using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Distributr.Core.Domain.Master;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Data.MappingExtensions;
using Distributr.Core.Utility;
using Distributr.Core.Data.Utility.Caching;
using Distributr.Core.Repository.Master;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.Data.Utility;
using Distributr.Core.Factory.Master;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository.MasterData.ProductRepositories
{
    internal class ProductRepository : RepositoryMasterBase<Product>, IProductRepository
    {
        private CokeDataContext _ctx;
        private ICacheProvider _cacheProvider;
        private IProductBrandRepository _productBrandRepository;
        private IProductFlavourRepository _productFlavourRepository;
        private IProductPackagingRepository _productPackagingRepository;
        private IProductPackagingTypeRepository _productPackagingTypeRepository;
        private IProductTypeRepository _productTypeRepository;
        private IVATClassRepository _vatClassRepository;
       
        public ProductRepository(CokeDataContext ctx, ICacheProvider cacheProvider,
                                 IProductBrandRepository productBrandRepository,
                                 IProductFlavourRepository productFlavourRepository,
                                 IProductPackagingRepository productPackagingRepository,
                                 IProductPackagingTypeRepository productPackagingTypeRepository,
                                 IProductTypeRepository productTypeRepository,
                              
                                 IVATClassRepository vatClassRepository
            
            )
        {
            _ctx = ctx;
            _cacheProvider = cacheProvider;
            _productBrandRepository = productBrandRepository;
            _productFlavourRepository = productFlavourRepository;
            _productPackagingTypeRepository = productPackagingTypeRepository;
            _productTypeRepository = productTypeRepository;
            _productPackagingRepository = productPackagingRepository;
           _vatClassRepository = vatClassRepository;
           
        }

        public Guid Save(Product entity, bool? isSync = null)
        {
            var vri = new ValidationResultInfo();
            if (isSync == null || !isSync.Value)
            {
                vri = Validate(entity);
            }
            if (!vri.IsValid)
            {
                LogErrors(entity, vri);
                throw new DomainValidationException(vri, "Product Entity Not valid");
            }
            tblProduct productToSave = _ctx.tblProduct.FirstOrDefault(n => n.id == entity.Id);
            DateTime dt = DateTime.Now;
            if (productToSave == null)
            {
                productToSave = new tblProduct
                                    {
                                        IM_DateCreated = dt,
                                        IM_Status = (int) EntityStatus.Active, // true,
                                        id = entity.Id,
                                    };
                if (entity is ConsolidatedProduct)
                {
                    ConsolidatedProduct cp = entity as ConsolidatedProduct;
                    productToSave.DomainTypeId = (int) ProductDomainType.ConsolidatedProduct;
                    foreach (var pd in cp.ProductDetails)
                    {
                        productToSave.tblConsolidatedProductProducts.Add(new tblConsolidatedProductProducts
                                                                             {
                                                                                 ProductId = pd.Product.Id,
                                                                                 QtyPerConsolidatedProduct =
                                                                                     pd.QuantityPerConsolidatedProduct
                                                                             });
                    }
                }
                if (entity is SaleProduct)
                    productToSave.DomainTypeId = (int) ProductDomainType.SaleProduct;
                if (entity is ReturnableProduct)
                    productToSave.DomainTypeId = (int) ProductDomainType.ReturnableProduct;

                _ctx.tblProduct.AddObject(productToSave);
            }

            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (productToSave.IM_Status != (int) entityStatus)
                productToSave.IM_Status = (int) entity._Status;

            productToSave.IM_DateLastUpdated = dt;
            productToSave.Description = entity.Description;
            productToSave.BrandId = entity.Brand.Id;
            productToSave.ExFactoryPrice = entity.ExFactoryPrice;

            // productToSave.FlavourId = product.Flavours.Id;
            if (entity.Flavour != null)
            {
                productToSave.FlavourId = entity.Flavour.Id;
            }
            if (entity.Packaging != null)
                productToSave.PackagingId = entity.Packaging.Id;

            productToSave.PackagingTypeId = entity.PackagingType.Id;
            productToSave.ProductCode = entity.ProductCode;
            productToSave.ReturnableType = (int) entity.ReturnableType;
            //productToSave.ProductTypeId = product.ProductTypes.Id;

            if (entity is SaleProduct)
            {
                SaleProduct sp = entity as SaleProduct;
                productToSave.ProductTypeId = sp.ProductType.Id;
                if (sp.Flavour != null)
                {
                    productToSave.FlavourId = sp.Flavour.Id;
                }
                productToSave.VatClassId = sp.VATClass != null ? sp.VATClass.Id : Guid.Empty;
                if (sp.ReturnableProduct != null)
                {
                    productToSave.Returnable = sp.ReturnableProduct.Id;
                }
            }

            if (entity is ReturnableProduct)
            {
                ReturnableProduct rp = entity as ReturnableProduct;
                if (rp.Flavour != null)
                {
                    productToSave.FlavourId = rp.Flavour.Id;
                }
                if (rp.ReturnAbleProduct != null)
                {
                    productToSave.Returnable = rp.ReturnAbleProduct.Id;
                }
                else productToSave.Returnable = null;
                productToSave.Capacity = rp.Capacity;
                productToSave.VatClassId = rp.VATClass.Id;
                if (rp.ReturnAbleProduct != null)
                {
                    productToSave.Returnable = rp.ReturnAbleProduct.Id;
                }
            }
            // bool Doesexists =_ctx.tblProduct.Any(n => n.id == product.Id);
            if (entity is ConsolidatedProduct)
            {
                //products
                ConsolidatedProduct cp = entity as ConsolidatedProduct;
                productToSave.DomainTypeId = (int) ProductDomainType.ConsolidatedProduct;

                foreach (var pd in cp.ProductDetails)
                {
                    bool exists = productToSave.tblConsolidatedProductProducts.Any(n => n.ProductId == pd.Product.Id);
                    //check if exists
                    if (exists)
                    {
                        productToSave.tblConsolidatedProductProducts.First(n => n.ProductId == pd.Product.Id).
                            QtyPerConsolidatedProduct = pd.QuantityPerConsolidatedProduct;
                    }
                    else
                    {
                        //add
                        productToSave.tblConsolidatedProductProducts.Add(new tblConsolidatedProductProducts
                                                                             {
                                                                                 ProductId = pd.Product.Id,
                                                                                 QtyPerConsolidatedProduct =
                                                                                     pd.QuantityPerConsolidatedProduct
                                                                             });
                    }
                }
            }
            //int productId = product.Id;
            // ProductPricing price = _productPricingFactory.CreateProductPricing(productId, 1, 0, 0, dt);
            //int id = _pricingRepository.Save(price);
            //return id;
            _ctx.SaveChanges();
            _cacheProvider.Put(_cacheListKey,
                               _ctx.tblProduct.Where(n => n.IM_Status != (int) EntityStatus.Deleted).Select(s => s.id).
                                   ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, productToSave.id));
            return productToSave.id;
        }



        public void SetInactive(Product entity)
        {
            ValidationResultInfo vri = Validate(entity);
            bool hasConsolidated = _ctx.tblConsolidatedProductProducts.Any(s => s.ProductId == entity.Id);
            bool hasProductPricing =
                _ctx.tblPricing.Any(n => n.ProductRef == entity.Id && n.IM_Status == (int) EntityStatus.Active);
            if (hasConsolidated)
            {
                throw new DomainValidationException(vri,
                                                    "Cannot deactivate this product.\nHas consolidated product under it");
            }
            if (hasProductPricing)
            {
                throw new DomainValidationException(vri,
                                                    "This Product has price attached to it\n Please deactivate price first from pricings");
            }
            if (entity is ReturnableProduct)
            {
                var hasProductAttachment = _ctx.tblProduct
                    .Any(n => n.IM_Status != (int) EntityStatus.Deleted && n.Returnable.Value == entity.Id);
                if (hasProductAttachment)
                    throw new DomainValidationException(vri,
                                                        "This returnable product has a consolidated or sale product attached to it");
            }
            tblProduct product = _ctx.tblProduct.FirstOrDefault(p => p.id == entity.Id);
            if (product != null)
            {
                product.IM_Status = (int) EntityStatus.Inactive; // false;
                product.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey,
                                   _ctx.tblProduct.Where(n => n.IM_Status != (int) EntityStatus.Deleted).Select(
                                       s => s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, product.id));
            }
        }

        public void SetActive(Product entity)
        {
            tblProduct product = _ctx.tblProduct.FirstOrDefault(p => p.id == entity.Id);
            if (product != null)
            {
                product.IM_Status = (int) EntityStatus.Active;
                product.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey,
                                   _ctx.tblProduct.Where(n => n.IM_Status != (int) EntityStatus.Deleted).Select(
                                       s => s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, product.id));
            }
        }

        public void SetAsDeleted(Product entity)
        {
            ValidationResultInfo vri = Validate(entity);

            bool hasDiscountDependency = _ctx.tblDiscounts
                .Where(n => n.IM_Status != (int) EntityStatus.Deleted)
                .Any(n => n.ProductRef == entity.Id);
            if (hasDiscountDependency)
                vri.Results.Add(new ValidationResult("Cannot delete this product.\nHas discount attached to it"));

            bool hasPromotioniscountDependency = _ctx.tblPromotionDiscount
                .Where(n => n.IM_Status != (int) EntityStatus.Deleted)
                .Any(n => n.ProductRef == entity.Id);
            if (hasPromotioniscountDependency)
                vri.Results.Add(
                    new ValidationResult("Cannot delete this product.\nHas promotion discount attached to it"));

            bool hasCvcpDiscountDependency = _ctx.tblCertainValueCertainProductDiscountItem
                .Where(n => n.IM_Status != (int) EntityStatus.Deleted)
                .Any(n => n.Product == entity.Id);
            if (hasCvcpDiscountDependency)
                vri.Results.Add(
                    new ValidationResult(
                        "Cannot delete this product.\nHas certain value certain product discount attached to it"));


            bool hasFreeOfChargeDiscountDependency = _ctx.tblFreeOfChargeDiscount
                .Where(n => n.IM_Status != (int) EntityStatus.Deleted)
                .Any(n => n.ProductRef == entity.Id);
            if (hasFreeOfChargeDiscountDependency)
                vri.Results.Add(
                    new ValidationResult("Cannot delete this product.\nHas free of charge discount attached to it"));

            bool hasConsolidated = _ctx.tblConsolidatedProductProducts.Any(s => s.ProductId == entity.Id);
            if (hasConsolidated)
            {
                throw new DomainValidationException(vri,
                                                    "Cannot delete this product.\nHas consolidated product under it");
            }

            //bool hasProductPricing =
            //    _ctx.tblPricing.Any(n => n.ProductRef == entity.Id && n.IM_Status == (int) EntityStatus.Active);
            //if (hasProductPricing)
            //{
            //    throw new DomainValidationException(vri,
            //                                        "This Product has price attached to it\n Please delete price first from pricings");
            //}

            if (!vri.IsValid)
                throw new DomainValidationException(vri, "Product Entity Not valid");

            tblProduct product = _ctx.tblProduct.FirstOrDefault(p => p.id == entity.Id);
            if (product != null)
            {
                product.IM_Status = (int) EntityStatus.Deleted; // false;
                product.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey,
                                   _ctx.tblProduct.Where(n => n.IM_Status != (int) EntityStatus.Deleted).Select(
                                       s => s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, product.id));
            }
        }

        public Product GetById(Guid Id, bool includeDeactivated = false)
        {
            Product entity = (Product) _cacheProvider.Get(string.Format(_cacheKey, Id));
            if (entity == null)
            {
                var tbl = _ctx.tblProduct.FirstOrDefault(s => s.id == Id);
                if (tbl != null)
                {
                    entity = Map(tbl, null);
                    _cacheProvider.Put(string.Format(_cacheKey, entity.Id), entity);
                }

            }
            return entity;

        }


        protected override string _cacheKey
        {
            get { return "Product-{0}"; }
        }

        protected override string _cacheListKey
        {
            get { return "ProductList"; }
        }

        public override IEnumerable<Product> GetAll(bool includeDeactivated = false)
        {
            int[] nonConsolidatedProducts = (new[] {ProductDomainType.SaleProduct, ProductDomainType.ReturnableProduct})
                .Select(n => (int) n).ToArray();
            IList<Product> entities = null;
            IList<Guid> ids = (IList<Guid>) _cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<Product>(ids.Count);
                foreach (Guid id in ids)
                {
                    Product entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities =
                    _ctx.tblProduct.Where(n => n.IM_Status != (int) EntityStatus.Deleted).ToList().Select(
                        s => Map(s, null)).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList(); //new List<int>(persons.Count);
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (Product p in entities)
                    {
                        _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                    }
                }
            }

            if (!includeDeactivated)
                entities = entities.Where(n => n._Status != EntityStatus.Inactive).ToList();
            return entities;
        }

        private Product Map(tblProduct product, List<tblProduct> consolidatedProducts)
        {
            //assuming all dependant product properties are cached
            Product p = null;
            ProductDomainType pdt = (ProductDomainType) product.DomainTypeId;
            switch (pdt)
            {
                case ProductDomainType.ConsolidatedProduct:
                    p = new ConsolidatedProduct(product.id);
                    break;
                case ProductDomainType.ReturnableProduct:
                    p = new ReturnableProduct(product.id);
                    break;
                case ProductDomainType.SaleProduct:
                    p = new SaleProduct(product.id);
                    break;
            }

            p.Brand = _productBrandRepository.GetById(product.BrandId.Value);
            // p.Flavours = _productFlavourRepository.GetById(product.FlavourId.Value);
            p.Description = product.Description;
            p.ExFactoryPrice = product.ExFactoryPrice;
            if (product.PackagingId != null)
            {
                p.Packaging = _productPackagingRepository.GetById(product.PackagingId.Value);
            }

            p.PackagingType = _productPackagingTypeRepository.GetById(product.PackagingTypeId.Value);
            p.ProductCode = product.ProductCode;
            if (product.ReturnableType != null)
            {
                p.ReturnableType = (ReturnableType) product.ReturnableType;
            }

            if (product.FlavourId != null)
            {
                p.Flavour = _productFlavourRepository.GetById(product.FlavourId.Value);
            }
            p._SetStatus((EntityStatus) product.IM_Status);
            p._SetDateCreated(product.IM_DateCreated);
            p._SetDateLastUpdated(product.IM_DateLastUpdated);

            if (p is SaleProduct || p is ReturnableProduct)
            {
                var qry = _ctx.tblPricing as IQueryable<tblPricing>;

                List<ProductPricing> plist = (List<ProductPricing>) qry
                                                                        .Where(n => n.ProductRef == p.Id)
                                                                        .ToList()
                                                                        .Select(
                                                                            n => MapToDomain.Map(n) as ProductPricing)
                                                                        .ToList();
                p.ProductPricings = plist;
            }

            if (p is ConsolidatedProduct)
            {
                ConsolidatedProduct cp = p as ConsolidatedProduct;
                
                return cp;
            }

            if (p is ReturnableProduct)
            {
                ReturnableProduct rp = p as ReturnableProduct;
                rp.Flavour = product.FlavourId == null
                                 ? null
                                 : _productFlavourRepository.GetById(product.FlavourId.Value);
                rp.Capacity = product.Capacity;
                rp.VATClass = product.VatClassId == null ? null : _vatClassRepository.GetById(product.VatClassId.Value);
                if (product.Returnable != null)
                {
                    rp.ReturnAbleProduct = GetMyReturnable(product.Returnable.Value);
                }
            }

            if (p is SaleProduct)
            {
                SaleProduct sp = p as SaleProduct;
                sp.ProductType = _productTypeRepository.GetById(product.ProductTypeId.Value);
                sp.Flavour = product.FlavourId == null
                                 ? null
                                 : _productFlavourRepository.GetById(product.FlavourId.Value);
                sp.VATClass = product.VatClassId == null ? null : _vatClassRepository.GetById(product.VatClassId.Value);
                if (product.Returnable != null)
                {

                    sp.ReturnableProduct = GetMyReturnable(product.Returnable.Value);

                }
                return sp;
            }

            return p;
        }

        private ReturnableProduct GetMyReturnable(Guid id)
        {
            ReturnableProduct rp = null;
            var tblRp = _ctx.tblProduct.FirstOrDefault(n => n.id == id);
            if (tblRp != null)
            {
                rp = new ReturnableProduct(tblRp.id)
                         {
                             Brand = _productBrandRepository.GetById(tblRp.BrandId.Value),
                             Capacity = tblRp.Capacity,
                             Description = tblRp.Description,
                             ExFactoryPrice = tblRp.ExFactoryPrice,
                             VATClass =
                                 tblRp.VatClassId == null ? null : _vatClassRepository.GetById(tblRp.VatClassId.Value),
                             Flavour =
                                 tblRp.FlavourId == null
                                     ? null
                                     : _productFlavourRepository.GetById(tblRp.FlavourId.Value),
                             Packaging =
                                 tblRp.PackagingId == null
                                     ? null
                                     : _productPackagingRepository.GetById(tblRp.PackagingId.Value),
                             PackagingType = _productPackagingTypeRepository.GetById(tblRp.PackagingTypeId.Value),
                             ProductCode = tblRp.ProductCode
                         };
                var qry = _ctx.tblPricing as IQueryable<tblPricing>;

                List<ProductPricing> plist = (List<ProductPricing>) qry
                                                                        .Where(n => n.ProductRef == rp.Id)
                                                                        .ToList()
                                                                        .Select(
                                                                            n => MapToDomain.Map(n) as ProductPricing)
                                                                        .ToList();
                rp.ProductPricings = plist;
            }
            if (tblRp.Returnable != null)
            {
                var tblRpyake = _ctx.tblProduct.FirstOrDefault(n => n.id == tblRp.Returnable);
                if (tblRpyake != null)
                {
                    var brand = _productBrandRepository.GetById(tblRpyake.BrandId.Value);
                    var capacity = tblRpyake.Capacity;
                    var vatClass = tblRpyake.VatClassId == null
                                       ? null
                                       : _vatClassRepository.GetById(tblRpyake.VatClassId.Value);
                    var description = tblRpyake.Description;
                    var flavour = _productFlavourRepository.GetById(tblRpyake.FlavourId.Value);
                    var packaging = tblRpyake.PackagingId == null
                                        ? null
                                        : _productPackagingRepository.GetById(tblRpyake.PackagingId.Value);
                    var packagingType = _productPackagingTypeRepository.GetById(tblRpyake.PackagingTypeId.Value);
                    var productCode = tblRpyake.ProductCode;
                    var exFactoryPrice = tblRpyake.ExFactoryPrice;
                    rp.ReturnAbleProduct = new ReturnableProduct(tblRpyake.id)
                                               {
                                                   Brand = brand,
                                                   Capacity = capacity,
                                                   VATClass = vatClass,
                                                   Description = description,
                                                   Flavour = flavour,
                                                   Packaging = packaging,
                                                   PackagingType = packagingType,
                                                   ProductCode = productCode,
                                                   ExFactoryPrice = exFactoryPrice
                                               };
                    var qry = _ctx.tblPricing as IQueryable<tblPricing>;

                    List<ProductPricing> plist = (List<ProductPricing>) qry
                                                                            .Where(
                                                                                n =>
                                                                                n.ProductRef == rp.ReturnAbleProduct.Id)
                                                                            .ToList()
                                                                            .Select(
                                                                                n =>
                                                                                MapToDomain.Map(n) as ProductPricing)
                                                                            .ToList();
                    rp.ReturnAbleProduct.ProductPricings = plist;
                }
            }
            return rp;
        }


        public ValidationResultInfo Validate(Product itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));


            if (itemToValidate is SaleProduct)
            {
                bool hasDuplicateCode = _ctx.tblProduct
                    .Any(s => s.id != itemToValidate.Id && s.ProductCode == itemToValidate.ProductCode);
                   
                if (hasDuplicateCode)
                    vri.Results.Add(
                        new ValidationResult(CoreResourceHelper.GetText("hq.product.validation.dupcode") + " (" +
                                             itemToValidate.ProductCode + ")"));
            }
            if (itemToValidate is ReturnableProduct)
            {
                bool hasDuplicateCode =
                _ctx.tblProduct
                    .Any(s => s.id != itemToValidate.Id && s.ProductCode == itemToValidate.ProductCode);
                if (hasDuplicateCode)
                    vri.Results.Add(
                        new ValidationResult(CoreResourceHelper.GetText("hq.product.validation.dupcode") + " (" +
                                             itemToValidate.ProductCode + ")"));
            }
            if (itemToValidate is ConsolidatedProduct)
            {
                //must be at least one child product
                ConsolidatedProduct cp = itemToValidate as ConsolidatedProduct;

                if (cp.ProductDetails.Any(n => n.QuantityPerConsolidatedProduct == 0))
                    vri.Results.Add(
                        new ValidationResult("All products in a consolidated product must have a quantity of at least 1"));

                bool dupeTest = (from n in cp.ProductDetails.Select(n => n.Product.Id)
                                 group n by n
                                 into n1
                                 where n1.Count() > 1
                                 select (n1.Key)).Any();
                if (dupeTest)
                    vri.Results.Add(new ValidationResult("Cannot have a consolidated product with duplicate products"));

            }
            return vri;
            //return itemToValidate.BasicValidation();
        }

        public void AddProductToConsolidatedProduct(Product consolidatedProduct, Product productToAdd,
                                                    int productToAddQuantity)
        {
            if (consolidatedProduct == null)
                throw new ArgumentNullException("Consolidated product cannot be null");

            if (productToAdd == null)
                throw new ArgumentNullException("Product to be added to consolidated product cannot be null");

            if (productToAddQuantity <= 0)
                throw new ArgumentException(
                    "Product to be added to consolidated product must have a quantity of at least one");

            tblProduct p = _ctx.tblProduct.FirstOrDefault(n => n.id == consolidatedProduct.Id);
            if (p == null)
                return;
            if (p.tblConsolidatedProductProducts.Any(n => n.ProductId == productToAdd.Id))
                return;


            p.tblConsolidatedProductProducts.Add(new tblConsolidatedProductProducts
                                                     {
                                                         ProductId = productToAdd.Id,
                                                         QtyPerConsolidatedProduct = productToAddQuantity
                                                     });

            _ctx.SaveChanges();


        }

        public void RemoveProductFromConsolidatedProduct(Product consolidatedProduct, Product productToRemove)
        {
            if (consolidatedProduct == null)
                throw new ArgumentNullException("Consolidated product cannot be null");

            if (productToRemove == null)
                throw new ArgumentNullException("Product to be added to consolidated product cannot be null");

            tblProduct p = _ctx.tblProduct.FirstOrDefault(n => n.id == consolidatedProduct.Id);

            if (p == null)
                return;

            if (!p.tblConsolidatedProductProducts.Any(n => n.ProductId == productToRemove.Id))
                return;

            tblConsolidatedProductProducts ppr =
                p.tblConsolidatedProductProducts.First(n => n.ProductId == productToRemove.Id);
            p.tblConsolidatedProductProducts.Remove(ppr);

            _ctx.SaveChanges();
        }

        public Product GetReturnableProduct(Guid returnableId)
        {
            var rtProduct = _ctx.tblProduct.FirstOrDefault(n => n.Returnable == returnableId);
            if (rtProduct != null)
                return Map(rtProduct, null);
            return null;
        }

        public Product GetByCode(string code, bool showInActive = false)
        {
            if (string.IsNullOrEmpty(code)) return null;
                code = code.Trim().ToLower();
            var tbl = showInActive
                          ? _ctx.tblProduct.FirstOrDefault(
                              s =>
                              s.IM_Status != (int) EntityStatus.Deleted && s.ProductCode != null &&
                              s.ProductCode.ToLower() == code)
                          : _ctx.tblProduct.FirstOrDefault(
                              s =>
                              s.ProductCode != null && s.ProductCode.ToLower() == code &&
                              s.IM_Status == (int) EntityStatus.Active);
            if (tbl != null)
            {
                return Map(tbl,null);
            }
            return null;
        }

        public IEnumerable<Product> GetAll(string searchText, int pageIndex, int pageSize, out int count, bool includeDeactivated = false)
        {
            var entities = _ctx.tblProduct.Where(n => n.IM_Status != (int) EntityStatus.Deleted);
            if (!includeDeactivated)
                entities = entities.Where(n => n.IM_Status != (int) EntityStatus.Inactive).OrderBy(n => n.Description);
            count = entities.Count();
            var items = entities.Skip(pageIndex * pageSize).Take(pageSize).ToList().Select(s => Map(s, null)).ToArray();
            return items;
        }

        public IEnumerable<Product> Filter(Expression<Func<object, bool>> tCriteria,int? take=null)
        {
            var entities = _ctx.tblProduct.Where(n => n.IM_Status != (int)EntityStatus.Deleted)
                .Where(tCriteria.Compile()).Cast<tblProduct>();
            if(take.HasValue)
                return entities.Take(take.Value).Select(n => Map(n, null)).ToList();
            return entities.Select(n => Map(n, null)).ToList();
        }

       
        public QueryResult<Product> Query(QueryStandard q)
        {
            IQueryable<tblProduct> productQuery;
            if (q.ShowInactive)
                productQuery = _ctx.tblProduct.Where(p => p.IM_Status != (int)EntityStatus.Deleted).AsQueryable();
            else
                productQuery = _ctx.tblProduct.Where(s => s.IM_Status == (int)EntityStatus.Active).AsQueryable();

            var queryResult = new QueryResult<Product>();
            if (q.SupplierId.HasValue)
            {
                productQuery = productQuery
                    .Where(s => s.tblProductBrand.SupplierId==q.SupplierId.Value );
            }
            if (!string.IsNullOrWhiteSpace(q.Name))
            {

                productQuery = productQuery
                    .Where(s => s.Description.ToLower().Contains(q.Name.ToLower()) || s.ProductCode.ToLower().Contains(q.Name.ToLower()));
            }

            queryResult.Count = productQuery.Count();
            productQuery = productQuery.OrderBy(s => s.BrandId).ThenBy(s => s.ProductCode);
            if (q.Skip.HasValue && q.Take.HasValue)
                productQuery = productQuery.Skip(q.Skip.Value).Take(q.Take.Value);
            var result = productQuery.ToList();
            queryResult.Data = result.Select(s => Map(s, null)).OfType<Product>().ToList();
            q.ShowInactive = false;
            return queryResult;
        }

        public Product GetTypeOfProduct(Product product)
        {
            var type = product.GetType();
            if (type == typeof(SaleProduct))
            {
                return product as SaleProduct;
            }
            if (type == typeof(ReturnableProduct))
            {
                return product as ReturnableProduct;
            }
            if (type == typeof(ConsolidatedProduct))
            {
                return product as ConsolidatedProduct;
            }
            
                return product;
        }

     
    }
}
