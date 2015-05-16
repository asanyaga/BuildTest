using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Distributr.Core.ClientApp;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.Commands.DocumentCommands.OutletDocument;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.Recollections;
using Distributr.Core.Notifications;
using Distributr.Core.Repository.Financials;
using Distributr.Core.Repository.InventoryRepository;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.OutletVisitReasonsTypeRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Master.SettingsRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.CreditNoteRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.DispatchRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IInvoiceRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.InventoryRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.ReceiptInventories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.ReturnsRepositories;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.Impl.Repository.Transactional.AuditLog;

using Distributr.WPF.Lib.Impl.Repository.Utility;
using Distributr.WPF.Lib.Services.Service.CommandQueues;
using Distributr.WPF.Lib.Services.Service.Transactional.Commands.CommandRouting;
using Distributr.WPF.Lib.Services.Service.Utility;
using StructureMap;

namespace Distributr.WPF.UI.Views.Settings
{
    /// <summary>
    /// Interaction logic for DataViewer.xaml
    /// </summary>
    public partial class DataViewer : Page
    {
        public DataViewer()
        {
            InitializeComponent();
        }

        void clear()
        {
            dg1.ItemsSource = dgDetail.ItemsSource = null;
        }

        private void ViewProductBrand_Click(object sender, RoutedEventArgs e)
        {
            clear();
            dg1.ItemsSource = ObjectFactory.GetInstance<IProductBrandRepository>().GetAll();

        }

        private void ViewProductFlavour_Click(object sender, RoutedEventArgs e)
        {
            clear();
            dg1.ItemsSource = ObjectFactory.GetInstance<IProductFlavourRepository>().GetAll();
        }

        private void OutletCategory_Click(object sender, RoutedEventArgs e)
        {
            clear();
            dg1.ItemsSource = ObjectFactory.GetInstance<IOutletCategoryRepository>().GetAll();
        }

        private void Territory_Click(object sender, RoutedEventArgs e)
        {
            clear();
            dg1.ItemsSource = ObjectFactory.GetInstance<ITerritoryRepository>().GetAll();
        }

        private void PricingTier_Click(object sender, RoutedEventArgs e)
        {
            clear();
            dg1.ItemsSource = ObjectFactory.GetInstance<IProductPricingTierRepository>().GetAll();
        }

        private void ProductType_Click(object sender, RoutedEventArgs e)
        {
            clear();
            dg1.ItemsSource = ObjectFactory.GetInstance<IProductTypeRepository>().GetAll();
        }

        private void ProductPackagingType_Click(object sender, RoutedEventArgs e)
        {
            clear();
            dg1.ItemsSource = ObjectFactory.GetInstance<IProductPackagingTypeRepository>().GetAll();

        }

        private void SocioEconomicStatus_Click(object sender, RoutedEventArgs e)
        {
            clear();
            dg1.ItemsSource = ObjectFactory.GetInstance<ISocioEconomicStatusRepository>().GetAll();

        }

        private void Country_Click(object sender, RoutedEventArgs e)
        {
            clear();
            dg1.ItemsSource = ObjectFactory.GetInstance<ICountryRepository>().GetAll();

        }

        private void Region_Click(object sender, RoutedEventArgs e)
        {
            clear();
            dg1.ItemsSource = ObjectFactory.GetInstance<IRegionRepository>().GetAll();

        }

        private void Area_Click(object sender, RoutedEventArgs e)
        {
            clear();
            dg1.ItemsSource = ObjectFactory.GetInstance<IAreaRepository>().GetAll();

        }

        private void Distributor_Click(object sender, RoutedEventArgs e)
        {
            clear();
            dg1.ItemsSource = ObjectFactory.GetInstance<ICostCentreRepository>().GetAll().OfType<Distributor>();

        }

        private void Producer_Click(object sender, RoutedEventArgs e)
        {
            clear();
            dg1.ItemsSource = ObjectFactory.GetInstance<ICostCentreRepository>().GetAll().OfType<Producer>();

        }

