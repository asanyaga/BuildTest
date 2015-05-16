using Distributr.Core.Data.Repository.CompetitorManagement;
using Distributr.Core.Data.Repository.Financials;
using Distributr.Core.Data.Repository.InventoryRepository;
using Distributr.Core.Data.Repository.MasterData.Agrimanagr;
using Distributr.Core.Data.Repository.MasterData.AssetRepositories;
using Distributr.Core.Data.Repository.MasterData.BankRepositories;
using Distributr.Core.Data.Repository.MasterData.CentreRepositories;
using Distributr.Core.Data.Repository.MasterData.ChannelPackagings;
using Distributr.Core.Data.Repository.MasterData.CommodityOwnerrepositories;
using Distributr.Core.Data.Repository.MasterData.CommodityRepositories;
using Distributr.Core.Data.Repository.MasterData.CoolerTypeRepositories;
using Distributr.Core.Data.Repository.MasterData.CostCentreRepositories;
using Distributr.Core.Data.Repository.MasterData.DistributorTargetRepositories;
using Distributr.Core.Data.Repository.MasterData.EquipmentRepository;
using Distributr.Core.Data.Repository.MasterData.MarketAuditRepositories;
using Distributr.Core.Data.Repository.MasterData.MasterDataAllocationRepositories;
using Distributr.Core.Data.Repository.MasterData.OutletVisitReasonsTypeRepositories;
using Distributr.Core.Data.Repository.MasterData.ProductRepositories;
using Distributr.Core.Data.Repository.MasterData.SettingsRepositories;
using Distributr.Core.Data.Repository.MasterData.SuppliersRepositories;
using Distributr.Core.Data.Repository.MasterData.UserRepository;
using Distributr.Core.Data.Repository.ReOrderLevelRepository;
using Distributr.Core.Data.Repository.Transactional.AuditLogRepositories;
using Distributr.Core.Data.Repository.Transactional.DocumentRepositories;
using Distributr.Core.Data.Repository.Transactional.RecollectionRepositories;
using Distributr.Core.Data.Repository.Transactional.SourcingDocumentRepositories;
using Distributr.Core.Data.Repository.Transactional.ThirdPartyIntegrationRepository;
using Distributr.Core.Data.Utility;
using Distributr.Core.Data.Utility.Caching;
using Distributr.Core.Factory.ActivityDocuments;
using Distributr.Core.Factory.ActivityDocuments.Impl;
using Distributr.Core.Factory.Documents;
using Distributr.Core.Factory.Documents.Impl;
using Distributr.Core.Factory.Master;
using Distributr.Core.Factory.Master.Impl;
using Distributr.Core.Factory.SourcingDocuments;
using Distributr.Core.Factory.SourcingDocuments.Impl;
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
using Distributr.Core.Repository.Master.MarketAuditRepositories;
using Distributr.Core.Repository.Master.MasterDataAllocationRepositories;
using Distributr.Core.Repository.Master.OutletVisitReasonsTypeRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Master.ReOrderLevelRepository;
using Distributr.Core.Repository.Master.SettingsRepositories;
using Distributr.Core.Repository.Master.SuppliersRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Transactional.AuditLogRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.CreditNoteRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.DisbursementRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.DispatchRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IIntegrationDocumentRepository;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IInvoiceRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.InventoryRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.PaymentNoteRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.ReceiptInventories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.ReturnsRepositories;
using Distributr.Core.Repository.Transactional.RecollectionRepositories;
using Distributr.Core.Repository.Transactional.SourcingDocumentRepositories;
using Distributr.Core.Repository.Transactional.ThirdPartyIntegrationRepository;
using Distributr.Core.Utility;
using Distributr.WPF.Lib.Data.EF;
using Distributr.WPF.Lib.Data.Repository.AuditLog;
using Distributr.WPF.Lib.Data.Repository.Commands;

using Distributr.WPF.Lib.Data.Repository.Payment.Request;
using Distributr.WPF.Lib.Data.Repository.Payment.Response;
using Distributr.WPF.Lib.Data.Repository.Utility;
using Distributr.WPF.Lib.Impl.Repository.Transactional.AuditLog;

