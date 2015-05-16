using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Distributr.Core.Domain.Master;
using Distributr.Core.MasterDataDTO.DTOModels.FinancialDTO;
using Distributr.Core.MasterDataDTO.DTOModels.InventoriesDTO;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.CostCentre;
using Distributr.Core.Utility.MasterData;
using  Distributr.Core.Utility;
using Distributr.WSAPI.Lib.ResponseBuilders.MasterData;
using log4net;

namespace Distributr.WSAPI.Lib.Services.Sync.Impl
{
    public class MasterDataZipperService : IMasterDataZipperService
    {
        ILog _log = LogManager.GetLogger("MasterDataZipperService");
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
         private IPullMasterDataResponseBuilder _pullMasterDataResponseBuilder;
        private int threshold = 500;

        public MasterDataZipperService(ISyncAreaMasterDataService syncArea,
            ISyncAssetCategoryMasterDataService syncAssetCategory, ISyncAssetMasterDataService syncAsset,
            ISyncAssetStatusMasterDataService syncAssetStatus, ISyncAssetTypeMasterDataService syncAssetType,
            ISyncBankBranchMasterDataService syncBankBranch, ISyncBankMasterDataService syncBank,
            ISyncCompetitorMasterDataService syncCompetitor,
            ISyncCompetitorProductMasterDataService syncCompetitorProduct,
            ISyncConsolidatedProductMasterDataService syncConsolidatedProduct, ISyncContactMasterDataService syncContact,
            ISyncContactTypeMasterDataService syncContactType, ISyncCountryMasterDataService syncCountry,
            ISyncCvcpDiscountMasterDataService syncCvcpDiscount, ISyncDiscountGroupMasterDataService syncDiscountGroup,
            ISyncDistributorMasterDataService syncDistributor,
            ISyncDistributorPendingDispatchWarehouseService syncDistributorPendingDispatchWarehouse,
            ISyncDistributorSalesmanMasterDataService syncDistributorSalesman,
            ISyncDistrictMasterDataService syncDistrict,
            ISyncFreeOfChargeDiscountMasterDataService syncFreeOfChargeDiscount,
            ISyncOutletCategoryMasterDataService syncOutletCategory, ISyncOutletMasterDataService syncOutlet,
            ISyncOutletPriorityMasterDataService syncOutletPriority, ISyncOutletTypeMasterDataService syncOutletType,
            ISyncOutletVisitDayMasterDataService syncOutletVisitDay, ISyncPricingMasterDataService syncPricing,
            ISyncPricingTierMasterDataService syncPricingTier, ISyncProducerMasterDataService syncProducer,
            ISyncProductBrandMasterDataService syncProductBrand,
            ISyncProductDiscountMasterDataService syncProductDiscount,
            ISyncProductFlavourMasterDataService syncProductFlavour,
            ISyncProductGroupDiscountMasterDataService syncProductGroupDiscount,
            ISyncProductPackagingMasterDataService syncProductPackaging,
            ISyncProductPackagingTypeMasterDataService syncProductPackagingType,
            ISyncProductTypeMasterDataService syncProductType,
            ISyncPromotionDiscountMasterDataService syncPromotionDiscount, ISyncProvinceMasterDataService syncProvince,
            ISyncRegionMasterDataService syncRegion, ISyncReorderLevelMasterDataService syncReorderLevel,
            ISyncRetireSettingMasterDataService syncRetireSetting,
            ISyncReturnableProductMasterDataService syncReturnableProduct,
            ISyncRouteCentreMasterDataService syncRouteCentreAllocation,
            ISyncRouteCostCentreMasterDataService syncRouteCostCentreAllocation, ISyncRouteMasterDataService syncRoute,
            ISyncRouteRegionAllocationMasterDataService syncRouteRegionAllocation,
            ISyncSaleProductMasterDataService syncSaleProduct, ISyncSalesmanRouteMasterDataService syncSalesmanRoute,
            ISyncSalesmanSupplierMasterDataService syncSalesmanSupplier,
            ISyncSaleValueDiscountMasterDataService syncSaleValueDiscount, ISyncSettingMasterDataService syncSetting,
            ISyncSocioEconomicStatusMasterDataService syncSocioEconomicStatus,
            ISyncSupplierMasterDataService syncSupplier, ISyncTargetItemMasterDataService syncTargetItem,
            ISyncTargetMasterDataService syncTarget, ISyncTargetPeriodMasterDataService syncTargetPeriod,
            ISyncTerritoryMasterDataService syncTerritory, ISyncUserMasterDataService syncUser,
            ISyncUserGroupMasterDataService syncUserGroup, ISyncUserGroupRoleMasterDataService syncUserGroupRole,
            ISyncVatClassMasterDataService syncVatClass, ISyncCommodityTypeMasterDataService syncCommodityType,
            ISyncCommodityMasterDataService syncCommodityMasterDataService,
            ISyncCentreTypeMasterDataService syncCentreType, ISyncCentreMasterDataService syncCentre,
            ISyncHubMasterDataService syncHub, ISyncStoreMasterDataService syncStore,
            ISyncCommodityMasterDataService syncCommodity, ISyncCommoditySupplierMasterDataService syncCommoditySupplier,
            ISyncCommodityProducerMasterDataService syncCommodityProducer,
            ISyncCommodityProducerCentreMasterDataService syncCommodityProducerCentre,
            ISyncCommodityOwnerTypeMasterDataService syncCommodityOwnerType,
            ISyncCommodityOwnerMasterDataService syncCommodityOwner, ISyncFieldClerkMasterDataService syncFieldClerk,
            ISyncContainerTypeMasterDataService syncContainerType, ISyncPrinterMasterDataService syncPrinter,
            ISyncWeighScaleMasterDataService syncWeighScale,
            ISyncSourcingContainerMasterDataService syncSourcingContainer, ISyncVehicleMasterDataService syncVehicle,
            ISyncUnderBankingService syncUnderBankingService,
            ISyncServiceProviderMasterdataService syncServiceProviderMasterdataService,
            ISyncServicesMasterdataService syncServicesMasterdataService,
            ISyncShiftMasterDataService syncShiftMasterDataService,
            ISyncSeasonMasterDataService syncSeasonMasterDataService,
            ISyncInfectionsMasterDataService syncInfectionsMasterDataService,
            ISyncActivityTypeMasterDataService syncActivityTypeMasterDataService,
            ISyncOutletVisitReasonsTypeMasterDataService syncOutletVisitReasonsTypeMasterDataService,
            ISyncChannelPackagingMasterDataService channelPackagingMasterDataService, IPullMasterDataResponseBuilder pullMasterDataResponseBuilder)
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
            _syncPrinter = syncPrinter;
            _syncWeighScale = syncWeighScale;
            _syncSourcingContainer = syncSourcingContainer;
            _syncVehicle = syncVehicle;
            _syncUnderBankingService = syncUnderBankingService;
            _syncServiceProviderMasterdataService = syncServiceProviderMasterdataService;
            _syncServicesMasterdataService = syncServicesMasterdataService;
            _syncShiftMasterDataService = syncShiftMasterDataService;
            _syncSeasonMasterDataService = syncSeasonMasterDataService;
            _syncInfectionsMasterDataService = syncInfectionsMasterDataService;
            _syncActivityTypeMasterDataService = syncActivityTypeMasterDataService;
            _syncOutletVisitReasonsTypeMasterDataService = syncOutletVisitReasonsTypeMasterDataService;
            _channelPackagingMasterDataService = channelPackagingMasterDataService;
            _pullMasterDataResponseBuilder = pullMasterDataResponseBuilder;
        }

