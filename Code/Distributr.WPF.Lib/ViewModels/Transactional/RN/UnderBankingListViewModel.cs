using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Transactional.Recollections;
using Distributr.Core.Repository.Transactional.RecollectionRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.UI.Pages;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace Distributr.WPF.Lib.ViewModels.Transactional.RN
{
  public  class UnderBankingListViewModel:DistributrViewModelBase
    {
      public ObservableCollection<UnderBankingListSummaryViewModel> LineItems { get; set; }
        public RelayCommand LoadCommand { get; set; }
        public RelayCommand SetUpCommand { get; set; }
        public RelayCommand SelectSalesmanCommand { get; set; }
        public RelayCommand SelectCostCentreCommand { get; set; }
        public RelayCommand<UnderBankingListSummaryViewModel> ShowPaymentCommand { get; set; }
       
        public RelayCommand ClearCommand { get; set; }

      public UnderBankingListViewModel()
      {
          LineItems = new ObservableCollection<UnderBankingListSummaryViewModel>();
          LoadCommand= new RelayCommand(Load);
          SelectSalesmanCommand = new RelayCommand(SelectSalesman);
          SelectCostCentreCommand = new RelayCommand(SelectCostCentre);
          SetUpCommand = new RelayCommand(SetUp);
          ClearCommand = new RelayCommand(ClearFilter);
          ShowPaymentCommand = new RelayCommand<UnderBankingListSummaryViewModel>(ShowPayment);

      }

      private void ShowPayment(UnderBankingListSummaryViewModel item)
      {
          using (var container = NestedContainer)
          {
              Messenger.Default.Send(item.Id);
             var status = Using<IUnderBankingConfirmationPopUp>(container).ShowPaymentInfor();

             Load();
          }

      }

      private void SetUp()
      {
          ClearFilter();
          Load();
      }

      private void ClearFilter()
      {
          Salesman=new DistributorSalesman(Guid.Empty){Name = "----Select Salesman ----"};
          CostCentre = new Outlet(Guid.Empty) { Name = "----Select Salesman/Outlet ----" };
      }

      

      private void SelectCostCentre()
      {
          if(Salesman==null || Salesman.Id ==Guid.Empty)
          {
              MessageBox.Show("Select Salesman to select Costcentre");
              return; 
          }
          CostCentre = null;
          using (var container = NestedContainer)
          {
              var selected = Using<IItemsLookUp>(container).SelectUnderbankingCostCentre(Salesman.Id);

              CostCentre = selected;
              if (selected == null)
              {
                  CostCentre = new Outlet(Guid.Empty) { Name = "--Select CostCentre---" };
              }
          }
      }

      private void SelectSalesman()
      {
          Salesman = null;
          using (var container = NestedContainer)
          {
              var selected = Using<IItemsLookUp>(container).SelectDistributrSalesman();

              Salesman = selected as DistributorSalesman;
              if (selected == null)
              {
                  Salesman = new DistributorSalesman(Guid.Empty) { Name = "--Select Salesman---" };
              }
          }
      }

      private void Load()
      {
          using (var container = NestedContainer)
          {
              var query = new QueryUnderBanking{Skip = 0,Take = 20};
              if (CostCentre != null && CostCentre.Id != Guid.Empty)
                  query.CostCentreId = CostCentre.Id;
              if (Salesman != null && Salesman.Id != Guid.Empty)
                  query.SalesmanId = Salesman.Id;
            var data  = Using<IReCollectionRepository>(container).QuerySummary(query);
             LineItems.Clear();
              foreach (var u in data)
              {
                  var item = new UnderBankingListSummaryViewModel();
                  item.Id = u.Id;
                  item.Amount = u.Amount;
                  item.CanConfirm =  u.ConfirmedAmount != u.ReceivedAmount && u.ReceivedAmount>0 ;
                  item.ConfirmedAmount = u.ConfirmedAmount;
                  item.CostCentreId = u.CostCentreId;
                  item.CostCentreName = u.CostCentreName;
                  item.ReceivedAmount = u.ReceivedAmount;
                  item.SalesmanId = u.SalesmanId;
                  item.SalesmanName = u.SalesmanName;
                  item.CostCenterType = u.CostCentreType;
                  item.Description = u.Description;

                  LineItems.Add(item);
              }
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

    }

    public class UnderBankingListSummaryViewModel : ViewModelBase
    {
        //public Guid CostCentreId { get; set; }
        //public Guid SalesmanId { get; set; }
        //public string CostCentreName { get; set; }
        //public string SalesmanName { get; set; }
        //public decimal Amount { get; set; }
        //public decimal ReceivedAmount { get; set; }
        //public decimal ConfirmedAmount { get; set; }
        //public string Description { get; set; }


        public const string CostCenterTypePropertyName = "CostCenterType";
        private string _CostCenterType = "";
        public string CostCenterType
        {
            get
            {
                return _CostCenterType;
            }

            set
            {
                if (_CostCenterType == value)
                {
                    return;
                }

                RaisePropertyChanging(CostCenterTypePropertyName);
                _CostCenterType = value;
                RaisePropertyChanged(CostCenterTypePropertyName);
            }
        }
        
        public const string CanConfirmPropertyName = "CanConfirm";
        private bool _canconfirm = false;
        public bool CanConfirm
        {
            get
            {
                return _canconfirm;
            }

            set
            {
                if (_canconfirm == value)
                {
                    return;
                }

                RaisePropertyChanging(CanConfirmPropertyName);
                _canconfirm = value;
                RaisePropertyChanged(CanConfirmPropertyName);
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

        public const string ConfirmedAmountPropertyName = "ConfirmedAmount";
        private decimal _confirmedamount = 0;
        public decimal ConfirmedAmount
        {
            get
            {
                return _confirmedamount;
            }

            set
            {
                if (_confirmedamount == value)
                {
                    return;
                }

                RaisePropertyChanging(ConfirmedAmountPropertyName);
                _confirmedamount = value;
                RaisePropertyChanged(ConfirmedAmountPropertyName);
            }
        }
       
        public const string ReceivedAmountPropertyName = "ReceivedAmount";
        private decimal _receivedAmount = 0;
        public decimal ReceivedAmount
        {
            get
            {
                return _receivedAmount;
            }

            set
            {
                if (_receivedAmount == value)
                {
                    return;
                }

                RaisePropertyChanging(ReceivedAmountPropertyName);
                _receivedAmount = value;
                RaisePropertyChanged(ReceivedAmountPropertyName);
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

       
        public const string SalesmanNamePropertyName = "SalesmanName";
        private string _salesmanname = "";
        public string SalesmanName
        {
            get
            {
                return _salesmanname;
            }

            set
            {
                if (_salesmanname == value)
                {
                    return;
                }

                RaisePropertyChanging(SalesmanNamePropertyName);
                _salesmanname = value;
                RaisePropertyChanged(SalesmanNamePropertyName);
            }
        }
        
        public const string CostCentreNamePropertyName = "CostCentreName";
        private string _costcentrename = "";
        public string CostCentreName
        {
            get
            {
                return _costcentrename;
            }

            set
            {
                if (_costcentrename == value)
                {
                    return;
                }

                RaisePropertyChanging(CostCentreNamePropertyName);
                _costcentrename = value;
                RaisePropertyChanged(CostCentreNamePropertyName);
            }
        }
       
        public const string SalesmanIdPropertyName = "SalesmanId";
        private Guid _salesmanid = Guid.Empty;
        public Guid SalesmanId
        {
            get
            {
                return _salesmanid;
            }

            set
            {
                if (_salesmanid == value)
                {
                    return;
                }

                RaisePropertyChanging(SalesmanIdPropertyName);
                _salesmanid = value;
                RaisePropertyChanged(SalesmanIdPropertyName);
            }
        }
       
        public const string CostCentreIdPropertyName = "CostCentreId";
        private Guid _costcentreId = Guid.Empty;
        public Guid CostCentreId
        {
            get
            {
                return _costcentreId;
            }

            set
            {
                if (_costcentreId == value)
                {
                    return;
                }

                RaisePropertyChanging(CostCentreIdPropertyName);
                _costcentreId = value;
                RaisePropertyChanged(CostCentreIdPropertyName);
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
}
