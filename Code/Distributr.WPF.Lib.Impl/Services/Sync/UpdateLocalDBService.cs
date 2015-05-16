using System;
using System.Reflection;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using System.Linq;
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
using Distributr.Core.Repository.Financials;
using Distributr.Core.Repository.InventoryRepository;
using Distributr.Core.Repository.Master.Agrimanagr;
using Distributr.Core.Repository.Master.AssetRepositories;
using Distributr.Core.Repository.Master.BankRepositories;
using Distributr.Core.Repository.Master.CentreRepositories;
using Distributr.Core.Repository.Master.ChannelPackagings;
using Distributr.Core.Repository.Master.CommodityOwnerRepository;
using Distributr.Core.Repository.Master.CommodityRepositories;
using Distributr.Core.Repository.Master.CompetitorManagement;
using Distributr.Core.Repository.Master.CoolerTypeRepositories;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.DistributorTargetRepositories;
using Distributr.Core.Repository.Master.EquipmentRepository;
using Distributr.Core.Repository.Master.MasterDataAllocationRepositories;
using Distributr.Core.Repository.Master.OutletVisitReasonsTypeRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Master.ReOrderLevelRepository;
using Distributr.Core.Repository.Master.SettingsRepositories;
using Distributr.Core.Repository.Master.SuppliersRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Utility.Mapping;
using log4net;


//using Distributr.SL.Lib.CoreLink.MasterDTO.CostCentre;

namespace Distributr.WPF.Lib.Impl.Services.Sync
{
    public class UpdateLocalDBService : IUpdateLocalDBService
    {
        #region Declarations
        ICountryRepository _countryRepository;
        IAreaRepository _areaRepository;
        IContactRepository _contactRepository;
        IDistributorRepository _distributorRepository;
        IProducerRepository _producerRepository;
        ITransporterRepository _transporterRepository;
        IOutletCategoryRepository _outletCategoryRepository;
        IOutletRepository _outletRepository;
        IOutletTypeRepository _outletTypeRepository;
        IRouteRepository _routeRepository;
        IRegionRepository _regionRepository;
        ISocioEconomicStatusRepository _socioEconomicRepository;
        ITerritoryRepository _territoryRepository;
        IProductBrandRepository _productBrandRepository;
        IProductFlavourRepository _productFlavourRepository;
        IProductPackagingRepository _productPackagingRepository;
        IProductPackagingTypeRepository _productPackagingTypeRepository;
        IProductPricingRepository _productPricingRepository;
        IProductPricingTierRepository _productPricingTierRepository;
        IProductTypeRepository _productTypeRepository;
        IVATClassRepository _vatClassRepository;
        IUserRepository _userRepository;
        IDistributorSalesmanRepository _distributorSalesmanRepository;
        IDistributorPendingDispatchWarehouseRepository _distributorPendingDispatchWarehouseRepository;
        ISaleValueDiscountRepository _saleValueDiscountRepository;
        IProductDiscountRepository _productDiscountRepository;
        ITargetPeriodRepository _targetPeriodRepository;
        ITargetRepository _targetRepository;
        IProvincesRepository _provinceRepository;
        IDistrictRepository _districtRepository;
        IReOrderLevelRepository _reOrderLevelRepository;
        IChannelPackagingRepository _channelPackagingRepository;
        ICompetitorProductsRepository _competitorProductRepository;
        ICompetitorRepository _competitorRepository;
        IAssetRepository _coolerRepository;
        IAssetTypeRepository _coolerTypeRepository;
        IDiscountGroupRepository _discountGroupRepository;
        IPromotionDiscountRepository _promotionDiscountRepository;
        ICertainValueCertainProductDiscountRepository _certainValueCertainProductDiscountRepository;
        IProductDiscountGroupRepository _productGroupDiscountRepository;
        IFreeOfChargeDiscountRepository _freeOfChargeDiscountRepository;
        ISalesmanRouteRepository _salesmanRouteRepository;
        ISalesmanSupplierRepository _salesmanSupplierRepository;

        IUserGroupRepository _userGroupRepository;
        IUserGroupRolesRepository _userGroupRoleRepository;
        IBankRepository _bankRepository;
        IBankBranchRepository _bankBranchRepository;
        ISupplierRepository _supplierRepository;
        IContactTypeRepository _contactTypeRepository;
        IAssetCategoryRepository _assetCategoryRepository;
        IAssetStatusRepository _assetStatusRepository;
        IOutletPriorityRepository _outletPriorityRepository;
        IOutletVisitDayRepository _outletVisitDayRepository;
        ITargetItemRepository _targetItemRepository;
        ISettingsRepository _appSettingsRepository;
        IInventoryRepository _inventoryRepository;
        IPaymentTrackerRepository _paymentTrackerRepository;
        IRetireDocumentSettingRepository _retireSettingRepository;
        IProductRepository _productRepository;
        ICommodityTypeRepository _commodityTypeRepository;
        ICommodityRepository _commodityRepository;
        ICommodityOwnerTypeRepository _commodityOwnerTypeRepository;
        ICommodityProducerRepository _commodityProducerRepository;
        ICommoditySupplierRepository _commoditySupplierRepository;
        ICommodityOwnerRepository _commodityOwnerRepository;
        ICentreRepository _centreRepository;
        IHubRepository _hubRepository;
        IStoreRepository _storeRepository;
        IPurchasingClerkRepository _purchasingClerkRepository;
        ICentreTypeRepository _centreTypeRepository;
        IEquipmentRepository _equipmentRepository;
        IDTOToEntityMapping _mapper;
        IMasterDataAllocationRepository _masterDataAllocationRepository;
        IContainerTypeRepository _containerTypeRepository;
        IVehicleRepository _vehicleRepository;
        private CokeDataContext _cokeDataContext;

        private IServiceProviderRepository _serviceProviderRepository;
        private IShiftRepository _shiftRepository;
        private ISeasonRepository _seasonRepository;
        private IInfectionRepository _infectionRepository;
        private IServiceRepository _serviceRepository;
        private IActivityTypeRepository _activityTypeRepository;
        private IOutletVisitReasonsTypeRepository _outletVisitReasonsTypeRepository;

        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        public UpdateLocalDBService(ICountryRepository countryRepository, IAreaRepository areaRepository, IContactRepository contactRepository, IDistributorRepository distributorRepository, IProducerRepository producerRepository, ITransporterRepository transporterRepository, IOutletCategoryRepository outletCategoryRepository, IOutletRepository outletRepository, IOutletTypeRepository outletTypeRepository, IRouteRepository routeRepository, IRegionRepository regionRepository, ISocioEconomicStatusRepository socioEconomicRepository, ITerritoryRepository territoryRepository, IProductBrandRepository productBrandRepository, IProductFlavourRepository productFlavourRepository, IProductPackagingRepository productPackagingRepository, IProductPackagingTypeRepository productPackagingTypeRepository, IProductPricingRepository productPricingRepository, IProductPricingTierRepository productPricingTierRepository, IProductTypeRepository productTypeRepository, IVATClassRepository vatClassRepository, IUserRepository userRepository, IDistributorSalesmanRepository distributorSalesmanRepository, IDistributorPendingDispatchWarehouseRepository distributorPendingDispatchWarehouseRepository, ISaleValueDiscountRepository saleValueDiscountRepository, IProductDiscountRepository productDiscountRepository, ITargetPeriodRepository targetPeriodRepository, ITargetRepository targetRepository, IProvincesRepository provinceRepository, IDistrictRepository districtRepository, IReOrderLevelRepository reOrderLevelRepository, IChannelPackagingRepository channelPackagingRepository, ICompetitorProductsRepository competitorProductRepository, ICompetitorRepository competitorRepository, IAssetRepository coolerRepository, IAssetTypeRepository coolerTypeRepository, IDiscountGroupRepository discountGroupRepository, IPromotionDiscountRepository promotionDiscountRepository, ICertainValueCertainProductDiscountRepository certainValueCertainProductDiscountRepository, IProductDiscountGroupRepository productGroupDiscountRepository, IFreeOfChargeDiscountRepository freeOfChargeDiscountRepository, ISalesmanRouteRepository salesmanRouteRepository,ISalesmanSupplierRepository  salesmanSupplierRepository,IUserGroupRepository userGroupRepository, IUserGroupRolesRepository userGroupRoleRepository, IBankRepository bankRepository, IBankBranchRepository bankBranchRepository, ISupplierRepository supplierRepository, IContactTypeRepository contactTypeRepository, IAssetCategoryRepository assetCategoryRepository, IAssetStatusRepository assetStatusRepository, IOutletPriorityRepository outletPriorityRepository, IOutletVisitDayRepository outletVisitDayRepository, ITargetItemRepository targetItemRepository, ISettingsRepository appSettingsRepository, IInventoryRepository inventoryRepository, IPaymentTrackerRepository paymentTrackerRepository, IRetireDocumentSettingRepository retireSettingRepository, IProductRepository productRepository, ICommodityTypeRepository commodityTypeRepository, ICommodityRepository commodityRepository, ICommodityOwnerTypeRepository commodityOwnerTypeRepository, ICommodityProducerRepository commodityProducerRepository, ICommoditySupplierRepository commoditySupplierRepository, ICommodityOwnerRepository commodityOwnerRepository, ICentreRepository centreRepository, IHubRepository hubRepository, IStoreRepository storeRepository, IPurchasingClerkRepository purchasingClerkRepository, ICentreTypeRepository centreTypeRepository, IEquipmentRepository equipmentRepository, IDTOToEntityMapping mapper, IMasterDataAllocationRepository masterDataAllocationRepository, IContainerTypeRepository containerTypeRepository, IVehicleRepository vehicleRepository, CokeDataContext cokeDataContext, IServiceProviderRepository serviceProviderRepository, IShiftRepository shiftRepository, ISeasonRepository seasonRepository, IInfectionRepository infectionRepository, IServiceRepository serviceRepository, IActivityTypeRepository activityTypeRepository, IOutletVisitReasonsTypeRepository outletVisitReasonsTypeRepository)
        {
            _countryRepository = countryRepository;
            _areaRepository = areaRepository;
            _contactRepository = contactRepository;
            _distributorRepository = distributorRepository;
            _producerRepository = producerRepository;
            _transporterRepository = transporterRepository;
            _outletCategoryRepository = outletCategoryRepository;
            _outletRepository = outletRepository;
            _outletTypeRepository = outletTypeRepository;
            _routeRepository = routeRepository;
            _regionRepository = regionRepository;
            _socioEconomicRepository = socioEconomicRepository;
            _territoryRepository = territoryRepository;
            _productBrandRepository = productBrandRepository;
            _productFlavourRepository = productFlavourRepository;
            _productPackagingRepository = productPackagingRepository;
            _productPackagingTypeRepository = productPackagingTypeRepository;
            _productPricingRepository = productPricingRepository;
            _productPricingTierRepository = productPricingTierRepository;
            _productTypeRepository = productTypeRepository;
            _vatClassRepository = vatClassRepository;
            _userRepository = userRepository;
            _distributorSalesmanRepository = distributorSalesmanRepository;
            _distributorPendingDispatchWarehouseRepository = distributorPendingDispatchWarehouseRepository;
            _saleValueDiscountRepository = saleValueDiscountRepository;
            _productDiscountRepository = productDiscountRepository;
            _targetPeriodRepository = targetPeriodRepository;
            _targetRepository = targetRepository;
            _provinceRepository = provinceRepository;
            _districtRepository = districtRepository;
            _reOrderLevelRepository = reOrderLevelRepository;
            _channelPackagingRepository = channelPackagingRepository;
            _competitorProductRepository = competitorProductRepository;
            _competitorRepository = competitorRepository;
            _coolerRepository = coolerRepository;
            _coolerTypeRepository = coolerTypeRepository;
            _discountGroupRepository = discountGroupRepository;
            _promotionDiscountRepository = promotionDiscountRepository;
            _certainValueCertainProductDiscountRepository = certainValueCertainProductDiscountRepository;
            _productGroupDiscountRepository = productGroupDiscountRepository;
            _freeOfChargeDiscountRepository = freeOfChargeDiscountRepository;
            _salesmanRouteRepository = salesmanRouteRepository;
            _salesmanSupplierRepository = salesmanSupplierRepository;

            _userGroupRepository = userGroupRepository;
            _userGroupRoleRepository = userGroupRoleRepository;
            _bankRepository = bankRepository;
            _bankBranchRepository = bankBranchRepository;
            _supplierRepository = supplierRepository;
            _contactTypeRepository = contactTypeRepository;
            _assetCategoryRepository = assetCategoryRepository;
            _assetStatusRepository = assetStatusRepository;
            _outletPriorityRepository = outletPriorityRepository;
            _outletVisitDayRepository = outletVisitDayRepository;
            _targetItemRepository = targetItemRepository;
            _appSettingsRepository = appSettingsRepository;
            _inventoryRepository = inventoryRepository;
            _paymentTrackerRepository = paymentTrackerRepository;
            _retireSettingRepository = retireSettingRepository;
            _productRepository = productRepository;
            _commodityTypeRepository = commodityTypeRepository;
            _commodityRepository = commodityRepository;
            _commodityOwnerTypeRepository = commodityOwnerTypeRepository;
            _commodityProducerRepository = commodityProducerRepository;
            _commoditySupplierRepository = commoditySupplierRepository;
            _commodityOwnerRepository = commodityOwnerRepository;
            _centreRepository = centreRepository;
            _hubRepository = hubRepository;
            _storeRepository = storeRepository;
            _purchasingClerkRepository = purchasingClerkRepository;
            _centreTypeRepository = centreTypeRepository;
            _equipmentRepository = equipmentRepository;
            _mapper = mapper;
            _masterDataAllocationRepository = masterDataAllocationRepository;
            _containerTypeRepository = containerTypeRepository;
            _vehicleRepository = vehicleRepository;
            _cokeDataContext = cokeDataContext;
            _serviceProviderRepository = serviceProviderRepository;
            _shiftRepository = shiftRepository;
            _seasonRepository = seasonRepository;
            _infectionRepository = infectionRepository;
            _serviceRepository = serviceRepository;
            _activityTypeRepository = activityTypeRepository;
            _outletVisitReasonsTypeRepository = outletVisitReasonsTypeRepository;
        }

