using System;
using System.Windows;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using Distributr.WPF.Lib.UI.Pages;
using Distributr.WPF.Lib.ViewModels.Warehousing.EagcClient;
using GalaSoft.MvvmLight.Command;
using VCVouchers.API.Dtos;
using WarehouseReceipt.Client;

namespace Distributr.WPF.Lib.ViewModels.Warehousing
{
    public class WarehouseDepositorViewModel : DistributrViewModelBase
    {
        public WarehouseDepositorViewModel()
        {
            AccountSelectionChangedCommand = new RelayCommand(SelectAccount);
            SaveCommand = new RelayCommand(AddDepositor);
            new RelayCommand(LogOut);
        }

        private void LogOut()
        {
            EAGCLoginDetails.TokenId = null;
            MessageBox.Show("Logged Out Successfully", "Eagc Portal", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        #region Methods

        private void Resetvm()
        {
            FirstName = "";
            MiddleName = "";
            LastName = "";
            MobileNumber = "";
            AccountName = "--Select Account Name--";
        }


        private async void AddDepositor()
        {
           if (string.IsNullOrEmpty(EAGCLoginDetails.TokenId))
            {

                Login();
            }
            else
            {
                using (var c = NestedContainer)
                {
                    var cd =
                        new ContactCreateDepositorCommand(
                            new ContactPersonalNameDto(FirstName, MiddleName, LastName), MobileNumber,
                            AccountId.ToString());

                    var newDepositor = await Using<IEagcServiceProxy>(c).AddDepositor(cd);
                    if (newDepositor.CommandExecutionStatus == CommandExecutionStatus.RanToCompletion)
                    {
                        MessageBox.Show("Depositor Saved Successfully");
                    }
                    
                    Resetvm();
                }
            }

        }



        private void SelectAccount()
        {
            using (var container = NestedContainer) 
            {
                var selected = Using<IAgriItemsLookUp>(container).SelectCommoditySupplier();

                AccountId = selected.Id;
                AccountName = selected.AccountName;
            }
        }


        private void Login()
        {
            using (var container = NestedContainer)
            {
                var selected = Using<ILoginPopup>(container).ShowLoginPopup();
               
            }
        }





        #endregion
        #region members
        public RelayCommand AccountSelectionChangedCommand { get; set; }
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand LogCommand { get; set; }
        #endregion

       

        #region Properties



        public const string AccountIdPropertyName = "AccountId";
        private Guid _accountIdGuid = Guid.Empty;
        public Guid AccountId
        {
            get
            {
                return _accountIdGuid;
            }

            set
            {
                if (_accountIdGuid == value)
                {
                    return;
                }

                RaisePropertyChanging(AccountIdPropertyName);
                _accountIdGuid = value;
                RaisePropertyChanged(AccountIdPropertyName);
            }
        }



        public const string AccountNamePropertyName = "AccountName";
        private string _accountName = "";
        public string AccountName
        {
            get
            {
                return _accountName;
            }

            set
            {
                if (_accountName == value)
                {
                    return;
                }

                RaisePropertyChanging(AccountNamePropertyName);
                _accountName = value;
                RaisePropertyChanged(AccountNamePropertyName);
            }
        }


        public const string FirstNamePropertyName = "FirstName";
        private string _firstName = null;
        public string FirstName
        {
            get
            {
                return _firstName;
            }

            set
            {
                if (_firstName == value)
                {
                    return;
                }

                RaisePropertyChanging(FirstNamePropertyName);
                _firstName = value;
                RaisePropertyChanged(FirstNamePropertyName);
            }
        }


        public const string MiddleNamePropertyName = "MiddleName";
        private string _middleName = null;
        public string MiddleName
        {
            get
            {
                return _middleName;
            }

            set
            {
                if (_middleName == value)
                {
                    return;
                }

                RaisePropertyChanging(MiddleNamePropertyName);
                _middleName = value;
                RaisePropertyChanged(MiddleNamePropertyName);
            }
        }


        public const string LastNamePropertyName = "LastName";
        private string _lastName = null;
        public string LastName
        {
            get
            {
                return _lastName;
            }

            set
            {
                if (_lastName == value)
                {
                    return;
                }

                RaisePropertyChanging(LastNamePropertyName);
                _lastName = value;
                RaisePropertyChanged(LastNamePropertyName);
            }
        }

        public const string MobileNumberPropertyName = "MobileNumber";

        private string _mobileNumber = null;
        public string MobileNumber
        {
            get
            {
                return _mobileNumber;
            }

            set
            {
                if (_mobileNumber == value)
                {
                    return;
                }

                RaisePropertyChanging(MobileNumberPropertyName);
                _mobileNumber = value;
                RaisePropertyChanged(MobileNumberPropertyName);
            }
        }



        public const string SelectedAccountPropertyName = "SelectedAccount";
        private CommoditySupplier _selectedAccount = new CommoditySupplier(Guid.Empty) { AccountName = "---Select Account---" };
        public CommoditySupplier SelectedAccount
        {
            get
            {
                return _selectedAccount;
            }

            set
            {
                if (_selectedAccount == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedAccountPropertyName);
                _selectedAccount = value;
                RaisePropertyChanged(SelectedAccountPropertyName);
            }
        }
        #endregion



    }
}
