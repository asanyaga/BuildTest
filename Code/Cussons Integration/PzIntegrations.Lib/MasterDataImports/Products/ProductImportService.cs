using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Master.SuppliersEntities;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Master.SuppliersRepositories;
using Distributr.Core.Utility;
using PzIntegrations.Lib.ImportEntities;
using StructureMap;

namespace PzIntegrations.Lib.MasterDataImports.Products
{
    public class ProductImportService : IProductImportService
    {
        List<ProductImport> ProductImports;
        private List<string> _failedImpoprts;
        public ProductImportService()
        {
            _failedImpoprts=new List<string>();
            ProductImports=new List<ProductImport>();
        }

        public async Task<IEnumerable<ProductImport>> Import(string path)
        {
            return await Task.Factory.StartNew(() =>
            {
              
                var tempFolder =
                    Path.Combine(FileUtility.GetApplicationTempFolder(), Path.GetFileName(path));
                if (File.Exists(tempFolder))
                    File.Delete(tempFolder);
                File.Copy(path, tempFolder);

                using (var parser = new Microsoft.VisualBasic.FileIO.TextFieldParser(tempFolder))
                {
                    parser.SetDelimiters("\t");
                    string[] currentRow = null;

                    while (!parser.EndOfData)
                    {
                        currentRow = parser.ReadFields();
                        if (currentRow != null && currentRow.Length > 0)
                        {
                            ProductImports.Add(MapProductImport(currentRow));
                        }
                    }

                }
                File.Delete(tempFolder);
                return ProductImports;
            });
        }

        private ProductImport MapProductImport(string[] dataRow)
        {
            var spstring = SetFieldValue(dataRow, 5);
            var expricestring = SetFieldValue(dataRow, 6);
            decimal sp = 0m;
            decimal exprice = 0m;
            try
            {
                if(!string.IsNullOrEmpty(spstring))
                    sp = Convert.ToDecimal(spstring);

            }catch
            {
                sp = 0m;
            }

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
                               BrandCode = SetFieldValue(dataRow, 1),
                               ProductCode = SetFieldValue(dataRow, 2),
                               Description = SetFieldValue(dataRow, 3),
                               Status = SetFieldValue(dataRow, 4),
                               SellingPrice = sp,
                               ExFactoryPrice = exprice

                           };

           

        }

        string SetFieldValue(string[] dataRow, int index)
        {
            index = index - 1;
            return (dataRow.Length - 1 < index || string.IsNullOrEmpty(dataRow[index])) ? "" : dataRow[index];
        }

