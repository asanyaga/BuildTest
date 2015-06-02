using System;
using System.Linq;
using AutoMapper;
using Distributr.Core.Domain.FinancialEntities;
using Distributr.Core.Domain.InventoryEntities;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.AssetEntities;
using Distributr.Core.Domain.Master.BankEntities;
using Distributr.Core.Domain.Master.CentreEntity;
using Distributr.Core.Domain.Master.ChannelPackagings;
using Distributr.Core.Domain.Master.CommodityEntities;
using Distributr.Core.Domain.Master.CommodityEntity;
using Distributr.Core.Domain.Master.CompetitorManagement;
using Distributr.Core.Domain.Master.CoolerEntities;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.DistributorTargetEntities;
using Distributr.Core.Domain.Master.EquipmentEntities;
using Distributr.Core.Domain.Master.FarmActivities;
using Distributr.Core.Domain.Master.OutletVisitReasonsTypeEntities;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Master.ReOrdeLevelEntities;
using Distributr.Core.Domain.Master.SettingsEntities;
using Distributr.Core.Domain.Master.SuppliersEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.MasterDataDTO.DTOModels.AgrimanagrDTO.CommodityDTOs;
using Distributr.Core.MasterDataDTO.DTOModels.AgrimanagrDTO.CostCentreDTOs;
using Distributr.Core.MasterDataDTO.DTOModels.AgrimanagrDTO.EquipmentDTOs;
using Distributr.Core.MasterDataDTO.DTOModels.AgrimanagrDTO.FarmActivities;
using Distributr.Core.MasterDataDTO.DTOModels.FinancialDTO;
using Distributr.Core.MasterDataDTO.DTOModels.InventoriesDTO;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Assets;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Banks;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.ChannelPackaging;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Competitor;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.CostCentre;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.DistributorTargets;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.MaritalStatus;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.MasterDataAllocations;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Product;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Retire;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Settings;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Suppliers;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.User;
using Distributr.Core.Repository.Master.AssetRepositories;
using Distributr.Core.Repository.Master.BankRepositories;
using Distributr.Core.Repository.Master.CentreRepositories;
using Distributr.Core.Repository.Master.CommodityRepositories;
using Distributr.Core.Repository.Master.CompetitorManagement;
using Distributr.Core.Repository.Master.CoolerTypeRepositories;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.DistributorTargetRepositories;
using Distributr.Core.Repository.Master.EquipmentRepository;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Master.SuppliersRepositories;
using Distributr.Core.Repository.Master.UserRepositories;

namespace Distributr.Core.Utility.Mapping.impl
{
   public class DTOToEntityMapping : IDTOToEntityMapping
   {
       private IProductTypeRepository _productTypeRepository;
       private IRegionRepository _regionRepository;
       private IContactTypeRepository _contactTypeRepository;
       //private IMaritalStatusRepository _maritalStatusRepository;
       private IUserRepository _userRepository;
       private IProductPricingTierRepository _pricingTierRepository;
       private IRouteRepository _routeRepository;
       private IOutletCategoryRepository _outletCategoryRepository;
       private IOutletTypeRepository _outletTypeRepository;
     
       private IDiscountGroupRepository _discountGroupRepository;
       private IVATClassRepository _vatClassRepository;
       
       private ICountryRepository _countryRepository;
       private ISupplierRepository _supplierRepository;
       private IProductBrandRepository _productBrandRepository;
       private IProductRepository _productRepository;
       private IProductFlavourRepository _productFlavourRepository;
       private IUserGroupRepository _userGroupRepository;
       private ICostCentreRepository _costCentreRepository;
       private IProvincesRepository _provincesRepository;
       private ITargetPeriodRepository _targetPeriodRepository;
       private IAssetTypeRepository _assetTypeRepository;
       private IAssetStatusRepository _assetStatusRepository;
       private IAssetCategoryRepository _assetCategoryRepository;
       private IProductPackagingRepository _productPackagingRepository;
       private IBankRepository _bankRepository;
       private ICompetitorRepository _competitorRepository;
       private IProductPackagingTypeRepository _productPackagingTypeRepository;
       private IContainmentRepository _containmentRepository;
      
       private ITargetRepository _targetRepository;
       private ICommodityTypeRepository _commodityTypeRepository;
       private ICommodityRepository _commodityRepository;
       private ICommodityOwnerTypeRepository _commodityOwnerTypeRepository;
      
       private ICommoditySupplierRepository _commoditySupplierRepository;
      private IHubRepository _hubRepository;
    
       private ICentreTypeRepository _centreTypeRepository;
       private IContainerTypeRepository _containerTypeRepository;
  
       private IBankBranchRepository _bankBranchRepository;
       private ICommodityProducerRepository _commodityProducerRepository;

       public DTOToEntityMapping(IProductTypeRepository productTypeRepository, IRegionRepository regionRepository, IContactTypeRepository contactTypeRepository, IUserRepository userRepository, IProductPricingTierRepository pricingTierRepository, IRouteRepository routeRepository, IOutletCategoryRepository outletCategoryRepository, IOutletTypeRepository outletTypeRepository, IDiscountGroupRepository discountGroupRepository, IVATClassRepository vatClassRepository, ICountryRepository countryRepository, ISupplierRepository supplierRepository, IProductBrandRepository productBrandRepository, IProductRepository productRepository, IProductFlavourRepository productFlavourRepository, IUserGroupRepository userGroupRepository, ICostCentreRepository costCentreRepository, IProvincesRepository provincesRepository, ITargetPeriodRepository targetPeriodRepository, IAssetTypeRepository assetTypeRepository, IAssetStatusRepository assetStatusRepository, IAssetCategoryRepository assetCategoryRepository, IProductPackagingRepository productPackagingRepository, IBankRepository bankRepository, ICompetitorRepository competitorRepository, IProductPackagingTypeRepository productPackagingTypeRepository, IContainmentRepository containmentRepository, ITargetRepository targetRepository, ICommodityTypeRepository commodityTypeRepository, ICommodityRepository commodityRepository, ICommodityOwnerTypeRepository commodityOwnerTypeRepository, ICommoditySupplierRepository commoditySupplierRepository, IHubRepository hubRepository, ICentreTypeRepository centreTypeRepository, IContainerTypeRepository containerTypeRepository, IBankBranchRepository bankBranchRepository, ICommodityProducerRepository commodityProducerRepository)
       {
           _productTypeRepository = productTypeRepository;
           _regionRepository = regionRepository;
           _contactTypeRepository = contactTypeRepository;
           _userRepository = userRepository;
           _pricingTierRepository = pricingTierRepository;
           _routeRepository = routeRepository;
           _outletCategoryRepository = outletCategoryRepository;
           _outletTypeRepository = outletTypeRepository;
           _discountGroupRepository = discountGroupRepository;
           _vatClassRepository = vatClassRepository;
           _countryRepository = countryRepository;
           _supplierRepository = supplierRepository;
           _productBrandRepository = productBrandRepository;
           _productRepository = productRepository;
           _productFlavourRepository = productFlavourRepository;
           _userGroupRepository = userGroupRepository;
           _costCentreRepository = costCentreRepository;
           _provincesRepository = provincesRepository;
           _targetPeriodRepository = targetPeriodRepository;
           _assetTypeRepository = assetTypeRepository;
           _assetStatusRepository = assetStatusRepository;
           _assetCategoryRepository = assetCategoryRepository;
           _productPackagingRepository = productPackagingRepository;
           _bankRepository = bankRepository;
           _competitorRepository = competitorRepository;
           _productPackagingTypeRepository = productPackagingTypeRepository;
           _containmentRepository = containmentRepository;
           _targetRepository = targetRepository;
           _commodityTypeRepository = commodityTypeRepository;
           _commodityRepository = commodityRepository;
           _commodityOwnerTypeRepository = commodityOwnerTypeRepository;
           _commoditySupplierRepository = commoditySupplierRepository;
           _hubRepository = hubRepository;
           _centreTypeRepository = centreTypeRepository;
           _containerTypeRepository = containerTypeRepository;
           _bankBranchRepository = bankBranchRepository;
           _commodityProducerRepository = commodityProducerRepository;
       }

