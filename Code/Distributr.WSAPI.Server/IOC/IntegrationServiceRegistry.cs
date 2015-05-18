using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Integrations.AgrimanagrImports;
using Distributr.Integrations.AgrimanagrImports.Impl;
using Distributr.Integrations.Imports;
using Distributr.Integrations.Imports.Impl;
using StructureMap.Configuration.DSL;

namespace Distributr.WSAPI.Server.IOC
{
    public class IntegrationServiceRegistry : Registry
    {
        public IntegrationServiceRegistry()
        {
            foreach (var item in DefaultServiceList())
            {
                For(item.Item1).Use(item.Item2);
            }
        }

        
        public static List<Tuple<Type, Type>> DefaultServiceList()
        {
            var serviceList = new List<Tuple<Type, Type>>
                                  {
                                      Tuple.Create(typeof (ICountryImporterService), typeof (CountryImporterService)),
                                      Tuple.Create(typeof (IRegionImporterService), typeof (RegionImporterService)),
                                      Tuple.Create(typeof (IAreaImporterService), typeof (AreaImporterService)),


                                      Tuple.Create(typeof (IBankImporterService), typeof (BankImporterService)),
                                      Tuple.Create(typeof (IBankBranchImporterService), typeof (BankBranchImporterService)),
                                      Tuple.Create(typeof (ISupplierImporterService), typeof (SupplierImporterService)),
                                      Tuple.Create(typeof (IVATClassImporterService), typeof (VATClassImporterService)),
                                      Tuple.Create(typeof (ISaleProductImporterService), typeof (SaleProductImporterService)),
                                      Tuple.Create(typeof (IProductBrandImporterService), typeof (ProductBrandImporterService)),
                                      Tuple.Create(typeof (IPricingImporterService), typeof (PricingImporterService)),
                                      Tuple.Create(typeof (IPricingTierImporterService), typeof (PricingTierImporterService)),
                                      Tuple.Create(typeof (IDistributorImporterService), typeof (DistributorImporterService)),
                                      Tuple.Create(typeof (IDistributorSalesmanImporterService), typeof (DistributorSalesmanImporterService)),

                                      Tuple.Create(typeof (IRouteImporterService), typeof (RouteImporterService)),
                                      Tuple.Create(typeof (IOutletImporterService), typeof (OutletImporterService)),
                                      Tuple.Create(typeof (IProductFlavourImporterService), typeof (ProductFlavourImporterService)),
                                      Tuple.Create(typeof (IProductTypeImporterService), typeof (ProductTypeImporterService)),
                                      Tuple.Create(typeof (IProductPackagingTypeImporterService), typeof (ProductPackagingTypeImporterService)),
                                      Tuple.Create(typeof (IProductPackagingImporterService), typeof (ProductPackagingImporterService)),

                                       Tuple.Create(typeof (IProductDiscountGroupImporterService), typeof (ProductDiscountGroupImporterService)),
                                       Tuple.Create(typeof (IDiscountGroupImporterService), typeof (DiscountGroupImporterService)),
                                        Tuple.Create(typeof (IProductDiscountImporterService), typeof (ProductDiscountImporterService)),
                                       
                                      Tuple.Create(typeof (IInventoryImporterService), typeof (InventoryImporterService)),
                                       Tuple.Create(typeof (IOutletTypeImporterService ), typeof (OutletTypeImporterService)),
                                        Tuple.Create(typeof (IOutletCategoryImporterService ), typeof (OutletCategoryImporterService)),
                                      
                                        
                                        Tuple.Create(typeof (ICommodityTypeImporterService ), typeof (CommodityTypeImporterService)),
                                        Tuple.Create(typeof (ICommodityImporterService ), typeof (CommodityImporterService)),
                                        Tuple.Create(typeof (ICommoditySupplierImporterService ), typeof (CommoditySupplierImporterService)),
                                        Tuple.Create(typeof (ICommodityOwnerImporterService ), typeof (CommodityOwnerImporterService)),
                                        Tuple.Create(typeof (ICommodityOwnerTypeImporterService ), typeof (CommodityOwnerTypeImporterService)),
                                  };
            return serviceList;
        }
    }
}
