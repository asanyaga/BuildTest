using System;
using System.Collections.ObjectModel;
using System.Windows;
using Distributr.Core.Domain.Transactional.Recollections;
using Distributr.Core.Repository.Transactional.RecollectionRepositories;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.Services.Service.Utility;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Distributr.WPF.Lib.ViewModels.Transactional.RN
{
    public class UnderBankingCollectionViewModel : DistributrViewModelBase
    {
        public RelayCommand LoadCommand { get; set; }
        public ObservableCollection<UnderBankingCollectionItemViewModel> LineItems { get; set; }
        public RelayCommand<UnderBankingCollectionItemViewModel> ConfirmCommand { get; set; }

        public UnderBankingCollectionViewModel()
        {
            LoadCommand = new RelayCommand(Load);
            LineItems = new ObservableCollection<UnderBankingCollectionItemViewModel>();
            ConfirmCommand = new RelayCommand<UnderBankingCollectionItemViewModel>(ConfirmCollection);
        }

        private void ConfirmCollection(UnderBankingCollectionItemViewModel item)
        {
            using (var container = NestedContainer)
            {
                if (item != null)
                {
                    var wf = Using<IReCollectionWFManager>(container);
                    var config = Using<IConfigService>(container).Load();
                    ReCollection doc = new ReCollection(item.Id);
                    doc.CostCentreId = config.CostCentreId;
                    doc.CostCentreApplicationId = config.CostCentreApplicationId;
                    doc.RecepientCostCentreId = config.CostCentreId;
                    doc.Id = Id;
                    UnderBankingItem sItem = new UnderBankingItem(item.Id);
                    sItem.FromCostCentreId = config.CostCentreId;
                    doc.ConfirmLineItem(sItem);
                    wf.SubmitChanges(doc);
                    MessageBox.Show("Received Underbanking Saved successfully");
                    Load();

                }
            }
        }

        public void SetUnderBankingId(Guid id)
        {
            Id = id;
        }
        private void Load()
        {
            using (var container = NestedContainer)
            {
                var data = Using<IReCollectionRepository>(container).UnderBankingItemReceived(Id);
                LineItems.Clear();
                foreach(var item in data)
                {
                    var lineitem = new UnderBankingCollectionItemViewModel
                                       {
                                           Amount=item.Amount,
                                           IsConfirmed = !item.IsConfirmed,
                                           ReCollectionType = item.Type,
                                           Id = item.Id,
                                       };
                    LineItems.Add(lineitem);
                }
            }
        }

       
        public const string IdPropertyName = "Id";
        private Guid _id = Guid.Empty;
        public Guid Id
        {
            get
            {
                return _id;
            }

            set
            {
                if (_id == value)
                {
                    return;
                }

                RaisePropertyChanging(IdPropertyName);
                _id = value;
                RaisePropertyChanged(IdPropertyName);
            }
        }
       


    }

    public class UnderBankingCollectionItemViewModel : ViewModelBase
    {
        public RelayCommand LoadCommand { get; set; }


        public const string IdPropertyName = "Id";
        private Guid _id = Guid.Empty;
        public Guid Id
        {
            get
            {
                return _id;
            }

            set
            {
                if (_id == value)
                {
                    return;
                }

                RaisePropertyChanging(IdPropertyName);
                _id = value;
                RaisePropertyChanged(IdPropertyName);
            }
        }

        public const string AmountPropertyName = "Amount";
        private decimal _amount = 0;
        public decimal Amount
        {
            get
            {
                return _amount;
            }

            set
            {
                if (_amount == value)
                {
                    return;
                }

                RaisePropertyChanging(AmountPropertyName);
                _amount = value;
                RaisePropertyChanged(AmountPropertyName);
            }
        }


        public const string IsConfirmedPropertyName = "IsConfirmed";
        private bool _isconfirmed = false;
        public bool IsConfirmed
        {
            get
            {
                return _isconfirmed;
            }

            set
            {
                if (_isconfirmed == value)
                {
                    return;
                }

                RaisePropertyChanging(IsConfirmedPropertyName);
                _isconfirmed = value;
                RaisePropertyChanged(IsConfirmedPropertyName);
            }
        }


        public const string ReCollectionTypePropertyName = "ReCollectionType";
        private ReCollectionType _type = 0;
        public ReCollectionType ReCollectionType
        {
            get
            {
                return _type;
            }

            set
            {
                if (_type == value)
                {
                    return;
                }

                RaisePropertyChanging(ReCollectionTypePropertyName);
                _type = value;
                RaisePropertyChanged(ReCollectionTypePropertyName);
            }
        }



    }

}