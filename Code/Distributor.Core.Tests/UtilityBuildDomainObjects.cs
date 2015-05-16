using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CompetitorManagement;
using Distributr.Core.Domain.Master.CoolerEntities;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using System.Reflection;
using FizzWare.NBuilder;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Domain.Master.ChannelPackagings;
using Distributr.Core.Domain.Master.DistributorTargetEntities;
using Distributr.Core.Domain.Master.ReOrdeLevelEntities;
using Distributr.Core.Domain.Master.BankEntities;
using Distributr.Core.Domain.Master.SuppliersEntities;

namespace Distributr.Core.Tests
{
    public class UtilityBuildDomainObjects
    {
        public  static ProductBrand BuildProductBrand(Guid id)
        {
            var pb = Builder<ProductBrand>.CreateNew()
                //.WithConstructorArgs(1, DateTime.Now, DateTime.Now, true)
                .WithConstructor(() => new ProductBrand(id, DateTime.Now, DateTime.Now, EntityStatus.Active))
                .With(n=>n.Name="Product1")
                .With(n=>n.Supplier=BuildSupplier())
                .Build();
            return pb;
        }

        public static ProductPricingTier BuildProductPricingTier()
        {
            var ppt = Builder<ProductPricingTier>.CreateNew()
                .WithConstructor(() => new ProductPricingTier(Guid.NewGuid(), DateTime.Now, DateTime.Now, EntityStatus.Active))
                
                .Build();
            return ppt;
        }
        public static ProductFlavour BuildProductFlavour()
        {
            var pf = Builder<ProductFlavour>.CreateNew()
                  .WithConstructor(() => new ProductFlavour(Guid.NewGuid(), DateTime.Now, DateTime.Now, EntityStatus.Active))
                  .With(n => n.ProductBrand = BuildProductBrand(Guid.NewGuid()))
                .Build();
            return pf;
        }
        public static SaleProduct BuildSaleProduct(ReturnableType returnableType)
        {
            SaleProduct p = Builder<SaleProduct>.CreateNew()
                 .WithConstructor(() => new SaleProduct(Guid.NewGuid(), DateTime.Now, DateTime.Now, EntityStatus.Active, new List<ProductPricing> { BuildProductPricing(Guid.NewGuid()) }))
                 .With(n => n.Flavour = BuildProductFlavour())
                 .With(n => n.Brand = BuildProductBrand(Guid.NewGuid()))
                 .With(n => n.ProductType = BuildProductType())
                 .With(n => n.Packaging = BuildProductPackaging())
                 .With(n => n.PackagingType = BuildProductPackagingType())
                 .With(n=>n.ReturnableType=returnableType)
                 .With(n=>n.VATClass=BuildVatClass())
                 .With(n=>n.ReturnableProduct=BuildReturnableProduct(ReturnableType.GenericReturnable))
                .Build();
            return p;
        }
        public static ProductPackaging BuildProductPackaging()
        {
            var pp = Builder<ProductPackaging>.CreateNew()
                   .WithConstructor(() => new ProductPackaging(Guid.NewGuid(), DateTime.Now, DateTime.Now, EntityStatus.Active))
                .Build();
            return pp;
        }

        public static ProductPackagingType BuildProductPackagingType()
        {
            var ppt = Builder<ProductPackagingType>.CreateNew()
                   .WithConstructor(() => new ProductPackagingType(Guid.NewGuid(), DateTime.Now, DateTime.Now, EntityStatus.Active))
                .Build();
            return ppt;
        }

        public static ProductPricing BuildProductPricing(Guid productId)
        {
            var ppi1 = BuildProductPricingItem(Guid.NewGuid());
            var ppi2 = BuildProductPricingItem(Guid.NewGuid());

            var pp = Builder<ProductPricing>.CreateNew()
                   .WithConstructor(() => new ProductPricing(Guid.NewGuid(), DateTime.Now, DateTime.Now, EntityStatus.Active, new List<ProductPricing.ProductPricingItem> { ppi1, ppi2 }))
                  .With(n => n.Tier = BuildProductPricingTier())
                  .With(n => n.ProductRef = new ProductRef { ProductId = productId })                  
                  .Build();
            return pp;
        }

