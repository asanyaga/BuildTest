using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Distributr.Core.CommandHandler.ActivityDocumentHandlers;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.DisbursementNotes;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.InventoryReceivedNotes;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.Invoices;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.OutletDocument;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.Receipts;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.Recollections;
using Distributr.Core.CommandHandler.SourcingDocumentCommandHandlers.CommodityDeliveryCommandHandler;
using Distributr.Core.CommandHandler.SourcingDocumentCommandHandlers.CommodityPurchaseCommandHandler;
using Distributr.Core.CommandHandler.SourcingDocumentCommandHandlers.CommodityReceptionCommandHandler;
using Distributr.Core.CommandHandler.SourcingDocumentCommandHandlers.CommodityReleaseCommandHandler;
using Distributr.Core.CommandHandler.SourcingDocumentCommandHandlers.CommodityStorageCommandHandler;
using Distributr.Core.CommandHandler.SourcingDocumentCommandHandlers.CommodityTranferCommandHandler;
using Distributr.Core.CommandHandler.SourcingDocumentCommandHandlers.CommodityWarehouseStorageCommandHandler;
using Distributr.Core.CommandHandler.SourcingDocumentCommandHandlers.ReceivedDeliveryCommandHandler;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityWarehouseStorageCommands;
using Distributr.Core.Data.CommandHandlers.ActivityDocumentHandlers;
using Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.AdjustmentNotes;
using Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.CreditNotes;
using Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.DisbursementNotes;
using Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.DispatchNotes;
using Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.InventoryReceivedNotes;
using Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.InventoryTransferNotes;
using Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.Invoices;
using Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.Orders;
using Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.OutletDocuments;
using Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.PN;
using Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.Receipts;
using Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.Recollections;
using Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.ReturnsNotes;
using Distributr.Core.Data.CommandHandlers.SourcingDocumentCommandHandlers.CommoditityReleaseCommandHandlers;
using Distributr.Core.Data.CommandHandlers.SourcingDocumentCommandHandlers.CommoditityStorageCommandHandlers;
using Distributr.Core.Data.CommandHandlers.SourcingDocumentCommandHandlers.CommodityDeliveryCommandHandlers;
using Distributr.Core.Data.CommandHandlers.SourcingDocumentCommandHandlers.CommodityReceptionCommandHandlers;
using Distributr.Core.Data.CommandHandlers.SourcingDocumentCommandHandlers.CommodityTransferCommandHandlers;
using Distributr.Core.Data.CommandHandlers.SourcingDocumentCommandHandlers.CommodityWarehouseStorageCommandHandlers;
using Distributr.Core.Data.CommandHandlers.SourcingDocumentCommandHandlers.ReceivedDeliveryCommandHandlers;
using Distributr.Core.Data.Repository.Map;
using Distributr.Core.Data.Repository.MasterData;
using Distributr.Core.Data.Repository.MasterData.Agrimanagr;
using Distributr.Core.Data.Repository.MasterData.AssetRepositories;
using Distributr.Core.Data.Repository.MasterData.CentreRepositories;
using Distributr.Core.Data.Repository.MasterData.CommodityOwnerrepositories;
using Distributr.Core.Data.Repository.MasterData.CommodityRepositories;
using Distributr.Core.Data.Repository.MasterData.EquipmentRepository;
using Distributr.Core.Data.Repository.MasterData.MasterDataAllocationRepositories;
using Distributr.Core.Data.Repository.MasterData.OutletVisitReasonsTypeRepositories;
using Distributr.Core.Data.Repository.MasterData.SettingsRepositories;
using Distributr.Core.Data.Repository.PG;
using Distributr.Core.Data.Repository.Transactional.RecollectionRepositories;
using Distributr.Core.Data.Repository.Transactional.SourcingDocumentRepositories;
using Distributr.Core.Data.Repository.Transactional.ThirdPartyIntegrationRepository;
using Distributr.Core.Data.Repository.Util;
using Distributr.Core.Data.Utility;
using Distributr.Core.Factory.Documents;
using Distributr.Core.Factory.Documents.Impl;
using Distributr.Core.Factory.SourcingDocuments;
using Distributr.Core.Factory.SourcingDocuments.Impl;
using Distributr.Core.MasterDataDTO;
using Distributr.Core.Repository.Map;
using Distributr.Core.Repository.Master.Agrimanagr;
using Distributr.Core.Repository.Master.AssetRepositories;
using Distributr.Core.Repository.Master.CentreRepositories;
using Distributr.Core.Repository.Master.CommodityOwnerRepository;
using Distributr.Core.Repository.Master.CommodityRepositories;
using Distributr.Core.Repository.Master.EquipmentRepository;
using Distributr.Core.Repository.Master.MasterDataAllocationRepositories;
using Distributr.Core.Repository.Master.OutletVisitReasonsTypeRepositories;
using Distributr.Core.Repository.Master.SettingsRepositories;
using Distributr.Core.Repository.PG;
using Distributr.Core.Repository.Transactional.DocumentRepositories.CreditNoteRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.DisbursementRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IInvoiceRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.PaymentNoteRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.ReceiptInventories;
using Distributr.Core.Repository.Transactional.RecollectionRepositories;
using Distributr.Core.Repository.Transactional.SourcingDocumentRepositories;
using Distributr.Core.Repository.Transactional.ThirdPartyIntegrationRepository;
using Distributr.Core.Repository.Util;
using Distributr.Core.Resources.Util;

