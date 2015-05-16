using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.Util;
using GalaSoft.MvvmLight.Command;

namespace Distributr.WPF.Lib.ViewModels.Admin
{
    public abstract class ListingsViewModelBase : DistributrViewModelBase
    {
        protected IPagenatedList<MasterEntity> CurrentList { get; set; }

        public ObservableCollection<uint> PageSizes
        {
            get { return new ObservableCollection<uint> {10,20,50,100}; }
        }
        private RelayCommand<PageDestinations> _goToPageCommand;
        public RelayCommand<PageDestinations> GoToPageCommand
        {
            get { return _goToPageCommand ?? (_goToPageCommand = new RelayCommand<PageDestinations>(GoToPage)); }
        }

        private RelayCommand<uint> _comboPageSizesSelectionChangedCommand;
        public RelayCommand<uint> ComboPageSizesSelectionChangedCommand
        {
            get
            {
                return _comboPageSizesSelectionChangedCommand ??
                       (_comboPageSizesSelectionChangedCommand = new RelayCommand<uint>(ComboPageSizesSelectionChanged));
            }
        }

        private RelayCommand _loadPageCommand = null;
        public RelayCommand LoadPageCommand
        {
            get { return _loadPageCommand ?? (_loadPageCommand = new RelayCommand(Load)); }
        }
         
        //public const string PagenationDetailsPropertyName = "PagenationDetails";
        //private string _pagenationDetails = "";
        //public string PagenationDetails
        //{
        //    get
        //    {
        //        return _pagenationDetails;
        //    }

        //    set
        //    {
        //        if (_pagenationDetails == value)
        //        {
        //            return;
        //        }

        //        RaisePropertyChanging(PagenationDetailsPropertyName);
        //        _pagenationDetails = value;
        //        RaisePropertyChanged(PagenationDetailsPropertyName);
        //    }
        //}

        public string PagenationDetails { get; set; }
        public bool FirstPageButtonIsEnabled { get; set; }
        public bool PrevPageButtonIsEnabled { get; set; }

        protected abstract void Load();
        
        protected abstract void GoToPage(PageDestinations page);
        protected abstract void ComboPageSizesSelectionChanged(uint take);
        
        protected void UpdatePagenationControl()
        {
            PagenationDetails = string.Format("Showing page {0} of {1} pages :: {2} Records returned.",
                                              CurrentList.PageNumber, CurrentList.PageCount, CurrentList.TotalItemCount);
            FirstPageButtonIsEnabled = !CurrentList.IsFirstPage;
            PrevPageButtonIsEnabled = !CurrentList.IsFirstPage;
        }
    }

    public enum PageDestinations
    {
        First = 1,
        Previous = 2,
        Next = 3,
        Last = 4
    }

}
 