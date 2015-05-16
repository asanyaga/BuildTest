using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight.Command;

namespace Distributr.WPF.Lib.ViewModels.Utils
{
    public class ComboPopUpViewModel : DistributrViewModelBase
    {
        public ComboPopUpViewModel()
        {
            ComboPopUpItemList = new List<ComboPopUpItem>();
            DataGridItems = new ObservableCollection<ComboPopUpItem>();
            CancelCommand = new RelayCommand(Cancel);
        }

        

        #region Properties

        public RelayCommand CancelCommand { get; set; }
        public List<ComboPopUpItem> ComboPopUpItemList { get; set; }
        public event EventHandler RequestClose = (s, e) => { };
        public ObservableCollection<ComboPopUpItem> DataGridItems { get; set; }

        public const string SearchTextPropertyName = "SearchText";
        private string _searchText = "";
        public string SearchText
        {
            get
            {
                return _searchText;
            }

            set
            {
                if (_searchText == value)
                {
                    return;
                }

                var oldValue = _searchText;
                _searchText = value;

                RaisePropertyChanged(SearchTextPropertyName);
            }
        }
         
        public const string SelectedItemPropertyName = "SelectedItem";
        private ComboPopUpItem _selectedItem = null;
        public ComboPopUpItem SelectedItem
        {
            get
            {
                return _selectedItem;
            }

            set
            {
                if (_selectedItem == value)
                {
                    return;
                }

                var oldValue = _selectedItem;
                _selectedItem = value;

                RaisePropertyChanged(SelectedItemPropertyName);
            }
        }
         
        public const string CodeVisiblePropertyName = "CodeVisible";
        private bool _codeVisible = false;
        public bool CodeVisible
        {
            get
            {
                return _codeVisible;
            }

            set
            {
                if (_codeVisible == value)
                {
                    return;
                }

                var oldValue = _codeVisible;
                _codeVisible = value;

                RaisePropertyChanged(CodeVisiblePropertyName);
            }
        }

        #endregion

        #region Methods
        private void Cancel()
        {
            SelectedItem = null;
            RequestClose(this, EventArgs.Empty);
        }
        public void ClearAndSetUp()
        {
            SelectedItem = null;
            ComboPopUpItemList.Clear();
            
            DataGridItems.Clear();
            SearchText = "";
        }

        public void Search()
        {
            string searchText = SearchText.ToLower();
            DataGridItems.Clear();
            if (SearchText == "")
            {
                ComboPopUpItemList.ForEach(DataGridItems.Add);
            }
            else
            {
                ComboPopUpItemList.Where(n =>
                                         n.Name.ToLower().Contains(searchText)
                                         || n.Code.ToLower().Contains(searchText)).ToList()
                    .ForEach(DataGridItems.Add);
            }
            SelectedItem = DataGridItems.FirstOrDefault();
        }
        #endregion
       
    }

    public class ComboPopUpItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }
    public class ComboPopUpEnumItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}