       public static  void SetupAutomapperMappings()
       {
           Mapper.CreateMap<AreaDTO, Area>()
               .ConstructUsing(x => new Area(x.MasterId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<CountryDTO, Country>()
               .ConstructUsing(x => new Country(x.MasterId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<ContactDTO, Contact>()
               .ConstructUsing((ContactDTO x) => new Contact(x.MasterId))
               .ForMember(dest => dest.ContactClassification, opt => opt.MapFrom(src => src.ContactClassificationId))
               .ForMember(dest => dest.ContactOwnerType, opt => opt.MapFrom(src => src.ContactOwnerType))
               .ForMember(dest => dest.MStatus, opt => opt.MapFrom(src => src.MaritalStatusMasterId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<DistributorDTO, Distributor>()
               .ConstructUsing((DistributorDTO x) => new Distributor(x.MasterId))
               .ForMember(dest => dest.ParentCostCentre, opt => opt.MapFrom(src => new CostCentreRef{Id= src.ParentCostCentreId}))
               .ForMember(dest => dest.CostCentreType, opt => opt.MapFrom(src => src.CostCentreTypeId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<ProducerDTO, Producer>()
               .ConstructUsing((ProducerDTO x) => new Producer(x.MasterId))
               .ForMember(dest => dest.CostCentreType, opt => opt.MapFrom(src => src.CostCentreTypeId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<TransporterDTO, Transporter>()
               .ConstructUsing((TransporterDTO x) => new Transporter(x.MasterId))
               .ForMember(dest => dest.ParentCostCentre, opt => opt.MapFrom(src => new CostCentreRef { Id = src.ParentCostCentreId }))
               .ForMember(dest => dest.CostCentreType, opt => opt.MapFrom(src => src.CostCentreTypeId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<OutletCategoryDTO, OutletCategory>()
               .ConstructUsing((OutletCategoryDTO x) => new OutletCategory(x.MasterId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<OutletDTO, Outlet>()
               .ConstructUsing((OutletDTO x) => new Outlet(x.MasterId))
               .ForMember(dest => dest.ParentCostCentre, opt => opt.MapFrom(src => new CostCentreRef { Id = src.ParentCostCentreId }))
                .ForMember(dest => dest.CostCentreType, opt => opt.MapFrom(src => src.CostCentreTypeId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<ShipToAddressDTO, ShipToAddress>()
               .ConstructUsing((ShipToAddressDTO x) => new ShipToAddress(x.MasterId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<OutletTypeDTO, OutletType>()
               .ConstructUsing((OutletTypeDTO x) => new OutletType(x.MasterId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<RouteDTO, Route>()
               .ConstructUsing((RouteDTO x) => new Route(x.MasterId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<RegionDTO, Region>()
               .ConstructUsing((RegionDTO x) => new Region(x.MasterId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<SocioEconomicStatusDTO, SocioEconomicStatus>()
               .ConstructUsing((SocioEconomicStatusDTO x) => new SocioEconomicStatus(x.MasterId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<TerritoryDTO, Territory>()
               .ConstructUsing((TerritoryDTO x) => new Territory(x.MasterId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<ProductBrandDTO, ProductBrand>()
               .ConstructUsing((ProductBrandDTO x) => new ProductBrand(x.MasterId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<ProductFlavourDTO, ProductFlavour>()
               .ConstructUsing((ProductFlavourDTO x) => new ProductFlavour(x.MasterId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<ProductPackagingDTO, ProductPackaging>()
               .ConstructUsing((ProductPackagingDTO x) => new ProductPackaging(x.MasterId))
               .ForMember(dest => dest.ReturnableProductRef, opt => opt.MapFrom(src => 
                   src.ReturnableProductMasterId == Guid.Empty 
                   ? null : new ProductRef { ProductId = src.ReturnableProductMasterId }))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<ProductPackagingTypeDTO, ProductPackagingType>()
               .ConstructUsing((ProductPackagingTypeDTO x) => new ProductPackagingType(x.MasterId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<ProductPricingDTO, ProductPricing>()
               .ConstructUsing((ProductPricingDTO x) => new ProductPricing(x.MasterId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<ProductPricingItemDTO, ProductPricing.ProductPricingItem>()
               .ConstructUsing((ProductPricingItemDTO x) => new ProductPricing.ProductPricingItem(x.MasterId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<ProductPricingTierDTO, ProductPricingTier>()
               .ConstructUsing((ProductPricingTierDTO x) => new ProductPricingTier(x.MasterId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<ProductTypeDTO, ProductType>()
               .ConstructUsing((ProductTypeDTO x) => new ProductType(x.MasterId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<ReturnableProductDTO, ReturnableProduct>()
               .ConstructUsing((ReturnableProductDTO x) => new ReturnableProduct(x.MasterId))
               .ForMember(dest => dest.ReturnableType, opt => opt.MapFrom(src => src.ReturnableTypeMasterId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<SaleProductDTO, SaleProduct>()
               .ConstructUsing((SaleProductDTO x) => new SaleProduct(x.MasterId))
               .ForMember(dest => dest.ReturnableType, opt=> opt.MapFrom(src => src.ReturnableTypeMasterId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<VATClassDTO, VATClass>()
               .ConstructUsing((VATClassDTO x) => new VATClass(x.MasterId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<VatClassItemDTO, VATClass.VATClassItem>()
               .ConstructUsing((VatClassItemDTO x) => new VATClass.VATClassItem(x.MasterId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<UserDTO, User>()
               .ConstructUsing((UserDTO x) => new User(x.MasterId))
               .ForMember(dest => dest.UserType, opt => opt.MapFrom(src => src.UserTypeId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<ConsolidatedProductDTO, ConsolidatedProduct>()
               .ConstructUsing((ConsolidatedProductDTO x) => new ConsolidatedProduct(x.MasterId))
               .AfterMap(AfterMapAction);
           //Mapper.CreateMap<ConsolidatedProductProductDetailDTO, ConsolidatedProductDetail>();
           Mapper.CreateMap<DistributorSalesmanDTO, DistributorSalesman>()
               .ConstructUsing((DistributorSalesmanDTO x) => new DistributorSalesman(x.MasterId))
               .ForMember(dest => dest.ParentCostCentre, opt => opt.MapFrom(src => new CostCentreRef { Id = src.ParentCostCentreId }))
               .ForMember(dest => dest.CostCentreType, opt => opt.MapFrom(src => src.CostCentreTypeId))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.TypeId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<DistributorPendingDispatchWarehouseDTO, DistributorPendingDispatchWarehouse>()
               .ConstructUsing((DistributorPendingDispatchWarehouseDTO x) => new DistributorPendingDispatchWarehouse(x.MasterId))
               .ForMember(dest => dest.ParentCostCentre, opt => opt.MapFrom(src => new CostCentreRef { Id = src.ParentCostCentreId }))
               .ForMember(dest => dest.CostCentreType, opt => opt.MapFrom(src => src.CostCentreTypeId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<ProductDiscountDTO, ProductDiscount>()
               .ConstructUsing((ProductDiscountDTO x) => new ProductDiscount(x.MasterId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<ProductDiscountItemDTO, ProductDiscount.ProductDiscountItem>()
               .ConstructUsing((ProductDiscountItemDTO x) => new ProductDiscount.ProductDiscountItem(x.MasterId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<SaleValueDiscountDTO, SaleValueDiscount>()
               .ConstructUsing((SaleValueDiscountDTO x) => new SaleValueDiscount(x.MasterId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<SaleValueDiscountItemDTO, SaleValueDiscount.SaleValueDiscountItem>()
               .ConstructUsing((SaleValueDiscountItemDTO x) => new SaleValueDiscount.SaleValueDiscountItem(x.MasterId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<TargetPeriodDTO, TargetPeriod>()
               .ConstructUsing((TargetPeriodDTO x) => new TargetPeriod(x.MasterId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<TargetDTO, Target>()
               .ConstructUsing((TargetDTO x) => new Target(x.MasterId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<ReorderLevelDTO, ReOrderLevel>()
               .ConstructUsing((ReorderLevelDTO x) => new ReOrderLevel(x.MasterId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<ProvinceDTO, Province>()
               .ConstructUsing((ProvinceDTO x) => new Province(x.MasterId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<DistrictDTO, District>()
               .ConstructUsing((DistrictDTO x) => new District(x.MasterId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<AssetDTO, Asset>()
               .ConstructUsing((AssetDTO x) => new Asset(x.MasterId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<AssetTypeDTO, AssetType>()
               .ConstructUsing((AssetTypeDTO x) => new AssetType(x.MasterId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<ChannelPackagingDTO, ChannelPackaging>()
               .ConstructUsing((ChannelPackagingDTO x) => new ChannelPackaging(x.MasterId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<CompetitorDTO, Competitor>()
               .ConstructUsing((CompetitorDTO x) => new Competitor(x.MasterId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<CompetitorProductDTO, CompetitorProducts>()
               .ConstructUsing((CompetitorProductDTO x) => new CompetitorProducts(x.MasterId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<DiscountGroupDTO, DiscountGroup>()
               .ConstructUsing((DiscountGroupDTO x) => new DiscountGroup(x.MasterId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<ProductGroupDiscountDTO, ProductGroupDiscount>()
               .ConstructUsing((ProductGroupDiscountDTO x) => new ProductGroupDiscount(x.MasterId))
               .AfterMap(AfterMapAction);
          
           Mapper.CreateMap<CertainValueCertainProductDiscountDTO, CertainValueCertainProductDiscount>()
               .ConstructUsing((CertainValueCertainProductDiscountDTO x) => new CertainValueCertainProductDiscount(x.MasterId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<CertainValueCertainProductDiscountItemDTO, CertainValueCertainProductDiscount.CertainValueCertainProductDiscountItem>()
               .ConstructUsing((CertainValueCertainProductDiscountItemDTO x) => new CertainValueCertainProductDiscount.CertainValueCertainProductDiscountItem(x.MasterId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<PromotionDiscountDTO, PromotionDiscount>()
               .ConstructUsing((PromotionDiscountDTO x) => new PromotionDiscount(x.MasterId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<PromotionDiscountItemDTO, PromotionDiscount.PromotionDiscountItem>()
               .ConstructUsing((PromotionDiscountItemDTO x) => new PromotionDiscount.PromotionDiscountItem(x.MasterId))
               .ForMember(dest => dest.FreeOfChargeQuantity, opt => opt.MapFrom(src => src.FreeQuantity))
               .ForMember(dest => dest.ParentProductQuantity, opt => opt.MapFrom(src => src.ParentQuantity))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<FreeOfChargeDiscountDTO, FreeOfChargeDiscount>()
               .ConstructUsing((FreeOfChargeDiscountDTO x) => new FreeOfChargeDiscount(x.MasterId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<SalesmanRouteDTO, SalesmanRoute>()
               .ConstructUsing((SalesmanRouteDTO x) => new SalesmanRoute(x.MasterId))
               .ForMember(dest => dest.DistributorSalesmanRef, opt => opt.MapFrom(src => new CostCentreRef { Id = src.DistributorSalesmanMasterId }))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<SalesmanSupplierDTO, SalesmanSupplier>()
              .ConstructUsing((SalesmanSupplierDTO x) => new SalesmanSupplier(x.MasterId))
              .ForMember(dest => dest.DistributorSalesmanRef, opt => opt.MapFrom(src => new CostCentreRef { Id = src.DistributorSalesmanMasterId }))
              .AfterMap(AfterMapAction);

           Mapper.CreateMap<UserGroupDTO, UserGroup>()
               .ConstructUsing((UserGroupDTO x) => new UserGroup(x.MasterId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<UserGroupRoleDTO, UserGroupRoles>()
               .ForMember(dest => dest.UserRole, opt => opt.MapFrom(src => src.UserRoleMasterId))
               .ConstructUsing((UserGroupRoleDTO x) => new UserGroupRoles(x.MasterId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<BankBranchDTO, BankBranch>()
               .ConstructUsing((BankBranchDTO x) => new BankBranch(x.MasterId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<BankDTO, Bank>()
               .ConstructUsing((BankDTO x) => new Bank(x.MasterId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<SupplierDTO, Supplier>()
               .ConstructUsing((SupplierDTO x) => new Supplier(x.MasterId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<MaritalStatusDTO, MaritalStatus>()
               .ForMember(dest => dest.MStatus, opt => opt.MapFrom(src => src.MStatus))
               .ConstructUsing((MaritalStatusDTO x) => new MaritalStatus(x.MasterId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<ContactTypeDTO, ContactType>()
               .ConstructUsing((ContactTypeDTO x) => new ContactType(x.MasterId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<AssetCategoryDTO, AssetCategory>()
               .ConstructUsing((AssetCategoryDTO x) => new AssetCategory(x.MasterId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<AssetStatusDTO, AssetStatus>()
               .ConstructUsing((AssetStatusDTO x) => new AssetStatus(x.MasterId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<OutletVisitDayDTO, OutletVisitDay>()
               .ConstructUsing((OutletVisitDayDTO x) => new OutletVisitDay(x.MasterId))
               .ForMember(dest => dest.Outlet, opt => opt.MapFrom(src => new CostCentreRef { Id= src.OutletMasterId }))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<OutletPriorityDTO, OutletPriority>()
               .ConstructUsing((OutletPriorityDTO x) => new OutletPriority(x.MasterId))
               .ForMember(dest => dest.Outlet, opt => opt.MapFrom(src => new CostCentreRef { Id = src.OutletMasterId }))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<TargetItemDTO, TargetItem>()
               .ConstructUsing((TargetItemDTO x) => new TargetItem(x.MasterId))
               .ForMember(dest => dest.Product, opt => opt.MapFrom(src => new ProductRef {ProductId = src.ProductMasterId}))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<AppSettingsDTO, AppSettings>()
               .ConstructUsing((AppSettingsDTO x) => new AppSettings(x.MasterId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<InventoryDTO, Inventory>()
               .ConstructUsing((InventoryDTO x) => new Inventory(x.MasterId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<PaymentTrackerDTO, PaymentTracker>()
               .ConstructUsing((PaymentTrackerDTO x) => new PaymentTracker(x.MasterId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<RetireSettingDTO, RetireDocumentSetting>()
               .ConstructUsing((RetireSettingDTO x) => new RetireDocumentSetting(x.MasterId))
               .AfterMap(AfterMapAction);
           //Mapper.AssertConfigurationIsValid();
           Mapper.CreateMap<CommodityTypeDTO, CommodityType>()
               .ConstructUsing((CommodityTypeDTO x) => new CommodityType(x.MasterId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<CommodityDTO, Commodity>()
               .ConstructUsing((CommodityDTO x) => new Commodity(x.MasterId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<CommodityGradeDTO, CommodityGrade>()
               .ConstructUsing((CommodityGradeDTO x) => new CommodityGrade(x.MasterId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<CommodityOwnerTypeDTO, CommodityOwnerType>()
               .ConstructUsing((CommodityOwnerTypeDTO x) => new CommodityOwnerType(x.MasterId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<CommodityProducerDTO, CommodityProducer>()
               .ConstructUsing((CommodityProducerDTO x) => new CommodityProducer(x.MasterId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<CommoditySupplierDTO, CommoditySupplier>()
               .ConstructUsing((CommoditySupplierDTO x) => new CommoditySupplier(x.MasterId))
               .ForMember(dest => dest.ParentCostCentre, opt => opt.MapFrom(src => new CostCentreRef { Id = src.ParentCostCentreId }))
               .ForMember(dest => dest.CostCentreType, opt => opt.MapFrom(src => src.CostCentreTypeId))
               .ForMember(dest => dest.CommoditySupplierType, opt => opt.MapFrom(src => src.CommoditySupplierTypeId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<CommodityOwnerDTO, CommodityOwner>()
               .ConstructUsing((CommodityOwnerDTO x) => new CommodityOwner(x.MasterId))
               .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.GenderId))
               .ForMember(dest => dest.MaritalStatus, opt => opt.MapFrom(src => src.MaritalStatusId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<CentreTypeDTO, CentreType>()
               .ConstructUsing((CentreTypeDTO x) => new CentreType(x.MasterId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<CentreDTO, Centre>()
               .ConstructUsing((CentreDTO x) => new Centre(x.MasterId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<HubDTO, Hub>()
               .ConstructUsing((HubDTO x) => new Hub(x.MasterId))
               .ForMember(dest => dest.ParentCostCentre, opt => opt.MapFrom(src => new CostCentreRef { Id = src.ParentCostCentreId }))
               .ForMember(dest => dest.CostCentreType, opt => opt.MapFrom(src => src.CostCentreTypeId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<StoreDTO, Store>()
               .ConstructUsing((StoreDTO x) => new Store(x.MasterId))
               .ForMember(dest => dest.ParentCostCentre, opt => opt.MapFrom(src => new CostCentreRef { Id = src.ParentCostCentreId }))
               .ForMember(dest => dest.CostCentreType, opt => opt.MapFrom(src => src.CostCentreTypeId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<PurchasingClerkRouteDTO, PurchasingClerkRoute>()
               .ConstructUsing((PurchasingClerkRouteDTO x) => new PurchasingClerkRoute(x.MasterId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<PurchasingClerkDTO, PurchasingClerk>()
               .ConstructUsing((PurchasingClerkDTO x) => new PurchasingClerk(x.MasterId))
               .ForMember(dest => dest.ParentCostCentre, opt => opt.MapFrom(src => new CostCentreRef { Id = src.ParentCostCentreId }))
               .ForMember(dest => dest.CostCentreType, opt => opt.MapFrom(src => src.CostCentreTypeId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<ContainerTypeDTO, ContainerType>()
               .ConstructUsing((ContainerTypeDTO x) => new ContainerType(x.MasterId))
               .ForMember(dest => dest.ContainerUseType, opt => opt.MapFrom(src => src.ContainerUseTypeId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<PrinterDTO, Printer>()
               .ConstructUsing((PrinterDTO x) => new Printer(x.MasterId))
               .ForMember(dest => dest.EquipmentType, opt => opt.MapFrom(src => src.EquipmentTypeId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<VehicleDTO, Vehicle>()
               .ConstructUsing((VehicleDTO x) => new Vehicle(x.MasterId))
               .ForMember(dest => dest.EquipmentType, opt => opt.MapFrom(src => src.EquipmentTypeId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<WeighScaleDTO, WeighScale>()
               .ConstructUsing((WeighScaleDTO x) => new WeighScale(x.MasterId))
               .ForMember(dest => dest.EquipmentType, opt => opt.MapFrom(src => src.EquipmentTypeId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<SourcingContainerDTO, SourcingContainer>()
               .ConstructUsing((SourcingContainerDTO x) => new SourcingContainer(x.MasterId))
               .ForMember(dest => dest.EquipmentType, opt => opt.MapFrom(src => src.EquipmentTypeId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<CommodityProducerCentreAllocationDTO, MasterDataAllocation>()
               .ConstructUsing((CommodityProducerCentreAllocationDTO x) => new MasterDataAllocation(x.MasterId))
               .ForMember(dest => dest.EntityAId, opt => opt.MapFrom(src => src.CommodityProducerId))
               .ForMember(dest => dest.EntityBId, opt => opt.MapFrom(src => src.CentreId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<RouteCentreAllocationDTO, MasterDataAllocation>()
               .ConstructUsing((RouteCentreAllocationDTO x) => new MasterDataAllocation(x.MasterId))
               .ForMember(dest => dest.EntityAId, opt => opt.MapFrom(src => src.RouteId))
               .ForMember(dest => dest.EntityBId, opt => opt.MapFrom(src => src.CentreId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<RouteCostCentreAllocationDTO, MasterDataAllocation>()
               .ConstructUsing((RouteCostCentreAllocationDTO x) => new MasterDataAllocation(x.MasterId))
               .ForMember(dest => dest.EntityAId, opt => opt.MapFrom(src => src.RouteId))
               .ForMember(dest => dest.EntityBId, opt => opt.MapFrom(src => src.CostCentreId))
               .AfterMap(AfterMapAction);
           Mapper.CreateMap<RouteRegionAllocationDTO, MasterDataAllocation>()
               .ConstructUsing((RouteRegionAllocationDTO x) => new MasterDataAllocation(x.MasterId))
               .ForMember(dest => dest.EntityAId, opt => opt.MapFrom(src => src.RouteId))
               .ForMember(dest => dest.EntityBId, opt => opt.MapFrom(src => src.RegionId))
               .AfterMap(AfterMapAction);

           Mapper.CreateMap<ShiftDTO, Shift>()
              .ConstructUsing(x => new Shift(x.MasterId))
              .AfterMap(AfterMapAction);

           Mapper.CreateMap<ActivityTypeDTO, ActivityType>()
              .ConstructUsing(x => new ActivityType(x.MasterId))
              .AfterMap(AfterMapAction);

           Mapper.CreateMap<SeasonDTO, Season>()
             .ConstructUsing(x => new Season(x.MasterId))
             .AfterMap(AfterMapAction);

           Mapper.CreateMap<ServiceDTO, CommodityProducerService>()
         .ConstructUsing(x => new CommodityProducerService(x.MasterId))
         .AfterMap(AfterMapAction);

           Mapper.CreateMap<InfectionDTO, Infection>()
             .ConstructUsing(x => new Infection(x.MasterId))
              .ForMember(dest => dest.InfectionType, opt => opt.MapFrom(src => src.InfectionTypeId))
             .AfterMap(AfterMapAction);

           Mapper.CreateMap<ServiceProviderDTO, ServiceProvider>()
              .ConstructUsing(x => new ServiceProvider(x.MasterId))
              .ForMember(dest=>dest.Gender,opt=>opt.MapFrom(src=>src.GenderId))
               .AfterMap(AfterMapAction);

           Mapper.CreateMap<OutletVisitReasonTypeDTO, OutletVisitReasonsType>()
            .ConstructUsing(x => new OutletVisitReasonsType(x.MasterId))
             .ForMember(dest => dest.OutletVisitAction, opt => opt.MapFrom(src => src.OutletVisitActionId))
            .AfterMap(AfterMapAction);
          
       }

       static void AfterMapAction(MasterBaseDTO dto, MasterEntity entity)
       {
           if (dto == null)
           {
               return;
           }
           entity._SetDateCreated(dto.DateCreated); 
           entity._SetDateLastUpdated(dto.DateLastUpdated);
           entity._SetStatus((EntityStatus)dto.StatusId);
       }

       public DistributorPendingDispatchWarehouse Map(DistributorPendingDispatchWarehouseDTO dto)
       {
           if (dto == null) return null;
           var warehouse = Mapper.Map<DistributorPendingDispatchWarehouseDTO, DistributorPendingDispatchWarehouse>(dto);
           return warehouse;
       }

       public DistributorSalesman Map(DistributorSalesmanDTO dto)
       {
           if (dto == null) return null;
           var distributorSalesman = Mapper.Map<DistributorSalesmanDTO, DistributorSalesman>(dto);
           var parentCostCentre = new CostCentreRef { Id = dto.ParentCostCentreId };
           parentCostCentre.Id = dto.ParentCostCentreId;
           distributorSalesman.ParentCostCentre = parentCostCentre;
           distributorSalesman.Type = (DistributorSalesmanType) dto.TypeId;
           return distributorSalesman;
       }

       public Area Map(AreaDTO dto)
       {
           if (dto == null) return null;
           Area area = Mapper.Map<AreaDTO, Area>(dto);
           area.region = _regionRepository.GetById(dto.RegionMasterId);
           return area;
       }

       public Country Map(CountryDTO dto)
       {
           if (dto == null) return null;
           var country = Mapper.Map<CountryDTO, Country>(dto);
           return country;
       }

       public Contact Map(ContactDTO dto)
       {
           if (dto == null) return null;
           Contact contact =  Mapper.Map<ContactDTO, Contact>(dto);
           contact.MStatus = MaritalStatas.Single;// _maritalStatusRepository.GetById(dto.MaritalStatusMasterId);
           contact.ContactType = _contactTypeRepository.GetById(dto.ContactTypeMasterId);
           return contact;
       }

       public Distributor Map(DistributorDTO dto)
       {
           if (dto == null) return null;
           var distributor = Mapper.Map<DistributorDTO, Distributor>(dto);
           distributor.Region = _regionRepository.GetById(dto.RegionMasterId);
           distributor.ASM = _userRepository.GetById(dto.ASMUserMasterId.Value);
           distributor.SalesRep = _userRepository.GetById(dto.SalesRepUserMasterId.Value);
           distributor.Surveyor = _userRepository.GetById(dto.SurveyorUserMasterId.Value);
           distributor.ProductPricingTier = _pricingTierRepository.GetById(dto.ProductPricingTierMasterId);
           return distributor;
       }

       public Producer Map(ProducerDTO dto)
       {
           if (dto == null) return null;
           var producer = Mapper.Map<ProducerDTO, Producer>(dto);
           return producer;
       }

       public Transporter Map(TransporterDTO dto)
       {
           if (dto == null) return null;
           var transporter = Mapper.Map<TransporterDTO, Transporter>(dto);
           return transporter;
       }

       public OutletCategory Map(OutletCategoryDTO dto)
       {
           if (dto == null) return null;
            OutletCategory outletCategory = Mapper.Map<OutletCategoryDTO, OutletCategory>(dto);
           return outletCategory;
       }

       public Outlet Map(OutletDTO dto)
       {
           if (dto == null) return null;
           var outlet = Mapper.Map<OutletDTO, Outlet>(dto);
           outlet.Route = _routeRepository.GetById(dto.RouteMasterId);
           outlet.OutletCategory = _outletCategoryRepository.GetById(dto.OutletCategoryMasterId);
           outlet.OutletType = _outletTypeRepository.GetById(dto.OutletTypeMasterId);
           //outlet.UsrSurveyor;
           //outlet.UsrSalesRep;
           //outlet.UsrASM;
           //outlet.ContactPerson;
           //outlet.PhoneNumber;
           outlet.OutletProductPricingTier = _pricingTierRepository.GetById(dto.OutletProductPricingTierMasterId);
           outlet.SpecialPricingTier = _pricingTierRepository.GetById(dto.SpecialPricingTierMasterId);
           outlet.VatClass = _vatClassRepository.GetById(dto.VatClassMasterId);
           outlet.DiscountGroup = _discountGroupRepository.GetById(dto.DiscountGroupMasterId);
           var shippingAddresses = dto.ShippingAddresses.Select(n => Map((ShipToAddressDTO) n)).ToList();
           shippingAddresses.ForEach(outlet.AddShipToAddress);
           return outlet;
       }

       public ShipToAddress Map(ShipToAddressDTO dto)
       {
           if (dto == null) return null;
           var address = Mapper.Map<ShipToAddressDTO, ShipToAddress>(dto);
           return address;
       }

       public OutletType Map(OutletTypeDTO dto)
       {
           if (dto == null) return null;
           var outletType = Mapper.Map<OutletTypeDTO, OutletType>(dto);
           return outletType;
       }

       public Route Map(RouteDTO dto)
       {
           if (dto == null) return null;
           var route = Mapper.Map<RouteDTO, Route>(dto);
           route.Region = _regionRepository.GetById(dto.RegionId);
           return route;
       }

       public Region Map(RegionDTO dto)
       {
           if (dto == null) return null;
           var region = Mapper.Map<RegionDTO, Region>(dto);
           region.Country = _countryRepository.GetById(dto.CountryMasterId);
           return region;
       }

       public SocioEconomicStatus Map(SocioEconomicStatusDTO dto)
       {
           if (dto == null) return null;
           var socioEconomicStatus = Mapper.Map<SocioEconomicStatusDTO, SocioEconomicStatus>(dto);
           return socioEconomicStatus;
       }

       public Territory Map(TerritoryDTO dto)
       {
           if (dto == null) return null;
           var territory = Mapper.Map<TerritoryDTO, Territory>(dto);
           return territory;
       }

       public ConsolidatedProduct Map(ConsolidatedProductDTO dto)
       {
           if (dto == null) return null;
           var consolidatedProduct = Mapper.Map<ConsolidatedProductDTO, ConsolidatedProduct>(dto);
           //consolidatedProduct.ProductType = _productTypeRepository.GetById(dto.ProductType);
           consolidatedProduct.Brand = _productBrandRepository.GetById(dto.ProductBrandMasterId);
           consolidatedProduct.PackagingType = _productPackagingTypeRepository.GetById(dto.ProductPackagingTypeMasterId);
           
           if (dto.VatClassMasterId != null)
               consolidatedProduct.VATClass = _vatClassRepository.GetById(dto.VatClassMasterId.Value);
           consolidatedProduct.Packaging = _productPackagingRepository.GetById(dto.ProductPackagingMasterId);
           return consolidatedProduct;
       }

       public ProductBrand Map(ProductBrandDTO dto)
       {
           if (dto == null) return null;
           var productBrand = Mapper.Map<ProductBrandDTO, ProductBrand>(dto);
           productBrand.Supplier = _supplierRepository.GetById(dto.SupplierMasterId);
           return productBrand;
       }

       public ProductFlavour Map(ProductFlavourDTO dto)
       {
           if (dto == null) return null;
           var productFlavour = Mapper.Map<ProductFlavourDTO, ProductFlavour>(dto);
           productFlavour.ProductBrand = _productBrandRepository.GetById(dto.ProductBrandMasterId);
           return productFlavour;
       }

       public ProductPackaging Map(ProductPackagingDTO dto)
       {
           if (dto == null) return null;
           var productPackaging = Mapper.Map<ProductPackagingDTO, ProductPackaging>(dto);
           productPackaging.Containment = _containmentRepository.GetById(dto.ContainmentMasterId);
           return productPackaging;
       }

       public ProductPackagingType Map(ProductPackagingTypeDTO dto)
       {
           if (dto == null) return null;
           var productPackagingType = Mapper.Map<ProductPackagingTypeDTO, ProductPackagingType>(dto);
           return productPackagingType;
       }

       public ProductPricing Map(ProductPricingDTO dto)
       {
           if (dto == null) return null;
           var productPricing = Mapper.Map<ProductPricingDTO, ProductPricing>(dto);
           var productRef = new ProductRef { ProductId = dto.ProductMasterId };
           productPricing.ProductRef = productRef;
           productPricing.Tier = _pricingTierRepository.GetById(dto.ProductPricingTierMasterId);
           productPricing.ProductPricingItems = dto.ProductPricingItems.Select(n => Map(n)).ToList();
           return productPricing;
       }

       public ProductPricing.ProductPricingItem Map(ProductPricingItemDTO dto)
       {
           if (dto == null) return null;
           var pricingItem = Mapper.Map<ProductPricingItemDTO, ProductPricing.ProductPricingItem>(dto);
           return pricingItem;
       }

       public ProductPricingTier Map(ProductPricingTierDTO dto)
       {
           if (dto == null) return null;
           var pricingTier = Mapper.Map<ProductPricingTierDTO, ProductPricingTier>(dto);
           return pricingTier;
       }

       public ProductType Map(ProductTypeDTO dto)
       {
           if (dto == null) return null;
           var productType = Mapper.Map<ProductTypeDTO, ProductType>(dto);
           return productType;
       }

       public ReturnableProduct Map(ReturnableProductDTO dto)
       {
           if (dto == null) return null;
           var returnableProduct = Mapper.Map<ReturnableProductDTO, ReturnableProduct>(dto);
           returnableProduct.ReturnAbleProduct = (ReturnableProduct) _productRepository.GetById(dto.ReturnableProductMasterId);
           returnableProduct.Flavour = _productFlavourRepository.GetById(dto.ProductFlavourMasterId);
           returnableProduct.Brand = _productBrandRepository.GetById(dto.ProductBrandMasterId);
           returnableProduct.PackagingType = _productPackagingTypeRepository.GetById(dto.ProductPackagingTypeMasterId);
           returnableProduct.VATClass = _vatClassRepository.GetById(dto.VatClassMasterId.Value);
           returnableProduct.Packaging = _productPackagingRepository.GetById(dto.ProductPackagingMasterId);
           return returnableProduct;
       }

       public SaleProduct Map(SaleProductDTO dto)
       {
           if (dto == null) return null;
           var saleProduct = Mapper.Map<SaleProductDTO, SaleProduct>(dto);
           saleProduct.ReturnableProduct = (ReturnableProduct) _productRepository.GetById(dto.ReturnableProductMasterId);
           saleProduct.ProductType = _productTypeRepository.GetById(dto.ProductTypeMasterId);
           saleProduct.Flavour = _productFlavourRepository.GetById(dto.ProductFlavourMasterId);
           saleProduct.Brand = _productBrandRepository.GetById(dto.ProductBrandMasterId);
           saleProduct.PackagingType = _productPackagingTypeRepository.GetById(dto.ProductPackagingTypeMasterId);
           saleProduct.VATClass = _vatClassRepository.GetById(dto.VatClassMasterId.Value);
           saleProduct.Packaging = _productPackagingRepository.GetById(dto.ProductPackagingMasterId);
           return saleProduct;
       }

       public VATClass Map(VATClassDTO dto)
       {
           if (dto == null) return null;
           var vatClass = Mapper.Map<VATClassDTO, VATClass>(dto);
           vatClass.VATClassItems = dto.VatClassItems.Select(n => Map(n, n.MasterId)).ToList();
           return vatClass;
       }

       public VATClass.VATClassItem Map(VatClassItemDTO dto, Guid id)
       {
           if (dto == null) return null;
           var vatClassItem = Mapper.Map<VatClassItemDTO, VATClass.VATClassItem>(dto);
           return vatClassItem;
       }

       public User Map(UserDTO dto)
       {
           if (dto == null) return null;
           var user = Mapper.Map<UserDTO, User>(dto);
           user.Group = _userGroupRepository.GetById(dto.GroupMasterId);
           return user;
       }

       # region Discounts Mappings
       public SaleValueDiscount.SaleValueDiscountItem Map(SaleValueDiscountItemDTO dto, Guid id)
       {
           if (dto == null) return null;
           var discountItem = Mapper.Map<SaleValueDiscountItemDTO, SaleValueDiscount.SaleValueDiscountItem>(dto);
           discountItem._Status = EntityStatus.New;
           return discountItem;
       }

       public SaleValueDiscount Map(SaleValueDiscountDTO dto)
       {
           if (dto == null) return null;
           var saleValueDiscount = Mapper.Map<SaleValueDiscountDTO, SaleValueDiscount>(dto);
           saleValueDiscount.Tier = _pricingTierRepository.GetById(dto.TierMasterId);
           saleValueDiscount.DiscountItems = dto.DiscountItems.Select(n => Map(n, n.MasterId)).ToList();
           return saleValueDiscount;
       }

       public ProductDiscount.ProductDiscountItem Map(ProductDiscountItemDTO dto, Guid id)
       {
           if (dto == null) return null;
           var productDiscountItem = Mapper.Map<ProductDiscountItemDTO, ProductDiscount.ProductDiscountItem>(dto);
           productDiscountItem._Status = EntityStatus.New;
           return productDiscountItem;
       }

       public ProductDiscount Map(ProductDiscountDTO dto)
       {
           if (dto == null) return null;
           var productDiscount = Mapper.Map<ProductDiscountDTO, ProductDiscount>(dto);
           productDiscount.Tier = _pricingTierRepository.GetById(dto.TierMasterId);
           productDiscount.ProductRef = new ProductRef {ProductId = dto.ProductMasterId};
           productDiscount.DiscountItems = dto.DiscountItem.Select(n => Map(n, n.MasterId)).ToList();
           return productDiscount;
       }

       public PromotionDiscount.PromotionDiscountItem Map(PromotionDiscountItemDTO dto, Guid id)
       {
           if (dto == null) return null;
           var item = Mapper.Map<PromotionDiscountItemDTO, PromotionDiscount.PromotionDiscountItem>(dto);
           item.FreeOfChargeProduct = new ProductRef {ProductId = dto.ProductMasterId};
           item._Status = EntityStatus.New; 
           return item;
       }

       public PromotionDiscount Map(PromotionDiscountDTO dto)
       {
           if (dto == null) return null;
           var promotionDiscount = Mapper.Map<PromotionDiscountDTO, PromotionDiscount>(dto);
           promotionDiscount.ProductRef = new ProductRef {ProductId = dto.ProductMasterId};
           promotionDiscount.PromotionDiscountItems = dto.PromotionDiscountItems.Select(n => Map(n, n.MasterId)).ToList();
           return promotionDiscount;
       }

       public CertainValueCertainProductDiscount.CertainValueCertainProductDiscountItem Map(CertainValueCertainProductDiscountItemDTO dto, Guid id)
       {
           if (dto == null) return null;
           var cvcpItem =
               Mapper.Map<CertainValueCertainProductDiscountItemDTO, CertainValueCertainProductDiscount.CertainValueCertainProductDiscountItem>(dto);
           cvcpItem.Product = new ProductRef {ProductId = dto.ProductMasterId};
           cvcpItem._Status = EntityStatus.New;
           return cvcpItem;
       }

       public CertainValueCertainProductDiscount Map(CertainValueCertainProductDiscountDTO dto)
       {
           if (dto == null) return null;
           var cvcp = Mapper.Map<CertainValueCertainProductDiscountDTO, CertainValueCertainProductDiscount>(dto);
           cvcp.CertainValueCertainProductDiscountItems = dto.CertainValueCertainProductDiscountItems.Select(n => Map(n, n.MasterId)).ToList();
           return cvcp;
       }

     

       public ProductGroupDiscount Map(ProductGroupDiscountDTO dto)
       {
           if (dto == null) return null;
           var productGroupDiscount = Mapper.Map<ProductGroupDiscountDTO, ProductGroupDiscount>(dto);
           productGroupDiscount.GroupDiscount = _discountGroupRepository.GetById(dto.DiscountGroupMasterId);
           productGroupDiscount.DiscountRate = dto.DiscountRate;
           productGroupDiscount.EffectiveDate = dto.EffectiveDate;
           productGroupDiscount.EndDate = dto.EndDate;
           productGroupDiscount.Quantity = dto.Quantity;
           productGroupDiscount.Product = new ProductRef() {ProductId = dto.ProductMasterId};
           return productGroupDiscount;
       }
       public ProductGroupDiscount MapProductGroupDiscount(ProductGroupDiscountDTO dto)
       {
           if (dto == null) return null;
           var productGroupDiscount = new ProductGroupDiscount(dto.MasterId)
                                          {
                                              GroupDiscount =
                                                  _discountGroupRepository.GetById(dto.DiscountGroupMasterId),
                                              _DateCreated = dto.DateCreated,
                                              _DateLastUpdated = dto.DateLastUpdated,
                                              _Status = (EntityStatus) dto.StatusId,
                                          };


           productGroupDiscount.DiscountRate = dto.DiscountRate;
           productGroupDiscount.EffectiveDate = dto.EffectiveDate;
           productGroupDiscount.EndDate = dto.EndDate;
           productGroupDiscount.Product = new ProductRef() {ProductId = dto.ProductMasterId};
                          
        
           return productGroupDiscount;
       }
       #endregion

       public TargetPeriod Map(TargetPeriodDTO dto)
       {
           if (dto == null) return null;
           var targetPeriod = Mapper.Map<TargetPeriodDTO, TargetPeriod>(dto);
           return targetPeriod;
       }

       public Target Map(TargetDTO dto)
       {
           if (dto == null) return null;
           var target = Mapper.Map<TargetDTO, Target>(dto);
           target.CostCentre = _costCentreRepository.GetById(dto.CostCentreId);
           target.TargetPeriod = _targetPeriodRepository.GetById(dto.TargetPeriodMasterId);
           return target;
       }

       public Province Map(ProvinceDTO dto)
       {
           if (dto == null) return null;
           var province = Mapper.Map<ProvinceDTO, Province>(dto);
           province.Country = _countryRepository.GetById(dto.CountryMasterId);
           return province;
       }

       public District Map(DistrictDTO dto)
       {
           if (dto == null) return null;
           var district = Mapper.Map<DistrictDTO, District>(dto);
           district.Province = _provincesRepository.GetById(dto.ProvinceMasterId);
           return district;
       }

       public ReOrderLevel Map(ReorderLevelDTO dto)
       {
           if (dto == null) return null;
           var reorderLevel = Mapper.Map<ReorderLevelDTO, ReOrderLevel>(dto);
           reorderLevel.DistributorId = _costCentreRepository.GetById(dto.DistributorMasterId);
           reorderLevel.ProductId = _productRepository.GetById(dto.ProductMasterId);
           return reorderLevel;
       }

       public AssetType Map(AssetTypeDTO dto)
       {
           if (dto == null) return null;
           var assetType = Mapper.Map<AssetTypeDTO, AssetType>(dto);
           return assetType;
       }

       public Asset Map(AssetDTO dto)
       {
           if (dto == null) return null;
           var asset = Mapper.Map<AssetDTO, Asset>(dto);
           asset.AssetCategory = _assetCategoryRepository.GetById(dto.AssetCategoryMasterId);
           asset.AssetStatus = _assetStatusRepository.GetById(dto.AssetStatusMasterId);
           asset.AssetType = _assetTypeRepository.GetById(dto.AssetTypeMasterId);
           return asset;
       }

       public Competitor Map(CompetitorDTO dto)
       {
           if (dto == null) return null;
           var competitor = Mapper.Map<CompetitorDTO, Competitor>(dto);
           return competitor;
       }

       public CompetitorProducts Map(CompetitorProductDTO dto)
       {
           if (dto == null) return null;
           var competitorProduct = Mapper.Map<CompetitorProductDTO, CompetitorProducts>(dto);
           competitorProduct.Competitor = _competitorRepository.GetById(dto.CompetitorMasterId);
           competitorProduct.Brand = _productBrandRepository.GetById(dto.BrandMasterId);
           competitorProduct.Packaging = _productPackagingRepository.GetById(dto.PackagingMasterId);
           competitorProduct.ProductType = _productTypeRepository.GetById(dto.ProductTypeMasterId);
           competitorProduct.Flavour = _productFlavourRepository.GetById(dto.FlavourMasterId);
           competitorProduct.PackagingType = _productPackagingTypeRepository.GetById(dto.PackagingTypeMasterId);
           return competitorProduct;
       }

       public ChannelPackaging Map(ChannelPackagingDTO dto)
       {
           if (dto == null) return null;
           var channelPackaging = Mapper.Map<ChannelPackagingDTO, ChannelPackaging>(dto);
           channelPackaging.OutletType = _outletTypeRepository.GetById(dto.OutletTypeMasterId);
           channelPackaging.Packaging = _productPackagingRepository.GetById(dto.ProductPackagingMasterId);
           return channelPackaging;
       }

       public DiscountGroup Map(DiscountGroupDTO dto)
       {
           if (dto == null) return null;
           var discountGroup = Mapper.Map<DiscountGroupDTO, DiscountGroup>(dto);
           return discountGroup;
       }

       public FreeOfChargeDiscount Map(FreeOfChargeDiscountDTO dto)
       {
           if (dto == null) return null;
           var freeOfChargeDiscount = Mapper.Map<FreeOfChargeDiscountDTO, FreeOfChargeDiscount>(dto);
           freeOfChargeDiscount.ProductRef = new ProductRef {ProductId = dto.ProductRefMasterId};
           return freeOfChargeDiscount;
       }

       public SalesmanRoute Map(SalesmanRouteDTO dto)
       {
           if (dto == null) return null;
           var salesmanRoute = Mapper.Map<SalesmanRouteDTO, SalesmanRoute>(dto);
           salesmanRoute.Route = _routeRepository.GetById(dto.RouteMasterId);
           return salesmanRoute;
       }

       public SalesmanSupplier Map(SalesmanSupplierDTO dto)
       {
           if (dto == null) return null;
           var salesmanSupplier = Mapper.Map<SalesmanSupplierDTO, SalesmanSupplier>(dto);
           salesmanSupplier.Supplier = _supplierRepository.GetById(dto.SupplierMasterId);
           return salesmanSupplier;
       }

       public UserGroup Map(UserGroupDTO dto)
       {
           if (dto == null) return null;
           var userGroup = Mapper.Map<UserGroupDTO, UserGroup>(dto);
           return userGroup;
       }

       public UserGroupRoles Map(UserGroupRoleDTO dto)
       {
           if (dto == null) return null;
           var userGroupRole = Mapper.Map<UserGroupRoleDTO, UserGroupRoles>(dto);
           userGroupRole.UserGroup = _userGroupRepository.GetById(dto.UserGroupMasterId);
           return userGroupRole;
       }

       public Bank Map(BankDTO dto)
       {
           if (dto == null) return null;
           var bank = Mapper.Map<BankDTO, Bank>(dto);
           return bank;
       }

       public BankBranch Map(BankBranchDTO dto)
       {
           if (dto == null) return null;
           var bankBranch = Mapper.Map<BankBranchDTO, BankBranch>(dto);
           bankBranch.Bank = _bankRepository.GetById(dto.BankMasterId);
           return bankBranch;
       }

       public Supplier Map(SupplierDTO dto)
       {
           if (dto == null) return null;
           var supplier = Mapper.Map<SupplierDTO, Supplier>(dto);
           return supplier;
       }

       public ContactType Map(ContactTypeDTO dto)
       {
           if (dto == null) return null;
           var contactType = Mapper.Map<ContactTypeDTO, ContactType>(dto);
           return contactType;
       }

       public MaritalStatus Map(MaritalStatusDTO dto)
       {
           if (dto == null) return null;
           var maritalStatus = Mapper.Map<MaritalStatusDTO, MaritalStatus>(dto);
           return maritalStatus;
       }

       public AssetCategory Map(AssetCategoryDTO dto)
       {
           if (dto == null) return null;
           var assetCategory = Mapper.Map<AssetCategoryDTO, AssetCategory>(dto);
           assetCategory.AssetType = _assetTypeRepository.GetById(dto.AssetTypeMasterId);
           return assetCategory;
       }

       public AssetStatus Map(AssetStatusDTO dto)
       {
           if (dto == null) return null;
           var assetStatus = Mapper.Map<AssetStatusDTO, AssetStatus>(dto);
           return assetStatus;
       }

       public OutletVisitDay Map(OutletVisitDayDTO dto)
       {
           if (dto == null) return null;
           var outletVisitDay = Mapper.Map<OutletVisitDayDTO, OutletVisitDay>(dto);
           return outletVisitDay;
       }

       public OutletPriority Map(OutletPriorityDTO dto)
       {
           if (dto == null) return null;
           var outletPriority = Mapper.Map<OutletPriorityDTO, OutletPriority>(dto);
           outletPriority.Route = _routeRepository.GetById(dto.RouteMasterId);
           return outletPriority;
       }

       public TargetItem Map(TargetItemDTO dto)
       {
           if (dto == null) return null;
           var targetItem = Mapper.Map<TargetItemDTO, TargetItem>(dto);
           targetItem.Target = _targetRepository.GetById(dto.TargetMasterId);
           return targetItem;
       }

       public AppSettings Map(AppSettingsDTO dto)
       {
           if (dto == null) return null;
           var appSettings = Mapper.Map<AppSettingsDTO, AppSettings>(dto);
           return appSettings;
       }

       public Inventory Map(InventoryDTO dto)
       {
           if (dto == null) return null;
           var inventory = Mapper.Map<InventoryDTO, Inventory>(dto);
           inventory.Warehouse = (Warehouse) _costCentreRepository.GetById(dto.WarehouseMasterID);
           inventory.Product = _productRepository.GetById(dto.ProductMasterID);
           return inventory;
       }

       public PaymentTracker Map(PaymentTrackerDTO dto)
       {
           if (dto == null) return null;
           var paymentTracker = Mapper.Map<PaymentTrackerDTO, PaymentTracker>(dto);
           return paymentTracker;
       }

       public RetireDocumentSetting Map(RetireSettingDTO dto)
       {
           if (dto == null) return null;
           var retireSetting =  Mapper.Map<RetireSettingDTO, RetireDocumentSetting>(dto);
           return retireSetting;
       }

       public CommodityType Map(CommodityTypeDTO dto)
       {
           if (dto == null) return null;
           var commodityType = Mapper.Map<CommodityTypeDTO, CommodityType>(dto);
           return commodityType;
       }

       public Commodity Map(CommodityDTO dto)
       {
           if (dto == null) return null;
           var commodity = Mapper.Map<CommodityDTO, Commodity>(dto);
           commodity.CommodityType = _commodityTypeRepository.GetById(dto.CommodityTypeId);
           if(dto.CommodityGrades!=null)
                commodity.CommodityGrades = dto.CommodityGrades.Select(n => Map(n, dto.MasterId)).ToList();
           return commodity;
       }

       public CommodityGrade Map(CommodityGradeDTO dto, Guid commodityId)
       {
           if (dto == null) return null;
           var grade = Mapper.Map<CommodityGradeDTO, CommodityGrade>(dto);
           grade.Commodity = new Commodity(commodityId);
           return grade;
       }

       public CommodityOwnerType Map(CommodityOwnerTypeDTO dto)
       {
           if (dto == null) return null;
           var commodityOwnerType = Mapper.Map<CommodityOwnerTypeDTO, CommodityOwnerType>(dto);
           return commodityOwnerType;
       }

       public CommodityProducer Map(CommodityProducerDTO dto)
       {
           if (dto == null) return null;
           var commodityProducer = Mapper.Map<CommodityProducerDTO, CommodityProducer>(dto);
           commodityProducer.CommoditySupplier = _commoditySupplierRepository.GetById(dto.CommoditySupplierId) as CommoditySupplier;
           return commodityProducer;
       }

       public CommoditySupplier Map(CommoditySupplierDTO dto)
       {
           if (dto == null) return null;
           var commoditySupplier = Mapper.Map<CommoditySupplierDTO, CommoditySupplier>(dto);
           return commoditySupplier;
       }

       public CommodityOwner Map(CommodityOwnerDTO dto)
       {
           if (dto == null) return null;
           var commodityOwner = Mapper.Map<CommodityOwnerDTO, CommodityOwner>(dto);
           commodityOwner.CommodityOwnerType = _commodityOwnerTypeRepository.GetById(dto.CommodityOwnerTypeId);
           commodityOwner.CommoditySupplier = _commoditySupplierRepository.GetById(dto.CommoditySupplierId) as CommoditySupplier;
           /*commodityOwner.MaritalStatus = _maritalStatusRepository.GetById(dto.MaritalStatusId);*/
           return commodityOwner;
       }

       public CentreType Map(CentreTypeDTO dto)
       {
           if (dto == null) return null;
           var centreType = Mapper.Map<CentreTypeDTO, CentreType>(dto);
           return centreType;
       }

       public Centre Map(CentreDTO dto)
       {
           if (dto == null) return null;
           var centre = Mapper.Map<CentreDTO, Centre>(dto);
           centre.CenterType = _centreTypeRepository.GetById(dto.CenterTypeId);
           centre.Route = _routeRepository.GetById(dto.RouteId);
           centre.Hub = _costCentreRepository.GetById(dto.HubId) as Hub;
           return centre;
       }

       public Hub Map(HubDTO dto)
       {
           if (dto == null) return null;
           var hub = Mapper.Map<HubDTO, Hub>(dto);
           hub.Region = _regionRepository.GetById(dto.RegionId);
           return hub;
       }

       public Store Map(StoreDTO dto)
       {
           if (dto == null) return null;
           var store = Mapper.Map<StoreDTO, Store>(dto);
           return store;
       }

       public PurchasingClerk Map(PurchasingClerkDTO dto)
       {
           if (dto == null) return null;
           var purchasingClerk = Mapper.Map<PurchasingClerkDTO, PurchasingClerk>(dto);
           purchasingClerk.PurchasingClerkRoutes = dto.PurchasingClerkRoutes.Select(n => Map(n, dto.MasterId)).ToList();
           purchasingClerk.User = Map(dto.UserDto);
           return purchasingClerk;
       }

       public PurchasingClerkRoute Map(PurchasingClerkRouteDTO dto, Guid clerkId)
       {
           if (dto == null) return null;
           var purchasingClerkRoute = Mapper.Map<PurchasingClerkRouteDTO, PurchasingClerkRoute>(dto);
           purchasingClerkRoute.PurchasingClerkRef = new CostCentreRef {Id = clerkId};
           purchasingClerkRoute.Route = _routeRepository.GetById(dto.RouteId);
           return purchasingClerkRoute;
       }

       public Printer Map(PrinterDTO dto)
       {
           if (dto == null) return null;
           var printer = Mapper.Map<PrinterDTO, Printer>(dto);
           printer.CostCentre = _hubRepository.GetById(dto.HubId) as Hub;
           return printer;
       }

       public WeighScale Map(WeighScaleDTO dto)
       {
           if (dto == null) return null;
           var weighScale = Mapper.Map<WeighScaleDTO, WeighScale>(dto);
           weighScale.CostCentre = _hubRepository.GetById(dto.HubId) as Hub;
           return weighScale;
       }

       public Vehicle Map(VehicleDTO dto)
       {
           if (dto == null) return null;
           var vehicle = Mapper.Map<VehicleDTO, Vehicle>(dto);
           vehicle.CostCentre = _hubRepository.GetById(dto.HubId) as Hub;
           return vehicle;
       }

       public SourcingContainer Map(SourcingContainerDTO dto)
       {
           if (dto == null) return null;
           var container = Mapper.Map<SourcingContainerDTO, SourcingContainer>(dto);
           container.CostCentre = _hubRepository.GetById(dto.HubId) as Hub;
           container.ContainerType = _containerTypeRepository.GetById(dto.ContainerTypeId);
           return container;
       }

       public ContainerType Map(ContainerTypeDTO dto)
       {
           if (dto == null) return null;
           var containerType = Mapper.Map<ContainerTypeDTO, ContainerType>(dto);
           containerType.CommodityGrade = dto.CommodityGradeId != Guid.Empty 
               ? _commodityRepository.GetGradeByGradeId(dto.CommodityGradeId) : null;
           return containerType;
       }

       public MasterDataAllocation Map(CommodityProducerCentreAllocationDTO dto)
       {
           if (dto == null) return null;
           var allocation = Mapper.Map<CommodityProducerCentreAllocationDTO, MasterDataAllocation>(dto);
           allocation.AllocationType = MasterDataAllocationType.CommodityProducerCentreAllocation;
           return allocation;
       }

       public ActivityType Map(ActivityTypeDTO dto)
       {
           if (dto == null) return null;
           var activity = Mapper.Map<ActivityTypeDTO, ActivityType>(dto);
           return activity;
       }

       public ServiceProvider Map(ServiceProviderDTO dto)
       {
           if (dto == null) return null;
           ServiceProvider p = Mapper.Map<ServiceProviderDTO, ServiceProvider>(dto);
           p.Bank = _bankRepository.GetById(dto.BankId);
           p.BankBranch = _bankBranchRepository.GetById(dto.BankBranchId);
           return p;
       }

       public Shift Map(ShiftDTO dto)
       {
           if (dto == null) return null;
           var shift = Mapper.Map<ShiftDTO, Shift>(dto);
           return shift;
       }

       public Season Map(SeasonDTO dto)
       {
           if (dto == null) return null;
           var s = Mapper.Map<SeasonDTO, Season>(dto);
           s.CommodityProducer = _commodityProducerRepository.GetById(dto.CommodityProducerId);
           return s;
       }

       public Infection Map(InfectionDTO dto)
       {
           if (dto == null) return null;
           var s = Mapper.Map<InfectionDTO, Infection>(dto);
           return s;
       }

       public CommodityProducerService Map(ServiceDTO dto)
       {
           if (dto == null) return null;
           var s = Mapper.Map<ServiceDTO, CommodityProducerService>(dto);
           return s;
       }

       public OutletVisitReasonsType Map(OutletVisitReasonTypeDTO dto)
       {
           if (dto == null) return null;
           var s = Mapper.Map<OutletVisitReasonTypeDTO, OutletVisitReasonsType>(dto);
           return s;
       }

       public MasterDataAllocation Map(RouteCentreAllocationDTO dto)
       {
           if (dto == null) return null;
           var allocation = Mapper.Map<RouteCentreAllocationDTO, MasterDataAllocation>(dto);
           allocation.AllocationType = MasterDataAllocationType.RouteCentreAllocation;
           return allocation;
       }

       public MasterDataAllocation Map(RouteRegionAllocationDTO dto)
       {
           if (dto == null) return null;
           var allocation = Mapper.Map<RouteRegionAllocationDTO, MasterDataAllocation>(dto);
           allocation.AllocationType = MasterDataAllocationType.RouteRegionAllocation;
           return allocation;
       }

       public MasterDataAllocation Map(RouteCostCentreAllocationDTO dto)
       {
           if (dto == null) return null;
           var allocation = Mapper.Map<RouteCostCentreAllocationDTO, MasterDataAllocation>(dto);
           allocation.AllocationType = MasterDataAllocationType.RouteCostCentreAllocation;
           return allocation;
       }
   }
}