        public string CreateCsvAndZip(QueryMasterData myQuery)
        {
            string folder = HttpContext.Current.Server.MapPath("~/download/" + myQuery.ApplicationId + "/");
            if (Directory.Exists(folder) )
            {
                DirectoryInfo directory = new DirectoryInfo(folder);
                foreach (FileInfo file in directory.GetFiles()) file.Delete();
                foreach (DirectoryInfo subDirectory in directory.GetDirectories()) subDirectory.Delete(true);
            }
            Directory.CreateDirectory(folder);
            // MasterDataCollective.Country);
            CreateCountryCsv(myQuery, folder);

            //MasterDataCollective.Territory;
            CreateTerritoryCsv(myQuery, folder);

            //MasterDataCollective.Region);
            CreateRegionCsv(myQuery, folder);

            //MasterDataCollective.Area);
            CreateAreaCsv(myQuery, folder);

            //MasterDataCollective.Province);
            CreateProvinceCsv(myQuery, folder);

            //MasterDataCollective.District);
            CreateDistrictCsv(myQuery, folder);

            //MasterDataCollective.SocioEconomicStatus);
            CreateSocioEconomicStatusCsv(myQuery, folder);
            //MasterDataCollective.ContactType);
            CreateContactTypeCsv(myQuery, folder);

            //MasterDataCollective.OutletCategory);
            CreateOutletCategoryCsv(myQuery, folder);
            //MasterDataCollective.OutletType);
            CreateOutletTypeCsv(myQuery, folder);
            //MasterDataCollective.Bank);
            CreateBankCsv(myQuery, folder);
            //MasterDataCollective.BankBranch);
            CreateBankBranchCsv(myQuery, folder);
            //MasterDataCollective.AssetStatus);
            CreateAssetStatusCsv(myQuery, folder);
            //MasterDataCollective.AssetType);
            CreateAssetTypeCsv(myQuery, folder);
            //MasterDataCollective.AssetCategory);
            CreateAssetCategoryCsv(myQuery, folder);
            //MasterDataCollective.Asset);
            CreateAssetCsv(myQuery, folder);
            //MasterDataCollective.Setting);
            CreateSettingCsv(myQuery, folder);
            //MasterDataCollective.RetireSetting);
            CreateRetireSettingCsv(myQuery, folder);

            //MasterDataCollective.UserGroup);
            //MasterDataCollective.UserGroupRole);
            //MasterDataCollective.Supplier);
            CreateSupplierCsv(myQuery, folder);

            //MasterDataCollective.PricingTier);
            CreatePricingTierCsv(myQuery, folder);
            //MasterDataCollective.VatClass);
            CreateVatClassCsv(myQuery, folder);
            //MasterDataCollective.ProductType);
            CreateProductTypeCsv(myQuery, folder);
            //MasterDataCollective.ProductBrand);
            CreateProductBrandCsv(myQuery, folder);
            //MasterDataCollective.ProductFlavour);
            CreateProductFlavourCsv(myQuery, folder);
            //MasterDataCollective.ProductPackagingType);
            CreateProductPackagingTypeCsv(myQuery, folder);

            //MasterDataCollective.ProductPackaging);
            CreateProductPackagingCsv(myQuery, folder);
            //MasterDataCollective.ReturnableProduct);
            CreateReturnableProductCsv(myQuery, folder);
            //MasterDataCollective.SaleProduct);
            CreateSaleProductCsv(myQuery, folder);
          //MasterDataCollective.ChannelPackaging);
            CreateChannelPackagingCsv(myQuery, folder);

            //MasterDataCollective.Pricing);
            CreatePricingCsv(myQuery, folder);
            //MasterDataCollective.DiscountGroup);
            CreateDiscountGroupCsv(myQuery, folder);
            //MasterDataCollective.ProductGroupDiscount);
            CreateProductGroupDiscountCsv(myQuery, folder);
            //MasterDataCollective.ProductDiscount);
            CreateProductDiscountCsv(myQuery, folder);
            //MasterDataCollective.SaleValueDiscount);
            CreateSaleValueDiscountCsv(myQuery, folder);
            //MasterDataCollective.PromotionDiscount);
            CreatePromotionDiscountCsv(myQuery, folder);
            //MasterDataCollective.CertainValueCertainProductDiscount);
            CreateCertainValueCertainProductDiscountCsv(myQuery, folder);
            //MasterDataCollective.FreeOfChargeDiscount);
            CreateFreeOfChargeDiscountCsv(myQuery, folder);

            //MasterDataCollective.Producer);
            CreateProducerCsv(myQuery, folder);
            //MasterDataCollective.Distributor);
            CreateDistributorCsv(myQuery, folder);
            //MasterDataCollective.DistributorPendingDispatchWarehouse);
            CreateDistributorPendingDispatchWarehouseCsv(myQuery, folder);
            //MasterDataCollective.DistributorSalesman);
            CreateDistributorSalesmanCsv(myQuery, folder);
            //MasterDataCollective.Route); //check
            CreateRouteCsv(myQuery, folder);
            //MasterDataCollective.Outlet);
            CreateOutletCsv(myQuery, folder);
            //MasterDataCollective.OutletVisitDay);
            CreateOutletVisitDayCsv(myQuery, folder);
            //MasterDataCollective.OutletPriority);
            CreateOutletPriorityCsv(myQuery, folder);

            //MasterDataCollective.Competitor);
            CreateCompetitorCsv(myQuery, folder);
            //MasterDataCollective.User);
            CreateUserCsv(myQuery, folder);
            //MasterDataCollective.SalesmanRoute);
            CreateSalesmanRouteCsv(myQuery, folder);
            //MasterDataCollective.SalesmanSupplier);
            CreateSalesmanSupplierCsv(myQuery, folder);
            //MasterDataCollective.Contact);
            CreateContactCsv(myQuery, folder);

            //MasterDataCollective.TargetPeriod);
            CreateTargetPeriodCsv(myQuery, folder);

            //MasterDataCollective.Target);
            CreateTargetCsv(myQuery, folder);
            //MasterDataCollective.TargetItem);
            CreateTargetItemCsv(myQuery, folder);
            //MasterDataCollective.ReorderLevel);
            CreateReorderLevelCsv(myQuery, folder);
            //MasterDataCollective.CompetitorProduct);
            CreateCompetitorProductCsv(myQuery, folder);


            //MasterDataCollective.RouteRegionAllocation);
            CreateRouteRegionAllocationCsv(myQuery, folder);
            //MasterDataCollective.RouteCostCentreAllocation);
            CreateRouteCostCentreAllocationCsv(myQuery, folder);
            //MasterDataCollective.OutletVisitReasonsType);
            CreateOutletVisitReasonsTypenCsv(myQuery, folder);
                //Financials
            //typeof (UnderBankingDTO),
            CreateUnderBankingCsv(myQuery, folder);
           // typeof (PaymentTrackerDTO),
            CreatePaymentTrackerCsv(myQuery, folder);

            //Inventory
           // typeof (InventoryDTO)
            CreateInventoryCsv(myQuery, folder);


            string zipPath = HttpContext.Current.Server.MapPath("~/download/" + myQuery.ApplicationId + ".zip");
            if (File.Exists(zipPath))
            {
                File.Delete(zipPath);
            }
            ZipFile.CreateFromDirectory(folder, zipPath);
            return zipPath;
        }

