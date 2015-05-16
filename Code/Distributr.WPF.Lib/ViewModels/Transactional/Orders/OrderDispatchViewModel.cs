using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Distributr.Core.ClientApp;
using Distributr.Core.Commands.CommandPackage;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.OrderDocumentEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.UI.Pages;
using GalaSoft.MvvmLight.Command;

namespace Distributr.WPF.Lib.ViewModels.Transactional.Orders
{
    
    public class OrderDispatchViewModel : DistributrViewModelBase
    {
        public ObservableCollection<DistributorSalesman> SalesmanLookUp { get; set; }
        public ObservableCollection<Route> RoutesLookUp { get; set; }
        public RelayCommand<object> SalesmanDropDownOpenedCommand { get; set; }
        public RelayCommand<object> RouteDropDownOpenedCommand { get; set; }
        public RelayCommand RouteChangedCommand { get; set; }
        public RelayCommand SetupCommand { get; set; }
        public RelayCommand SelectOrderItemCommand { get; set; }
        public ObservableCollection<OrderDispatchItemSummary> OrderDispatchItemSummaryList { get; set; }
        public RelayCommand SalesmanChangedCommand { get; set; }
        public RelayCommand DispatchCommand { get; set; }
        public RelayCommand BackCommand { get; set; }
        public RelayCommand<OrderDispatchItemSummary> ChangeSalesmanCommand { get; set; }
        public RelayCommand<List<Guid>> DispatchBatchesCommand { get; set; }
        
        
        public OrderDispatchViewModel()
        {
            RouteChangedCommand = new RelayCommand(RouteChanged);
            SalesmanDropDownOpenedCommand = new RelayCommand<object>(SalesmanDropDownOpened);
            RouteDropDownOpenedCommand = new RelayCommand<object>(RouteDropDownOpened);
            SalesmanLookUp = new ObservableCollection<DistributorSalesman>();
            RoutesLookUp = new ObservableCollection<Route>();
            SetupCommand = new RelayCommand(Setup);
            OrderDispatchItemSummaryList = new ObservableCollection<OrderDispatchItemSummary>();
            SelectOrderItemCommand = new RelayCommand(SelectOrderItem);
            SalesmanChangedCommand = new RelayCommand(SalesmanChanged);
            DispatchCommand = new RelayCommand(Dispatch);
            BackCommand = new RelayCommand(Back);
            ChangeSalesmanCommand = new RelayCommand<OrderDispatchItemSummary>(ChangeSalesman);
            DispatchBatchesCommand = new RelayCommand<List<Guid>>(DispatchBatches);
        }

        private void ChangeSalesman(OrderDispatchItemSummary selected)
        {
            using (var c = NestedContainer)
            {
                var newSalesman = Using<IChangeSalesmanToDeliveryPopUp>(c).ShowPopUp(selected.OrderRoute);
                if( newSalesman!=null)
                {
                    var item= OrderDispatchItemSummaryList.FirstOrDefault(s => s.OrderId == selected.OrderId);
                    if (item != null) item.ChangeToSalesman = newSalesman;
                }
            }
        }