        public void UpdateLocalDB(ResponseMasterDataInfo responseMasterDataInfo)
        {
            MasterDataCollective mdc = (MasterDataCollective)Enum.Parse(typeof(MasterDataCollective), responseMasterDataInfo.MasterData.EntityName, true);
            if (mdc == null)
                throw new Exception("Failed to resolve Master Data Collective Type");
            switch (mdc)
            {
                default:
                    throw new Exception("Failed to Update local db with " + mdc.ToString());
                case MasterDataCollective.ProductBrand:
                    SaveProductBrands(responseMasterDataInfo);
                    break;
                case MasterDataCollective.OutletCategory:
                    SaveOutletCategories(responseMasterDataInfo);
                    break;
                case MasterDataCollective.Territory:
                    SaveTerritories(responseMasterDataInfo);
                    break;
                case MasterDataCollective.OutletType:
                    SaveOutletTypes(responseMasterDataInfo);
                    break;
                case MasterDataCollective.PricingTier:
                    SavePricingTiers(responseMasterDataInfo);
                    break;
                case MasterDataCollective.ProductFlavour:
                    SaveProductFlavours(responseMasterDataInfo);
                    break;
                case MasterDataCollective.ProductType:
                    SaveProductTypes(responseMasterDataInfo);
                    break;
                case MasterDataCollective.ProductPackagingType:
                    SaveProductPackagingTypes(responseMasterDataInfo);
                    break;
                case MasterDataCollective.SocioEconomicStatus:
                    SaveSocioEconomicStatus(responseMasterDataInfo);
                    break;
                case MasterDataCollective.Country:
                    SaveCountries(responseMasterDataInfo);
                    break;
                case MasterDataCollective.Region:
                    SaveRegions(responseMasterDataInfo);
                    break;
                case MasterDataCollective.Area:
                    SaveAreas(responseMasterDataInfo);
                    break;
                case MasterDataCollective.Distributor:
                    SaveDistributors(responseMasterDataInfo);
                    break;
                case MasterDataCollective.Producer:
                    SaveProducers(responseMasterDataInfo);
                    break;
                case MasterDataCollective.Outlet:
                    SaveOutlets(responseMasterDataInfo);
                    break;
                case MasterDataCollective.Contact:
                    SaveContact(responseMasterDataInfo);
                    break;
                case MasterDataCollective.Route:
                    SaveRoutes(responseMasterDataInfo);
                    break;
                case MasterDataCollective.User:
                    SaveUsers(responseMasterDataInfo);
                    break;
                case MasterDataCollective.Pricing:
                   // SavePricing(responseMasterDataInfo);
                    SavePricingDirect(responseMasterDataInfo);
                    break;
                case MasterDataCollective.SaleProduct:
                    SaveSaleProduct(responseMasterDataInfo);
                    break;
                case MasterDataCollective.ReturnableProduct:
                    SaveReturnableProduct(responseMasterDataInfo);
                    break;
                case MasterDataCollective.ConsolidatedProduct:
                    SaveConsolidatedProduct(responseMasterDataInfo);
                    break;
                case MasterDataCollective.VatClass:
                    SaveVatClass(responseMasterDataInfo);
                    break;
                case MasterDataCollective.ProductPackaging:
                    SaveProductPackaging(responseMasterDataInfo);
                    break;
                case MasterDataCollective.DistributorSalesman:
                    SaveDistributorSalesman(responseMasterDataInfo);
                    break;
                case MasterDataCollective.DistributorPendingDispatchWarehouse:
                    SaveDistributorPendingDispatchWarehouse(responseMasterDataInfo);
                    break;
                case MasterDataCollective.SaleValueDiscount:
                    SaveSaleValueDiscount(responseMasterDataInfo);
                    break;
                case MasterDataCollective.ProductDiscount:
                    SaveProductDiscount(responseMasterDataInfo);
                    break;
                case MasterDataCollective.PromotionDiscount:
                    SavePromotionDiscount(responseMasterDataInfo);
                    break;
                case MasterDataCollective.CertainValueCertainProductDiscount:
                    SaveCertainValueCertainProductDiscount(responseMasterDataInfo);
                    break;
                case MasterDataCollective.ProductGroupDiscount:
                    SaveProductGroupDiscountDirect(responseMasterDataInfo);
                    break;
                case MasterDataCollective.TargetPeriod:
                    SaveTargetPeriod(responseMasterDataInfo);
                    break;
                case MasterDataCollective.Target:
                    SaveTarget(responseMasterDataInfo);
                    break;
                case MasterDataCollective.Province:
                    SaveProvince(responseMasterDataInfo);
                    break;
                case MasterDataCollective.District:
                    SaveDistrict(responseMasterDataInfo);
                    break;
                case MasterDataCollective.ReorderLevel:
                    SaveReOrderLevel(responseMasterDataInfo);
                    break;
                case MasterDataCollective.ChannelPackaging:
                    SaveChannelPackaging(responseMasterDataInfo);
                    break;
                case MasterDataCollective.Competitor:
                    SaveCompetitor(responseMasterDataInfo);
                    break;
                case MasterDataCollective.CompetitorProduct:
                    SaveCompetitorProduct(responseMasterDataInfo);
                    break;
                case MasterDataCollective.Asset:
                    SaveCooler(responseMasterDataInfo);
                    break;
                case MasterDataCollective.AssetType:
                    SaveCoolerType(responseMasterDataInfo);
                    break;
                case MasterDataCollective.DiscountGroup:
                    SaveDiscountgroup(responseMasterDataInfo);
                    break;
                case MasterDataCollective.FreeOfChargeDiscount:
                    SaveFreeOfChargeDiscount(responseMasterDataInfo);
                    break;
                case MasterDataCollective.SalesmanRoute:
                    SaveSalesmanRoute(responseMasterDataInfo);
                    break;
                case MasterDataCollective.SalesmanSupplier:
                    SaveSalesmanSupplier(responseMasterDataInfo);
                    break;

                 
                case MasterDataCollective.UserGroup:
                    SaveUserGroup(responseMasterDataInfo);
                    break;
                case MasterDataCollective.UserGroupRole:
                    SaveUserGroupRole(responseMasterDataInfo);
                    break;
                case MasterDataCollective.Bank:
                    SaveBank(responseMasterDataInfo);
                    break;
                case MasterDataCollective.BankBranch:
                    SaveBankBranch(responseMasterDataInfo);
                    break;
                case MasterDataCollective.Supplier:
                    SaveSupplier(responseMasterDataInfo);
                    break;
                case MasterDataCollective.ContactType:
                    SaveContactType(responseMasterDataInfo);
                    break;
                case MasterDataCollective.AssetCategory:
                    SaveAssetCategory(responseMasterDataInfo);
                    break;
                case MasterDataCollective.AssetStatus:
                    SaveAssetStatus(responseMasterDataInfo);
                    break;
                case MasterDataCollective.OutletPriority:
                    SaveOutletPriority(responseMasterDataInfo);
                    break;
                case MasterDataCollective.OutletVisitDay:
                    SaveOutletVisitDay(responseMasterDataInfo);
                    break;
                case MasterDataCollective.TargetItem:
                    SaveTargetItem(responseMasterDataInfo);
                    break;
                case MasterDataCollective.Setting:
                    SaveSetting(responseMasterDataInfo);
                    break;
                case MasterDataCollective.RetireSetting:
                    SaveRetireSetting(responseMasterDataInfo);
                    break;
                case MasterDataCollective.CommodityType:
                    SaveCommodityType(responseMasterDataInfo);
                    break;
                case MasterDataCollective.Commodity:
                    SaveCommodity(responseMasterDataInfo);
                    break;
                case MasterDataCollective.CommodityOwnerType:
                    SaveCommodityOwnerType(responseMasterDataInfo);
                    break;
                case MasterDataCollective.CommodityProducer:
                    SaveCommodityProducer(responseMasterDataInfo);
                    break;
                case MasterDataCollective.CommoditySupplier:
                    SaveCommoditySupplier(responseMasterDataInfo);
                    break;
                case MasterDataCollective.CommodityOwner:
                    SaveCommodityOwner(responseMasterDataInfo);
                    break;
                case MasterDataCollective.Centre:
                    SaveCentre(responseMasterDataInfo);
                    break;
                case MasterDataCollective.CentreType:
                    SaveCentreType(responseMasterDataInfo);
                    break;
                case MasterDataCollective.Hub:
                    SaveHub(responseMasterDataInfo);
                    break;
                case MasterDataCollective.Store:
                    SaveStore(responseMasterDataInfo);
                    break;
                case MasterDataCollective.FieldClerk:
                    SavePurchasingClerk(responseMasterDataInfo);
                    break;
                case MasterDataCollective.Printer:
                    SavePrinter(responseMasterDataInfo);
                    break;
                case MasterDataCollective.WeighScale:
                    SaveWeighScale(responseMasterDataInfo);
                    break;
                case MasterDataCollective.SourcingContainer:
                    SaveSourcingContainer(responseMasterDataInfo);
                    break;
                case MasterDataCollective.Vehicle:
                    SaveVehicle(responseMasterDataInfo);
                    break;
                case MasterDataCollective.ContainerType:
                    SaveContainerType(responseMasterDataInfo);
                    break;
                case MasterDataCollective.CommodityProducerCentreAllocation:
                    SaveFarmCentreAllocation(responseMasterDataInfo);
                    break;
                case MasterDataCollective.RouteRegionAllocation:
                    SaveRouteRegionAllocation(responseMasterDataInfo);
                    break;
                case MasterDataCollective.RouteCostCentreAllocation:
                    SaveRouteCostCentreAllocation(responseMasterDataInfo);
                    break;
                case MasterDataCollective.RouteCentreAllocation:
                    SaveRouteCentreAllocation(responseMasterDataInfo);
                    break;

                case MasterDataCollective.ServiceProvider:
                    SaveServiceProviders(responseMasterDataInfo);
                    break;
                case MasterDataCollective.Service:
                    SaveServices(responseMasterDataInfo);
                    break;
                case MasterDataCollective.Shift:
                    SaveShifts(responseMasterDataInfo);
                    break;
                case MasterDataCollective.Infection:
                    SaveInfections(responseMasterDataInfo);
                    break;
                case MasterDataCollective.Season:
                    SaveSeasons(responseMasterDataInfo);
                    break;
                case MasterDataCollective.ActivityType:
                    SaveActivityType(responseMasterDataInfo);
                    break;
                case MasterDataCollective.OutletVisitReasonsType:
                    SaveOutletVisitReasonsType(responseMasterDataInfo);
                    
                    break;
            }
        }

        private void SaveOutletVisitReasonsType(ResponseMasterDataInfo responseMasterDataInfo)
        {
           
            if (responseMasterDataInfo.DeletedItems.Any())
            {
                foreach (var deletedItem in responseMasterDataInfo.DeletedItems)
                {
                    var outletVisitReasonsType = _outletVisitReasonsTypeRepository.GetById(deletedItem, true);
                    if (outletVisitReasonsType != null)
                    {
                        try
                        {
                            _outletVisitReasonsTypeRepository.SetAsDeleted(outletVisitReasonsType);
                        }
                        catch (Exception ex)
                        {
                            _log.Error("Deleting Outlet Visit Reasons error-" + ex.ToString());

                        }
                      
                    }

                }

            }
            responseMasterDataInfo.MasterData.MasterDataItems
               .OfType<OutletVisitReasonTypeDTO>()
               .Select(n => _mapper.Map(n)).ToList()
               .ForEach(n => _outletVisitReasonsTypeRepository.Save(n, true));
        
        }

