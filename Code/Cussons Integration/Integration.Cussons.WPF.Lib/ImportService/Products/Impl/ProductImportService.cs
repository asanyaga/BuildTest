using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Master.SuppliersEntities;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Master.SuppliersRepositories;
using Integration.Cussons.WPF.Lib.MasterDataImportService;
using Integration.Cussons.WPF.Lib.Utils;
using LINQtoCSV;

namespace Integration.Cussons.WPF.Lib.ImportService.Products.Impl
{
   internal class ProductImportService:IProductImportService
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductBrandRepository _productBrandRepository;
        private readonly IProductFlavourRepository _flavourRepository;
        private readonly IProductPackagingRepository _productPackagingRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IProductPackagingTypeRepository _productPackagingTypeRepository;
        private readonly IProductTypeRepository _productTypeRepository;
        private readonly IVATClassRepository _vatClassRepository;
        private readonly IProductPricingRepository _productPricingRepository;
        private readonly IProductPricingTierRepository _productPricingTierRepository;
        private List<string> _failedImpoprts;

       public ProductImportService(IProductRepository productRepository, IProductBrandRepository productBrandRepository, IProductFlavourRepository flavourRepository, IProductPackagingRepository productPackagingRepository, ISupplierRepository supplierRepository, IProductPackagingTypeRepository productPackagingTypeRepository, IProductTypeRepository productTypeRepository, IVATClassRepository vatClassRepository, IProductPricingRepository productPricingRepository, IProductPricingTierRepository productPricingTierRepository)
       {
           _productRepository = productRepository;
           _productBrandRepository = productBrandRepository;
           _flavourRepository = flavourRepository;
           _productPackagingRepository = productPackagingRepository;
           _supplierRepository = supplierRepository;
           _productPackagingTypeRepository = productPackagingTypeRepository;
           _productTypeRepository = productTypeRepository;
           _vatClassRepository = vatClassRepository;
           _productPricingRepository = productPricingRepository;
           _productPricingTierRepository = productPricingTierRepository;
           _failedImpoprts=new List<string>();
       }