        public static ProductPricing.ProductPricingItem BuildProductPricingItem(Guid id)
        {
            var ppi = Builder<ProductPricing.ProductPricingItem>.CreateNew()
                  .WithConstructor(() => new ProductPricing.ProductPricingItem(id, DateTime.Now, DateTime.Now, EntityStatus.Active))
                .Build();
            return ppi;
        }

        public static ProductType BuildProductType()
        {
            var pt = Builder<ProductType>.CreateNew()
                 .WithConstructor(() => new ProductType(Guid.NewGuid(), DateTime.Now, DateTime.Now, EntityStatus.Active))
                .Build();
            return pt;
        }
        public static VATClass BuildVatClass()
        {
            var vci1 = BuildVatClassItem(Guid.NewGuid());
            var vci2 = BuildVatClassItem(Guid.NewGuid());

            var vc = Builder<VATClass>.CreateNew()
                  .WithConstructor(() => new VATClass(Guid.NewGuid(), DateTime.Now, DateTime.Now, EntityStatus.Active, new List<VATClass.VATClassItem> { vci1, vci2 }))
                .Build();
            return vc;
        }

        public static VATClass.VATClassItem BuildVatClassItem(Guid id)
        {
            var vci = Builder<VATClass.VATClassItem>.CreateNew()
                  .WithConstructor(() => new VATClass.VATClassItem(id, DateTime.Now, DateTime.Now, EntityStatus.Active))
                 .Build();
            return vci;
        }

        public static Region BuildRegion()
        {
            Region r = Builder<Region>.CreateNew()
                 .WithConstructor(() => new Region(Guid.NewGuid(), DateTime.Now, DateTime.Now, EntityStatus.Active))
                .With(n => n.Country = BuildCountry())
                 .Build();
            return r;
        }

        public static Area BuildArea()
        {
            Area a = Builder<Area>.CreateNew()
                 .WithConstructor(() => new Area(Guid.NewGuid(), DateTime.Now, DateTime.Now, EntityStatus.Active))
                .With(n => n.region = BuildRegion())
                 .Build();
            return a;
        }

        public static Country BuildCountry()
        {
            Country c = Builder<Country>.CreateNew()
                  .WithConstructor(() => new Country(Guid.NewGuid(), DateTime.Now, DateTime.Now, EntityStatus.Active))
                 
                 .Build();
            return c;
        }

        public static User BuildUser( Guid id,UserType userType)
        {
            User u = Builder<User>.CreateNew()
                 .WithConstructor(() => new User(id, DateTime.Now, DateTime.Now, EntityStatus.Active))
                .With(n => n.UserType = userType)
                .Build();
            return u;
        }

        public static Contact BuildContact(Guid contactId, Guid costcentreId)
        {
            Contact c = Builder<Contact>.CreateNew()
                 .WithConstructor(() => new Contact(contactId, DateTime.Now, DateTime.Now, EntityStatus.Active))
                .With(n=>n.ContactOwnerMasterId=costcentreId)
               
                .With(n=>n.ContactType=BuildContactType())
                .With(n=>n.ContactOwnerType=ContactOwnerType.Distributor)
                .Build();
            return c;
        }

        public static Producer BuildProducer()
        {
            Producer p = Builder<Producer>.CreateNew()
                 .WithConstructor(() => new Producer(Guid.NewGuid(), DateTime.Now, DateTime.Now, EntityStatus.Active))
                .With(n => n.Contact = new List<Contact> { BuildContact(Guid.NewGuid(), Guid.NewGuid()), BuildContact(Guid.NewGuid(), Guid.NewGuid()) })
                .With( n=> n.CostCentreType = CostCentreType.Producer)                
                .Build();
            return p;
        }
        //public static void SetPrivateProperty(object obj, string propertyName, object value)
        //{
        //    PropertyInfo prop = obj.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        //    if (null != prop && prop.CanWrite)
        //    {
        //        prop.SetValue(obj, value, null);
        //    }
        //}

