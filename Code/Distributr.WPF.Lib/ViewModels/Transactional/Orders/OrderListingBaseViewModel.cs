using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using Distributr.Core;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.Util;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.OrderDocumentEntities;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.Impl.Services.Transactional.SaveAndContinue;
using Distributr.WPF.Lib.ViewModels.Admin;
using Distributr.WPF.Lib.ViewModels.Transactional.Fiscalprinter;
using GalaSoft.MvvmLight.Command;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;


namespace Distributr.WPF.Lib.ViewModels.Transactional.Orders
{
    public  class OrdersListingBaseViewModel : ListingsViewModelBase
    {
        protected bool isUnconfirmedTab;
        
       protected OrdersListingBaseViewModel()
       {
           SetupCommand = new RelayCommand(SetUp);
           TabSelectionChangedCommand = new RelayCommand<SelectionChangedEventArgs>(TabSelectionChanged);
           OrdersSummaryList = new ObservableCollection<OrderItemSummary>();
           PaymentInfoList = new ObservableCollection<PaymentInfo>();
           SalesmenList=new ObservableCollection<DistributorSalesman>();
          ClearSearchTextCommand=new RelayCommand(ClearSearchText);
           ExportToCsvCommand=new RelayCommand(ExportSelectedTabItem);
           SetupCommand.Execute(null);
           ContinueSelectedOrderCommand = new RelayCommand<OrderItemSummary>(ContinueSelectedOrder);
           ListOrdersCommand = new RelayCommand(ListOrders);
       }

        protected virtual void ContinueSelectedOrder(OrderItemSummary obj)
        {
            
        }


        public RelayCommand<OrderItemSummary> ContinueSelectedOrderCommand { get; set; }
        public RelayCommand SetupCommand { get; set; }
        public RelayCommand ListOrdersCommand { get; set; }
       public RelayCommand ClearSearchTextCommand { get; set; }
     
       public RelayCommand<SelectionChangedEventArgs> TabSelectionChangedCommand { get; set; }
       public ObservableCollection<OrderItemSummary> OrdersSummaryList { get; set; }
       protected IPagenatedList<MainOrderSummary> PagedDocumentList;
      
       public ObservableCollection<PaymentInfo> PaymentInfoList { get; set; }
       public ObservableCollection<DistributorSalesman> SalesmenList { get; set; }
       public RelayCommand<PaymentInfo> RemovePaymentLineItemCommand { get; set; }
       public RelayCommand<PaymentInfo> ConfirmCommand { get; set; }
       public RelayCommand<OrderItemSummary> ReceivePaymentsCommand { get; set; }
       public RelayCommand ExportToCsvCommand { get; set; }

       protected virtual void TabSelectionChanged(SelectionChangedEventArgs eventArgs)
       {
           CurrentPage = 1;
       }

       private void ExportSelectedTabItem()
       {
          ExportTabItems();
       }

       
        protected virtual void LoadSelectedTab(TabItem selectedTab){}
       protected virtual void SetUp() { }
      
       protected virtual void ClearSearchText()
       {
           SearchText = string.Empty;
       }

       protected override void LoadPage(Page page)
       {
           if (page != null)
           {
              
               if (page.NavigationService != null)
                   SelectedTabItem = PresentationUtility.GetLastTokenFromUri(page.NavigationService.CurrentSource);
            
               if (!string.IsNullOrEmpty(SelectedTabItem))
               {
                   var tab = page.FindName(SelectedTabItem) as TabItem;
                   if (tab != null)
                   { 
                       LoadSelectedTab(tab);
                       PutFocusOnControl(tab);
                   }
               }
               else
               {
                   TabItem tab = GetdefaultTabItem(page);
                   if(tab !=null)
                   {
                       LoadSelectedTab(tab);
                       PutFocusOnControl(tab);
                   }
                   
               }
           }
       }

        static TabItem GetdefaultTabItem(Page page)
        {
            var tabs = FindVisualChildren<TabItem>(page).ToList();
            TabItem tab=null;
            switch (page.Name)
            {
                case "SalesmanOrdersPage":
                    tab = tabs.FirstOrDefault(p => p.Name == "PendingApprovalTab");
                    break;
                case "PurchaseOrdersPage":
                    tab = tabs.FirstOrDefault(p => p.Name == "IncompleteTab");
                    break;
            }
            return tab;
        }

        

