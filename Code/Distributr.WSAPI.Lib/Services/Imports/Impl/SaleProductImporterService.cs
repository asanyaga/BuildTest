using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Utility.Validation;
using Distributr.Import.Entities;
using log4net;
using ProductPackagingType = Distributr.Core.Domain.Master.ProductEntities.ProductPackagingType;

namespace Distributr.WSAPI.Lib.Services.Imports.Impl
{
    public class SaleProductImporterService : BaseImporterService, ISaleProductImporterService
    {
        private IProductRepository _saleProductRepository;
        private IVATClassRepository _vatClassRepository;
        private IProductBrandRepository _productBrandRepository;
        private IProductFlavourRepository _productFlavourRepository;
        private IProductPackagingRepository _productPackagingRepository;
        private IProductTypeRepository _productTypeRepository;
        private IProductPackagingTypeRepository _productPackagingTypeRepository;
        private readonly CokeDataContext _context;

        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public SaleProductImporterService(IProductRepository saleProductRepository, CokeDataContext context,
                                          IVATClassRepository vatClassRepository,
                                          IProductBrandRepository productBrandRepository,
                                          IProductTypeRepository productTypeRepository, IProductPackagingTypeRepository productPackagingTypeRepository, IProductFlavourRepository productFlavourRepository, IProductPackagingRepository productPackagingRepository)
        {
            _saleProductRepository = saleProductRepository;
            _context = context;
            _vatClassRepository = vatClassRepository;
            _productBrandRepository = productBrandRepository;
            _productTypeRepository = productTypeRepository;
            _productPackagingTypeRepository = productPackagingTypeRepository;
            _productFlavourRepository = productFlavourRepository;
            _productPackagingRepository = productPackagingRepository;
        }

        public ImportResponse Save(List<SaleProductImport> imports)
        {
            List<SaleProduct> saleProducts = imports.Select(Map).ToList();

            List<ValidationResultInfo> validationResults = saleProducts.Select(Validate).ToList();

            if (validationResults.Any(p => !p.IsValid))
            {
                return new ImportResponse() {Status = false, Info = ValidationResultsInfo(validationResults)};

            }
            List<SaleProduct> changedSaleProducts = HasChanged(saleProducts);

            foreach (var changedSaleProduct in changedSaleProducts)
            {
                _saleProductRepository.Save(changedSaleProduct);
            }
            return new ImportResponse()
                       {Status = true, Info = changedSaleProducts.Count + " Sale Products Successfully Imported"};
        }

        public ImportResponse Delete(List<string> deletedCodes)
        {
            foreach (var deletedCode in deletedCodes)
            {
                try
                {
                    
                    var saleProductId = _context.tblProduct.Where(p => p.ProductCode == deletedCode).Select(p => p.id).FirstOrDefault();

                    var saleProduct = _saleProductRepository.GetById(saleProductId);
                    if (saleProduct != null)
                    {
                        _saleProductRepository.SetAsDeleted(saleProduct);
                    }
                }
                catch (Exception ex)
                {
                    _log.Error("SaleProduct Error" + ex.ToString());
                }
            }
            return new ImportResponse() { Info = "SaleProduct Deleted Succesfully", Status = true };
        }

        private List<SaleProduct> HasChanged(List<SaleProduct> saleProducts)
        {
            var changedSaleProducts = new List<SaleProduct>();
            foreach (var saleProduct in saleProducts)
            {
                var entity = _saleProductRepository.GetById(saleProduct.Id) as SaleProduct;
                if (entity == null)
                {
                    changedSaleProducts.Add(saleProduct);
                    continue;
                }

                var currentVATClassId = entity.VATClass!=null ? entity.VATClass.Id.ToString():"";
                var previousVATClassId = saleProduct.VATClass != null ? saleProduct.VATClass.Id.ToString() : "";

                var currentBrandId = entity.Brand != null ? entity.Brand.Id.ToString() : "";
                var previousBrandId = saleProduct.Brand != null ? saleProduct.Brand.Id.ToString() : "";

                var previousFlavourId = entity.Flavour != null ? entity.Flavour.Id.ToString() : "";
                var currentFlavourId = saleProduct.Flavour != null ? saleProduct.Flavour.Id.ToString() : "";

                var previousPackagingId = entity.Packaging != null ? entity.Packaging.Id.ToString() : "";
                var currentPackagingId = saleProduct.Packaging != null ? saleProduct.Packaging.Id.ToString() : "";

                 //entity.ProductDiscounts.FirstOrDefault().Id != saleProduct.ProductDiscounts.FirstOrDefault().Id ||
                 //entity.ProductPricings.FirstOrDefault().Id != saleProduct.ProductPricings.FirstOrDefault().Id ||

                bool hasChanged = entity.Description.ToLower() != saleProduct.Description.ToLower() ||
                                  entity.ProductCode.ToLower() != saleProduct.ProductCode.ToLower() ||
                                  currentVATClassId != previousVATClassId ||
                                  currentBrandId != previousBrandId ||
                                  currentFlavourId!=previousFlavourId||
                                  previousPackagingId!=currentPackagingId||
                                  entity.ExFactoryPrice != saleProduct.ExFactoryPrice;

                if (hasChanged)
                {
                    changedSaleProducts.Add(saleProduct);
                }
            }
            return changedSaleProducts;
        }