        public static Distributor BuildDistributor()
        {
            Distributor d = Builder<Distributor>.CreateNew()
                 .WithConstructor(() => new Distributor(Guid.NewGuid(), DateTime.Now, DateTime.Now, EntityStatus.Active))
                .With(n => n.Region = BuildRegion())
                 .With(n => n.ASM = BuildUser(Guid.NewGuid(), UserType.ASM))
                 .With(n => n.SalesRep = BuildUser(Guid.NewGuid(), UserType.SalesRep))
                 .With(n => n.Surveyor = BuildUser(Guid.NewGuid(), UserType.Surveyor))
                .With(n => n.Contact = new List<Contact> { BuildContact(Guid.NewGuid(), Guid.NewGuid()), BuildContact(Guid.NewGuid(), Guid.NewGuid()) })
                .With(n => n.CostCentreType = CostCentreType.Distributor)
                .With(n =>n.ParentCostCentre = new CostCentreRef { Id = BuildProducer().Id })
                .With(n=>n.ProductPricingTier=BuildProductPricingTier())
                .Build();
         

            return d;
        }
        public static DistributorSalesman BuildDistributorSalesman()
        {
            DistributorSalesman d = Builder<DistributorSalesman>.CreateNew()
                 .WithConstructor(() => new DistributorSalesman(Guid.NewGuid(), DateTime.Now, DateTime.Now, EntityStatus.Active))
                .With(n => n.Contact = new List<Contact> { BuildContact(Guid.NewGuid(), Guid.NewGuid()), BuildContact(Guid.NewGuid(), Guid.NewGuid()) })
                
                .With(n => n.CostCentreType = CostCentreType.DistributorSalesman)
                .With(n => n.ParentCostCentre = new CostCentreRef { Id = BuildDistributor().Id })
                .Build();
            d.Routes.Add(BuildSalesmanRoute());

            return d;
        }
        public static SalesmanRoute BuildSalesmanRoute()
        {
            SalesmanRoute oc = Builder<SalesmanRoute>.CreateNew()
                 .WithConstructor(() => new SalesmanRoute(Guid.NewGuid(), DateTime.Now, DateTime.Now, EntityStatus.Active))
                  .With(n => n.DistributorSalesmanRef = new CostCentreRef { Id = Guid.NewGuid() })
                   .With(n => n.Route = BuildRoute())
                .Build();
            return oc;
        }
        public static OutletCategory BuildOutletCategory()
        {
            OutletCategory oc = Builder<OutletCategory>.CreateNew()
                 .WithConstructor(() => new OutletCategory(Guid.NewGuid(), DateTime.Now, DateTime.Now, EntityStatus.Active))
                .Build();
            return oc;
        }

        public static OutletType BuildOutletType()
        {
            OutletType ot = Builder<OutletType>.CreateNew()
                           .WithConstructor(() => new OutletType(Guid.NewGuid(), DateTime.Now, DateTime.Now, EntityStatus.Active))
           .Build();
            return ot;
        }

        public static Route BuildRoute()
        {
            Route r = Builder<Route>.CreateNew()
                 .WithConstructor(() => new Route(Guid.NewGuid(), DateTime.Now, DateTime.Now, EntityStatus.Active))
                .With(n => n.Region = BuildRegion())
                .Build();

            return r;
        }

        public static Outlet BuildOutet()
        {
            Outlet o = Builder<Outlet>.CreateNew()
                 .WithConstructor(() => new Outlet(Guid.NewGuid(), DateTime.Now, DateTime.Now, EntityStatus.Active))
                .With(n => n.Route = BuildRoute())
                .With(n => n.OutletCategory = BuildOutletCategory())
                .With(n => n.OutletType = BuildOutletType())
                .With(n => n.Contact = new List<Contact> { BuildContact(Guid.NewGuid(), Guid.NewGuid()), BuildContact(Guid.NewGuid(), Guid.NewGuid()) })
                .With(n=> n.CostCentreType  = CostCentreType.Outlet)
                .With(n => n.ParentCostCentre = new CostCentreRef { Id = BuildDistributor().Id })
                .With(n=>n.VatClass=BuildVatClass())
                .Build();
            return o;
        }
        public static SocioEconomicStatus BuildSocioEconomicStatus()
        {
            SocioEconomicStatus s = Builder<SocioEconomicStatus>.CreateNew()
                 .WithConstructor(() => new SocioEconomicStatus(Guid.NewGuid(), DateTime.Now, DateTime.Now, EntityStatus.Active))
                .Build();
            return s;
        }

        public static Territory BuildTerritory()
        {
            Territory t = Builder<Territory>.CreateNew()
                 .WithConstructor(() => new Territory(Guid.NewGuid(), DateTime.Now, DateTime.Now, EntityStatus.Active))
                .Build();
            return t;
        }