        public IList<ImportValidationResultInfo> ValidateAndSave(List<ProductImport> entities = null)
        {
            int batchSize = Convert.ToInt32(0.2 * entities.Count);
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

            #region Construct Pricings

           batchSize = Convert.ToInt32(0.2 * entities.Count);
           productImports = entities.OrderBy(p => p.ProductCode).Batch(batchSize).Select(x => x.ToList()).ToList();
           var pricingTaskArray = new Task<ProductPricing[]>[productImports.Count];
           var pricingResults = new List<ProductPricing>();
            try
            {
                for (int i = 0; i < pricingTaskArray.Length; i++)
                {
                    var current = productImports.FirstOrDefault();
                    if (current != null && current.Any())
                    {
                        pricingTaskArray[i] = Task<ProductPricing[]>.Factory.StartNew(() => ConstructPricings(current));
                        productImports.Remove(current);
                    }
                }

                foreach (var result in pricingTaskArray.Select(n => n.Result).ToList())
                {
                    pricingResults.AddRange(result);
                }
            }
            catch (AggregateException ex)
            {
            }

            #endregion

            #region validate pricings
            if (pricingResults.Any())
            {
                batchSize = Convert.ToInt32(0.2 * results.Count);
                var productPricings = pricingResults.OrderBy(p => p.CurrentEffectiveDate).Batch(batchSize).Select(x => x.ToList()).ToList();
                var validationTaskArray = new Task<ImportValidationResultInfo[]>[productPricings.Count];


                try
                {
                    for (int i = 0; i < validationTaskArray.Length; i++)
                    {
                        var current = productPricings.FirstOrDefault();
                        if (current != null && current.Any())
                        {
                            validationTaskArray[i] =
                                Task<ImportValidationResultInfo[]>.Factory.StartNew(() => ValidatePricings(current));
                            productPricings.Remove(current);
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

            #region Save pricings
            var validatedPricings = validationResults.Where(n => n.Entity is ProductPricing && n.IsValid).Select(n => (ProductPricing)n.Entity).ToList();
            if (validatedPricings.Any())
            {
                batchSize = Convert.ToInt32(0.2 * validatedPricings.Count);
                var pricings =
                    validatedPricings.OrderBy(p => p.CurrentEffectiveDate).Batch(batchSize).Select(x => x.ToList()).ToList();

                var saveTasksArray = new Task<Guid[]>[pricings.Count];
                try
                {
                    for (int i = 0; i < saveTasksArray.Length; i++)
                    {
                        var current = pricings.FirstOrDefault();
                        if (current != null && current.Any())
                        {
                            saveTasksArray[i] =
                                Task<Guid[]>.Factory.StartNew(() => SaveProductPricings(current));
                            pricings.Remove(current);
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
                        Entity = product
                    }).ToArray();
        }

        private SaleProduct[] ConstructProducts(List<ProductImport> entities)
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

            foreach (var entity in entities)
            {
               var product = ObjectFactory.GetInstance<IProductRepository>().GetByCode(entity.ProductCode) as SaleProduct;
                    
                bool isNew = false;
                if(product==null)
                {
                    isNew = true;
                    product= new SaleProduct(Guid.NewGuid());
                }

                var brand = ObjectFactory.GetInstance<IProductBrandRepository>().GetAll(true)
                        .FirstOrDefault(
                            p => p.Code != null && (p.Code.Equals(entity.BrandCode, StringComparison.CurrentCultureIgnoreCase)) ||
                            p.Name != null && p.Name.Equals(entity.BrandCode, StringComparison.CurrentCultureIgnoreCase));

                if (string.IsNullOrEmpty(entity.BrandCode))
                    brand = ObjectFactory.GetInstance<IProductBrandRepository>().GetAll(true).FirstOrDefault(p => p.Code != null && (p.Code.Equals("default", StringComparison.CurrentCultureIgnoreCase) || p.Name != null && p.Name.Equals("default", StringComparison.CurrentCultureIgnoreCase)));


                if (brand == null)
                {
                    brand = ObjectFactory.GetInstance<IProductBrandRepository>().GetAll(true).FirstOrDefault(p => p.Code != null && p.Code == "default") ??
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

                if (product.ProductType == null)
                {
                    var productType = ObjectFactory.GetInstance<IProductTypeRepository>().GetAll(true).FirstOrDefault(
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
                            ObjectFactory.GetInstance<IProductTypeRepository>().Save(productType);
                        }
                        catch
                        {
                        }
                    }
                    product.ProductType = productType;
                }



                if (product.PackagingType == null)
                {
                    var packagingType = ObjectFactory.GetInstance<IProductPackagingTypeRepository>().GetAll(true).FirstOrDefault(
                        p => p.Code != null &&
                        p.Code.Equals("default", StringComparison.CurrentCultureIgnoreCase) ||
                        p.Name != null &&
                        p.Name.Equals("default", StringComparison.CurrentCultureIgnoreCase));
                    if (packagingType == null)
                    {
                        packagingType = new ProductPackagingType(Guid.NewGuid())
                        {
                            Name = "default",
                            Code =
                                "default",
                            Description = "default"
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
                }


                if (product.Packaging == null)
                {
                    var packaging =
                        ObjectFactory.GetInstance<IProductPackagingRepository>().GetAll(true).FirstOrDefault(
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
                            ObjectFactory.GetInstance<IProductPackagingRepository>().Save(packaging);
                        }
                        catch
                        {

                        }
                    }
                    product.Packaging = packaging;
                }

                if (product.VATClass == null)
                {
                    var productVat = ObjectFactory.GetInstance<IVATClassRepository>().GetAll(true).FirstOrDefault(
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
                            ObjectFactory.GetInstance<IVATClassRepository>().Save(productVat);
                        }
                        catch
                        {
                        }

                    }
                    product.VATClass = productVat;
                }
                if (product.Flavour == null)
                {
                    var flavor = ObjectFactory.GetInstance<IProductFlavourRepository>().GetAll(true).FirstOrDefault(
                            p => p.Name != null &&
                            p.Name == "defaultflavor" || p.Code
                            != null &&
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
                            ObjectFactory.GetInstance<IProductFlavourRepository>().Save(flavor);
                        }
                        catch
                        {
                        }

                    }
                    product.Flavour = flavor;
                }

                product.ProductCode = entity.ProductCode;
                product._Status = EntityStatus.Active; 
                product.ExFactoryPrice = entity.ExFactoryPrice;
                product.Description = entity.Description.Trim();

                if (isNew || HasProductChanged(product))
                    temp.Add(product);
            }
            return temp.ToArray();

        }

        private bool HasProductChanged(SaleProduct item)
        {
            using (var context = ObjectFactory.Container.GetNestedContainer())
            {
                var product = context.GetInstance<IProductRepository>().GetById(item.Id);
                if (product == null) return true;
                var desc = product.Description.Trim().ToLower();
                var itemdesc =item.Description.Trim().ToLower();
                var productprice = Math.Round(product.ExFactoryPrice, 2);
                var itemprice=Math.Round(product.ExFactoryPrice, 2);

                return (product.Id != item.Id) || (product.Brand.Id != item.Brand.Id) || (productprice != itemprice)
                       || (desc != itemdesc)||(product.ProductCode.ToLower()!=item.ProductCode.ToLower());
            }

        }

        #region Pricings
        private bool HasProductPricingChanged(ProductPricing pricing)
        {
            using (var context = ObjectFactory.Container.GetNestedContainer())
            {
                var item = context.GetInstance<IProductPricingRepository>().GetById(pricing.Id);
                if (item == null) return true;
               
                var currentPrice = Math.Round(item.CurrentSellingPrice, 2);
                var currentfactory = Math.Round(item.CurrentExFactory, 2);

                var pPrice = Math.Round(pricing.CurrentSellingPrice, 2);
                var pExF = Math.Round(pricing.CurrentExFactory, 2);
                if ((currentPrice != pPrice) || (currentfactory != pExF))
                    return true;

                return false;
            }
        }
        private Guid[] SaveProductPricings(IEnumerable<ProductPricing> pricings)
        {
            return (from pricing in pricings
                    let res = ObjectFactory.GetInstance<IProductPricingRepository>().Save(pricing, true)
                    select new Guid(res.ToString())
                    {

                    }).ToArray();
        }
        private ImportValidationResultInfo[] ValidatePricings(List<ProductPricing> current)
        {
            return (from pricing in current
                    let res = ObjectFactory.GetInstance<IProductPricingRepository>().Validate(pricing)
                    select new ImportValidationResultInfo()
                               {
                                   Results = res.Results,
                                   Entity = pricing
                               }).ToArray();
        }

        private ProductPricing[] ConstructPricings(IEnumerable<ProductImport> entities)
        {
          
                var newpricing = new List<ProductPricing>();
                var exisitingPricing = ObjectFactory.GetInstance<IProductPricingRepository>().GetAll(true).ToList();
                var pricingTiers = ObjectFactory.GetInstance<IProductPricingTierRepository>().GetAll(true).ToList();
               
                foreach (var entity in entities)
                {
                    if (!string.IsNullOrEmpty(entity.ProductCode))
                    {

                        var product =ObjectFactory.GetInstance<CokeDataContext>().tblProduct.FirstOrDefault(
                            p => p.ProductCode.ToLower() == entity.ProductCode.ToLower());

                        if (product != null)
                        {
                            var pricingExist =
                                exisitingPricing.FirstOrDefault(
                                    p => p.ProductRef.ProductId == product.id && p.Tier.Code == "KES");
                            bool isNew = false;
                            if (pricingExist == null)
                            {
                                
                                var tire = pricingTiers
                                               .FirstOrDefault(
                                                   p =>
                                                   p.Code.Equals("KES",
                                                                 StringComparison.
                                                                     CurrentCultureIgnoreCase)) ??
                                           pricingTiers.FirstOrDefault(
                                               p => p.Code != null && p.Name == "DefaultPricingTire");

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
                                        ObjectFactory.GetInstance<IProductPricingTierRepository>().Save(tire);
                                    }
                                    catch
                                    {
                                    }
                                }

                                pricingExist = new ProductPricing(Guid.NewGuid())
                                                   {
                                                       ProductRef =
                                                           new ProductRef() {ProductId = product.id},
                                                       Tier = tire
                                                   };
                                isNew = true;

                            }
                            if(isNew)
                            {
                                pricingExist.ProductPricingItems
                                .Add(new ProductPricing.ProductPricingItem(Guid.NewGuid())
                                {
                                    EffectiveDate = DateTime.Now,
                                    ExFactoryRate = entity.ExFactoryPrice,
                                    SellingPrice = entity.SellingPrice
                                });
                                pricingExist._Status = EntityStatus.Active;
                                newpricing.Add(pricingExist);
                            }
                            else if (HasProductPricingChanged(pricingExist))
                            {
                                pricingExist.ProductPricingItems
                                .Add(new ProductPricing.ProductPricingItem(Guid.NewGuid())
                                {
                                    EffectiveDate = DateTime.Now,
                                    ExFactoryRate = entity.ExFactoryPrice,
                                    SellingPrice = entity.SellingPrice
                                });
                                pricingExist._Status = EntityStatus.Active;
                                newpricing.Add(pricingExist);
                                
                            }

                        }
                        else
                        {
                            if (
                                !_failedImpoprts.Any(
                                    p => p.Equals(entity.ProductCode, StringComparison.CurrentCultureIgnoreCase)))
                            {
                                _failedImpoprts.Add(entity.ProductCode);
                            }
                        }
                    }
                    else
                    {
                        _failedImpoprts.Add(entity.Description);
                    }
                }
                return newpricing.ToArray();

         
        }

      
        #endregion
    }
}
