using System;
using System.Configuration;
using System.IO;
using System.Linq;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.IOC;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Utility;
using Distributr_Middleware.WPF.Lib.Utils;
using NUnit.Framework;
using StructureMap;

namespace Distributr_Middleware.WPF.Lib.Tests
{
   public class TestMasterDataGenerator
    {
       [Test]
       public void Run()
       {
           string conn = ConfigurationManager.AppSettings["cokeconnectionstring"];
           ObjectFactory.Initialize(x => x.AddRegistry<DataRegistry>());
           using (var ctx = new CokeDataContext(conn))
           {
               GenerateTestTerritory(ctx);
               GenerateTestCountry(ctx);
               GenerateTestRegion(ctx);
               GenerateTestArea(ctx);
               GenerateTestProvince(ctx);
               GenerateTestDistrict(ctx);
               GenerateTestContactTypes(ctx);
               GenerateTestOutletCategory(ctx);
               GenerateTestOutletType(ctx);
               GenerateTestBanks(ctx);
               GenerateTestBankBranches(ctx);
               GenerateTestSuppliers(ctx);
               GenerateTestPricingTier(ctx);
               GenerateTestVATClass(ctx);
               GenerateTestProductType(ctx);
               GenerateTestProductBrand(ctx);
               GenerateTestProductFlavour(ctx);
               GenerateTestProductPackagingType(ctx);
               GenerateTestProductPackaging();
               GenerateProducts();
               GeneratePricings();
               GenerateDiscountGroups();
               GenerateProductGroupDiscounts();
               GenerateSaleValueDiscounts();
               GeneratePromotionDiscounts();
               GenerateTestOutlets();
               GenerateTestSalesman();
               GenerateTestRoutes();
               GenerateTestDistributor();
           }
       }

       private void GenerateTestDistributor()
       {
           var items = ObjectFactory.GetInstance<ICostCentreRepository>().GetAll()
               .OfType<Distributor>().Select(n => new
                                                      {
                                                          n.CostCentreCode,
                                                          n.Name,
                                                          Region=n.Region.Name,
                                                          n.PIN,
                                                          n.VatRegistrationNo,
                                                          n.PaybillNumber,
                                                          n.MerchantNumber,
                                                          Tier=n.ProductPricingTier !=null?(n.ProductPricingTier.Code??n.ProductPricingTier.Name):"",
                                                          n.Latitude,
                                                          n.Longitude
                                                          
                                                      }).ToList();

           DumpExportFilesAsync(items.ToCsv(), "Distributor.txt");
       }

       private void GenerateTestRoutes()
       {
           var items = ObjectFactory.GetInstance<IRouteRepository>().GetAll()
              .Take(10).Select(p => new
              {
                  p.Code,
                  p.Name,
                  Region =p.Region.Name,
              }).ToList();
           DumpExportFilesAsync(items.ToCsv(), "Route.txt");
       }

       private void GenerateTestSalesman()
       {
           var items = ObjectFactory.GetInstance<ICostCentreRepository>().GetAll()
               .OfType<DistributorSalesman>().Take(10).Select(p => new
               {
                   p.CostCentreCode,
                   p.Name,
                   Distributr = ObjectFactory.GetInstance<ICostCentreRepository>().GetById(p.ParentCostCentre.Id).CostCentreCode,
                   MobileNumber="0722000000"
                   }).ToList();
           DumpExportFilesAsync(items.ToCsv(), "DistributorSalesman.txt");
       }

       private void GenerateTestOutlets()
       {
           var items = ObjectFactory.GetInstance<ICostCentreRepository>().GetAll()
               .OfType<Outlet>().Take(10).Select(p => new
                                            {
                                                p.CostCentreCode,
                                                p.Name,
                                                Distributr = ObjectFactory.GetInstance<ICostCentreRepository>().GetById(p.ParentCostCentre.Id).CostCentreCode,
                                                Route = p.Route.Code ?? p.Route.Name,
                                                Category = p.OutletCategory.Code ?? p.OutletCategory.Name,
                                                Type = p.OutletType.Code ?? p.OutletType.Name,
                                                DiscountGroup =p.DiscountGroup!=null? p.DiscountGroup.Code ?? p.DiscountGroup.Name:"",
                                                PricingTierCode =p.OutletProductPricingTier!=null? p.OutletProductPricingTier.Code ?? p.OutletProductPricingTier.Code:"",
                                                ClassCode =p.VatClass!=null? p.VatClass.VatClass ?? p.VatClass.Name:"",
                                                specialPricingTierCode =p.SpecialPricingTier!=null? p.SpecialPricingTier.Code ?? p.SpecialPricingTier.Name:"",
                                                p.Latitude,
                                                p.Longitude
                                            }).ToList();
           DumpExportFilesAsync(items.ToCsv(), "Outlet.txt");

           
       }

