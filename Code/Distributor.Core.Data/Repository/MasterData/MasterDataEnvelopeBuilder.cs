using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CentreEntity;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.DistributorTargetEntities;
using Distributr.Core.Domain.Master.EquipmentEntities;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.MasterDataDTO;
using Distributr.Core.Repository.Financials;
using Distributr.Core.Repository.InventoryRepository;
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
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Master.ReOrderLevelRepository;
using Distributr.Core.Repository.Master.SettingsRepositories;
using Distributr.Core.Repository.Master.SuppliersRepositories;
using Distributr.Core.Repository.Master.UserRepositories;

namespace Distributr.Core.Data.Repository.MasterData
{
    public class MasterDataEnvelopeBuilder : IMasterDataEnvelopeBuilder
    {
        protected IProductTypeRepository _productTypeRepository;
        protected IProductBrandRepository _productBrandRepository;
        protected IProductFlavourRepository _productFlavourRepository;
        protected IProductPackagingRepository _productPackagingRepository;
        protected IProductPackagingTypeRepository _productPackagingTypeRepository;
        protected IProductRepository _productRepository;
        protected IRegionRepository _regionRepository;
        protected ICostCentreRepository _costCentreRepository;
        protected ICostCentreApplicationRepository _costCentreApplicationRepository;
        protected IProductPricingRepository _pricingRepository;
        protected IVATClassRepository _vatClassRepository;
        protected ICountryRepository _countryRepository;
        protected IProductPricingTierRepository _ProductPricingTierRepository;
        protected IOutletTypeRepository _outletTypeRepository;
        protected IUserRepository _userRepository;
        protected IOutletRepository _outletRepository;
        protected IRouteRepository _routeRepository;
        protected ITransporterRepository _transporterRepository;
        protected IDistributorSalesmanRepository _distributorSalesmanRepository;
        protected IProducerRepository _producerRepository;
        protected ISocioEconomicStatusRepository _socioEconomicStatusRepository;
        protected IDistributorRepository _distributorrepository;
        protected IOutletCategoryRepository _outletCategoryRepository;//
        protected ITerritoryRepository _territoryRepository;
        protected IAreaRepository _areaRepository;
        protected IContactRepository _contactRepository;
        protected IDistributorPendingDispatchWarehouseRepository _distributorPendingDispatchWarehouseRepository;
        protected IChannelPackagingRepository _channelPackagingRepository;
        protected ICompetitorRepository _competitorRepository;
        protected ICompetitorProductsRepository _competitorProductsRepository;
        protected IAssetRepository _coolerRepository;
        protected IAssetTypeRepository _coolerTypeRepository;
        protected IDistrictRepository _districtRepository;
        protected IProvincesRepository _provinceRepository;
        protected IReOrderLevelRepository _reorderLevelRepository;
        protected ITargetPeriodRepository _targetPeriodRepository;
        protected ITargetRepository _targetRepository;
        protected IProductDiscountRepository _productDiscountRepository;
        protected ISaleValueDiscountRepository _saleValueDiscountRepository;
        protected IDiscountGroupRepository _discountGroupRepository;
        protected IPromotionDiscountRepository _promotionDiscountRepository;
        protected ICertainValueCertainProductDiscountRepository _certainValueCertainProductDiscountRepository;
        protected IProductDiscountGroupRepository _productDiscountGroupRepository;
        protected IFreeOfChargeDiscountRepository _freeOfChargeDiscountRepository;
        protected ISalesmanRouteRepository _salesmanRouteRepository;
        protected IContainmentRepository _containmentRepository;
        protected IUserGroupRepository _userGroupRepository;
        protected IUserGroupRolesRepository _userGroupRolesRepository;
        protected IBankRepository _bankRepository;
        protected IBankBranchRepository _bankBranchRepository;
        protected ISupplierRepository _supplierRepository;
        protected IContactTypeRepository _contactTypeRepository;
        protected IAssetCategoryRepository _assetCategoryRepository;
        protected IAssetStatusRepository _assetStatusRepository;
        protected IOutletVisitDayRepository _outletVisitDayRepository;
        protected IOutletPriorityRepository _outletPriorityRepository;
        protected ITargetItemRepository _targetItemRepository;
        protected ISettingsRepository _settingsRepository;
        protected IInventoryRepository _inventoryRepository;
        protected IPaymentTrackerRepository _paymentTrackerRepository;
        protected IRetireDocumentSettingRepository _retireDocumentSettingRepository;
        protected ICommodityTypeRepository _commodityTypeRepository;
        protected ICommodityRepository _commodityRepository;
        protected ICommodityOwnerTypeRepository _commodityOwnerTypeRepository;
        protected ICommodityProducerRepository _commodityProducerRepository;
        protected ICommoditySupplierRepository _commoditySupplierRepository;
        protected ICommodityOwnerRepository _commodityOwnerRepository;
        protected ICentreRepository _centreRepository;
        protected IHubRepository _hubRepository;
        protected IPurchasingClerkRepository _purchasingClerkRepository;
        protected IPurchasingClerkRouteRepository _purchasingClerkRouteRepository;
        protected ICentreTypeRepository _centreTypeRepository;
        protected IEquipmentRepository _equipmentRepository;
        protected IStoreRepository _storeRepository;
        protected IContainerTypeRepository _containerTypeRepository;
        protected IMasterDataAllocationRepository _masterDataAllocatioRepository;
        protected IVehicleRepository _vehicleRepository;