        private void SaveActivityType(ResponseMasterDataInfo responseMasterDataInfo)
        {
            if (responseMasterDataInfo.DeletedItems.Any())
            {
                foreach (var deletedItem in responseMasterDataInfo.DeletedItems)
                {
                    var activityType = _activityTypeRepository.GetById(deletedItem, true);
                    if (activityType != null)
                    {
                        try
                        {
                            _activityTypeRepository.SetAsDeleted(activityType);
                        }
                        catch (Exception ex)
                        {
                            _log.Error("Deleting Activity Type error-" + ex.ToString());

                        }
                        
                    }
                    
                }

            }
            responseMasterDataInfo.MasterData.MasterDataItems
                .OfType<ActivityTypeDTO>()
                .Select(n => _mapper.Map(n)).ToList()
                .ForEach(n => _activityTypeRepository.Save(n, true));
        }

        private void SaveSeasons(ResponseMasterDataInfo responseMasterDataInfo)
        {
            if (responseMasterDataInfo.DeletedItems.Any())
            {
                foreach (var deletedItem in responseMasterDataInfo.DeletedItems)
                {
                    var season = _seasonRepository.GetById(deletedItem, true);
                    if (season != null)
                    {
                        try
                        {
                            _seasonRepository.SetAsDeleted(season);
                        }
                        catch (Exception ex)
                        {
                            _log.Error("Deleting Season error-" + ex.ToString());

                        }
                        
                    }

                }

            }
            responseMasterDataInfo.MasterData.MasterDataItems
               .OfType<SeasonDTO>()
               .Select(n => _mapper.Map(n)).ToList()
               .ForEach(n => _seasonRepository.Save(n, true));
        }

        private void SaveInfections(ResponseMasterDataInfo responseMasterDataInfo)
        {
            if (responseMasterDataInfo.DeletedItems.Any())
            {
                foreach (var deletedItem in responseMasterDataInfo.DeletedItems)
                {
                    var infection = _infectionRepository.GetById(deletedItem, true);
                    if (infection != null)
                    {
                        try
                        {
                            _infectionRepository.SetAsDeleted(infection);
                        }
                        catch (Exception ex)
                        {
                            _log.Error("Deleting Infection error-" + ex.ToString());

                        }
                        
                    }

                }

            }
            responseMasterDataInfo.MasterData.MasterDataItems
               .OfType<InfectionDTO>()
               .Select(n => _mapper.Map(n)).ToList()
               .ForEach(n => _infectionRepository.Save(n, true));
        }

        private void SaveShifts(ResponseMasterDataInfo responseMasterDataInfo)
        {
            if (responseMasterDataInfo.DeletedItems.Any())
            {
                foreach (var deletedItem in responseMasterDataInfo.DeletedItems)
                {
                    var shift = _shiftRepository.GetById(deletedItem, true);
                    if (shift != null)
                    {
                        try
                        {
                            _shiftRepository.SetAsDeleted(shift);
                        }
                        catch (Exception ex)
                        {
                            _log.Error("Deleting Shift error-" + ex.ToString());

                        }
                        
                    }

                }

            }
            responseMasterDataInfo.MasterData.MasterDataItems
              .OfType<ShiftDTO>()
              .Select(n => _mapper.Map(n)).ToList()
              .ForEach(n => _shiftRepository.Save(n, true));
        }

        private void SaveServices(ResponseMasterDataInfo responseMasterDataInfo)
        {
            if (responseMasterDataInfo.DeletedItems.Any())
            {
                foreach (var deletedItem in responseMasterDataInfo.DeletedItems)
                {
                    var service = _serviceRepository.GetById(deletedItem, true);
                    if (service != null)
                    {
                        try
                        {
                            _serviceRepository.SetAsDeleted(service);
                        }
                        catch (Exception ex)
                        {
                            _log.Error("Deleting service error-" + ex.ToString());

                        }
                      
                    }

                }

            }
            responseMasterDataInfo.MasterData.MasterDataItems
              .OfType<ServiceDTO>()
              .Select(n => _mapper.Map(n)).ToList()
              .ForEach(n => _serviceRepository.Save(n, true));
        }

        private void SaveServiceProviders(ResponseMasterDataInfo responseMasterDataInfo)
        {
            if (responseMasterDataInfo.DeletedItems.Any())
            {
                foreach (var deletedItem in responseMasterDataInfo.DeletedItems)
                {
                    var serviceProvider = _serviceProviderRepository.GetById(deletedItem, true);
                    if (serviceProvider != null)
                    {
                        try
                        {
                            _serviceProviderRepository.SetAsDeleted(serviceProvider);
                        }
                        catch (Exception ex)
                        {
                            _log.Error("Deleting Service Provider error-" + ex.ToString());

                        }
                       
                    }

                }

            }
            responseMasterDataInfo.MasterData.MasterDataItems
              .OfType<ServiceProviderDTO>()
              .Select(n => _mapper.Map(n)).ToList()
              .ForEach(n => _serviceProviderRepository.Save(n, true));
        }

        private void SaveRouteCentreAllocation(ResponseMasterDataInfo responseMasterDataInfo)
        {
            if (responseMasterDataInfo.DeletedItems.Any())
            {
                foreach (var deletedItem in responseMasterDataInfo.DeletedItems)
                {
                    var routeCentre = _masterDataAllocationRepository.GetById(deletedItem, true);
                    if (routeCentre != null)
                    {
                        try
                        {
                            _masterDataAllocationRepository.SetAsDeleted(routeCentre);
                        }
                        catch (Exception ex)
                        {
                            _log.Error("Deleting Route Centre Allocation error-" + ex.ToString());

                        }
                       
                    }

                }

            }
            responseMasterDataInfo.MasterData.MasterDataItems
                .OfType<RouteCentreAllocationDTO>()
                .Select(n => _mapper.Map(n)).ToList()
                .ForEach(n => _masterDataAllocationRepository.Save(n, true));
        }

        private void SaveRouteRegionAllocation(ResponseMasterDataInfo responseMasterDataInfo)
        {
            responseMasterDataInfo.MasterData.MasterDataItems
                .OfType<RouteRegionAllocationDTO>()
                .Select(n => _mapper.Map(n)).ToList()
                .ForEach(n => _masterDataAllocationRepository.Save(n, true));
        }

        private void SaveRouteCostCentreAllocation(ResponseMasterDataInfo responseMasterDataInfo)
        {
            responseMasterDataInfo.MasterData.MasterDataItems
                .OfType<RouteCostCentreAllocationDTO>()
                .Select(n => _mapper.Map(n)).ToList()
                .ForEach(n => _masterDataAllocationRepository.Save(n, true));
        }

        private void SaveFarmCentreAllocation(ResponseMasterDataInfo responseMasterDataInfo)
        {
            if (responseMasterDataInfo.DeletedItems.Any())
            {
                foreach (var deletedItem in responseMasterDataInfo.DeletedItems)
                {
                    var masterDataAllocation = _masterDataAllocationRepository.GetById(deletedItem, true);
                    if (masterDataAllocation != null)
                    {
                        try
                        {
                            _masterDataAllocationRepository.SetAsDeleted(masterDataAllocation);
                        }
                        catch (Exception ex)
                        {
                            _log.Error("Deleting Farm Centre Allocation error-" + ex.ToString());

                        }
                       
                    }

                }

            }
            responseMasterDataInfo.MasterData.MasterDataItems
                .OfType<CommodityProducerCentreAllocationDTO>()
                .Select(n => _mapper.Map(n)).ToList()
                .ForEach(n => _masterDataAllocationRepository.Save(n, true));
        }

        private void SaveRetireSetting(ResponseMasterDataInfo responseMasterDataInfo)
        {
            if (responseMasterDataInfo.DeletedItems.Any())
            {
                foreach (var deletedItem in responseMasterDataInfo.DeletedItems)
                {
                    var retireSetting = _retireSettingRepository.GetById(deletedItem, true);
                    if (retireSetting != null)
                    {
                        try
                        {
                            _retireSettingRepository.SetAsDeleted(retireSetting);
                        }
                        catch (Exception ex)
                        {
                            _log.Error("Deleting Retire Setting error-" + ex.ToString());

                        }
                        
                    }

                }

            }

            responseMasterDataInfo.MasterData.MasterDataItems
                .OfType<RetireSettingDTO>()
                .OrderBy(n => n.StatusId)
                .Select(n => _mapper.Map(n)).ToList()
                .ForEach(n => _retireSettingRepository.Save(n, true));
        }

       public void UpdateInventoryDB(ResponseMasterDataInfo responseMasterDataInfo)
       {
           responseMasterDataInfo.MasterData.MasterDataItems
                 .OfType<InventoryDTO>()
                  .Select(n => _mapper.Map(n)).ToList()
                 .ForEach(n => _inventoryRepository.UpdateFromServer(n));
       }

       public void UpdatePaymentDB(ResponseMasterDataInfo responseMasterDataInfo)
       {
           responseMasterDataInfo.MasterData.MasterDataItems
                  .OfType<PaymentTrackerDTO>()
                  .Select(n => _mapper.Map(n)).ToList()
                  .ForEach(n => _paymentTrackerRepository.Save(n));
       }

        public void UnderBankingDB(ResponseMasterDataInfo responseMasterDataInfo)
        {
           var data= responseMasterDataInfo.MasterData.MasterDataItems.OfType<UnderBankingDTO>().ToList();
            foreach (var underBankingDTO in data)
            {
                DateTime date = DateTime.Now;
                var doc = _cokeDataContext.tblRecollection.FirstOrDefault(s => s.Id == underBankingDTO.MasterId);
                if (doc == null)
                {
                    doc = new tblRecollection { Id = underBankingDTO.MasterId };
                    doc.IM_DateCreated = date;
                    _cokeDataContext.tblRecollection.AddObject(doc);
                }
                foreach (var paid  in underBankingDTO.Items)
                {
                    var item = doc.tblRecollectionItem.FirstOrDefault(s => s.Id == paid.Id);
                    if(item== null)
                    {

                        item= new tblRecollectionItem();
                        item.IM_DateCreated = date;
                        item.Id = paid.Id;
                        item.IsComfirmed = paid.IsConfirmed;
                        _cokeDataContext.tblRecollectionItem.AddObject(item);
                    }
                    item.IM_DateLastUpdated = date;
                    item.DateInserted = date;
                    item.Amount = paid.Amount;
                    item.RecollectionId = doc.Id;

                }
            
               
                doc.IsReceived = false;
                doc.IM_Status = (int)EntityStatus.Active;
                doc.Amount = underBankingDTO.Amount;
                doc.FromCostCentreId = underBankingDTO.CostCentreId;
                doc.CostCentreId = underBankingDTO.CostCentreId;
                doc.Description = underBankingDTO.Description;
                doc.DateInserted = date;
                doc.IM_DateLastUpdated = date;
                _cokeDataContext.SaveChanges();
            }
                 
        }


        private void SaveSetting(ResponseMasterDataInfo responseMasterDataInfo)
       {
           if (responseMasterDataInfo.DeletedItems.Any())
           {
               foreach (var deletedItem in responseMasterDataInfo.DeletedItems)
               {
                   var appSettings = _appSettingsRepository.GetById(deletedItem, true);
                   if (appSettings != null)
                   {
                       try
                       {
                           _appSettingsRepository.SetAsDeleted(appSettings);
                       }
                       catch (Exception ex)
                       {
                           _log.Error("Deleting App Setting error-" + ex.ToString());

                       }
                      
                   }

               }

           }
           responseMasterDataInfo.MasterData.MasterDataItems
               .OfType<AppSettingsDTO>()
               .OrderBy(n => n.StatusId)
               .Select(n => _mapper.Map(n)).ToList()
               .ForEach(n => _appSettingsRepository.Save(n));
       }

       private void SaveTargetItem(ResponseMasterDataInfo responseMasterDataInfo)
       {
           responseMasterDataInfo.MasterData.MasterDataItems
               .OfType<TargetItemDTO>()
               .OrderBy(n => n.StatusId)
               .Select(n => _mapper.Map(n)).ToList()
               .ForEach(n => _targetItemRepository.Save(n, true));
       }

