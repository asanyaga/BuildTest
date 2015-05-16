using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Master.SuppliersEntities;
using Distributr.Core.Factory.Master;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Master.SuppliersRepositories;
using Distributr.Core.Utility;
using Distributr.DataImporter.Lib.Experimental;
using Distributr.DataImporter.Lib.ImportEntity;
using Distributr.DataImporter.Lib.Utils;
using StructureMap;

namespace Distributr.DataImporter.Lib.ImportService.Products.Impl
{
   public class ProductImportService :IProductImportService
   {
      
       public IEnumerable<ProductImport> Import(string path)
       {
           try
           {
               
           var productImports = new List<ProductImport>();
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
                       productImports.Add(MapProductImport(currentRow));
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

       private ProductImport MapProductImport(string[] dataRow)
       {
          var expricestring = SetFieldValue(dataRow, 3);
           decimal exprice = 0m;
           try
           {
               if (!string.IsNullOrEmpty(expricestring))
                   exprice = Convert.ToDecimal(expricestring);

           }
           catch
           {
               exprice = 0m;
           }
           return new ProductImport
           {
               ProductCode = SetFieldValue(dataRow, 1),
               Description = SetFieldValue(dataRow, 2),
               ExFactoryPrice = exprice,
               PackagingTypeCode=SetFieldValue(dataRow,4),
               DiscountGroup=SetFieldValue(dataRow,5),
               CustomerDiscount=SetFieldValue(dataRow,6),
               VATClass=SetFieldValue(dataRow,7),
               BrandCode = SetFieldValue(dataRow, 8)
           };



       }

       string SetFieldValue(string[] dataRow, int index)
       {
           index = index - 1;
           return (dataRow.Length - 1 < index || string.IsNullOrEmpty(dataRow[index])) ? "" : dataRow[index];
       }

       public IList<ImportValidationResultInfo> Validate(List<ProductImport> entities)
       {
            IList<ImportValidationResultInfo> results=new List<ImportValidationResultInfo>();
           int count = 1;
           foreach (var product in ConstructProducts(entities))
           {
               var res = ObjectFactory.GetInstance<IProductRepository>().Validate(product);
              
                   var importValidationResult = new ImportValidationResultInfo()
                                                    {
                                                        Results = res.Results,
                                                        Description = "Row-"+count,
                                                        Entity = product,
                                                        EntityNameOrCode = product.ProductCode ?? product.Description
                                                    };
                   results.Add(importValidationResult);
                  
             
               count++;
              
           }
           return results;
       }

       public async Task<IList<ImportValidationResultInfo>> ValidateAsync(List<ProductImport> entities)
       {
           var task = Task.Run(async () =>
                                               {
                                                   var results = new List<ImportValidationResultInfo>();
                                                   var products = ConstructProducts(entities);
                                                   int count = 0;
                                                   foreach (var product in products)
                                                   {
                                                       var res = await ValidateProductAsync(product);
                                                       var importValidationResult = new ImportValidationResultInfo()
                                                       {
                                                           Results = res.Results,
                                                           Description = "Row-" + count,
                                                           Entity = product,
                                                           EntityNameOrCode = product.ProductCode ?? product.Description
                                                       };
                                                       results.Add(importValidationResult);
                                                       count++;
                                                   }
                                                   return results;
                                               });
           return await task;

       }

       private async Task<ImportValidationResultInfo> ValidateProductAsync(Product product)
       {
           var task = Task.Run(() =>
           {

               var res = ObjectFactory.GetInstance<IProductRepository>().Validate(product);
               return new ImportValidationResultInfo()
                          {
                              Results = res.Results,
                              Entity = product
                          };
           });
           return await task;
       }

       private SaleProduct[] ConstructProducts(IEnumerable<ProductImport> entities)
       {
           var temp = new List<SaleProduct>();

           var defaultSupplier = ObjectFactory.GetInstance<ISupplierRepository>().GetAll(true).FirstOrDefault(p => p.Name != null && p.Name.Equals("default"));
           if (defaultSupplier == null)
           {
               defaultSupplier = new Supplier(Guid.NewGuid()) { Name = "default", Code = "default", Description = "default" };
               try
               {
                   ObjectFactory.GetInstance<ISupplierRepository>().Save(defaultSupplier);

               }
               catch
               {
               }
           }
          var validEntities = entities.Where(p => p.Description != null && !p.Description.ToLower().Contains("discontinued"));
          foreach (var entity in validEntities)
          {
              var product =
                  ObjectFactory.GetInstance<IProductRepository>().GetByCode(entity.ProductCode, true) as SaleProduct;
              bool isNew = false;
              if(product==null)
              {
                  product = new SaleProduct(Guid.NewGuid());
                  isNew = true;
              }
                             
               #region brand
               var allBrands = ObjectFactory.GetInstance<IProductBrandRepository>().GetAll(true).ToList();
               var brand = allBrands
                       .FirstOrDefault(
                           p =>
                           p.Code != null && p.Code.Equals(entity.BrandCode, StringComparison.CurrentCultureIgnoreCase) ||
                           p.Name != null && p.Name.Equals(entity.BrandCode, StringComparison.CurrentCultureIgnoreCase)
                           );

               if (string.IsNullOrEmpty(entity.BrandCode))
                   brand = allBrands.FirstOrDefault(p => p.Code != null && p.Code == "default");

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
                           ObjectFactory.GetInstance<IProductBrandRepository>().Save(brand);
                       }
                       catch
                       {
                       }
                   }
               
               product.Brand = brand;
               #endregion

               #region Product Type

               var productType = ObjectFactory.GetInstance<IProductTypeRepository>().GetAll(true).FirstOrDefault(
                       p => p.Code != null &&
                       p.Code.Equals(entity.ProductTypeCode, StringComparison.CurrentCultureIgnoreCase) ||
                       p.Code!= null &&p.Code.Equals("default", StringComparison.CurrentCultureIgnoreCase) ||
                      p.Name != null &&
                       p.Name.Equals("default", StringComparison.CurrentCultureIgnoreCase));
               if (productType == null)
               {
                   productType = new ProductType(Guid.NewGuid())
                                     {
                                         Name = string.IsNullOrEmpty(entity.ProductTypeCode)
                                                    ? "default"
                                                    : entity.ProductTypeCode,
                                         Code = string.IsNullOrEmpty(entity.ProductTypeCode)
                                                    ? "default"
                                                    : entity.ProductTypeCode,
                                         Description = string.IsNullOrEmpty(entity.ProductTypeCode)
                                                           ? "default"
                                                           : entity.ProductTypeCode
                                     };
                   try
                   {
                      ObjectFactory.GetInstance<IProductTypeRepository>().Save(productType);
                   }catch
                   {
                   }
               }
               product.ProductType = productType;
               #endregion

               #region Packaging type
               var allPackagingTypes = ObjectFactory.GetInstance<IProductPackagingTypeRepository>().GetAll(true).ToList();
               ProductPackagingType packagingType = null;

               if (string.IsNullOrEmpty(entity.PackagingTypeCode))
                   packagingType = allPackagingTypes.FirstOrDefault(p => p.Name != null && p.Name == "default");
               else
               {
                   packagingType = allPackagingTypes.FirstOrDefault(
                   p =>
                   p.Code != null && p.Code.Equals(entity.PackagingTypeCode, StringComparison.CurrentCultureIgnoreCase));
                   
               }
               if(packagingType==null)
               {
                   packagingType =allPackagingTypes.FirstOrDefault(p => p.Name != null && p.Name == "default")?? new ProductPackagingType(Guid.NewGuid())
                                       {
                                           Name =
                                               string.IsNullOrEmpty(entity.PackagingTypeCode)
                                                   ? "default"
                                                   : entity.PackagingTypeCode,
                                           Code =
                                               string.IsNullOrEmpty(entity.PackagingTypeCode)
                                                   ? "default"
                                                   : entity.PackagingTypeCode,
                                           Description =
                                               string.IsNullOrEmpty(entity.PackagingTypeCode)
                                                   ? "default"
                                                   : entity.PackagingTypeCode
                                       };
                   try
                   {
                       ObjectFactory.GetInstance<IProductPackagingTypeRepository>().Save(packagingType);
                   }
                   catch
                   {
                   }
               }
               product.PackagingType = packagingType;

               #endregion

               #region Packaging
               var packagings = ObjectFactory.GetInstance<IProductPackagingRepository>().GetAll(true).ToList();

               ProductPackaging packaging;
               if(string.IsNullOrEmpty(entity.PackagingCode))
                   packaging =
                       packagings.FirstOrDefault(
                           p => p.Code != null && p.Code.Equals("default", StringComparison.CurrentCultureIgnoreCase));
               else
               {
                   packaging =
                       packagings.FirstOrDefault(
                           p =>
                           p.Code != null &&
                           p.Code.Equals(entity.PackagingCode, StringComparison.CurrentCultureIgnoreCase));
               }
               if(packaging==null)
               {
                   packaging = packagings.FirstOrDefault(
                       p => p.Code != null && p.Code.Equals("default", StringComparison.CurrentCultureIgnoreCase)) ??
                               new ProductPackaging(Guid.NewGuid())
                                   {
                                       Name =
                                           string.IsNullOrEmpty(entity.PackagingCode) ? "default" : entity.PackagingCode,
                                       Code =
                                           string.IsNullOrEmpty(entity.PackagingCode) ? "default" : entity.PackagingCode,
                                       Description =
                                           string.IsNullOrEmpty(entity.PackagingCode) ? "default" : entity.PackagingCode
                                   };
                   try
                   {
                       ObjectFactory.GetInstance<IProductPackagingRepository>().Save(packaging);
                   }catch
                   {
                       
                   }
               }
               product.Packaging = packaging;
               #endregion

               #region VAT
               VATClass productVat;

               var productVats = ObjectFactory.GetInstance<IVATClassRepository>().GetAll(true).ToList();
               if(string.IsNullOrEmpty(entity.VATClass))
                  productVat= productVats.FirstOrDefault(p => p.VatClass != null  && p.VatClass.Equals("defaultVAT", StringComparison.CurrentCultureIgnoreCase));
               else
               {
                   productVat =
                       productVats.FirstOrDefault(
                           p =>
                           p.VatClass != null &&
                           p.VatClass.Equals(entity.VATClass, StringComparison.CurrentCultureIgnoreCase) ||
                           p.Name != null && p.Name.Equals(entity.VATClass, StringComparison.CurrentCultureIgnoreCase));
               }

               if(productVat==null)
               {
                   var viatItem = new VATClass.VATClassItem(Guid.NewGuid())
                                      {
                                          EffectiveDate = DateTime.Now,
                                          Rate = 0,

                                      };

                   productVat =
                       productVats.FirstOrDefault(
                           p =>
                           p.VatClass != null &&
                           p.VatClass.Equals("defaultVAT", StringComparison.CurrentCultureIgnoreCase)) ??
                       new VATClass(Guid.NewGuid())
                           {
                               Name = string.IsNullOrEmpty(entity.VATClass) ? "defaultVAT" : entity.VATClass,
                               VatClass = string.IsNullOrEmpty(entity.VATClass) ? "defaultVAT" : entity.VATClass,
                           };
                   productVat.VATClassItems.Add(viatItem);
                   try
                   {
                       ObjectFactory.GetInstance<IVATClassRepository>().Save(productVat);
                   }catch
                   {
                   }
               }
               product.VATClass = productVat;
               #endregion
              
               #region Flavour
               var allFlavors = ObjectFactory.GetInstance<IProductFlavourRepository>().GetAll(true).ToList();
               ProductFlavour flavor;
               if (string.IsNullOrEmpty(entity.ProductFlavourCode))
                   flavor =
                       allFlavors.FirstOrDefault(
                           p =>
                           p.Code != null && p.Code.Equals("defaultflavor", StringComparison.CurrentCultureIgnoreCase));
               else
               {
                   flavor=allFlavors.FirstOrDefault(p => p.Code != null && p.Code == entity.ProductFlavourCode);
               }
               if(flavor==null)
               {
                   flavor =
                       allFlavors.FirstOrDefault(
                           p =>
                           p.Code != null && p.Code.Equals("defaultflavor", StringComparison.CurrentCultureIgnoreCase)) ??
                       new ProductFlavour(Guid.NewGuid())
                           {
                               Name =
                                   string.IsNullOrEmpty(entity.ProductFlavourCode)
                                       ? "defaultflavor"
                                       : entity.ProductFlavourCode,
                               Code =
                                   string.IsNullOrEmpty(entity.ProductFlavourCode)
                                       ? "defaultflavor"
                                       : entity.ProductFlavourCode,
                               Description =
                                   string.IsNullOrEmpty(entity.ProductFlavourCode)
                                       ? "defaultflavor"
                                       : entity.ProductFlavourCode,
                               ProductBrand = product.Brand
                           };
                   try
                   {
                       ObjectFactory.GetInstance<IProductFlavourRepository>().Save(flavor);
                   }
                   catch (Exception ex)
                   {
                       FileUtility.LogError(ex.Message);
                   }
               }
               product.Flavour = flavor;
               #endregion

               #region Discount Group
               if(!string.IsNullOrEmpty(entity.DiscountGroup))
               {
                   DiscountGroup discountGroup = ObjectFactory.GetInstance<IGroupDiscountMapper>().FindByCode(entity.DiscountGroup);
                      
                   if(discountGroup==null)
                   {
                       discountGroup = new DiscountGroup(Guid.NewGuid())
                       {
                           Code = entity.DiscountGroup.Trim(),
                           Name = entity.DiscountGroup.Trim(),
                           _Status = EntityStatus.Active
                       };
                       try
                       {
                           ObjectFactory.GetInstance<IGroupDiscountMapper>().Insert(discountGroup);
                       }catch(Exception ex)
                       {
                           FileUtility.LogError(ex.Message);
                       }
                   }

               }
              
               #endregion
              
               product.ProductCode = entity.ProductCode;
               product.ExFactoryPrice = entity.ExFactoryPrice;
               product.Description = entity.Description;
              UpdateDefaultTierPricing(product.Id, product.ExFactoryPrice, product.ExFactoryPrice);

               if (isNew || HasProductChanged(product))
                   temp.Add(product);
           }
           return temp.ToArray();
       }

