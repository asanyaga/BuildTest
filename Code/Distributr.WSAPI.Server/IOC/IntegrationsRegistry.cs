using System;
using System.Collections.Generic;
using Distributr.Core.Data.Repository.Transactional.ThirdPartyIntegrationRepository;
using Distributr.Core.Factory.Documents;
using Distributr.Core.Factory.Documents.Impl;
using Distributr.Core.Repository.Transactional.DocumentRepositories.InventoryRepositories;
using Distributr.Core.Repository.Transactional.ThirdPartyIntegrationRepository;
using Distributr.Core.Workflow;
using Distributr.WSAPI.Lib.Integrations;
using Distributr.WSAPI.Lib.Integrations.InventoryWorkflows;
using Distributr.WSAPI.Lib.Integrations.MasterData;
using Distributr.WSAPI.Lib.Integrations.MasterData.Exports;
using Distributr.WSAPI.Lib.Integrations.MasterData.Impl;
using Distributr.WSAPI.Lib.Integrations.Transactions;
using Distributr.WSAPI.Lib.Integrations.Transactions.Impl;
using StructureMap.Configuration.DSL;

namespace Distributr.WSAPI.Server.IOC
{
    public class IntegrationsRegistry : Registry
    {
        public IntegrationsRegistry()
        {
            foreach (var item in DefaultServiceList())
            {
                For(item.Item1).Use(item.Item2);
            }
        }