       private void SaveOutletVisitDay(ResponseMasterDataInfo responseMasterDataInfo)
       {
           responseMasterDataInfo.MasterData.MasterDataItems
               .OfType<OutletVisitDayDTO>()
               .OrderBy(n => n.StatusId)
               .Select(n => _mapper.Map(n)).ToList()
               .ForEach(n => _outletVisitDayRepository.Save(n, true));
       }

       private void SaveOutletPriority(ResponseMasterDataInfo responseMasterDataInfo)
       {
           responseMasterDataInfo.MasterData.MasterDataItems
               .OfType<OutletPriorityDTO>()
               .OrderBy(n => n.StatusId)
               .Select(n => _mapper.Map(n)).ToList()
               .ForEach(n => _outletPriorityRepository.Save(n, true));
       }

       private void SaveAssetStatus(ResponseMasterDataInfo responseMasterDataInfo)
       {
           if (responseMasterDataInfo.DeletedItems.Any())
           {
               foreach (var deletedItem in responseMasterDataInfo.DeletedItems)
               {
                   var assetStatus = _assetStatusRepository.GetById(deletedItem, true);
                   if (assetStatus != null)
                   {
                       try
                       {
                           _assetStatusRepository.SetAsDeleted(assetStatus);
                       }
                       catch (Exception ex)
                       {
                           _log.Error("Deleting Asset error-" + ex.ToString());

                       }
                       
                   }

               }

           }
           responseMasterDataInfo.MasterData.MasterDataItems
              .OfType<AssetStatusDTO>()
              .OrderBy(n => n.StatusId)
              .Select(n => _mapper.Map(n)).ToList()
              .ForEach(n => _assetStatusRepository.Save(n, true));
       }

       private void SaveAssetCategory(ResponseMasterDataInfo responseMasterDataInfo)
       {
           if (responseMasterDataInfo.DeletedItems.Any())
           {
               foreach (var deletedItem in responseMasterDataInfo.DeletedItems)
               {
                   var assetCategory = _assetCategoryRepository.GetById(deletedItem, true);
                   if (assetCategory != null)
                   {
                       try
                       {
                           _assetCategoryRepository.SetAsDeleted(assetCategory);
                       }
                       catch (Exception ex)
                       {
                           _log.Error("Deleting Asset Category error-" + ex.ToString());

                       }
                       
                   }

               }

           }
           responseMasterDataInfo.MasterData.MasterDataItems
               .OfType<AssetCategoryDTO>()
               .OrderBy(n => n.StatusId)
               .Select(n => _mapper.Map(n)).ToList()
               .ForEach(n => _assetCategoryRepository.Save(n, true));
       }

       private void SaveContactType(ResponseMasterDataInfo responseMasterDataInfo)
       {
           if (responseMasterDataInfo.DeletedItems.Any())
           {
               foreach (var deletedItem in responseMasterDataInfo.DeletedItems)
               {
                   var contactType = _contactTypeRepository.GetById(deletedItem, true);
                   if (contactType != null)
                   {
                       try
                       {
                           _contactTypeRepository.SetAsDeleted(contactType);
                       }
                       catch (Exception ex)
                       {
                           _log.Error("Deleting Contact Type error-" + ex.ToString());

                       }
                       
                   }

               }

           }
           responseMasterDataInfo.MasterData.MasterDataItems
               .OfType<ContactTypeDTO>()
               .OrderBy(n => n.StatusId)
               .Select(n => _mapper.Map(n)).ToList()
               .ForEach(n => _contactTypeRepository.Save(n, true));
       }
        
       private void SaveUserGroupRole(ResponseMasterDataInfo responseMasterDataInfo)
       {
           if (responseMasterDataInfo.DeletedItems.Any())
           {
               foreach (var deletedItem in responseMasterDataInfo.DeletedItems)
               {
                   var userGroupRole = _userGroupRoleRepository.GetById(deletedItem, true);
                   if (userGroupRole != null)
                   {
                       try
                       {
                           _userGroupRoleRepository.SetAsDeleted(userGroupRole);
                       }
                       catch (Exception ex)
                       {
                           _log.Error("Deleting User Group Role error-" + ex.ToString());

                       }
                       
                   }

               }

           }
           responseMasterDataInfo.MasterData.MasterDataItems
               .OfType<UserGroupRoleDTO>()
               .OrderBy(n => n.StatusId)
               .Select(n => _mapper.Map(n)).ToList()
               .ForEach(n => _userGroupRoleRepository.Save(n, true));
       }

       private void SaveUserGroup(ResponseMasterDataInfo responseMasterDataInfo)
       {
           if (responseMasterDataInfo.DeletedItems.Any())
           {
               foreach (var deletedItem in responseMasterDataInfo.DeletedItems)
               {
                   var userGroup = _userGroupRepository.GetById(deletedItem, true);
                   if (userGroup != null)
                   {
                       try
                       {
                           _userGroupRepository.SetAsDeleted(userGroup);
                       }
                       catch (Exception ex)
                       {
                           _log.Error("Deleting User Group error-" + ex.ToString());

                       }
                       
                   }

               }

           }
           responseMasterDataInfo.MasterData.MasterDataItems
                .OfType<UserGroupDTO>()
                .OrderBy(n => n.StatusId)
                .Select(n => _mapper.Map(n)).ToList()
                .ForEach(n => _userGroupRepository.Save(n, true));
       }

       private void SaveSalesmanSupplier(ResponseMasterDataInfo responseMasterDataInfo)
       {
           if(responseMasterDataInfo.DeletedItems.Any())
           {
               foreach (var deletedItem in responseMasterDataInfo.DeletedItems)
               {
                   try
                   {
                       _salesmanSupplierRepository.Delete(deletedItem);
                   }
                   catch (Exception ex)
                   {
                       _log.Error("Deleting Salesman Supplier error-" + ex.ToString());

                   }
                   
               }
              
           }
           responseMasterDataInfo.MasterData.MasterDataItems
               .OfType<SalesmanSupplierDTO>()
               .OrderBy(n => n.StatusId)
               .Select(n => _mapper.Map(n)).ToList()
               .ForEach(n => _salesmanSupplierRepository.Save(n, true));
       }

             private void SaveSalesmanRoute(ResponseMasterDataInfo responseMasterDataInfo)
       {
           if(responseMasterDataInfo.DeletedItems.Any())
           {
               foreach (var deletedItem in responseMasterDataInfo.DeletedItems)
               {
                   try
                   {
                       _salesmanRouteRepository.Delete(deletedItem);
                   }
                   catch (Exception ex)
                   {
                       _log.Error("Deleting Salesman Route error-" + ex.ToString());

                   }
                   
               }
              
           }
           responseMasterDataInfo.MasterData.MasterDataItems
               .OfType<SalesmanRouteDTO>()
               .OrderBy(n => n.StatusId)
               .Select(n => _mapper.Map(n)).ToList()
               .ForEach(n => _salesmanRouteRepository.Save(n, true));
       }
         
       private void SaveCoolerType(ResponseMasterDataInfo responseMasterDataInfo)
       {
           responseMasterDataInfo.MasterData.MasterDataItems
               .OfType<AssetTypeDTO>()
               .OrderBy(n => n.StatusId)
               .Select(n => _mapper.Map(n)).ToList()
               .ForEach(n => _coolerTypeRepository.Save(n, true));
       }

       private void SaveCooler(ResponseMasterDataInfo responseMasterDataInfo)
       {
           responseMasterDataInfo.MasterData.MasterDataItems
               .OfType<AssetDTO>()
               .OrderBy(n => n.StatusId)
               .Select(n => _mapper.Map(n)).ToList()
               .ForEach(n => _coolerRepository.Save(n, true));
       }

       private void SaveCompetitorProduct(ResponseMasterDataInfo responseMasterDataInfo)
       {
           if (responseMasterDataInfo.DeletedItems.Any())
           {
               foreach (var deletedItem in responseMasterDataInfo.DeletedItems)
               {
                   var competitorProduct = _competitorProductRepository.GetById(deletedItem, true);
                   if (competitorProduct != null)
                   {
                       try
                       {
                           _competitorProductRepository.SetAsDeleted(competitorProduct);
                       }
                       catch (Exception ex)
                       {
                           _log.Error("Deleting Competitor Product error-" + ex.ToString());

                       }
                       
                   }

               }

           }
           responseMasterDataInfo.MasterData.MasterDataItems
                 .OfType<CompetitorProductDTO>()
                 .OrderBy(n => n.StatusId)
                 .Select(n => _mapper.Map(n)).ToList()
                 .ForEach(n => _competitorProductRepository.Save(n, true));
       }

       private void SaveCompetitor(ResponseMasterDataInfo responseMasterDataInfo)
       {
           if (responseMasterDataInfo.DeletedItems.Any())
           {
               foreach (var deletedItem in responseMasterDataInfo.DeletedItems)
               {
                   var competitor = _competitorRepository.GetById(deletedItem, true);
                   if (competitor != null)
                   {
                       try
                       {
                           _competitorRepository.SetAsDeleted(competitor);
                       }
                       catch (Exception ex)
                       {
                           _log.Error("Deleting Competitor error-" + ex.ToString());

                       }
                       
                   }

               }

           }
           responseMasterDataInfo.MasterData.MasterDataItems
                .OfType<CompetitorDTO>()
                .OrderBy(n => n.StatusId)
                .Select(n => _mapper.Map(n)).ToList()
                .ForEach(n => _competitorRepository.Save(n, true));
       }

       private void SaveChannelPackaging(ResponseMasterDataInfo responseMasterDataInfo)
       {
           responseMasterDataInfo.MasterData.MasterDataItems
                  .OfType<ChannelPackagingDTO>()
                  .OrderBy(n => n.StatusId)
                  .Select(n => _mapper.Map(n)).ToList()
                  .ForEach(n => _channelPackagingRepository.Save(n, true));
       }

       private void SaveFreeOfChargeDiscount(ResponseMasterDataInfo responseMasterDataInfo)
       {
           if (responseMasterDataInfo.DeletedItems.Any())
           {
               foreach (var deletedItem in responseMasterDataInfo.DeletedItems)
               {
                   var freeOfChargeDiscount = _freeOfChargeDiscountRepository.GetById(deletedItem, true);
                   if (freeOfChargeDiscount != null)
                   {
                       try
                       {
                           _freeOfChargeDiscountRepository.SetAsDeleted(freeOfChargeDiscount);
                       }
                       catch (Exception ex)
                       {
                           _log.Error("Deleting Free of Charge Discount  error-" + ex.ToString());

                       }
                       
                   }

               }

           }

           responseMasterDataInfo.MasterData.MasterDataItems
               .OfType<FreeOfChargeDiscountDTO>()
               .OrderBy(n => n.StatusId)
               .Select(n => _mapper.Map(n)).ToList()
               .ForEach(n => _freeOfChargeDiscountRepository.Save(n, true));
       }

       private void SaveDistributorPendingDispatchWarehouse(ResponseMasterDataInfo responseMasterDataInfo)
       {
           responseMasterDataInfo.MasterData.MasterDataItems
               .OfType<DistributorPendingDispatchWarehouseDTO>()
               .OrderBy(n => n.StatusId)
               .Select(n => _mapper.Map(n)).ToList()
               .ForEach(n => _distributorPendingDispatchWarehouseRepository.Save(n, true));
       }

       private void SaveDistributorSalesman(ResponseMasterDataInfo responseMasterDataInfo)
       {
           responseMasterDataInfo.MasterData.MasterDataItems
               .OfType<DistributorSalesmanDTO>()
               .OrderBy(n => n.StatusId)
               .Select(n => _mapper.Map(n))
               .ToList().ForEach(n => _distributorSalesmanRepository.Save(n, true));
       }

      

       private void SaveProductPackaging(ResponseMasterDataInfo responseMasterDataInfo)
       {
           if (responseMasterDataInfo.DeletedItems.Any())
           {
               foreach (var deletedItem in responseMasterDataInfo.DeletedItems)
               {
                   var productPackaging = _productPackagingRepository.GetById(deletedItem, true);
                   if (productPackaging != null)
                   {
                       try
                       {
                           _productPackagingRepository.SetAsDeleted(productPackaging);
                       }
                       catch (Exception ex)
                       {
                           _log.Error("Deleting Product Packaging error-" + ex.ToString());

                       }
                      
                   }

               }

           }
           responseMasterDataInfo.MasterData.MasterDataItems
               .OfType<ProductPackagingDTO>()
               .Where(n => n.ReturnableProductMasterId == Guid.Empty)
               .OrderBy(n => n.StatusId)
               .Select(n => _mapper.Map(n))
               .ToList().ForEach(n => _productPackagingRepository.Save(n, true));
       }

