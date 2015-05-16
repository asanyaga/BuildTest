using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using Distributr.WPF.Lib.Service.Utility;
using Distributr.WPF.Lib.Services.Service.Utility;
using GalaSoft.MvvmLight.Command;
using System.Linq;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Master.CostCentreEntities;
using GalaSoft.MvvmLight.Threading;

namespace Distributr.WPF.Lib.ViewModels.Transactional.Purchasing
{
    public class ListPurchaseOrderViewModel : DistributrViewModelBase
    {
        

        public ListPurchaseOrderViewModel()
        {
            
            LoadOrders = new RelayCommand(LoadPurchaseOrder);
            AddOrder = new RelayCommand(RunAddOrder);
            ClearSearchCommand = new RelayCommand(ClearSearch);
            SearchPurchaseOrderCommand = new RelayCommand(SearchPurchaseOrder);
            PurchaseOrders = new ObservableCollection<ListPurchaseOrderItem>();
            using (var container = NestedContainer)
            {
              
                IConfigService _configService = Using<IConfigService>(container);
                CanAddPurchaseOrder = _configService.ViewModelParameters.CurrentUserRights.CanCreatPurchaseOrders;
            }
        }

        public RelayCommand AddOrder { get; set; }
        public RelayCommand SearchPurchaseOrderCommand { get; set; }
        public RelayCommand ClearSearchCommand { get; set; }

        void ClearSearch()
        {
            PurchaseOrders.Clear();
            SearchText = "";
            RunLoadOrders();
        }

        void SearchPurchaseOrder()
        {
            if (SearchText == "")
            {
                MessageBox.Show("Enter Search text", " Purchase Order summary", MessageBoxButton.OKCancel);
                return;
            }
            else
            {
                PurchaseOrders.Clear();
                RunLoadOrders();
            }

        }
        void RunAddOrder()
        {
            SendNavigationRequestMessage(new Uri("/views/purchasing/editpurchaseorder.xaml", UriKind.Relative));
        }

        public RelayCommand LoadOrders { get; set; }
        public void LoadPurchaseOrder()
        {
            SearchText = "";
            PurchaseOrders.Clear();
            RunLoadOrders();
        }


        void RunLoadOrders()
        {
            using (var container = NestedContainer)
            {
                IOrderRepository _orderService = Using<IOrderRepository>(container);
                IConfigService _configService = Using<IConfigService>(container);
                
                IEnumerable<Order> orders;
                DocumentStatus status = DocumentStatus.New;
                if (TabItemName == "tabItemIncomplete")
                    status = DocumentStatus.New;
                else if (TabItemName == "tabItemPendingAproval")
                    status = DocumentStatus.Confirmed;
                else if (TabItemName == "tabItemApproved")
                    status = DocumentStatus.Approved;
                else if (TabItemName == "tabItemRejected")
                    status = DocumentStatus.Rejected;

                if (SearchText != "")
                    orders =
                        _orderService.GetDistributorPurchaseOrdersToProducer(_configService.Load().CostCentreId, status)
                            .Where(p => p.DocumentReference.ToLower().Contains(SearchText.ToLower()));
                else
                    orders = _orderService.GetDistributorPurchaseOrdersToProducer(_configService.Load().CostCentreId,
                                                                                  status);
                DispatcherHelper.CheckBeginInvokeOnUI( 
                    () =>
                        {
                            PurchaseOrders.Clear();
                            orders.OrderByDescending(n => n.DocumentDateIssued).ToList()
                                  .ForEach(
                                      n => PurchaseOrders.Add(
                                          new ListPurchaseOrderItem
                                              {
                                                  DateRequired = n.DateRequired.ToString("dd-MMM-yyyy"),
                                                  DocIssuerInfo = DocIssuerInfo(n),
                                                  DocumentRef = n.DocumentReference,
                                                  TotalGross = n.TotalGross,
                                                  TotalNet = n.TotalNet,
                                                  TotalVat = n.TotalVat,
                                                  OrderId = n.Id,
                                                  Status = n.Status.ToString()
                                                 
                                              }
                                               ));
                        });



                TotalGross = orders.Sum(n => n.TotalGross);
            }
        }


        public ObservableCollection<ListPurchaseOrderItem> PurchaseOrders { get; set; }

        string DocIssuerInfo(Order o)
        {
            using (var container = NestedContainer)
            {
                
                ICostCentreRepository _costCentreService = Using<ICostCentreRepository>(container);
                IUserRepository _userService = Using<IUserRepository>(container);
                CostCentre cc = _costCentreService.GetById(o.DocumentIssuerCostCentre.Id);
                User u = _userService.GetById(o.DocumentIssuerUser == null ? Guid.Empty : o.DocumentIssuerUser.Id);
                return string.Format("{0} ({1})", u == null ? string.Empty : u.Username, cc.Name);
            }
        }
        /// <summary>
        /// The <see cref="PageProgressBar" /> property's name.
        /// </summary>
        public const string PageProgressBarPropertyName = "PageProgressBar";
        private string _pageProgressBar = "";
        public string PageProgressBar
        {
            get
            {
                return _pageProgressBar;
            }

            set
            {
                if (_pageProgressBar == value)
                {
                    return;
                }

                var oldValue = _pageProgressBar;
                _pageProgressBar = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(PageProgressBarPropertyName);

            }
        }

        public const string TotalGrossPropertyName = "TotalGross";
        private decimal _totalGross = 0;
        public decimal TotalGross
        {
            get
            {
                return _totalGross;
            }

            set
            {
                if (_totalGross == value)
                    return;
                var oldValue = _totalGross;
                _totalGross = value;
                RaisePropertyChanged(TotalGrossPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="SearchText" /> property's name.
        /// </summary>
        public const string SearchTextPropertyName = "SearchText";

        private string _searchtext = "";

        public string SearchText
        {
            get
            {
                return _searchtext;
            }

            set
            {
                if (_searchtext == value)
                {
                    return;
                }

                var oldValue = _searchtext;
                _searchtext = value;



                // Update bindings, no broadcast
                RaisePropertyChanged(SearchTextPropertyName);


            }
        }

        /// <summary>
        /// The <see cref="TabItemName" /> property's name.
        /// </summary>
        public const string TabItemNamePropertyName = "TabItemName";
        private string _tabItemName = "tabItemIncomplete";
        public string TabItemName
        {
            get
            {
                return _tabItemName;
            }

            set
            {
                if (_tabItemName == value)
                {
                    return;
                }

                var oldValue = _tabItemName;
                _tabItemName = value;


                // Update bindings, no broadcast
                RaisePropertyChanged(TabItemNamePropertyName);


            }
        }

        public const string CanAddPurchaseOrderPropertyName = "CanAddPurchaseOrder";
        private bool _canAddPurchaseOrder = false;
        public bool CanAddPurchaseOrder
        {
            get
            {
                return _canAddPurchaseOrder;
            }

            set
            {
                if (_canAddPurchaseOrder == value)
                {
                    return;
                }

                var oldValue = _canAddPurchaseOrder;
                _canAddPurchaseOrder = value;
                RaisePropertyChanged(CanAddPurchaseOrderPropertyName);
            }
        }
    }

    public class ListPurchaseOrderItem : INotifyPropertyChanged
    {
        public Guid OrderId { get; set; }
        public string DocumentRef { get; set; }
        public string DateRequired { get; set; }
        public string DocIssuerInfo { get; set; }
        public string Status { get; set; }
        public decimal TotalNet { get; set; }
        public decimal TotalVat { get; set; }
        public decimal TotalGross { get; set; }
        public bool IsEditable { get { return Status == "New"; } }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }



}