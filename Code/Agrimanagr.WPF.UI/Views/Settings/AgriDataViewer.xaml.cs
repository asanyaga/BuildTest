using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.FarmActivities;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Factory.ActivityDocuments;
using Distributr.Core.Notifications;
using Distributr.Core.Repository.Financials;
using Distributr.Core.Repository.InventoryRepository;
using Distributr.Core.Repository.Master.Agrimanagr;
using Distributr.Core.Repository.Master.CentreRepositories;
using Distributr.Core.Repository.Master.CommodityRepositories;
using Distributr.Core.Repository.Master.CostCentreRepositories;
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
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Workflow.Impl.Activities;
using Distributr.WPF.Lib.Impl.Repository.Transactional.AuditLog;
using Distributr.WPF.Lib.Impl.Repository.Utility;
using Distributr.WPF.Lib.Services.Service.CommandQueues;
using Distributr.WPF.Lib.Services.Service.Utility;
using StructureMap;

namespace Agrimanagr.WPF.UI.Views.Settings
{
    /// <summary>
    /// Interaction logic for AgriDataViewer.xaml
    /// </summary>
    public partial class AgriDataViewer : UserControl
    {
        public AgriDataViewer()
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
            dg1.ItemsSource = ObjectFactory.GetInstance<IOutgoingCommandQueueRepository>().GetAll().OrderByDescending(p=>p.DateSent);

        }

        private void ViewInCommands_Click(object sender, RoutedEventArgs e)
        {
            clear();
            dg1.ItemsSource = ObjectFactory.GetInstance<IIncomingCommandQueueRepository>().GetAll().OrderByDescending(p => p.DateInserted);
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

        private void Test_Click(object sender, RoutedEventArgs e)
        {
            TestNotification();
        }
        private static void TestNotification()
        {

            var wf = ObjectFactory.GetInstance<IActivityWFManager>();
            var factory = ObjectFactory.GetInstance<IActivityFactory>();
            var cost = ObjectFactory.GetInstance<ICostCentreRepository>().GetAll().First();
            var producer = ObjectFactory.GetInstance<ICommodityProducerRepository>().GetAll().First();
            var product = ObjectFactory.GetInstance<IProductRepository>().GetAll().First();
            var activitytype = ObjectFactory.GetInstance<IActivityTypeRepository>().Query(new QueryActivityType()).Data.OfType<ActivityType>().First();
             var route = ObjectFactory.GetInstance<IRouteRepository>().GetAll().First();
             var center = ObjectFactory.GetInstance<ICentreRepository>().GetAll().First();
            var season = ObjectFactory.GetInstance<ISeasonRepository>().GetAll().First();
            var service = ObjectFactory.GetInstance<IServiceRepository>().GetAll().First();
            var shift = ObjectFactory.GetInstance<IShiftRepository>().GetAll().First();
            var infection = ObjectFactory.GetInstance<IInfectionRepository>().GetAll().First();
            var commodity = ObjectFactory.GetInstance<ICommodityRepository>().GetAll().First();
            var serviceProvider = ObjectFactory.GetInstance<IServiceProviderRepository>().GetAll().First();
            var config = ObjectFactory.GetInstance<IConfigService>().Load();
            var activity = factory.Create(cost, cost, cost, producer, activitytype, route, center, season, "Test Activity", config.CostCentreApplicationId,
                DateTime.Now,DateTime.Now,"dettss");
            var inputitem = factory.CreateInputLineItem(product.Id, 10, "2232323", DateTime.Now, DateTime.Now);
            activity.Add(inputitem);
            var servieitem = factory.CreateServiceLineItem(service.Id, serviceProvider.Id,shift.Id, "2232323");
            activity.Add(servieitem);
            var infectionitem = factory.CreateInfectionLineItem(infection.Id, 10, "2232323");
            activity.Add(infectionitem);
            var produceitem = factory.CreateProduceLineItem(commodity.Id,commodity.CommodityGrades.First().Id,serviceProvider.Id, 10, "2232323");
            activity.Add(produceitem);
            activity.Confirm();
            wf.SubmitChanges(activity);
        }
    }
}