       private void SaveAreas(ResponseMasterDataInfo responseMasterDataInfo)
       {
           if (responseMasterDataInfo.DeletedItems.Any())
           {
               foreach (var deletedItem in responseMasterDataInfo.DeletedItems)
               {
                   var area = _areaRepository.GetById(deletedItem, true);
                   if (area != null)
                   {
                       try
                       {
                           _areaRepository.SetAsDeleted(area);
                       }
                       catch (Exception ex)
                       {
                           _log.Error("Deleting Area error-" + ex.ToString());

                       }
                       
                   }

               }

           }
           responseMasterDataInfo.MasterData.MasterDataItems
              .OfType<AreaDTO>()
              .OrderBy(n => n.StatusId)
              .Select(n => _mapper.Map(n))
              .ToList().ForEach(n => _areaRepository.Save(n, true));
       }

       private void SaveConsolidatedProduct(ResponseMasterDataInfo responseMasterDataInfo)
       {
           if (responseMasterDataInfo.DeletedItems.Any())
           {
               foreach (var deletedItem in responseMasterDataInfo.DeletedItems)
               {
                   var consolidatedProduct = _productRepository.GetById(deletedItem, true);
                   if (consolidatedProduct != null)
                   {
                       try
                       {
                           _productRepository.SetAsDeleted(consolidatedProduct);
                       }
                       catch (Exception ex)
                       {
                           _log.Error("Deleting Consolidated Product error-" + ex.ToString());

                       }
                      
                   }

               }

           }
          responseMasterDataInfo.MasterData.MasterDataItems
              .OfType<ConsolidatedProductDTO>()
              .OrderBy(n => n.StatusId)
              .Select(n => _mapper.Map(n))
              .ToList()
              .ForEach(n => _productRepository.Save(n, true));

          
       }

       private void SaveDistributors(ResponseMasterDataInfo responseMasterDataInfo)
       {
           //if (responseMasterDataInfo.DeletedItems.Any())
           //{
           //    foreach (var deletedItem in responseMasterDataInfo.DeletedItems)
           //    {
           //        var distributor = _distributorRepository.GetById(deletedItem, true);
           //        if (distributor != null)
           //        {
           //            _distributorRepository.SetAsDeleted(distributor);
           //        }

           //    }

           //}

           // dist
           responseMasterDataInfo.MasterData.MasterDataItems
              .OfType<DistributorDTO>()
              .OrderBy(n => n.StatusId)
              .Select(n => _mapper.Map(n))
              .ToList().ForEach(n => _distributorRepository.Save(n, true));

           responseMasterDataInfo.MasterData.MasterDataItems
               .OfType<DistributorDTO>()
               .Select(n => n.ASMDTO)
               .Where(n => n != null && n.MasterId != Guid.Empty)
               .OrderBy(n => n.StatusId)
               .Select(n => _mapper.Map(n))
               .ToList().ForEach(n => _userRepository.Save(n, true));

           responseMasterDataInfo.MasterData.MasterDataItems
              .OfType<DistributorDTO>()
              .Select(n => n.SurveyorDTO)
              .Where(n => n != null && n.MasterId != Guid.Empty)
              .OrderBy(n => n.StatusId)
              .Select(n => _mapper.Map(n))
              .ToList().ForEach(n => _userRepository.Save(n, true));

           responseMasterDataInfo.MasterData.MasterDataItems
              .OfType<DistributorDTO>()
              .Select(n => n.SalesRepDTO)
              .Where(n => n != null && n.MasterId != Guid.Empty)
              .OrderBy(n => n.StatusId)
              .Select(n => _mapper.Map(n))
              .ToList().ForEach(n => _userRepository.Save(n, true));
       }

       private void SaveRegions(ResponseMasterDataInfo responseMasterDataInfo)
       {
           if (responseMasterDataInfo.DeletedItems.Any())
           {
               foreach (var deletedItem in responseMasterDataInfo.DeletedItems)
               {
                   var region = _regionRepository.GetById(deletedItem, true);
                   if (region != null)
                   {
                       try
                       {
                           _regionRepository.SetAsDeleted(region);
                       }
                       catch (Exception ex)
                       {
                           _log.Error("Deleting Region error-" + ex.ToString());
                           
                       }
                       
                   }

               }

           }
           responseMasterDataInfo.MasterData.MasterDataItems
              .OfType<RegionDTO>()
              .OrderBy(n => n.StatusId)
              .Select(n => _mapper.Map(n))
              .ToList().ForEach(n => _regionRepository.Save(n, true));
       }

       private void SaveReturnableProduct(ResponseMasterDataInfo responseMasterDataInfo)
       {

           if (responseMasterDataInfo.DeletedItems.Any())
           {
               foreach (var deletedItem in responseMasterDataInfo.DeletedItems)
               {
                   var returnableProduct = _productRepository.GetById(deletedItem, true);
                   if (returnableProduct != null)
                   {
                       try
                       {
                           _productRepository.SetAsDeleted(returnableProduct);
                       }
                       catch (Exception ex)
                       {
                           _log.Error("Deleting Returnable Product error-" + ex.ToString());

                       }
                      
                   }

               }

           }

           responseMasterDataInfo.MasterData.MasterDataItems
               .OfType<ReturnableProductDTO>().Where(s=>s.ReturnableProductMasterId==Guid.Empty)
               .OrderBy(n => n.StatusId)
               .Select(n => _mapper.Map(n))
               .ToList().ForEach(n => _productRepository.Save(n, true));
          
           responseMasterDataInfo.MasterData.MasterDataItems
               .OfType<ReturnableProductDTO>().Where(s => s.ReturnableProductMasterId != Guid.Empty)
               .OrderBy(n => n.StatusId)
               .Select(n => _mapper.Map(n))
               .ToList().ForEach(n => _productRepository.Save(n, true));
       }

       private void SaveSaleProduct(ResponseMasterDataInfo responseMasterDataInfo)
       {
           if (responseMasterDataInfo.DeletedItems.Any())
           {
               foreach (var deletedItem in responseMasterDataInfo.DeletedItems)
               {
                   var saleProduct = _productRepository.GetById(deletedItem, true);
                   if (saleProduct != null)
                   {
                       try
                       {
                           _productRepository.SetAsDeleted(saleProduct);
                       }
                       catch (Exception ex)
                       {
                           _log.Error("Deleting Sale Product  error-" + ex.ToString());

                       }
                      
                   }

               }

           }

           responseMasterDataInfo.MasterData.MasterDataItems
               .OfType<SaleProductDTO>()
               .OrderBy(n => n.StatusId)
               .Select(n => _mapper.Map(n))
               .ToList().ForEach(n => _productRepository.Save(n, true));
       }

       private void SaveUsers(ResponseMasterDataInfo responseMasterDataInfo)
       {
           responseMasterDataInfo.MasterData.MasterDataItems
               .OfType<UserDTO>()
               .OrderBy(n => n.StatusId)
               .Select(n => _mapper.Map(n))
               .ToList().ForEach(n => _userRepository.Save(n, true));
       }

       private void SaveVatClass(ResponseMasterDataInfo responseMasterDataInfo)
       {
           if (responseMasterDataInfo.DeletedItems.Any())
           {
               foreach (var deletedItem in responseMasterDataInfo.DeletedItems)
               {
                   var vatClass = _vatClassRepository.GetById(deletedItem, true);
                   if (vatClass != null)
                   {
                       try
                       {
                           _vatClassRepository.SetAsDeleted(vatClass);
                       }
                       catch (Exception ex)
                       {
                           _log.Error("Deleting VAT Class error-" + ex.ToString());

                       }
                       
                   }

               }

           }
           responseMasterDataInfo.MasterData.MasterDataItems
               .OfType<VATClassDTO>()
               .OrderBy(n => n.StatusId)
               .Select(n => _mapper.Map(n))
               .ToList().ForEach(n => _vatClassRepository.Save(n, true));
       }

       private void SavePricing(ResponseMasterDataInfo responseMasterDataInfo)
       {
           //if (responseMasterDataInfo.DeletedItems.Any())
           //{
           //    foreach (var deletedItem in responseMasterDataInfo.DeletedItems)
           //    {
           //        var productPricing = _productPricingRepository.GetById(deletedItem, true);
           //        if (productPricing != null)
           //        {
           //            _productPricingRepository.SetAsDeleted(productPricing);
           //        }

           //    }

           //}
           responseMasterDataInfo.MasterData.MasterDataItems
               .OfType<ProductPricingDTO>()
               .OrderBy(n => n.StatusId)
               .Select(n => _mapper.Map(n))
               .ToList().ForEach(n => _productPricingRepository.Save(n, true));
       }
       private void SavePricingDirect(ResponseMasterDataInfo responseMasterDataInfo)
       {
           var pricings = responseMasterDataInfo.MasterData.MasterDataItems
               .OfType<ProductPricingDTO>()
               .OrderBy(n => n.StatusId).ToArray();
           foreach (var entity in pricings)
           {
               var dt = DateTime.Now;
               if(!_cokeDataContext.tblProduct.Any(p=>p.id==entity.ProductMasterId))continue;
               tblPricing pricing = _cokeDataContext.tblPricing.FirstOrDefault(n => n.id == entity.MasterId);
               if (pricing == null)
               {

                   pricing = new tblPricing
                                 {IM_DateCreated = dt, IM_Status = (int) EntityStatus.Active, id = entity.MasterId};
                   _cokeDataContext.tblPricing.AddObject(pricing);
               }
               
              
               var entityStatus = (entity.StatusId == (int) EntityStatus.New)
                                      ? EntityStatus.Active
                                      : (EntityStatus) entity.StatusId;
               if (pricing.IM_Status != (int) entityStatus)
                   pricing.IM_Status = (int) entity.StatusId;
               var pricingItem = pricing.tblPricingItem.FirstOrDefault(p => p.id == entity.LineItemId);
               if (pricingItem == null)
               {
                   pricingItem = new tblPricingItem
                                     {
                                         id = Guid.NewGuid(),
                                         IM_Status = (int) EntityStatus.Active
                                     };
                   pricing.tblPricingItem.Add(pricingItem);
               }
               pricingItem.Exfactory = entity.ExFactoryRate;
               pricingItem.SellingPrice = entity.SellingPrice;
               pricingItem.EffecitiveDate = entity.EffectiveDate;
               pricingItem.IM_DateCreated = dt;
               pricingItem.IM_DateLastUpdated = dt;
               pricing.ProductRef = entity.ProductMasterId;
               pricing.Tier = entity.ProductPricingTierMasterId;
               pricing.IM_DateLastUpdated = dt;

           }
           _cokeDataContext.SaveChanges();
       }

       private void SaveRoutes(ResponseMasterDataInfo responseMasterDataInfo)
       {
           if (responseMasterDataInfo.DeletedItems.Any())
           {
               foreach (var deletedItem in responseMasterDataInfo.DeletedItems)
               {
                   var route = _routeRepository.GetById(deletedItem, true);
                   if (route != null)
                   {
                       try
                       {
                           _routeRepository.SetAsDeleted(route);
                       }
                       catch (Exception ex)
                       {
                           _log.Error("Deleting Route error-" + ex.ToString());

                       }
                       
                   }

               }

           }
           responseMasterDataInfo.MasterData.MasterDataItems
               .OfType<RouteDTO>()
               .OrderBy(n => n.StatusId)
               .Select(n => _mapper.Map(n))
               .ToList().ForEach(n => _routeRepository.Save(n, true));
       }

       private void SaveContact(ResponseMasterDataInfo responseMasterDataInfo)
       {

           if (responseMasterDataInfo.DeletedItems.Any())
           {
               foreach (var deletedItem in responseMasterDataInfo.DeletedItems)
               {
                   var contact = _contactRepository.GetById(deletedItem, true);
                   if (contact != null)
                   {
                       try
                       {
                           _contactRepository.SetAsDeleted(contact);
                       }
                       catch (Exception ex)
                       {
                           _log.Error("Deleting Contact error-" + ex.ToString());

                       }
                      
                   }

               }

           }
           responseMasterDataInfo.MasterData.MasterDataItems
               .OfType<ContactDTO>()
               .OrderBy(n => n.StatusId)
               .Select(n => _mapper.Map(n))
               .ToList().ForEach(n => _contactRepository.Save(n, true));
       }

       private void SaveTransporters(ResponseMasterDataInfo responseMasterDataInfo)
       {
           responseMasterDataInfo.MasterData.MasterDataItems
               .OfType<TransporterDTO>()
               .OrderBy(n => n.StatusId)
               .Select(n => _mapper.Map(n))
               .ToList().ForEach(n => _transporterRepository.Save(n, true));
       }