        public static ReturnableProduct BuildReturnableProduct(ReturnableType returnableType)
        {
            ReturnableProduct r = Builder<ReturnableProduct>.CreateNew()
                  .WithConstructor(() => new ReturnableProduct(Guid.NewGuid(), DateTime.Now, DateTime.Now, EntityStatus.Active, new List<ProductPricing> { BuildProductPricing(Guid.NewGuid()) }))
                 .With(n => n.Brand = BuildProductBrand(Guid.NewGuid()))
                 .With(n=>n.Flavour=BuildProductFlavour())
                 .With(n => n.Packaging = BuildProductPackaging())
                 .With(n => n.PackagingType = BuildProductPackagingType())
                 .With(n=>n.ReturnableType=returnableType)
                 .With(n=>n.VATClass=BuildVatClass())
                 .With(n => n.ReturnAbleProduct = BuildRetProduct(ReturnableType.GenericReturnable))
                .Build();
            return r;
        }
        public static ReturnableProduct BuildRetProduct(ReturnableType returnableType)
        { 
         ReturnableProduct rp = Builder<ReturnableProduct>.CreateNew()
                  .WithConstructor(() => new ReturnableProduct(Guid.NewGuid(), DateTime.Now, DateTime.Now, EntityStatus.Active, new List<ProductPricing> { BuildProductPricing(Guid.NewGuid()) }))
                 .With(n => n.Brand = BuildProductBrand(Guid.NewGuid()))
                 .With(n=>n.Flavour=BuildProductFlavour())
                 .With(n => n.Packaging = BuildProductPackaging())
                 .With(n => n.PackagingType = BuildProductPackagingType())
                 .With(n=>n.ReturnableType=returnableType)
                 .With(n=>n.VATClass=BuildVatClass())
                 .Build();
         return rp;
        }

        public static ConsolidatedProduct BuildConsolidatedProduct(ReturnableType returnableType)
        {
            ConsolidatedProduct c = Builder<ConsolidatedProduct>.CreateNew()
                 .WithConstructor(() => new ConsolidatedProduct(Guid.NewGuid(), DateTime.Now, DateTime.Now, EntityStatus.Active, new List<ProductPricing> { BuildProductPricing(Guid.NewGuid()) },
                new List<ConsolidatedProduct.ProductDetail> { BuildConsolidatedProductProductDetail() }
                ))
                 .With(n => n.Brand = BuildProductBrand(Guid.NewGuid()))
                 .With(n => n.Packaging = BuildProductPackaging())
                 .With(n => n.PackagingType = BuildProductPackagingType())
                 .With(n=>n.ReturnableType=returnableType)
                .Build();
            return c;
        }

        public static ConsolidatedProduct.ProductDetail BuildConsolidatedProductProductDetail()
        {
            return new ConsolidatedProduct.ProductDetail
            {
                Product = BuildSaleProduct(ReturnableType.GenericReturnable),
                QuantityPerConsolidatedProduct = 2
            };
        }

        //public static InventoryAdjustmentNote BuildInventoryAdjustmentNote()
        //{
        //    Guid DocumentId = Guid.NewGuid();
        //    InventoryAdjustmentNote c = Builder<InventoryAdjustmentNote>.CreateNew()
        //         .WithConstructor(() => new InventoryAdjustmentNote( DocumentId, "xxs", 
        //        BuildDistributor(), 10,
        //        BuildUser(88, UserType.DistributorSalesman), 
        //        DateTime.Now, BuildDistributor(), DocumentStatus.New,
        //        BuildInvemtoryAdjustmentLineItem(DocumentId)                
        //        )                 )
        //        .Build();
        //    return c;
        //}

