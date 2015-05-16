using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.MasterDataDTO.DTOModels.AgrimanagrDTO.CommodityDTOs;
using Distributr.Core.MasterDataDTO.DTOModels.AgrimanagrDTO.CostCentreDTOs;
using Distributr.Core.MasterDataDTO.DTOModels.AgrimanagrDTO.EquipmentDTOs;
using Distributr.Core.MasterDataDTO.DTOModels.AgrimanagrDTO.FarmActivities;
using Distributr.Core.MasterDataDTO.DTOModels.FinancialDTO;
using Distributr.Core.MasterDataDTO.DTOModels.InventoriesDTO;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Assets;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Banks;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.ChannelPackaging;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Competitor;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.CostCentre;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.DistributorTargets;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.MasterDataAllocations;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Product;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Retire;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Settings;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Suppliers;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.User;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.WSAPI.Lib.Services.Sync;
using Distributr.WSAPI.Lib.Services.Sync.Impl;
using Distributr.WSAPI.Lib.System.Utility;
using Distributr.WebApi.Models;

namespace Distributr.WebApi.ApiControllers
{
     [RoutePrefix("api/masterdata/sync")]
    public class SyncMasterDataController : ApiController
    {
        private ISyncInventoryService _syncInventoryService;
        private readonly ISyncAreaMasterDataService _syncArea;
        private readonly ISyncAssetCategoryMasterDataService _syncAssetCategory;
        private readonly ISyncAssetMasterDataService _syncAsset;
        private readonly ISyncAssetStatusMasterDataService _syncAssetStatus;
        private readonly ISyncAssetTypeMasterDataService _syncAssetType;
        private readonly ISyncBankBranchMasterDataService _syncBankBranch;
        private readonly ISyncBankMasterDataService _syncBank;
        private readonly ISyncCompetitorMasterDataService _syncCompetitor;
        private readonly ISyncCompetitorProductMasterDataService _syncCompetitorProduct;
        private readonly ISyncConsolidatedProductMasterDataService _syncConsolidatedProduct;
        private readonly ISyncContactMasterDataService _syncContact;
        private readonly ISyncContactTypeMasterDataService _syncContactType;
        private readonly ISyncCountryMasterDataService _syncCountry;
        private readonly ISyncCvcpDiscountMasterDataService _syncCvcpDiscount;
        private readonly ISyncDiscountGroupMasterDataService _syncDiscountGroup;
        private readonly ISyncDistributorMasterDataService _syncDistributor;
        private readonly ISyncDistributorPendingDispatchWarehouseService _syncDistributorPendingDispatchWarehouse;
        private readonly ISyncDistributorSalesmanMasterDataService _syncDistributorSalesman;
        private readonly ISyncDistrictMasterDataService _syncDistrict;
        private readonly ISyncFreeOfChargeDiscountMasterDataService _syncFreeOfChargeDiscount;
        private readonly ISyncOutletCategoryMasterDataService _syncOutletCategory;
        private readonly ISyncOutletMasterDataService _syncOutlet;
        private readonly ISyncOutletPriorityMasterDataService _syncOutletPriority;
        private readonly ISyncOutletTypeMasterDataService _syncOutletType;
        private readonly ISyncOutletVisitDayMasterDataService _syncOutletVisitDay;
        private readonly ISyncPricingMasterDataService _syncPricing;
        private readonly ISyncPricingTierMasterDataService _syncPricingTier;
        private readonly ISyncProducerMasterDataService _syncProducer;
        private readonly ISyncProductBrandMasterDataService _syncProductBrand;
        private readonly ISyncProductDiscountMasterDataService _syncProductDiscount;
        private readonly ISyncProductFlavourMasterDataService _syncProductFlavour;
        private readonly ISyncProductGroupDiscountMasterDataService _syncProductGroupDiscount;
        private readonly ISyncProductPackagingMasterDataService _syncProductPackaging;
        private readonly ISyncProductPackagingTypeMasterDataService _syncProductPackagingType;
        private readonly ISyncProductTypeMasterDataService _syncProductType;
        private readonly ISyncPromotionDiscountMasterDataService _syncPromotionDiscount;
        private readonly ISyncProvinceMasterDataService _syncProvince;
        private readonly ISyncRegionMasterDataService _syncRegion;
        private readonly ISyncReorderLevelMasterDataService _syncReorderLevel;
        private readonly ISyncRetireSettingMasterDataService _syncRetireSetting;
        private readonly ISyncReturnableProductMasterDataService _syncReturnableProduct;
        private readonly ISyncRouteCentreMasterDataService _syncRouteCentreAllocation;
        private readonly ISyncRouteCostCentreMasterDataService _syncRouteCostCentreAllocation;
        private readonly ISyncRouteMasterDataService _syncRoute;
        private readonly ISyncRouteRegionAllocationMasterDataService _syncRouteRegionAllocation;
        private readonly ISyncSaleProductMasterDataService _syncSaleProduct;
        private readonly ISyncSalesmanRouteMasterDataService _syncSalesmanRoute;
        private readonly ISyncSalesmanSupplierMasterDataService _syncSalesmanSupplier;