        private void CreateChannelPackagingCsv(QueryMasterData myQuery, string folder)
        {
            var entityName = MasterDataCollective.ChannelPackaging;
            int i = 0;
            myQuery.Skip = 0;
            while (true)
            {

                myQuery.Take = threshold;
                var result = _channelPackagingMasterDataService.GetChannelPackaging(myQuery);
                var data = result.MasterData.MasterDataItems;
                if (data == null || data.Count() == 0)
                {
                    break;
                }
                string path = folder + entityName + "_" + i + ".csv";
                data.WriteCSV(path);

                myQuery.Skip += threshold;
                i++;
            }    
        }

        private void CreateInventoryCsv(QueryMasterData myQuery, string folder)
        {
            var entityName = "Inventory";
            int i = 0;
            myQuery.Skip = 0;
            while (true)
            {

                myQuery.Take = threshold;
                var result = _pullMasterDataResponseBuilder.GetInventory(myQuery.ApplicationId);
                var data = result.MasterData.MasterDataItems.OfType<InventoryDTO>().ToList();
                if (data == null || data.Count() == 0)
                {
                    break;
                }
                string path = folder + entityName + "_" + i + ".csv";
                data.WriteCSV(path);

                myQuery.Skip += threshold;
                break;
                i++;
            }    
        }

        private void CreatePaymentTrackerCsv(QueryMasterData myQuery, string folder)
        {
             var entityName = "PaymentTracker";
            int i = 0;
            myQuery.Skip = 0;
            while (true)
            {

                myQuery.Take = threshold;
                var result = _pullMasterDataResponseBuilder.GetPayments(myQuery.ApplicationId);
                var data = result.MasterData.MasterDataItems.OfType<PaymentTrackerDTO>().ToList();
                if (data == null || data.Count() == 0)
                {
                    break;
                }
                string path = folder + entityName + "_" + i + ".csv";
                data.WriteCSV(path);
                break;
                myQuery.Skip += threshold;
                i++;
            }    
        
        }

        private void CreateUnderBankingCsv(QueryMasterData myQuery, string folder)
        {
            var entityName = "UnderBanking";
            int i = 0;
            myQuery.Skip = 0;
            while (true)
            {

                myQuery.Take = threshold;
                var result = _syncUnderBankingService.GetUnderBanking(myQuery);
                var data = result.MasterData.MasterDataItems;
                if (data == null || data.Count() == 0)
                {
                    break;
                }
                string path = folder + entityName + "_" + i + ".csv";
                data.WriteCSV(path);
                var items = data.SelectMany(s => s.Items).ToArray();
                path = folder + entityName + "_Items_" + i + ".csv";
                GenerateCsv(path, items);
                myQuery.Skip += threshold;
                i++;
            }    
        }

        private void CreateCompetitorCsv(QueryMasterData myQuery, string folder)
        {
            var entityName = MasterDataCollective.Competitor;
            int i = 0;
            myQuery.Skip = 0;
            while (true)
            {

                myQuery.Take = threshold;
                var result = _syncCompetitor.GetCompetitor(myQuery);
                var data = result.MasterData.MasterDataItems;
                if (data == null || data.Count() == 0)
                {
                    break;
                }
                string path = folder + entityName + "_" + i + ".csv";
                data.WriteCSV(path);

                myQuery.Skip += threshold;
                i++;
            }    
        }

        private void CreateOutletPriorityCsv(QueryMasterData myQuery, string folder)
        {
            var entityName = MasterDataCollective.OutletPriority;
            int i = 0;
            myQuery.Skip = 0;
            while (true)
            {

                myQuery.Take = threshold;
                var result = _syncOutletPriority.GetOutletPriority(myQuery);
                var data = result.MasterData.MasterDataItems;
                if (data == null || data.Count() == 0)
                {
                    break;
                }
                string path = folder + entityName + "_" + i + ".csv";
                data.WriteCSV(path);

                myQuery.Skip += threshold;
                i++;
            }    
        }

        private void CreateOutletVisitDayCsv(QueryMasterData myQuery, string folder)
        {
            var entityName = MasterDataCollective.OutletVisitDay;
            int i = 0;
            myQuery.Skip = 0;
            while (true)
            {

                myQuery.Take = threshold;
                var result = _syncOutletVisitDay.GetOutletVisitDay(myQuery);
                var data = result.MasterData.MasterDataItems;
                if (data == null || data.Count() == 0)
                {
                    break;
                }
                string path = folder + entityName + "_" + i + ".csv";
                data.WriteCSV(path);

                myQuery.Skip += threshold;
                i++;
            }    
        }

        private void CreateOutletVisitReasonsTypenCsv(QueryMasterData myQuery, string folder)
        {
            var entityName = MasterDataCollective.OutletVisitReasonsType;
            int i = 0;
            myQuery.Skip = 0;
            while (true)
            {

                myQuery.Take = threshold;
                var result = _syncOutletVisitReasonsTypeMasterDataService.GetOutletVisitReasonsType(myQuery);
                var data = result.MasterData.MasterDataItems;
                if (data == null || data.Count() == 0)
                {
                    break;
                }
                string path = folder + entityName + "_" + i + ".csv";
                data.WriteCSV(path);

                myQuery.Skip += threshold;
                i++;
            }    
        }

