using System;
using System.Collections.Generic;
using Distributr.WSAPI.Lib.Services.Sync;
using Distributr.WSAPI.Lib.Services.Sync.Impl;
using StructureMap.Configuration.DSL;

namespace Distributr.WSAPI.Server.IOC
{
    public class    SyncRegistry : Registry
    {
        public SyncRegistry()
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
                    Tuple.Create(typeof (ISyncAreaMasterDataService), typeof (SyncAreaMasterDataService)),
                    Tuple.Create(typeof (ISyncAssetCategoryMasterDataService), typeof (SyncAssetCategoryMasterDataService)),
                    Tuple.Create(typeof (ISyncAssetMasterDataService), typeof (SyncAssetMasterDataService)),
                    Tuple.Create(typeof (ISyncAssetStatusMasterDataService), typeof (SyncAssetStatusMasterDataService)),
                    Tuple.Create(typeof (ISyncAssetTypeMasterDataService), typeof (SyncAssetTypeMasterDataService)),
                    Tuple.Create(typeof (ISyncBankBranchMasterDataService), typeof (SyncBankBranchMasterDataService)),
                    Tuple.Create(typeof (ISyncBankMasterDataService), typeof (SyncBankMasterDataService)),
                    Tuple.Create(typeof (ISyncCompetitorMasterDataService), typeof (SyncCompetitorMasterDataService)),
                    Tuple.Create(typeof (ISyncCompetitorProductMasterDataService), typeof (SyncCompetitorProductMasterDataService)),
                    Tuple.Create(typeof (ISyncConsolidatedProductMasterDataService), typeof (SyncConsolidatedProductMasterDataService)),
                    Tuple.Create(typeof (ISyncContactMasterDataService), typeof (SyncContactMasterDataService)),
                    Tuple.Create(typeof (ISyncContactTypeMasterDataService), typeof (SyncContactTypeMasterDataService)),
                    Tuple.Create(typeof (ISyncCountryMasterDataService), typeof (SyncCountryMasterDataService)),
                    Tuple.Create(typeof (ISyncCvcpDiscountMasterDataService), typeof (SyncCvcpDiscountMasterDataService)),
                    Tuple.Create(typeof (ISyncDiscountGroupMasterDataService), typeof (SyncDiscountGroupMasterDataService)),
                    Tuple.Create(typeof (ISyncDistributorMasterDataService), typeof (SyncDistributorMasterDataService)),
                    Tuple.Create(typeof (ISyncDistributorPendingDispatchWarehouseService), typeof (SyncDistributorPendingDispatchWarehouseService)),
                    Tuple.Create(typeof (ISyncDistributorSalesmanMasterDataService), typeof (SyncDistributorSalesmanMasterDataService)),
                    Tuple.Create(typeof (ISyncDistrictMasterDataService), typeof (SyncDistrictMasterDataService)),
                    Tuple.Create(typeof (ISyncFreeOfChargeDiscountMasterDataService), typeof (SyncFreeOfChargeDiscountMasterDataService)),
                    Tuple.Create(typeof (ISyncOutletCategoryMasterDataService), typeof (SyncOutletCategoryMasterDataService)),
                    Tuple.Create(typeof (ISyncOutletMasterDataService), typeof (SyncOutletMasterDataService)),
                    Tuple.Create(typeof (ISyncOutletPriorityMasterDataService), typeof (SyncOutletPriorityMasterDataService)),
                    Tuple.Create(typeof (ISyncOutletTypeMasterDataService), typeof (SyncOutletTypeMasterDataService)),
                    Tuple.Create(typeof (ISyncOutletVisitDayMasterDataService), typeof (SyncOutletVisitDayMasterDataService)),
                    Tuple.Create(typeof (ISyncPricingMasterDataService), typeof (SyncPricingMasterDataService)),
                    Tuple.Create(typeof (ISyncPricingTierMasterDataService), typeof (SyncPricingTierMasterDataService)),
                    Tuple.Create(typeof (ISyncProducerMasterDataService), typeof (SyncProducerMasterDataService)),
                    Tuple.Create(typeof (ISyncProductBrandMasterDataService), typeof (SyncProductBrandMasterDataService)),
                    Tuple.Create(typeof (ISyncProductDiscountMasterDataService), typeof (SyncProductDiscountMasterDataService)),
                    Tuple.Create(typeof (ISyncProductFlavourMasterDataService), typeof (SyncProductFlavourMasterDataService)),
                    Tuple.Create(typeof (ISyncProductGroupDiscountMasterDataService), typeof (SyncProductGroupDiscountMasterDataService)),
                    Tuple.Create(typeof (ISyncProductPackagingMasterDataService), typeof (SyncProductPackagingMasterDataService)),
                    Tuple.Create(typeof (ISyncProductPackagingTypeMasterDataService), typeof (SyncProductPackagingTypeMasterDataService)),
                    Tuple.Create(typeof (ISyncProductTypeMasterDataService), typeof (SyncProductTypeMasterDataService)),
                    Tuple.Create(typeof (ISyncPromotionDiscountMasterDataService), typeof (SyncPromotionDiscountMasterDataService)),
                    Tuple.Create(typeof (ISyncProvinceMasterDataService), typeof (SyncProvinceMasterDataService)),
                    Tuple.Create(typeof (ISyncRegionMasterDataService), typeof (SyncRegionMasterDataService)),
                    Tuple.Create(typeof (ISyncReorderLevelMasterDataService), typeof (SyncReorderLevelMasterDataService)),
                    Tuple.Create(typeof (ISyncRetireSettingMasterDataService), typeof (SyncRetireSettingMasterDataService)),
                    Tuple.Create(typeof (ISyncReturnableProductMasterDataService), typeof (SyncReturnableProductMasterDataService)),
                    Tuple.Create(typeof (ISyncRouteCentreMasterDataService), typeof (SyncRouteCentreMasterDataService)),
                    Tuple.Create(typeof (ISyncRouteCostCentreMasterDataService), typeof (SyncRouteCostCentreMasterDataService)),
                    Tuple.Create(typeof (ISyncRouteMasterDataService), typeof (SyncRouteMasterDataService)),
                    Tuple.Create(typeof (ISyncRouteRegionAllocationMasterDataService), typeof (SyncRouteRegionAllocationMasterDataService)),
                    Tuple.Create(typeof (ISyncSaleProductMasterDataService), typeof (SyncSaleProductMasterDataService)),
                    Tuple.Create(typeof (ISyncSalesmanRouteMasterDataService), typeof (SyncSalesmanRouteMasterDataService)),
                    Tuple.Create(typeof (ISyncSalesmanSupplierMasterDataService), typeof (SyncSalesmanSupplierMasterDataService)),