        //private static InventoryAdjustmentNoteLineItem BuildInvemtoryAdjustmentLineItem(Guid DocumentId)
        //{
        //    InventoryAdjustmentNoteLineItem p = Builder<InventoryAdjustmentNoteLineItem>.CreateNew()
        //        .WithConstructor(() => new InventoryAdjustmentNoteLineItem(Guid.NewGuid()))
        //        .With(n => n.Actual = 10)
        //        .With(n => n.Qty = 20)
        //        .With(n => n.Product = BuildSaleProduct())
        //        .Build();
        //    return p;
        //}
        //private static ChannelPackaging BuildChannelPackaging()
        //{
        //    ChannelPackaging p = Builder<ChannelPackaging>.CreateNew()
        //        .WithConstructor(() => new ChannelPackaging()
        //        .With(n => n.Actual = 10)
        //        .With(n => n.Qty = 20)
        //        .With(n => n.Product = BuildSaleProduct())
        //        .Build();
        //    return p;
        //}





       
        public static SaleValueDiscount BuildSaleValueDisCount()
        {
            var vci1 = BuildSaleValueDisCountItem(Guid.NewGuid());
            var vci2 = BuildSaleValueDisCountItem(Guid.NewGuid());

            var vc = Builder<SaleValueDiscount>.CreateNew()
                 .With(n => n.Tier = BuildProductPricingTier())
                 .WithConstructor(() => new SaleValueDiscount(Guid.NewGuid(), DateTime.Now, DateTime.Now, EntityStatus.Active, new List<SaleValueDiscount.SaleValueDiscountItem> { vci1, vci2 }))
                .Build();
            return vc;
        }

        public static SaleValueDiscount.SaleValueDiscountItem BuildSaleValueDisCountItem(Guid id)
        {
            var vci = Builder<SaleValueDiscount.SaleValueDiscountItem>.CreateNew()
                 .WithConstructor(() => new SaleValueDiscount.SaleValueDiscountItem(id, DateTime.Now, DateTime.Now, EntityStatus.Active))
                 .With(n=>n.DiscountThreshold=1000)
                 .With(n=>n.EffectiveDate= DateTime.Now)
                 .With(n=>n.DiscountValue= 20)
                 .Build();
            return vci;
        }

        public static PromotionDiscount BuildPromotionDiscount()
        {
            var vci1 = BuildPromotionDiscountItem(Guid.NewGuid());
            var vci2 = BuildPromotionDiscountItem(Guid.NewGuid());

            var vc = Builder<PromotionDiscount>.CreateNew()
                 .With(n => n.ProductRef = new ProductRef { ProductId = BuildSaleProduct(ReturnableType.GenericReturnable).Id })
                 .WithConstructor(() => new PromotionDiscount(Guid.NewGuid(), DateTime.Now, DateTime.Now, EntityStatus.Active, new List<PromotionDiscount.PromotionDiscountItem> { vci1, vci2 }))
                .Build();
            return vc;
        }

        public static PromotionDiscount.PromotionDiscountItem BuildPromotionDiscountItem(Guid id)
        {
            var vci = Builder<PromotionDiscount.PromotionDiscountItem>.CreateNew()
                 .WithConstructor(() => new PromotionDiscount.PromotionDiscountItem(id, DateTime.Now, DateTime.Now, EntityStatus.Active))
                 .With(n => n.FreeOfChargeProduct = new ProductRef { ProductId = BuildSaleProduct(ReturnableType.GenericReturnable).Id })
                 .With(n => n.EffectiveDate = DateTime.Now)
                 .With(n => n.FreeOfChargeQuantity = 20)
                 .With(n=>n.ParentProductQuantity=400)
                 .Build();
            return vci;
        }

        public static ProductGroupDiscount BuildProductGroupDiscount()
        {
            return null;
        }

       

        public static CertainValueCertainProductDiscount BuildCertainValueCertainProductDiscount()
        {
            var vci1 = BuildCertainValueCertainProductDiscountItem(Guid.NewGuid());
            var vci2 = BuildCertainValueCertainProductDiscountItem(Guid.NewGuid());

            var vc = Builder<CertainValueCertainProductDiscount>.CreateNew()
                 .With(n => n.InitialValue = 1000)
                 .WithConstructor(() => new CertainValueCertainProductDiscount(Guid.NewGuid(), DateTime.Now, DateTime.Now, EntityStatus.Active, new List<CertainValueCertainProductDiscount.CertainValueCertainProductDiscountItem> { vci1, vci2 }))
                .Build();
            return vc;
        }

        public static CertainValueCertainProductDiscount.CertainValueCertainProductDiscountItem BuildCertainValueCertainProductDiscountItem(Guid id)
        {
            var vci = Builder<CertainValueCertainProductDiscount.CertainValueCertainProductDiscountItem>.CreateNew()
                 .WithConstructor(() => new CertainValueCertainProductDiscount.CertainValueCertainProductDiscountItem(id, DateTime.Now, DateTime.Now, EntityStatus.Active))
                 .With(n => n.Product = new ProductRef { ProductId = BuildSaleProduct(ReturnableType.GenericReturnable).Id })
                 .With(n => n.EffectiveDate = DateTime.Now)
                 .With(n => n.CertainValue = 4000)
                 .With(n=>n.Quantity=2000)
                 .Build();
            return vci;
        }

