using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Distributr_Middleware.WPF.Lib.ViewModels;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using Integration.QuickBooks.Lib.QBIntegrationCore;

namespace Integration.QuickBooks.Lib.QBIntegrationViewModels
{
    public class QBEditAccountViewModel : MiddleWareViewModelBase
    {
        public QBEditAccountViewModel()
        {
            SaveCommand = new RelayCommand(SaveAccount);
            CancelCommand = new RelayCommand(Cancel);
            QBAccountTypeList = new ObservableCollection<QBAccountType>();
        }

        #region properties

        public RelayCommand SaveCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }

        public ObservableCollection<QBAccountType> QBAccountTypeList { get; set; }

        public const string SelectedAccountTypePropertyName = "SelectedAccountType";
        private QBAccountType _selectedAccountType = 0;
        public QBAccountType SelectedAccountType
        {
            get { return _selectedAccountType; }

            set
            {
                if (_selectedAccountType == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedAccountTypePropertyName);
                _selectedAccountType = value;
                RaisePropertyChanged(SelectedAccountTypePropertyName);
            }
        }

        private QBAccount _defaultAccount;

        public QBAccount DefaultAccount
        {
            get
            {
                return _defaultAccount ??
                       (_defaultAccount = new QBAccount {QBAccountId = "", AccountName = "--Select Account--"});
            }
        }

        public const string AccountPropertyName = "Account";
        private QBAccount _account = null;

        public QBAccount Account
        {
            get { return _account; }

            set
            {
                if (_account == value)
                {
                    return;
                }

                RaisePropertyChanging(AccountPropertyName);
                _account = value;
                RaisePropertyChanged(AccountPropertyName);
            }
        }

        public const string PageTitlePropertyName = "PageTitle";
        private string _pageTitle = "Create Quick Books Account";

        public string PageTitle
        {
            get { return _pageTitle; }

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

        public const string CanEditPropertyName = "CanEdit";
        private bool _canEdit = true;

        public bool CanEdit
        {
            get { return _canEdit; }

            set
            {
                if (_canEdit == value)
                {
                    return;
                }

                RaisePropertyChanging(CanEditPropertyName);
                _canEdit = value;
                RaisePropertyChanged(CanEditPropertyName);
            }
        }
         
        public const string QBAccountIdPropertyName = "QBAccountId";
        private string _QBAccountId = "";
        public string QBAccountId
        {
            get
            {
                return _QBAccountId;
            }

            set
            {
                if (_QBAccountId == value)
                {
                    return;
                }

                RaisePropertyChanging(QBAccountIdPropertyName);
                _QBAccountId = value;
                RaisePropertyChanged(QBAccountIdPropertyName);
            }
        }

        #endregion


        #region methods

        protected override void LoadPage()
        {
            CanEdit = true;
            Setup();
            Account = new QBAccount();
            PageTitle = "Create Quick Books Account";
        }

        private void SaveAccount()
        {
            SimpleIoc.Default.GetInstance<QBMainWindowViewModel>().GlobalStatus = "Exporting account to Quick Books ...";
            //if (!IsValid(Account))
                //return;

            if (SelectedAccountType == QBAccountType.None)
            {
                MessageBox.Show("Select a valid account type.");
            }

            Account.AccountType = SelectedAccountType;
            var accRet = QBIntegrationMethods.AddAccount(Account);
            if (accRet != null)
            {
                Account.QBAccountId = accRet.ListID.GetValue();
                QBAccountId = accRet.ListID.GetValue();
                CanEdit = false;
                PageTitle = "Quick Books Account Name " + accRet.FullName.GetValue();
            }
            SimpleIoc.Default.GetInstance<QBMainWindowViewModel>().GlobalStatus = "Exported account " + Account.FullName + " to Quick Books ...";
            Navigate(@"/views/ListSalesPage.xaml");
        }

        private void Setup()
        {
            CanEdit = true;
            QBAccountId = "";
            LoadAccountTypeList();
        }

        private void Cancel()
        {
            SelectedAccountType = QBAccountType.None;
            Account = null;
            Navigate(@"/views/ListSalesPage.xaml");
        }

        private void LoadAccountTypeList()
        {
            Type _enumType = typeof (QBAccountType);
            QBAccountTypeList.Clear();
            FieldInfo[] infos = _enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (FieldInfo fi in infos)
                QBAccountTypeList.Add((QBAccountType) Enum.Parse(_enumType, fi.Name, true));
            SelectedAccountType = QBAccountType.None;
        }

        #endregion

    }

    #region helpers

    public enum QBAccountType
    {
        None = 0,
        COGSAccount = 1,
        CurrentAssetsAccount = 2,
        IncomeAccount = 3,
        ReceivableAccount=4
    }

    public class QBAccount
    {
        public string QBAccountId { get; set; }
        [Required(ErrorMessage="Account Name is required.")]
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public QBAccountType AccountType { get; set; }

        public string FullName
        {
            get { return AccountNumber + " - " + AccountName; }
        }
    }

    #endregion

}