        private void CreateRouteCostCentreAllocationCsv(QueryMasterData myQuery, string folder)
        {
            var entityName = MasterDataCollective.RouteCostCentreAllocation;
            int i = 0;
            myQuery.Skip = 0;
            while (true)
            {

                myQuery.Take = threshold;
                var result = _syncRouteCostCentreAllocation.GetRouteCostCentre(myQuery);
                var data = result.MasterData.MasterDataItems;
                if (data == null || data.Count() == 0)
                {
                    break;
                }
                string path = folder + entityName + "_" + i + ".csv";
                data.WriteCSV(path);

                myQuery.Skip += threshold;
                i++;
            }    
        }

        private void CreateRouteRegionAllocationCsv(QueryMasterData myQuery, string folder)
        {
            var entityName = MasterDataCollective.RouteRegionAllocation;
            int i = 0;
            myQuery.Skip = 0;
            while (true)
            {

                myQuery.Take = threshold;
                var result = _syncRouteRegionAllocation.GetRouteRegionAllocation(myQuery);
                var data = result.MasterData.MasterDataItems;
                if (data == null || data.Count() == 0)
                {
                    break;
                }
                string path = folder + entityName + "_" + i + ".csv";
                data.WriteCSV(path);

                myQuery.Skip += threshold;
                i++;
            }    
        }

        private void CreateCompetitorProductCsv(QueryMasterData myQuery, string folder)
        {
            var entityName = MasterDataCollective.CompetitorProduct;
            int i = 0;
            myQuery.Skip = 0;
            while (true)
            {

                myQuery.Take = threshold;
                var result = _syncCompetitorProduct.GetCompetitorProduct(myQuery);
                var data = result.MasterData.MasterDataItems;
                if (data == null || data.Count() == 0)
                {
                    break;
                }
                string path = folder + entityName + "_" + i + ".csv";
                data.WriteCSV(path);

                myQuery.Skip += threshold;
                i++;
            }    
        }

        private void CreateReorderLevelCsv(QueryMasterData myQuery, string folder)
        {
            var entityName = MasterDataCollective.ReorderLevel;
            int i = 0;
            myQuery.Skip = 0;
            while (true)
            {

                myQuery.Take = threshold;
                var result = _syncReorderLevel.GetReorderLevel(myQuery);
                var data = result.MasterData.MasterDataItems;
                if (data == null || data.Count() == 0)
                {
                    break;
                }
                string path = folder + entityName + "_" + i + ".csv";
                data.WriteCSV(path);

                myQuery.Skip += threshold;
                i++;
            }    
        }

        private void CreateTargetItemCsv(QueryMasterData myQuery, string folder)
        {
            var entityName = MasterDataCollective.TargetItem;
            int i = 0;
            myQuery.Skip = 0;
            while (true)
            {

                myQuery.Take = threshold;
                var result = _syncTargetItem.GetTargetItem(myQuery);
                var data = result.MasterData.MasterDataItems;
                if (data == null || data.Count() == 0)
                {
                    break;
                }
                string path = folder + entityName + "_" + i + ".csv";
                data.WriteCSV(path);

                myQuery.Skip += threshold;
                i++;
            }    
        }

        private void CreateTargetCsv(QueryMasterData myQuery, string folder)
        {
            var entityName = MasterDataCollective.Target;
            int i = 0;
            myQuery.Skip = 0;
            while (true)
            {

                myQuery.Take = threshold;
                var result = _syncTarget.GetTarget(myQuery);
                var data = result.MasterData.MasterDataItems;
                if (data == null || data.Count() == 0)
                {
                    break;
                }
                string path = folder + entityName + "_" + i + ".csv";
                data.WriteCSV(path);

                myQuery.Skip += threshold;
                i++;
            }    
        }

        private void CreateTargetPeriodCsv(QueryMasterData myQuery, string folder)
        {
            var entityName = MasterDataCollective.TargetPeriod;
            int i = 0;
            myQuery.Skip = 0;
            while (true)
            {

                myQuery.Take = threshold;
                var result = _syncTargetPeriod.GetTargetPeriod(myQuery);
                var data = result.MasterData.MasterDataItems;
                if (data == null || data.Count() == 0)
                {
                    break;
                }
                string path = folder + entityName + "_" + i + ".csv";
                data.WriteCSV(path);

                myQuery.Skip += threshold;
                i++;
            }    
        }

        private void CreateContactCsv(QueryMasterData myQuery, string folder)
        {
            var entityName = MasterDataCollective.Contact;
            int i = 0;
            myQuery.Skip = 0;
            while (true)
            {

                myQuery.Take = threshold;
                var result = _syncContact.GetContact(myQuery);
                var data = result.MasterData.MasterDataItems;
                if (data == null || data.Count() == 0)
                {
                    break;
                }
                string path = folder + entityName + "_" + i + ".csv";
                data.WriteCSV(path);

                myQuery.Skip += threshold;
                i++;
            }    
        }

        private void CreateSalesmanSupplierCsv(QueryMasterData myQuery, string folder)
        {
            var entityName = MasterDataCollective.SalesmanSupplier;
            int i = 0;
            myQuery.Skip = 0;
            while (true)
            {

                myQuery.Take = threshold;
                var result = _syncSalesmanSupplier.GetSalesmanSupplier(myQuery);
                var data = result.MasterData.MasterDataItems;
                if (data==null || data.Count() == 0)
                {
                    break;
                }
                string path = folder + entityName + "_" + i + ".csv";
                data.WriteCSV(path);

                myQuery.Skip += threshold;
                i++;
            }
        }

        private void CreateSalesmanRouteCsv(QueryMasterData myQuery, string folder)
        {
            var entityName = MasterDataCollective.SalesmanRoute;
            int i = 0;
            myQuery.Skip = 0;
            while (true)
            {

                myQuery.Take = threshold;
                var result = _syncSalesmanRoute.GetSalesmanRoute(myQuery);
                var data = result.MasterData.MasterDataItems;
                if (data==null || data.Count() == 0)
                {
                    break;
                }
                string path = folder + entityName + "_" + i + ".csv";
                data.WriteCSV(path);

                myQuery.Skip += threshold;
                i++;
            }
        }

        private void CreateUserCsv(QueryMasterData myQuery, string folder)
        {
            var entityName = MasterDataCollective.User;
            int i = 0;
            myQuery.Skip = 0;
            while (true)
            {

                myQuery.Take = threshold;
                var result = _syncUser.GetUser(myQuery);
                var data = result.MasterData.MasterDataItems;
                if (data==null || data.Count() == 0)
                {
                    break;
                }
                string path = folder + entityName + "_" + i + ".csv";
                data.WriteCSV(path);

                myQuery.Skip += threshold;
                i++;
            }
        }