        private void Outlet_Click(object sender, RoutedEventArgs e)
        {
            clear();
            dg1.ItemsSource = ObjectFactory.GetInstance<ICostCentreRepository>().GetAll().OfType<Outlet>();

        }

        private void Transporter_Click(object sender, RoutedEventArgs e)
        {
            clear();
            dg1.ItemsSource = ObjectFactory.GetInstance<ICostCentreRepository>().GetAll().OfType<Transporter>();

        }

        private void DistributorSalesman_Click(object sender, RoutedEventArgs e)
        {
            clear();
            dg1.ItemsSource = ObjectFactory.GetInstance<ICostCentreRepository>().GetAll().OfType<DistributorSalesman>();

        }

        private void Contact_Click(object sender, RoutedEventArgs e)
        {
            clear();
            dg1.ItemsSource = ObjectFactory.GetInstance<IContactRepository>().GetAll();

        }

        private void Route_Click(object sender, RoutedEventArgs e)
        {
            clear();
            dg1.ItemsSource = ObjectFactory.GetInstance<IRouteRepository>().GetAll();

        }

        private void User_Click(object sender, RoutedEventArgs e)
        {
            clear();
            dg1.ItemsSource = ObjectFactory.GetInstance<IUserRepository>().GetAll();

        }

        private void Pricing_Click(object sender, RoutedEventArgs e)
        {
            clear();
            dg1.ItemsSource = ObjectFactory.GetInstance<IProductPricingRepository>().GetAll();

        }

        private void PricingItem_Click(object sender, RoutedEventArgs e)
        {
            dg1.ItemsSource = ObjectFactory.GetInstance<IProductPricingRepository>().GetAll().SelectMany(s => s.ProductPricingItems);

        }

        private void SaleProduct_Click(object sender, RoutedEventArgs e)
        {
            clear();
            dg1.ItemsSource = ObjectFactory.GetInstance<IProductRepository>().GetAll().OfType<SaleProduct>();

        }

        private void ReturnableProduct_Click(object sender, RoutedEventArgs e)
        {
            clear();
            dg1.ItemsSource = ObjectFactory.GetInstance<IProductRepository>().GetAll().OfType<ReturnableProduct>();

        }

        private void ConsolidatedProduct_Click(object sender, RoutedEventArgs e)
        {
            clear();
            dg1.ItemsSource = ObjectFactory.GetInstance<IProductRepository>().GetAll().OfType<ConsolidatedProduct>();

        }

        private void ConsolidatedProductItem_Click(object sender, RoutedEventArgs e)
        {
            clear();
            dg1.ItemsSource = ObjectFactory.GetInstance<IProductRepository>().GetAll().OfType<ConsolidatedProduct>().SelectMany(n => n.ProductDetails);
        }

        private void ProductPackaging_Click(object sender, RoutedEventArgs e)
        {
            clear();
            dg1.ItemsSource = ObjectFactory.GetInstance<IProductPackagingRepository>().GetAll();

        }



        private void VatClass_Click(object sender, RoutedEventArgs e)
        {
            clear();
            dg1.ItemsSource = ObjectFactory.GetInstance<IVATClassRepository>().GetAll();
        }

        private void VatClassItem_Click(object sender, RoutedEventArgs e)
        {
            clear();
            dg1.ItemsSource = ObjectFactory.GetInstance<IVATClassRepository>().GetAll().SelectMany(p => p.VATClassItems);
        }

        private void ViewOrders_Click(object sender, RoutedEventArgs e)
        {
            clear();
            var items = ObjectFactory.GetInstance<IOrderRepository>().GetAll().OfType<Order>();
            dg1.ItemsSource = items;
            dgDetail.ItemsSource = items.SelectMany(p => p.LineItems);
        }

        private void ViewOutCommands_Click(object sender, RoutedEventArgs e)
        {
            clear();
            dg1.ItemsSource = ObjectFactory.GetInstance<IOutgoingCommandQueueRepository>().GetUnSentCommands().OrderByDescending(p=>p.DateInserted).ToList();

        }

