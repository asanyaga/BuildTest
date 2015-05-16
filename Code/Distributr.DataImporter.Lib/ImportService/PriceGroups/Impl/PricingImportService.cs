using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Factory.Master;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Utility;
using Distributr.DataImporter.Lib.Experimental;
using Distributr.DataImporter.Lib.ImportEntity;
using Distributr.DataImporter.Lib.Utils;
using LINQtoCSV;
using StructureMap;

namespace Distributr.DataImporter.Lib.ImportService.PriceGroups.Impl
{
    public class PricingImportService : IPricingImportService
    {
       private List<string> _failedPricings;

        public PricingImportService()
        {
            _failedPricings = new List<string>();
        }

        public IEnumerable<PricingImport> Import(string path)
        {
            try
            {
                var productImports = new List<PricingImport>();
                var tempFolder =
                    Path.Combine(FileUtility.GetApplicationTempFolder(), Path.GetFileName(path));
                if (File.Exists(tempFolder))
                    File.Delete(tempFolder);
                File.Copy(path, tempFolder);

                using (var parser = new Microsoft.VisualBasic.FileIO.TextFieldParser(tempFolder))
                {
                    parser.SetDelimiters(",");
                    string[] currentRow = null;

                    while (!parser.EndOfData)
                    {
                        currentRow = parser.ReadFields();
                        if (currentRow != null && currentRow.Length > 0)
                        {
                            productImports.Add(MapImport(currentRow));
                        }
                    }

                }
                File.Delete(tempFolder);
                return productImports;
            }
            catch (FileNotFoundException ex)
            {
                throw ex;
            }
            catch (FieldAccessException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Importer Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }

        }

        private PricingImport MapImport(string[] dataRow)
        {
            var spstring = SetFieldValue(dataRow, 3);
            var exfactorystring = SetFieldValue(dataRow, 4);
            decimal exprice = 0m;
            try
            {
                if (!string.IsNullOrEmpty(exfactorystring))
                    exprice = Convert.ToDecimal(exfactorystring);

            }
            catch
            {
                exprice = 0m;
            }
            decimal sellingprice = 0m;
            try
            {
                if (!string.IsNullOrEmpty(spstring))
                    sellingprice = Convert.ToDecimal(spstring);

            }
            catch
            {
                sellingprice = 0m;
            }
            return new PricingImport
            {
                ProductCode = SetFieldValue(dataRow, 1),
                PricingTireCode = SetFieldValue(dataRow, 2),
                SellingPrice = sellingprice,
                ExFactoryRate = exprice,
               
            };

        }

        string SetFieldValue(string[] dataRow, int index)
        {
            index = index - 1;
            return (dataRow.Length - 1 < index || string.IsNullOrEmpty(dataRow[index])) ? "" : dataRow[index];
        }