using Distributr.Core.Workflow;
using Distributr.Core.Workflow.FinancialWorkflow;
using Distributr.Core.Workflow.FinancialWorkflow.Impl;
using StructureMap.Configuration.DSL;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Data.Repository;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.Repository.Master;
using Distributr.Core.Factory.Master;
using Distributr.Core.Factory.Master.Impl;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Data.Utility.Caching;
using log4net.Core;

using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.CommandHandler.DocumentCommandHandlers;
using Distributr.Core.Repository.Transactional.DocumentRepositories;
using Distributr.Core.Data.Repository.DocumentRepositories;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.Orders;
using Distributr.Core.Data.Repository.MasterData.ProductRepositories;
using Distributr.Core.Data.Repository.MasterData.UserRepository;
using Distributr.Core.Data.Repository.MasterData.CostCentreRepositories;
using Distributr.Core.Repository.Financials;
using Distributr.Core.Data.Repository.Financials;
using Distributr.Core.Repository.InventoryRepository;
using Distributr.Core.Data.Repository.InventoryRepository;
using Distributr.Core.Utility;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using Distributr.Core.Repository.Master.DistributorTargetRepositories;
using Distributr.Core.Data.Repository.MasterData.DistributorTargetRepositories;

using Distributr.Core.Workflow.InventoryWorkflow.Impl;
using Distributr.Core.Workflow.InventoryWorkflow;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.AdjustmentNotes;
using Distributr.Core.Repository.Master.CompetitorManagement;
using Distributr.Core.Domain.Master.CompetitorManagement;
using Distributr.Core.Data.Repository.CompetitorManagement;
using Distributr.Core.Data.Repository.MasterData.CoolerTypeRepositories;
using Distributr.Core.Repository.Master.CoolerTypeRepositories;
using Distributr.Core.Repository.Master.ReOrderLevelRepository;
using Distributr.Core.Data.Repository.ReOrderLevelRepository;
using Distributr.Core.Repository.Master.MarketAuditRepositories;
using Distributr.Core.Data.Repository.MasterData.MarketAuditRepositories;
using Distributr.Core.Repository.Master.ChannelPackagings;
using Distributr.Core.Data.Repository.MasterData.ChannelPackagings;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.DispatchNotes;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.InventoryTransferNotes;
using Distributr.Core.Repository.Transactional.DocumentRepositories.InventoryRepositories;
using Distributr.Core.Data.Repository.Transactional.DocumentRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.DispatchRepositories;
using log4net.Repository.Hierarchy;
using log4net;
using log4net.Repository;
using Distributr.Core.Security;
using Distributr.Core.Security.Impl;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.ReturnsNotes;
using Distributr.Core.Repository.Transactional.DocumentRepositories.ReturnsRepositories;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.CreditNotes;
using Distributr.Core.Repository.Master.BankRepositories;
using Distributr.Core.Data.Repository.MasterData.BankRepositories;
using Distributr.Core.Repository.Transactional.AuditLogRepositories;
using Distributr.Core.Data.Repository.Transactional.AuditLogRepositories;
using Distributr.Core.Repository.Master.SuppliersRepositories;
using Distributr.Core.Data.Repository.MasterData.SuppliersRepositories;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.Losses;
using Distributr.Core.Repository.Transactional.DocumentRepositories.LossesRepositories;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.Command;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Setup;
using AddCommodityPurchaseLineItemCommandHandler = Distributr.Core.Data.CommandHandlers.SourcingDocumentCommandHandlers.CommoditityPurchaseCommandHandlers.AddCommodityPurchaseLineItemCommandHandler;
using AddCommodityReceptionLineItemCommandHandler = Distributr.Core.Data.CommandHandlers.SourcingDocumentCommandHandlers.CommodityReceptionCommandHandlers.AddCommodityReceptionLineItemCommandHandler;
using ConfirmCommodityPurchaseCommandHandler = Distributr.Core.Data.CommandHandlers.SourcingDocumentCommandHandlers.CommoditityPurchaseCommandHandlers.ConfirmCommodityPurchaseCommandHandler;
using ConfirmCommodityReceptionCommandHandler = Distributr.Core.Data.CommandHandlers.SourcingDocumentCommandHandlers.CommodityReceptionCommandHandlers.ConfirmCommodityReceptionCommandHandler;
using CreateCommodityPurchaseCommandHandler = Distributr.Core.Data.CommandHandlers.SourcingDocumentCommandHandlers.CommoditityPurchaseCommandHandlers.CreateCommodityPurchaseCommandHandler;
using StructureMap;


namespace Distributr.Core.Data.IOC
{
    public class DataRegistry : Registry
    {