        private void ViewInCommands_Click(object sender, RoutedEventArgs e)
        {
            clear();
            dg1.ItemsSource = ObjectFactory.GetInstance<IIncomingCommandQueueRepository>().GetUnProcessedCommands().OrderByDescending(p => p.DateInserted).ToList();
        }

        private void Inventory_Click(object sender, RoutedEventArgs e)
        {
            clear();
            dg1.ItemsSource = ObjectFactory.GetInstance<IInventoryRepository>().GetAll();
        }

        private void AdjustmentNote_Click(object sender, RoutedEventArgs e)
        {
            clear();
            dg1.ItemsSource = ObjectFactory.GetInstance<IInventoryAdjustmentNoteRepository>().GetAll();
        }

        private void DispatchNote_Click(object sender, RoutedEventArgs e)
        {
            clear();
            dg1.ItemsSource = ObjectFactory.GetInstance<IDispatchNoteRepository>().GetAll();
        }

        private void CreditNote_Click(object sender, RoutedEventArgs e)
        {
            clear();
            dg1.ItemsSource = ObjectFactory.GetInstance<ICreditNoteRepository>().GetAll();
        }

        private void SalesmanRoutes_Click(object sender, RoutedEventArgs e)
        {
            clear();
            dg1.ItemsSource = ObjectFactory.GetInstance<ISalesmanRouteRepository>().GetAll();
        }

        private void UnExecutedCommand_Click(object sender, RoutedEventArgs e)
        {
            clear();
            dg1.ItemsSource = ObjectFactory.GetInstance<IUnExecutedCommandRepository>().GetAll();
        }

        private void ViewOutGoingMasterData_Click(object sender, RoutedEventArgs e)
        {
            clear();
            dg1.ItemsSource = ObjectFactory.GetInstance<IOutGoingMasterDataQueueItemRepository>().GetAll();
        }

        private void Errorlog_Click(object sender, RoutedEventArgs e)
        {
            clear();
            dg1.ItemsSource = ObjectFactory.GetInstance<IErrorLogRepository>().GetAll();
        }

        private void PaymentTracker_Click(object sender, RoutedEventArgs e)
        {
            clear();
            dg1.ItemsSource = ObjectFactory.GetInstance<IPaymentTrackerRepository>().GetAll();
        }

        private void RetireSetting_Click(object sender, RoutedEventArgs e)
        {
            clear();
            dg1.ItemsSource = ObjectFactory.GetInstance<IRetireDocumentSettingRepository>().GetAll();
        }

        private void Invoices_Click(object sender, RoutedEventArgs e)
        {
            clear();
            dg1.ItemsSource = ObjectFactory.GetInstance<IInvoiceRepository>().GetAll();

        }

        private void Reciept_Click(object sender, RoutedEventArgs e)
        {
            clear();
            dg1.ItemsSource = ObjectFactory.GetInstance<IReceiptRepository>().GetAll();
        }

        private void Dispatch_Click(object sender, RoutedEventArgs e)
        {
            clear();
            dg1.ItemsSource = ObjectFactory.GetInstance<IDispatchNoteRepository>().GetAll();
        }

        private void InventoryReceiveNote_Click(object sender, RoutedEventArgs e)
        {
            clear();
            dg1.ItemsSource = ObjectFactory.GetInstance<IInventoryReceivedNoteRepository>().GetAll();
        }

        private void InventoryTransferNote_Click(object sender, RoutedEventArgs e)
        {
            clear();
            dg1.ItemsSource = ObjectFactory.GetInstance<IInventoryTransferNoteRepository>().GetAll();
        }

        private void ReturnsNote_Click(object sender, RoutedEventArgs e)
        {
            clear();
            dg1.ItemsSource = ObjectFactory.GetInstance<IReturnsNoteRepository>().GetAll();
        }


        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            clear();
            dg1.ItemsSource = ObjectFactory.GetInstance<ISettingsRepository>().GetAll();
        }

