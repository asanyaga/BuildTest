using Distributr.Core.Resources.Util;
using Distributr.Core.Utility;
using Distributr.Core.Workflow;
using Distributr.HQ.Lib.Service;
using Distributr.HQ.Lib.Service.Impl;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.ApplicationSetupViewModelBuilders;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Discounts;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Discounts.Impl;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.OutletVisitReasonsTypeViewModelBuilders;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.SettingsViewBuilder;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.SettingsViewBuilder.Impl;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.UserModelBuilder.Impl;
using Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.AgriUserViewModelBuilders;
using Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.AgriUserViewModelBuilders.Impl;
using Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.CentreViewModelBuilders;
using Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.CentreViewModelBuilders.Impl;
using Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.CommodityTransferViewModelBuilders;
using Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.CommodityTransferViewModelBuilders.Impl;
using Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.CommodityViewModelBuilders;
using Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.CommodityViewModelBuilders.Impl;
using Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.CostCentreViewModelBuilders;
using Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.CostCentreViewModelBuilders.Impl;
using Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.InventoryViewModelBuilders;
using Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.InventoryViewModelBuilders.Impl;
using Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.PrinterViewModelBuilders;
using Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.PrinterViewModelBuilders.Impl;
using Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.EquipmentViewModelBuilders;
using Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.EquipmentViewModelBuilders.Impl;
using Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.SettingsViewModelBuilder;
using Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.SettingsViewModelBuilder.Impl;
using Distributr.HQ.Lib.Workflows.CommodityRelease;
using Distributr.HQ.Lib.Workflows.CommodityStorage;
using Distributr.HQ.Lib.Workflows.CommodityTransfer;
using Distributr.HQ.Lib.Workflows.PurchaseOrder;
using Distributr.WSAPI.Lib.Services;
using StructureMap.Configuration.DSL;
using Distributr.HQ.Lib.ViewModelBuilders;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Impl;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Products.Impl;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Products;

using Distributr.HQ.Lib.ViewModelBuilders.Admin.UserModelBuilder;

using Distributr.HQ.Lib.ViewModelBuilders.Admin.Outlets;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.RouteViewBuilder;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.RouteViewBuilder.Impl;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.ProducerModelBuilder;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Distributors;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Contacts;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.CostCenter.Impl;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.CostCenter;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Orders;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Orders.Impl;
using Distributr.HQ.Lib.ViewModelBuilders.DistributorTargetsViewModelBuilders;
using Distributr.HQ.Lib.ViewModelBuilders.DistributorTargetsViewModelBuilders.Impl;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.CompetitorViewModelBuilders;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.CompetitorViewModelBuilders.Impl;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.CoolerViewModelBuilders;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.CoolerViewModelBuilders.Impl;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.ReOrderLevelViewModelBuilders;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.ReOrderLevelViewModelBuilders.Impl;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.MarketAuditViewModelBuilders;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.MarketAuditViewModelBuilders.Impl;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.ChannelPackagingsViewModelBuilders;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.ChannelPackagingsViewModelBuilders.Impl;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.BankViewModelBuilders;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.BankViewModelBuilders.Impl;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Transaction;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.SuppliersViewModelBuilders;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.MaritalStatusViewModelBuilders;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.AssetViewModelBuilders;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.AssetViewModelBuilders.Impl;

