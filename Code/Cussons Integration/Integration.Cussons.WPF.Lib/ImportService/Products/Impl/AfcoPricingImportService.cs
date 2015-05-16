using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Repository.Master.ProductRepositories;
using Integration.Cussons.WPF.Lib.ImportEntities;
using Integration.Cussons.WPF.Lib.MasterDataImportService;
using Integration.Cussons.WPF.Lib.Utils;
using LINQtoCSV;

namespace Integration.Cussons.WPF.Lib.ImportService.Products.Impl
{
    /// <summary>
    /// this is a special pricing for the military so we create an afco pricing tier and append the prices
    /// </summary>
   internal class AfcoPricingImportService:IAfcoPricingImportService
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductPricingRepository _productPricingRepository;
        private readonly IProductPricingTierRepository _productPricingTierRepository;
        private List<string> _failedImpoprts;

        public AfcoPricingImportService(IProductRepository productRepository, IProductPricingRepository productPricingRepository, IProductPricingTierRepository productPricingTierRepository)
        {
            _productRepository = productRepository;
            _productPricingRepository = productPricingRepository;
            _productPricingTierRepository = productPricingTierRepository;
            _failedImpoprts = new List<string>();
        }

        public async Task<IEnumerable<AfcoProductPricingImport>> Import(string path)
       {
           return await Task.Factory.StartNew(() =>
           {
               IEnumerable<AfcoProductPricingImport> ProductImports;
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
                 
                   ProductImports = cc.Read<AfcoProductPricingImport>(path, inputFileDescription);
                  

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

       public async Task<IList<ImportValidationResultInfo>> ValidateAsync(List<AfcoProductPricingImport> entities)
       {
           return await Task.Run(async () =>
                                           {
                                               IList<ImportValidationResultInfo> results =
                                                   new List<ImportValidationResultInfo>();
                                               int count = 1;
                                               var pricings = await ConstructPricingDomainEntities(entities);
                                               foreach (var domainentity in pricings)
                                               {
                                                   var res =await ValidatePricingAsync(domainentity);

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
       private async Task<ImportValidationResultInfo> ValidatePricingAsync(ProductPricing pricing)
       {
           return await Task.Run(() =>
           {
               var res = _productPricingRepository.Validate(pricing);
               return new ImportValidationResultInfo()
               {
                   Results = res.Results,
                   Entity = pricing
               };
           });
       }

       public async Task<bool> SaveAsync(List<ProductPricing> entities)
       {
           return await Task.Run(() =>
           {
               foreach (var pricing in entities)
               {
                   if (!_productPricingRepository.GetAll(true).Any(p => p.Tier.Id == pricing.Tier.Id && p.ProductRef.ProductId == pricing.ProductRef.ProductId))
                       _productPricingRepository.Save(pricing);
               }
               return true;

           });
       }

        public List<string> GetNonExistingProductCodes()
        {
            return _failedImpoprts;
        }


        private async Task<IEnumerable<ProductPricing>> ConstructPricingDomainEntities(IEnumerable<AfcoProductPricingImport> entities)
       {
           return await Task.Run(() =>
           {
               var newpricing = new List<ProductPricing>();
               var exisitingPricing = _productPricingRepository.GetAll(true).ToList();
               var products = _productRepository.GetAll(true).ToList();
               var tiers = _productPricingTierRepository.GetAll(true).ToList();
               foreach (var entity in entities)
               {
                   var product = products
                       .FirstOrDefault(p => p.ProductCode.ToLower() == entity.ProductCode.ToLower());
                   if (product != null)
                   {
                       var pricingExist =
                           exisitingPricing.FirstOrDefault(
                               p =>
                               p.ProductRef.ProductId == product.Id && (p.Tier.Code != null && p.Tier.Code.ToLower()== "afco"));
                       if (pricingExist == null)
                       {
                           var tire = tiers
                               .FirstOrDefault(
                                   p =>
                                   p.Code.Equals("Afco",
                                                 StringComparison.CurrentCultureIgnoreCase));

                           if (tire == null)
                           {
                               tire = new ProductPricingTier(Guid.NewGuid())
                               {
                                   Name = "Afco",
                                   Code = "Afco",
                                   Description = "Afco",
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
                                   new ProductRef() { ProductId = product.Id },
                               Tier = tire
                           };
                       }

                       pricingExist.ProductPricingItems
                           .Add(new ProductPricing.ProductPricingItem(Guid.NewGuid())
                           {
                               EffectiveDate = DateTime.Now,
                               ExFactoryRate =product.ExFactoryPrice,
                               SellingPrice = entity.SellingPrice
                           });
                      
                       newpricing.Add(pricingExist);

                   }
                   else
                   {
                       if (!_failedImpoprts.Any(p => p.Equals(entity.ProductCode,StringComparison.CurrentCultureIgnoreCase)))
                       {
                           _failedImpoprts.Add(entity.ProductCode);
                       }
                   }
               }

               return newpricing;

           });

       }

       
    }
}