using Distributr.WPF.Lib.Impl.Repository.Utility;
using Distributr.WPF.Lib.Services.Repository.Payment;
using Distributr.WPF.Lib.Services.Service.CommandQueues;
using Distributr.WPF.Lib.Services.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Utility.Mapping;
using Distributr.Core.Utility.Mapping.impl;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Setup;

namespace Distributr.WPF.Lib.Data.IOC.WPFDefaultServices
{
    public class WPFRepositoryDefaultServices
    {
        /// <summary>
        /// Default Repository service list to be registered in an IOC container
        /// </summary>
        /// <returns></returns>
        public static List<Tuple<Type, Type>> DefaultServiceList()
        {
            var serviceList = new List<Tuple<Type, Type>>
                                  {
                                      Tuple.Create(typeof (ICostCentreRepository), typeof (CostCentreRepository)),
                                      Tuple.Create(typeof (ICostCentreFactory), typeof (CostCentreFactory)),
                                      Tuple.Create(typeof (IProductRepository), typeof (ProductRepository)),
                                      Tuple.Create(typeof (IProductPricingFactory), typeof (ProductPricingFactory)),
                                      Tuple.Create(typeof (IProductDiscountFactory), typeof (ProductDiscountFactory)),
                                      Tuple.Create(typeof (ISaleValueDiscountFactory), typeof (SaleValueDiscountFactory)),
                                      Tuple.Create(typeof (IVATClassFactory), typeof (VatClassFactory)),
                                      Tuple.Create(typeof (IProductFactory), typeof (ProductFactory)),
                                      Tuple.Create(typeof (IRouteFactory), typeof (RouteFactory)),
                                      Tuple.Create(typeof (IDocumentFactory), typeof (DocumentFactory)),
                                      Tuple.Create(typeof (IInventoryAdjustmentNoteFactory),typeof (InventoryAdjustmentNoteFactory)),
                                      Tuple.Create(typeof (IInventoryReceivedNoteFactory),typeof (InventoryReceivedNoteFactory)),
                                      Tuple.Create(typeof (IInvoiceFactory), typeof (InvoiceFactory)),
                                      Tuple.Create(typeof (IOrderFactory), typeof (OrderFactory)),
                                      Tuple.Create(typeof (IDispatchNoteFactory), typeof (DispatchNoteFactory)),
                                      Tuple.Create(typeof (IInventoryTransferNoteFactory),typeof (InventoryTransferNoteFactory)),
                                      Tuple.Create(typeof (ICustomerDiscountFactory), typeof (CustomerDiscountFactory)),
                                      Tuple.Create(typeof (IPromotionDiscountFactory), typeof (PromotionDiscountFactory)),
                                      Tuple.Create(typeof (IProductDiscountGroupFactory),typeof (ProductDiscountGroupFactory)),
                                      Tuple.Create(typeof (ICertainValueCertainProductDiscountFactory),typeof (CertainValueCertainProductDiscountFactory)),
                                      Tuple.Create(typeof (IContainmentRepository), typeof (ContainmentRepository)),
                                      Tuple.Create(typeof (IPaymentNoteRepository), typeof (PaymentNoteRepository)),
                                      Tuple.Create(typeof (IProductBrandRepository), typeof (ProductBrandRepository)),
                                      Tuple.Create(typeof (IAreaRepository), typeof (AreaRepository)),
                                      Tuple.Create(typeof (IContactRepository), typeof (ContactRepository)),
                                      Tuple.Create(typeof (ICountryRepository), typeof (CountryRepository)),
                                      Tuple.Create(typeof (IDistributorRepository), typeof (DistributorRepository)),
                                      Tuple.Create(typeof (IProducerRepository), typeof (ProducerRepository)),
                                      Tuple.Create(typeof (ITransporterRepository), typeof (TransporterRepository)),
                                      Tuple.Create(typeof (IOutletRepository), typeof (OutletRepository)),
                                      Tuple.Create(typeof (IOutletCategoryRepository), typeof (OutletCategoryRepository)),
                                      Tuple.Create(typeof (IOutletTypeRepository), typeof (OutletTypeRepository)),
                                      Tuple.Create(typeof (IRouteRepository), typeof (RouteRepository)),
                                      Tuple.Create(typeof (IRegionRepository), typeof (RegionRepository)),
                                      Tuple.Create(typeof (ISocioEconomicStatusRepository),typeof (SocioEconomicStatusRepository)),
                                      Tuple.Create(typeof (ITerritoryRepository), typeof (TerritoryRepository)),
                                      Tuple.Create(typeof (IProductFlavourRepository), typeof (ProductFlavourRepository)),
                                      Tuple.Create(typeof (IProductPackagingRepository),typeof (ProductPackagingRepository)),
                                      Tuple.Create(typeof (IProductPackagingTypeRepository),typeof (ProductPackagingTypeRepository)),
                                      Tuple.Create(typeof (IProductPricingRepository), typeof (ProductPricingRepository)),
                                      Tuple.Create(typeof (IProductPricingTierRepository),typeof (ProductPricingTierRepository)),
                                      Tuple.Create(typeof (IProductTypeRepository), typeof (ProductTypeRepository)),
                                      Tuple.Create(typeof (IVATClassRepository), typeof (VATClassRepository)),
                                      Tuple.Create(typeof (IIncomingCommandQueueRepository),typeof (IncomingCommandQueueRepository)),
                                      Tuple.Create(typeof (IOutgoingCommandQueueRepository),typeof (OutgoingCommandQueueRepository)),
                                      Tuple.Create(typeof (IOutgoingCommandEnvelopeQueueRepository),typeof (OutgoingCommandEnvelopeQueueRepository)),
                                      Tuple.Create(typeof (IIncomingCommandEnvelopeQueueRepository),typeof (IncomingCommandEnvelopeQueueRepository)),
                                      Tuple.Create(typeof (IConfigRepository), typeof (ConfigRepository)),
                                      Tuple.Create(typeof (IAppTempTransactionRepository), typeof (AppTempTransactionRepository)),
                                      
                                      Tuple.Create(typeof (IAccountTransactionRepository),
                                                   typeof (AccountTransactionRepository)),
                                      Tuple.Create(typeof (IInventoryTransactionRepository),
                                                   typeof (InventoryTransactionRepository)),
                                      Tuple.Create(typeof (IAccountTransactionRepository),typeof (AccountTransactionRepository)),
                                      Tuple.Create(typeof (IInventoryTransactionRepository),typeof (InventoryTransactionRepository)),
                                      Tuple.Create(typeof (ISalesmanRouteRepository), typeof (SalesmanRouteRepository)),
                                       Tuple.Create(typeof (ISalesmanSupplierRepository), typeof (SalesmanSupplierRepository)),
                                     
                                      
                                      Tuple.Create(typeof (IUserRepository), typeof (UserRepository)),
                                      Tuple.Create(typeof (ILogRepository), typeof (LogRepository)),
                                      Tuple.Create(typeof (IInventoryRepository), typeof (InventoryRepository)),
                                      Tuple.Create(typeof (IDistributorSalesmanRepository),typeof (DistributorSalesmanRepository)),
                                      Tuple.Create(typeof (ISaleValueDiscountRepository),typeof (SaleValueDiscountRepository)),
                                      Tuple.Create(typeof (IProductDiscountRepository),typeof (ProductDiscountRepository)),
                                      Tuple.Create(typeof (ITargetPeriodRepository), typeof (TargetPeriodRepository)),
                                      Tuple.Create(typeof (ITargetRepository), typeof (TargetRepository)),
                                      Tuple.Create(typeof (IProvincesRepository), typeof (ProvincesRepository)),
                                      Tuple.Create(typeof (IDistrictRepository), typeof (DistrictRepository)),
                                      Tuple.Create(typeof (IReOrderLevelRepository), typeof (ReOrderLevelRepository)),
                                      Tuple.Create(typeof (IAssetRepository), typeof (AssetRepository)),
                                      Tuple.Create(typeof (IAssetTypeRepository), typeof (AssetTypeRepository)),
                                      Tuple.Create(typeof (IChannelPackagingRepository),typeof (ChannelPackagingRepository)),
                                      Tuple.Create(typeof (ICompetitorRepository), typeof (CompetitorRepository)),
                                      Tuple.Create(typeof (ICompetitorProductsRepository),typeof (CompetitorProductsRepository)),
                                      Tuple.Create(typeof (IDiscountGroupRepository), typeof (DiscountGroupRepository)),
                                      Tuple.Create(typeof (IDisbursementNoteRepository),typeof (DisbursementNoteRepository)),
                                      Tuple.Create(typeof (ICertainValueCertainProductDiscountRepository),typeof (CertainValueCertainProductDiscountRepository)),
                                      Tuple.Create(typeof (IPromotionDiscountRepository),typeof (PromotionDiscountRepository)),
                                      Tuple.Create(typeof (IProductDiscountGroupRepository),typeof (ProductDiscountGroupRepository)),
                                      Tuple.Create(typeof (IFreeOfChargeDiscountRepository),typeof (FreeOfChargeDiscountRepository)),
                                      Tuple.Create(typeof (IUserGroupRolesRepository), typeof (UserGroupRolesRepository)),
                                      Tuple.Create(typeof (IUserGroupRepository), typeof (UserGroupRepository)),
                                      Tuple.Create(typeof (IBankRepository), typeof (BankRepository)),
                                      Tuple.Create(typeof (IBankBranchRepository), typeof (BankBranchRepository)),
                                      Tuple.Create(typeof (IReturnsNoteRepository), typeof (ReturnsNoteRepository)),
                                      Tuple.Create(typeof (ISupplierRepository), typeof (SupplierRepository)),
                                      Tuple.Create(typeof (IContactTypeRepository), typeof (ContactTypeRepository)),
                                      Tuple.Create(typeof (IUnExecutedCommandRepository),typeof (UnExecutedCommandRepository)),
                                      Tuple.Create(typeof (IGeneralSettingRepository), typeof (GeneralSettingRepository)),
                                      Tuple.Create(typeof (IAssetCategoryRepository), typeof (AssetCategoryRepository)),
                                      Tuple.Create(typeof (IAssetStatusRepository), typeof (AssetStatusRepository)),
                                      Tuple.Create(typeof (IOutletVisitDayRepository), typeof (OutletVisitDayRepository)),
                                      Tuple.Create(typeof (IOutletPriorityRepository), typeof (OutletPriorityRepository)),
                                      Tuple.Create(typeof (IErrorLogRepository), typeof (ErrorLogRepository)),
                                      Tuple.Create(typeof (IOutGoingMasterDataQueueItemRepository),typeof (OutGoingMasterDataQueueItemRepository)),
                                      Tuple.Create(typeof (IOrderRepository), typeof (OrderRepository)),
                                      Tuple.Create(typeof (IInventoryAdjustmentNoteRepository),typeof (InventoryAdjustmentNoteRepository)),
                                      Tuple.Create(typeof (IInventoryTransferNoteRepository),typeof (InventoryTransferNoteRepository)),
                                      Tuple.Create(typeof (ICreditNoteRepository), typeof (CreditNoteRepository)),
                                      Tuple.Create(typeof (IDispatchNoteRepository), typeof (DispatchNoteRepository)),
                                      Tuple.Create(typeof (IAccountRepository), typeof (AccountRepository)),
                                      Tuple.Create(typeof (IInventoryReceivedNoteRepository),typeof (InventoryReceivedNoteRepository)),
                                      Tuple.Create(typeof (IDistributorPendingDispatchWarehouseRepository),typeof (DistributorPendingDispatchWarehouseRepository)),
                                      Tuple.Create(typeof (IInvoiceRepository), typeof (InvoiceRepository)),
                                      Tuple.Create(typeof (IReceiptRepository), typeof (ReceiptRepository)),
                                      Tuple.Create(typeof (ITargetItemRepository), typeof (TargetItemRepository)),
                                      Tuple.Create(typeof (IAsynchronousPaymentRequestRepository),typeof (AsynchronousPaymentRequestRepository)),
                                      Tuple.Create(typeof (IAsynchronousPaymentResponseRepository),typeof (AsynchronousPaymentResponseRepository)),
                                      Tuple.Create(typeof (IAsynchronousPaymentNotificationRequestRepository),typeof (AsynchronousPaymentNotificationRequestRepository)),
                                      Tuple.Create(typeof (IAsynchronousPaymentNotificationResponseRepository),typeof (AsynchronousPaymentNotificationResponseRepository)),
                                      Tuple.Create(typeof (IBuyGoodsNotificationResponseRepository),typeof (BuyGoodsNotificationResponseRepository)),
                                      Tuple.Create(typeof (IPaymentTrackerRepository), typeof (PaymentTrackerRepository)),
                                      Tuple.Create(typeof (IInventorySerialsRepository),typeof (InventorySerialsRepository)),
                                      Tuple.Create(typeof (ISettingsRepository), typeof (SettingsRepository)),
                                      Tuple.Create(typeof (IRetireDocumentSettingRepository),typeof (RetireDocumentSettingRepository)),
                                      Tuple.Create(typeof (IOutGoingMasterDataQueueItemRepository),typeof (OutGoingMasterDataQueueItemRepository)),
                                      Tuple.Create(typeof (IAuditLogRepository), typeof (AuditLogRepository)),
                                      (Tuple.Create(typeof (ICommodityPurchaseRepository),typeof (CommodityPurchaseRepository))),
                                      (Tuple.Create(typeof (ICommodityReceptionRepository),typeof (CommodityReceptionRepository))),
                                      (Tuple.Create(typeof (IEquipmentRepository), typeof (EquipmentRepository))),
                                      (Tuple.Create(typeof (IVehicleRepository), typeof (VehicleRepository))),
                                      (Tuple.Create(typeof (IMasterDataAllocationRepository),typeof (MasterDataAllocationRepository))),
                                      (Tuple.Create(typeof (IReceiptFactory), typeof (ReceiptFactory))),
                                      (Tuple.Create(typeof (ICreditNoteFactory), typeof (CreditNoteFactory))),
                                      (Tuple.Create(typeof (ICommodityPurchaseNoteFactory),typeof (CommodityPurchaseNoteFactory))),
                                      (Tuple.Create(typeof (ICommodityReceptionNoteFactory),typeof (CommodityReceptionNoteFactory))),
                                      (Tuple.Create(typeof (ICommodityStorageNoteFactory),typeof (CommodityStorageNoteFactory))),
                                      (Tuple.Create(typeof (ICommodityDeliveryFactory),typeof (CommodityDeliveryFactory))),
                                      (Tuple.Create(typeof (ICommodityWarehouseStorageFactory),typeof (CommodityWarehouseStorageFactory))),
                                      Tuple.Create(typeof (IMainOrderFactory), typeof (MainOrderFactory)),
                                      Tuple.Create(typeof (IMainOrderRepository), typeof (MainOrderRepository)),
                                      Tuple.Create(typeof (ICommodityTypeRepository), typeof (CommodityTypeRepository)),
                                      Tuple.Create(typeof (ICommodityOwnerTypeRepository),typeof (CommodityOwnerTypeRepository)),
                                      Tuple.Create(typeof (ICentreRepository), typeof (CentreRepository)),
                                      Tuple.Create(typeof (ICentreTypeRepository), typeof (CentreTypeRepository)),
                                      Tuple.Create(typeof (IStoreRepository), typeof (StoreRepository)),
                                      Tuple.Create(typeof (IHubRepository), typeof (HubRepository)),
                                      Tuple.Create(typeof (ICommoditySupplierRepository),typeof (CommoditySupplierRepository)),
                                      Tuple.Create(typeof (ICommodityProducerRepository),typeof (CommodityProducerRepository)),
                                      Tuple.Create(typeof (ICommodityRepository), typeof (CommodityRepository)),
                                      Tuple.Create(typeof (ICommodityOwnerRepository), typeof (CommodityOwnerRepository)),
                                      Tuple.Create(typeof (IPurchasingClerkRepository),typeof (PurchasingClerkRepository)),
                                      Tuple.Create(typeof (IPurchasingClerkRouteRepository),typeof (PurchasingClerkRouteRepository)),
                                      Tuple.Create(typeof (ICommodityStorageRepository),typeof (CommodityStorageRepository)),
                                      Tuple.Create(typeof (ICommodityDeliveryFactory), typeof (CommodityDeliveryFactory)),
                                      Tuple.Create(typeof (IReceivedDeliveryNoteFactory),typeof (ReceivedDeliveryNoteFactory)),
                                      Tuple.Create(typeof (ICommodityDeliveryRepository),typeof (CommodityDeliveryRepository)),
                                      Tuple.Create(typeof (IContainerTypeRepository), typeof (ContainerTypeRepository)),
                                      Tuple.Create(typeof (IReceivedDeliveryRepository),typeof (ReceivedDeliveryRepository)),
                                      Tuple.Create(typeof (IMasterDataUsage), typeof (MasterDataUsage)),
                                      Tuple.Create(typeof (IApplicationSetup), typeof (ApplicationSetup)),
                                      Tuple.Create(typeof (IExportImportAuditRepository),typeof (ExportImportAuditRepository)),
                                         Tuple.Create(typeof (IReceiptExportDocumentRepository),typeof (ReceiptExportDocumentRepository)),
                                          Tuple.Create(typeof (IOrderExportDocumentRepository),typeof (OrderExportDocumentRepository)),
                                             Tuple.Create(typeof (IInvoiceExportDocumentRepository),typeof (InvoiceExportDocumentRepository)),
                                             Tuple.Create(typeof (IReturnInventoryExportDocumentRepository),typeof (ReturnInventoryExportDocumentRepository)),
                                      Tuple.Create(typeof (IIntegrationDocumentRepository),typeof (IntegrationDocumentRepository)),
                                      Tuple.Create(typeof (IGenericDocumentRepository),typeof (GenericDocumentRepository)),
                                      Tuple.Create(typeof (IReCollectionRepository), typeof (ReCollectionRepository)),
                                      Tuple.Create(typeof (IOutgoingNotificationQueueRepository),
                                                   typeof (OutgoingNotificationQueueRepository)),
                                      Tuple.Create(typeof (IPrintedReceiptsTrackerRepository),
                                                   typeof (PrintedReceiptsTrackerRepository)),
                                      Tuple.Create(typeof (ICommodityTransferNoteFactory),
                                                   typeof (CommodityTransferNoteFactory)),
                                      Tuple.Create(typeof (ICommodityWarehouseStorageRepository),
                                                   typeof (CommodityWarehouseStorageRepository)),

                                      Tuple.Create(typeof (ICommoditySupplierInventoryRepository),
                                                   typeof (CommoditySupplierInventoryRepository)),

                                     Tuple.Create(typeof (ISourcingInventoryRepository),
                                                   typeof (SourcingInventoryRepository)),
                                     Tuple.Create(typeof(ICommodityTransferRepository),
                                                   typeof(CommodityTransferRepository)),
                                      Tuple.Create(typeof (IOutgoingNotificationQueueRepository),typeof (OutgoingNotificationQueueRepository)),
                                      Tuple.Create(typeof (IPrintedReceiptsTrackerRepository),typeof (PrintedReceiptsTrackerRepository)),
                                      Tuple.Create(typeof (ICommodityTransferNoteFactory),typeof (CommodityTransferNoteFactory)),

                                     Tuple.Create(typeof (IServiceProviderRepository),
                                                   typeof (ServiceProviderRepository)),
                                     Tuple.Create(typeof (IServiceRepository),
                                                   typeof (ServiceRepository)),
                                     Tuple.Create(typeof (ISeasonRepository),
                                                   typeof (SeasonRepository)),
                                     Tuple.Create(typeof(IInfectionRepository),
                                                   typeof(InfectionRepository)),
                                     Tuple.Create(typeof(IShiftRepository),
                                                   typeof(ShiftRepository)),
                                     Tuple.Create(typeof(IMasterDataToDTOMapping),
                                                   typeof(MasterDataToDTOMapping)),
                                      Tuple.Create(typeof(IActivityTypeRepository),
                                                   typeof(ActivityTypeRepository)),
                                    Tuple.Create(typeof(IMarketAuditRepository),
                                    typeof(MarketAuditRepository)),
                                    Tuple.Create(typeof(IOutletAuditRepository),
                                    typeof(OutletAuditRepository)),
                                                 
                                           Tuple.Create(typeof(IActivityFactory),
                                                   typeof(ActivityFactory)),  
                                                    Tuple.Create(typeof(IActivityRepository),
                                                   typeof(ActivityRepository)),

                                    Tuple.Create(typeof (IOutletVisitReasonsTypeRepository), typeof (OutletVisitReasonsTypeRepository)),
                                     (Tuple.Create(typeof (ICommodityReleaseNoteFactory),typeof (CommodityReleaseNoteFactory))),
                                     (Tuple.Create(typeof (ICommodityReleaseRepository),typeof (CommodityReleaseRepository))),

                                  };



            return serviceList;
        }
    }
}