       public async Task<IEnumerable<ProductImport>> Import(string path)
       {
           return await Task.Factory.StartNew(() =>
           {
               IEnumerable<ProductImport> ProductImports;
               try
               {

                   var inputFileDescription = new CsvFileDescription
                   {
                       // cool - I can specify my own separator!
                       SeparatorChar = '\t',//tab delimited
                       FirstLineHasColumnNames =
                           false,
                       QuoteAllFields = true,
                       EnforceCsvColumnAttribute =
                           true
                   };

                   CsvContext cc = new CsvContext();
                 
                   ProductImports = cc.Read<ProductImport>(path, inputFileDescription);
                  


               }
               catch (FileNotFoundException ex)
               {
                   MessageBox.Show("File not found on specified path:\n" + path);
                   return null;
               }
               catch (FieldAccessException ex)
               {
                   MessageBox.Show("File cannot be accessed,is it in use by another application?", "Importer Error", MessageBoxButton.OK, MessageBoxImage.Stop);
                   return null;
               }
               catch (Exception ex)
               {
                   MessageBox.Show("Unknown Error:Details\n" + ex.Message, "Importer Error",
                                   MessageBoxButton.OK, MessageBoxImage.Error);
                   return null;
               }
               return ProductImports;
           });
       }
       private async Task<IEnumerable<Product>> ConstructEntities(IEnumerable<ProductImport> entities)
       {
           return await Task.Run(() =>
                                     {
                                        
               var temp = new List<SaleProduct>();
               var exisitingsproducts = _productRepository.GetAll(true).OfType<SaleProduct>().ToList();
               var defaultSupplier =
                   _supplierRepository.GetAll(true).FirstOrDefault(p => p.Name != null && p.Name.Equals("default"));
               var allBrands = _productBrandRepository.GetAll(true).ToList();
               if (defaultSupplier == null)
               {
                   defaultSupplier = new Supplier(Guid.NewGuid())
                                         {Name = "default", Code = "default", Description = "default"};
                   try
                   {
                       _supplierRepository.Save(defaultSupplier);

                   }
                   catch
                   {
                   }
               }
               
               foreach (var entity in entities)
               {
                   
                   var product = exisitingsproducts.FirstOrDefault(p =>  p.ProductCode !=null && p.ProductCode.Equals(entity.ProductCode,StringComparison.CurrentCultureIgnoreCase
                       )) ?? new SaleProduct(Guid.NewGuid());

                   
                   var brand = allBrands
                           .FirstOrDefault(
                               p =>p.Code != null && (p.Code.Equals(entity.BrandCode, StringComparison.CurrentCultureIgnoreCase)) ||
                               p.Name != null && p.Name.Equals(entity.BrandCode, StringComparison.CurrentCultureIgnoreCase));

                   if (string.IsNullOrEmpty(entity.BrandCode))
                       brand = allBrands.FirstOrDefault(p => p.Code != null && (p.Code.Equals("default", StringComparison.CurrentCultureIgnoreCase) || p.Name != null && p.Name.Equals("default", StringComparison.CurrentCultureIgnoreCase)));
                               
                              
                   if (brand == null)
                   {
                       brand = allBrands.FirstOrDefault(p => p.Code != null && p.Code == "default") ??
                               new ProductBrand(Guid.NewGuid())
                                   {
                                       Code = string.IsNullOrEmpty(entity.BrandCode) ? "default" : entity.BrandCode,
                                       Name = string.IsNullOrEmpty(entity.BrandCode) ? "default" : entity.BrandCode,
                                       Supplier = defaultSupplier
                                   };
                       try
                       {
                           _productBrandRepository.Save(brand);
                       }
                       catch
                       {
                       }
                   }

                   product.Brand = brand;

                   if(product.ProductType==null)
                   {
                      var productType= _productTypeRepository.GetAll(true).FirstOrDefault(
                       p => p.Code != null && p.Code.Equals("default", StringComparison.CurrentCultureIgnoreCase) ||
                           p.Name != null &&
                       p.Name.Equals("default", StringComparison.CurrentCultureIgnoreCase));

                       if (productType == null)
                       {
                           productType = new ProductType(Guid.NewGuid())
                           {
                               Name = "default",
                               Code = "default",
                               Description = "default"
                           };
                           try
                           {
                               _productTypeRepository.Save(productType);
                           }
                           catch
                           {
                           }
                       }
                       product.ProductType = productType;
                   }
                   
                  

                   if (product.PackagingType == null)
                   {
                       var packagingType = _productPackagingTypeRepository.GetAll(true).FirstOrDefault(
                           p => p.Code != null &&
                           p.Code.Equals("default", StringComparison.CurrentCultureIgnoreCase) ||
                           p.Name != null &&
                           p.Name.Equals("default", StringComparison.CurrentCultureIgnoreCase));
                       if (packagingType == null)
                       {
                          packagingType= new ProductPackagingType(Guid.NewGuid())
                           {
                               Name = "default",
                               Code =
                                   "default",
                               Description = "default"
                           };
                           try
                           {
                               _productPackagingTypeRepository.Save(packagingType);
                           }
                           catch
                           {
                           }
                           
                       }
                       product.PackagingType = packagingType;
                   }


                   if (product.Packaging == null)
                   {
                       var packaging =
                           _productPackagingRepository.GetAll(true).FirstOrDefault(
                               p => p.Code != null && p.Code.Equals("default", StringComparison.CurrentCultureIgnoreCase) ||
                                   p.Name != null &&
                                    p.Name.Equals("default", StringComparison.CurrentCultureIgnoreCase));
                       if (packaging == null)
                       {
                           packaging = new ProductPackaging(Guid.NewGuid())
                                           {
                                               Name = "default",
                                               Code =
                                                   "default",
                                               Description =
                                                   "default",
                                           };
                           try
                           {
                               _productPackagingRepository.Save(packaging);
                           }
                           catch
                           {

                           }
                       }
                       product.Packaging = packaging;
                   }

                   if (product.VATClass == null)
                   {
                       var productVat =
                           _vatClassRepository.GetAll(true).FirstOrDefault(
                               p => p.VatClass != null &&
                               p.VatClass == "defaultVAT" ||
                               p.Name != null &&
                               p.Name.Equals("defaultVAT", StringComparison.CurrentCultureIgnoreCase));

                       if (productVat == null)
                       {
                           var viatItem = new VATClass.VATClassItem(Guid.NewGuid())
                                              {
                                                  EffectiveDate = DateTime.Now,
                                                  Rate = 0,

                                              };
                           productVat = new VATClass(Guid.NewGuid())
                                            {
                                                Name = "defaultVAT",
                                                VatClass = "defaultVAT",
                                            };
                           productVat.VATClassItems.Add(viatItem);
                           try
                           {
                               _vatClassRepository.Save(productVat);
                           }
                           catch
                           {
                           }

                       }
                       product.VATClass = productVat;
                   }
                   if(product.Flavour==null)
                   {
                       var flavor =
                           _flavourRepository.GetAll(true).FirstOrDefault(
                               p =>p.Name !=null &&
                               p.Name == "defaultflavor" ||p.Code
                               !=null &&
                               p.Code.Equals("defaultflavor", StringComparison.CurrentCultureIgnoreCase));

                       if (flavor == null)
                       {
                           flavor = new ProductFlavour(Guid.NewGuid())
                                        {
                                            Name = "defaultflavor",
                                            Code = "defaultflavor",
                                            Description = "defaultflavor",
                                            ProductBrand = brand
                                        };
                           try
                           {
                               _flavourRepository.Save(flavor);
                           }
                           catch
                           {
                           }

                       }
                       product.Flavour = flavor;
                   }
                   
                   product.ProductCode = entity.ProductCode;
                   product._Status =EntityStatus.New; //GeneralHelper.GetStatus(entity.Status);
                   product.ExFactoryPrice = entity.ExFactoryPrice;
                   product.Description = entity.Description;

                   

                   temp.Add(product);
               }
               return temp.OfType<Product>().ToList();
           });

       }

