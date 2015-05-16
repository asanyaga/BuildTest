using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Agrimanagr.DataImporter.Lib.ImportEntities;
using Agrimanagr.DataImporter.Lib.Utils;
using Distributr.Core.Domain.Master.Util;
using Distributr.WPF.Lib.ViewModels.Admin;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;

namespace Agrimanagr.DataImporter.Lib.ViewModels
{
    public class ImporterListingsViewModelBase: ListingsViewModelBase
    {
        protected Dictionary<int, string> Positions { get; set; }
        internal IPagenatedList<ImportItemVM> PagedList;

        public ImporterListingsViewModelBase()
        {
            Positions = new Dictionary<int, string>();
        }

        protected int GetIndex(string propertyName)
        {
            if (!Positions.Any() || string.IsNullOrEmpty(propertyName)) return -1;
            propertyName = propertyName.ToLower();
            var pos=(Positions.FirstOrDefault(p => p.Value.ToLower() == propertyName).Key - 1);
            return pos;

        }
        protected string GetColumn(MasterImportEntity row, int index,bool handleDateTime=false,bool handleEnum=false)
        {
            var element = row.ElementAtOrDefault(index);
            string value = string.Empty;
            if(element !=null)
            {
                if(handleDateTime)
                {
                    DateTime date;
                    if (!DateTime.TryParse(element.Value, out date))
                        date = DateTime.Now;
                   value= date.ToString();
                }else if(handleEnum)
                {
                    int enumVal;
                    if (!Int32.TryParse(element.Value, out enumVal))
                        enumVal = 0;
                    value = enumVal.ToString();
                }
                else
                {
                   value= element.Value;
                }
            }
            return value;

        }
        protected  void GotoHomePage()
        {
            Navigate("/views/homepage.xaml");
                MainViewModel.ProsessMessage = "Import terminated by user";
               
        }

        protected virtual async void UploadAll()
        {
           
        }

        protected virtual async void UploadSelected()
        {
            
        }

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
            UpdatePagenationControlBase(PagedList.PageNumber, PagedList.PageCount, PagedList.TotalItemCount,
                                        PagedList.IsFirstPage, PagedList.IsLastPage);
        }

      #endregion

        #region Base Properties
       
      
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
            NavigateCommand.Execute(@"/views/HomePage.xaml");
        }
        

        protected MainViewModel MainViewModel
        {
            get { return SimpleIoc.Default.GetInstance<MainViewModel>(); }
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
        #endregion
    }
}
