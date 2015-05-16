using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Transactional.Recollections;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.UI.Pages;
using Distributr.WPF.Lib.ViewModels.Admin;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Distributr.WPF.Lib.ViewModels.Transactional.RN
{
    public class UnderBankingSetup
    {
        public Guid SalesmanId { get; set; }
        public decimal Amount { get; set; }
    }

    public class UnderBankingViewModel : DistributrViewModelBase
    {
        public event EventHandler RequestClose = (s, e) => { };
        public bool Status { set; get; }
        public UnderBankingViewModel()
        {
            LineItems = new ObservableCollection<UnderBankingItemViewModel>();
            SaveCommand = new RelayCommand(Save);
            AddCommand= new RelayCommand(AddItem);
            RemoveItemCommand = new RelayCommand<UnderBankingItemViewModel>(RemoveItem);
            SelectCostCentreCommand = new RelayCommand(SelectCostCentre);
            CancelCommand= new RelayCommand(Cancel);
        }

        private void Cancel()
        {
            Status = false;
            RequestClose(this, EventArgs.Empty);

        }

        private void RemoveItem(UnderBankingItemViewModel obj)
        {
           
            var item = LineItems.FirstOrDefault(s => s.CostCentre.Id ==obj.CostCentre.Id);
            if (item != null)
            {
                
                LineItems.Remove(item);
            }
            CalculateSummary();
        }

        private void CalculateSummary()
        {
            TotalAllocatedAmount = LineItems.Sum(s => s.Amount);
            TotaBalanceAmount = TotalUnderbankingAmout - TotalAllocatedAmount;
        }

        public void SetSalesman(UnderBankingSetup setup)
        {
            using (var container = NestedContainer)
            {
                Salesman = Using<ICostCentreRepository>(container).GetById(setup.SalesmanId) as DistributorSalesman;
                TotalUnderbankingAmout = setup.Amount;
            }
        }

        private void SelectCostCentre()
        {
            CostCentre = null;
             using (var container = NestedContainer)
             {
                 var selected = Using<IItemsLookUp>(container).SelectUnderbankingCostCentre(Salesman.Id);

                 CostCentre = selected;
                 if(selected==null)
                 {
                     CostCentre = new Outlet(Guid.Empty){Name = "--Select CostCentre---"};
                 }
             }

            
        }

        public ObservableCollection<UnderBankingItemViewModel> LineItems { get; set; }
        public RelayCommand AddCommand { get; set; }
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        public RelayCommand SelectCostCentreCommand { get; set; }
        public RelayCommand<UnderBankingItemViewModel> RemoveItemCommand { get; set; }
        private void AddItem()
        {
            if(CostCentre==null || CostCentre.Id==Guid.Empty)
            {
                MessageBox.Show("Please Select salesman or outlet to underbank");
                return;
            }
            if(UnderBankAmount<=0)
            {
                MessageBox.Show("Please Enter Amount");
                return;
            }
            decimal allocatedamount = LineItems.Where(s=>s.CostCentre.Id!=CostCentre.Id).Sum(s => s.Amount);
            if ((allocatedamount+UnderBankAmount) >TotalUnderbankingAmout)
            {
                MessageBox.Show("Amount cant be more that Total Underbanking Amount less Amount already allocated");
                return;
            }
            var item = LineItems.FirstOrDefault(s => s.CostCentre.Id == CostCentre.Id);
            if(item==null)
            {
                item= new UnderBankingItemViewModel();
                LineItems.Add(item);
            }

            item.Amount = UnderBankAmount;
            item.CostCentre = CostCentre;
            item.Description = Description;
            ClearViewModel();
            CalculateSummary();

        }

        private void ClearViewModel()
        {
            UnderBankAmount = 0;
            CostCentre = new Outlet(Guid.Empty) { Name = "--Select CostCentre---" };
            Description = "";
        }

        private void Save()
        {
            try
            {


                decimal allocatedamount = LineItems.Sum(s => s.Amount);
                if (allocatedamount != TotalUnderbankingAmout)
                {
                    MessageBox.Show("Please make sure the whole amount is allocated");
                    return;
                }
                using (var container = NestedContainer)
                {
                    var wf = Using<IReCollectionWFManager>(container);
                    var config = Using<IConfigService>(container).Load();
                    ReCollection doc = new ReCollection(Guid.NewGuid());
                    doc.CostCentreId = Salesman.Id;
                    doc.CostCentreApplicationId = config.CostCentreApplicationId;
                    doc.RecepientCostCentreId = config.CostCentreId;
                    foreach (var i in LineItems)
                    {
                        UnderBankingItem item = new UnderBankingItem(Guid.NewGuid());
                        item.Amount = i.Amount;
                        item.Description = i.Description;
                        item.FromCostCentreId = i.CostCentre.Id;
                        doc.AddLineItem(item);
                    }
                    wf.SubmitChanges(doc);
                    MessageBox.Show("Under banking Saved successfully");
                }
            }catch(Exception ex)
            {
                MessageBox.Show("ERROR "+ex.Message);
                return;
            }

            Status = true;
            RequestClose(this, EventArgs.Empty);
        }

        public const string UnderBankAmountPropertyName = "UnderBankAmount";
        private decimal _underBankAmount = 0;
        public decimal UnderBankAmount
        {
            get
            {
                return _underBankAmount;
            }

            set
            {
                if (_underBankAmount == value)
                {
                    return;
                }

                RaisePropertyChanging(UnderBankAmountPropertyName);
                _underBankAmount = value;
                RaisePropertyChanged(UnderBankAmountPropertyName);
            }
        }

     
        public const string SalesmanPropertyName = "Salesman";
        private DistributorSalesman _salesman = null;
        public DistributorSalesman Salesman
        {
            get
            {
                return _salesman;
            }

            set
            {
                if (_salesman == value)
                {
                    return;
                }

                RaisePropertyChanging(SalesmanPropertyName);
                _salesman = value;
                RaisePropertyChanged(SalesmanPropertyName);
            }
        }

        
        public const string CostCentrePropertyName = "CostCentre";
        private CostCentre _costCentre = null;
        public CostCentre CostCentre
        {
            get
            {
                return _costCentre;
            }

            set
            {
                if (_costCentre == value)
                {
                    return;
                }

                RaisePropertyChanging(CostCentrePropertyName);
                _costCentre = value;
                RaisePropertyChanged(CostCentrePropertyName);
            }
        }



        public const string TotalAllocatedAmountPropertyName = "TotalAllocatedAmount";
        private decimal _totalAllocatedAmount = 0;
        public decimal TotalAllocatedAmount
        {
            get
            {
                return _totalAllocatedAmount;
            }

            set
            {
                if (_totalAllocatedAmount == value)
                {
                    return;
                }

                RaisePropertyChanging(TotalAllocatedAmountPropertyName);
                _totalAllocatedAmount = value;
                RaisePropertyChanged(TotalAllocatedAmountPropertyName);
            }
        }



        public const string TotaBalanceAmountPropertyName = "TotaBalanceAmount";
        private decimal _totaBalanceAmount = 0;
        public decimal TotaBalanceAmount
        {
            get
            {
                return _totaBalanceAmount;
            }

            set
            {
                if (_totaBalanceAmount == value)
                {
                    return;
                }

                RaisePropertyChanging(TotaBalanceAmountPropertyName);
                _totaBalanceAmount = value;
                RaisePropertyChanged(TotaBalanceAmountPropertyName);
            }
        }

        public const string TotalUnderbankingAmoutPropertyName = "TotalUnderbankingAmout";
        private decimal _totalUnderBankingAmount = 0;
        public decimal TotalUnderbankingAmout
        {
            get
            {
                return _totalUnderBankingAmount;
            }

            set
            {
                if (_totalUnderBankingAmount == value)
                {
                    return;
                }

                RaisePropertyChanging(TotalUnderbankingAmoutPropertyName);
                _totalUnderBankingAmount = value;
                RaisePropertyChanged(TotalUnderbankingAmoutPropertyName);
            }
        }

       
       
        public const string DescriptionPropertyName = "Description";
        private string _description = "";
        public string Description
        {
            get
            {
                return _description;
            }

            set
            {
                if (_description == value)
                {
                    return;
                }

                RaisePropertyChanging(DescriptionPropertyName);
                _description = value;
                RaisePropertyChanged(DescriptionPropertyName);
            }
        }

        
    }

    public class UnderBankingItemViewModel : ViewModelBase
    {
        
        public const string CostCentrePropertyName = "CostCentre";
        private CostCentre _costCentre = null;
        public CostCentre CostCentre
        {
            get
            {
                return _costCentre;
            }

            set
            {
                if (_costCentre == value)
                {
                    return;
                }

                RaisePropertyChanging(CostCentrePropertyName);
                _costCentre = value;
                RaisePropertyChanged(CostCentrePropertyName);
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

        
        public const string DescriptionPropertyName = "Description";
        private string _description = "";
        public string Description
        {
            get
            {
                return _description;
            }

            set
            {
                if (_description == value)
                {
                    return;
                }

                RaisePropertyChanging(DescriptionPropertyName);
                _description = value;
                RaisePropertyChanged(DescriptionPropertyName);
            }
        }
    }
}