       public List<string> GetNonExistingProductCodes()
       {
           return _failedImpoprts;
       }

       public async Task<IList<ImportValidationResultInfo>> ValidateAsync(List<ProductImport> entities)
       {
           return await Task.Run(async () =>
           {
               var results = new List<ImportValidationResultInfo>();
               var productBrands = await ConstructEntities(entities);
               int count = 0;
               foreach (var product in productBrands)
               {
                   
                   var res = await ValidateEntityAsync(product);
                   var importValidationResult = new ImportValidationResultInfo()
                   {
                       Results = res.Results,
                       Description = "Row-" + count,
                       Entity = product
                   };
                   results.Add(importValidationResult);
                   count++;
               }
               return results;
           });

       }


       public async Task<bool> SaveAsync(IEnumerable<Product> entities)
       {
           return await Task.Run(() =>
                                     {
                                         foreach (var product in entities)
                                         {
                                             _productRepository.Save(product);
                                         }

                                         return true;
                                     });
       }

       private async Task<ImportValidationResultInfo> ValidateEntityAsync(Product product)
       {
           return await Task.Run(() =>
           {
               var res = _productRepository.Validate(product);
               return new ImportValidationResultInfo()
               {
                   Results = res.Results,
                   Entity = product
               };
           });
       }

       #region Product Pricing
       public async Task<IList<ImportValidationResultInfo>> ValidatePricingAsync(List<ProductImport> entities)
       {
           return await Task.Run(async () =>
                                           {
                                               IList<ImportValidationResultInfo> results =
                                                   new List<ImportValidationResultInfo>();
                                               int count = 1;
                                               var pricings = await ConstructPricingDomainEntities(entities);
                                               foreach (var domainentity in pricings)
                                               {
                                                   var res = _productPricingRepository.Validate(domainentity);

                                                   var importValidationResult = new ImportValidationResultInfo()
                                                                                    {
                                                                                        Results = res.Results,
                                                                                        Description = "Row-" + count,
                                                                                        Entity = domainentity
                                                                                    };
                                                   results.Add(importValidationResult);

                                                   count++;

                                               }
                                               return results;
                                           });
       }
       