        private void CreateRouteCsv(QueryMasterData myQuery, string folder)
        {
            var entityName = MasterDataCollective.Route;
            int i = 0;
            myQuery.Skip = 0;
            while (true)
            {

                myQuery.Take = threshold;
                var result = _syncRoute.GetRoute(myQuery);
                var data = result.MasterData.MasterDataItems;
                if (data==null || data.Count() == 0)
                {
                    break;
                }
                string path = folder + entityName + "_" + i + ".csv";
                data.WriteCSV(path);

                myQuery.Skip += threshold;
                i++;
            }
        }

        private void CreateDistributorSalesmanCsv(QueryMasterData myQuery, string folder)
        {
            var entityName = MasterDataCollective.DistributorSalesman;
            int i = 0;
            myQuery.Skip = 0;
            while (true)
            {

                myQuery.Take = threshold;
                var result = _syncDistributorSalesman.GetDistributorSalesman(myQuery);
                var data = result.MasterData.MasterDataItems;
                if (data==null || data.Count() == 0)
                {
                    break;
                }
                string path = folder + entityName + "_" + i + ".csv";
                data.WriteCSV(path);

                myQuery.Skip += threshold;
                i++;
            }
        }

        private void CreateDistributorCsv(QueryMasterData myQuery, string folder)
        {
            var entityName = MasterDataCollective.Distributor;
            int i = 0;
            myQuery.Skip = 0;
            while (true)
            {

                myQuery.Take = threshold;
                var result = _syncDistributor.GetDistributor(myQuery);
                var data = result.MasterData.MasterDataItems;
                if (data==null || data.Count() == 0)
                {
                    break;
                }
                string path = folder + entityName + "_" + i + ".csv";
                data.WriteCSV(path);

                myQuery.Skip += threshold;
                i++;
            }
        }

        private void CreateDistributorPendingDispatchWarehouseCsv(QueryMasterData myQuery, string folder)
        {
            var entityName = MasterDataCollective.DistributorPendingDispatchWarehouse;
            int i = 0;
            myQuery.Skip = 0;
            while (true)
            {

                myQuery.Take = threshold;
                var result = _syncDistributorPendingDispatchWarehouse.GetDistributorPendingDispatchWarehouse(myQuery);
                var data = result.MasterData.MasterDataItems;
                if (data==null || data.Count() == 0)
                {
                    break;
                }
                string path = folder + entityName + "_" + i + ".csv";
                data.WriteCSV(path);

                myQuery.Skip += threshold;
                i++;
            }
        }

        private void CreateProducerCsv(QueryMasterData myQuery, string folder)
        {
            var entityName = MasterDataCollective.Producer;
            int i = 0;
            myQuery.Skip = 0;
            while (true)
            {

                myQuery.Take = threshold;
                var result = _syncProducer.GetProducer(myQuery);
                var data = result.MasterData.MasterDataItems;
                if (data==null || data.Count() == 0)
                {
                    break;
                }
                string path = folder + entityName + "_" + i + ".csv";
                data.WriteCSV(path);

                myQuery.Skip += threshold;
                i++;
            }
        }

        private void CreateFreeOfChargeDiscountCsv(QueryMasterData myQuery, string folder)
        {
            var entityName = MasterDataCollective.FreeOfChargeDiscount;
            int i = 0;
            myQuery.Skip = 0;
            while (true)
            {

                myQuery.Take = threshold;
                var result = _syncFreeOfChargeDiscount.GetFreeOfChargeDiscount(myQuery);
                var data = result.MasterData.MasterDataItems;
                if (data==null || data.Count() == 0)
                {
                    break;
                }
                string path = folder + entityName + "_" + i + ".csv";
                data.WriteCSV(path);

                myQuery.Skip += threshold;
                i++;
            }
        }

        private void CreateCertainValueCertainProductDiscountCsv(QueryMasterData myQuery, string folder)
        {
            var entityName = MasterDataCollective.CertainValueCertainProductDiscount;
            int i = 0;
            myQuery.Skip = 0;
            while (true)
            {

                myQuery.Take = threshold;
                var result = _syncCvcpDiscount.GetCvcpDiscount(myQuery);
                var data = result.MasterData.MasterDataItems;
                if (data==null || data.Count() == 0)
                {
                    break;
                }
                string path = folder + entityName + "_" + i + ".csv";
                data.WriteCSV(path);
                var items = data.SelectMany(s => s.CertainValueCertainProductDiscountItems).ToArray();
                path = folder + entityName + "_Items_" + i + ".csv";
                GenerateCsv(path, items);
                myQuery.Skip += threshold;
                i++;
            }
        }

        private void CreatePromotionDiscountCsv(QueryMasterData myQuery, string folder)
        {
            var entityName = MasterDataCollective.PromotionDiscount;
            int i = 0;
            myQuery.Skip = 0;
            while (true)
            {

                myQuery.Take = threshold;
                var result = _syncPromotionDiscount.GetPromotionDiscount(myQuery);
                var data = result.MasterData.MasterDataItems;
                if (data==null || data.Count() == 0)
                {
                    break;
                }
                string path = folder + entityName + "_" + i + ".csv";
                data.WriteCSV(path);
                var items = data.SelectMany(s => s.PromotionDiscountItems).ToArray();
                path = folder + entityName + "_Items_" + i + ".csv";
                GenerateCsv(path, items);
                myQuery.Skip += threshold;
                i++;
            }
        }

        private void CreateSaleValueDiscountCsv(QueryMasterData myQuery, string folder)
        {
            var entityName = MasterDataCollective.SaleValueDiscount;
            int i = 0;
            myQuery.Skip = 0;
            while (true)
            {

                myQuery.Take = threshold;
                var result = _syncSaleValueDiscount.GetSaleValueDiscount(myQuery);
                var data = result.MasterData.MasterDataItems;
                if (data==null || data.Count() == 0)
                {
                    break;
                }
                string path = folder + entityName + "_" + i + ".csv";
                data.WriteCSV(path);
                var items = data.SelectMany(s => s.DiscountItems).ToArray();
                path = folder + entityName + "_Items_" + i + ".csv";
                GenerateCsv(path, items);
                myQuery.Skip += threshold;
                i++;
            }
        }