        private void Test_Click_1(object sender, RoutedEventArgs e)
        {
           // TestRerouteCommand();
           // TestArchivingCommand();
            //TestNotification();
            TestOutLetVisitCommand();

        }
        private static void TestOutLetVisitCommand()
        {
            CreateOutletVisitNoteCommand cmd = new CreateOutletVisitNoteCommand();
            var route = ObjectFactory.GetInstance<IOutgoingDocumentCommandRouter>();
            var costCentreRepository = ObjectFactory.GetInstance<ICostCentreRepository>();
            var config = ObjectFactory.GetInstance<IConfigService>().Load();
            cmd.CommandGeneratedByCostCentreApplicationId = config.CostCentreApplicationId;
            cmd.CommandId = Guid.NewGuid();
            cmd.DocumentId = Guid.NewGuid();
            cmd.DocumentRecipientCostCentreId = costCentreRepository.GetAll().OfType<Distributor>().First().Id;
            cmd.DocumentOnBehalfCostCentreId = costCentreRepository.GetAll().OfType<Outlet>().First().Id;
            cmd.DocumentIssuerCostCentreId = config.CostCentreId;
            cmd.DocumentDateIssued = DateTime.Now;
            cmd.Note = "TEST";
            cmd.ReasonId = ObjectFactory.GetInstance<IOutletVisitReasonsTypeRepository>().GetAll().First().Id;
            route.RouteDocumentCommand(cmd);
        }
        private static void TestNotification()
        {
            var notificationservice = ObjectFactory.GetInstance<IOutgoingNotificationQueueRepository>();
            var cost = ObjectFactory.GetInstance<ICostCentreRepository>().GetAll().First();
            var config = ObjectFactory.GetInstance<IConfigService>().Load();
            var data = new NotificationOrderSale();
            data.Id = Guid.NewGuid();
            data.DistributorId = config.CostCentreId;
            data.OutletId = cost.Id;
            data.SalemanId = cost.Id;
            data.Items = new List<NotificationOrderSaleItem>
                             {
                                 new NotificationOrderSaleItem {Discount = 2, ItemName = "Mango,"},
                                 new NotificationOrderSaleItem {Discount = 2, ItemName = "Banana,"}
                             };

            notificationservice.Add(data);
        }

        private static void TestReCollectionCommand()
        {
            var wf = ObjectFactory.GetInstance<IReCollectionWFManager>();
            var cost = ObjectFactory.GetInstance<ICostCentreRepository>().GetAll().First();
            var config = ObjectFactory.GetInstance<IConfigService>().Load();
            ReCollection doc = new ReCollection(Guid.NewGuid() );
            doc.CostCentreId = cost.Id;
            doc.CostCentreApplicationId = config.CostCentreApplicationId;
            UnderBankingItem item= new UnderBankingItem(Guid.NewGuid());
            item.Amount = 200;
            item.FromCostCentreId = cost.Id;
            doc.AddLineItem(item);
            wf.SubmitChanges(doc);
        }

        private static void TestRerouteCommand()
        {
            ReRouteDocumentCommand cmd = new ReRouteDocumentCommand();
            var route = ObjectFactory.GetInstance<IOutgoingDocumentCommandRouter>();
            var config = ObjectFactory.GetInstance<IConfigService>().Load();
            cmd.CommandGeneratedByCostCentreApplicationId = config.CostCentreApplicationId;
            cmd.CommandId = Guid.NewGuid();
            cmd.DocumentId = new Guid("71cba9ce-46d5-46b8-8448-2be79281055e");
            cmd.ReciepientCostCentreId = Guid.NewGuid();
            route.RouteDocumentCommand(cmd);
        }
        private static void TestArchivingCommand()
        {
            RetireDocumentCommand cmd = new RetireDocumentCommand();
            var route = ObjectFactory.GetInstance<IOutgoingDocumentCommandRouter>();
            var config = ObjectFactory.GetInstance<IConfigService>().Load();
            cmd.CommandGeneratedByCostCentreApplicationId = config.CostCentreApplicationId;
            cmd.CommandId = Guid.NewGuid();
            cmd.DocumentId = new Guid("71cba9ce-46d5-46b8-8448-2be79281055e");
           // cmd.ReciepientCostCentreId = Guid.NewGuid();
            route.RouteDocumentCommand(cmd);
        }
    }
}
