using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.DataImporter.Lib.ImportService;
using Distributr.WPF.Lib.ViewModels.Admin;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;

namespace Distributr.DataImporter.Lib.ViewModel.FCL
{
    public class FCLListingBaseViewModel : ListingsViewModelBase
    {
        public RelayCommand<string> GetMasterdataImportCommand { get; set; }
        public RelayCommand UploadSelectedCommand { get; set; }
        public RelayCommand UploadAllCommand { get; set; }
        public RelayCommand UploadCurrentPageCommand { get; set; }
        public RelayCommand<Page> ReloadPageCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        protected FclMainWindowViewModel MainWindowViewModel
        {
            get { return SimpleIoc.Default.GetInstance<FclMainWindowViewModel>(); }
        }

        public FCLListingBaseViewModel()
        {
            GetMasterdataImportCommand = new RelayCommand<string>(LoadMasterdataFromFile);
            UploadAllCommand=new RelayCommand(UploadAll);
            UploadSelectedCommand = new RelayCommand(UploadSelected);
            ReloadPageCommand=new RelayCommand<Page>(Reload);
            CancelCommand=new RelayCommand(ClosePage);
            UploadCurrentPageCommand = new RelayCommand(UploadCurrentPage);
           
        }

        private void ClosePage()
        {
            NavigateCommand.Execute( @"/views/HomePage.xaml");
        }

        private void Reload(Page page)
        {
            if(page !=null)
                page.NavigationService.Refresh();
        }

        
       protected virtual void ReportProgress(int value)
       {

       }
       protected virtual async void UploadSelectedAsync()
        {

        }

        protected virtual void UploadSelected()
        {
           
        }

        protected virtual void UploadAll()
        {
            
        }
        protected virtual void UploadCurrentPage(){}

       
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
            throw new NotImplementedException();
        }

        protected override void ComboPageSizesSelectionChanged(int take)
        {
            throw new NotImplementedException();
        }

        protected override void UpdatePagenationControl()
        {
            throw new NotImplementedException();
        }

        private void LoadMasterdataFromFile(string masterdata)
        {
            LoadedImportItem = masterdata;
            switch (masterdata)
            {
                case "products":

                    break;

            }
        }

        protected virtual void Reload()
        {
        }

        protected virtual async Task<int> ReloadAsync(/*CancellationToken ct,*/ IProgress<double> progress)
        {
            return 0;
        }
        protected void ShowValidationErrors(IEnumerable<ImportValidationResultInfo> result)
        {
            Application.Current.Dispatcher.BeginInvoke(
                new Action(
                    delegate
                    {
                        using (var cont = NestedContainer)
                        {
                            Using<IImportValidationPopUp>(cont).ShowPopUp(
                                result.Where(o => !o.IsValid).ToList());
                        }
                    }));
        }

        public const string DeefaultDistributrPropertyName = "DefaultDistributr";
        private CostCentre _defaultDistributr ;
        public CostCentre DefaultDistributr
        {
            get { return _defaultDistributr; }

            set
            {
                if (_defaultDistributr == value)
                {
                    return;
                }

                RaisePropertyChanging(DeefaultDistributrPropertyName);
                _defaultDistributr = value;
                RaisePropertyChanged(DeefaultDistributrPropertyName);
            }
        }

        public const string LoadedImportItemPropertyName = "LoadedImportItem";
        private string _loadedImportItem = "";
        public string LoadedImportItem
        {
            get { return _loadedImportItem; }

            set
            {
                if (_loadedImportItem == value)
                {
                    return;
                }

                RaisePropertyChanging(LoadedImportItemPropertyName);
                _loadedImportItem = value;
                RaisePropertyChanged(LoadedImportItemPropertyName);
            }
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

        public const string IsUploadSuccessPropertyName = "IsUploadSuccess";
        private bool _isUploadSuccess = false;
        public bool IsUploadSuccess
        {
            get { return _isUploadSuccess; }

            set
            {
                if (_isUploadSuccess == value)
                {
                    return;
                }

                RaisePropertyChanging(IsUploadSuccessPropertyName);
                _isUploadSuccess = value;
                RaisePropertyChanged(IsUploadSuccessPropertyName);
            }
        }
    }


}