        protected ValidationResultInfo Validate(SaleProduct saleProduct)
        {
            return _saleProductRepository.Validate(saleProduct);
        }

        protected SaleProduct Map(SaleProductImport saleProductImport)
        {
            var exists = _context.tblProduct.FirstOrDefault(p => p.ProductCode == saleProductImport.Code);
            Guid id = exists != null ? exists.id : Guid.NewGuid();

            var vatClassExists = _context.tblVATClass.FirstOrDefault(p => p.Name == saleProductImport.VATClass);

            VATClass vatClass = null;
            if (vatClassExists != null)
                vatClass = _vatClassRepository.GetById(vatClassExists.id);

            var productBrandExists =
                _context.tblProductBrand.FirstOrDefault(p => p.code == saleProductImport.ProductBrandCode);

            ProductBrand productBrand = null;
            if (productBrandExists != null)
                productBrand = _productBrandRepository.GetById(productBrandExists.id);

            var productFlavourExists =
              _context.tblProductFlavour.FirstOrDefault(p => p.code == saleProductImport.ProductFlavour);

            ProductFlavour productFlavour = null;
            if (productFlavourExists != null)
                productFlavour = _productFlavourRepository.GetById(productFlavourExists.id);

             var productPackagingExists =
              _context.tblProductPackaging.FirstOrDefault(p => p.code == saleProductImport.ProductPackaging);

            ProductPackaging productPackaging = null;
            if (productPackagingExists != null)
                productPackaging = _productPackagingRepository.GetById(productPackagingExists.Id);

            

            var saleProduct = new SaleProduct(id);
            saleProduct.Description = saleProductImport.Name;
            saleProduct.ProductCode = saleProductImport.Code;
            saleProduct.ExFactoryPrice = saleProductImport.ExFactoryPrice;
            saleProduct.VATClass = vatClass;
            saleProduct.Brand = productBrand;
            saleProduct.Flavour = productFlavour;
            saleProduct.ProductType = GetProductType(saleProductImport.ProductType);
            saleProduct.PackagingType = GetProductPackagingType(saleProductImport.ProductPackagingType);
            saleProduct.Packaging = productPackaging;

            return saleProduct;


        }

        private ProductType GetProductType(string productTypeName)
        {
            ProductType productType = null;
            if (!string.IsNullOrEmpty(productTypeName))
            {
                productType =
                    _productTypeRepository.GetAll().FirstOrDefault(
                        p =>
                        p.Name.ToLower() == productTypeName.ToLower() ||
                        p.Code != null &&
                        p.Code.ToLower() == productTypeName.ToLower());
            }

            if (productType == null)
            {
                productType = _productTypeRepository.GetAll().FirstOrDefault(p => p.Name.ToLower() == "default");
                if (productType == null)
                {
                    productType = new ProductType(Guid.NewGuid())
                                      {

                                          Name = string.IsNullOrEmpty(productTypeName) ? "default" : productTypeName,
                                          Description =
                                              string.IsNullOrEmpty(productTypeName) ? "default" : productTypeName,
                                          Code = string.IsNullOrEmpty(productTypeName) ? "default" : productTypeName,

                                      };
                    _productTypeRepository.Save(productType);

                }

            }

            return productType;
        }



        private ProductPackagingType GetProductPackagingType(string productTypeName)
        {
            
                ProductPackagingType productPackagingType = null;
                if (!string.IsNullOrEmpty(productTypeName))
                {
                    productPackagingType =
                        _productPackagingTypeRepository.GetAll().FirstOrDefault(
                            p =>
                            p.Name.ToLower() == productTypeName.ToLower() ||
                            p.Code != null &&
                            p.Code.ToLower() == productTypeName.ToLower());
                }

                if (productPackagingType == null)
                {
                    productPackagingType = _productPackagingTypeRepository.GetAll().FirstOrDefault(p => p.Name.ToLower() == "default");
                    if (productPackagingType == null)
                    {
                        productPackagingType = new ProductPackagingType(Guid.NewGuid())
                        {

                            Name = string.IsNullOrEmpty(productTypeName) ? "default" : productTypeName,
                            Description =
                                string.IsNullOrEmpty(productTypeName) ? "default" : productTypeName,
                            Code = string.IsNullOrEmpty(productTypeName) ? "default" : productTypeName,

                        };
                        _productPackagingTypeRepository.Save(productPackagingType);

                    }

                }

                return productPackagingType;
            }


        }
    }