         /*   For<ICommodityImportService>().Use<CommodityImportService>();
            For<ICommodityOwnerTypeImportService>().Use<CommodityOwnerTypeImportService>();
            For<ICommoditySupplierImportService>().Use<CommoditySupplierImportService>();
            For<ICommodityTypeImportService>().Use<CommodityTypeImportService>();
            For<ICommodityOwnerImportService>().Use<CommodityOwnerImportService>();

            For<IDistributrIntegrationService>().Use<DistributrIntegrationService>();
           


            //transactions
            For<ISageTransactionsExportService>().Use<SageTransactionsExportService>();
            For<ISapTransactionsDownloadService>().Use<SapTransactionsDownloadService>();
            For<IExportImportAuditRepository>().Use<ExportImportAuditRepository>();
            For<IQuickBooksTransactionsDownloadService>().Use<QuickBooksTransactionsDownloadService>();
            For<IMasterDataExportService>().Use<MasterDataExportService>();
            For<IWsInventoryAdjustmentWorflow>().Use<WsInventoryAdjustmentWorflow>();
            For<IInventoryTransferService>().Use<InventoryTransferService>();

            For<IInventoryAdjustmentNoteFactory>().Use<InventoryAdjustmentNoteFactory>();
            For<IInventoryTransferNoteFactory>().Use<InventoryTransferNoteFactory>();
         }*/
        public static List<Tuple<Type, Type>> DefaultServiceList()
        {
            var serviceList = new List<Tuple<Type, Type>>
                {
                    Tuple.Create(typeof (ITerritoryImportService), typeof (TerritoryImportService)),
                    Tuple.Create(typeof (ICountryImportService), typeof (CountryImportService)),
                    Tuple.Create(typeof (IRegionImportService), typeof (RegionImportService)),
                    Tuple.Create(typeof (IAreaImportService), typeof (AreaImportService)),
                    Tuple.Create(typeof (IAssetCategoryImportService), typeof (AssetCategoryImportService)),
                    Tuple.Create(typeof (IProvinceImportService), typeof (ProvinceImportService)),
                    Tuple.Create(typeof (IDistrictImportService), typeof (DistrictImportService)),
                    Tuple.Create(typeof (IContactTypeImportService), typeof (ContactTypeImportService)),
                    Tuple.Create(typeof (IOutletCategoryImportService), typeof (OutletCategoryImportService)),
                    Tuple.Create(typeof (IOutletTypeImportService), typeof (OutletTypeImportService)),
                    Tuple.Create(typeof (IBankBranchImportService), typeof (BankBranchImportService)),
                    Tuple.Create(typeof (IBankImportService), typeof (BankImportService)),
                    Tuple.Create(typeof (ISupplierImportService), typeof (SupplierImportService)),
                    Tuple.Create(typeof (IPricingTierImportService), typeof (PricingTierImportService)),
                    Tuple.Create(typeof (IVatClassImportService), typeof (VatClassImportService)),
                    Tuple.Create(typeof (IProductTypeImportService), typeof (ProductTypeImportService)),
                    Tuple.Create(typeof (IProductBrandImportService), typeof (ProductBrandImportService)),
                    Tuple.Create(typeof (IProductPackagingTypeImportService), typeof (ProductPackagingTypeImportService)),
                    Tuple.Create(typeof (IProductFlavourImportService), typeof (ProductFlavourImportService)),
                    Tuple.Create(typeof (IProductPackagingImportService), typeof (ProductPackagingImportService)),
                    Tuple.Create(typeof (IProductImportService), typeof (SaleProductImportService)),
                    Tuple.Create(typeof (IDiscountGroupImportService), typeof (DiscountGroupImportService)),
                    Tuple.Create(typeof (IChannelPackagingImportService), typeof (ChannelPackagingImportService)),
                    Tuple.Create(typeof (IPricingImportService), typeof (PricingImportService)),
                    Tuple.Create(typeof (ISaleValueDiscountImportService), typeof (SaleValueDiscountImportService)),
                    Tuple.Create(typeof (IProductGroupDiscountImportService), typeof (ProductGroupDiscountImportService)),
                    Tuple.Create(typeof (IPromotionDiscountImportService), typeof (PromotionDiscountImportService)),
                    Tuple.Create(typeof (ICertainValueCertainProductDiscountImportService), typeof (CertainValueCertainProductDiscountImportService)),
                    Tuple.Create(typeof (ISalesmanImportService), typeof (SalesmanImportService)),
                    Tuple.Create(typeof (IOutletImportService), typeof (OutletImportService)),
                    Tuple.Create(typeof (IRouteImportService), typeof (RouteImportService)),
                    Tuple.Create(typeof (IDistributorsImportService), typeof (DistributorsImportService)),
                    Tuple.Create(typeof (IShiptoAddressesImportService), typeof (ShiptoAddressesImportService)),
                    Tuple.Create(typeof (IDistributrIntegrationService), typeof (DistributrIntegrationService)),
                    //transactions
                    Tuple.Create(typeof (ISageTransactionsExportService), typeof (SageTransactionsExportService)),
                    Tuple.Create(typeof (ISapTransactionsDownloadService), typeof (SapTransactionsDownloadService)),
                    Tuple.Create(typeof (IExportImportAuditRepository), typeof (ExportImportAuditRepository)),
                    Tuple.Create(typeof (IQuickBooksTransactionsDownloadService), typeof (QuickBooksTransactionsDownloadService)),
                    Tuple.Create(typeof (IMasterDataExportService), typeof (MasterDataExportService)),
                    Tuple.Create(typeof (IWsInventoryAdjustmentWorflow), typeof (WsInventoryAdjustmentWorflow)),
                    Tuple.Create(typeof (IInventoryTransferService), typeof (InventoryTransferService)),
                    Tuple.Create(typeof (IInventoryAdjustmentNoteFactory), typeof (InventoryAdjustmentNoteFactory)),
                    Tuple.Create(typeof (IInventoryTransferNoteFactory), typeof (InventoryTransferNoteFactory)),
                    Tuple.Create(typeof(ICommodityImportService),typeof(CommodityImportService)),
                    Tuple.Create(typeof(ICommodityOwnerTypeImportService),typeof(CommodityOwnerTypeImportService)),
                    Tuple.Create(typeof(ICommoditySupplierImportService),typeof(CommoditySupplierImportService)),
                   Tuple.Create(typeof(ICommodityTypeImportService),typeof(CommodityTypeImportService)),
                    Tuple.Create(typeof(ICommodityOwnerImportService),typeof(CommodityOwnerImportService)),

                };
            return serviceList;
        }
    }
}