       private void SaveOutlets(ResponseMasterDataInfo responseMasterDataInfo)
       {

           if (responseMasterDataInfo.DeletedItems.Any())
           {
               foreach (var deletedItem in responseMasterDataInfo.DeletedItems)
               {
                   var outlet = _outletRepository.GetById(deletedItem, true);
                   if (outlet != null)
                   {
                       try
                       {
                           _outletRepository.SetAsDeleted(outlet);
                       }
                       catch (Exception ex)
                       {
                           _log.Error("Deleting Outlet error-" + ex.ToString());

                       }
                       
                   }

               }

           }
           responseMasterDataInfo.MasterData.MasterDataItems
               .OfType<OutletDTO>()
               .OrderBy(n => n.StatusId)
               .Select(n => _mapper.Map(n))
               .ToList().ForEach(n => _outletRepository.Save(n, true));
       }

       private void SaveProducers(ResponseMasterDataInfo responseMasterDataInfo)
       {
           if (responseMasterDataInfo.DeletedItems.Any())
           {
               foreach (var deletedItem in responseMasterDataInfo.DeletedItems)
               {
                   var producer = _producerRepository.GetById(deletedItem, true);
                   if (producer != null)
                   {
                       try
                       {
                           _producerRepository.SetAsDeleted(producer);
                       }
                       catch (Exception ex)
                       {
                           _log.Error("Deleting Producer error-" + ex.ToString());

                       }

                       
                   }

               }

           }

           responseMasterDataInfo.MasterData.MasterDataItems
               .OfType<ProducerDTO>()
               .OrderBy(n => n.StatusId)
               .Select(n => _mapper.Map(n))
               .ToList().ForEach(n => _producerRepository.Save(n, true));
       }

       private void SaveCountries(ResponseMasterDataInfo responseMasterDataInfo)
       {
           if (responseMasterDataInfo.DeletedItems.Any())
           {
               foreach (var deletedItem in responseMasterDataInfo.DeletedItems)
               {
                   var country = _countryRepository.GetById(deletedItem, true);
                   if (country != null)
                   {
                       try
                       {
                           _countryRepository.SetAsDeleted(country);
                       }
                       catch (Exception ex)
                       {
                           _log.Error("Deleting Country error-" + ex.ToString());

                       }
                       
                   }

               }

           }
           responseMasterDataInfo.MasterData.MasterDataItems
               .OfType<CountryDTO>()
               .OrderBy(n => n.StatusId)
               .Select(n => _mapper.Map(n))
               .ToList().ForEach(n => _countryRepository.Save(n, true));
       }

       private void SaveSocioEconomicStatus(ResponseMasterDataInfo responseMasterDataInfo)
       {
           responseMasterDataInfo.MasterData.MasterDataItems
                .OfType<SocioEconomicStatusDTO>()
                .OrderBy(n => n.StatusId)
                .Select(n => _mapper.Map(n))
                .ToList().ForEach(n => _socioEconomicRepository.Save(n, true));
       }

       private void SaveProductPackagingTypes(ResponseMasterDataInfo responseMasterDataInfo)
       {
           if (responseMasterDataInfo.DeletedItems.Any())
           {
               foreach (var deletedItem in responseMasterDataInfo.DeletedItems)
               {
                   var productPackagingTypes = _productPackagingTypeRepository.GetById(deletedItem, true);
                   if (productPackagingTypes != null)
                   {
                       try
                       {
                           _productPackagingTypeRepository.SetAsDeleted(productPackagingTypes);
                       }
                       catch (Exception ex)
                       {
                           _log.Error("Deleting Product Packaging error-" + ex.ToString());

                       }
                       
                   }

               }

           }
           responseMasterDataInfo.MasterData.MasterDataItems
                .OfType<ProductPackagingTypeDTO>()
                .OrderBy(n => n.StatusId)
                .Select(n => _mapper.Map(n))
                .ToList().ForEach(n => _productPackagingTypeRepository.Save(n, true));
       }

       private void SaveProductTypes(ResponseMasterDataInfo responseMasterDataInfo)
       {
           if (responseMasterDataInfo.DeletedItems.Any())
           {
               foreach (var deletedItem in responseMasterDataInfo.DeletedItems)
               {
                   var productTypes = _productTypeRepository.GetById(deletedItem, true);
                   if (productTypes != null)
                   {
                       try
                       {
                           _productTypeRepository.SetAsDeleted(productTypes);
                       }
                       catch (Exception ex)
                       {
                           _log.Error("Deleting Product Type error-" + ex.ToString());

                       }
                       
                   }

               }

           }
           responseMasterDataInfo.MasterData.MasterDataItems
                .OfType<ProductTypeDTO>()
                .OrderBy(n => n.StatusId)
                .Select(n => _mapper.Map(n))
                .ToList().ForEach(n => _productTypeRepository.Save(n, true));
       }

       private void SaveProductFlavours(ResponseMasterDataInfo responseMasterDataInfo)
       {
           if (responseMasterDataInfo.DeletedItems.Any())
           {
               foreach (var deletedItem in responseMasterDataInfo.DeletedItems)
               {
                   var productFlavours = _productFlavourRepository.GetById(deletedItem, true);
                   if (productFlavours != null)
                   {
                       try
                       {
                           _productFlavourRepository.SetAsDeleted(productFlavours);
                       }
                       catch (Exception ex)
                       {
                           _log.Error("Deleting Product Flavour error-" + ex.ToString());

                       }
                       
                   }

               }

           }
           responseMasterDataInfo.MasterData.MasterDataItems
                .OfType<ProductFlavourDTO>()
                .OrderBy(n => n.StatusId)
                .Select(n => _mapper.Map(n))
                .ToList().ForEach(n => _productFlavourRepository.Save(n, true));
       }

       private void SavePricingTiers(ResponseMasterDataInfo responseMasterDataInfo)
       {
           if (responseMasterDataInfo.DeletedItems.Any())
           {
               foreach (var deletedItem in responseMasterDataInfo.DeletedItems)
               {
                   var productPricingTier = _productPricingTierRepository.GetById(deletedItem, true);
                   if (productPricingTier != null)
                   {
                       try
                       {
                           _productPricingTierRepository.SetAsDeleted(productPricingTier);
                       }
                       catch (Exception ex)
                       {
                           _log.Error("Deleting Product Pricing error-" + ex.ToString());

                       }
                       
                   }

               }

           }
           responseMasterDataInfo.MasterData.MasterDataItems
                .OfType<ProductPricingTierDTO>()
                .OrderBy(n => n.StatusId)
                .Select(n => _mapper.Map(n))
                .ToList().ForEach(n => _productPricingTierRepository.Save(n, true));
       }

       private void SaveOutletTypes(ResponseMasterDataInfo responseMasterDataInfo)
       {
           if (responseMasterDataInfo.DeletedItems.Any())
           {
               foreach (var deletedItem in responseMasterDataInfo.DeletedItems)
               {
                   var outletType = _outletTypeRepository.GetById(deletedItem, true);
                   if (outletType != null)
                   {
                       try
                       {
                           _outletTypeRepository.SetAsDeleted(outletType);
                       }
                       catch (Exception ex)
                       {
                           _log.Error("Deleting Outlet Type error-" + ex.ToString());

                       }
                       
                   }

               }

           }
           responseMasterDataInfo.MasterData.MasterDataItems
                  .OfType<OutletTypeDTO>()
                  .OrderBy(n => n.StatusId)
                  .Select(n => _mapper.Map(n))
                  .ToList().ForEach(n => _outletTypeRepository.Save(n, true));
       }

       private void SaveTerritories(ResponseMasterDataInfo responseMasterDataInfo)
       {
           if (responseMasterDataInfo.DeletedItems.Any())
           {
               foreach (var deletedItem in responseMasterDataInfo.DeletedItems)
               {
                   var territory = _territoryRepository.GetById(deletedItem, true);
                   if (territory != null)
                   {
                       try
                       {
                           _territoryRepository.SetAsDeleted(territory);
                       }
                       catch (Exception ex)
                       {
                           _log.Error("Deleting Territory error-" + ex.ToString());

                       }
                      
                   }

               }

           }

           responseMasterDataInfo.MasterData.MasterDataItems
               .OfType<TerritoryDTO>()
               .OrderBy(n => n.StatusId)
               .Select(n => _mapper.Map(n))
               .ToList().ForEach(n => _territoryRepository.Save(n, true));
       }

       private void SaveOutletCategories(ResponseMasterDataInfo responseMasterDataInfo)
       {
           if (responseMasterDataInfo.DeletedItems.Any())
           {
               foreach (var deletedItem in responseMasterDataInfo.DeletedItems)
               {
                   var outletCategory = _outletCategoryRepository.GetById(deletedItem, true);
                   if (outletCategory != null)
                   {
                       try
                       {
                           _outletCategoryRepository.SetAsDeleted(outletCategory);
                       }
                       catch (Exception ex)
                       {
                           _log.Error("Deleting Outlet Category error-" + ex.ToString());

                       }
                      
                   }

               }

           }
           responseMasterDataInfo.MasterData.MasterDataItems
               .OfType<OutletCategoryDTO>()
               .OrderBy(n => n.StatusId)
               .Select(n => _mapper.Map(n))
               .ToList().ForEach(n => _outletCategoryRepository.Save(n, true));
       }

       private void SaveProductBrands(ResponseMasterDataInfo responseMasterDataInfo)
       {
           if (responseMasterDataInfo.DeletedItems.Any())
           {
               foreach (var deletedItem in responseMasterDataInfo.DeletedItems)
               {
                   var productBrands = _productBrandRepository.GetById(deletedItem, true);
                   if (productBrands != null)
                   {
                      
                       try
                       {
                           _productBrandRepository.SetAsDeleted(productBrands);
                       }
                       catch (Exception ex)
                       {
                           _log.Error("Deleting Product Brand error-" + ex.ToString());

                       }
                   }

               }

           }
           responseMasterDataInfo.MasterData.MasterDataItems
               .OfType<ProductBrandDTO>()
               .OrderBy(n => n.StatusId)
               .Select(n => _mapper.Map(n))
               .ToList().ForEach(n => _productBrandRepository.Save(n, true));
       }

       private void SaveSaleValueDiscount(ResponseMasterDataInfo responseMasterDataInfo)
       {
           if (responseMasterDataInfo.DeletedItems.Any())
           {
               foreach (var deletedItem in responseMasterDataInfo.DeletedItems)
               {
                   var saleValueDiscount = _saleValueDiscountRepository.GetById(deletedItem, true);
                   if (saleValueDiscount != null)
                   {
                       try
                       {
                           _saleValueDiscountRepository.SetAsDeleted(saleValueDiscount);
                       }
                       catch (Exception ex)
                       {
                           _log.Error("Deleting Sale Value Discount error-" + ex.ToString());

                       }
                       
                   }

               }

           }
           responseMasterDataInfo.MasterData.MasterDataItems
               .OfType<SaleValueDiscountDTO>()
               .OrderBy(n => n.StatusId)
               .Select(n => _mapper.Map(n))
               .ToList().ForEach(n => _saleValueDiscountRepository.Save(n, true));
       }

       private void SavePromotionDiscount(ResponseMasterDataInfo responseMasterDataInfo)
       {
           if (responseMasterDataInfo.DeletedItems.Any())
           {
               foreach (var deletedItem in responseMasterDataInfo.DeletedItems)
               {
                   var promotionDiscount = _promotionDiscountRepository.GetById(deletedItem, true);
                   if (promotionDiscount != null)
                   {
                       try
                       {
                           _promotionDiscountRepository.SetAsDeleted(promotionDiscount);
                       }
                       catch (Exception ex)
                       {
                           _log.Error("Deleting Promotion Discount error-" + ex.ToString());

                       }
                       
                   }

               }

           }
           responseMasterDataInfo.MasterData.MasterDataItems
               .OfType<PromotionDiscountDTO>()
               .OrderBy(n => n.StatusId)
               .Select(n => _mapper.Map(n))
               .ToList().ForEach(n => _promotionDiscountRepository.Save(n, true));
       }