        private readonly ISyncSaleValueDiscountMasterDataService _syncSaleValueDiscount;
        private readonly ISyncSettingMasterDataService _syncSetting;
        private readonly ISyncSocioEconomicStatusMasterDataService _syncSocioEconomicStatus;
        private readonly ISyncSupplierMasterDataService _syncSupplier;
        private readonly ISyncTargetItemMasterDataService _syncTargetItem;
        private readonly ISyncTargetMasterDataService _syncTarget;
        private readonly ISyncTargetPeriodMasterDataService _syncTargetPeriod;
        private readonly ISyncTerritoryMasterDataService _syncTerritory;
        private readonly ISyncUserMasterDataService _syncUser;
        private readonly ISyncUserGroupMasterDataService _syncUserGroup;
        private readonly ISyncUserGroupRoleMasterDataService _syncUserGroupRole;
        private readonly ISyncVatClassMasterDataService _syncVatClass;
        private readonly ISyncCommodityTypeMasterDataService _syncCommodityType;
        private readonly ISyncCommodityMasterDataService _syncCommodityMasterDataService;
        private readonly ISyncCentreTypeMasterDataService _syncCentreType;
        private readonly ISyncCentreMasterDataService _syncCentre;
        private readonly ISyncHubMasterDataService _syncHub;
        private readonly ISyncStoreMasterDataService _syncStore;
        private readonly ISyncCommodityMasterDataService _syncCommodity;
        private readonly ISyncCommoditySupplierMasterDataService _syncCommoditySupplier;
        private readonly ISyncCommodityProducerMasterDataService _syncCommodityProducer;
        private readonly ISyncCommodityProducerCentreMasterDataService _syncCommodityProducerCentre;
        private readonly ISyncCommodityOwnerTypeMasterDataService _syncCommodityOwnerType;
        private readonly ISyncCommodityOwnerMasterDataService _syncCommodityOwner;
        private readonly ISyncFieldClerkMasterDataService _syncFieldClerk;
        private readonly ISyncContainerTypeMasterDataService _syncContainerType;
        private readonly ISyncPrinterMasterDataService _syncPrinter;
        private readonly ISyncWeighScaleMasterDataService _syncWeighScale;
        private readonly ISyncSourcingContainerMasterDataService _syncSourcingContainer;
        private readonly ISyncVehicleMasterDataService _syncVehicle;
        private readonly ISyncUnderBankingService _syncUnderBankingService;
        private readonly ISyncServiceProviderMasterdataService _syncServiceProviderMasterdataService;
        private readonly ISyncServicesMasterdataService _syncServicesMasterdataService;
        private readonly ISyncShiftMasterDataService _syncShiftMasterDataService;
        private readonly ISyncSeasonMasterDataService _syncSeasonMasterDataService;
        private readonly ISyncInfectionsMasterDataService _syncInfectionsMasterDataService;
        private readonly ISyncActivityTypeMasterDataService _syncActivityTypeMasterDataService;
        private readonly ISyncOutletVisitReasonsTypeMasterDataService _syncOutletVisitReasonsTypeMasterDataService;
        private ISyncChannelPackagingMasterDataService _channelPackagingMasterDataService;
         private readonly ISyncPaymentsService _syncPaymentsService;