       private Guid[] SaveProducts(IEnumerable<SaleProduct> products)
       {
           return (from outlet in products
                   let res = ObjectFactory.GetInstance<IProductRepository>().Save(outlet, true)
                   select new Guid(res.ToString())
                   {

                   }).ToArray();
       }
       private ImportValidationResultInfo[] ValidateProducts(IEnumerable<SaleProduct> products)
       {
           return (from product in products
                   let res = ObjectFactory.GetInstance<IProductRepository>().Validate(product)
                   select new ImportValidationResultInfo()
                   {
                       Results = res.Results,
                       Entity = product,
                       Description = string.Format("{0},{1}",product.ProductCode??"",product.Description??"")
                   }).ToArray();
       }
       private bool HasProductChanged(SaleProduct item)
       {
           
               var product = ObjectFactory.GetInstance<IProductRepository>().GetById(item.Id);
               if (product == null) return true;
               var desc = product.Description.Trim().ToLower();
               var itemdesc = item.Description.Trim().ToLower();
               var productprice = Math.Round(product.ExFactoryPrice, 2);
               var itemprice = Math.Round(item.ExFactoryPrice, 2);

              
           return (product.Id != item.Id) || (product.Brand.Id != item.Brand.Id) || (productprice != itemprice)
                      || (desc != itemdesc) || (product.ProductCode.ToLower() != item.ProductCode.ToLower());
           

       }
       private void UpdateDefaultTierPricing(Guid productId, decimal exfactory, decimal sellingprice)
       {
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
                   ObjectFactory.GetInstance<IProductPricingTierRepository>().Save(pricingTierDefault);
               }
               catch
               {
               }
           }
       //    var exist = ObjectFactory.GetInstance<IProductPricingRepository>().GetByProductAndTierId(productId,
       //                                                                                             pricingTierDefault.
       //                                                                                                 Id);
       //    if (exist == null)
       //    {
       //        exist = ObjectFactory.GetInstance<IProductPricingFactory>().CreateProductPricing(
       //            productId, pricingTierDefault.Id,
       //            exfactory,
       //            sellingprice,
       //            DateTime.Now);
       //        ObjectFactory.GetInstance<IProductPricingRepository>().Save(exist);
       //    }
       //    else
       //    {
       //        exist.ProductPricingItems.Clear();
       //        exist.ProductPricingItems.Add(
       //            new ProductPricing.ProductPricingItem(Guid.NewGuid())
       //                {
       //                    EffectiveDate = DateTime.Now,
       //                    ExFactoryRate = exfactory,
       //                    SellingPrice = sellingprice
       //                });
       //        ObjectFactory.GetInstance<IPricingMapper>().Update(exist);
       //    }
       }

       public void Save(List<Product> entities)
       {
            using (var context = ObjectFactory.Container.GetNestedContainer())
            {
                foreach (var product in entities)
                {
                  context.GetInstance<IProductRepository>().Save(product);
                }
            }
       }

       public IList<ImportValidationResultInfo> ValidateAndSave(List<ProductImport> entities)
       {
           int batchSize =Convert.ToInt32(0.2 * entities.Count);
           var productImports = entities.OrderBy(p => p.ProductCode).Batch(batchSize).Select(x => x.ToList()).ToList();

           #region Contruct Items
           var taskArray = new Task<SaleProduct[]>[productImports.Count];
           var results = new List<SaleProduct>();
           try
           {
               for (int i = 0; i < taskArray.Length; i++)
               {
                   var current = productImports.FirstOrDefault();
                   if (current != null && current.Any())
                   {
                       taskArray[i] = Task<SaleProduct[]>.Factory.StartNew(() => ConstructProducts(current));
                       productImports.Remove(current);
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
               var products = results.OrderBy(p => p.ProductCode).Batch(batchSize).Select(x => x.ToList()).ToList();
               var validationTaskArray = new Task<ImportValidationResultInfo[]>[products.Count];


               try
               {
                   for (int i = 0; i < validationTaskArray.Length; i++)
                   {
                       var current = products.FirstOrDefault();
                       if (current != null && current.Any())
                       {
                           validationTaskArray[i] =
                               Task<ImportValidationResultInfo[]>.Factory.StartNew(() => ValidateProducts(current));
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
           var validatedSaleProducts = validationResults.Where(n => n.IsValid).Select(n => (SaleProduct)n.Entity).ToList();
           if (validatedSaleProducts.Any())
           {
               batchSize = Convert.ToInt32(0.2 * validatedSaleProducts.Count);
               var products =
                   validatedSaleProducts.OrderBy(p => p.ProductCode).Batch(batchSize).Select(x => x.ToList()).ToList();

               var saveTasksArray = new Task<Guid[]>[products.Count];
               try
               {
                   for (int i = 0; i < saveTasksArray.Length; i++)
                   {
                       var current = products.FirstOrDefault();
                       if (current != null && current.Any())
                       {
                           saveTasksArray[i] =
                               Task<Guid[]>.Factory.StartNew(() => SaveProducts(current));
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
   }
}
