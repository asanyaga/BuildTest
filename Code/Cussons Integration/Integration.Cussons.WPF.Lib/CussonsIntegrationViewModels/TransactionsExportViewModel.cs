using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Distributr.WPF.Lib.ViewModels;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Integration.Cussons.WPF.Lib.ExportService.Orders;
using Integration.Cussons.WPF.Lib.Utils;

namespace Integration.Cussons.WPF.Lib.CussonsIntegrationViewModels
{
    public class TransactionsExportViewModel : DistributrViewModelBase
    {
        public RelayCommand<TextBox> ExportedByExternalRefCommand { get; set; }
        public RelayCommand<SelectionChangedEventArgs> TabSelectionChangedCommand { get; set; }
        public TransactionsExportViewModel()
        {
            TabSelectionChangedCommand = new RelayCommand<SelectionChangedEventArgs>(TabSelectionChanged);
            ExportedByExternalRefCommand = new RelayCommand<TextBox>(ExportByExtRef);
            SearchText = string.Empty;
        }

       

        internal void ReceiveMessage(string msg)
        {
            ExportActivityMessage += "\n" + msg;
            FileUtility.LogCommandActivity(msg);
        }
        protected void TabSelectionChanged(SelectionChangedEventArgs eventArgs)
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(delegate
            {
                if (eventArgs.Source.GetType() != typeof(TabControl))
                    return;

                TabItem tabItem = eventArgs.AddedItems[0] as TabItem;
                LoadSelectedTab(tabItem);
                eventArgs.Handled = true;

            }));
        }

        private void LoadSelectedTab(TabItem selectedTab)
        {
            ExportActivityMessage = "";
            switch (selectedTab.Name)
            {
                case "ordersPendingExportTab":
                    using (var c=NestedContainer)
                    {
                        Using<IOrderExportService>(c).GetAndExportOrders(StartDate, EndDate);
                    }
                    
                    break;
                case "exportByRefTab":
                    using (var c = NestedContainer)
                    {
                       

                        Using<IOrderExportService>(c).GetAndExportOrders(SearchText);
                        SearchText = string.Empty;
                        ShowSearchTabVisibility = Visibility.Collapsed;
                        MessageBox.Show("Export completed");
                    }
                    break;

            }
        }

        private void ExportByExtRef(TextBox textBox)
        {
            Dispatcher.CurrentDispatcher.InvokeAsync((() =>ShowSearchTabVisibility = Visibility.Visible ));
            
            if (textBox != null)
                SearchText = textBox.Text;
            if (string.IsNullOrEmpty(SearchText))
            {
                Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") +
                                       string.Format(" External Ref is invalid,export terminated"));
                ShowSearchTabVisibility=Visibility.Collapsed;
                return;
            }
            
            Dispatcher.CurrentDispatcher.InvokeAsync((() => LoadSelectedTab(new TabItem() {Name = "exportByRefTab"})));

        }
        #region properties

        public const string ShowSearchTabVisibilityPropertyName = "ShowSearchTabVisibility";
        private Visibility _showSearchTabVisibility = Visibility.Collapsed;
        public Visibility ShowSearchTabVisibility
        {
            get { return _showSearchTabVisibility; }

            set
            {
                if (_showSearchTabVisibility == value)
                {
                    return;
                }

                RaisePropertyChanging(ShowSearchTabVisibilityPropertyName);
                _showSearchTabVisibility = value;
                RaisePropertyChanged(ShowSearchTabVisibilityPropertyName);
            }
        }

        public const string ExportActivityMessagePropertyName = "ExportActivityMessage";
        private string _exportActivityMessage = "";
        public string ExportActivityMessage
        {
            get { return _exportActivityMessage; }

            set
            {
                if (_exportActivityMessage == value)
                {
                    return;
                }

                RaisePropertyChanging(ExportActivityMessagePropertyName);
                _exportActivityMessage = value;
                RaisePropertyChanged(ExportActivityMessagePropertyName);
            }
        }

        public const string StartDatePropertyName = "StartDate";
        private DateTime _startDate = DateTime.Today;

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

        public const string SearchTextPropertyName = "SearchText";
        private string _searchText = "";
        protected string SearchText
        {
            get { return _searchText; }

            set
            {
                if (_searchText == value)
                {
                    return;
                }

                RaisePropertyChanging(SearchTextPropertyName);
                _searchText = value;
                RaisePropertyChanged(SearchTextPropertyName);
            }
        }
        #endregion
    }
}