        private void CreateProductDiscountCsv(QueryMasterData myQuery, string folder)
        {
            var entityName = MasterDataCollective.ProductDiscount;
            int i = 0;
            myQuery.Skip = 0;
            while (true)
            {

                myQuery.Take = threshold;
                var result = _syncProductDiscount.GetProductDiscount(myQuery);
                var data = result.MasterData.MasterDataItems;
                if (data==null || data.Count() == 0)
                {
                    break;
                }
                string path = folder + entityName + "_" + i + ".csv";
                data.WriteCSV(path);
                var items = data.SelectMany(s => s.DiscountItem).ToArray();
                path = folder + entityName + "_Items_" + i + ".csv";
                GenerateCsv(path, items);
                myQuery.Skip += threshold;
                i++;
            }
        }

        private void CreateProductGroupDiscountCsv(QueryMasterData myQuery, string folder)
        {
            var entityName = MasterDataCollective.ProductGroupDiscount;
            int i = 0;
            myQuery.Skip = 0;
            while (true)
            {

                myQuery.Take = threshold;
                var result = _syncProductGroupDiscount.GetProductGroupDiscount(myQuery);
                var data = result.MasterData.MasterDataItems;
                if (data==null ||  data.Count() == 0)
                {
                    break;
                }
                string path = folder + entityName + "_" + i + ".csv";
                data.WriteCSV(path);
               
                myQuery.Skip += threshold;
                i++;
            }
        }

        private void CreateDiscountGroupCsv(QueryMasterData myQuery, string folder)
        {
            var entityName = MasterDataCollective.DiscountGroup;
            int i = 0;
            myQuery.Skip = 0;
            while (true)
            {

                myQuery.Take = threshold;
                var result = _syncDiscountGroup.GetDiscountGroup(myQuery);
                var data = result.MasterData.MasterDataItems;
                if (data==null || data.Count() == 0)
                {
                    break;
                }
                string path = folder + entityName + "_" + i + ".csv";
                data.WriteCSV(path);
               
                myQuery.Skip += threshold;
                i++;
            }
        }

        private void CreatePricingCsv(QueryMasterData myQuery, string folder)
        {
            var entityName = MasterDataCollective.Pricing;
            int i = 0;
            myQuery.Skip = 0;
            while (true)
            {

                myQuery.Take = threshold;
                var result = _syncPricing.GetPricing(myQuery);
                var data = result.MasterData.MasterDataItems;
                if (data==null || data.Count() == 0)
                {
                    break;
                }
                string path = folder + entityName + "_" + i + ".csv";
                data.WriteCSV(path);
                var shipto = data.SelectMany(s => s.ProductPricingItems).ToArray();
                path = folder + entityName + "_Items_" + i + ".csv";
                GenerateCsv(path, shipto);
                myQuery.Skip += threshold;
                i++;
            }
        }

        private void CreateSaleProductCsv(QueryMasterData myQuery, string folder)
        {
            var entityName = MasterDataCollective.SaleProduct;
            int i = 0;
            myQuery.Skip = 0;
            while (true)
            {

                myQuery.Take = threshold;
                var result = _syncSaleProduct.GetSaleProduct(myQuery);
                var data = result.MasterData.MasterDataItems;
                if (data==null || data.Count() == 0)
                {
                    break;
                }
                string path = folder + entityName + "_" + i + ".csv";
                data.WriteCSV(path);
                myQuery.Skip += threshold;
                i++;
            }
        }

        private void CreateReturnableProductCsv(QueryMasterData myQuery, string folder)
        {
            var entityName = MasterDataCollective.ReturnableProduct;
            int i = 0;
            myQuery.Skip = 0;
            while (true)
            {

                myQuery.Take = threshold;
                var result = _syncReturnableProduct.GetReturnableProduct(myQuery);
                var data = result.MasterData.MasterDataItems;
                if (data==null || data.Count() == 0)
                {
                    break;
                }
                string path = folder + entityName + "_" + i + ".csv";
                data.WriteCSV(path);
                myQuery.Skip += threshold;
                i++;
            }
        }

        private void CreateProductPackagingCsv(QueryMasterData myQuery, string folder)
        {
            var entityName = MasterDataCollective.ProductPackaging;
            int i = 0;
            myQuery.Skip = 0;
            while (true)
            {

                myQuery.Take = threshold;
                var result = _syncProductPackaging.GetProductPackaging(myQuery);
                var data = result.MasterData.MasterDataItems;
                if (data==null || data.Count() == 0)
                {
                    break;
                }
                string path = folder + entityName + "_" + i + ".csv";
                data.WriteCSV(path);
                myQuery.Skip += threshold;
                i++;
            }
        }

        private void CreateProductPackagingTypeCsv(QueryMasterData myQuery, string folder)
        {
            var entityName = MasterDataCollective.ProductPackagingType;
            int i = 0;
            myQuery.Skip = 0;
            while (true)
            {

                myQuery.Take = threshold;
                var result = _syncProductPackagingType.GetProductPackagingType(myQuery);
                var data = result.MasterData.MasterDataItems;
                if (data==null || data.Count() == 0)
                {
                    break;
                }
                string path = folder + entityName + "_" + i + ".csv";
                data.WriteCSV(path);
                myQuery.Skip += threshold;
                i++;
            }
        }

        private void CreateProductFlavourCsv(QueryMasterData myQuery, string folder)
        {
            var entityName = MasterDataCollective.ProductFlavour;
            int i = 0;
            myQuery.Skip = 0;
            while (true)
            {

                myQuery.Take = threshold;
                var result = _syncProductFlavour.GetProductFlavour(myQuery);
                var data = result.MasterData.MasterDataItems;
                if (data==null || data.Count() == 0)
                {
                    break;
                }
                string path = folder + entityName + "_" + i + ".csv";
                data.WriteCSV(path);
                myQuery.Skip += threshold;
                i++;
            }
        }

        private void CreateProductBrandCsv(QueryMasterData myQuery, string folder)
        {
            var entityName = MasterDataCollective.ProductBrand;
            int i = 0;
            myQuery.Skip = 0;
            while (true)
            {

                myQuery.Take = threshold;
                var result = _syncProductBrand.GetProductBrand(myQuery);
                var data = result.MasterData.MasterDataItems;
                if (data==null || data.Count() == 0)
                {
                    break;
                }
                string path = folder + entityName + "_" + i + ".csv";
                data.WriteCSV(path);
                myQuery.Skip += threshold;
                i++;
            }
        }

        private void CreateProductTypeCsv(QueryMasterData myQuery, string folder)
        {
            var entityName = MasterDataCollective.ProductType;
            int i = 0;
            myQuery.Skip = 0;
            while (true)
            {

                myQuery.Take = threshold;
                var result = _syncProductType.GetProductType(myQuery);
                var data = result.MasterData.MasterDataItems;
                if (data==null || data.Count() == 0)
                {
                    break;
                }
                string path = folder + entityName + "_" + i + ".csv";
                data.WriteCSV(path);
                myQuery.Skip += threshold;
                i++;
            }
        }