        public MasterDataEnvelopeBuilder(IProductTypeRepository productTypeRepository, IProductBrandRepository productBrandRepository, IProductFlavourRepository productFlavourRepository,
            IProductPackagingRepository productPackagingRepository, IProductPackagingTypeRepository productPackagingTypeRepository, IProductRepository productRepository, 
            IRegionRepository regionRepository, ICostCentreRepository costCentreRepository, ICostCentreApplicationRepository costCentreApplicationRepository, 
            IProductPricingRepository pricingRepository, IVATClassRepository vatClassRepository, IProductPricingTierRepository productPricingTierRepository, 
            ICountryRepository countryRepository, IOutletTypeRepository outletTypeRepository, IUserRepository userRepository, IOutletRepository outletRepository, 
            IRouteRepository routeRepository, ITransporterRepository transporterRepository, IDistributorSalesmanRepository distributorSalesmanRepository, 
            IProducerRepository producerRepository, IDistributorRepository distributorrepository, ISocioEconomicStatusRepository socioEconomicStatusRepository, 
            IOutletCategoryRepository outletCategoryRepository, ITerritoryRepository territoryRepository, IAreaRepository areaRepository, IContactRepository contactRepository, 
            IDistributorPendingDispatchWarehouseRepository distributorPendingDispatchWarehouseRepository, IChannelPackagingRepository channelPackagingRepository, 
            ICompetitorRepository competitorRepository, ICompetitorProductsRepository competitorProductsRepository, IAssetRepository coolerRepository, 
            IAssetTypeRepository coolerTypeRepository, IDistrictRepository districtRepository, IProvincesRepository provinceRepository, IReOrderLevelRepository reorderLevelRepository, 
            ITargetPeriodRepository targetPeriodRepository, ITargetRepository targetRepository, IProductDiscountRepository productDiscountRepository, 
            ISaleValueDiscountRepository saleValueDiscountRepository, IDiscountGroupRepository discountGroupRepository, IPromotionDiscountRepository promotionDiscountRepository, 
            ICertainValueCertainProductDiscountRepository certainValueCertainProductDiscountRepository, IFreeOfChargeDiscountRepository freeOfChargeDiscountRepository, 
            IProductDiscountGroupRepository productDiscountGroupRepository, ISalesmanRouteRepository salesmanRouteRepository, IContainmentRepository containmentRepository, 
            IUserGroupRepository userGroupRepository, IUserGroupRolesRepository userGroupRolesRepository, IBankRepository bankRepository, IBankBranchRepository bankBranchRepository, 
            ISupplierRepository supplierRepository, IContactTypeRepository contactTypeRepository, 
            IAssetCategoryRepository assetCategoryRepository, IAssetStatusRepository assetStatusRepository, IOutletVisitDayRepository outletVisitDayRepository, 
            IOutletPriorityRepository outletPriorityRepository, ITargetItemRepository targetItemRepository, ISettingsRepository settingsRepository, 
            IInventoryRepository inventoryRepository, IPaymentTrackerRepository paymentTrackerRepository, IRetireDocumentSettingRepository retireDocumentSettingRepository, 
            ICommodityTypeRepository commodityTypeRepository, ICommodityRepository commodityRepository, ICommodityOwnerTypeRepository commodityOwnerTypeRepository, 
            ICommodityProducerRepository commodityProducerRepository, ICommoditySupplierRepository commoditySupplierRepository, ICommodityOwnerRepository commodityOwnerRepository, 
            ICentreRepository centreRepository, IHubRepository hubRepository, IPurchasingClerkRepository purchasingClerkRepository, 
            IPurchasingClerkRouteRepository purchasingClerkRouteRepository, ICentreTypeRepository centreTypeRepository, IEquipmentRepository equipmentRepository, 
            IStoreRepository storeRepository, IMasterDataAllocationRepository masterDataAllocationRepository, IContainerTypeRepository containerTypeRepository, IVehicleRepository vehicleRepository)
        {
            _productTypeRepository = productTypeRepository;
            _productBrandRepository = productBrandRepository;
            _productFlavourRepository = productFlavourRepository;
            _productPackagingRepository = productPackagingRepository;
            _productPackagingTypeRepository = productPackagingTypeRepository;
            _productRepository = productRepository;
            _regionRepository = regionRepository;
            _costCentreRepository = costCentreRepository;
            _costCentreApplicationRepository = costCentreApplicationRepository;
            _pricingRepository = pricingRepository;
            _vatClassRepository = vatClassRepository;
            _ProductPricingTierRepository = productPricingTierRepository;
            _countryRepository = countryRepository;
            _outletTypeRepository = outletTypeRepository;
            _userRepository = userRepository;
            _outletRepository = outletRepository;
            _routeRepository = routeRepository;
            _transporterRepository = transporterRepository;
            _distributorSalesmanRepository = distributorSalesmanRepository;
            _producerRepository = producerRepository;
            _distributorrepository = distributorrepository;
            _socioEconomicStatusRepository = socioEconomicStatusRepository;
            _outletCategoryRepository = outletCategoryRepository;
            _territoryRepository = territoryRepository;
            _areaRepository = areaRepository;
            _contactRepository = contactRepository;
            _distributorPendingDispatchWarehouseRepository = distributorPendingDispatchWarehouseRepository;
            _channelPackagingRepository = channelPackagingRepository;
            _competitorRepository = competitorRepository;
            _competitorProductsRepository = competitorProductsRepository;
            _coolerRepository = coolerRepository;
            _coolerTypeRepository = coolerTypeRepository;
            _districtRepository = districtRepository;
            _provinceRepository = provinceRepository;
            _reorderLevelRepository = reorderLevelRepository;
            _targetPeriodRepository = targetPeriodRepository;
            _targetRepository = targetRepository;
            _productDiscountRepository = productDiscountRepository;
            _saleValueDiscountRepository = saleValueDiscountRepository;
            _discountGroupRepository = discountGroupRepository;
            _promotionDiscountRepository = promotionDiscountRepository;
            _certainValueCertainProductDiscountRepository = certainValueCertainProductDiscountRepository;
            _freeOfChargeDiscountRepository = freeOfChargeDiscountRepository;
            _productDiscountGroupRepository = productDiscountGroupRepository;
            _salesmanRouteRepository = salesmanRouteRepository;
            _containmentRepository = containmentRepository;
            _userGroupRepository = userGroupRepository;
            _userGroupRolesRepository = userGroupRolesRepository;
            _bankRepository = bankRepository;
            _bankBranchRepository = bankBranchRepository;
            _supplierRepository = supplierRepository;
            _contactTypeRepository = contactTypeRepository;
            _assetCategoryRepository = assetCategoryRepository;
            _assetStatusRepository = assetStatusRepository;
            _outletVisitDayRepository = outletVisitDayRepository;
            _outletPriorityRepository = outletPriorityRepository;
            _targetItemRepository = targetItemRepository;
            _settingsRepository = settingsRepository;
            _inventoryRepository = inventoryRepository;
            _paymentTrackerRepository = paymentTrackerRepository;
            _retireDocumentSettingRepository = retireDocumentSettingRepository;
            _commodityTypeRepository = commodityTypeRepository;
            _commodityRepository = commodityRepository;
            _commodityOwnerTypeRepository = commodityOwnerTypeRepository;
            _commodityProducerRepository = commodityProducerRepository;
            _commoditySupplierRepository = commoditySupplierRepository;
            _commodityOwnerRepository = commodityOwnerRepository;
            _centreRepository = centreRepository;
            _hubRepository = hubRepository;
            _purchasingClerkRepository = purchasingClerkRepository;
            _purchasingClerkRouteRepository = purchasingClerkRouteRepository;
            _centreTypeRepository = centreTypeRepository;
            _equipmentRepository = equipmentRepository;
            _storeRepository = storeRepository;
            _masterDataAllocatioRepository = masterDataAllocationRepository;
            _containerTypeRepository = containerTypeRepository;
            _vehicleRepository = vehicleRepository;
        }