        public static ProductDiscount BuildProductDiscount()
        {
            var vci1 = ProductDiscountItem(Guid.NewGuid());
            var vci2 = ProductDiscountItem(Guid.NewGuid());

            var vc = Builder<ProductDiscount>.CreateNew()
                 .WithConstructor(() => new ProductDiscount(Guid.NewGuid(), DateTime.Now, DateTime.Now, EntityStatus.Active, new List<ProductDiscount.ProductDiscountItem> { vci1, vci2 }))
                 .With(n => n.Tier = BuildProductPricingTier())
                 .With(n => n.ProductRef = new ProductRef{ProductId= BuildSaleProduct(ReturnableType.GenericReturnable).Id})
                .Build();
            return vc;
        }

        public static ProductDiscount.ProductDiscountItem ProductDiscountItem(Guid  id)
        {
            var vci = Builder<ProductDiscount.ProductDiscountItem>.CreateNew()
                 .WithConstructor(() => new ProductDiscount.ProductDiscountItem(id, DateTime.Now, DateTime.Now, EntityStatus.Active))
                 .With(n => n.EffectiveDate = DateTime.Now)
                 .With(n => n.DiscountRate = 20)
                 .Build();
            return vci;
        }
        public static TargetPeriod BuildTargetPeriod()
        {
            var vci = Builder<TargetPeriod>.CreateNew()
                .WithConstructor(() => new TargetPeriod(Guid.NewGuid(), DateTime.Now, DateTime.Now, EntityStatus.Active))
                .With(n => n.Name = "September 2011")
                .With(n => n.StartDate = DateTime.Now)
                .With(n => n.EndDate = DateTime.Now)
                .Build();
            return vci;
        }
        public static Target BuildTarget()
        {
            var vci = Builder<Target>.CreateNew()
                .WithConstructor(() => new Target(Guid.NewGuid(), DateTime.Now, DateTime.Now, EntityStatus.Active))
                .With(n => n.TargetPeriod = BuildTargetPeriod())
                .With(n => n.CostCentre = BuildDistributor())
                .With(n => n.TargetValue = 1200)
                .With(n => n.IsQuantityTarget = true)
                .Build();
            return vci;
        }
        public static Province BuildProvince()
        {
            var vci = Builder<Province>.CreateNew()
                .WithConstructor(() => new Province(Guid.NewGuid(), DateTime.Now, DateTime.Now, EntityStatus.Active))
                .With(n => n.Country = BuildCountry())

                .Build();
            return vci;
        }
        public static District BuildDistrict()
        {
            var vci = Builder<District>.CreateNew()
                .WithConstructor(() => new District(Guid.NewGuid(), DateTime.Now, DateTime.Now, EntityStatus.Active))
                .With(n => n.Province = BuildProvince())

                .Build();
            return vci;
        }
        public static ReOrderLevel BuildReOrderLevel()
        {
            var vci = Builder<ReOrderLevel>.CreateNew()
                .WithConstructor(() => new ReOrderLevel(Guid.NewGuid(), DateTime.Now, DateTime.Now, EntityStatus.Active))
                .With(n => n.DistributorId = BuildDistributor())
                .With(n => n.ProductId = BuildSaleProduct(ReturnableType.GenericReturnable))
                .With(n => n.ProductReOrderLevel = 12)
                .Build();
            return vci;
        }