        private void CreateVatClassCsv(QueryMasterData myQuery, string folder)
        {
            var entityName = MasterDataCollective.VatClass;
            int i = 0;
            myQuery.Skip = 0;
            while (true)
            {

                myQuery.Take = threshold;
                var result = _syncVatClass.GetVatClass(myQuery);
                var data = result.MasterData.MasterDataItems;
                if (data==null || data.Count() == 0)
                {
                    break;
                }
                string path = folder + entityName + "_" + i + ".csv";
                data.WriteCSV(path);
                var shipto = data.SelectMany(s => s.VatClassItems).ToArray();
                path = folder + entityName + "_Items_" + i + ".csv";
                GenerateCsv(path, shipto);
                myQuery.Skip += threshold;
                i++;
            }
        }

        private void CreatePricingTierCsv(QueryMasterData myQuery, string folder)
        {
            int i = 0;
            myQuery.Skip = 0;
            while (true)
            {

                myQuery.Take = threshold;
                var result = _syncPricingTier.GetPricingTier(myQuery);
                var data = result.MasterData.MasterDataItems;
                if (data==null || data.Count() == 0)
                {
                    break;
                }
                string path = folder + MasterDataCollective.PricingTier + "_" + i + ".csv";
                data.WriteCSV(path);
                myQuery.Skip += threshold;
                i++;
            }
        }

        private void CreateSupplierCsv(QueryMasterData myQuery, string folder)
        {
            int i = 0;
            myQuery.Skip = 0;
            while (true)
            {

                myQuery.Take = threshold;
                var result = _syncSupplier.GetSupplier(myQuery);
                var data = result.MasterData.MasterDataItems;
                if (data==null || data.Count() == 0)
                {
                    break;
                }
                string path = folder + MasterDataCollective.Supplier + "_" + i + ".csv";
                data.WriteCSV(path);
                myQuery.Skip += threshold;
                i++;
            }
        }

        private void CreateRetireSettingCsv(QueryMasterData myQuery, string folder)
        {
            int i = 0;
            myQuery.Skip = 0;
            while (true)
            {

                myQuery.Take = threshold;
                var result = _syncRetireSetting.GetRetireSetting(myQuery);
                var data = result.MasterData.MasterDataItems;
                if (data==null || data.Count() == 0)
                {
                    break;
                }
                string path = folder + MasterDataCollective.RetireSetting + "_" + i + ".csv";
                data.WriteCSV(path);
                myQuery.Skip += threshold;
                i++;
            }
        }

        private void CreateSettingCsv(QueryMasterData myQuery, string folder)
        {
            int i = 0;
            myQuery.Skip = 0;
            while (true)
            {

                myQuery.Take = threshold;
                var result = _syncSetting.GetAppSettings(myQuery);
                var data = result.MasterData.MasterDataItems;
                if (data==null || data.Count() == 0)
                {
                    break;
                }
                string path = folder + MasterDataCollective.Setting + "_" + i + ".csv";
                data.WriteCSV(path);
                myQuery.Skip += threshold;
                i++;
            }
        }

        private void CreateAssetCsv(QueryMasterData myQuery, string folder)
        {
            int i = 0;
            myQuery.Skip = 0;
            while (true)
            {

                myQuery.Take = threshold;
                var result = _syncAsset.GetAsset(myQuery);
                var data = result.MasterData.MasterDataItems;
                if (data==null || data.Count() == 0)
                {
                    break;
                }
                string path = folder + MasterDataCollective.Asset + "_" + i + ".csv";
                data.WriteCSV(path);
                myQuery.Skip += threshold;
                i++;
            }
        }

        private void CreateAssetCategoryCsv(QueryMasterData myQuery, string folder)
        {
            int i = 0;
            myQuery.Skip = 0;
            while (true)
            {

                myQuery.Take = threshold;
                var result = _syncAssetCategory.GetAssetCategory(myQuery);
                var data = result.MasterData.MasterDataItems;
                if (data==null || data.Count() == 0)
                {
                    break;
                }
                string path = folder + MasterDataCollective.AssetCategory + "_" + i + ".csv";
                data.WriteCSV(path);
                myQuery.Skip += threshold;
                i++;
            }
        }

        private void CreateAssetTypeCsv(QueryMasterData myQuery, string folder)
        {
            int i = 0;
            myQuery.Skip = 0;
            while (true)
            {

                myQuery.Take = threshold;
                var result = _syncAssetType.GetAssetType(myQuery);
                var data = result.MasterData.MasterDataItems;
                if (data==null || data.Count() == 0)
                {
                    break;
                }
                string path = folder + MasterDataCollective.AssetType + "_" + i + ".csv";
                data.WriteCSV(path);
                myQuery.Skip += threshold;
                i++;
            }
        }

        private void CreateAssetStatusCsv(QueryMasterData myQuery, string folder)
        {
            int i = 0;
            myQuery.Skip = 0;
            while (true)
            {

                myQuery.Take = threshold;
                var result = _syncAssetStatus.GetAssetStatus(myQuery);
                var data = result.MasterData.MasterDataItems;
                if (data==null || data.Count() == 0)
                {
                    break;
                }
                string path = folder + MasterDataCollective.AssetStatus + "_" + i + ".csv";
                data.WriteCSV(path);
                myQuery.Skip += threshold;
                i++;
            }
        }

        private void CreateBankBranchCsv(QueryMasterData myQuery, string folder)
        {
            int i = 0;
            myQuery.Skip = 0;
            while (true)
            {

                myQuery.Take = threshold;
                var result = _syncBankBranch.GetBankBranch(myQuery);
                var data = result.MasterData.MasterDataItems;
                if (data==null || data.Count() == 0)
                {
                    break;
                }
                string path = folder + MasterDataCollective.BankBranch + "_" + i + ".csv";
                data.WriteCSV(path);
                myQuery.Skip += threshold;
                i++;
            }
        }

        private void CreateBankCsv(QueryMasterData myQuery, string folder)
        {
            int i = 0;
            myQuery.Skip = 0;
            while (true)
            {

                myQuery.Take = threshold;
                var result = _syncBank.GetBank(myQuery);
                var data = result.MasterData.MasterDataItems;
                if (data==null || data.Count() == 0)
                {
                    break;
                }
                string path = folder + MasterDataCollective.Bank + "_" + i + ".csv";
                data.WriteCSV(path);
                myQuery.Skip += threshold;
                i++;
            }
        }