        public MasterDataEnvelope BuildOutletCategory(DateTime dateSince, MasterDataCollective masterdata, CostCentre cc)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_outletCategoryRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                envelope.masterData = _outletCategoryRepository.GetItemUpdated(dateSince).Select(n => n as MasterEntity).ToList();
            }
            return envelope;
        }

        public MasterDataEnvelope BuildOutlet(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            bool isAssignedoutlet = _salesmanRouteRepository.GetItemUpdatedSinceDateTime(dateSince);
            if (_outletRepository.GetItemUpdatedSinceDateTime(dateSince) || isAssignedoutlet)
            {
                List<Outlet> itier = _outletRepository.GetItemUpdated(dateSince).OfType<Outlet>().ToList();
                if (cct != null)
                {
                    switch (cct.CostCentreType)
                    {
                        case CostCentreType.Distributor:
                        case CostCentreType.Hub:
                            itier = itier.Where(o => o.ParentCostCentre.Id == cct.Id).ToList();//cn: above code filters n returns outlets for routes assigned to salesmen only. For routes not assigned to salesmen their outltets r not returned.
                            break;
                        case CostCentreType.DistributorSalesman:
                            {
                                var salesmanroutes = _salesmanRouteRepository.GetAll(true).ToList();
                                salesmanroutes = salesmanroutes.Where(n => n.DistributorSalesmanRef.Id == cct.Id).ToList();
                                itier = itier.Where(n => salesmanroutes.Select(r => r.Route.Id).Contains(n.Route.Id)).ToList();
                                if (isAssignedoutlet)
                                {
                                    var routeIds = _salesmanRouteRepository.GetItemUpdated(dateSince)
                                        .Where(s => s.DistributorSalesmanRef.Id == cct.Id)
                                        .Select(n => n.Route.Id);
                                    var outlets = _outletRepository.GetAll(true).OfType<Outlet>().Where(o => routeIds.Contains(o.Id))
                                     .ToList();
                                    itier = itier.Union(outlets).Distinct().ToList();
                                }
                                break;
                            }
                    }
                }
                envelope.masterData = itier.Select(n => n as MasterEntity).ToList();// _outletRepository.GetAll().Select(n => n as MasterEntity).ToList()
            }
            return envelope;
        }

        public MasterDataEnvelope BuildOutletType(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope {masterDataName = masterdata.ToString()};
            if (_outletTypeRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                envelope.masterData = _outletTypeRepository.GetItemUpdated(dateSince).Select(n => n as MasterEntity).ToList();
            }
            return envelope;
        }

        public MasterDataEnvelope BuildDistributorPendingDispatchWarehouse(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_distributorPendingDispatchWarehouseRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                var entity = _distributorPendingDispatchWarehouseRepository.GetItemUpdated(dateSince);
                if (cct != null)
                    switch (cct.CostCentreType)
                    {
                        case CostCentreType.Distributor:
                        case CostCentreType.Hub:
                            entity = entity.Where(n => n.ParentCostCentre.Id == cct.Id);
                            break;
                        case CostCentreType.DistributorSalesman:
                            entity = entity.Where(n => n.ParentCostCentre.Id == cct.ParentCostCentre.Id);
                            break;
                    }
                envelope.masterData = entity.Select(n => n as MasterEntity).ToList();
            }
            return envelope;
        }

        public MasterDataEnvelope BuildPricingTier(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_ProductPricingTierRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                envelope.masterData =
                    _ProductPricingTierRepository.GetItemUpdated(dateSince).Select(n => n as MasterEntity).ToList();
            }
            return envelope;
        }

        public MasterDataEnvelope BuildSaleProduct(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_productRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                envelope.masterData = _productRepository.GetItemUpdated(dateSince).OfType<SaleProduct>()
                    .Select(n => n as MasterEntity).ToList();
            }
            return envelope;
        }

        public MasterDataEnvelope BuildReturnableProduct(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_productRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                envelope.masterData = _productRepository.GetItemUpdated(dateSince).OfType<ReturnableProduct>()
                    .Select(n => n as MasterEntity).ToList();
            }
            return envelope;
        }

        public MasterDataEnvelope BuildConsolidatedProduct(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_productRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                envelope.masterData = _productRepository.GetItemUpdated(dateSince).OfType<ConsolidatedProduct>()
                    .Select(n => n as MasterEntity).ToList();
            }
            return envelope;
        }

        public MasterDataEnvelope BuildCountry(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_countryRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                envelope.masterData =
                    _countryRepository.GetItemUpdated(dateSince).Select(n => n as MasterEntity).ToList();
            }
            return envelope;
        }

        public MasterDataEnvelope BuildProductBrand(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_productBrandRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                envelope.masterData =
                    _productBrandRepository.GetItemUpdated(dateSince).Select(n => n as MasterEntity).ToList();
            }
            return envelope;
        }

        public MasterDataEnvelope BuildArea(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_areaRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                envelope.masterData = _areaRepository.GetAll(true).Select(n => n as MasterEntity).ToList();
            }
            return envelope;
        }

        public MasterDataEnvelope BuildContact(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_contactRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                var contacts = new List<Contact>();
                var ccIds = new List<Guid>();
                if (cct != null)
                    switch (cct.CostCentreType)
                    {
                        case CostCentreType.Distributor:
                        case CostCentreType.Hub:
                            var relevantCostCentreIds = _costCentreRepository.GetAll(true)
                                .Where(c => (c.ParentCostCentre != null && c.ParentCostCentre.Id == cct.Id) || c.Id == cct.Id)
                                .Select(c => c.Id);
                            var relevantdUserIds =
                                _userRepository.GetAll(true).Where(u => relevantCostCentreIds.Contains(u.CostCentre)).Select(u => u.Id);
                            var requiredContactOwnerIds = relevantCostCentreIds.Union(relevantdUserIds).ToList();
                            contacts = _contactRepository.GetItemUpdated(dateSince)
                                .Where(n => requiredContactOwnerIds.Contains(n.ContactOwnerMasterId))
                                .ToList();
                            break;
                        case CostCentreType.DistributorSalesman:
                            var distId = _costCentreRepository.GetById(cct.Id).ParentCostCentre.Id;
                            ccIds = _costCentreRepository.GetAll(true)
                                .Where(n => (n.ParentCostCentre != null && n.ParentCostCentre.Id == distId)
                                    || n.Id == distId || n.Id == cct.Id)
                                .Select(n => n.Id).ToList();
                            contacts = _contactRepository.GetItemUpdated(dateSince)
                                .Where(n => ccIds.Contains(n.ContactOwnerMasterId))
                                .ToList();
                            break;
                        case CostCentreType.PurchasingClerk:
                            var hubId = _costCentreRepository.GetById(cct.Id).ParentCostCentre.Id;
                            ccIds = _costCentreRepository.GetAll(true)
                                .Where(n => (n.ParentCostCentre != null && n.ParentCostCentre.Id == hubId)
                                    || n.Id == hubId || n.Id == cct.Id)
                                .Select(n => n.Id).ToList();
                            contacts = _contactRepository.GetItemUpdated(dateSince)
                                .Where(n => ccIds.Contains(n.ContactOwnerMasterId))
                                .ToList();
                            break;
                    }
                envelope.masterData = contacts.Select(n => n as MasterEntity).ToList();
            }
            return envelope;
        }

        public MasterDataEnvelope BuildDistributor(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_distributorrepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                envelope.masterData =
                    _distributorrepository.GetItemUpdated(dateSince).Select(n => n as MasterEntity).ToList();
            }
            return envelope;
        }

        public MasterDataEnvelope BuildPricing(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_pricingRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                envelope.masterData =
                    _pricingRepository.GetItemUpdated(dateSince).Select(n => n as MasterEntity).ToList();
            }
            return envelope;
        }

        public MasterDataEnvelope BuildProducer(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_producerRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                envelope.masterData =
                    _producerRepository.GetItemUpdated(dateSince).Select(n => n as MasterEntity).ToList();
            }
            return envelope;
        }

        public MasterDataEnvelope BuildProductFlavour(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_productFlavourRepository.GetItemUpdatedSinceDateTime(dateSince))
            {

                envelope.masterData =
                    _productFlavourRepository.GetItemUpdated(dateSince).Select(n => n as MasterEntity).ToList();
            }
            return envelope;
        }

        public MasterDataEnvelope BuildProductPackagingType(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_productPackagingTypeRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                envelope.masterData =
                    _productPackagingTypeRepository.GetItemUpdated(dateSince).Select(n => n as MasterEntity).ToList();
            }
            return envelope;
        }

        public MasterDataEnvelope BuildProductType(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_productTypeRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                envelope.masterData =
                    _productTypeRepository.GetItemUpdated(dateSince).Select(n => n as MasterEntity).ToList();
            }
            return envelope;
        }

        public MasterDataEnvelope BuildRegion(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_regionRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                envelope.masterData =
                    _regionRepository.GetItemUpdated(dateSince).Select(n => n as MasterEntity).ToList();
            }
            return envelope;
        }

        public MasterDataEnvelope BuildRoute(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            bool hasSalesmanRoutes = _salesmanRouteRepository.GetItemUpdatedSinceDateTime(dateSince);
            bool hasPurchasingClerkRoutes = false;
            bool isPurchasingClerk = cct is PurchasingClerk;
            if (isPurchasingClerk)
            {
                hasPurchasingClerkRoutes = ((PurchasingClerk) cct).PurchasingClerkRoutes.Any();
            }
            if ( _routeRepository.GetItemUpdatedSinceDateTime(dateSince) || hasSalesmanRoutes || hasPurchasingClerkRoutes)
            {
                List<Route> routes = _routeRepository.GetItemUpdated(dateSince).ToList();
                if (cct != null)
                    switch (cct.CostCentreType)
                    {
                        case CostCentreType.Distributor:
                            routes = routes.Where(n => n.Region.Id == ((Distributor)cct).Region.Id).ToList();
                            break;
                        case CostCentreType.Hub:
                            routes = routes.Where(n => n.Region.Id == ((Hub)cct).Region.Id).ToList();
                            break;
                        case CostCentreType.DistributorSalesman:
                            routes = routes.Where(o => _salesmanRouteRepository.GetAll()
                                                         .Where(s => s.DistributorSalesmanRef.Id == cct.Id)
                                                         .Select(n => n.Route.Id)
                                                         .Contains(o.Id))
                                .ToList();
                            if (hasSalesmanRoutes)
                            {
                                var routeIds = _salesmanRouteRepository.GetItemUpdated(dateSince)
                                    .Where(s => s.DistributorSalesmanRef.Id == cct.Id)
                                    .Select(n => n.Route.Id);
                                var route = _routeRepository.GetAll(true).Where(o => routeIds.Contains(o.Id)).ToList();
                                routes = routes.Union(route).Distinct().ToList();
                            }
                            break;
                        case CostCentreType.PurchasingClerk:
                            routes = routes.Where(r => 
                                _purchasingClerkRouteRepository.GetAll()
                                .Where(pcr => pcr.PurchasingClerkRef.Id == cct.Id)
                                .Select(n => n.Route.Id)
                                .Contains(r.Id))
                                .ToList();
                            if (hasPurchasingClerkRoutes)
                            {
                                var routeIds = ((PurchasingClerk) cct).PurchasingClerkRoutes.Select(n => n.Route.Id).ToList();
                                routes = _routeRepository.GetAll(true).Where(r => routeIds.Contains(r.Id)).ToList();
                            }
                            break; 
                    }

                envelope.masterData = routes.Select(n => n as MasterEntity).ToList();
            }
            return envelope;
        }

        public MasterDataEnvelope BuildSocioEconomicStatus(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_socioEconomicStatusRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                envelope.masterData = _socioEconomicStatusRepository.GetItemUpdated(dateSince).Select(n => n as MasterEntity)
                    .ToList();
            }
            return envelope;
        }

        public MasterDataEnvelope BuildTerritory(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_territoryRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                envelope.masterData = _territoryRepository.GetItemUpdated(dateSince).Select(n => n as MasterEntity)
                    .ToList();
            }
            return envelope;
        }

        public MasterDataEnvelope BuildUser(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            List<User> users = _userRepository.GetItemUpdated(dateSince).ToList();
            if (cct != null)
                switch (cct.CostCentreType)
                {
                    case CostCentreType.Distributor:
                    case CostCentreType.Hub:
                        {
                            var costcentreid = _costCentreRepository.GetAll(true)
                                .Where(n => n.ParentCostCentre != null)
                                .Where(n => n.ParentCostCentre.Id == cct.Id)
                                .Select(x => x.Id)
                                .ToList();
                            users = users.Where(n => n.CostCentre == cct.Id || costcentreid.Contains(n.CostCentre)).ToList();
                        }
                        break;
                    case CostCentreType.DistributorSalesman:
                        users = users.Where(n => n.CostCentre == cct.Id).ToList();
                        break;
                }
            envelope.masterData = users.Select(n => n as MasterEntity).ToList();
            return envelope;
        }

        public MasterDataEnvelope BuildVatClass(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_vatClassRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                envelope.masterData = _vatClassRepository.GetItemUpdated(dateSince).Select(n => n as MasterEntity)
                    .ToList();
            }
            return envelope;
        }

        public MasterDataEnvelope BuildProductPackaging(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_productPackagingRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                envelope.masterData =
                    _productPackagingRepository.GetItemUpdated(dateSince).Select(n => n as MasterEntity).ToList();
            }
            return envelope;
        }

        public MasterDataEnvelope BuildDistributorSalesman(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_distributorSalesmanRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                List<DistributorSalesman> ds = _distributorSalesmanRepository.GetItemUpdated(dateSince)
                    .OfType<DistributorSalesman>().ToList();
                if (cct != null)
                    switch (cct.CostCentreType)
                    {
                        case CostCentreType.Distributor:
                            ds = ds.Where(n => n.ParentCostCentre.Id == cct.Id).ToList();
                            break;
                        case CostCentreType.DistributorSalesman:
                            ds = ds.Where(n => n.ParentCostCentre.Id == cct.ParentCostCentre.Id).ToList();
                            break;
                    }
                envelope.masterData = ds.Select(n => n as MasterEntity).ToList();
            }
            return envelope;
        }

        public MasterDataEnvelope BuildChannelPackaging(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_channelPackagingRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                envelope.masterData = _channelPackagingRepository.GetItemUpdated(dateSince).Select(n => n as MasterEntity).ToList();
            }
            return envelope;
        }

        public MasterDataEnvelope BuildFreeOfChargeDiscount(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_freeOfChargeDiscountRepository.GetItemUpdatedSinceDateTime(dateSince))
            {

                envelope.masterData = _freeOfChargeDiscountRepository.GetItemUpdated(dateSince).Select(n => n as MasterEntity).ToList();
            }
            return envelope;
        }

        public MasterDataEnvelope BuildCompetitor(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_competitorRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                envelope.masterData = _competitorRepository.GetItemUpdated(dateSince).Select(n => n as MasterEntity).ToList();
            }
            return envelope;
        }

        public MasterDataEnvelope BuildCompetitorProduct(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_competitorProductsRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                envelope.masterData = _competitorProductsRepository.GetItemUpdated(dateSince).Select(n => n as MasterEntity).ToList();
            }
            return envelope;
        }

        public MasterDataEnvelope BuildAsset(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_coolerRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                envelope.masterData = _coolerRepository.GetItemUpdated(dateSince).Select(n => n as MasterEntity).ToList();
            }
            return envelope;
        }

        public MasterDataEnvelope BuildAssetType(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_coolerTypeRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                envelope.masterData = _coolerTypeRepository.GetItemUpdated(dateSince).Select(n => n as MasterEntity).ToList();
            }
            return envelope;
        }

        public MasterDataEnvelope BuildDistrict(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_districtRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                envelope.masterData = _districtRepository.GetItemUpdated(dateSince).Select(n => n as MasterEntity).ToList();
            }
            return envelope;
        }

        public MasterDataEnvelope BuildProvince(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_provinceRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                envelope.masterData = _provinceRepository.GetItemUpdated(dateSince).Select(n => n as MasterEntity).ToList();
            }
            return envelope;
        }

        public MasterDataEnvelope BuildReorderLevel(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_reorderLevelRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                envelope.masterData = _reorderLevelRepository.GetItemUpdated(dateSince).Select(n => n as MasterEntity).ToList();
            }
            return envelope;
        }

        public MasterDataEnvelope BuildTargetPeriod(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_targetPeriodRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                envelope.masterData = _targetPeriodRepository.GetItemUpdated(dateSince).Select(n => n as MasterEntity).ToList();
            }
            return envelope;
        }

        public MasterDataEnvelope BuildTarget(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_targetRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                var targets = new List<Target>();
                if (cct != null)
                    switch (cct.CostCentreType)
                    {
                        case CostCentreType.Distributor:
                        case CostCentreType.Hub:
                            var costCentreIds = _costCentreRepository.GetAll(true)
                                .Where(n => n.ParentCostCentre != null && n.ParentCostCentre.Id == cct.Id)
                                .Select(n => n.Id)
                                .ToList();
                            targets = _targetRepository.GetItemUpdated(dateSince)
                                .Where(n => n.CostCentre.Id == cct.Id || costCentreIds.Contains(n.CostCentre.Id))
                                .ToList();
                            break;
                        case CostCentreType.DistributorSalesman:
                            var dist = _costCentreRepository.GetById(cct.ParentCostCentre.Id) as Distributor;
                            var distCCIds = _costCentreRepository.GetAll(true)
                                .Where(n => n.ParentCostCentre != null && n.ParentCostCentre.Id == dist.Id)
                                .Select(n => n.Id).ToList();
                            targets = _targetRepository.GetItemUpdated(dateSince)
                               .Where(n => distCCIds.Contains(n.CostCentre.Id) || cct.Id == n.CostCentre.Id)
                               .ToList();
                            break;
                    }

                envelope.masterData = targets.Select(n => n as MasterEntity).ToList();
            }
            return envelope;
        }

        public MasterDataEnvelope BuildProductDiscount(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_productDiscountRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                envelope.masterData = _productDiscountRepository.GetItemUpdated(dateSince).Select(n => n as MasterEntity).ToList();
            }
            return envelope;
        }

        public MasterDataEnvelope BuildSaleValueDiscount(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_saleValueDiscountRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                envelope.masterData = _saleValueDiscountRepository.GetItemUpdated(dateSince).Select(n => n as MasterEntity).ToList();
            }
            return envelope;
        }

        public MasterDataEnvelope BuildDiscountGroup(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_discountGroupRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                envelope.masterData = _discountGroupRepository.GetItemUpdated(dateSince).Select(n => n as MasterEntity).ToList();
            }
            return envelope;
        }

        public MasterDataEnvelope BuildPromotionDiscount(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_promotionDiscountRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                envelope.masterData = _promotionDiscountRepository.GetItemUpdated(dateSince).Select(n => n as MasterEntity).ToList();
            }
            return envelope;
        }

        public MasterDataEnvelope BuildCvcpDiscount(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_certainValueCertainProductDiscountRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                envelope.masterData = _certainValueCertainProductDiscountRepository.GetItemUpdated(dateSince).Select(n => n as MasterEntity).ToList();
            }
            return envelope;
        }

        public MasterDataEnvelope BuildProductGroupDiscount(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_productDiscountGroupRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                envelope.masterData = _productDiscountGroupRepository.GetItemUpdated(dateSince).Select(n => n as MasterEntity).ToList();
            }
            return envelope;
        }

        public MasterDataEnvelope BuildSalesmanRoute(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_salesmanRouteRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                var sroutes = new List<SalesmanRoute>();
                if (cct != null)
                    switch (cct.CostCentreType)
                    {
                        case CostCentreType.Distributor:
                            sroutes =  _salesmanRouteRepository.GetItemUpdated(dateSince)
                                .Where(n => n.Route.Region.Id == ((Distributor) cct).Region.Id).ToList();
                            break;
                        case CostCentreType.DistributorSalesman:
                            sroutes =  _salesmanRouteRepository.GetItemUpdated(dateSince)
                                .Where(n => n.DistributorSalesmanRef.Id == cct.Id).ToList();
                            break;
                        case CostCentreType.PurchasingClerk:
                            sroutes = _salesmanRouteRepository.GetItemUpdated(dateSince)
                                .Where(n => n.DistributorSalesmanRef.Id == cct.Id).ToList();
                            break;
                    }

                envelope.masterData = sroutes.Select(n => n as MasterEntity).ToList();
            }
            return envelope;
        }

        public MasterDataEnvelope BuildUserGroup(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_userGroupRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                envelope.masterData = _userGroupRepository.GetItemUpdated(dateSince).Select(n => n as MasterEntity).ToList();
            }
            return envelope;
        }

        public MasterDataEnvelope BuildUserGroupRole(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_userGroupRolesRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                envelope.masterData = _userGroupRolesRepository.GetItemUpdated(dateSince).Select(n => n as MasterEntity).ToList();
            }
            return envelope;
        }

        public MasterDataEnvelope BuildBank(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_bankRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                envelope.masterData = _bankRepository.GetItemUpdated(dateSince).Select(n => n as MasterEntity).ToList();
            }
            return envelope;
        }

        public MasterDataEnvelope BuildBankBranch(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_bankBranchRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                envelope.masterData = _bankBranchRepository.GetItemUpdated(dateSince).Select(n => n as MasterEntity).ToList();
            }
            return envelope;
        }

        public MasterDataEnvelope BuildSupplier(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_supplierRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                envelope.masterData = _supplierRepository.GetItemUpdated(dateSince).Select(n => n as MasterEntity).ToList();
            }
            return envelope;
        }

       

        public MasterDataEnvelope BuildContactType(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_contactTypeRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                envelope.masterData = _contactTypeRepository.GetItemUpdated(dateSince).Select(n => n as MasterEntity).ToList();
            }
            return envelope;
        }

        public MasterDataEnvelope BuildAssetCategory(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_assetCategoryRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                envelope.masterData = _assetCategoryRepository.GetItemUpdated(dateSince).Select(n => n as MasterEntity).ToList();
            }
            return envelope;
        }

        public MasterDataEnvelope BuildAssetStatus(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_assetStatusRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                envelope.masterData = _assetStatusRepository.GetItemUpdated(dateSince).Select(n => n as MasterEntity).ToList();
            }
            return envelope;
        }

        public MasterDataEnvelope BuildOutletPriority(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_outletPriorityRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                List<OutletPriority> outletPriorityList = new List<OutletPriority>();
                List<Route> routes = new List<Route>();
                if (cct != null)
                    switch (cct.CostCentreType)
                    {
                        case CostCentreType.Distributor:
                            routes = _routeRepository.GetAll(true).Where(p => p.Region.Id == cct.Id).ToList();
                            outletPriorityList = _outletPriorityRepository.GetItemUpdated(dateSince).Where(n => routes.Select(p => p.Id).Contains(n.Route.Id)).ToList();
                            break;
                        case CostCentreType.DistributorSalesman:
                            routes = _routeRepository.GetAll(true).Where(o => _salesmanRouteRepository
                                                                                        .GetAll()
                                                                                        .Where(s => s.DistributorSalesmanRef.Id == cct.Id)
                                                                                        .Select(n => n.Route.Id)
                                                                                        .Contains(o.Id))
                                 .ToList();
                            outletPriorityList = _outletPriorityRepository.GetItemUpdated(dateSince).Where(n => routes.Select(p => p.Id).Contains(n.Route.Id)).ToList();
                            break;
                    }
                envelope.masterData = outletPriorityList.Select(n => n as MasterEntity).ToList();
            }
            return envelope;
        }

        public MasterDataEnvelope BuildOutletVisitDay(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_outletVisitDayRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                List<OutletVisitDay> outletdaysList = new List<OutletVisitDay>();
                List<Outlet> outlet = new List<Outlet>();
                if (cct != null)
                    switch (cct.CostCentreType)
                    {
                        case CostCentreType.Distributor:
                            outlet = _costCentreRepository.GetAll(false).OfType<Outlet>().Where(p => p.ParentCostCentre.Id == cct.Id).ToList();
                            outletdaysList = _outletVisitDayRepository.GetItemUpdated(dateSince).Where(n => outlet.Select(p => p.Id).Contains(n.Outlet.Id)).ToList();
                            break;
                        case CostCentreType.DistributorSalesman:
                            var salesmanroutes = _salesmanRouteRepository.GetAll(true).ToList();
                            salesmanroutes = salesmanroutes.Where(n => n.DistributorSalesmanRef.Id == cct.Id).ToList();
                            outlet = _outletRepository.GetAll().OfType<Outlet>().Where(n => salesmanroutes.Select(r => r.Route.Id).Contains(n.Route.Id)).ToList();
                            outletdaysList = _outletVisitDayRepository.GetItemUpdated(dateSince).Where(n => outlet.Select(p => p.Id).Contains(n.Outlet.Id)).ToList();
                            break;
                    }
                envelope.masterData = outletdaysList.Select(n => n as MasterEntity).ToList();
            }
            return envelope;
        }

        public MasterDataEnvelope BuildTargetItem(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_targetItemRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                List<TargetItem> targetItemList = new List<TargetItem>();
                List<Outlet> outlet = new List<Outlet>();
                if (cct != null)
                    switch (cct.CostCentreType)
                    {
                        case CostCentreType.Distributor:
                            outlet = _costCentreRepository.GetAll(false).OfType<Outlet>().Where(p => p.ParentCostCentre.Id == cct.Id).ToList();
                            targetItemList = _targetItemRepository.GetItemUpdated(dateSince).ToList();
                            targetItemList = targetItemList.Where(n => outlet.Select(p => p.Id).Contains(n.Target.CostCentre.Id)).ToList();
                            break;
                        case CostCentreType.DistributorSalesman:
                            var salesmanroutes = _salesmanRouteRepository.GetAll(true).ToList();
                            salesmanroutes = salesmanroutes.Where(n => n.DistributorSalesmanRef.Id == cct.Id).ToList();
                            outlet = _outletRepository.GetAll().OfType<Outlet>().Where(n => salesmanroutes.Select(r => r.Route.Id).Contains(n.Route.Id)).ToList();
                            targetItemList = _targetItemRepository.GetItemUpdated(dateSince).Where(n => outlet.Select(p => p.Id).Contains(n.Target.CostCentre.Id)).ToList();
                            break;
                    }
                envelope.masterData = targetItemList.Select(n => n as MasterEntity).ToList();
            }
            return envelope;
        }

        public MasterDataEnvelope BuildSetting(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_settingsRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
              
                envelope.masterData = _settingsRepository.GetAll().Select(n => n as MasterEntity).ToList();
            }
            return envelope;
        }

        public MasterDataEnvelope BuildRetireSetting(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_retireDocumentSettingRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                envelope.masterData = _retireDocumentSettingRepository.GetItemUpdated(dateSince)
                    .Select(n => n as MasterEntity).ToList();
            }
            return envelope;
        }

        public MasterDataEnvelope BuildCommodityType(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_commodityTypeRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                if (cct != null)
                    switch (cct.CostCentreType)
                    {
                        case CostCentreType.Hub:
                            envelope.masterData = _commodityTypeRepository.GetItemUpdated(dateSince)
                                .Select(n => n as MasterEntity).ToList();
                            break;
                        case CostCentreType.PurchasingClerk:
                            envelope.masterData = _commodityTypeRepository.GetItemUpdated(dateSince)
                                .Select(n => n as MasterEntity).ToList();
                            break;
                    }
            }
            return envelope;
        }

        public MasterDataEnvelope BuildCommodity(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_commodityRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                if (cct != null)
                    switch (cct.CostCentreType)
                    {
                        case CostCentreType.Hub:
                            envelope.masterData = _commodityRepository.GetItemUpdated(dateSince)
                                .Select(n => n as MasterEntity).ToList();
                            break;
                        case CostCentreType.PurchasingClerk:
                            envelope.masterData = _commodityRepository.GetItemUpdated(dateSince)
                                .Select(n => n as MasterEntity).ToList();
                            break;
                    }
            }
            return envelope;
        }

        public MasterDataEnvelope BuildCommodityOwnerType(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_commodityOwnerTypeRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                if (cct != null)
                    switch (cct.CostCentreType)
                    {
                        case CostCentreType.Hub:
                            envelope.masterData = _commodityOwnerTypeRepository.GetItemUpdated(dateSince)
                                .Select(n => n as MasterEntity).ToList();
                            break;
                        case CostCentreType.PurchasingClerk:
                            envelope.masterData = _commodityOwnerTypeRepository.GetItemUpdated(dateSince)
                                .Select(n => n as MasterEntity).ToList();
                            break;

                    }
            }
            return envelope;
        }

        public MasterDataEnvelope BuildCommodityProducer(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_commodityProducerRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                if (cct != null)
                    switch (cct.CostCentreType)
                    {
                        case CostCentreType.Hub:
                            var supplierIds = _commoditySupplierRepository.GetAll(true)
                                .Where(n => n.ParentCostCentre.Id == cct.Id)
                                .Select(n => n.Id);
                            envelope.masterData = _commodityProducerRepository.GetItemUpdated(dateSince)
                                .Where(n => n.CommoditySupplier != null && supplierIds.Contains(n.CommoditySupplier.Id))
                                .Select(n => n as MasterEntity).ToList();
                            break;
                        case CostCentreType.PurchasingClerk:
                            var hub = _costCentreRepository.GetById(cct.ParentCostCentre.Id) as Hub;
                            supplierIds = _commoditySupplierRepository.GetAll(true)
                                .Where(n => n.ParentCostCentre.Id == hub.Id)
                                .Select(n => n.Id);
                            envelope.masterData = _commodityProducerRepository.GetItemUpdated(dateSince)
                                    .Where(n =>  supplierIds.Contains(n.CommoditySupplier.Id))
                                    .Select(n => n as MasterEntity).ToList();
                            break;
                    }
            }
            return envelope;
        }

        public MasterDataEnvelope BuildCommoditySupplier(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_commoditySupplierRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                var suppliers = new List<CommoditySupplier>();
                var ids = new List<Guid>();
                if (cct != null)
                {
                    switch (cct.CostCentreType)
                    {
                        case CostCentreType.Hub:
                            suppliers = _commoditySupplierRepository.GetItemUpdated(dateSince)
                                .Where(n => n.ParentCostCentre.Id == cct.Id).Cast<CommoditySupplier>().ToList();
                            break;
                        case CostCentreType.PurchasingClerk:
                            var parentCostCentreRef= (_costCentreRepository.GetById(cct.Id)).ParentCostCentre;
                            ids = _costCentreRepository.GetAll(true)
                                .Where(n => n.ParentCostCentre != null && n.ParentCostCentre.Id == parentCostCentreRef.Id)
                                .Select(n => n.Id).ToList();
                            suppliers = _commoditySupplierRepository.GetItemUpdated(dateSince)
                                .Where(n => ids.Contains(n.Id)).Cast<CommoditySupplier>().ToList();
                            break;
                    }
                }
                envelope.masterData = suppliers.Select(n => n as MasterEntity).ToList();
            }
            return envelope;
        }

        public MasterDataEnvelope BuildCommodityOwner(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_commodityOwnerRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                var supplierIds = new List<Guid>();
                if (cct != null)
                    switch (cct.CostCentreType)
                    {
                        case CostCentreType.Hub:
                            supplierIds = _commoditySupplierRepository.GetAll(true)
                                .Where(n => n.ParentCostCentre.Id == cct.Id)
                                .Select(n => n.Id).ToList();
                            envelope.masterData = _commodityOwnerRepository.GetItemUpdated(dateSince)
                                    .Where(n => supplierIds.Contains(n.CommoditySupplier.Id))
                                    .Select(n => n as MasterEntity).ToList();
                            break;
                        case CostCentreType.PurchasingClerk:
                            var hub = _costCentreRepository.GetById(cct.ParentCostCentre.Id) as Hub;
                            supplierIds = _commoditySupplierRepository.GetAll(true)
                                .Where(n => n.ParentCostCentre.Id == hub.Id)
                                .Select(n => n.Id).ToList();
                            envelope.masterData = _commodityOwnerRepository.GetItemUpdated(dateSince)
                                    .Where(n =>  supplierIds.Contains(n.CommoditySupplier.Id))
                                    .Select(n => n as MasterEntity).ToList();
                            break;
                    }
            }
            return envelope;
        }

        public MasterDataEnvelope BuildCentreType(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_centreTypeRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                if (cct != null)
                    switch (cct.CostCentreType)
                    {
                        case CostCentreType.Hub:
                            envelope.masterData = _centreTypeRepository.GetItemUpdated(dateSince).Select(n => n as MasterEntity)
                                .ToList();
                            break;
                        case CostCentreType.PurchasingClerk:
                            envelope.masterData = _centreTypeRepository.GetItemUpdated(dateSince).Select(n => n as MasterEntity)
                                .ToList();
                            break;
                    }
            }
            return envelope;
        }

        public MasterDataEnvelope BuildCentre(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_centreRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                var centres = new List<Centre>();
                var hubRoutes = new List<Route>();
                List<MasterDataAllocation> hubRouteAllocations = new List<MasterDataAllocation>();
                List<Guid> centreIds_allocated_to_hubRoutes = new List<Guid>();
                if (cct != null)
                    switch (cct.CostCentreType)
                    {
                        case CostCentreType.Hub:
                            centres = _centreRepository.GetAll(true).Where(r => r.Hub.Id == cct.Id).ToList();
                            break;
                        case CostCentreType.PurchasingClerk:
                            centres = _centreRepository.GetAll(true).ToList();
                            break;
                    }
                envelope.masterData = centres.Select(n => n as MasterEntity).ToList();
            }
            return envelope;
        }

        public MasterDataEnvelope BuildHub(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_hubRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                if (cct != null)
                    switch (cct.CostCentreType)
                    {
                        case CostCentreType.Hub:
                            envelope.masterData = _hubRepository.GetItemUpdated(dateSince)
                                .Select(n => n as MasterEntity).ToList();
                            break;
                        case CostCentreType.PurchasingClerk:
                            envelope.masterData = _hubRepository.GetItemUpdated(dateSince)
                                .Select(n => n as MasterEntity).ToList();
                            break;
                    }
            }
            return envelope;
        }

        public MasterDataEnvelope BuildStore(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_storeRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                if (cct != null)
                    switch (cct.CostCentreType)
                    {
                        case CostCentreType.Hub:
                            envelope.masterData = _storeRepository.GetItemUpdated(dateSince)
                                .Where(n => n.ParentCostCentre.Id == cct.Id )
                                .Select(n => n as MasterEntity).ToList();
                            break;
                        case CostCentreType.PurchasingClerk:
                            envelope.masterData = _storeRepository.GetItemUpdated(dateSince)
                                .Select(n => n as MasterEntity).ToList();
                            break;
                    }
            }
            return envelope;
        }

        public MasterDataEnvelope BuildFieldClerk(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_purchasingClerkRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                var costCentreIds = _costCentreRepository.GetAll(true)
                    .Where(n => n.ParentCostCentre != null && n.ParentCostCentre.Id == cct.Id)
                    .Distinct().Select(n => n.Id).ToList();
                var userIds = new List<Guid>();
                var clerks = new List<PurchasingClerk>();
                if (cct != null)
                    switch (cct.CostCentreType)
                    {
                        case CostCentreType.Hub:
                            userIds = _userRepository.GetAll(true)
                                .Where(n => n.CostCentre == cct.Id || costCentreIds.Contains(n.CostCentre))
                                .Select(n => n.Id)
                                .ToList();
                            clerks = _purchasingClerkRepository
                                .GetItemUpdated(dateSince)
                                .Cast<PurchasingClerk>()
                                .Where(n => n.User != null && userIds.Contains(n.User.Id))
                                .ToList();
                            break;
                        case CostCentreType.PurchasingClerk:
                            userIds = _userRepository.GetAll(true)
                                .Where(n => n.CostCentre == cct.Id || costCentreIds.Contains(n.CostCentre))
                                .Select(n => n.Id)
                                .ToList();
                            clerks = _purchasingClerkRepository
                                .GetItemUpdated(dateSince)
                                .Cast<PurchasingClerk>()
                                .Where(n => n.User != null && userIds.Contains(n.User.Id))
                                .ToList();
                            break;
                    }
                envelope.masterData = clerks.Select(n => n as MasterEntity).ToList();
            }
            return envelope;
        }

        public MasterDataEnvelope BuildContainerType(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_containerTypeRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                if (cct != null)
                    switch (cct.CostCentreType)
                    {
                        case CostCentreType.Hub:
                        case CostCentreType.PurchasingClerk:
                            envelope.masterData = _containerTypeRepository.GetItemUpdated(dateSince)
                                .Select(n => n as MasterEntity)
                                .ToList();
                                break;
                    }
            }
            return envelope;
        }

        public MasterDataEnvelope BuildPrinter(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_equipmentRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                if (cct != null)
                    switch (cct.CostCentreType)
                    {
                        case CostCentreType.Hub:
                            envelope.masterData = _equipmentRepository.GetItemUpdated(dateSince)
                                .OfType<Printer>().Select(n => n as MasterEntity)
                                .ToList();
                            break;
                        case CostCentreType.PurchasingClerk:
                            envelope.masterData = _equipmentRepository.GetItemUpdated(dateSince)
                                .OfType<Printer>().Select(n => n as MasterEntity)
                                .ToList();
                            break;
                    }
            }
            return envelope;
        }

        public MasterDataEnvelope BuildWeighScale(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_equipmentRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                if (cct != null)
                    switch (cct.CostCentreType)
                    {
                        case CostCentreType.Hub:
                            envelope.masterData = _equipmentRepository.GetItemUpdated(dateSince)
                                .OfType<WeighScale>().Select(n => n as MasterEntity)
                                .ToList();
                            break;
                        case CostCentreType.PurchasingClerk:
                            envelope.masterData = _equipmentRepository.GetItemUpdated(dateSince)
                                .OfType<WeighScale>().Select(n => n as MasterEntity)
                                .ToList();
                            break;
                    }
            }
            return envelope;
        }

        public MasterDataEnvelope BuildSourcingContainer(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_equipmentRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                if (cct != null)
                    switch (cct.CostCentreType)
                    {
                        case CostCentreType.Hub:
                            envelope.masterData = _equipmentRepository.GetItemUpdated(dateSince)
                                .OfType<SourcingContainer>().Select(n => n as MasterEntity)
                                .ToList();
                            break;
                        case CostCentreType.PurchasingClerk:
                            envelope.masterData = _equipmentRepository.GetItemUpdated(dateSince)
                                .OfType<SourcingContainer>().Select(n => n as MasterEntity)
                                .ToList();
                            break;
                    }
            }
            return envelope;
        }

        public MasterDataEnvelope BuildVehicle(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_vehicleRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                if (cct != null)
                    switch (cct.CostCentreType)
                    {
                        case CostCentreType.Hub:
                            envelope.masterData = _vehicleRepository.GetItemUpdated(dateSince)
                                .Select(n => n as MasterEntity)
                                .ToList();
                            break;
                        case CostCentreType.PurchasingClerk:
                            envelope.masterData = _vehicleRepository.GetItemUpdated(dateSince)
                                .Select(n => n as MasterEntity)
                                .ToList();
                            break;
                    }
            }
            return envelope;
        }

        public MasterDataEnvelope BuildRouteCentreAllocation(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() {masterDataName = masterdata.ToString()};
            if(_masterDataAllocatioRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                if(cct != null)
                {
                    envelope.masterData = _masterDataAllocatioRepository
                        .GetItemUpdated(dateSince)
                        .Where(x => x.AllocationType == MasterDataAllocationType.RouteCentreAllocation)
                        .Select(n => n as MasterEntity)
                        .ToList();
                }
            }
            return envelope;
        }

        public MasterDataEnvelope BuildCommodityProducerCentreAllocation(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_masterDataAllocatioRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                if (cct != null)
                {
                    envelope.masterData = _masterDataAllocatioRepository
                        .GetItemUpdated(dateSince)
                        .Where(x => x.AllocationType == MasterDataAllocationType.CommodityProducerCentreAllocation)
                        .Select(n => n as MasterEntity)
                        .ToList();
                }
            }
            return envelope;
        }

        public MasterDataEnvelope BuildRouteCostCentreAllocation(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_masterDataAllocatioRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                if (cct != null)
                {
                    envelope.masterData = _masterDataAllocatioRepository
                        .GetItemUpdated(dateSince)
                        .Where(x => x.AllocationType == MasterDataAllocationType.RouteCostCentreAllocation)
                        .Select(n => n as MasterEntity)
                        .ToList();
                }
            }
            return envelope;
        }

        public MasterDataEnvelope BuildRouteRegionAllocation(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct)
        {
            var envelope = new MasterDataEnvelope() { masterDataName = masterdata.ToString() };
            if (_masterDataAllocatioRepository.GetItemUpdatedSinceDateTime(dateSince))
            {
                if (cct != null)
                {
                    envelope.masterData = _masterDataAllocatioRepository
                        .GetItemUpdated(dateSince)
                        .Where(x => x.AllocationType == MasterDataAllocationType.RouteRegionAllocation)
                        .Select(n => n as MasterEntity)
                        .ToList();
                }
            }
            return envelope;
        }

        public bool IsUpdated(MasterDataCollective masterdata, DateTime date)
        {
            switch (masterdata)
            {
                case MasterDataCollective.PricingTier:
                    if (_ProductPricingTierRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;
                case MasterDataCollective.User:
                    if (_userRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;
                case MasterDataCollective.Area:
                    if (_areaRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;
                case MasterDataCollective.ConsolidatedProduct:
                    if (_productRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;
                case MasterDataCollective.Contact:
                    if (_contactRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;
                case MasterDataCollective.Country:
                    if (_countryRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;
                case MasterDataCollective.Distributor:
                    if (_distributorrepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;
                case MasterDataCollective.DistributorSalesman:
                    if (_distributorSalesmanRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;
                case MasterDataCollective.Outlet:
                    if (_outletRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;
                case MasterDataCollective.OutletCategory:
                    if (_outletCategoryRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;
                case MasterDataCollective.OutletType:
                    if (_outletTypeRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;
                case MasterDataCollective.Pricing:
                    if (_pricingRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;
                case MasterDataCollective.Producer:
                    if (_producerRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;
                case MasterDataCollective.ProductBrand:
                    if (_productBrandRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;
                case MasterDataCollective.ProductFlavour:
                    if (_productFlavourRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;
                case MasterDataCollective.ProductPackaging:
                    if (_productPackagingRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;
                case MasterDataCollective.ProductPackagingType:
                    if (_productPackagingTypeRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;
                case MasterDataCollective.ProductType:
                    if (_productTypeRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;
                case MasterDataCollective.Region:
                    if (_regionRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;
                case MasterDataCollective.ReturnableProduct:
                    if (_productRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;
                case MasterDataCollective.Route:
                    if (_routeRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;
                case MasterDataCollective.SaleProduct:
                    if (_productRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;
                case MasterDataCollective.SocioEconomicStatus:
                    if (_socioEconomicStatusRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;
                case MasterDataCollective.Territory:
                    if (_territoryRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;
                case MasterDataCollective.VatClass:
                    if (_vatClassRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;
                case MasterDataCollective.ChannelPackaging:
                    if (_channelPackagingRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;
                case MasterDataCollective.FreeOfChargeDiscount:
                    if (_freeOfChargeDiscountRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;
                case MasterDataCollective.Competitor:
                    if (_competitorRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;
                case MasterDataCollective.CompetitorProduct:
                    if (_competitorProductsRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;
                case MasterDataCollective.Asset:
                    if (_coolerRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;
                case MasterDataCollective.AssetType:
                    if (_coolerTypeRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;
                case MasterDataCollective.District:
                    if (_districtRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;
                case MasterDataCollective.Province:
                    if (_provinceRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;
                case MasterDataCollective.ReorderLevel:
                    if (_reorderLevelRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;
                //case MasterDataCollective.Returnables:
                //    if (_returnablesRepository.GetItemUpdatedSinceDateTime(date))
                //        return false;
                //    break;
                //case MasterDataCollective.Shells:
                //    if (_shellsRepository.GetItemUpdatedSinceDateTime(date))
                //        return false;
                //    break;
                case MasterDataCollective.TargetPeriod:
                    if (_targetPeriodRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;
                case MasterDataCollective.Target:
                    if (_targetRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;
                case MasterDataCollective.SaleValueDiscount:
                    if (_saleValueDiscountRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;
                case MasterDataCollective.ProductDiscount:
                    if (_productDiscountRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;
                case MasterDataCollective.PromotionDiscount:
                    if (_promotionDiscountRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;
                case MasterDataCollective.CertainValueCertainProductDiscount:
                    if (_certainValueCertainProductDiscountRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;
                case MasterDataCollective.ProductGroupDiscount:
                    if (_productDiscountGroupRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;
                case MasterDataCollective.DiscountGroup:
                    if (_discountGroupRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;

                case MasterDataCollective.SalesmanRoute:
                    if (_salesmanRouteRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;
                case MasterDataCollective.UserGroup:
                    if (_userGroupRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;
                case MasterDataCollective.UserGroupRole:
                    if (_userGroupRolesRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;
                case MasterDataCollective.Supplier:
                    if (_supplierRepository.GetItemUpdatedSinceDateTime(date))

                        return false;
                    break;
                case MasterDataCollective.ContactType:
                    if (_contactTypeRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;
                
                  
                case MasterDataCollective.AssetCategory:
                    if (_assetCategoryRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;
                case MasterDataCollective.AssetStatus:
                    if (_assetStatusRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;
                case MasterDataCollective.OutletPriority:
                    if (_outletPriorityRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;
                case MasterDataCollective.OutletVisitDay:
                    if (_outletVisitDayRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;
                case MasterDataCollective.TargetItem:
                    if (_targetItemRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;
                case MasterDataCollective.Setting:
                    if (_settingsRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;
                case MasterDataCollective.DistributorPendingDispatchWarehouse:
                    if (_distributorPendingDispatchWarehouseRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;
                case MasterDataCollective.Bank:
                    if (_bankRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;
                case MasterDataCollective.BankBranch:
                    if (_bankBranchRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;
                case MasterDataCollective.RetireSetting:
                    if (_retireDocumentSettingRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;
                case MasterDataCollective.CommodityType:
                    if (_commodityTypeRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;
                case MasterDataCollective.Commodity:
                    if (_commodityRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;
                case MasterDataCollective.CommodityOwnerType:
                    if (_commodityOwnerTypeRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;
                case MasterDataCollective.CommodityProducer:
                    if (_commodityProducerRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;
                case MasterDataCollective.CommoditySupplier:
                    if (_commoditySupplierRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;
                case MasterDataCollective.CommodityOwner:
                    if (_commodityOwnerRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;
                case MasterDataCollective.Centre:
                    if (_centreRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;
                case MasterDataCollective.CentreType:
                    if (_centreTypeRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;
                case MasterDataCollective.Hub:
                    if (_hubRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;
                case MasterDataCollective.Store:
                    if (_storeRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;
                case MasterDataCollective.FieldClerk:
                    if (_purchasingClerkRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;
                case MasterDataCollective.ContainerType:
                    if (_containerTypeRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;
                case MasterDataCollective.Vehicle:
                    if (_vehicleRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;

                case MasterDataCollective.Printer:
                case MasterDataCollective.WeighScale:
                case MasterDataCollective.SourcingContainer:
                    if (_equipmentRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;
                case MasterDataCollective.CommodityProducerCentreAllocation:
                case MasterDataCollective.RouteCentreAllocation:
                case MasterDataCollective.RouteCostCentreAllocation:
                case MasterDataCollective.RouteRegionAllocation:
                    if (_masterDataAllocatioRepository.GetItemUpdatedSinceDateTime(date))
                        return false;
                    break;
                default:
                    throw new Exception("Internal Error: MasterData not mapped!" + masterdata.ToString());
            }
            return true;
        }
    }
}
