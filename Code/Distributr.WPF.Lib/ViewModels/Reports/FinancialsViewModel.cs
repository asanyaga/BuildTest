using System.Collections.ObjectModel;
using Distributr.Core.Repository.Financials;
using Distributr.WPF.Lib.Service.Utility;
using Distributr.WPF.Lib.Services.Service.Utility;
using GalaSoft.MvvmLight;
using Distributr.Core.Domain.FinancialEntities;
using System.Collections.Generic;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using System.Linq;
using System.Windows.Data;
using GalaSoft.MvvmLight.Command;

namespace Distributr.WPF.Lib.ViewModels.Reports
{
    public class FinancialsViewModel : DistributrViewModelBase
    {
       
       public RelayCommand LoadFinancialsCommand { get; set; }
       

        public FinancialsViewModel()
        {
            LoadFinancialsCommand = new RelayCommand(LoadFinancials);
            Financials= new ObservableCollection<FinancialsItemsViewModel>();
           
        }
        public ObservableCollection<FinancialsItemsViewModel> Financials { get; set; }
        void LoadFinancials()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                var listFinancials =
                    Using<IAccountTransactionRepository>(c)
                        .GetByCostCentre(Using<IConfigService>(c).Load().CostCentreId);
                var financialitems =
                    listFinancials.OrderByDescending(n => n.DateInserted).Select(n => new FinancialsItemsViewModel
                        {
                            Id = n.DocumentId.ToString(),
                            Account = n.Account.AccountType,
                            Amount = n.Amount,
                            DateIserted = n.DateInserted.ToString("dd-MMM-yyyy"),
                            Documenttype = n.DocumentType,
                            Balance = n.Account.Balance
                        });
                foreach (var model in financialitems)
                {
                    Financials.Add(model);
                }
               
               
            }
        }

        public class FinancialsItemsViewModel : ViewModelBase
        {
            /// <summary>
            /// The <see cref="DateIserted" /> property's name.
            /// </summary>
            public const string DateIsertedPropertyName = "DateIserted";
            private string _DateIserted = null;
            /// <summary>
            /// Gets the DateIserted property.
            /// Changes to that property's value raise the PropertyChanged event. 
            /// This property's value is broadcasted by the Messenger's default instance when it changes.
            /// </summary>
            public string DateIserted
            {
                get
                {
                    return _DateIserted;
                }

                set
                {
                    if (_DateIserted == value)
                    {
                        return;
                    }

                    var oldValue = _DateIserted;
                    _DateIserted = value;

                    // Update bindings, no broadcast
                    RaisePropertyChanged(DateIsertedPropertyName);
                }
            }

            /// <summary>
            /// The <see cref="Account" /> property's name.
            /// </summary>
            public const string AccountPropertyName = "Account";
            private AccountType _Account = new AccountType();
            /// <summary>
            /// Gets the Account property.
            
            /// Changes to that property's value raise the PropertyChanged event. 
            /// This property's value is broadcasted by the Messenger's default instance when it changes.
            /// </summary>
            public AccountType Account
            {
                get
                {
                    return _Account;
                }

                set
                {
                    if (_Account == value)
                    {
                        return;
                    }

                    var oldValue = _Account;
                    _Account = value;

                    // Update bindings, no broadcast
                    RaisePropertyChanged(AccountPropertyName);
                }
            }

            /// <summary>
            /// The <see cref="Documenttype" /> property's name.
            /// </summary>
            public const string DocumenttypePropertyName = "Documenttype";
            private DocumentType _DocumentType = new DocumentType();
            /// <summary>
            /// Gets the Documenttype property.
            
            /// Changes to that property's value raise the PropertyChanged event. 
            /// This property's value is broadcasted by the Messenger's default instance when it changes.
            /// </summary>
            public DocumentType Documenttype
            {
                get
                {
                    return _DocumentType;
                }

                set
                {
                    if (_DocumentType == value)
                    {
                        return;
                    }

                    var oldValue = _DocumentType;
                    _DocumentType = value;

                    // Update bindings, no broadcast
                    RaisePropertyChanged(DocumenttypePropertyName);
                }
            }

            /// <summary>
            /// The <see cref="Amount" /> property's name.
            /// </summary>
            public const string AmountPropertyName = "Amount";
            private decimal _Amount = 0;
            /// <summary>
            /// Gets the Amount property.
            
            /// Changes to that property's value raise the PropertyChanged event. 
            /// This property's value is broadcasted by the Messenger's default instance when it changes.
            /// </summary>
            public decimal Amount
            {
                get
                {
                    return _Amount;
                }

                set
                {
                    if (_Amount == value)
                    {
                        return;
                    }

                    var oldValue = _Amount;
                    _Amount = value;

                    // Update bindings, no broadcast
                    RaisePropertyChanged(AmountPropertyName);
                }
            }

            /// <summary>
            /// The <see cref="Id" /> property's name.
            /// </summary>
            public const string IdPropertyName = "Id";
            private string _Id = null;
            /// <summary>
            /// Gets the Id property.
            
            /// Changes to that property's value raise the PropertyChanged event. 
            /// This property's value is broadcasted by the Messenger's default instance when it changes.
            /// </summary>
            public string Id
            {
                get
                {
                    return _Id;
                }

                set
                {
                    if (_Id == value)
                    {
                        return;
                    }

                    var oldValue = _Id;
                    _Id = value;

                    // Update bindings, no broadcast
                    RaisePropertyChanged(IdPropertyName);
                }
            }

            /// <summary>
            /// The <see cref="Balance" /> property's name.
            /// </summary>
            public const string BalancePropertyName = "Balance";
            private decimal _Balance = 0;
            /// <summary>
            /// Gets the Balance property.
            
            /// Changes to that property's value raise the PropertyChanged event. 
            /// This property's value is broadcasted by the Messenger's default instance when it changes.
            /// </summary>
            public decimal Balance
            {
                get
                {
                    return _Balance;
                }

                set
                {
                    if (_Balance == value)
                    {
                        return;
                    }

                    var oldValue = _Balance;
                    _Balance = value;

                    // Update bindings, no broadcast
                    RaisePropertyChanged(BalancePropertyName);
                }
            }
        }
    }
}