       private void SaveCertainValueCertainProductDiscount(ResponseMasterDataInfo responseMasterDataInfo)
       {
           if (responseMasterDataInfo.DeletedItems.Any())
           {
               foreach (var deletedItem in responseMasterDataInfo.DeletedItems)
               {
                   var cvcpDiscount = _certainValueCertainProductDiscountRepository.GetById(deletedItem, true);
                   if (cvcpDiscount != null)
                   {
                       try
                       {
                           _certainValueCertainProductDiscountRepository.SetAsDeleted(cvcpDiscount);
                       }
                       catch (Exception ex)
                       {
                           _log.Error("Deleting  error-" + ex.ToString());

                       }
                       
                   }

               }

           }
           responseMasterDataInfo.MasterData.MasterDataItems.OfType<CertainValueCertainProductDiscountDTO>()
               .OrderBy(n => n.StatusId)
               .Select(n => _mapper.Map(n))
               .ToList().ForEach(n => _certainValueCertainProductDiscountRepository.Save(n, true));
       }
        [Obsolete("Consider changing")]
       private void SaveProductGroupDiscount(ResponseMasterDataInfo responseMasterDataInfo)
       {
           responseMasterDataInfo.MasterData.MasterDataItems
                   .OfType<ProductGroupDiscountDTO>()
                   .OrderBy(n => n.StatusId)
                   .Select(n => _mapper.Map(n))
                   .ToList().ForEach(n => _productGroupDiscountRepository.Save(n, true));
       }
       private void SaveProductGroupDiscountDirect(ResponseMasterDataInfo responseMasterDataInfo)
       {
          var pDiscounts=responseMasterDataInfo.MasterData.MasterDataItems
               .OfType<ProductGroupDiscountDTO>()
               .OrderBy(n => n.StatusId).ToArray();
          foreach (var entity in pDiscounts)
          {
              var discountGroup = _cokeDataContext.tblDiscountGroup.FirstOrDefault(p => p.id == entity.DiscountGroupMasterId);
              var product  = _cokeDataContext.tblProduct.FirstOrDefault(n => n.id == entity.ProductMasterId);

              if (product != null && discountGroup != null)
              {

                  tblProductDiscountGroup tblPDG = _cokeDataContext.tblProductDiscountGroup.FirstOrDefault(n => n.id == entity.MasterId);
         
                  if (tblPDG == null)
                  {
                      tblPDG = new tblProductDiscountGroup
                                   {
                                       IM_Status = (int) EntityStatus.Active,
                                       IM_DateCreated = entity.DateCreated,
                                       id = entity.MasterId
                                   };
                      _cokeDataContext.tblProductDiscountGroup.AddObject(tblPDG);
                  }
                  var entityStatus = (entity.StatusId == (int) EntityStatus.New)
                                         ? EntityStatus.Active
                                         : (EntityStatus) entity.StatusId;
                  if (tblPDG.IM_Status != (int) entityStatus)
                      tblPDG.IM_Status = entity.StatusId;
                  tblPDG.DiscountRate = entity.DiscountRate;
                  tblPDG.EndDate = entity.EndDate;
                  tblPDG.EffectiveDate = entity.EffectiveDate;
                  tblPDG.Quantity = entity.Quantity;
                  tblPDG.ProductRef = product.id;

                  var dt = DateTime.Now;

                
               
                  tblPDG.IM_DateLastUpdated = entity.DateLastUpdated;
                  tblPDG.DiscountGroup = entity.DiscountGroupMasterId;
              }

          }
          _cokeDataContext.SaveChanges();
       }

       private void SaveProductDiscount(ResponseMasterDataInfo responseMasterDataInfo)
       {
           if (responseMasterDataInfo.DeletedItems.Any())
           {
               foreach (var deletedItem in responseMasterDataInfo.DeletedItems)
               {
                   var productDiscount = _productDiscountRepository.GetById(deletedItem, true);
                   if (productDiscount != null)
                   {
                       try
                       {
                           _productDiscountRepository.SetAsDeleted(productDiscount);
                       }
                       catch (Exception ex)
                       {
                           _log.Error("Deleting  error-" + ex.ToString());

                       }
                       
                   }

              } 

           }
          
           responseMasterDataInfo.MasterData.MasterDataItems
               .OfType<ProductDiscountDTO>()
               .OrderBy(n => n.StatusId)
               .Select(n => _mapper.Map(n))
               .ToList().ForEach(n => _productDiscountRepository.Save(n, true));
           var discount = responseMasterDataInfo.MasterData.MasterDataItems.OfType<ProductDiscountDTO>().SelectMany(s=>s.DeletedProductDiscountItem).ToList();
           foreach (var id in discount)
           {
               try
               {
                   _productDiscountRepository.DeactivateLineItem(id);
               }
               catch (Exception ex)
               {
                   _log.Error("Deleting Product Discount LineItem error-" + ex.ToString());

               }
              
           }
       }

       private void SaveTargetPeriod(ResponseMasterDataInfo responseMasterDataInfo)
       {
           if (responseMasterDataInfo.DeletedItems.Any())
           {
               foreach (var deletedItem in responseMasterDataInfo.DeletedItems)
               {
                   var targetPeriod = _targetPeriodRepository.GetById(deletedItem, true);
                   if (targetPeriod != null)
                   {
                       try
                       {
                           _targetPeriodRepository.SetAsDeleted(targetPeriod);
                       }
                       catch (Exception ex)
                       {
                           _log.Error("Deleting Target Period error-" + ex.ToString());

                       }
                       
                   }

               }

           }
           responseMasterDataInfo.MasterData.MasterDataItems
               .OfType<TargetPeriodDTO>()
               .OrderBy(n => n.StatusId)
               .Select(n => _mapper.Map(n))
               .ToList().ForEach(n => _targetPeriodRepository.Save(n, true));
       }

       private void SaveTarget(ResponseMasterDataInfo responseMasterDataInfo)
       {
           responseMasterDataInfo.MasterData.MasterDataItems
               .OfType<TargetDTO>()
               .OrderBy(n => n.StatusId)
               .Select(n => _mapper.Map(n))
               .ToList().ForEach(n => _targetRepository.Save(n, true));
       }

       private void SaveProvince(ResponseMasterDataInfo responseMasterDataInfo)
       {
           if (responseMasterDataInfo.DeletedItems.Any())
           {
               foreach (var deletedItem in responseMasterDataInfo.DeletedItems)
               {
                   var province = _provinceRepository.GetById(deletedItem, true);
                   if (province != null)
                   {
                       try
                       {
                           _provinceRepository.SetAsDeleted(province);
                       }
                       catch (Exception ex)
                       {
                           _log.Error("Deleting Province error-" + ex.ToString());

                       }
                      
                   }

               }

           }
           responseMasterDataInfo.MasterData.MasterDataItems
               .OfType<ProvinceDTO>()
               .OrderBy(n => n.StatusId)
               .Select(n => _mapper.Map(n))
               .ToList().ForEach(n => _provinceRepository.Save(n, true));
       }

       private void SaveDistrict(ResponseMasterDataInfo responseMasterDataInfo)
       {
           if (responseMasterDataInfo.DeletedItems.Any())
           {
               foreach (var deletedItem in responseMasterDataInfo.DeletedItems)
               {
                   var district = _districtRepository.GetById(deletedItem, true);
                   if (district != null)
                   {
                       try
                       {
                           _districtRepository.SetAsDeleted(district);
                       }
                       catch (Exception ex)
                       {
                           _log.Error("Deleting District error-" + ex.ToString());

                       }
                      
                   }

               }

           }
           responseMasterDataInfo.MasterData.MasterDataItems
               .OfType<DistrictDTO>()
               .OrderBy(n => n.StatusId)
               .Select(n => _mapper.Map(n))
               .ToList().ForEach(n => _districtRepository.Save(n, true));
       }

       private void SaveReOrderLevel(ResponseMasterDataInfo responseMasterDataInfo)
       {
           if (responseMasterDataInfo.DeletedItems.Any())
           {
               foreach (var deletedItem in responseMasterDataInfo.DeletedItems)
               {
                   var reOderLevel = _reOrderLevelRepository.GetById(deletedItem, true);
                   if (reOderLevel != null)
                   {
                       try
                       {
                           _reOrderLevelRepository.SetAsDeleted(reOderLevel);
                       }
                       catch (Exception ex)
                       {
                           _log.Error("Deleting Reorder Level error-" + ex.ToString());

                       }
                      
                   }

               }

           }
           responseMasterDataInfo.MasterData.MasterDataItems
               .OfType<ReorderLevelDTO>()
               .OrderBy(n => n.StatusId)
               .Select(n => _mapper.Map(n))
               .ToList().ForEach(n => _reOrderLevelRepository.Save(n, true));
       }

       private void SaveDiscountgroup(ResponseMasterDataInfo responseMasterDataInfo)
       {
           responseMasterDataInfo.MasterData.MasterDataItems
               .OfType<DiscountGroupDTO>()
               .OrderBy(n => n.StatusId)
               .Select(n => _mapper.Map(n))
               .ToList().ForEach(n => _discountGroupRepository.Save(n, true));
       }

       private void SaveBank(ResponseMasterDataInfo responseMasterDataInfo)
       {
           if (responseMasterDataInfo.DeletedItems.Any())
           {
               foreach (var deletedItem in responseMasterDataInfo.DeletedItems)
               {
                   var bank = _bankRepository.GetById(deletedItem, true);
                   if (bank != null)
                   {
                       try
                       {
                           _bankRepository.SetAsDeleted(bank);
                       }
                       catch (Exception ex)
                       {
                           _log.Error("Deleting Bank error-" + ex.ToString());

                       }
                       
                   }

               }

           }
           responseMasterDataInfo.MasterData.MasterDataItems
               .OfType<BankDTO>()
               .OrderBy(n => n.StatusId)
               .Select(n => _mapper.Map(n))
               .ToList().ForEach(n => _bankRepository.Save(n, true));
       }

       private void SaveBankBranch(ResponseMasterDataInfo responseMasterDataInfo)
       {
           if (responseMasterDataInfo.DeletedItems.Any())
           {
               foreach (var deletedItem in responseMasterDataInfo.DeletedItems)
               {
                   var bankBranch = _bankBranchRepository.GetById(deletedItem, true);
                   if (bankBranch != null)
                   {
                       try
                       {
                           _bankBranchRepository.SetAsDeleted(bankBranch);
                       }
                       catch (Exception ex)
                       {
                           _log.Error("Deleting Bank Branch error-" + ex.ToString());

                       }
                       
                   }

               }

           }
           responseMasterDataInfo.MasterData.MasterDataItems
               .OfType<BankBranchDTO>()
               .OrderBy(n => n.StatusId)
               .Select(n => _mapper.Map(n))
               .ToList().ForEach(n => _bankBranchRepository.Save(n, true));
       }

       private void SaveSupplier(ResponseMasterDataInfo responseMasterDataInfo)
       {
           if (responseMasterDataInfo.DeletedItems.Any())
           {
               foreach (var deletedItem in responseMasterDataInfo.DeletedItems)
               {
                   var supplier = _supplierRepository.GetById(deletedItem, true);
                   if (supplier != null)
                   {
                       try
                       {
                           _supplierRepository.SetAsDeleted(supplier);
                       }
                       catch (Exception ex)
                       {
                           _log.Error("Deleting Supplier error-" + ex.ToString());

                       }
                      
                   }

               }

           }
           responseMasterDataInfo.MasterData.MasterDataItems
               .OfType<SupplierDTO>()
               .OrderBy(n => n.StatusId)
               .Select(n => _mapper.Map(n))
               .ToList().ForEach(n => _supplierRepository.Save(n, true));
       }

       private void SaveCommodityType(ResponseMasterDataInfo responseMasterDataInfo)
       {
           if (responseMasterDataInfo.DeletedItems.Any())
           {
               foreach (var deletedItem in responseMasterDataInfo.DeletedItems)
               {
                   var commodityType = _commodityTypeRepository.GetById(deletedItem, true);
                   if (commodityType != null)
                   {
                       try
                       {
                           _commodityTypeRepository.SetAsDeleted(commodityType);
                       }
                       catch (Exception ex)
                       {
                           _log.Error("Deleting Commodity Type error-" + ex.ToString());

                       }
                       
                   }

               }

           }
           responseMasterDataInfo.MasterData.MasterDataItems
               .OfType<CommodityTypeDTO>()
               .OrderBy(n => n.StatusId)
               .Select(n => _mapper.Map(n))
               .ToList().ForEach(n => _commodityTypeRepository.Save(n, true));
       }

       private void SaveCommodity(ResponseMasterDataInfo responseMasterDataInfo)
       {
           if (responseMasterDataInfo.DeletedItems.Any())
           {
               foreach (var deletedItem in responseMasterDataInfo.DeletedItems)
               {
                   var commodity = _commodityRepository.GetById(deletedItem, true);
                   if (commodity != null)
                   {
                       try
                       {
                           _commodityRepository.SetAsDeleted(commodity);
                       }
                       catch (Exception ex)
                       {
                           _log.Error("Deleting Commodity error-" + ex.ToString());

                       }
                       
                   }

               }

           }
           responseMasterDataInfo.MasterData.MasterDataItems
               .OfType<CommodityDTO>()
               .OrderBy(n => n.StatusId)
               .Select(n => _mapper.Map(n))
               .ToList().ForEach(n => _commodityRepository.Save(n, true));

           //var grade = responseMasterDataInfo.MasterData.MasterDataItems.OfType<CommodityDTO>().SelectMany(s => s.DeletedCommodityGradesItem).ToList();
           
           var commodities = responseMasterDataInfo.MasterData.MasterDataItems.OfType<CommodityDTO>();
           foreach (var commodity in commodities )
           {
               if(commodity.DeletedCommodityGradesItem.Any())
               {
                   var commodityId = commodity.MasterId;
                   var commodityGradeId = commodity.DeletedCommodityGradesItem;
                   foreach (var id in commodityGradeId)
                   {
                       try
                       {
                           _commodityRepository.SetGradeAsDeleted(commodityId, id);
                       }
                       catch (Exception ex)
                       {
                           _log.Error("Deleting Commodity Grade error-" + ex.ToString());

                       }
                      
                   }
                  
               }
              
           }
       }

