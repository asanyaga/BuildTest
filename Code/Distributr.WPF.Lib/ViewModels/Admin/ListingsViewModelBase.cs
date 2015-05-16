using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.Util;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using GalaSoft.MvvmLight.Command;

namespace Distributr.WPF.Lib.ViewModels.Admin
{
    public abstract class ListingsViewModelBase : DistributrViewModelBase
    {
        #region properties

        public const string ShowInactivePropertyName = "ShowInactive";
        private bool _showInactive = false;

        public bool ShowInactive
        {
            get { return _showInactive; }
            set
            {
                if (_showInactive == value)
                {
                    return;
                }
                var oldValue = _showInactive;
                _showInactive = value;
                RaisePropertyChanged(ShowInactivePropertyName);
            }
        }

        public const string CurrentPagePropertyName = "CurrentPage";
        private int _currentPage = 1;
        protected int CurrentPage
        {
            get
            {
                if (_currentPage < 1) _currentPage = 1;
                return _currentPage;
            }

            set
            {
                if (_currentPage == value)
                {
                    return;
                }

                RaisePropertyChanging(CurrentPagePropertyName);
                _currentPage = value;
                RaisePropertyChanged(CurrentPagePropertyName);
            }
        }

        public const string ItemsPerPagePropertyName = "ItemsPerPage";
        private int _itemsPerPage = 10;
        protected int ItemsPerPage
        {
            get
            {
                if (_itemsPerPage < 10) _itemsPerPage = 10;
                return _itemsPerPage;
            }

            set
            {
                if (_itemsPerPage == value)
                {
                    return;
                }

                RaisePropertyChanging(ItemsPerPagePropertyName);
                _itemsPerPage = value;
                RaisePropertyChanged(ItemsPerPagePropertyName);
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
         
        public const string PageTitlePropertyName = "PageTitle";
        private string _pageTitle = "";
        public string PageTitle
        {
            get
            {
                return _pageTitle;
            }

            set
            {
                if (_pageTitle == value)
                {
                    return;
                }

                RaisePropertyChanging(PageTitlePropertyName);
                _pageTitle = value;
                RaisePropertyChanged(PageTitlePropertyName);
            }
        }

        public ObservableCollection<uint> PageSizes
        {
            get { return new ObservableCollection<uint> {10, 20, 50, 100}; }
        }

        private RelayCommand<PageDestinations> _goToPageCommand;
        public RelayCommand<PageDestinations> GoToPageCommand
        {
            get { return _goToPageCommand ?? (_goToPageCommand = new RelayCommand<PageDestinations>(GoToPage)); }
        }

        private RelayCommand<int> _comboPageSizesSelectionChangedCommand;
        public RelayCommand<int> ComboPageSizesSelectionChangedCommand
        {
            get
            {
                return _comboPageSizesSelectionChangedCommand ??
                       (_comboPageSizesSelectionChangedCommand = new RelayCommand<int>(ComboPageSizesSelectionChanged));
            }
        }

        private RelayCommand<bool> _loadListingPageCommand = null;
        public RelayCommand<bool> LoadListingPageCommand
        {
            get { return _loadListingPageCommand ?? (_loadListingPageCommand = new RelayCommand<bool>(Load)); }
        }

        private RelayCommand<string> _searchCommand;
        public RelayCommand<string> SearchCommand
        {
            get { return _searchCommand ?? (_searchCommand = new RelayCommand<string>(Search)); }
        }

        private RelayCommand<bool> _toggleShowInactiveCommand;

        public RelayCommand<bool> ToggleShowInactiveCommand
        {
            get
            {
                return _toggleShowInactiveCommand ??
                       (_toggleShowInactiveCommand = new RelayCommand<bool>(ToggleShowInactive));
            }
        }

        private RelayCommand _editSelectedCommand;
        public RelayCommand EditSelectedCommand { get { return _editSelectedCommand ?? (_editSelectedCommand = new RelayCommand(EditSelected)); } }

        //private RelayCommand _farmerCommand;
        //public RelayCommand FarmerCommand { get{return _farmerCommand??(_farmerCommand=new RelayCommand(AddFarmer)} }


        private RelayCommand _deleteSelectedCommand;
        public RelayCommand DeleteSelectedCommand { get { return _deleteSelectedCommand ?? (_deleteSelectedCommand = new RelayCommand(DeleteSelected)); } }

        private RelayCommand _activateSelectedCommand;
        public RelayCommand ActivateSelectedCommand { get { return _activateSelectedCommand ?? (_activateSelectedCommand = new RelayCommand(ActivateSelected)); } }

        public const string PagenationDetailsPropertyName = "PagenationDetails";
        private string _pagenationDetails = "";

        public string PagenationDetails
        {
            get { return _pagenationDetails; }

            set
            {
                if (_pagenationDetails == value)
                {
                    return;
                }

                RaisePropertyChanging(PagenationDetailsPropertyName);
                _pagenationDetails = value;
                RaisePropertyChanged(PagenationDetailsPropertyName);
            }
        }

        public const string FirstPageButtonIsEnabledPropertyName = "FirstPageButtonIsEnabled";
        private bool _firstPageButtonIsEnabled = false;

        public bool FirstPageButtonIsEnabled
        {
            get { return _firstPageButtonIsEnabled; }

            set
            {
                if (_firstPageButtonIsEnabled == value)
                {
                    return;
                }

                RaisePropertyChanging(FirstPageButtonIsEnabledPropertyName);
                _firstPageButtonIsEnabled = value;
                RaisePropertyChanged(FirstPageButtonIsEnabledPropertyName);
            }
        }

        public const string PrevPageButtonIsEnabledPropertyName = "PrevPageButtonIsEnabled";
        private bool _prevPageButtonIsEnabled = false;

        public bool PrevPageButtonIsEnabled
        {
            get { return _prevPageButtonIsEnabled; }

            set
            {
                if (_prevPageButtonIsEnabled == value)
                {
                    return;
                }

                RaisePropertyChanging(PrevPageButtonIsEnabledPropertyName);
                _prevPageButtonIsEnabled = value;
                RaisePropertyChanged(PrevPageButtonIsEnabledPropertyName);
            }
        }

        public const string NextPageButtonIsEnabledPropertyName = "NextPageButtonIsEnabled";
        private bool _nextPageButtonIsEnabled = false;

        public bool NextPageButtonIsEnabled
        {
            get { return _nextPageButtonIsEnabled; }

            set
            {
                if (_nextPageButtonIsEnabled == value)
                {
                    return;
                }

                RaisePropertyChanging(NextPageButtonIsEnabledPropertyName);
                _nextPageButtonIsEnabled = value;
                RaisePropertyChanged(NextPageButtonIsEnabledPropertyName);
            }
        }

        public const string LastPageButtonIsEnabledPropertyName = "LastPageButtonIsEnabled";
        private bool _lastPageButtonIsEnabled = false;
        public bool LastPageButtonIsEnabled
        {
            get { return _lastPageButtonIsEnabled; }

            set
            {
                if (_lastPageButtonIsEnabled == value)
                {
                    return;
                }

                RaisePropertyChanging(LastPageButtonIsEnabledPropertyName);
                _lastPageButtonIsEnabled = value;
                RaisePropertyChanged(LastPageButtonIsEnabledPropertyName);
            }
        }

        #endregion

        #region abstract methods

        protected abstract void Load(bool isFirstLoad = false);

        protected void ToggleShowInactive(bool showInactive)
        {
            ShowInactive = showInactive;
            Load();
        }

        protected abstract void EditSelected();

        protected abstract void ActivateSelected();

        protected abstract void DeleteSelected();

        protected abstract void GoToPage(PageDestinations page);

        protected abstract void ComboPageSizesSelectionChanged(int take);

        protected abstract void UpdatePagenationControl();

        #endregion

        #region base methods

        protected void GoToPageBase(PageDestinations page, int pageCount)
        {
            switch (page)
            {
                case PageDestinations.First:
                    if (CurrentPage > 1 || CurrentPage == 0)
                        CurrentPage = 1;
                    break;
                case PageDestinations.Previous:
                    if (CurrentPage > 1)
                        CurrentPage -= 1;
                    break;
                case PageDestinations.Next:
                    if (pageCount > 1 && CurrentPage < pageCount)
                        CurrentPage += 1;
                    break;
                case PageDestinations.Last:
                    if (CurrentPage < pageCount)
                        CurrentPage = pageCount;
                    break;
            }
            Load();

        }

        protected void UpdatePagenationControlBase(int currentPage, int pageCount, int totalItems, bool isFirstPage,
                                                   bool isLastPage)
        {
            PagenationDetails = string.Format("Showing page {0} of {1} pages :: {2} Records returned.",
                                              currentPage, pageCount, totalItems);

            FirstPageButtonIsEnabled = !isFirstPage;
            PrevPageButtonIsEnabled = !isFirstPage;
            NextPageButtonIsEnabled = !isLastPage;
            LastPageButtonIsEnabled = !isLastPage;
        }

        void Search(string searchText)
        {
            SearchText = searchText;
           
            Load();
        }
        #endregion

    }

    #region utils

    public enum PageDestinations
    {
        First = 1,
        Previous = 2,
        Next = 3,
        Last = 4
    }

    public static class BooleanHelper
    {
        public static bool False
        {
            get { return false; }
        }
        public static bool True
        {
            get { return true; }
        }
    }

    #endregion


}
 