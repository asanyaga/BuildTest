using System;
using Distributr.Core.Domain.Master.Util;
using Distributr.WPF.Lib.ViewModels.Admin;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;

namespace Integration.Cussons.WPF.Lib.CussonsIntegrationViewModels.MasterData
{
    public class MasterDataImportListingsBase : ListingsViewModelBase
    {
        protected internal IPagenatedList<ImportItemVM> PagedList;
        #region Overrides of ListingsViewModelBase

        protected override void Load(bool isFirstLoad = false)
        {
            throw new NotImplementedException();
        }

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

        protected override void GoToPage(PageDestinations page)
        {
            GoToPageBase(page, PagedList.PageCount);
        }

        protected override void ComboPageSizesSelectionChanged(int take)
        {
            ItemsPerPage = take;
            Load();
        }

        protected override void UpdatePagenationControl()
        {
            UpdatePagenationControlBase(CurrentPage, PagedList.PageCount, PagedList.TotalItemCount, PagedList.IsFirstPage,
                                        PagedList.IsLastPage);
        }

        #endregion

        private RelayCommand _uploadSelectedCommand = null;
        public RelayCommand UploadSelectedCommand
        {
            get { return _uploadSelectedCommand ?? (_uploadSelectedCommand = new RelayCommand(UploadSelected)); }
        }

        private RelayCommand _uploadAllCommand = null;
        public RelayCommand UploadAllCommand
        {
            get { return _uploadAllCommand ?? (_uploadAllCommand = new RelayCommand(UploadAll)); }
        }

        private RelayCommand _cancelImportCommand = null;
        public RelayCommand CancelImportCommand
        {
            get { return _cancelImportCommand ?? (_cancelImportCommand = new RelayCommand(CancelImport)); }
        }

        private void CancelImport()
        {
            MainViewModel.GlobalStatus = "";
            NavigateCommand.Execute(@"/Pages/HomePage.xaml");
        }
        protected virtual async void UploadAll()
        {

        }

        protected virtual async void UploadSelected()
        {

        }

        protected CussonsMainWindowViewModel MainViewModel
        {
            get { return SimpleIoc.Default.GetInstance<CussonsMainWindowViewModel>(); }
        }

        public const string SelectedPathPropertyName = "SelectedPath";
        private string _selectedPath = null;
        public string SelectedPath
        {
            get { return _selectedPath; }

            set
            {
                if (_selectedPath == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedPathPropertyName);
                _selectedPath = value;
                RaisePropertyChanged(SelectedPathPropertyName);
            }
        }



        public const string UploadStatusMessagePropertyName = "UploadStatusMessage";
        private string _uploadStatusMessage = null;
        public string UploadStatusMessage
        {
            get { return _uploadStatusMessage; }

            set
            {
                if (_uploadStatusMessage == value)
                {
                    return;
                }

                RaisePropertyChanging(UploadStatusMessagePropertyName);
                _uploadStatusMessage = value;
                RaisePropertyChanged(UploadStatusMessagePropertyName);
            }
        }
    }
}
