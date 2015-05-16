using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using Distributr.Core.Domain.Master.Util;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.OrderDocumentEntities;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using Distributr.Core.Utility;
using Distributr.DataImporter.Lib.Utils;
using Distributr.WPF.Lib.ViewModels.Admin;
using Distributr.WPF.Lib.ViewModels.Transactional.Orders;

namespace Distributr.DataImporter.Lib.ViewModel.FCL
{
    public class ListImportOrderViewModel : FCLListingBaseViewModel
    {
        private IPagenatedList<OrderItemSummary> _pagedItemSummaries;
        public ListImportOrderViewModel()
        {
            OrdersSummaryList=new ObservableCollection<OrderItemSummary>();
            
        }

        internal IPagenatedList<MainOrderSummary> PagedList;

        #region methods

        protected override void Load(bool isFirstLoad = false)
        {
            if (isFirstLoad)
                Setup();

            if (LoadForExport)
                LoadConfirmedOrders();
            else
            {
                LoadImportOrders();
            }
           
        }

        internal void LoadImportOrders()
        {
            Application.Current.Dispatcher.BeginInvoke(
                new Action(() => OrdersSummaryList.Clear()
                    ));
        }

        

        private bool IsSaved = false;
        private async void DumpExportFilesAsync(string orders)
        {
            SelectedPath = FileUtility.GetExportsDirectory().ToLower();
            if(SelectedPath=="not defined")
            {
                MessageBox.Show("Working folder MUST be defined first", "Import tool warning", MessageBoxButton.OK);
                IsSaved=false;
                return;
            }
            try
            {
                string fileName = Path.Combine(SelectedPath, @"ExportOrders-" + DateTime.Now.ToString("ddmmyyy").Replace(':', '-') + ".csv");
                using (var fs = new FileStream(fileName, FileMode.OpenOrCreate))
                {
                    fs.Close();
                    using (var wr = new StreamWriter(fileName, false))
                    {
                        await wr.WriteLineAsync(orders);

                    }
                }
                IsSaved = true;
            }catch(IOException ex)
            {
                MessageBox.Show("Error copying import oders\nDetails=>" + ex.Message);
                IsSaved = false;
            }
            
        }

        protected override void UploadSelected()
        {
            if (!OrdersSummaryList.Any(p=>p.IsChecked)) return;
            
            var selected = OrdersSummaryList.Where(p => p.IsChecked).ToList();
            if(selected.Any())
            {
                string orders = selected.ToCsv();
                  DumpExportFilesAsync(orders);
                if(IsSaved)
                {
                    foreach (var order in selected)
                    {
                        OrdersSummaryList.Remove(order);
                    }
                    MainWindowViewModel.ImportStatusMessage = string.Format("{0} orders copied to export folder",
                                                                            selected.Count);
                }

            }
        }

        
        protected override void UploadAll()
        {
            if(!OrdersSummaryList.Any())return;
            string orders = OrdersSummaryList.ToCsv();
            DumpExportFilesAsync(orders);
            if (IsSaved)
            {
                MainWindowViewModel.ImportStatusMessage = string.Format("{0} orders copied to export folder",
                                                                        OrdersSummaryList.Count);
                OrdersSummaryList.Clear();
            }
        }

        internal void LoadConfirmedOrders()
       {
           Application.Current.Dispatcher.BeginInvoke(
               new Action(
                   () =>
                       {
                           using (var c = NestedContainer)
                           {
                               var orders = Using<IMainOrderRepository>(c)
                                   .PagedDocumentList(CurrentPage, ItemsPerPage, StartDate, EndDate,
                                                      OrderType.OutletToDistributor, DocumentStatus.Confirmed,
                                                      SearchText);
                                 
                               
                               var items = orders
                                   .Select((n, i) => new OrderItemSummary()
                                                         {
                                                             SequenceNo = i + 1,
                                                             OrderId = n.OrderId,
                                                             TotalVat = n.TotalVat,
                                                             GrossAmount = n.GrossAmount,
                                                             NetAmount = n.NetAmount,
                                                             Required = n.Required,
                                                             OutstandingAmount =
                                                                 FormatOutstandingAmount(n.OutstandingAmount),
                                                             OrderReference = n.OrderReference,
                                                             PaidAmount = n.PaidAmount,
                                                             Status = n.Status,
                                                             Salesman = n.Salesman,
                                                             ShippingAddress = GetShippingAddress(n.OrderId),
                                                             IsChecked = false
                                                             
                                                         }).AsQueryable();

                               _pagedItemSummaries = new PagenatedList<OrderItemSummary>(items, CurrentPage, ItemsPerPage,
                                                                                                  items.Count());
                               OrdersSummaryList.Clear();
                              
                               _pagedItemSummaries.ToList().ForEach(OrdersSummaryList.Add);

                               UpdatePagenationControl();
                           }
                       }));
       }

        