       private void SaveCommodityOwnerType(ResponseMasterDataInfo responseMasterDataInfo)
       {
           if (responseMasterDataInfo.DeletedItems.Any())
           {
               foreach (var deletedItem in responseMasterDataInfo.DeletedItems)
               {
                   var commodityOwnertype = _commodityOwnerTypeRepository.GetById(deletedItem, true);
                   if (commodityOwnertype != null)
                   {
                       try
                       {
                           _commodityOwnerTypeRepository.SetAsDeleted(commodityOwnertype);
                       }
                       catch (Exception ex)
                       {
                           _log.Error("Deleting Commodity Owner Type  error-" + ex.ToString());

                       }
                       
                   }

               }

           }
           responseMasterDataInfo.MasterData.MasterDataItems
               .OfType<CommodityOwnerTypeDTO>()
               .OrderBy(n => n.StatusId)
               .Select(n => _mapper.Map(n))
               .ToList().ForEach(n => _commodityOwnerTypeRepository.Save(n, true));
       }

       private void SaveCommodityProducer(ResponseMasterDataInfo responseMasterDataInfo)
       {
           if (responseMasterDataInfo.DeletedItems.Any())
           {
               foreach (var deletedItem in responseMasterDataInfo.DeletedItems)
               {
                   var commodityProducer = _commodityProducerRepository.GetById(deletedItem, true);
                   if (commodityProducer != null)
                   {
                       try
                       {
                           _commodityProducerRepository.SetAsDeleted(commodityProducer);
                       }
                       catch (Exception ex)
                       {
                           _log.Error("Deleting Commodity Producer  error-" + ex.ToString());

                       }
                       
                   }

               }

           }
           responseMasterDataInfo.MasterData.MasterDataItems
               .OfType<CommodityProducerDTO>()
               .OrderBy(n => n.StatusId)
               .Select(n => _mapper.Map(n))
               .ToList().ForEach(n => _commodityProducerRepository.Save(n, true));
       }

       private void SaveCommoditySupplier(ResponseMasterDataInfo responseMasterDataInfo)
       {
           if (responseMasterDataInfo.DeletedItems.Any())
           {
               foreach (var deletedItem in responseMasterDataInfo.DeletedItems)
               {
                   var commoditySupplier = _commoditySupplierRepository.GetById(deletedItem, true);
                   if (commoditySupplier != null)
                   {
                       try
                       {
                           _commoditySupplierRepository.SetAsDeleted(commoditySupplier);
                       }
                       catch (Exception ex)
                       {
                           _log.Error("Deleting Commodity Supplier error-" + ex.ToString());

                       }
                      
                   }

               }

           }
           responseMasterDataInfo.MasterData.MasterDataItems
               .OfType<CommoditySupplierDTO>()
               .OrderBy(n => n.StatusId)
               .Select(n => _mapper.Map(n))
               .ToList().ForEach(n => _commoditySupplierRepository.Save(n, true));
       }

       private void SaveCommodityOwner(ResponseMasterDataInfo responseMasterDataInfo)
       {
           if (responseMasterDataInfo.DeletedItems.Any())
           {
               foreach (var deletedItem in responseMasterDataInfo.DeletedItems)
               {
                   var commodityOwner = _commodityOwnerRepository.GetById(deletedItem, true);
                   if (commodityOwner != null)
                   {
                       try
                       {
                           _commodityOwnerRepository.SetAsDeleted(commodityOwner);
                       }
                       catch (Exception ex)
                       {
                           _log.Error("Deleting Commodity Owner error-" + ex.ToString());

                       }
                       
                   }

               }

           }
           responseMasterDataInfo.MasterData.MasterDataItems
               .OfType<CommodityOwnerDTO>()
               .OrderBy(n => n.StatusId)
               .Select(n => _mapper.Map(n))
               .ToList().ForEach(n => _commodityOwnerRepository.Save(n, true));
       }

       private void SaveCentre(ResponseMasterDataInfo responseMasterDataInfo)
       {

           if (responseMasterDataInfo.DeletedItems.Any())
           {
               foreach (var deletedItem in responseMasterDataInfo.DeletedItems)
               {
                   var centre = _centreRepository.GetById(deletedItem, true);
                   if (centre != null)
                   {
                       try
                       {
                           _centreRepository.SetAsDeleted(centre);
                       }
                       catch (Exception ex)
                       {
                           _log.Error("Deleting Centre error-" + ex.ToString());

                       }
                       
                   }

               }

           }
           responseMasterDataInfo.MasterData.MasterDataItems
               .OfType<CentreDTO>()
               .OrderBy(n => n.StatusId)
               .Select(n => _mapper.Map(n))
               .ToList().ForEach(n => _centreRepository.Save(n, true));
       }

       private void SaveCentreType(ResponseMasterDataInfo responseMasterDataInfo)
       {
           if (responseMasterDataInfo.DeletedItems.Any())
           {
               foreach (var deletedItem in responseMasterDataInfo.DeletedItems)
               {
                   var centreType = _centreTypeRepository.GetById(deletedItem, true);
                   if (centreType != null)
                   {
                       try
                       {
                           _centreTypeRepository.SetAsDeleted(centreType);
                       }
                       catch (Exception ex)
                       {
                           _log.Error("Deleting Centre Type error-" + ex.ToString());

                       }
                       
                   }

               }

           }
           responseMasterDataInfo.MasterData.MasterDataItems
               .OfType<CentreTypeDTO>()
               .OrderBy(n => n.StatusId)
               .Select(n => _mapper.Map(n))
               .ToList().ForEach(n => _centreTypeRepository.Save(n, true));
       }

       private void SaveHub(ResponseMasterDataInfo responseMasterDataInfo)
       {
           if (responseMasterDataInfo.DeletedItems.Any())
           {
               foreach (var deletedItem in responseMasterDataInfo.DeletedItems)
               {
                   var hub = _hubRepository.GetById(deletedItem, true);
                   if (hub != null)
                   {
                       try
                       {
                           _hubRepository.SetAsDeleted(hub);
                       }
                       catch (Exception ex)
                       {
                           _log.Error("Deleting Hub error-" + ex.ToString());

                       }
                       
                   }

               }

           }
           responseMasterDataInfo.MasterData.MasterDataItems
               .OfType<HubDTO>()
               .OrderBy(n => n.StatusId)
               .Select(n => _mapper.Map(n))
               .ToList().ForEach(n => _hubRepository.Save(n, true));
       }

       private void SaveStore(ResponseMasterDataInfo responseMasterDataInfo)
       {
           if (responseMasterDataInfo.DeletedItems.Any())
           {
               foreach (var deletedItem in responseMasterDataInfo.DeletedItems)
               {
                   var store = _storeRepository.GetById(deletedItem, true);
                   if (store != null)
                   {
                       try
                       {
                           _storeRepository.SetAsDeleted(store);
                       }
                       catch (Exception ex)
                       {
                           _log.Error("Deleting Store  error-" + ex.ToString());

                       }
                       
                   }

               }

           }
           responseMasterDataInfo.MasterData.MasterDataItems
               .OfType<StoreDTO>()
               .OrderBy(n => n.StatusId)
               .Select(n => _mapper.Map(n))
               .ToList().ForEach(n => _storeRepository.Save(n, true));
       }

       private void SavePurchasingClerk(ResponseMasterDataInfo responseMasterDataInfo)
       {
           responseMasterDataInfo.MasterData.MasterDataItems
               .OfType<PurchasingClerkDTO>()
               .OrderBy(n => n.StatusId)
               .Select(n => _mapper.Map(n))
               .ToList().ForEach(n => _purchasingClerkRepository.Save(n, true));
       }

       private void SaveContainerType(ResponseMasterDataInfo responseMasterDataInfo)
       {
           if (responseMasterDataInfo.DeletedItems.Any())
           {
               foreach (var deletedItem in responseMasterDataInfo.DeletedItems)
               {
                   var containerType = _containerTypeRepository.GetById(deletedItem, true);
                   if (containerType != null)
                   {
                       try
                       {
                           _containerTypeRepository.SetAsDeleted(containerType);
                       }
                       catch (Exception ex)
                       {
                           _log.Error("Deleting Container Type error-" + ex.ToString());

                       }
                       
                   }

               }

           }
           responseMasterDataInfo.MasterData.MasterDataItems
               .OfType<ContainerTypeDTO>()
               .OrderBy(n => n.StatusId)
               .Select(n => _mapper.Map(n))
               .ToList().ForEach(n => _containerTypeRepository.Save(n, true));
       }
       
       private void SavePrinter(ResponseMasterDataInfo responseMasterDataInfo)
       {
           if (responseMasterDataInfo.DeletedItems.Any())
           {
               foreach (var deletedItem in responseMasterDataInfo.DeletedItems)
               {
                   var printer = _equipmentRepository.GetById(deletedItem, true);
                   if (printer != null)
                   {
                       try
                       {
                           _equipmentRepository.SetAsDeleted(printer);
                       }
                       catch (Exception ex)
                       {
                           _log.Error("Deleting Printer error-" + ex.ToString());

                       }
                      
                   }

               }

           }
           responseMasterDataInfo.MasterData.MasterDataItems
               .OfType<PrinterDTO>()
               .OrderBy(n => n.StatusId)
               .Select(n => _mapper.Map(n))
               .ToList().ForEach(n => _equipmentRepository.Save(n, true));
       }

       private void SaveWeighScale(ResponseMasterDataInfo responseMasterDataInfo)
       {
           if (responseMasterDataInfo.DeletedItems.Any())
           {
               foreach (var deletedItem in responseMasterDataInfo.DeletedItems)
               {
                   var weighScale = _equipmentRepository.GetById(deletedItem, true);
                   if (weighScale != null)
                   {
                       try
                       {
                           _equipmentRepository.SetAsDeleted(weighScale);
                       }
                       catch (Exception ex)
                       {
                           _log.Error("Deleting Weigh Scale error-" + ex.ToString());

                       }
                       
                   }

               }

           }
           responseMasterDataInfo.MasterData.MasterDataItems
               .OfType<WeighScaleDTO>()
               .OrderBy(n => n.StatusId)
               .Select(n => _mapper.Map(n))
               .ToList().ForEach(n => _equipmentRepository.Save(n, true));
       }

       private void SaveVehicle(ResponseMasterDataInfo responseMasterDataInfo)
       {
           if (responseMasterDataInfo.DeletedItems.Any())
           {
               foreach (var deletedItem in responseMasterDataInfo.DeletedItems)
               {
                   var vehicle = _vehicleRepository.GetById(deletedItem, true);
                   if (vehicle != null)
                   {
                       try
                       {
                           _vehicleRepository.SetAsDeleted(vehicle);
                       }
                       catch (Exception ex)
                       {
                           _log.Error("Deleting Vehicle error-" + ex.ToString());

                       }
                       
                   }

               }

           }
           responseMasterDataInfo.MasterData.MasterDataItems
               .OfType<VehicleDTO>()
               .OrderBy(n => n.StatusId)
               .Select(n => _mapper.Map(n))
               .ToList().ForEach(n => _vehicleRepository.Save(n, true));
       }

       private void SaveSourcingContainer(ResponseMasterDataInfo responseMasterDataInfo)
       {
           if (responseMasterDataInfo.DeletedItems.Any())
           {
               foreach (var deletedItem in responseMasterDataInfo.DeletedItems)
               {
                   var sourcingContainer = _equipmentRepository.GetById(deletedItem, true);
                   if (sourcingContainer != null)
                   {
                       try
                       {
                           _equipmentRepository.SetAsDeleted(sourcingContainer);
                       }
                       catch (Exception ex)
                       {
                           _log.Error("Deleting Sourcing Container error-" + ex.ToString());

                       }
                       
                   }

               }

           }
           responseMasterDataInfo.MasterData.MasterDataItems
               .OfType<SourcingContainerDTO>()
               .OrderBy(n => n.StatusId)
               .Select(n => _mapper.Map(n))
               .ToList().ForEach(n => _equipmentRepository.Save(n, true));
       }
        
    }
}