       private void GeneratePromotionDiscounts()
       {
           var items =
               ObjectFactory.GetInstance<CokeDataContext>().tblPromotionDiscount.SelectMany(
                   n => n.tblPromotionDiscountItem).ToList();

           var temp = items.Select(n => new
                                            {
                                                Product =
                                            n.tblPromotionDiscount.tblProduct.ProductCode ??
                                            n.tblPromotionDiscount.tblProduct.Description,
                                                n.ParentProductQuantity,
                                                n.DiscountRate,
                                                n.EffectiveDate,
                                                n.EndDate,
                                                FreeProduct =
                                            ObjectFactory.GetInstance<CokeDataContext>().tblProduct.FirstOrDefault(
                                                p => p.id == n.FreeOfChargeProductRef) != null
                                                ? ObjectFactory.GetInstance<CokeDataContext>().tblProduct.FirstOrDefault
                                                      (p => p.id == n.FreeOfChargeProductRef).ProductCode
                                                : "",

                                            });
           DumpExportFilesAsync(temp.ToCsv(), "PromotionDiscount.txt");
       }
       private void GenerateSaleValueDiscounts()
       {
           var items =
               ObjectFactory.GetInstance<CokeDataContext>().tblSaleValueDiscount.SelectMany(
                   n => n.tblSaleValueDiscountItems).ToList();

           var temp = items.Select(n => new
                                            {
                                              Tier=n.tblSaleValueDiscount.tblPricingTier.Code??n.tblSaleValueDiscount.tblPricingTier.Name,
                                              n.SaleValue,
                                              n.EffectiveDate,
                                              n.EndDate,
                                              n.DiscountRate
                                            });
           DumpExportFilesAsync(temp.ToCsv(), "SaleValueDiscount.txt");
       }
       private void GenerateProductGroupDiscounts()
       {
           var items =
               ObjectFactory.GetInstance<CokeDataContext>().tblProductDiscountGroup.SelectMany(
                   p => p.tblProductDiscountGroupItem).ToList();
              
           
           var temp = items.Select(n => new
                                             {
                                                DiscountGroup= n.tblProductDiscountGroup.tblDiscountGroup.Code??n.tblProductDiscountGroup.tblDiscountGroup.Name,
                                                 Product=n.tblProduct.ProductCode??n.tblProduct.Description,
                                                 n.DiscountRate,
                                                 n.EffectiveDate,
                                                 n.EndDate
                                             });

           DumpExportFilesAsync(temp.ToCsv(), "ProductGroupDiscount.txt");
       }
       private void GenerateDiscountGroups()
       {
           var items = ObjectFactory.GetInstance<IDiscountGroupRepository>()
               .GetAll().Select(n => new
                                                   {
                                                       n.Code,
                                                       n.Name
                                                   });
           DumpExportFilesAsync(items.ToCsv(), "DiscountGroup.txt");
       }
       private void GeneratePricings()
       {
           var items = ObjectFactory.GetInstance<IProductPricingRepository>()
               .GetAll().OrderByDescending(p => p._DateLastUpdated)
               .Take(100).Select(n => new
                                          {
                                             Product=ObjectFactory.GetInstance<IProductRepository>().GetById(n.ProductRef.ProductId).ProductCode,
                                             Tier = n.Tier.Code ?? n.Tier.Name,
                                             n.CurrentSellingPrice,
                                             n.CurrentEffectiveDate,
                                             n.CurrentExFactory

                                          });
           DumpExportFilesAsync(items.ToCsv(), "Pricing.txt");
       }
       private void GenerateProducts()
       {
           var items = ObjectFactory.GetInstance<IProductRepository>()
               .GetAll().OfType<SaleProduct>().OrderByDescending(p=>p._DateLastUpdated)
               .Take(100).Select(n => new
                                         {
                                             n.ProductCode,
                                             n.Description,
                                             n.ExFactoryPrice,
                                             ProductType = n.ProductType.Code ?? n.ProductType.Name,
                                             Brand = n.Brand.Name ?? n.Brand.Code,
                                             Flavour = n.Flavour !=null?n.Flavour.Code ?? n.Flavour.Name:"",
                                             PackagingType = n.PackagingType !=null?n.PackagingType.Code ?? n.PackagingType.Name:"",
                                             VATClass=n.VATClass.VatClass??n.VATClass.Name,
                                             n.ReturnableType,
                                            ReturnableProduct= n.ReturnableProduct !=null?n.ReturnableProduct.ProductCode:""
                                            
                                         }).ToList();
           DumpExportFilesAsync(items.ToCsv(), "SaleProduct.txt");
               
       }
       private void GenerateTestProductPackaging()
       {

           var items = ObjectFactory.GetInstance<IProductPackagingRepository>()
               .GetAll().Take(10).Select(n => new
                                                  {
                                                      n.Code,
                                                      n.Name,
                                                      n.Description,
                                                      ProductCode =
                                                  n.ReturnableProductRef != null
                                                      ? ObjectFactory.GetInstance<IProductRepository>().GetById(
                                                          n.ReturnableProductRef.ProductId).ProductCode
                                                      : null
                                                  }).ToList();
           DumpExportFilesAsync(items.ToCsv(), "ProductPackaging.txt");
       }
       private void GenerateTestProductFlavour(CokeDataContext ctx)
       {
          
           var items = ObjectFactory.GetInstance<IProductFlavourRepository>()
               .GetAll().Take(10).Select(n => new {
                   n.Code,
                   n.Name,
                   n.Description,
                   Brand=n.ProductBrand.Name??n.ProductBrand.Code
                                                   }).ToList();
           DumpExportFilesAsync(items.ToCsv(), "ProductFlavour.txt");
       }
       private void GenerateTestProductPackagingType(CokeDataContext ctx)
       {
           var cTypes = ctx.tblProductPackagingType.Take(10).Select(n => new
           {
               n.code,
               n.name,
               n.description
           }).ToList();
           DumpExportFilesAsync(cTypes.ToCsv(), "ProductPackagingType.txt");
       }
       private void GenerateTestProductBrand(CokeDataContext ctx)
       {
           var cTypes = ctx.tblProductBrand.Take(10).Select(n => new
           {
               n.code,
               n.name,
               n.description,
               Supplier = n.tblSupplier.Code ?? n.tblSupplier.Name
           }).ToList();
           DumpExportFilesAsync(cTypes.ToCsv(), "ProductBrand.txt");
       }
       private void GenerateTestProductType(CokeDataContext ctx)
       {
           var cTypes = ctx.tblProductType.Take(10).Select(n => new
           {
               n.code,
               n.name,
               n.Description
           }).ToList();
           DumpExportFilesAsync(cTypes.ToCsv(), "ProductType.txt");
       }
       private void GenerateTestVATClass(CokeDataContext ctx)
       {
           var cTypes = ctx.tblVATClass.Take(10).Select(n => new
           {
               n.Name,
               n.Class,
               Rate=n.tblVATClassItem.OrderByDescending(p=>p.EffectiveDate).FirstOrDefault(p=>p.EffectiveDate<=DateTime.Now).Rate,
               EffectiveDate = n.tblVATClassItem.OrderByDescending(p => p.EffectiveDate).FirstOrDefault(p => p.EffectiveDate <= DateTime.Now).EffectiveDate
           }).ToList();

           DumpExportFilesAsync(cTypes.ToCsv(), "VATClass.txt");
       }
       private void GenerateTestBankBranches(CokeDataContext ctx)
       {
           var cTypes = ctx.tblBankBranch.Take(10).Select(n => new
           {
               n.Code,
               n.Name,
               n.Description,
               Bank = ctx.tblBank.FirstOrDefault(p => p.Id == n.BankId).Name ?? ctx.tblBank.FirstOrDefault(p => p.Id == n.BankId).Code
           }).ToList();
           DumpExportFilesAsync(cTypes.ToCsv(), "BankBranch.txt");
       }
       private void GenerateTestPricingTier(CokeDataContext ctx)
       {
           var cTypes = ctx.tblPricingTier.Take(10).Select(n => new
           {
               n.Code,
               n.Name,
               n.Description
           }).ToList();
           DumpExportFilesAsync(cTypes.ToCsv(), "PricingTier.txt");
       }
       private void GenerateTestSuppliers(CokeDataContext ctx)
       {
           var cTypes = ctx.tblSupplier.Take(10).Select(n => new
           {
               n.Code,
               n.Name,
               n.Description
           }).ToList();
           DumpExportFilesAsync(cTypes.ToCsv(), "Supplier.txt");
       }
       private void GenerateTestBanks(CokeDataContext ctx)
       {
           var cTypes = ctx.tblBank.Take(10).Select(n => new
           {
               n.Code,
               n.Name,
               n.Description
           }).ToList();
           DumpExportFilesAsync(cTypes.ToCsv(), "Bank.txt");
       }
       private void GenerateTestOutletType(CokeDataContext ctx)
       {
           var cTypes = ctx.tblOutletType.Take(10).Select(n => new
           {
               n.Code,
               n.Name
           }).ToList();
           DumpExportFilesAsync(cTypes.ToCsv(), "OutletType.txt");
       }
       private void GenerateTestOutletCategory(CokeDataContext ctx)
       {
           var cTypes = ctx.tblOutletCategory.Take(10).Select(n => new
           {
               n.Code,
               n.Name
           }).ToList();
           DumpExportFilesAsync(cTypes.ToCsv(), "OutletCategory.txt");
       }
       private void GenerateTestContactTypes(CokeDataContext ctx)
       {
           var cTypes = ctx.tblContactType.Take(10).Select(n => new
           {
               n.Code,n.Name,n.Description
           }).ToList();
           DumpExportFilesAsync(cTypes.ToCsv(), "ContactType.txt");
       }
       private void GenerateTestDistrict(CokeDataContext ctx)
       {
           var cTypes = ctx.tblDistrict.Take(10).Select(n => new
           {
               name = n.District,
               ProvinceName=ctx.tblProvince.FirstOrDefault(p=>p.Id==n.ProvinceId).Name,
           }).ToList();
           DumpExportFilesAsync(cTypes.ToCsv(), "District.txt");
       }
       private void GenerateTestProvince(CokeDataContext ctx)
       {
           var cTypes = ctx.tblProvince.Take(10).Select(n => new
           {
               name = n.Name,
               n.Description,
               country = ctx.tblCountry.FirstOrDefault(p => p.id == n.CountryId).Name,
           }).ToList();
           DumpExportFilesAsync(cTypes.ToCsv(), "Province.txt");
       }
       private void GenerateTestRegion(CokeDataContext ctx)
       {
           var cTypes = ctx.tblRegion.Take(10).Select(n => new
           {
                name=n.Name,
               n.Description,
                Country = ctx.tblCountry.FirstOrDefault(p => p.id == n.Country).Name,
           }).ToList();
           DumpExportFilesAsync(cTypes.ToCsv(), "Region.txt");
       }
       private void GenerateTestArea(CokeDataContext ctx)
       {
           var cTypes = ctx.tblArea.Take(10).Select(n => new
           {
               name = n.Name,
               n.Description,
               Region = ctx.tblRegion.FirstOrDefault(p => p.id == n.Region).Name,
           }).ToList();
           DumpExportFilesAsync(cTypes.ToCsv(), "Area.txt");
       }
       private void GenerateTestCountry(CokeDataContext ctx)
       {
           var cTypes = ctx.tblCountry.Take(10).Select(n => new
                                                                {
                                                                    n.Name,
                                                                    n.Code,
                                                                    n.Currency
                                                                }).ToList();
           DumpExportFilesAsync(cTypes.ToCsv(), "Country.txt");
       }
       private void GenerateTestTerritory(CokeDataContext ctx)
       {
           var cTypes = ctx.tblTerritory.Take(10).Select(n => new
                                                                  {
                                                                     n.Name,
                                                                  }).ToList();
           DumpExportFilesAsync(cTypes.ToCsv(), "Territory.txt");
       }
       private async void DumpExportFilesAsync(string orders, string entityname)
       {
           string selectedPath = Path.Combine(FileUtility.GetMiddleTestsDirectory(), entityname);
           if (string.IsNullOrEmpty(selectedPath))
           {
               return;
           }
           try
           {
               using (var fs = new FileStream(selectedPath, FileMode.OpenOrCreate))
               {
                   fs.Close();
                   using (var wr = new StreamWriter(selectedPath, false))
                   {
                       await wr.WriteLineAsync(orders);

                   }
               }
           }
           catch (IOException ex)
           {

           }
       }
    }
}