        public DataRegistry()
        {
            string currentPlatformType = "win";
            if (ConfigurationManager.AppSettings["PlatformType"] != null)
                currentPlatformType = ConfigurationManager.AppSettings["PlatformType"];

            string connectionString = ConfigurationManager.AppSettings["cokeconnectionstring"];

            if (currentPlatformType == "win")
            {
                For<CokeDataContext>()
                .Use<CokeDataContext>()
                 .Ctor<string>("connectionString")
                .Is(connectionString);
            }
            if (currentPlatformType == "service")
            {
                For<CokeDataContext>()
                .Use<CokeDataContext>()
                 .Ctor<string>("connectionString")
                .Is(connectionString);
            }
            else
            {
                For<CokeDataContext>()
                .HybridHttpOrThreadLocalScoped()
                .Use<CokeDataContext>()
                .Ctor<string>("connectionString")
                .Is(connectionString);
            }

            For<ICacheProvider>().Use(DefaultCacheProvider.GetInstance());
          //  For<IMessageSourceAccessor>().Use(MessageSourceAccessor.GetInstance(currentPlatformType));

            foreach (var item in DefaultServiceList())
            {
                For(item.Item1).Use(item.Item2);
            }
        }

       

        public static List<Tuple<Type, Type>> DefaultServiceList()
        {
            var serviceList = new List<Tuple<Type, Type>>
                {


                    Tuple.Create(typeof (IProductDiscountRepository), typeof (ProductDiscountRepository)),
                    Tuple.Create(typeof (IProductDiscountFactory), typeof (ProductDiscountFactory)),
                    Tuple.Create(typeof (ISaleValueDiscountRepository), typeof (SaleValueDiscountRepository)),
                    Tuple.Create(typeof (ISaleValueDiscountFactory), typeof (SaleValueDiscountFactory)),
                    Tuple.Create(typeof (IProductBrandRepository), typeof (ProductBrandRepository)),
                    Tuple.Create(typeof (IProductFlavourRepository), typeof (ProductFlavourRepository)),
                    Tuple.Create(typeof (IProductPackagingRepository), typeof (ProductPackagingRepository)),
                    Tuple.Create(typeof (IProductPackagingTypeRepository), typeof (ProductPackagingTypeRepository)),
                    Tuple.Create(typeof (IProductTypeRepository), typeof (ProductTypeRepository)),
                    Tuple.Create(typeof (IProductRepository), typeof (ProductRepository)),
                    Tuple.Create(typeof (IRegionRepository), typeof (RegionRepository)),
                    Tuple.Create(typeof (ICostCentreFactory), typeof (CostCentreFactory)),
                    Tuple.Create(typeof (ICostCentreRepository), typeof (CostCentreRepository)),
                    Tuple.Create(typeof (ICostCentreRefRepository), typeof (CostCentreRefRepository)),


            Tuple.Create(typeof (IServiceProviderRepository), typeof (ServiceProviderRepository)),
            Tuple.Create(typeof (IServiceRepository), typeof (ServiceRepository)),
            Tuple.Create(typeof (ISeasonRepository), typeof (SeasonRepository)),
            Tuple.Create(typeof (IInfectionRepository), typeof (InfectionRepository)),
            Tuple.Create(typeof (IShiftRepository), typeof (ShiftRepository)),


                    Tuple.Create(typeof (IPaymentTrackerWorkflow), typeof (PaymentTrackerWorkflow)),
                    Tuple.Create(typeof (IPaymentTrackerRepository), typeof (PaymentTrackerRepository)),

          
                    Tuple.Create(typeof (ICountryRepository), typeof (CountryRepository)),
                    Tuple.Create(typeof (IVATClassFactory), typeof (VatClassFactory)),
                    Tuple.Create(typeof (IVATClassRepository), typeof (VATClassRepository)),
                    Tuple.Create(typeof (IOutletTypeRepository), typeof (OutletTypeRepository)),
                    Tuple.Create(typeof (IProducerRepository), typeof (ProducerRepository)),
                    Tuple.Create(typeof (IProductPricingRepository), typeof (ProductPricingRepository)),
                    Tuple.Create(typeof (IProductPricingFactory), typeof (ProductPricingFactory)),
                    Tuple.Create(typeof (IProductPricingTierRepository), typeof (ProductPricingTierRepository)),
                    Tuple.Create(typeof (IOutletCategoryRepository), typeof (OutletCategoryRepository)),
                    Tuple.Create(typeof (IUserRepository), typeof (UserRepository)),
                    Tuple.Create(typeof (IOutletRepository), typeof (OutletRepository)),
                    Tuple.Create(typeof (ITransporterRepository), typeof (TransporterRepository)),
                    Tuple.Create(typeof (IProductFactory), typeof (ProductFactory)),
                    Tuple.Create(typeof (IRouteRepository), typeof (RouteRepository)),
                    Tuple.Create(typeof (IRouteFactory), typeof (RouteFactory)),
                    Tuple.Create(typeof (IDistributorSalesmanRepository), typeof (DistributorSalesmanRepository)),
                    Tuple.Create(typeof (IProducerRepository), typeof (ProducerRepository)),
                    Tuple.Create(typeof (IDistributorRepository), typeof (DistributorRepository)),
                    Tuple.Create(typeof (IDocumentFactory), typeof (DocumentFactory)),
                    Tuple.Create(typeof (IMainOrderFactory), typeof (MainOrderFactory)),


                    Tuple.Create(typeof (IAssetStatusRepository), typeof (AssetStatusRepository)),
                    Tuple.Create(typeof (IAssetCategoryRepository), typeof (AssetCategoryRepository)),

                    //Tuple.Create(typeof (ICreateOrderCommandHandler), typeof (CreateOrderCommandHandler)),
                    Tuple.Create(typeof (ISocioEconomicStatusRepository), typeof (SocioEconomicStatusRepository)),
                    Tuple.Create(typeof (ITerritoryRepository), typeof (TerritoryRepository)),
                    Tuple.Create(typeof (IClientMasterDataTrackerRepository), typeof (ClientMasterDataTrackerRepository)),
                    //Tuple.Create(typeof (IAddOrderLineItemCommandHandler), typeof (AddOrderLineItemCommandHandler)),
                    //Tuple.Create(typeof (IConfirmOrderCommandHandler), typeof (ConfirmOrderCommandHandler)),
                    //Tuple.Create(typeof (IApproveOrderCommandHandler), typeof (ApproveOrderCommandHandler)),
                    //Tuple.Create(typeof (IRejectOrderCommandHandler), typeof (RejectOrderCommandHandler)),
                    Tuple.Create(typeof (IContactRepository), typeof (ContactRepository)),
                    Tuple.Create(typeof (IAreaRepository), typeof (AreaRepository)),
                    Tuple.Create(typeof (IAccountRepository), typeof (AccountRepository)),
                    Tuple.Create(typeof (IAccountTransactionRepository), typeof (AccountTransactionRepository)),
                    Tuple.Create(typeof (IInventoryRepository), typeof (InventoryRepository)),
                    Tuple.Create(typeof (IInventoryTransactionRepository), typeof (InventoryTransactionRepository)),
                    Tuple.Create(typeof (ITargetItemRepository), typeof (TargetItemRepository)),
                    Tuple.Create(typeof (ITargetPeriodRepository), typeof (TargetPeriodRepository)),
                    Tuple.Create(typeof (ITargetRepository), typeof (TargetRepository)),
                    Tuple.Create(typeof (IInventoryWorkflow), typeof (InventoryWorkflow)),
                    Tuple.Create(typeof (IProvincesRepository), typeof (ProvincesRepository)),
                    Tuple.Create(typeof (IDistrictRepository), typeof (DistrictRepository)),
                    Tuple.Create(typeof (ICostCentreApplicationRepository), typeof (CostCentreApplicationRepository)),
                    Tuple.Create(typeof (IOrderRepository), typeof (OrderRepository)),
                    //Tuple.Create(typeof (IChangeOrderLineItemCommandHandler), typeof (ChangeOrderLineItemCommandHandler)),
                    //Tuple.Create(typeof (IRemoveOrderLineItemCommandHandler), typeof (RemoveOrderLineItemCommandHandler)),
                    Tuple.Create(typeof (ICreateInventoryReceivedNoteCommandHandler),typeof (CreateInventoryReceivedNoteCommandHandler)),
                    Tuple.Create(typeof (IAddInventoryReceivedNoteLineItemCommandHandler),typeof (AddInventoryReceivedNoteLineItemCommandHandler)),
                    Tuple.Create(typeof (IConfirmInventoryReceivedNoteCommandHandler),typeof (ConfirmInventoryReceivedNoteCommandHandler)),
                    Tuple.Create(typeof (IAssetTypeRepository), typeof (AssetTypeRepository)),
                    Tuple.Create(typeof (IAssetRepository), typeof (AssetRepository)),
                    //IAN
                    Tuple.Create(typeof (IAddInventoryAdjustmentNoteLineItemCommandHandler),typeof (AddInventoryAdjustmentNoteLineItemCommandHandler)),
                    Tuple.Create(typeof (ICreateInventoryAdjustmentNoteCommandHandler),typeof (CreateInventoryAdjustmentNoteCommandHandler)),
                    Tuple.Create(typeof (IConfirmInventoryAdjustmentNoteCommandHandler),typeof (ConfirmInventoryAdjustmentNoteCommandHandler)),
                    Tuple.Create(typeof (IInventoryAdjustmentNoteRepository), typeof (InventoryAdjustmentNoteRepository)),
                    //Inventory Transfer Note
                    Tuple.Create(typeof (IConfirmInventoryTransferNoteCommandHandler),typeof (ConfirmInventoryTransferNoteCommandHandler)),
                    Tuple.Create(typeof (ICreateInventoryTransferNoteCommandHandler),typeof (CreateInventoryTransferNoteCommandHandler)),
                    Tuple.Create(typeof (IAddInventoryTransferNoteLineItemCommandHandler),typeof (AddInventoryTransferNoteLineItemCommandHandler)),
                    Tuple.Create(typeof (IInventoryTransferNoteRepository), typeof (InventoryTransferNoteRepository)),
                    //Returns Note
                    Tuple.Create(typeof (IConfirmReturnsNoteCommandHandler), typeof (ConfirmReturnsNoteCommandHandler)),
                    Tuple.Create(typeof (ICreateReturnsNoteCommandHandler), typeof (CreateReturnsNoteCommandHandler)),
                    Tuple.Create(typeof (IAddReturnsNoteLineItemCommandHandler),typeof (AddReturnsNoteLineItemCommandHandler)),
                    Tuple.Create(typeof (ICloseReturnsNoteCommandHandler), typeof (CloseReturnsNoteCommandHandler)),
                    Tuple.Create(typeof (IReturnsNoteRepository), typeof (ReturnsNoteRepository)),

                    //Credit Note
                    Tuple.Create(typeof (IConfirmCreditNoteCommandHandler), typeof (ConfirmCreditNoteCommandHandler)),
                    Tuple.Create(typeof (ICreateCreditNoteCommandHandler), typeof (CreateCreditNoteCommandHandler)),
                    Tuple.Create(typeof (IAddCreditNoteLineItemCommandHandler),typeof (AddCreditNoteLineItemCommandHandler)),


                    Tuple.Create(typeof (ICompetitorRepository), typeof (CompetitorRepository)),
                    Tuple.Create(typeof (ICompetitorProductsRepository), typeof (CompetitorProductsRepository)),
                    Tuple.Create(typeof (IReOrderLevelRepository), typeof (ReOrderLevelRepository)),
                    Tuple.Create(typeof (ICloseOrderCommandHandler), typeof (CloseOrderCommandHandler)),
                    Tuple.Create(typeof (IChannelPackagingRepository), typeof (ChannelPackagingRepository)),
                    Tuple.Create(typeof (IMarketAuditRepository), typeof (MarketAuditRepository)),
                    Tuple.Create(typeof (IOutletAuditRepository), typeof (OutletAuditRepository)),
                
                    //DN
                    Tuple.Create(typeof (IAddDispatchNoteLineItemCommandHandler),typeof (AddDispatchNoteLineItemCommandHandler)),
                    Tuple.Create(typeof (ICreateDispatchNoteCommandHandler), typeof (CreateDispatchNoteCommandHandler)),
                    Tuple.Create(typeof (IConfirmDispatchNoteCommandHandler), typeof (ConfirmDispatchNoteCommandHandler)),
                    Tuple.Create(typeof (IDispatchNoteRepository), typeof (DispatchNoteRepository)),
                    Tuple.Create(typeof (IDistributorPendingDispatchWarehouseRepository),typeof (DistributorPendingDispatchWarehouseRepository)),
                    //Invoice Command Handlers
                    Tuple.Create(typeof (ICreateInvoiceCommandHandler), typeof (CreateInvoiceCommandHandler)),
                    Tuple.Create(typeof (IAddInvoiceLineItemCommandHandler), typeof (AddInvoiceLineItemCommandHandler)),
                    Tuple.Create(typeof (IConfirmInvoiceCommandHandler), typeof (ConfirmInvoiceCommandHandler)),
                    Tuple.Create(typeof (ICloseInvoiceCommandHandler), typeof (CloseInvoiceCommandHandler)),

                    Tuple.Create(typeof (IFinancialsWorkflow), typeof (FinancialsWorkflow)),

                    Tuple.Create(typeof (IOrderDispatchedToPhoneCommandHandler),typeof (OrderDispatchedToPhoneCommandHandler)),
                    Tuple.Create(typeof (IMasterDataCachingInvalidator), typeof (MasterDataCachingInvalidator)),

                    Tuple.Create(typeof (IOrderPendingDispatchCommandHandler),typeof (OrderPendingDispatchCommandHandler)),
                    //Tuple.Create(typeof (IBackOrderCommandHandler), typeof (BackOrderCommandHandler)),
                    Tuple.Create(typeof (IResolveCommand), typeof (ResolveCommand)),
                    

                    Tuple.Create(typeof (ICreateOutletVisitNoteCommandHandler), typeof (CreateOutletVisitNoteCommandHandler)),
                  
                    Tuple.Create(typeof (ICreateReceiptCommandHandler), typeof (CreateReceiptCommandHandler)),
                    Tuple.Create(typeof (IAddReceiptLineItemCommandHandler), typeof (AddReceiptLineItemCommandHandler)),
                    Tuple.Create(typeof (IConfirmReceiptCommandHandler), typeof (ConfirmReceiptCommandHandler)),
                    Tuple.Create(typeof (IConfirmReceiptLineItemCommandHandler),typeof (ConfirmReceiptLineItemCommandHandler)),

                    Tuple.Create(typeof (ICustomerDiscountRepository), typeof (CustomerDiscountRepository)),
                    Tuple.Create(typeof (ICustomerDiscountFactory), typeof (CustomerDiscountFactory)),

                    Tuple.Create(typeof (IDisbursementNoteRepository), typeof (DisbursementNoteRepository)),
                    Tuple.Create(typeof (ICreateDisbursementNoteCommandHandler),typeof (CreateDisbursementNoteCommandHandler)),
                    Tuple.Create(typeof (IAddDisbursementNoteLineItemCommandHandler),typeof (AddDisbursementNoteLineItemCommandHandler)),
                    Tuple.Create(typeof (IConfirmDisbursementNoteCommandHandler),typeof (ConfirmDisbursementNoteCommandHandler)),
                    Tuple.Create(typeof (IPromotionDiscountFactory), typeof (PromotionDiscountFactory)),
                    Tuple.Create(typeof (IPromotionDiscountRepository), typeof (PromotionDiscountRepository)),
                    Tuple.Create(typeof (IDiscountGroupRepository), typeof (DiscountGroupRepository)),
                    Tuple.Create(typeof (IProductDiscountGroupFactory), typeof (ProductDiscountGroupFactory)),
                    Tuple.Create(typeof (IProductDiscountGroupRepository), typeof (ProductDiscountGroupRepository)),
                    Tuple.Create(typeof (ICertainValueCertainProductDiscountRepository),typeof (CertainValueCertainProductDiscountRepository)),
                    Tuple.Create(typeof (ICertainValueCertainProductDiscountFactory),typeof (CertainValueCertainProductDiscountFactory)),
                    Tuple.Create(typeof (IFreeOfChargeDiscountRepository), typeof (FreeOfChargeDiscountRepository)),
                    Tuple.Create(typeof (ISecurityService), typeof (SecurityService)),
                    Tuple.Create(typeof (ISalesmanRouteRepository), typeof (SalesmanRouteRepository)),
                  

                    Tuple.Create(typeof (ISalesmanSupplierRepository), typeof (SalesmanSupplierRepository)),
                    Tuple.Create(typeof (IContainmentRepository), typeof (ContainmentRepository)),
                    Tuple.Create(typeof (IUserGroupRepository), typeof (UserGroupRepository)),
                    Tuple.Create(typeof (IUserGroupRolesRepository), typeof (UserGroupRolesRepository)),
                    Tuple.Create(typeof (IBankRepository), typeof (BankRepository)),
                    Tuple.Create(typeof (IBankBranchRepository), typeof (BankBranchRepository)),
                    Tuple.Create(typeof (IAuditLogRepository), typeof (AuditLogRepository)),
                    Tuple.Create(typeof (IInventoryReceivedNoteRepository), typeof (InventoryReceivedNoteRepository)),
                    Tuple.Create(typeof (ISupplierRepository), typeof (SupplierRepository)),
                    Tuple.Create(typeof (IContactTypeRepository), typeof (ContactTypeRepository)),



                    Tuple.Create(typeof (ICreatePaymentNoteCommandHandler), typeof (CreatePaymentNoteCommandHandler)),
                    Tuple.Create(typeof (IAddPaymentNoteLineItemCommandHandler),typeof (AddPaymentNoteLineItemCommandHandler)),
                    Tuple.Create(typeof (IConfirmPaymentNoteCommandHandler), typeof (ConfirmPaymentNoteCommandHandler)),

                    Tuple.Create(typeof (IDistributrFileRepository), typeof (DistributrFileRepository)),
                    Tuple.Create(typeof (IOutletVisitDayRepository), typeof (OutletVisitDayRepository)),
                    Tuple.Create(typeof (IOutletPriorityRepository), typeof (OutletPriorityRepository)),
                     Tuple.Create(typeof (IOutletVisitReasonsTypeRepository), typeof (OutletVisitReasonsTypeRepository)),
                    //Settings
                    Tuple.Create(typeof (ISettingsRepository), typeof (SettingsRepository)),
                    //System resources

                    Tuple.Create(typeof (IRetireDocumentSettingRepository),typeof (RetireDocumentSettingRepository)),
                    Tuple.Create(typeof (IInventorySerialsRepository), typeof (InventorySerialsRepository)),
                    Tuple.Create(typeof (IInvoiceRepository), typeof (InvoiceRepository)),
                    Tuple.Create(typeof (IReceiptRepository), typeof (ReceiptRepository)),
                    Tuple.Create(typeof (ICreditNoteRepository), typeof (CreditNoteRepository)),

                    Tuple.Create(typeof (ICommodityOwnerTypeRepository), typeof (CommodityOwnerTypeRepository)),
                    Tuple.Create(typeof (ICommodityTypeRepository), typeof (CommodityTypeRepository)),
                    Tuple.Create(typeof (ICentreRepository), typeof (CentreRepository)),
                    Tuple.Create(typeof (ICentreTypeRepository), typeof (CentreTypeRepository)),
                    Tuple.Create(typeof (IHubRepository), typeof (HubRepository)),
                    Tuple.Create(typeof (ICommoditySupplierRepository), typeof (CommoditySupplierRepository)),
                    Tuple.Create(typeof (ICommodityProducerRepository), typeof (CommodityProducerRepository)),
                    Tuple.Create(typeof (ICommodityRepository), typeof (CommodityRepository)),
                    Tuple.Create(typeof (ICommodityOwnerRepository), typeof (CommodityOwnerRepository)),
                    Tuple.Create(typeof (IPurchasingClerkRouteRepository), typeof (PurchasingClerkRouteRepository)),
                    Tuple.Create(typeof (IPurchasingClerkRepository), typeof (PurchasingClerkRepository)),
                    Tuple.Create(typeof (IEquipmentRepository), typeof (EquipmentRepository)),
                    Tuple.Create(typeof (IVehicleRepository), typeof (VehicleRepository)),
                    Tuple.Create(typeof (IContainerTypeRepository), typeof (ContainerTypeRepository)),
          
 Tuple.Create(typeof (IApproveCommodityTransferCommandHandler), typeof (ApproveCommodityTransferCommandHandler)),

                    Tuple.Create(typeof (ICommodityPurchaseRepository), typeof (CommodityPurchaseRepository)),
                    Tuple.Create(typeof (ICommodityReceptionRepository), typeof (CommodityReceptionRepository)),
                    Tuple.Create(typeof (IAddCommodityPurchaseLineItemCommandHandler),typeof (AddCommodityPurchaseLineItemCommandHandler)),
                    Tuple.Create(typeof (ICreateCommodityPurchaseCommandHandler),typeof (CreateCommodityPurchaseCommandHandler)),
                    Tuple.Create(typeof (IConfirmCommodityPurchaseCommandHandler),typeof (ConfirmCommodityPurchaseCommandHandler)),
                    Tuple.Create(typeof (IStoreRepository), typeof (StoreRepository)),
                    Tuple.Create(typeof (ICommodityStorageRepository), typeof (CommodityStorageRepository)),
                    Tuple.Create(typeof (ICommodityTransferRepository), typeof (CommodityTransferRepository)),

                    Tuple.Create(typeof (IAddCommodityReceptionLineItemCommandHandler),typeof (AddCommodityReceptionLineItemCommandHandler)),
                    Tuple.Create(typeof (ICreateCommodityReceptionCommandHandler),typeof (CreateCommodityReceptionCommandHandler)),
                    Tuple.Create(typeof (IConfirmCommodityReceptionCommandHandler),typeof (ConfirmCommodityReceptionCommandHandler)),

                    Tuple.Create(typeof (IAddCommodityStorageLineItemCommandHandler),typeof (AddCommodityStorageLineItemCommandHandler)),
                    Tuple.Create(typeof (ICreateCommodityStorageCommandHandler),typeof (CreateCommodityStorageCommandHandler)),
                    Tuple.Create(typeof (IConfirmCommodityStorageCommandHandler),typeof (ConfirmCommodityStorageCommandHandler)),
            Tuple.Create(typeof (ISourcingInventoryRepository),typeof (SourcingInventoryRepository)),
            Tuple.Create(typeof (IActivityTypeRepository),typeof (ActivityTypeRepository)),
            Tuple.Create(typeof (IActivityRepository),typeof (ActivityRepository)),


            //activity command handler

            Tuple.Create(typeof (IConfirmActivityCommandHandler),typeof (ConfirmActivityCommandHandler)),
            Tuple.Create(typeof (ICreateActivityCommandHandler),typeof (CreateActivityCommandHandler)),
            Tuple.Create(typeof (IAddActivityInfectionLineItemCommandHandler),typeof (AddActivityInfectionLineItemCommandHandler)),
            Tuple.Create(typeof (IAddActivityInputLineItemCommandHandler),typeof (AddActivityInputLineItemCommandHandler)),
            Tuple.Create(typeof (IAddActivityServiceLineItemCommandHandler),typeof (AddActivityServiceLineItemCommandHandler)),
            Tuple.Create(typeof (IAddActivityProduceLineItemCommandHandler),typeof (AddActivityProduceLineItemCommandHandler)),

                    Tuple.Create(typeof (IAddCommodityTransferLineItemCommandHandler),typeof (AddCommodityTransferLineItemCommandHandler)),
                    Tuple.Create(typeof (ICreateCommodityTransferCommandHandler),typeof (CreateCommodityTransferCommandHandler)),
                    Tuple.Create(typeof (IConfirmCommodityTransferCommandHandler),typeof (ConfirmCommodityTransferCommandHandler)),

                    Tuple.Create(typeof (IPaymentNoteRepository), typeof (PaymentNoteRepository)),
                    Tuple.Create(typeof (IGenericDocumentRepository), typeof (GenericDocumentRepository)),
                    Tuple.Create(typeof (IMasterDataEnvelopeBuilder), typeof (MasterDataEnvelopeBuilder)),

                    Tuple.Create(typeof (IMasterDataAllocationRepository), typeof (MasterDataAllocationRepository)),
                    Tuple.Create(typeof (IAddExternalDocRefCommandHandler), typeof (AddExternalDocRefCommandHandler)),
                    Tuple.Create(typeof (ICreateMainOrderCommandHandler), typeof (CreateMainOrderCommandHandler)),
                    Tuple.Create(typeof (IAddMainOrderLineItemCommandHandler),typeof (AddMainOrderLineItemCommandHandler)),
                    Tuple.Create(typeof (IConfirmMainOrderCommandHandler), typeof (ConfirmMainOrderCommandHandler)),
                    Tuple.Create(typeof (IMainOrderRepository), typeof (MainOrderRepository)),
                    Tuple.Create(typeof (IApproveOrderLineItemCommandHandler),typeof (ApproveOrderLineItemCommandHandler)),
                    Tuple.Create(typeof (IApproveMainOrderCommandHandler), typeof (ApproveMainOrderCommandHandler)),
                    Tuple.Create(typeof (IOrderDispatchedToPhoneCommandHandler),typeof (OrderDispatchedToPhoneCommandHandler)),
                    Tuple.Create(typeof (IChangeMainOrderLineItemCommandHandler),typeof (ChangeMainOrderLineItemCommandHandler)),
                    Tuple.Create(typeof (IRemoveMainOrderLineItemCommandHandler),typeof (RemoveMainOrderLineItemCommandHandler)),
                    Tuple.Create(typeof (IRejectMainOrderCommandHandler), typeof (RejectMainOrderCommandHandler)),
                    Tuple.Create(typeof (IOrderDispatchApprovedLineItemsCommandHandler),typeof (OrderDispatchApprovedLineItemsCommandHandler)),
                    Tuple.Create(typeof (IStoredCommodityReceptionLineItemCommandHandler),typeof (StoredCommodityReceptionLineItemCommandHandler)),
                    Tuple.Create(typeof (IAddOrderPaymentInfoCommandHandler), typeof (AddOrderPaymentInfoCommandHandler)),


                    Tuple.Create(typeof (IConfirmCommodityDeliveryCommandHandler),typeof (ConfirmCommodityDeliveryCommandHandler)),
                    Tuple.Create(typeof (ICreateCommodityDeliveryCommandHandler),typeof (CreateCommodityDeliveryCommandHandler)),
                    Tuple.Create(typeof (IAddCommodityDeliveryLineItemCommandHandler),typeof (AddCommodityDeliveryLineItemCommandHandler)),
                    Tuple.Create(typeof (IWeighedCommodityDeliveryLineItemCommandHandler),typeof (WeighedCommodityDeliveryLineItemCommandHandler)),
                    Tuple.Create(typeof (IStoredReceivedDeliveryLineItemCommandHandler),typeof (StoredReceivedDeliveryLineItemCommandHandler)),
                    Tuple.Create(typeof (IApproveDeliveryCommandHandler), typeof (ApproveDeliveryCommandHandler)),

                    Tuple.Create(typeof (ICommodityDeliveryRepository), typeof (CommodityDeliveryRepository)),
                    Tuple.Create(typeof (IGenericSourceDocumentRepository), typeof (GenericSourceDocumentRepository)),
                    Tuple.Create(typeof (IConfirmReceivedDeliveryCommandHandler),typeof (ConfirmReceivedDeliveryCommandHandler)),
                    Tuple.Create(typeof (ICreateReceivedDeliveryCommandHandler),typeof (CreateReceivedDeliveryCommandHandler)),
                    Tuple.Create(typeof (IAddReceivedDeliveryLineItemCommandHandler),typeof (AddReceivedDeliveryLineItemCommandHandler)),
                    Tuple.Create(typeof (IMasterDataUsage), typeof (MasterDataUsage)),
                    Tuple.Create(typeof (IApplicationSetup), typeof (ApplicationSetup)),
                    Tuple.Create(typeof (ITransactionsSummary), typeof (TransactionsSummary)),
                    Tuple.Create(typeof (IMapCordinateRepository), typeof (MapCordinateRepository)),
                    Tuple.Create(typeof (IReCollectionRepository), typeof (ReCollectionRepository)),
                    Tuple.Create(typeof (IReCollectionCommandHandler), typeof (ReCollectionCommandHandler)),
                    Tuple.Create(typeof (IDropdownRepository), typeof (DropdownRepository)),
                    Tuple.Create(typeof (IPgRepositoryHelper), typeof (PgRepositoryHelper)),
                   
                     Tuple.Create(typeof (IAddCommodityWarehouseStorageLineItemCommandHandler), typeof (AddCommodityWarehouseStorageLineItemCommandHandler)),
                     Tuple.Create(typeof (ICreateCommodityWarehouseStorageCommandHandler), typeof (CreateCommodityWarehouseStorageCommandHandler)),
                     Tuple.Create(typeof (IConfirmCommodityWarehouseStorageCommandHandler), typeof (ConfirmCommodityWarehouseStorageCommandHandler)),
                    Tuple.Create(typeof (IUpdateCommodityWarehouseStorageLineItemCommandHandler), typeof (UpdateCommodityWarehouseStorageLineItemCommandHandler)),
                    Tuple.Create(typeof (IApproveCommodityWarehouseStorageCommandHandler), typeof (ApproveCommodityWarehouseStorageCommandHandler)),
                    Tuple.Create(typeof (IStoreCommodityWarehouseStorageCommandHandler), typeof (StoreCommodityWarehouseStorageCommandHandler)),
                    Tuple.Create(typeof (IGenerateReceiptCommodityWarehouseStorageCommandHandler), typeof (GenerateReceiptCommodityWarehouseStorageCommandHandler)),

                    Tuple.Create(typeof (ICommodityWarehouseStorageRepository), typeof (CommodityWarehouseStorageRepository)),
                    Tuple.Create(typeof (ICommoditySupplierInventoryRepository), typeof (CommoditySupplierInventoryRepository)),
                     Tuple.Create(typeof (IOutletMainOrderRepository), typeof (OutletMainOrderRepository)),
                     
                     
                     Tuple.Create(typeof (ICreateCommodityReleaseCommandHandler), typeof (CreateCommodityReleaseCommandHandler)),
                     Tuple.Create(typeof (IAddCommodityReleaseLineItemCommandHandler), typeof (AddCommodityReleaseLineItemCommandHandler)),
                     Tuple.Create(typeof (IConfirmCommodityReleaseCommandHandler), typeof (ConfirmCommodityReleaseCommandHandler)),
                     
                     
                     Tuple.Create(typeof (ICommodityReleaseRepository), typeof (CommodityReleaseRepository)),
                     Tuple.Create(typeof (ISourcingInventoryWorkflow), typeof (SourcingInventoryWorkflow)),
                     
                     Tuple.Create(typeof (ICommodityReleaseNoteFactory), typeof (CommodityReleaseNoteFactory)),
                    
                };

            return serviceList;
        }



    }
}