        protected DependencyObject GetTopLevelControl(DependencyObject control)
        {
            DependencyObject tmp = control;
            DependencyObject parent = null;
            while ((tmp = VisualTreeHelper.GetParent(tmp)) != null)
            {
                parent = tmp;
            }
            return parent;
        }
       protected virtual string FormatOutstandingAmount(decimal amount)
       {
           if (amount < 0) //this is an overpayment
           {
               return string.Format("(" + ((amount)*-1).ToString("0.00") + ")");

           }

           return amount.ToString("0.00");
       }
      
       protected override void Load(bool isFirstLoad = false)
       {
           if (isFirstLoad)
               SetUp();
          
           ListOrders();
       }
       

       protected async virtual void ListOrders()
       {

          await Task.Factory.StartNew(() =>
                                     {
                                         Application.Current.Dispatcher.BeginInvoke(
                                             new Action(delegate
                                                            {
                                                                if (SelectedOrderStatus != DocumentStatus.Confirmed)
                                                                    ShowApproveSelectedButton = Visibility.Collapsed;
                                                                using (var container = NestedContainer)
                                                                {
                                                                    OrdersSummaryList.Clear();
                                                                    if (isUnconfirmedTab)
                                                                    {
                                                                        var orderSaveAndContinueService =
                                                                            Using<IOrderSaveAndContinueService>(
                                                                                container);
                                                                        var item =orderSaveAndContinueService.Query(
                                                                                StartDate, EndDate, SelectedOrderType);
                                                                        IPagenatedList<OrderSaveAndContinueLater> ItemList =
                                                                               new PagenatedList<OrderSaveAndContinueLater>(
                                                                                   item.AsQueryable(), CurrentPage,
                                                                                   ItemsPerPage, item.Count());
                                                                        ItemList.Select(Map).ToList().
                                                                            ForEach(
                                                                                OrdersSummaryList.Add);
                                                                    }
                                                                    else
                                                                    {


                                                                        //var orders = Using<IMainOrderRepository>(
                                                                        //    container)
                                                                        //    .GetMainOrderSummariyList(StartDate, EndDate,
                                                                        //                              SelectedOrderType,
                                                                        //                              SelectedOrderStatus,
                                                                        //                              SearchText);
                                                                        var orders =
                                                                            Using<IMainOrderRepository>(container).
                                                                                PagedDocumentList(CurrentPage,
                                                                                                  ItemsPerPage,
                                                                                                  StartDate, EndDate,
                                                                                                  SelectedOrderType,
                                                                                                  SelectedOrderStatus,
                                                                                                  SearchText);

                                                                     
                                                                        if (orders != null && orders.Any())
                                                                        {
                                                                            PagedDocumentList =
                                                                                new PagenatedList<MainOrderSummary>(
                                                                                    orders.AsQueryable(), CurrentPage,
                                                                                    ItemsPerPage, orders.TotalItemCount,true);
                                                                            PagedDocumentList.Select(Map).ToList().
                                                                                ForEach(
                                                                                    OrdersSummaryList.Add);

                                                                            UpdatePagenationControl();

                                                                        }
                                                                    }
                                                                   

                                                                }
                                                            }));
                                     });


       }

        private OrderItemSummary Map(MainOrderSummary orderSummary, int count)
        {
            var orderItemSummary = new OrderItemSummary();
            using (var c = NestedContainer)
            {
                var pricingService = Using<IDiscountProWorkflow>(c);


                orderItemSummary.SequenceNo = count + 1;
                orderItemSummary.OrderId = orderSummary.OrderId;
                orderItemSummary.TotalVat = orderSummary.TotalVat;
                orderItemSummary.GrossAmount = orderSummary.GrossAmount.GetTotalGross();
                orderItemSummary.NetAmount =  orderSummary.NetAmount.GetTotalGross();
                orderItemSummary.Required = orderSummary.Required;
                orderItemSummary.SaleDiscount = orderSummary.SaleDiscount;
                orderItemSummary.OutstandingAmount = orderSummary.OutstandingAmount;

                orderItemSummary.OrderReference = orderSummary.OrderReference;
                orderItemSummary.PaidAmount = orderSummary.PaidAmount;
                orderItemSummary.Status = orderSummary.Status;
                orderItemSummary.Salesman = orderSummary.Salesman;
                orderItemSummary.ExRefNo = orderSummary.ExternalRefNo;
                orderItemSummary.Outlet = orderSummary.Outlet;
            }

            return orderItemSummary;
        }

    
       private OrderItemSummary Map(OrderSaveAndContinueLater orderSummary, int count)
       {
           var pricingService = Using<IDiscountProWorkflow>(NestedContainer);

           var orderSummaryMapped= new OrderItemSummary()
           {
               SequenceNo = count + 1,
               OrderId = orderSummary.Id,
               TotalVat = orderSummary.LineItem.Sum(s=>s.UnitVat*s.Quantity),
               GrossAmount = (orderSummary.LineItem.Sum(s =>(s.UnitVat +s.UnitPrice)* s.Quantity).GetTruncatedValue()).GetTotalGross(),

               NetAmount = (orderSummary.LineItem.Sum(s => ((s.UnitVat + s.UnitPrice) * s.Quantity).GetTruncatedValue())).GetTotalGross(),
               Required = orderSummary.Required,
               SaleDiscount = orderSummary.SaleDiscount,
               OutstandingAmount =0,
                   
               OrderReference = "Not Generated",
               PaidAmount = 0,
               Status =DocumentStatus.New,
               Salesman = orderSummary.Salesman,
               ExRefNo = "",
               Outlet = orderSummary.Outlet

           };
           return orderSummaryMapped;

       }