        public SyncMasterDataController(ISyncUnderBankingService syncUnderBankingService, ISyncAreaMasterDataService syncArea, ISyncAssetCategoryMasterDataService syncAssetCategory,
            ISyncAssetMasterDataService syncAsset, ISyncAssetStatusMasterDataService syncAssetStatus, ISyncAssetTypeMasterDataService syncAssetType,
            ISyncBankBranchMasterDataService syncBankBranch, ISyncBankMasterDataService syncBank, ISyncCompetitorMasterDataService syncCompetitor,
            ISyncCompetitorProductMasterDataService syncCompetitorProduct, ISyncConsolidatedProductMasterDataService syncConsolidatedProduct,
            ISyncContactMasterDataService syncContact, ISyncContactTypeMasterDataService syncContactType, ISyncCountryMasterDataService syncCountry,
            ISyncCvcpDiscountMasterDataService syncCvcpDiscount, ISyncDiscountGroupMasterDataService syncDiscountGroup, ISyncDistributorMasterDataService syncDistributor,
            ISyncDistributorPendingDispatchWarehouseService syncDistributorPendingDispatchWarehouse, ISyncDistributorSalesmanMasterDataService syncDistributorSalesman,
            ISyncDistrictMasterDataService syncDistrict, ISyncFreeOfChargeDiscountMasterDataService syncFreeOfChargeDiscount,
            ISyncOutletCategoryMasterDataService syncOutletCategory, ISyncOutletMasterDataService syncOutlet, ISyncOutletPriorityMasterDataService syncOutletPriority,
            ISyncOutletTypeMasterDataService syncOutletType, ISyncOutletVisitDayMasterDataService syncOutletVisitDay, ISyncPricingMasterDataService syncPricing,
            ISyncPricingTierMasterDataService syncPricingTier, ISyncProducerMasterDataService syncProducer, ISyncProductBrandMasterDataService syncProductBrand,
            ISyncProductDiscountMasterDataService syncProductDiscount, ISyncProductFlavourMasterDataService syncProductFlavour,
            ISyncProductGroupDiscountMasterDataService syncProductGroupDiscount, ISyncProductPackagingMasterDataService syncProductPackaging,
            ISyncProductPackagingTypeMasterDataService syncProductPackagingType, ISyncProductTypeMasterDataService syncProductType,
            ISyncPromotionDiscountMasterDataService syncPromotionDiscount, ISyncProvinceMasterDataService syncProvince, ISyncRegionMasterDataService syncRegion,
            ISyncReorderLevelMasterDataService syncReorderLevel, ISyncRetireSettingMasterDataService syncRetireSetting,
            ISyncReturnableProductMasterDataService syncReturnableProduct, ISyncRouteCentreMasterDataService syncRouteCentreAllocation,
            ISyncRouteCostCentreMasterDataService syncRouteCostCentreAllocation, ISyncRouteMasterDataService syncRoute,
            ISyncRouteRegionAllocationMasterDataService syncRouteRegionAllocation, ISyncSaleProductMasterDataService syncSaleProduct,
            ISyncSalesmanRouteMasterDataService syncSalesmanRoute, ISyncSaleValueDiscountMasterDataService syncSaleValueDiscount,
            ISyncSalesmanSupplierMasterDataService syncSalesmanSupplier,

            ISyncSettingMasterDataService syncSetting, ISyncSocioEconomicStatusMasterDataService syncSocioEconomicStatus, ISyncSupplierMasterDataService syncSupplier,
            ISyncTargetItemMasterDataService syncTargetItem, ISyncTargetMasterDataService syncTarget, ISyncTargetPeriodMasterDataService syncTargetPeriod,
            ISyncTerritoryMasterDataService syncTerritory, ISyncUserMasterDataService syncUser, ISyncUserGroupMasterDataService syncUserGroup,
            ISyncUserGroupRoleMasterDataService syncUserGroupRole, ISyncVatClassMasterDataService syncVatClass, ISyncCommodityTypeMasterDataService syncCommodityType,
            ISyncCommodityMasterDataService syncCommodityMasterDataService, ISyncCentreTypeMasterDataService syncCentreType, ISyncCentreMasterDataService syncCentre,
            ISyncHubMasterDataService syncHub, ISyncStoreMasterDataService syncStore, ISyncCommodityMasterDataService syncCommodity, ISyncCommoditySupplierMasterDataService syncCommoditySupplier,
            ISyncCommodityProducerMasterDataService syncCommodityProducer, ISyncCommodityProducerCentreMasterDataService syncCommodityProducerCentre,
            ISyncCommodityOwnerTypeMasterDataService syncCommodityOwnerType, ISyncCommodityOwnerMasterDataService syncCommodityOwner,
            ISyncFieldClerkMasterDataService syncFieldClerk, ISyncContainerTypeMasterDataService syncContainerType, ISyncWeighScaleMasterDataService syncWeighScale,
            ISyncPrinterMasterDataService syncPrinter, ISyncSourcingContainerMasterDataService syncSourcingContainer, ISyncVehicleMasterDataService syncVehicle, ISyncServicesMasterdataService syncServicesMasterdataService, ISyncShiftMasterDataService syncShiftMasterDataService, ISyncSeasonMasterDataService syncSeasonMasterDataService, ISyncInfectionsMasterDataService syncInfectionsMasterDataService, ISyncActivityTypeMasterDataService syncactivityTypeMasterDataService, ISyncServiceProviderMasterdataService syncServiceProviderMasterdataService, ISyncChannelPackagingMasterDataService channelPackagingMasterDataService, ISyncOutletVisitReasonsTypeMasterDataService syncOutletVisitReasonsTypeMasterDataService, ISyncInventoryService syncInventoryService, ISyncPaymentsService syncPaymentsService)
        {
            _syncArea = syncArea;
            _syncAssetCategory = syncAssetCategory;
            _syncAsset = syncAsset;
            _syncAssetStatus = syncAssetStatus;
            _syncAssetType = syncAssetType;
            _syncBankBranch = syncBankBranch;
            _syncBank = syncBank;
            _syncCompetitor = syncCompetitor;
            _syncCompetitorProduct = syncCompetitorProduct;
            _syncConsolidatedProduct = syncConsolidatedProduct;
            _syncContact = syncContact;
            _syncContactType = syncContactType;
            _syncCountry = syncCountry;
            _syncCvcpDiscount = syncCvcpDiscount;
            _syncDiscountGroup = syncDiscountGroup;
            _syncDistributor = syncDistributor;
            _syncDistributorPendingDispatchWarehouse = syncDistributorPendingDispatchWarehouse;
            _syncDistributorSalesman = syncDistributorSalesman;
            _syncDistrict = syncDistrict;
            _syncFreeOfChargeDiscount = syncFreeOfChargeDiscount;
            _syncOutletCategory = syncOutletCategory;
            _syncOutlet = syncOutlet;
            _syncOutletPriority = syncOutletPriority;
            _syncOutletType = syncOutletType;
            _syncOutletVisitDay = syncOutletVisitDay;
            _syncPricing = syncPricing;
            _syncPricingTier = syncPricingTier;
            _syncProducer = syncProducer;
            _syncProductBrand = syncProductBrand;
            _syncProductDiscount = syncProductDiscount;
            _syncProductFlavour = syncProductFlavour;
            _syncProductGroupDiscount = syncProductGroupDiscount;
            _syncProductPackaging = syncProductPackaging;
            _syncProductPackagingType = syncProductPackagingType;
            _syncProductType = syncProductType;
            _syncPromotionDiscount = syncPromotionDiscount;
            _syncProvince = syncProvince;
            _syncRegion = syncRegion;
            _syncReorderLevel = syncReorderLevel;
            _syncRetireSetting = syncRetireSetting;
            _syncReturnableProduct = syncReturnableProduct;
            _syncRouteCentreAllocation = syncRouteCentreAllocation;
            _syncRouteCostCentreAllocation = syncRouteCostCentreAllocation;
            _syncRoute = syncRoute;
            _syncRouteRegionAllocation = syncRouteRegionAllocation;
            _syncSaleProduct = syncSaleProduct;
            _syncSalesmanRoute = syncSalesmanRoute;
            _syncSalesmanSupplier = syncSalesmanSupplier;
            _syncSaleValueDiscount = syncSaleValueDiscount;
            _syncSetting = syncSetting;
            _syncSocioEconomicStatus = syncSocioEconomicStatus;
            _syncSupplier = syncSupplier;
            _syncTargetItem = syncTargetItem;
            _syncTarget = syncTarget;
            _syncTargetPeriod = syncTargetPeriod;
            _syncTerritory = syncTerritory;
            _syncUser = syncUser;
            _syncUserGroup = syncUserGroup;
            _syncUserGroupRole = syncUserGroupRole;
            _syncVatClass = syncVatClass;
            _syncCommodityType = syncCommodityType;
            _syncCommodityMasterDataService = syncCommodityMasterDataService;
            _syncCentreType = syncCentreType;
            _syncCentre = syncCentre;
            _syncHub = syncHub;
            _syncStore = syncStore;
            _syncCommodity = syncCommodity;
            _syncCommoditySupplier = syncCommoditySupplier;
            _syncCommodityProducer = syncCommodityProducer;
            _syncCommodityProducerCentre = syncCommodityProducerCentre;
            _syncCommodityOwnerType = syncCommodityOwnerType;
            _syncCommodityOwner = syncCommodityOwner;
            _syncFieldClerk = syncFieldClerk;
            _syncContainerType = syncContainerType;
            _syncWeighScale = syncWeighScale;
            _syncPrinter = syncPrinter;
            _syncSourcingContainer = syncSourcingContainer;
            _syncVehicle = syncVehicle;
            _syncServicesMasterdataService = syncServicesMasterdataService;
            _syncShiftMasterDataService = syncShiftMasterDataService;
            _syncSeasonMasterDataService = syncSeasonMasterDataService;
            _syncInfectionsMasterDataService = syncInfectionsMasterDataService;
            _syncActivityTypeMasterDataService = syncactivityTypeMasterDataService;
            _syncServiceProviderMasterdataService = syncServiceProviderMasterdataService;
            _channelPackagingMasterDataService = channelPackagingMasterDataService;
            _syncOutletVisitReasonsTypeMasterDataService = syncOutletVisitReasonsTypeMasterDataService;
            _syncInventoryService = syncInventoryService;
            _syncPaymentsService = syncPaymentsService;
            _syncUnderBankingService = syncUnderBankingService;
        }
        [HttpGet]
        [Route("Inventory")]
        public SyncResponseMasterDataInfo<InventoryDTO> Inventory()
        {
            var query = GetSyncValues();
            var all = _syncInventoryService.GetInventory(query);
            return all;
        }