        private void Back()
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to go back to order creation", "Distributr",
                                                      MessageBoxButton.OKCancel);
            if(result==MessageBoxResult.Cancel)
                return;
            NavigateCommand.Execute(@"\Views\Orders\CreateOrder.xaml");
        }

        private async void Dispatch()
        {
            if(OrderDispatchItemSummaryList.All(a=>!a.IsChecked))
            {
                MessageBox.Show("You have not selected any order to dispatch","Distributr: Dispatch Order");
                return;
            }
            MessageBoxResult result = MessageBox.Show("Dispatch selected Order(s) ?", "Distributr",
                                                     MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.Cancel)
                return;

            DoWork();
        }

        public async Task DoWork()
        {
            var orderToprocess = OrderDispatchItemSummaryList.Where(s => s.IsChecked);
            await Task.WhenAll(orderToprocess.Select(ProcessDispatch));
            if (ShowDispatchMessage)
            OrderDispatchedMessage(orderToprocess.Count());
        }

        async void DispatchBatches(List<Guid> approvedOrdersBatch )
        {
            await Task.Run(() =>
                               {
                                   if (!approvedOrdersBatch.Any()) return;
                                   using (var c = NestedContainer)
                                   {

                                       var mainOrderRepository = Using<IMainOrderRepository>(c);
                                       var mainOrderworkflow = Using<IOrderWorkflow>(c);
                                       foreach (var orderId in approvedOrdersBatch)
                                       {
                                           MainOrder order = mainOrderRepository.GetById(orderId);
                                           Config config = Using<IConfigService>(c).Load();
                                           Guid costCentreApplicationid = config.CostCentreApplicationId;
                                           order.ChangeccId(costCentreApplicationid);
                                           order.DispatchPendingLineItems();
                                           mainOrderworkflow.Submit(order,config);
                                       }

                                   }
                               });
        }

        private async Task ProcessDispatch(OrderDispatchItemSummary item)
        {
           await Task.Run(() =>
                              {
                                 using(var c = NestedContainer)
                                 {
                                     var mainOrderRepository = Using<IMainOrderRepository>(c);
                                     var mainOrderworkflow = Using<IOrderWorkflow>(c);
                                     MainOrder order = mainOrderRepository.GetById(item.OrderId);
                                     Config config = Using<IConfigService>(c).Load();
                                     Guid costCentreApplicationid = config.CostCentreApplicationId;
                                     order.ChangeccId(costCentreApplicationid);
                                     if(item.ChangeToSalesman!=null)
                                     {
                                         var envelope = new CommandEnvelope();
                                         envelope.DocumentId = order.Id;
                                         envelope.RecipientCostCentreId = item.ChangeToSalesman.Id;
                                         envelope.DocumentTypeId = (int) DocumentType.Order;
                                         envelope.EnvelopeGeneratedTick = DateTime.Now.Ticks;
                                         envelope.GeneratedByCostCentreId = config.CostCentreId;
                                         envelope.Id = Guid.NewGuid();
                                         envelope.ParentDocumentId = order.Id;
                                         envelope.IsSystemEnvelope = true;
                                         var  commandEnvelopeRouter = Using< IOutgoingCommandEnvelopeRouter >(c);
                                         order.ChangeSaleman(item.ChangeToSalesman);
                                         ReRouteDocumentCommand cmd = new ReRouteDocumentCommand();
                                         cmd.CommandGeneratedByCostCentreApplicationId = config.CostCentreApplicationId;
                                         cmd.CommandId = Guid.NewGuid();
                                         cmd.DocumentId = order.Id;
                                         cmd.ReciepientCostCentreId = item.ChangeToSalesman.Id;
                                         envelope.CommandsList.Add(new CommandEnvelopeItem(1, cmd));
                                         commandEnvelopeRouter.RouteCommandEnvelope(envelope);
                                     }
                                     order.DispatchPendingLineItems();
                                     mainOrderworkflow.Submit(order,config);
                                 }

                              }

                ).ConfigureAwait(false);
            //t.Wait();
           // return t;
        }

        private void OrderDispatchedMessage(int count)
        {
            using (var c = NestedContainer)
            {
                var faction = new List<DistributrMessageBoxButton>
                                  {
                                      DistributrMessageBoxButton.SalesmanOrderApprove,
                                      DistributrMessageBoxButton.SalemanOrderViewDispatched
                                  };
                var fresult = Using<IDistributrMessageBox>(c)
                    .ShowBox(faction,
                             string.Format(" {0} order(s) were dispatched successfully ", count),
                             string.Format("Distributr: Dispatched orders "));
                NavigateCommand.Execute(fresult.Url);
                CleanViewModel();
            }
        }

        private void SalesmanChanged()
        {
            if (SelectedSalesman != null && SelectedSalesman.Id != Guid.Empty)
                LoadOrderPendingDispatch();
        }

        private void SelectOrderItem()
        {
            
            OrderDispatchItemSummaryList.ToList().ForEach(s => s.IsChecked = IsChecked);
        }

        private void Setup()
        {
            CleanViewModel();
            LoadRoutes();
            LoadOrderPendingDispatch();
        }

        private void CleanViewModel()
        {
            OrderDispatchItemSummaryList.Clear();
            SelectedSalesman = null;
            SelectedRoute = null;
            IsChecked = false;
        }

        private void LoadOrderPendingDispatch()
        {
            OrderDispatchItemSummaryList.Clear();
            using (var container = NestedContainer)
            {
                DateTime startDate = DateTime.Now.AddDays(-30);
                Guid? routeId = (SelectedRoute != null && SelectedRoute.Id != Guid.Empty) ? SelectedRoute.Id : (Guid?) null;
                Guid? salesmanId = (SelectedSalesman != null && SelectedSalesman.Id != Guid.Empty) ? SelectedSalesman.Id : (Guid?) null;

                var order = Using<IMainOrderRepository>(container).GetPendingDispatch(routeId,salesmanId);
                var costCentreRepository = Using<ICostCentreRepository>(container);
                var priceService = Using<IDiscountProWorkflow>(container);
                
                int sequenceNo = 1;
                foreach (var item in order)
                {
                    var outlet = costCentreRepository.GetById(item.OutletId) as Outlet;
                    var salesman = costCentreRepository.GetById(item.SalesmanId) as DistributorSalesman;
                    var vitem = new OrderDispatchItemSummary
                                    {
                                        GrossAmount =item.GrossAmount.GetTotalGross(),
                                        IsChecked = false,
                                        NetAmount = item.NetAmount.GetTotalGross(),
                                        OrderId = item.OrderId,
                                        OrderReference = item.OrderReference,
                                        OutstandingAmount = item.OutstandingAmount,
                                        PaidAmount = item.PaidAmount,
                                        Required = item.Required,
                                        Salesman = item.Salesman,
                                        Status = item.Status,
                                        TotalVat = item.TotalVat,
                                        SequenceNo = sequenceNo
                                    };
                    if (outlet != null)
                        vitem.OrderRoute = outlet.Route;
                    if (salesman != null) 
                        vitem.ChangeToSalesman = salesman;
                    OrderDispatchItemSummaryList.Add(vitem);
                    sequenceNo++;
                }
            }
        }

        private void LoadRoutes()
        {
            RoutesLookUp.Clear();
            RoutesLookUp.Add(new Route(Guid.Empty) {Name = "------Select Route-------  "});
            using (var container = NestedContainer)
            {
                var salesmen =
                    Using<IRouteRepository>(container).GetAll();
                salesmen.OrderBy(s => s.Name).ToList().ForEach(f => RoutesLookUp.Add(f));

            }
            SelectedRoute = RoutesLookUp.FirstOrDefault();
        }

        private void RouteDropDownOpened(object obj)
        {
            using (var container = NestedContainer)
            {
                SelectedRoute = Using<IComboPopUp>(container).ShowDlg(obj) as Route;
                
            }
        }

        private void SalesmanDropDownOpened(object obj)
        {
            using (var container = NestedContainer)
            {
                SelectedSalesman = Using<IComboPopUp>(container).ShowDlg(obj) as DistributorSalesman;

            }
        }

        private void RouteChanged()
        {
            SalesmanLookUp.Clear();
            SalesmanLookUp.Add(new DistributorSalesman(Guid.Empty) { Name = "------Select Salesman-------  " });
            if (SelectedRoute != null && SelectedRoute.Id != Guid.Empty)
            {
                using (var container = NestedContainer)
                {

                    var salesmen =
                        Using<ISalesmanRouteRepository>(container).GetAll().Where(s => s.Route.Id == SelectedRoute.Id).
                            Select(s => s.DistributorSalesmanRef.Id);
                        Using<ICostCentreRepository>(container).GetAll().OfType<DistributorSalesman>().Where(s=>salesmen.Contains(s.Id)).OrderBy(s => s.Name).ToList().ForEach(f => SalesmanLookUp.Add(f));
                   
                }
                LoadOrderPendingDispatch();
            }
          
            
        }
        
        public const string SelectedRoutePropertyName = "SelectedRoute";
        private Route _selectedRoute = null;
        public Route SelectedRoute
        {
            get
            {
                return _selectedRoute;
            }

            set
            {
                if (_selectedRoute == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedRoutePropertyName);
                _selectedRoute = value;
                RaisePropertyChanged(SelectedRoutePropertyName);
            }
        }

        
        public const string SelectedSalesmanPropertyName = "SelectedSalesman";
        private DistributorSalesman _distributorSalesman = null;
        public DistributorSalesman SelectedSalesman
        {
            get
            {
                return _distributorSalesman;
            }

            set
            {
                if (_distributorSalesman == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedSalesmanPropertyName);
                _distributorSalesman = value;
                RaisePropertyChanged(SelectedSalesmanPropertyName);
            }
        }
       
        public const string IsCheckedPropertyName = "IsChecked";
        private bool _checked = false;
        public bool IsChecked
        {
            get
            {
                return _checked;
            }

            set
            {
                if (_checked == value)
                {
                    return;
                }

                RaisePropertyChanging(IsCheckedPropertyName);
                _checked = value;
                RaisePropertyChanged(IsCheckedPropertyName);
            }
        }

        public const string ShowDispatchMessagePropertyName = "ShowDispatchMessage";
        private bool _showDispatchMessage =true;
        public bool ShowDispatchMessage
        {
            get
            {
                return _showDispatchMessage;
            }

            set
            {
                if (_showDispatchMessage == value)
                {
                    return;
                }

                RaisePropertyChanging(ShowDispatchMessagePropertyName);
                _showDispatchMessage = value;
                RaisePropertyChanged(ShowDispatchMessagePropertyName);
            }
        }
    }
}