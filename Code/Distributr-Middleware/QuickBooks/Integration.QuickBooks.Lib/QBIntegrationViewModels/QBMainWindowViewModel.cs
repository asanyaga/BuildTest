using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IIntegrationDocumentRepository;
using Distributr_Middleware.WPF.Lib.ViewModels;
using GalaSoft.MvvmLight.Command;
using Integration.QuickBooks.Lib.QBIntegrationCore;
using IAbout = Integration.QuickBooks.Lib.UI.IAbout;

namespace Integration.QuickBooks.Lib.QBIntegrationViewModels
{
    public class QBMainWindowViewModel : MiddleWareViewModelBase
    {
        public List<QuickBooksOrderDocumentDto> PagedList { get; set; }
        public bool CanConnectToQuickBooks = false;
        public QBMainWindowViewModel()
        {
            CanConnectToQuickBooks = false;

        }
        
        private void LoadMasterdataPage()
        {
            Navigate(@"/views/masterdata.xaml");
           

        }


        public event EventHandler ExitApplicationEventHandler = (s, e) => { };
     
        private RelayCommand<TabControl> _syncOrdersCommand;
        public RelayCommand<TabControl> SyncOrdersCommand
        {
            get { return _syncOrdersCommand ?? (_syncOrdersCommand = new RelayCommand<TabControl>(SyncOrders)); }
        }

        private RelayCommand _editQBAccountCommand;
        public RelayCommand EditQBAccountCommand
        {
            get { return _editQBAccountCommand ?? (_editQBAccountCommand = new RelayCommand(EditQBAccount)); }
        }

        private RelayCommand _pullMasterdataCommand;
        public RelayCommand PullMasterCommand
        {
            get { return _pullMasterdataCommand ?? (_pullMasterdataCommand = new RelayCommand(LoadMasterdataPage)); }
        }

        
        private RelayCommand _exitCommand;
        public RelayCommand ExitCommand
        {
            get { return _exitCommand ?? (_exitCommand = new RelayCommand(ExitApplication)); }
        }

        private RelayCommand _aboutCommand;
        public RelayCommand AboutCommand
        {
            get { return _aboutCommand ?? (_aboutCommand = new RelayCommand(ShowAbout)); }
        }

        public const string CanSyncmasterDataPropertyName = "CanSyncmasterData";
        private bool _canSync =false;
        public bool CanSyncmasterData
        {
            get
            {
                return _canSync;
            }

            set
            {
                if (_canSync == value)
                {
                    return;
                }

                RaisePropertyChanging(CanSyncmasterDataPropertyName);
                _canSync = value;
                RaisePropertyChanged(CanSyncmasterDataPropertyName);
            }
        }

        public const string GlobalStatusPropertyName = "GlobalStatus";
        private string _globalStatus = "...";
        public string GlobalStatus
        {
            get
            {
                return _globalStatus;
            }

            set
            {
                if (_globalStatus == value)
                {
                    return;
                }

                RaisePropertyChanging(GlobalStatusPropertyName);
                _globalStatus = value;
                RaisePropertyChanged(GlobalStatusPropertyName);
            }
        }

        private void ExitApplication()
        {
            ExitApplicationEventHandler(this, null);
        }

        private void SyncOrders(TabControl tabControl)
        {
            if (tabControl.SelectedIndex == 0) return;
            tabControl.SelectedItem = tabControl.Items.GetItemAt(0);
        }
        
       


        private void EditQBAccount()
        {
            Navigate(@"/Views/EditAccountsPage.xaml");
        }
        private void ShowAbout()
        {
            using (var c = NestedContainer)
            {
                Using<IAbout>(c).ShowAboutDialog();
            }
        }

        internal void SetGlobalStatus(string obj)
        {
            GlobalStatus = obj;
      
        }
    }
}
