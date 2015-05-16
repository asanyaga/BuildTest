using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.FinancialEntities;
using Distributr.Core.Domain.InventoryEntities;
using Distributr.Core.Domain.Master.AssetEntities;
using Distributr.Core.Domain.Master.SettingsEntities;
//using Distributr.Core.MasterDataDTO;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Master.ChannelPackagings;
using Distributr.Core.Domain.Master.CompetitorManagement;
using Distributr.Core.Domain.Master.CoolerEntities;
using Distributr.Core.Domain.Master.ReOrdeLevelEntities;
using Distributr.Core.Domain.Master.DistributorTargetEntities;
using Distributr.Core.Domain.Master.BankEntities;
using Distributr.Core.Domain.Master.SuppliersEntities;
using Distributr.Core.MasterDataDTO.DTOModels.FinancialDTO;
using Distributr.Core.MasterDataDTO.DTOModels.InventoriesDTO;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Assets;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Banks;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.ChannelPackaging;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Competitor;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.CostCentre;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.DistributorTargets;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.MaritalStatus;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Product;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Retire;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Settings;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Suppliers;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.User;

//using Distributr.WSAPI.Lib.DTOModels.MasterDataDTO.CostCentre;


namespace Distributr.WSAPI.Lib.Services.Mapping
{
    public interface IMasterDataToDTOMapping
    {
        //ProductDTO Map(Product product);
        ProductPricingTierDTO Map(ProductPricingTier productPricingTier);
        ProductBrandDTO Map(ProductBrand productBrand);
        ConsolidatedProductDTO Map(ConsolidatedProduct consolidatedProduct);
        ProductFlavourDTO Map(ProductFlavour productFlavour);
        ProductPackagingDTO Map(ProductPackaging productPackaging);
        ProductPackagingTypeDTO Map(ProductPackagingType productPackagingType);
        ProductPricingDTO Map(ProductPricing productPricing);
        ProductPricingItemDTO Map(ProductPricing.ProductPricingItem productPricingItem);
        ProductTypeDTO Map(ProductType productType);
        ReturnableProductDTO Map(ReturnableProduct returnableProduct);
        SaleProductDTO Map(SaleProduct saleProduct);
        VATClassDTO Map(VATClass vatClass);
        ConsolidatedProductProductDetailDTO Map(ConsolidatedProduct.ProductDetail consolidatedProductDetails, Guid consolidationProductId);


        AreaDTO Map(Area area);
        //CostCentreDTO Map(CostCentre costCentre);
        CountryDTO Map(Country country);
        DistributorDTO Map(Distributor distributor);
        OutletCategoryDTO Map(OutletCategory outletCategory);
        OutletDTO Map(Outlet outlet);
        OutletTypeDTO Map(OutletType outletType);
        RegionDTO Map(Region region);
        SocioEconomicStatusDTO Map(SocioEconomicStatus socioEconomicStatus);
        ProducerDTO Map(Producer Producer);
       // StandardWarehouseDTO Map(StandardWarehouse standardWarehouse);
        TerritoryDTO Map(Territory territory);
        UserDTO Map(User user);
        ContactDTO Map(Contact contact);
        RouteDTO Map(Route route);

        CostCentreApplicationDTO Map(CostCentreApplication costCentreApplication);

        DistributorSalesmanDTO Map(DistributorSalesman distributorSalesman);

        DistributorPendingDispatchWarehouseDTO Map(DistributorPendingDispatchWarehouse distributorPendingDispatchWarehouse);
        ChannelPackagingDTO Map(ChannelPackaging channelPackaging);
      //ChannelPacksDTO  Map(ChannelPackaging  channelPackaging);
        CompetitorDTO  Map(Competitor  competitor);
        CompetitorProductDTO  Map(CompetitorProducts  competitorProduct);
        AssetDTO  Map(Asset  cooler);
        AssetTypeDTO  Map(AssetType  coolerType);
        DistrictDTO  Map(District  district);
        ProvinceDTO  Map(Province  province);
        ReorderLevelDTO   Map(ReOrderLevel  reorderLevel);
        //ReturnablesDTO  Map(Returnables  returnables);
       // ShellsDTO  Map(Shells  shells);
        TargetPeriodDTO  Map(TargetPeriod  targetPeriod);
        TargetDTO  Map(Target  target);
        SaleValueDiscountDTO Map(SaleValueDiscount saleValueDiscount);
        ProductDiscountDTO Map(ProductDiscount productDiscount);
        DiscountGroupDTO Map(DiscountGroup discountGroup);
        PromotionDiscountDTO Map(PromotionDiscount promotionDiscount);
        ProductGroupDiscountDTO Map(ProductGroupDiscount productGroupDiscount);
        CertainValueCertainProductDiscountDTO Map(CertainValueCertainProductDiscount certainValueCertainProductDiscount);
        FreeOfChargeDiscountDTO Map(FreeOfChargeDiscount focDiscount);

        ContainmentDTO Map(Containment containment);
        SalesmanRouteDTO Map(SalesmanRoute salesmanRoute);
        UserGroupDTO Map(UserGroup group);
        UserGroupRoleDTO Map(UserGroupRoles role);
        BankDTO Map(Bank bank);
        BankBranchDTO Map(BankBranch bankBranch);
        SupplierDTO Map(Supplier supplier);
        MaritalStatusDTO Map(MaritalStatus maritalStatus);
        ContactTypeDTO Map(ContactType contactType);
        AssetCategoryDTO Map(AssetCategory category);
        AssetStatusDTO Map(AssetStatus status);
        OutletPriorityDTO Map(OutletPriority priority);
        OutletVisitDayDTO Map(OutletVisitDay priority);
        TargetItemDTO Map(TargetItem targetItem);
        AppSettingsDTO Map(AppSettings setting);
        InventoryDTO Map(Inventory inventory);
        PaymentTrackerDTO Map(PaymentTracker paymentTracker);
        RetireSettingDTO Map(RetireDocumentSetting setting);
        //InventorySerialsDTO Map(InventorySerials invSerials);
        SalesmanSupplierDTO Map(SalesmanSupplier salesmanSupplier);
    }
}