        private void CreateOutletTypeCsv(QueryMasterData myQuery, string folder)
        {
            int i = 0;
            myQuery.Skip = 0;
            while (true)
            {

                myQuery.Take = threshold;
                var result = _syncOutletType.GetOutletType(myQuery);
                var data = result.MasterData.MasterDataItems;
                if (data==null || data.Count() == 0)
                {
                    break;
                }
                string path = folder + MasterDataCollective.OutletType + "_" + i + ".csv";
                data.WriteCSV(path);
                myQuery.Skip += threshold;
                i++;
            }
        }

        private void CreateOutletCategoryCsv(QueryMasterData myQuery, string folder)
        {
            int i = 0;
            myQuery.Skip = 0;
            while (true)
            {

                myQuery.Take = threshold;
                var result = _syncOutletCategory.GetOutletCategory(myQuery);
                var data = result.MasterData.MasterDataItems;
                if (data==null || data.Count() == 0)
                {
                    break;
                }
                string path = folder + MasterDataCollective.OutletCategory + "_" + i + ".csv";
                data.WriteCSV(path);
                myQuery.Skip += threshold;
                i++;
            }
        }

        private void CreateContactTypeCsv(QueryMasterData myQuery, string folder)
        {
            int i = 0;
            myQuery.Skip = 0;
            while (true)
            {

                myQuery.Take = threshold;
                var result = _syncContactType.GetContactType(myQuery);
                var data = result.MasterData.MasterDataItems;
                if (data==null || data.Count() == 0)
                {
                    break;
                }
                string path = folder + MasterDataCollective.ContactType + "_" + i + ".csv";
                data.WriteCSV(path);
                myQuery.Skip += threshold;
                i++;
            }
        }

        private void CreateSocioEconomicStatusCsv(QueryMasterData myQuery, string folder)
        {
            int i = 0;
            myQuery.Skip = 0;
            while (true)
            {

                myQuery.Take = threshold;
                var result = _syncSocioEconomicStatus.GetEconomicStatus(myQuery);
                var data = result.MasterData.MasterDataItems;
                if (data==null || data.Count() == 0)
                {
                    break;
                }
                string path = folder + MasterDataCollective.SocioEconomicStatus + "_" + i + ".csv";
                data.WriteCSV(path);
                myQuery.Skip += threshold;
                i++;
            }
        }

        private void CreateOutletCsv(QueryMasterData myQuery, string folder)
        {
            var entityName = MasterDataCollective.Outlet;
            _log.InfoFormat("Start creating {0} CSV ", entityName);
            int i = 0;
            myQuery.Skip = 0;
            while (true)
            {
               
                myQuery.Take = threshold;
                var result = _syncOutlet.GetOutlet(myQuery);
                var data = result.MasterData.MasterDataItems;
                if (data==null || data.Count() == 0)
                {
                    break;
                }
                string path = folder + entityName + "_" + i + ".csv";
                GenerateCsv(path, data);
                var shipto = data.SelectMany(s => s.ShippingAddresses).ToArray();
                path = folder + entityName +"_"+ MasterDataCollective.ShipTo + "_" + i + ".csv";
                 GenerateCsv(path, shipto);
                myQuery.Skip += threshold;
                i++;
            }
            _log.InfoFormat("Finished creating {0} CSV ", entityName);
        }

        private void CreateDistrictCsv(QueryMasterData myQuery, string folder)
        {
          
            int i = 0;
            myQuery.Skip = 0;
            while (true)
            {

                myQuery.Take = threshold;
                var result = _syncDistrict.GetDistrict(myQuery);
                var data = result.MasterData.MasterDataItems;
                if (data==null || data.Count() == 0)
                {
                    break;
                }
                string path = folder + MasterDataCollective.District + "_" + i + ".csv";
                data.WriteCSV(path);
                myQuery.Skip += threshold;
                i++;
            }
        }

        private void CreateProvinceCsv(QueryMasterData myQuery, string folder)
        {
            int i = 0;
            myQuery.Skip = 0;
            while (true)
            {
               
                myQuery.Take = threshold;
                var result = _syncProvince.GetProvince(myQuery);
                var data = result.MasterData.MasterDataItems;
                if (data==null || data.Count() == 0)
                {
                    break;
                }
                string path = folder + MasterDataCollective.Producer + "_" + i + ".csv";
                data.WriteCSV(path);
                myQuery.Skip += threshold;
                i++;
            }

        }

        private void GenerateCsv<T>(string path, T[] data) where T : class
        {
            data.WriteCSV(path);
        }


       
       private void CreateAreaCsv(QueryMasterData myQuery, string folder)
       {
        
        int i = 0;
        myQuery.Skip = 0;
        while (true)
        {

            myQuery.Take = threshold;
            var result = _syncArea.GetArea(myQuery);
            var data = result.MasterData.MasterDataItems;
            if (data==null || data.Count() == 0)
            {
                break;
            }
            string path = folder + MasterDataCollective.Area + "_" + i + ".csv";
            data.WriteCSV(path);
            myQuery.Skip += threshold;
            i++;
        }
       }

       private void CreateRegionCsv(QueryMasterData myQuery, string folder)
       {
           
           int i = 0;
           myQuery.Skip = 0;
           while (true)
           {

               myQuery.Take = threshold;
               var result = _syncRegion.GetRegion(myQuery);
               var data = result.MasterData.MasterDataItems;
               if (data==null || data.Count() == 0)
               {
                   break;
               }
               string path = folder + MasterDataCollective.Region + "_" + i + ".csv";
               data.WriteCSV(path);
               myQuery.Skip += threshold;
               i++;
           }
       }

       private void CreateTerritoryCsv(QueryMasterData myQuery, string folder)
       {
         
         int i = 0;
         myQuery.Skip = 0;
         while (true)
         {

             myQuery.Take = threshold;
             var result = _syncTerritory.GetTerritory(myQuery);
             var data = result.MasterData.MasterDataItems;
             if (data==null || data.Count() == 0)
             {
                 break;
             }
             string path = folder + MasterDataCollective.Territory + "_" + i + ".csv";
             data.WriteCSV(path);
             myQuery.Skip += threshold;
             i++;
         }
       }

       private void CreateCountryCsv(QueryMasterData myQuery, string folder)
       {
         
         int i = 0;
         myQuery.Skip = 0;
         while (true)
         {

             myQuery.Take = threshold;
             var result = _syncCountry.GetCountry(myQuery);
             var data = result.MasterData.MasterDataItems;
             if (data==null || data.Count() == 0)
             {
                 break;
             }
             string path = folder + MasterDataCollective.Country + "_" + i + ".csv";
             data.WriteCSV(path);
             myQuery.Skip += threshold;
             i++;
         }
       }
     
   }
}