       private async Task<IEnumerable<ProductPricing>> ConstructPricingDomainEntities(IEnumerable<ProductImport> entities)
       {
           return await Task.Run(() =>
                                     {
                                         var newpricing = new List<ProductPricing>();
                                         var exisitingPricing = _productPricingRepository.GetAll(true).ToList();
                                         var pricingTiers = _productPricingTierRepository.GetAll(true).ToList();
                                         var products = _productRepository.GetAll(true).ToList();
                                         foreach (var entity in entities)
                                         {
                                             var product =products.FirstOrDefault(
                                                     p => p.ProductCode.ToLower() == entity.ProductCode.ToLower());

                                             if (product != null)
                                             {
                                                 var pricingExist =
                                                     exisitingPricing.FirstOrDefault(
                                                         p => p.ProductRef.ProductId == product.Id && p.Tier.Code == "KES");
                                                 if (pricingExist == null)
                                                 {
                                                     var tire = pricingTiers
                                                                    .FirstOrDefault(
                                                                        p =>
                                                                        p.Code.Equals("KES",
                                                                                      StringComparison.
                                                                                          CurrentCultureIgnoreCase)) ??
                                                                pricingTiers.FirstOrDefault(p => p.Code != null && p.Name =="DefaultPricingTire");

                                                     if (tire == null)
                                                     {
                                                         tire = new ProductPricingTier(Guid.NewGuid())
                                                                    {
                                                                        Name = "DefaultPricingTire",
                                                                        Code = "DefaultPricingTire",
                                                                        Description = "DefaultPricingTire",
                                                                    };
                                                         try
                                                         {
                                                             _productPricingTierRepository.Save(tire);
                                                         }
                                                         catch
                                                         {
                                                         }
                                                     }

                                                     pricingExist = new ProductPricing(Guid.NewGuid())
                                                                        {
                                                                            ProductRef =
                                                                                new ProductRef()
                                                                                    {ProductId = product.Id},
                                                                            Tier = tire
                                                                        };

                                                 }
                                                 pricingExist.ProductPricingItems
                                                     .Add(new ProductPricing.ProductPricingItem(Guid.NewGuid())
                                                              {
                                                                  EffectiveDate = DateTime.Now,
                                                                  ExFactoryRate = entity.ExFactoryPrice,
                                                                  SellingPrice = entity.SellingPrice
                                                              });
                                                pricingExist._Status=EntityStatus.New;
                                                 newpricing.Add(pricingExist);

                                             }
                                             else
                                             {
                                                 if (!_failedImpoprts.Any(p => p.Equals(entity.ProductCode, StringComparison.CurrentCultureIgnoreCase)))
                                                 {
                                                     _failedImpoprts.Add(entity.ProductCode);
                                                 }
                                             }

                                         }
                                         return newpricing;

                                     });

       }

       public async Task<bool> SaveAsync(List<ProductPricing> entities)
       {
          return  await Task.Run(() =>
                              {
                                  foreach (var pricing in entities)
                                  {
                                      if (!_productPricingRepository.GetAll(true).Any(p=>p.Tier.Id==pricing.Tier.Id && p.ProductRef.ProductId==pricing.ProductRef.ProductId))
                                     _productPricingRepository.Save(pricing);
                                  }
                                  return true;

                              });

       }

        #endregion


    }
}