        [HttpGet]
        [Route("Payments")]
        public SyncResponseMasterDataInfo<PaymentTrackerDTO> Payments()
        {
            var query = GetSyncValues();
            var all = _syncPaymentsService.GetPayment(query);
            return all;
        }
        [HttpGet]
        public SyncResponseMasterDataInfo<UnderBankingDTO> UnderBanking()
        {
            var query = GetSyncValues();
            var all = _syncUnderBankingService.GetUnderBanking(query);
            return all;
        }
        [HttpGet]
        public SyncResponseMasterDataInfo<ChannelPackagingDTO> ChannelPackaging()
        {
            var query = GetSyncValues();
            var all = _channelPackagingMasterDataService.GetChannelPackaging(query);
            return all;
        }
        [HttpGet]
        public SyncResponseMasterDataInfo<AreaDTO> Area()
        {
            var query = GetSyncValues();
            var all = _syncArea.GetArea(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<AssetCategoryDTO> AssetCategory()
        {
            var query = GetSyncValues();
            var all = _syncAssetCategory.GetAssetCategory(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<AssetDTO> Asset()
        {
            var query = GetSyncValues();
            var all = _syncAsset.GetAsset(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<AssetStatusDTO> AssetStatus()
        {
            var query = GetSyncValues();
            var all = _syncAssetStatus.GetAssetStatus(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<AssetTypeDTO> AssetType()
        {
            var query = GetSyncValues();
            var all = _syncAssetType.GetAssetType(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<BankBranchDTO> BankBranch()
        {
            var query = GetSyncValues();
            var all = _syncBankBranch.GetBankBranch(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<BankDTO> Bank()
        {
            var query = GetSyncValues();
            var all = _syncBank.GetBank(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<CompetitorDTO> Competitor()
        {
            var query = GetSyncValues();
            var all = _syncCompetitor.GetCompetitor(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<CompetitorProductDTO> CompetitorProduct()
        {
            var query = GetSyncValues();
            var all = _syncCompetitorProduct.GetCompetitorProduct(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<ConsolidatedProductDTO> ConsolidatedProduct()
        {
            var query = GetSyncValues();
            var all = _syncConsolidatedProduct.GetConsolidatedProduct(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<ContactDTO> Contact()
        {
            var query = GetSyncValues();
            var all = _syncContact.GetContact(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<ContactTypeDTO> ContactType()
        {
            var query = GetSyncValues();
            var all = _syncContactType.GetContactType(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<CountryDTO> Country()
        {
            var query = GetSyncValues();
            var all = _syncCountry.GetCountry(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<CertainValueCertainProductDiscountDTO> CertainValueCertainProductDiscount()
        {
            var query = GetSyncValues();
            var all = _syncCvcpDiscount.GetCvcpDiscount(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<DiscountGroupDTO> DiscountGroup()
        {
            var query = GetSyncValues();
            var all = _syncDiscountGroup.GetDiscountGroup(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<DistributorDTO> Distributor()
        {
            var query = GetSyncValues();
            var all = _syncDistributor.GetDistributor(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<DistributorPendingDispatchWarehouseDTO> DistributorPendingDispatchWarehouse()
        {
            var query = GetSyncValues();
            var all = _syncDistributorPendingDispatchWarehouse.GetDistributorPendingDispatchWarehouse(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<DistributorSalesmanDTO> DistributorSalesman()
        {
            var query = GetSyncValues();
            var all = _syncDistributorSalesman.GetDistributorSalesman(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<DistrictDTO> District()
        {
            var query = GetSyncValues();
            var all = _syncDistrict.GetDistrict(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<FreeOfChargeDiscountDTO> FreeOfChargeDiscount()
        {
            var query = GetSyncValues();
            var all = _syncFreeOfChargeDiscount.GetFreeOfChargeDiscount(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<OutletCategoryDTO> OutletCategory()
        {
            var query = GetSyncValues();
            var all = _syncOutletCategory.GetOutletCategory(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<OutletDTO> Outlet()
        {
            var query = GetSyncValues();
            var all = _syncOutlet.GetOutlet(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<OutletPriorityDTO> OutletPriority()
        {
            var query = GetSyncValues();
            var all = _syncOutletPriority.GetOutletPriority(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<OutletTypeDTO> OutletType()
        {
            var query = GetSyncValues();
            var all = _syncOutletType.GetOutletType(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<OutletVisitDayDTO> OutletVisitDay()
        {
            var query = GetSyncValues();
            var all = _syncOutletVisitDay.GetOutletVisitDay(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<ProductPricingDTO> Pricing()
        {
            var query = GetSyncValues();
            //var all = _syncPricing.GetPricing(query);
            //return all;

            var parameters = this.Request.RequestUri.ParseQueryString();
            bool isHub = false;
            bool.TryParse(parameters["isHub"], out isHub);

            if (isHub)
            {
                return _syncPricing.GetHubPricing(query);
            }
            else
            {
                return _syncPricing.GetPricing(query);
            }
        }



        [HttpGet]
        public SyncResponseMasterDataInfo<ProductPricingTierDTO> PricingTier()
        {
            var query = GetSyncValues();
            var all = _syncPricingTier.GetPricingTier(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<ProducerDTO> Producer()
        {
            var query = GetSyncValues();
            var all = _syncProducer.GetProducer(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<ProductBrandDTO> ProductBrand()
        {
            var query = GetSyncValues();
            var all = _syncProductBrand.GetProductBrand(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<ProductDiscountDTO> ProductDiscount()
        {
            var query = GetSyncValues();
            var all = _syncProductDiscount.GetProductDiscount(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<ProductFlavourDTO> ProductFlavour()
        {
            var query = GetSyncValues();
            var all = _syncProductFlavour.GetProductFlavour(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<ProductGroupDiscountDTO> ProductGroupDiscount()
        {
            var parameters = this.Request.RequestUri.ParseQueryString();
            bool isHub = false;
            bool.TryParse(parameters["isHub"], out isHub);
            var query = GetSyncValues();
            if (isHub)
            {
                return _syncProductGroupDiscount.GetHubProductGroupDiscount(query);
            }
            else
            {
                return _syncProductGroupDiscount.GetProductGroupDiscount(query);
            }


        }


        [HttpGet]
        public SyncResponseMasterDataInfo<ProductPackagingDTO> ProductPackaging()
        {
            var query = GetSyncValues();
            var all = _syncProductPackaging.GetProductPackaging(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<ProductPackagingTypeDTO> ProductPackagingType()
        {
            var query = GetSyncValues();
            var all = _syncProductPackagingType.GetProductPackagingType(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<ProductTypeDTO> ProductType()
        {
            var query = GetSyncValues();
            var all = _syncProductType.GetProductType(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<PromotionDiscountDTO> PromotionDiscount()
        {
            var query = GetSyncValues();
            var all = _syncPromotionDiscount.GetPromotionDiscount(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<ProvinceDTO> Province()
        {
            var query = GetSyncValues();
            var all = _syncProvince.GetProvince(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<RegionDTO> Region()
        {
            var query = GetSyncValues();
            var all = _syncRegion.GetRegion(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<ReorderLevelDTO> ReorderLevel()
        {
            var query = GetSyncValues();
            var all = _syncReorderLevel.GetReorderLevel(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<RetireSettingDTO> RetireSetting()
        {
            var query = GetSyncValues();
            var all = _syncRetireSetting.GetRetireSetting(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<ReturnableProductDTO> ReturnableProduct()
        {
            var query = GetSyncValues();
            var all = _syncReturnableProduct.GetReturnableProduct(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<RouteCentreAllocationDTO> RouteCentreAllocation()
        {
            var query = GetSyncValues();
            var all = _syncRouteCentreAllocation.GetRouteCentre(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<RouteCostCentreAllocationDTO> RouteCostCentreAllocation()
        {
            var query = GetSyncValues();
            var all = _syncRouteCostCentreAllocation.GetRouteCostCentre(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<RouteDTO> Route()
        {
            var query = GetSyncValues();
            var all = _syncRoute.GetRoute(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<RouteRegionAllocationDTO> RouteRegionAllocation()
        {
            var query = GetSyncValues();
            var all = _syncRouteRegionAllocation.GetRouteRegionAllocation(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<SaleProductDTO> SaleProduct()
        {
            var query = GetSyncValues();
            var all = _syncSaleProduct.GetSaleProduct(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<SalesmanRouteDTO> SalesmanRoute()
        {
            var query = GetSyncValues();
            var all = _syncSalesmanRoute.GetSalesmanRoute(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<SalesmanSupplierDTO> SalesmanSupplier()
        {
            var query = GetSyncValues();
            var all = _syncSalesmanSupplier.GetSalesmanSupplier(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<SaleValueDiscountDTO> SaleValueDiscount()
        {
            var query = GetSyncValues();
            var all = _syncSaleValueDiscount.GetSaleValueDiscount(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<AppSettingsDTO> Setting()
        {
            var query = GetSyncValues();
            var all = _syncSetting.GetAppSettings(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<SocioEconomicStatusDTO> SocioEconomicStatus()
        {
            var query = GetSyncValues();
            var all = _syncSocioEconomicStatus.GetEconomicStatus(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<SupplierDTO> Supplier()
        {
            var query = GetSyncValues();
            var all = _syncSupplier.GetSupplier(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<TargetItemDTO> TargetItem()
        {
            var query = GetSyncValues();
            var all = _syncTargetItem.GetTargetItem(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<TargetDTO> Target()
        {
            var query = GetSyncValues();
            var all = _syncTarget.GetTarget(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<TargetPeriodDTO> TargetPeriod()
        {
            var query = GetSyncValues();
            var all = _syncTargetPeriod.GetTargetPeriod(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<TerritoryDTO> Territory()
        {
            var query = GetSyncValues();
            var all = _syncTerritory.GetTerritory(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<UserDTO> User()
        {
            var query = GetSyncValuesWithPassChange();
            var all = _syncUser.GetUser(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<UserGroupDTO> UserGroup()
        {
            var query = GetSyncValues();
            var all = _syncUserGroup.GetUserGroup(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<UserGroupRoleDTO> UserGroupRole()
        {
            var query = GetSyncValues();
            var all = _syncUserGroupRole.GetUserGroupRole(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<VATClassDTO> VatClass()
        {
            var query = GetSyncValues();
            var all = _syncVatClass.GetVatClass(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<CommodityTypeDTO> CommodityType()
        {
            var query = GetSyncValues();
            var all = _syncCommodityType.GetCommodityType(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<CommodityDTO> Commodity()
        {
            var query = GetSyncValues();
            var all = _syncCommodity.GetCommodity(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<CentreTypeDTO> CentreType()
        {
            var query = GetSyncValues();
            var all = _syncCentreType.GetCentreType(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<CentreDTO> Centre()
        {
            var query = GetSyncValues();
            var all = _syncCentre.GetCentre(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<HubDTO> Hub()
        {
            var query = GetSyncValues();
            var all = _syncHub.GetHub(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<StoreDTO> Store()
        {
            var query = GetSyncValues();
            var all = _syncStore.GetStore(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<CommoditySupplierDTO> CommoditySupplier()
        {
            var query = GetSyncValues();
            var all = _syncCommoditySupplier.GetCommoditySupplier(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<CommodityProducerDTO> CommodityProducer()
        {
            var query = GetSyncValues();
            var all = _syncCommodityProducer.GetCommodityProducer(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<CommodityProducerCentreAllocationDTO> CommodityProducerCentreAllocation()
        {
            var query = GetSyncValues();
            var all = _syncCommodityProducerCentre.GetCommodityProducerCentre(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<CommodityOwnerTypeDTO> CommodityOwnerType()
        {
            var query = GetSyncValues();
            var all = _syncCommodityOwnerType.GetCommodityOwnerType(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<CommodityOwnerDTO> CommodityOwner()
        {
            var query = GetSyncValues();
            var all = _syncCommodityOwner.GetCommodityOwner(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<PurchasingClerkDTO> FieldClerk()
        {
            var query = GetSyncValues();
            var all = _syncFieldClerk.GetFieldClerk(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<ContainerTypeDTO> ContainerType()
        {
            var query = GetSyncValues();
            var all = _syncContainerType.GetContainerType(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<PrinterDTO> Printer()
        {
            var query = GetSyncValues();
            var all = _syncPrinter.GetPrinter(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<WeighScaleDTO> WeighScale()
        {
            var query = GetSyncValues();
            var all = _syncWeighScale.GetWeighScale(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<SourcingContainerDTO> SourcingContainer()
        {
            var query = GetSyncValues();
            var all = _syncSourcingContainer.GetSourcingContainer(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<VehicleDTO> Vehicle()
        {
            var query = GetSyncValues();
            var all = _syncVehicle.GetVehicle(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<ServiceDTO> Service()
        {
            var query = GetSyncValues();
            var all = _syncServicesMasterdataService.GetServices(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<ShiftDTO> Shift()
        {
            var query = GetSyncValues();
            var all = _syncShiftMasterDataService.GetShifts(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<SeasonDTO> Season()
        {
            var query = GetSyncValues();
            var all = _syncSeasonMasterDataService.GetSeasons(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<InfectionDTO> Infection()
        {
            var query = GetSyncValues();
            var all = _syncInfectionsMasterDataService.GetInfections(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<ServiceProviderDTO> ServiceProvider()
        {
            var query = GetSyncValues();
            var all = _syncServiceProviderMasterdataService.GetServiceProviders(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<ActivityTypeDTO> ActivityType()
        {
            var query = GetSyncValues();
            var all = _syncActivityTypeMasterDataService.GetActivityType(query);
            return all;
        }

        [HttpGet]
        public SyncResponseMasterDataInfo<OutletVisitReasonTypeDTO> OutletVisitReasonsType()
        {
            var query = GetSyncValues();
            var all = _syncOutletVisitReasonsTypeMasterDataService.GetOutletVisitReasonsType(query);
            return all;
        }

        private QueryMasterData GetSyncValues()
        {
            QueryMasterData query = new QueryMasterData();
            int take;
            int skip;
            Guid appId;
            DateTime syncTimeStamp = DateTime.Now;

            var parameters = this.Request.RequestUri.ParseQueryString();
            string search = parameters["search"];
            Guid.TryParse(parameters["costCentreApplicationId"], out appId);
            DateTime.TryParse(parameters["syncTimeStamp"], out syncTimeStamp);
            this.PagingParam(out take, out skip);
            query.ApplicationId = appId;
            query.From = syncTimeStamp;

            if (take != 0)
            {
                query.Skip = skip;
                query.Take = take;
            }
            if (!string.IsNullOrWhiteSpace(search))
            {
                query.Name = search;
                query.Description = search;
                //query.Skip = 0;
            }
            return query;
        }

        private QueryMasterData GetSyncValuesWithPassChange()
        {
            QueryMasterData query = new QueryMasterData();
            int take;
            int skip;
            Guid appId;
            DateTime syncTimeStamp = DateTime.Now;

            var parameters = this.Request.RequestUri.ParseQueryString();
            string search = parameters["search"];
            Guid.TryParse(parameters["costCentreApplicationId"], out appId);
            DateTime.TryParse(parameters["syncTimeStamp"], out syncTimeStamp);
            this.PagingParam(out take, out skip);
            query.ApplicationId = appId;
            query.PassChange = 1;
            query.From = syncTimeStamp;

            if (take != 0)
            {
                query.Skip = skip;
                query.Take = take;
            }
            if (!string.IsNullOrWhiteSpace(search))
            {
                query.Name = search;
                query.Description = search;
                //query.Skip = 0;
            }
            return query;
        }
    }
}