                    Tuple.Create(typeof (ISyncSaleValueDiscountMasterDataService), typeof (SyncSaleValueDiscountMasterDataService)),
                    Tuple.Create(typeof (ISyncSettingMasterDataService), typeof (SyncSettingMasterDataService)),
                    Tuple.Create(typeof (ISyncSocioEconomicStatusMasterDataService), typeof (SyncSocioEconomicStatusMasterDataService)),
                    Tuple.Create(typeof (ISyncSupplierMasterDataService), typeof (SyncSupplierMasterDataService)),
                    Tuple.Create(typeof (ISyncTargetItemMasterDataService), typeof (SyncTargetItemMasterDataService)),
                    Tuple.Create(typeof (ISyncTargetMasterDataService), typeof (SyncTargetMasterDataService)),
                    Tuple.Create(typeof (ISyncTargetPeriodMasterDataService), typeof (SyncTargetPeriodMasterDataService)),
                    Tuple.Create(typeof (ISyncTerritoryMasterDataService), typeof (SyncTerritoryMasterDataService)),
                    Tuple.Create(typeof (ISyncUserMasterDataService), typeof (SyncUserMasterDataService)),
                    Tuple.Create(typeof (ISyncUserGroupMasterDataService), typeof (SyncUserGroupMasterDataService)),
                    Tuple.Create(typeof (ISyncUserGroupRoleMasterDataService), typeof (SyncUserGroupRoleMasterDataService)),
                    Tuple.Create(typeof (ISyncVatClassMasterDataService), typeof (SyncVatClassMasterDataService)),
                    Tuple.Create(typeof (ISyncUnderBankingService), typeof (SyncUnderBankingService)),
                     Tuple.Create(typeof (ISyncChannelPackagingMasterDataService), typeof (SyncChannelPackagingMasterDataService)),
                     Tuple.Create(typeof (IMasterDataZipperService), typeof (MasterDataZipperService)),
                     Tuple.Create(typeof (ISyncPaymentsService), typeof (SyncPaymentsService)),
                    
            
                    //agrimanagr
                    Tuple.Create(typeof (ISyncCommodityTypeMasterDataService), typeof (SyncCommodityTypeMasterDataService)),
                    Tuple.Create(typeof (ISyncCommodityMasterDataService), typeof (SyncCommodityMasterDataService)),
                    Tuple.Create(typeof (ISyncCentreTypeMasterDataService), typeof (SyncCentreTypeMasterDataService)),
                    Tuple.Create(typeof (ISyncCentreMasterDataService), typeof (SyncCentreMasterDataService)),
                    Tuple.Create(typeof (ISyncHubMasterDataService), typeof (SyncHubMasterDataService)),
                    Tuple.Create(typeof (ISyncStoreMasterDataService), typeof (SyncStoreMasterDataService)),
                    Tuple.Create(typeof (ISyncCommoditySupplierMasterDataService), typeof (SyncCommoditySupplierMasterDataService)),
                    Tuple.Create(typeof (ISyncCommodityProducerMasterDataService), typeof (SyncCommodityProducerMasterDataService)),
                    Tuple.Create(typeof (ISyncCommodityOwnerTypeMasterDataService), typeof (SyncCommodityOwnerTypeMasterDataService)),
                    Tuple.Create(typeof (ISyncCommodityOwnerMasterDataService), typeof (SyncCommodityOwnerMasterDataService)),
                    Tuple.Create(typeof (ISyncFieldClerkMasterDataService), typeof (SyncFieldClerkMasterDataService)),
                    Tuple.Create(typeof (ISyncContainerTypeMasterDataService), typeof (SyncContainerTypeMasterDataService)),
                    Tuple.Create(typeof (ISyncPrinterMasterDataService), typeof (SyncPrinterMasterDataService)),
                    Tuple.Create(typeof (ISyncWeighScaleMasterDataService), typeof (SyncWeighScaleMasterDataService)),
                    Tuple.Create(typeof (ISyncSourcingContainerMasterDataService), typeof (SyncSourcingContainerMasterDataService)),
                    Tuple.Create(typeof (ISyncCommodityProducerCentreMasterDataService), typeof (SyncCommodityProducerCentreMasterDataService)),
                    Tuple.Create(typeof (ISyncVehicleMasterDataService), typeof (SyncVehicleMasterDataService)),
                     Tuple.Create(typeof (ISyncServiceProviderMasterdataService), typeof (SyncServiceProviderMasterdataService)),
            Tuple.Create(typeof (ISyncServicesMasterdataService), typeof (SyncServicesMasterdataService)),
            Tuple.Create(typeof (ISyncShiftMasterDataService), typeof (SyncShiftMasterDataService)),
            Tuple.Create(typeof (ISyncSeasonMasterDataService), typeof (SyncSeasonMasterDataService)),
            Tuple.Create(typeof (ISyncInfectionsMasterDataService), typeof (SyncInfectionsMasterDataService)),
            Tuple.Create(typeof (ISyncActivityTypeMasterDataService), typeof (SyncActivityTypeMasterDataService)),
            Tuple.Create(typeof (ISyncOutletVisitReasonsTypeMasterDataService), typeof (SyncOutletVisitReasonsTypeMasterDataService)),
            Tuple.Create(typeof (ISyncInventoryService), typeof (SyncInventoryService)),

                };
            return serviceList;
           
        }
    }
}