        private string GetShippingAddress(Guid orderId)
        {
            using (var c =NestedContainer)
            {
                var order = Using<IMainOrderRepository>(c).GetById(orderId);
                if (order != null)
                    return order.ShipToAddress;

            }
            return string.Empty;
        }

        protected override void GoToPage(PageDestinations page)
        {
            GoToPageBase(page, _pagedItemSummaries.PageCount);
        }

        protected override void ComboPageSizesSelectionChanged(int take)
        {
            ItemsPerPage = take;
            Load();
        }

        protected override void UpdatePagenationControl()
        {
            UpdatePagenationControlBase(_pagedItemSummaries.PageNumber, _pagedItemSummaries.PageCount, _pagedItemSummaries.TotalItemCount,
                                      _pagedItemSummaries.IsFirstPage, _pagedItemSummaries.IsLastPage);
          
        }

        private void Setup()
        {
            OrdersSummaryList.Clear();
            UploadStatusMessage = "";
            IsUploadSuccess = false;
            if (LoadForExport)
            {
                PageTitle = "Orders for export";
                LoadedImportItem = "ordersexport";
                ShowExportControl=Visibility.Visible;
                ShowImportControl = Visibility.Collapsed;
                
            }
            else
            {
                LoadedImportItem = "orders";
                PageTitle = "Import Orders";
                ShowExportControl = Visibility.Collapsed;
                ShowImportControl = Visibility.Visible;
            }
           
        }
        private string FormatOutstandingAmount(decimal amount)
        {
            if (amount < 0) //this is an overpayment
            {
                return string.Format("(" + ((amount) * -1).ToString("0.00") + ")");

            }
            return amount.ToString("0.00");
        }
#endregion
        
        #region properties
        public ObservableCollection<OrderItemSummary> OrdersSummaryList { get; set; } 
        public const string StartDatePropertyName = "StartDate";
        private DateTime _startDate = DateTime.Now;
        public DateTime StartDate
        {
            get { return _startDate; }

            set
            {
                if (_startDate == value)
                {
                    return;
                }

                RaisePropertyChanging(StartDatePropertyName);
                _startDate = value;
                RaisePropertyChanged(StartDatePropertyName);
            }
        }
        

        public const string EndDatePropertyName = "EndDate";
        private DateTime _endDate = DateTime.Now;
        public DateTime EndDate
        {
            get { return _endDate; }

            set
            {
                if (_endDate == value)
                {
                    return;
                }

                RaisePropertyChanging(EndDatePropertyName);
                _endDate = value;
                RaisePropertyChanged(EndDatePropertyName);
            }
        }

        public const string SelectedOrderSummaryItemPropertyName = "SelectedOrderSummaryItem";
        private OrderItemSummary _selectedOrderSummaryItem = null;
        public OrderItemSummary SelectedOrderSummaryItem
        {
            get { return _selectedOrderSummaryItem; }

            set
            {
                if (_selectedOrderSummaryItem == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedOrderSummaryItemPropertyName);
                _selectedOrderSummaryItem = value;
                RaisePropertyChanged(SelectedOrderSummaryItemPropertyName);
            }
        }
        public const string LoadForExportPropertyName = "LoadForExport";
        private bool _loadForExport = false;
        public bool LoadForExport
        {
            get { return _loadForExport; }

            set
            {
                if (_loadForExport == value)
                {
                    return;
                }

                RaisePropertyChanging(LoadForExportPropertyName);
                _loadForExport = value;
                RaisePropertyChanged(LoadForExportPropertyName);
            }
        }

        public const string ShowImportControlPropertyName = "ShowImportControl";
        private Visibility _showImportControl = Visibility.Visible;
        public Visibility ShowImportControl
        {
            get { return _showImportControl; }

            set
            {
                if (_showImportControl == value)
                {
                    return;
                }

                RaisePropertyChanging(ShowImportControlPropertyName);
                _showImportControl = value;
                RaisePropertyChanged(ShowImportControlPropertyName);
            }
        }
        public const string ShowExportControlPropertyName = "ShowExportControl";
        private Visibility _showExportControl = Visibility.Collapsed;
        public Visibility ShowExportControl
        {
            get { return _showExportControl; }

            set
            {
                if (_showExportControl == value)
                {
                    return;
                }

                RaisePropertyChanging(ShowExportControlPropertyName);
                _showExportControl = value;
                RaisePropertyChanged(ShowExportControlPropertyName);
            }
        }
        
        
        #endregion

        #region Unused inherited abstract methods
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

        internal void IsExport(bool isexport=false)
        {
            LoadForExport = isexport;
        }


    }
}