        public IList<ImportValidationResultInfo> Validate(List<PricingImport> entities)
        {
            IList<ImportValidationResultInfo> results = new List<ImportValidationResultInfo>();
            int count = 1;
            foreach (var domainentity in ConstructDomainEntities(entities))
            {
                var res = ObjectFactory.GetInstance<IProductPricingRepository>().Validate(domainentity);

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
        }

        public Task<IList<ImportValidationResultInfo>> ValidateAsync(List<PricingImport> entities)
        {
            throw new NotImplementedException();
        }

        public IList<ImportValidationResultInfo> ValidateAndSave(List<PricingImport> entities)
        {
            int batchSize = Convert.ToInt32(0.2 * entities.Count);
            var productPricingImports = entities.OrderBy(p => p.ProductCode).Batch(batchSize).Select(x => x.ToList()).ToList();

            #region Contruct Items
            var taskArray = new Task<IEnumerable<ProductPricing>>[productPricingImports.Count];
            var results = new List<ProductPricing>();
            try
            {
                for (int i = 0; i < taskArray.Length; i++)
                {
                    var current = productPricingImports.FirstOrDefault();
                    if (current != null && current.Any())
                    {
                        //taskArray[i] = Task<IEnumerable<ProductPricing>>.Factory.StartNew(() => ConstructDomainEntities(current));
                        ConstructDomainEntities(current);
                        productPricingImports.Remove(current);
                    }
                }

                foreach (var result in taskArray.Select(n => n.Result).ToList())
                {
                    results.AddRange(result);
                }
            }
            catch (AggregateException ex)
            {
            }
            #endregion

            #region Validate
            var validationResults = new List<ImportValidationResultInfo>();
            if (results.Any())
            {
                batchSize = Convert.ToInt32(0.2 * results.Count);
                var products = results.OrderBy(p => p.CurrentEffectiveDate).Batch(batchSize).Select(x => x.ToList()).ToList();
                var validationTaskArray = new Task<ImportValidationResultInfo[]>[products.Count];


                try
                {
                    for (int i = 0; i < validationTaskArray.Length; i++)
                    {
                        var current = products.FirstOrDefault();
                        if (current != null && current.Any())
                        {
                            validationTaskArray[i] =
                                Task<ImportValidationResultInfo[]>.Factory.StartNew(() => ValidatePricings(current));
                            products.Remove(current);
                        }
                    }

                    foreach (var result in validationTaskArray.Select(n => n.Result).ToList())
                    {
                        validationResults.AddRange(result);
                    }
                }
                catch (AggregateException ex)
                {
                }

            }
            #endregion

            #region Save valid Items
            var validatedPricings = validationResults.Where(n => n.IsValid).Select(n => (ProductPricing)n.Entity).ToList();
            if (validatedPricings.Any())
            {
                batchSize = Convert.ToInt32(0.2 * validatedPricings.Count);
                var products =
                    validatedPricings.OrderBy(p => p.CurrentEffectiveDate).Batch(batchSize).Select(x => x.ToList()).ToList();
                var saveTasksArray = new Task<Guid[]>[products.Count];
                try
                {
                    for (int i = 0; i < saveTasksArray.Length; i++)
                    {
                        var current = products.FirstOrDefault();
                        if (current != null && current.Any())
                        {
                            saveTasksArray[i] =
                                Task<Guid[]>.Factory.StartNew(() => SaveProductPricings(current));
                            products.Remove(current);
                        }
                    }
                    var savedResults = new List<Guid>();
                    foreach (var result in saveTasksArray.Select(n => n.Result).ToList())
                    {
                        savedResults.AddRange(result);
                    }
                }
                catch (AggregateException ex)
                {
                }
            }
            #endregion

            return validationResults.Where(p => !p.IsValid).ToList();
        }
        private bool Update(Guid pricingTierId, Guid productId, decimal sellingPrice,decimal exfactoryRate)
        {
            var exist = ObjectFactory.GetInstance<IProductPricingRepository>().GetByProductAndTierId(productId, pricingTierId);

            return exist == null || (exist.CurrentExFactory.ToString("0.00") != exfactoryRate.ToString("0.00")) || (exist.CurrentSellingPrice.ToString("0.00") != sellingPrice.ToString("0.00"));
        }

        private Guid[] SaveProductPricings(IEnumerable<ProductPricing> productpricings)
        {
            return (from pricing in productpricings
                    let res = ObjectFactory.GetInstance<IProductPricingRepository>().Save(pricing, true)
                    select new Guid(res.ToString())
                    {

                    }).ToArray();
        }
        private ImportValidationResultInfo[] ValidatePricings(IEnumerable<ProductPricing> pricings)
        {
            return (from pricing in pricings
                    let res = ObjectFactory.GetInstance<IProductPricingRepository>().Validate(pricing)
                    select new ImportValidationResultInfo()
                               {
                                   Results = res.Results,
                                   Entity = pricing
                               }).ToArray();
        }
   
        private IEnumerable<ProductPricing> ConstructDomainEntities(IEnumerable<PricingImport> entities)
        {
           var newpricing = new List<ProductPricing>();
          
               foreach (var importentity in entities)
               {
                   try
                   {
                       var product = ObjectFactory.GetInstance<IPricingMapper>().GetProduct(importentity.ProductCode);
                       if (product != null)
                       {


                           if (!string.IsNullOrEmpty(importentity.PricingTireCode))
                           {
                               ProductPricingTier tire =
                                   ObjectFactory.GetInstance<IProductPricingTierRepository>().GetAll(true).
                                       FirstOrDefault(
                                           p =>
                                           p.Code != null && p.Code.ToLower() == importentity.PricingTireCode.ToLower());
                               if (tire == null)
                               {
                                   tire = new ProductPricingTier(Guid.NewGuid())
                                              {
                                                  Name = importentity.PricingTireCode,
                                                  Code = importentity.PricingTireCode,
                                                  Description = importentity.PricingTireCode,
                                              };
                                   try
                                   {
                                       ObjectFactory.GetInstance<IProductPricingTierRepository>().Save(tire);
                                   }
                                   catch
                                   {
                                   }
                               }
                               if (Update(tire.Id, product.id, importentity.SellingPrice, product.ExFactoryPrice))
                               {
                                   var pricing =
                                       ObjectFactory.GetInstance<IProductPricingRepository>().GetByProductAndTierId(
                                           product.id, tire.Id);
                                   if (pricing != null)
                                   {
                                       pricing.ProductPricingItems.Clear();
                                       pricing.ProductPricingItems.Add(
                                           new ProductPricing.ProductPricingItem(Guid.NewGuid())
                                               {
                                                   EffectiveDate = DateTime.Now,
                                                   ExFactoryRate = product.ExFactoryPrice,
                                                   SellingPrice = importentity.SellingPrice
                                               });
                                       ObjectFactory.GetInstance<IPricingMapper>().Update(pricing);

                                   }
                                   else
                                   {
                                       var newprice =
                                           ObjectFactory.GetInstance<IProductPricingFactory>().CreateProductPricing(
                                               product.id, tire.Id,
                                               product.ExFactoryPrice,
                                               importentity.SellingPrice,
                                               DateTime.Now);
                                       if (newprice != null)
                                           newpricing.Add(newprice);

                                   }
                               }
                           }
                           else
                           {
                               #region default pricing tier

                               var pricingTierDefault =
                                   ObjectFactory.GetInstance<IProductPricingTierRepository>().GetAll(true).
                                       FirstOrDefault(
                                           p => p.Name == "DefaultPricingTier");
                               if (pricingTierDefault == null)
                               {
                                   pricingTierDefault = new ProductPricingTier(Guid.NewGuid())
                                                            {
                                                                Name = "DefaultPricingTier",
                                                                Code = "DefaultPricingTier",
                                                                Description = "DefaultPricingTier",
                                                            };
                                   try
                                   {
                                       ObjectFactory.GetInstance<IProductPricingTierRepository>().Save(
                                           pricingTierDefault);
                                   }
                                   catch
                                   {
                                   }
                               }

                               if (Update(pricingTierDefault.Id, product.id, product.ExFactoryPrice,
                                          product.ExFactoryPrice))
                               {
                                   var defaultPrice =
                                       ObjectFactory.GetInstance<IProductPricingRepository>().GetByProductAndTierId(
                                           product.id, pricingTierDefault.Id);
                                   if (defaultPrice != null)
                                   {
                                       defaultPrice.ProductPricingItems.Clear();
                                       defaultPrice.ProductPricingItems.Add(
                                           new ProductPricing.ProductPricingItem(Guid.NewGuid())
                                               {
                                                   EffectiveDate = DateTime.Now,
                                                   ExFactoryRate = product.ExFactoryPrice,
                                                   SellingPrice = product.ExFactoryPrice
                                               });
                                       ObjectFactory.GetInstance<IPricingMapper>().Update(defaultPrice);

                                   }
                                   else
                                   {
                                       defaultPrice =
                                           ObjectFactory.GetInstance<IProductPricingFactory>().CreateProductPricing(
                                               product.id, pricingTierDefault.Id,
                                               product.ExFactoryPrice,
                                               product.ExFactoryPrice,
                                               DateTime.Now);
                                       if (defaultPrice != null &&
                                           !newpricing.Any(
                                               p =>
                                               p.Tier.Id == pricingTierDefault.Id &&
                                               p.ProductRef.ProductId == product.id))
                                           newpricing.Add(defaultPrice);
                                   }

                               }

                               #endregion
                           }

                       }
                       else
                       {
                           if (
                               !_failedPricings.Any(
                                   p => p.Equals(importentity.ProductCode, StringComparison.CurrentCultureIgnoreCase)))
                           {
                               _failedPricings.Add(importentity.ProductCode);
                           }
                       }

                   }
                   catch (Exception ex)
                   {

                   }
               }
               return newpricing;
           

        }

        public void Save(List<ProductPricing> entities)
        {
            ObjectFactory.GetInstance<IPricingMapper>().Insert(entities);

        }
     
        public List<string> GetNonExistingProductCodes()
        {
            return _failedPricings;
        }
    }
}
