using System;
using System.Collections.ObjectModel;
using System.Windows;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using Distributr.WPF.Lib.Service.Utility;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.ViewModels.Transactional.Orders;
using GalaSoft.MvvmLight.Command;
using System.Linq;
using GalaSoft.MvvmLight.Messaging;

namespace Distributr.WPF.Lib.ViewModels.Transactional.GRN
{
    public class SelectApprovedOrdersViewModel : DistributrViewModelBase
    {
        private ViewModelParameters vmparams;

        public SelectApprovedOrdersViewModel()
        {
            LoadOrders = new RelayCommand(LoadPendingOrders);
            SelectOrders = new RelayCommand(RunSelectOrders);
            CancelSelectOrders = new RelayCommand(RunCancelSelectOrders);
            vmparams = new ViewModelParameters();
            ApprovedOrders = new ObservableCollection<SelectApprovedOrderItem>();
        }

        public RelayCommand LoadOrders { get; set; }
        void LoadPendingOrders()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                ApprovedOrders.Clear();

                var orders = Using<IMainOrderRepository>(c)
                    .GetPurchaseOrderPendingReceive();

                foreach (var o in orders)
                {
                    ApprovedOrders.Add(new SelectApprovedOrderItem
                        {
                            OrderId = o.OrderId,
                            DocumentRef = o.OrderReference,
                            Date = o.Required.ToString("dd-MMM-yyyy"),
                            OrderValue = string.Format("{0:0.00}", o.GrossAmount)
                        });
                }
            }
        }

        public ObservableCollection<SelectApprovedOrderItem> ApprovedOrders { get; set; }
        public ObservableCollection<Guid> SelectedOrderIds { get; set; }
         
        public const string SelectedApprovedOrderItemPropertyName = "SelectedApprovedOrderItem";
        private SelectApprovedOrderItem _selectedApprovedOrderItem = null;
        public SelectApprovedOrderItem SelectedApprovedOrderItem
        {
            get
            {
                return _selectedApprovedOrderItem;
            }

            set
            {
                if (_selectedApprovedOrderItem == value)
                {
                    return;
                }

                _selectedApprovedOrderItem = value;
                RaisePropertyChanged(SelectedApprovedOrderItemPropertyName);
            }
        }

        public const string IsSelectedPropertyName = "IsSelected";
        private bool _isSelected = false;
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }

            set
            {
                if (_isSelected == value)
                {
                    return;
                }

                var oldValue = _isSelected;
                _isSelected = value;

                if (SelectedApprovedOrderItem != null)
                {
                    if (value)
                        SelectOrder();
                    else
                        UnSelectOrder();
                }
                RaisePropertyChanged(IsSelectedPropertyName);

            }
        }

        public RelayCommand SelectOrders { get; set; }
        void RunSelectOrders()
        {
            vmparams.SelectedOrderIds = ApprovedOrders.Where(n => n.IsSelected).Select(n => n.OrderId).ToList();
            if (vmparams.SelectedOrderIds.Any())
            {
                const string uri = "/views/grn/addgrn.xaml";
                //Messenger.Default.Send<ViewModelMessage>(new ViewModelMessage { Id =  });
                NavigateCommand.Execute(uri);
            }
                //  SendNavigationRequestMessage(new Uri("views/grn/addgrn.xaml", UriKind.Relative));
            else
            {
                MessageBox.Show("No items were selected. Please select items to receive.",
                                "Distributr: Receive Inventory", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        public void SelectOrder()
        {
            if (SelectedApprovedOrderItem != null)
                SelectedApprovedOrderItem.IsSelected = true;
        }

        public void UnSelectOrder()
        {
            if (SelectedApprovedOrderItem != null)
                SelectedApprovedOrderItem.IsSelected = false;
        }

        public RelayCommand CancelSelectOrders { get; set; }
        void RunCancelSelectOrders()
        {
            SendNavigationRequestMessage(new Uri("/views/grn/listgrn.xaml", UriKind.Relative));
        }

       
    }

    public class SelectApprovedOrderItem : DistributrViewModelBase
    {
        public Guid OrderId { get; set; }
        public string DocumentRef { get; set; }
        public string Date { get; set; }

        public const string IsSelectedPropertyName = "IsSelected";
        private bool _isSelected = false;
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }

            set
            {
                if (_isSelected == value)
                {
                    return;
                }

                var oldValue = _isSelected;
                _isSelected = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(IsSelectedPropertyName);
               
            }
        }

        public string OrderValue { get; set; }
    }


}