        public ExportorderItem MapExportItem(OrderItemSummary item)
        {
            return new ExportorderItem()
                       {
                           OrderRef = item.OrderReference,
                           Externalref = item.ExRefNo,
                           OutletName = item.Outlet,
                           OrderDate = item.Required.ToShortDateString(),
                           NetAmount = item.NetAmount.ToString("0.00"),
                           PaidAmount = item.PaidAmount.ToString("0.00"),
                           OutStandingAmount = item.OutstandingAmount.ToString(),
                           DocumentStatus = Enum.GetName(typeof(DocumentStatus), item.Status),
                           

                       };
        }

        private ExportorderItem GenerateHeader()
        {
           return new ExportorderItem()
                       {
                           OrderRef = "Generic OrderRef",
                           Externalref = "External Ref",
                           OutletName = "Outlet Name",
                           OrderDate = "OrderDate",
                           NetAmount = "NetAmount",
                           PaidAmount="Amount Paid",
                           OutStandingAmount = "Outstanding Payment",
                           DocumentStatus = "Status",
                       };
        }   

        protected virtual void ExportTabItems()
        {
            string folder =SaveAs();
            if (!string.IsNullOrEmpty(folder))
            {
                var orderItems=new LinkedList<ExportorderItem>();
                orderItems.AddFirst(GenerateHeader());

                foreach (var exportorderItem in OrdersSummaryList.Select(MapExportItem).ToList())
                {
                    orderItems.AddLast(exportorderItem);
                }
                
                using (var sw = new StreamWriter(folder))
                {
                    sw.WriteAsync(orderItems.ToCsv());
                    sw.Close();
                }
                MessageBox.Show("Done", "Distributr Info");
            }
        }
        public static string SaveAs()
        {
            var saveFileDialog1 = new SaveFileDialog();

            string filter = "CSV file (*.csv)|*.csv| All Files (*.*)|*.*";
            saveFileDialog1.Filter = filter;
            saveFileDialog1.Title = "Order Export CSV ";
            saveFileDialog1.AddExtension = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                return saveFileDialog1.FileName;

            }
            return null;
        }
      

        protected override void GoToPage(PageDestinations page)
        {
            GoToPageBase(page, PagedDocumentList.PageCount);
        }

        protected override void ComboPageSizesSelectionChanged(int take)
        {
            ItemsPerPage = take;
            Load();
        }

        protected override void UpdatePagenationControl()
        {
            UpdatePagenationControlBase(PagedDocumentList.PageNumber, PagedDocumentList.PageCount,
                                        PagedDocumentList.TotalItemCount,
                                        PagedDocumentList.IsFirstPage, PagedDocumentList.IsLastPage);
        }

        #region Properties
        