        public static ChannelPackaging BuildChannelPackaging()
        {
            var vci = Builder<ChannelPackaging>.CreateNew()
                .WithConstructor(() => new ChannelPackaging(Guid.NewGuid(), DateTime.Now, DateTime.Now, EntityStatus.Active))
                 .With(n => n.Packaging = BuildProductPackaging())
                  .With(n => n.OutletType = BuildOutletType())
                  .Build();
            return vci;
        }
        public static FreeOfChargeDiscount BuildFreeOfChargeDiscount()
        {
            var vci = Builder<FreeOfChargeDiscount>.CreateNew()
                .WithConstructor(() => new FreeOfChargeDiscount(Guid.NewGuid(), DateTime.Now, DateTime.Now, EntityStatus.Active))
                .With(n => n.ProductRef = new ProductRef { ProductId = BuildSaleProduct(ReturnableType.GenericReturnable).Id })
                .Build();
            return vci;
        }
        public static Competitor BuildCompetitor()
        {
            var vci = Builder<Competitor>.CreateNew()
                .WithConstructor(() => new Competitor(Guid.NewGuid(), DateTime.Now, DateTime.Now, EntityStatus.Active))
                  .Build();
            return vci;
        }
        public static CompetitorProducts BuildCompetitorProducts()
        {
            var vci = Builder<CompetitorProducts>.CreateNew()
                .WithConstructor(() => new CompetitorProducts(Guid.NewGuid(), DateTime.Now, DateTime.Now, EntityStatus.Active))
                 .With(n => n.Packaging = BuildProductPackaging())
                  .With(n => n.PackagingType = BuildProductPackagingType())
                   .With(n => n.Flavour = BuildProductFlavour())
                    .With(n => n.ProductType = BuildProductType())
                     .With(n => n.Competitor = BuildCompetitor())
                      .With(n => n.Brand = BuildProductBrand(Guid.NewGuid()))
                  .Build();
            return vci;
        }
        public static AssetType BuildCoolerType()
        {
            var vci = Builder<AssetType>.CreateNew()
                .WithConstructor(() => new AssetType(Guid.NewGuid(), DateTime.Now, DateTime.Now, EntityStatus.Active))
                  .Build();
            return vci;
        }
        public static Asset BuildCooler()
        {
            var vci = Builder<Asset>.CreateNew()
                .WithConstructor(() => new Asset(Guid.NewGuid(), DateTime.Now, DateTime.Now, EntityStatus.Active))
                .With(n => n.AssetType = BuildCoolerType())
                  .Build();
            return vci;
        }
        public static DiscountGroup BuildDiscountGroup()
        {
            var vci = Builder<DiscountGroup>.CreateNew()
                .WithConstructor(() => new DiscountGroup(Guid.NewGuid(), DateTime.Now, DateTime.Now, EntityStatus.Active))
                .Build();
            return vci;
        }


        public static UserGroup BuildUserGroup()
        {
            var vci = Builder<UserGroup>.CreateNew()
                .WithConstructor(() => new UserGroup(Guid.NewGuid(), DateTime.Now, DateTime.Now, EntityStatus.Active))
                .Build();
            return vci;
        }

        public static UserGroupRoles BuildUserGroupRole()
        {
            var vci = Builder<UserGroupRoles>.CreateNew()
          .WithConstructor(() => new UserGroupRoles(Guid.NewGuid(), DateTime.Now, DateTime.Now, EntityStatus.Active))
          .With(n => n.UserGroup = BuildUserGroup())
           .With(n => n.UserRole = (int)UserRole.FinanceHandler)
          .Build();
            return vci;
        }
        public static Bank BuildBank()
        {
            var vci = Builder<Bank>.CreateNew()
          .WithConstructor(() => new Bank(Guid.NewGuid(), DateTime.Now, DateTime.Now, EntityStatus.Active))          
          .Build();
            return vci;
        }
        public static BankBranch BuildBankBranch()
        {
            var vci = Builder<BankBranch>.CreateNew()
          .WithConstructor(() => new BankBranch(Guid.NewGuid(), DateTime.Now, DateTime.Now, EntityStatus.Active))
          .With(n=>n.Bank=BuildBank())
          .Build();
            return vci;
        }
        public static Supplier BuildSupplier()
        {
            var vci = Builder<Supplier>.CreateNew()
                .WithConstructor(() => new Supplier(Guid.NewGuid(), DateTime.Now, DateTime.Now, EntityStatus.Active))
                .Build();
            return vci;
        }
        public static ContactType BuildContactType()
        {
            var vci = Builder<ContactType>.CreateNew()
                .WithConstructor(() => new ContactType(Guid.NewGuid(), DateTime.Now, DateTime.Now, EntityStatus.Active))
                .Build();
            return vci;
        }
        public static MaritalStatus BuildMaritalStatus()
        {
            var vci = Builder<MaritalStatus>.CreateNew()
                .WithConstructor(() => new MaritalStatus(Guid.NewGuid(), DateTime.Now, DateTime.Now, EntityStatus.Active))
                .Build();
            return vci;
            
        }
    }
}
