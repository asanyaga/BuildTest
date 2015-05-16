//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using GalaSoft.MvvmLight;
//using GalaSoft.MvvmLight.Command;

//namespace Distributr.WPF.Lib.ViewModels.Utils
//{
//    public class DistributrOrdersPagerViewModel : ViewModelBase
//    {
//        public DistributrOrdersPagerViewModel()
//        {
//            LoadFirstPageCommand = new RelayCommand(LoadFirstPage);
//            LoadNextPageCommand = new RelayCommand(LoadNextPage);
//            LoadPreviousPageCommand = new RelayCommand(LoadPreviousPage);
//            LoadGotoPageCommand = new RelayCommand(LoadGotoPage);
//            LoadLastPageCommand = new RelayCommand(LoadLastPage);
//        }

//        public RelayCommand LoadFirstPageCommand { get; set; }
//        public RelayCommand LoadPreviousPageCommand { get; set; }
//        public RelayCommand LoadGotoPageCommand { get; set; }
//        public RelayCommand LoadNextPageCommand { get; set; }
//        public RelayCommand LoadLastPageCommand { get; set; }

//        public const string TotalItemCountPropertyName = "TotalItemCount";
//        private int _totalItemCount = 1;
//        public int TotalItemCount
//        {
//            get
//            {
//                return _totalItemCount;
//            }

//            set
//            {
//                if (_totalItemCount == value)
//                {
//                    return;
//                }

//                var oldValue = _totalItemCount;
//                _totalItemCount = value;
//                RaisePropertyChanged(TotalItemCountPropertyName);
//            }
//        }

//        public const string CurrentPagePropertyName = "CurrentPage";
//        private int _currentPage = 1;
//        public int CurrentPage
//        {
//            get
//            {
//                return _currentPage;
//            }

//            set
//            {
//                if (_currentPage == value)
//                {
//                    return;
//                }
//                var oldValue = _currentPage;
//                _currentPage = value;
//                RaisePropertyChanged(CurrentPagePropertyName);
//            }
//        }

//        public const string GotoPagePropertyName = "GotoPage";
//        private int _gotoPage = 1;
//        public int GotoPage
//        {
//            get
//            {
//                return _gotoPage;
//            }

//            set
//            {
//                if (_gotoPage == value)
//                {
//                    return;
//                }
//                var oldValue = _gotoPage;
//                _gotoPage = value;
//                RaisePropertyChanged(GotoPagePropertyName);
//            }
//        }

//        public const string ItemsPerPagePropertyName = "ItemsPerPage";
//        private int _itemsPerPage = 2;
//        public int ItemsPerPage
//        {
//            get
//            {
//                return _itemsPerPage;
//            }

//            set
//            {
//                if (_itemsPerPage == value)
//                {
//                    return;
//                }

//                var oldValue = _itemsPerPage;
//                _itemsPerPage = value;
//                RaisePropertyChanged(ItemsPerPagePropertyName);
//            }
//        }

//        # region Paging

//        protected virtual void LoadLastPage()
//        {
//            GetPagedOrders(TotalPageCount, ItemsPerPage, StartDate, EndDate, OrderType.DistributorToProducer,
//                SelectedOrderStatus, SearchText);
//            CurrentPage = TotalPageCount;
//            GotoPage = CurrentPage;
//        }

//        protected virtual void LoadGotoPage()
//        {
//            if (GotoPage < 1)
//                GotoPage = 1;
//            if (GotoPage > TotalPageCount)
//                GotoPage = TotalPageCount;
//            GetPagedOrders(GotoPage, ItemsPerPage, StartDate, EndDate, OrderType.DistributorToProducer,
//                SelectedOrderStatus, SearchText);
//            CurrentPage = GotoPage;
//        }

//        protected virtual void LoadPreviousPage()
//        {
//            CurrentPage -= 1;
//            if (CurrentPage < 1)
//                CurrentPage = 1;
//            GetPagedOrders(CurrentPage, ItemsPerPage, StartDate, EndDate, OrderType.DistributorToProducer,
//                SelectedOrderStatus, SearchText);
//            GotoPage = CurrentPage;
//        }

//        protected virtual void LoadNextPage()
//        {
//            CurrentPage += 1;
//            if (CurrentPage > TotalPageCount)
//                CurrentPage = TotalPageCount;
//            GetPagedOrders(CurrentPage, ItemsPerPage, StartDate, EndDate, OrderType.DistributorToProducer,
//                SelectedOrderStatus, SearchText);
//            GotoPage = CurrentPage;
//        }

//        protected virtual void LoadFirstPage()
//        {
//            CurrentPage = 1;
//            GetPagedOrders(CurrentPage, ItemsPerPage, StartDate, EndDate, OrderType.DistributorToProducer,
//                SelectedOrderStatus, SearchText);
//            GotoPage = CurrentPage;
//        }

//        protected abstract void GetPagedOrders(int currentPage, int itemsPerPage, DateTime startDate, DateTime endDate,
//                                    OrderType orderType, DocumentStatus orderStatus, string searchText);

//        #endregion 
//    }
//}