        public const string ReportStartTimePropertyName = "ReportStartTime";
        private DateTime _reportDateTimeUp = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 01);
        public DateTime ReportStartTime
        {
            get
            {
                return _reportDateTimeUp;
            }

            set
            {
                if (Equals(_reportDateTimeUp, value))
                {
                    return;
                }
                RaisePropertyChanging(ReportStartTimePropertyName);
                _reportDateTimeUp = value;
                RaisePropertyChanged(ReportStartTimePropertyName);
            }
        }

        public const string ReportEndTimePropertyName = "ReportEndTime";
        private DateTime _reportEndTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
        public DateTime ReportEndTime
        {
            get
            {
                return _reportEndTime;
            }

            set
            {
                if (_reportEndTime==value)
                {
                    return;
                }
                RaisePropertyChanging(ReportEndTimePropertyName);
                _reportEndTime = value;
                RaisePropertyChanged(ReportEndTimePropertyName);
            }
        }
        public const string SelectedsalesmanPropertyName = "SelectedSalesman";
        private DistributorSalesman _selectedSalesman = null;
        public DistributorSalesman SelectedSalesman 
        {
            get
            {
                return _selectedSalesman;
            }

            set
            {
                if (_selectedSalesman == value)
                {
                    return;
                }
                RaisePropertyChanging(SelectedsalesmanPropertyName);
                _selectedSalesman = value;
                RaisePropertyChanged(SelectedsalesmanPropertyName);
            }
        }

        public const string SelectAllOrdersCheckedPropertyName = "SelectAllOrdersChecked";
        private bool _selectAllOrdersChecked;
        public bool SelectAllOrdersChecked
        {
            get
            {
                return _selectAllOrdersChecked;
            }

            set
            {
                if (_selectAllOrdersChecked == value)
                {
                    return;
                }
                RaisePropertyChanging(SelectAllOrdersCheckedPropertyName);
                _selectAllOrdersChecked = value;
                RaisePropertyChanged(SelectAllOrdersCheckedPropertyName);
            }
        }
       public const string ViewSelectedOrderToolTipPropertyName = "ViewSelectedOrderToolTip";
       private string _viewSelectedOrderToolTip ="Click link for details";
       public string ViewSelectedOrderToolTip
       {
           get
           {
               return _viewSelectedOrderToolTip;
           }

           set
           {
               if (_viewSelectedOrderToolTip == value)
               {
                   return;
               }
               _viewSelectedOrderToolTip = value;
               RaisePropertyChanged(ViewSelectedOrderToolTipPropertyName);
           }
       }
       public const string SelectedTabItemPropertyName = "SelectedTabItemPropertyName";
       private string _selectedTabItem = "";
       public string SelectedTabItem
       {
           get
           {
               return _selectedTabItem;
           }

           set
           {
               if (_selectedTabItem == value)
               {
                   return;
               }
               RaisePropertyChanging(SelectedTabItemPropertyName);
               _selectedTabItem = value;
               RaisePropertyChanged(SelectedTabItemPropertyName);
           }
       }


       public const string OrderIdPropertyName = "OrderId";
       private Guid _orderId = Guid.Empty;
       public Guid OrderId
       {
           get
           {
               return _orderId;
           }

           set
           {
               if (_orderId == value)
               {
                   return;
               }
               var oldValue = _orderId;
               _orderId = value;
               RaisePropertyChanged(OrderIdPropertyName, oldValue, value, true);
           }
       }

       public const string ShowApproveAndDispatchButtonPropertyName = "ShowApproveAndDispatchButton";
       private Visibility _showApproveAndDispatchButton = Visibility.Collapsed;
       public Visibility ShowApproveAndDispatchButton
       {
           get
           {
               return _showApproveAndDispatchButton;
           }

           set
           {
               if (_showApproveAndDispatchButton == value)
               {
                   return;
               }
               RaisePropertyChanging(ShowApproveAndDispatchButtonPropertyName);
               _showApproveAndDispatchButton = value;
               RaisePropertyChanged(ShowApproveAndDispatchButtonPropertyName);
           }
       }


       public const string CanApproveOrderPropertyName = "CanApproveOrder";
       private bool _canApproveOrder = false;
       public bool CanApproveOrder
       {
           get
           {
               return _canApproveOrder;
           }

           set
           {
               if (_canApproveOrder == value)
               {
                   return;
               }

               var oldValue = _canApproveOrder;
               _canApproveOrder = value;
               RaisePropertyChanged(CanApproveOrderPropertyName);
           }
       }
       public const string CanCreateOrdersPropertyName = "CanCreateOrders";
       private bool _canCreateOrders = false;
       public bool CanCreateOrders
       {
           get
           {
               return _canCreateOrders;
           }

           set
           {
               if (_canCreateOrders == value)
               {
                   return;
               }

               var oldValue = _canCreateOrders;
               _canCreateOrders = value;
               RaisePropertyChanged(CanCreateOrdersPropertyName);
           }
       }

       public const string CanDispatchOrdersPropertyName = "CanDispatchOrders";
       private bool _canDispatchOrders = false;
       public bool CanDispatchOrders
       {
           get
           {
               return _canDispatchOrders;
           }

           set
           {
               if (_canDispatchOrders == value)
               {
                   return;
               }

               var oldValue = _canDispatchOrders;
               _canDispatchOrders = value;
               RaisePropertyChanged(CanDispatchOrdersPropertyName);
           }
       }
     
     

       public const string CanReceivePaymentsPropertyName = "CanReceivePayments";
       private bool _canReceivePayments = false;
       public bool CanReceivePayments
       {
           get
           {
               return _canReceivePayments;
           }

           set
           {
               if (_canReceivePayments == value)
               {
                   return;
               }

               var oldValue = _canReceivePayments;
               _canReceivePayments = value;
               RaisePropertyChanged(CanReceivePaymentsPropertyName);
           }
       }

 

       public const string StartDatePropertyName = "StartDate";
       private DateTime _StartDate = DateTime.Now.AddDays(-2);
       public DateTime StartDate
       {
           get
           {
               return _StartDate;
           }

           set
           {
               if (_StartDate == value)
               {
                   return;
               }

               var oldValue = _StartDate;
               _StartDate = value;

               // Update bindings, no broadcast
               RaisePropertyChanged(StartDatePropertyName);
           }
       }

       public const string EndDatePropertyName = "EndDate";
       private DateTime _EndDate = DateTime.Now;
       public DateTime EndDate
       {
           get
           {
               return _EndDate;
           }

           set
           {
               if (_EndDate == value)
               {
                   return;
               }

               var oldValue = _EndDate;
               _EndDate = value;

               // Update bindings, no broadcast
               RaisePropertyChanged(EndDatePropertyName);
           }
       }

       public const string SelectedOrderTypePropertyName = "SelectedOrderType";
       private OrderType _Ordertype = OrderType.DistributorToProducer;
       public OrderType SelectedOrderType
       {
           get
           {
               return _Ordertype;
           }

           set
           {
               if (_Ordertype == value)
               {
                   return;
               }

               var oldValue = _Ordertype;
               _Ordertype = value;

               // Update bindings, no broadcast
               RaisePropertyChanged(SelectedOrderTypePropertyName);
           }
       }

       public const string ShowApproveSelectedButtonPropertyName = "ShowApproveSelectedButton";
       private Visibility _showApproveSelectedButton = Visibility.Collapsed;
       public Visibility ShowApproveSelectedButton
       {
           get
           {
               return _showApproveSelectedButton;
           }

           set
           {
               if (_showApproveSelectedButton == value)
               {
                   return;
               }

               _showApproveSelectedButton = value;

               RaisePropertyChanged(ShowApproveSelectedButtonPropertyName);
           }
       }

       public const string SelectedOrderStatusPropertyName = "SelectedOrderStatus";
       private DocumentStatus _OrderStatus = DocumentStatus.Confirmed;
       public DocumentStatus SelectedOrderStatus
       {
           get
           {
               return _OrderStatus;
           }

           set
           {
               if (_OrderStatus == value)
               {
                   return;
               }

               var oldValue = _OrderStatus;
               _OrderStatus = value;

               // Update bindings, no broadcast
               RaisePropertyChanged(SelectedOrderStatusPropertyName);
           }
       }

       public const string SelectedOrderSummaryItemPropertyName = "SelectedOrderSummaryItem";
       private MainOrderSummary _selectedOrderSummaryItem = null;
       public MainOrderSummary SelectedOrderSummaryItem
       {
           get
           {
               return _selectedOrderSummaryItem;
           }

           set
           {
               if (_selectedOrderSummaryItem == value)
               {
                   return;
               }

               var oldValue = _selectedOrderSummaryItem;
               _selectedOrderSummaryItem = value;
               RaisePropertyChanged(SelectedOrderSummaryItemPropertyName, oldValue, value, true);

           }
       }
       public const string IntegrationActivityMessagePropertyName = "IntegrationActivityMessage";
       private string _integrationActivityMessage = "";
       public string IntegrationActivityMessage
       {
           get { return _integrationActivityMessage; }

           set
           {
               if (_integrationActivityMessage == value)
               {
                   return;
               }

               RaisePropertyChanging(IntegrationActivityMessagePropertyName);
               _integrationActivityMessage = value;
               RaisePropertyChanged(IntegrationActivityMessagePropertyName);
           }
       }
       #endregion

       #region un used inherits

        
        protected override void EditSelected()
       {
           throw new NotImplementedException();
       }

       protected override void ActivateSelected()
       {
           throw new NotImplementedException();
       }

       protected override void DeleteSelected()
       {
           throw new NotImplementedException();
       }
       #endregion
    }

    public class ExportorderItem
    {
        public string OrderRef { get; set; }
        public string Externalref { get; set; }
        public string OutletName { get; set; }
        public string OrderDate { get; set; }
        public string NetAmount { get; set; }
        public string PaidAmount { get; set; }
        public string OutStandingAmount { get; set; }
        public string DocumentStatus { get; set; }
    }
}