namespace Distributr.HQ.Lib.IOC
{
    public class ViewModelBuilderRegistry : Registry
    {
        public ViewModelBuilderRegistry()
        {
            For<IHQMailerViewModelBuilder>().Use<HQMailerViewModelBuilder>();
            For<IAdminProductViewModelBuilder>().Use<AdminProductViewModelBuilder>();
            For<IProductFlavoursViewModelBuilder>().Use<ProductFlavoursViewModelBuilder>();
            For<IAdminProductPackagingViewModelbuilder>().Use<AdminProductPackagingViewModelBuilder>();
            For<IAdminProductPackagingTypeViewModelBuilder>().Use<AdminProductPackagingTypeViewModelBuilder>();
            For<IProductTypeViewModelBuilder>().Use<AdminProductTypeViewModelBuilder>();
            For<IProductBrandViewModelBuilder>().Use<ProductBrandViewModelBuilder>();
            For<IUserViewModelBuilder>().Use<UserViewModelBuilder>();
            //For<IUserTypeViewModelBuilder>().Use<UserTypeViewModelBuilder>();
            For<IAdminRouteViewModelBuilder>().Use<AdminRouteViewModelBuilder>();
            For<IAdminProducerViewModelBuilder>().Use<AdminProducerViewModelBuider>();
            For<IDistributorViewModelBuilder>().Use<DistributorViewModelBuilder>();
            For<IProductPricingViewModelBuilder>().Use<ProductPricingViewModelBuilder>();
            For<IOutletViewModelBuilder>().Use<OutletViewModelBuilder>();
            For<IContactViewModelBuilder>().Use<ContactViewModelBuilder>();
            For<ICountryViewModelBuilder>().Use<CountryViewModelBuilder>();
            For<IRegionViewModelBuilder>().Use<RegionViewModelBuilder>();
            For<IAreaViewModelBuilder>().Use<AreaViewModelBuilder>();
            For<ITerritoryViewModelBuilder>().Use<TerritoryViewModelBuilder>();
            For<ITransporterViewModelBuilder>().Use<TransporterViewModelBuilder>();
            For<IOutletCategoryViewModelBuilder>().Use<OutletCategoryViewModelBuilder>();
            For<IOutletTypeViewModelBuilder>().Use<OutletTypeViewModelBuilder>();
            For<ISocioEconomicStatusViewModelBuilder>().Use<SocioEconomicStatusViewModelBuilder>();
            For<IProductPricingTierViewModelBuilder>().Use<ProductPricingTierViewModelBuilder>();
            For<ICostCentreViewModelBuilder>().Use<CostCentreViewModelBuilder>();
            For<IVATClassViewModelBuilder>().Use<VATClassViewModelBuilder>();
            For<IProductViewModelBuilder>().Use<ProductViewModelBuilder>();
            For<IListVatClassViewModelBuilder>().Use<ListVatClassViewModelBuilder>();
            For<ICreateVatClassViewModelBuilder>().Use<CreateVatClassViewModelBuilder>();
            For<IEditVatClassViewModelBuilder>().Use<EditVatClassViewModelBuilder>();
            For<IEditProductPricingViewModelBuilder>().Use<EditProductPricingViewModelBuilder>();
            For<IProducerViewModelBuilder>().Use<ProducerViewModelBuilder>();
            For<IListOrdersViewModelBuilder>().Use<ListOrdersViewModelBuilder>();
            For<IApproveOrderViewModelBuilder>().Use<ApproveOrderViewModelBuilder>();
            For<IPurchaseOrderViewModelBuilder>().Use<PurchaseOrderViewModelBuilder>();
            For<IVatClassLineItemViewModelBuilder>().Use<VatClassLineItemViewModelBuilder>();
            For<ITargetPeriodViewModelBuilder>().Use<TargetPeriodViewModelBuilder>();
            For<IProvinceViewModelBuilder>().Use<ProvinceViewModelBuilder>();
            For<IDistrictViewModelBuilder>().Use<DistrictViewModelBuilder>();
            For<ICompetitorViewModelBuilder>().Use<CompetitorViewModelBuilder>();
            For<ICompetitorProductsViewModelBuilder>().Use<CompetitorProductsViewModelBuilder>();
            For<ICoolerTypeViewModelBuilder>().Use<CoolerTypeViewModelBuilder>();
            For<ICoolerViewModelBuilder>().Use<CoolerViewModelBuilder>();
            For<IReOrderLevelViewModelBuilder>().Use<ReOrderLevelViewModelBuilder>();
            For<IMarketAuditViewModelBuilder>().Use<MarketAuditViewModelBuilder>();
            For<IOutletAuditViewModelBuilder>().Use<OutletAuditViewModelBuilder>();
            //For<IReturnablesViewModelBuilder >().Use<ReturnablesViewModelBuilders >();
            //For<IShellsViewModelBuilder >().Use<ShellsViewModelBuilder >();
            For<IChannelPackagingViewModelBuilder>().Use<ChannelPackagingViewModelBuilder>();
            For<IAddPricingLineItemsViewModelBuilder>().Use<AddPricingLineItemsViewModelBuilder>();
            For<IProductDiscountViewModelBuilder>().Use<ProductDiscountViewModelBuilder>();
            For<ISaleValueDiscountViewModelBuilder>().Use<SaleValueDiscountViewModelBuilder>();
          
            For<ICustomerDiscountViewModelBuilder>().Use<CustomerDiscountViewModelBuilder>();
            For<IPromotionDiscountViewModelBuilder>().Use<PromotionDiscountViewModelBuilder>();
            For<IDiscountGroupViewModelBuilder>().Use<DiscountGroupViewModelBuilder>();
            For<IProductGroupDiscountViewModelBuilder>().Use<ProductGroupDiscountViewModelBuilder>();
            For<ICertainValueCertainProductDiscountViewModelBuilder>().Use<CertainValueCertainProductDiscountViewModelBuilder>();
            For<IFreeOfChargeDiscountViewModelBuilder>().Use<FreeOfChargeDiscountViewModelBuilder>();
            For<IUserGroupVeiwModelBuilder>().Use<UserGroupVeiwModelBuilder>();
            For<IUserGroupRoleVeiwModelBuilder>().Use<UserGroupRoleVeiwModelBuilder>();
            For<IBankViewModelBuilder>().Use<BankViewModelBuilder>();
            For<IBankBranchViewModelBuilder>().Use<BankBranchViewModelBuilder>();
            For<IAuditLogViewModelBuilder>().Use<AuditLogViewModelBuilder>();
           
            For<ISupplierViewModelBuilder>().Use<SupplierViewModelBuilder>();
            For<IMaritalStatusViewModelBuilder>().Use<MaritalStatusViewModelBuilder>();
            For<IContactTypeViewModelBuilder>().Use<ContactTypeViewModelBuilder>();
            For<IProductPackagingSummaryViewBuilder>().Use<ProductPackagingSummaryViewBuilder>();
            For<IAssetTypeViewModelBuilder>().Use<AssetTypeViewModelBuilder>();
            For<IAssetStatusViewModelBuilder>().Use<AssetStatusViewModelBuilder>();
            For<IAssetCategoryViewModelBuilder>().Use<AssetCategoryViewModelBuilder>();
            For<IAssetViewModelBuilder>().Use<AssetViewModelBuilder>();
            For<IDiscountProcessorService>().Use<DiscountProcessorService>();
            For<EntityUsage>().Use<EntityUsage>();
            For<ICommodityTypeViewModelBuilder>().Use<CommodityTypeViewModelBuilder>();
            For<ICommodityProducerViewModelBuilder>().Use<CommodityProducerViewModelBuilder>();
            For<ICommodityOwnerTypeViewModelBuilder>().Use<CommodityOwnerTypeViewModelBuilder>();
            For<IHubViewModelBuilder>().Use<HubViewModelBuilder>();
            For<ICommodityViewModelBuilder>().Use<CommodityViewModelBuilder>();
            For<IAgriUserViewModelBuilder>().Use<AgriUserViewModelBuilder>();
            For<ICentreTypeViewModelBuilder>().Use<CentreTypeViewModelBuilder>();
            For<ICentreViewModelBuilder>().Use<CentreViewModelBuilder>();
            For<ICommoditySupplierViewModelBuilder>().Use<CommoditySupplierViewModelBuilder>();
            For<ICommodityOwnerViewModelBuilder>().Use<CommodityOwnerViewModelBuilder>();
            For<IStoreViewModelBuilder>().Use<StoreViewModelBuilder>();
            For<ISourcingContainerViewModelBuilder>().Use<SourcingContainerViewModelBuilder>();
            For<IPrinterViewModelBuilder>().Use<PrinterViewModelBuilder>();
            For<IWeighScaleViewModelBuilder>().Use<WeighScaleViewModelBuilder>();
         
            For<ISettingsViewModelBuilder>().Use<SettingsViewModelBuilder>();
          
            For<IRetireSettingViewModelBuilder>().Use<RetireSettingViewModelBuilder>();
            For<IInventoryViewModelBuilder>().Use<InventoryViewModelBuilder>();
           
            For<IPurchaseOrderWorkflow>().Use<PurchaseOrderHqWorkflow>();
            For<IContainerTypeViewModelBuilder>().Use<ContainerTypeViewModelBuilder>();
            For<IVehicleViewModelBuilder>().Use<VehicleViewModelBuilder>();
            For<IAgrimanagrSettingsViewModelBuilder>().Use<AgrimanagrSettingsViewModelBuilder>();
            For<IApplicationSetupViewModelBuilder>().Use<ApplicationSetupViewModelBuilder>();
            For<IProductPackagingSummaryClient>().Use<ProductPackagingSummaryClient>();
            For<ICommodityTransferViewModelBuilder>().Use<CommodityTransferViewModelBuilder>();
            For<ICommodityTransferDetailViewModelBuilder>().Use<CommodityTransferDetailViewModelBuilder>();
            For<ICommodityTransferWFManager>().Use<CommodityTransferHqWorkFlow>();
            For<ICommodityTransferStoreAssignmentViewModelBuilder>().Use
                <CommodityTransferStoreAssignmentViewModelBuilder>();

            For<IOutletVisitReasonsTypeViewModelBuilder>().Use<OutletVisitReasonsTypeViewModelBuilder>();
            For<ICommodityStorageWFManager>().Use<CommodityStorageHQWFManager>();
            For<IMessageSourceAccessor>().Use(MessageSourceAccessor.GetInstance("web"));
            

            For<ICommodityTransferService>().Use<CommodityTransferService>();
            For<ICommodityReleaseWFManager>().Use<CommodityReleaseHQWFManager>();

          
        }
    }
}
