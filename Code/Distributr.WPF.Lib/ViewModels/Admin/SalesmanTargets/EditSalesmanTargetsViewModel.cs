using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Domain.Master.DistributorTargetEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.MasterDataDTO.DataContracts;
using Distributr.Core.Repository.Master.DistributorTargetRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using Distributr.WPF.Lib.UI.Pages;
using GalaSoft.MvvmLight.Command;

namespace Distributr.WPF.Lib.ViewModels.Admin.SalesmanTargets
{
    public class EditSalesmanTargetsViewModel : DistributrViewModelBase
    {
        public EditSalesmanTargetsViewModel()
        {
            LoadCommand = new RelayCommand(Load);
            OnSalesmanChangedCommand = new RelayCommand(OnSalemanChanged);
            OnTargetPeriodChangedCommand = new RelayCommand(OnTargetPeriodChanged);
            AddNewTargetCommand = new RelayCommand(AddNewTarget);
            EditExistingTargetCommand = new RelayCommand(EditExistingTarget);
            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
            OnDeleteTargetClickedCommand = new RelayCommand<SalesmanTarget>(OnDeleteTargetClicked);
            SalemanDropDownOpenedCommand = new RelayCommand(SalemanDropDownOpened);
            PeriodDropDownOpenedCommand = new RelayCommand(PeriodDropDownOpened);

            Salesmen = new ObservableCollection<Salesman>();
            TargetPeriods = new ObservableCollection<TargetPeriod>();
            SalesmanTargets = new ObservableCollection<SalesmanTarget>();
        }

        private void PeriodDropDownOpened()
        {
            using (var container = NestedContainer)
            {
                SelectedTargetPeriod = Using<IItemsLookUp>(container).SelectTargetPeriod(); //??default;
               /* if (SelectedSalesmanName != null)
                    SalesmanNameHolder = SelectedSalesmanName.Username;*/
            }
        }

        private void SalemanDropDownOpened()
        {
            using (var container = NestedContainer)
            {
                SelectedSalesmanName = Using<IItemsLookUp>(container).SelectSelectedSalesman(); //??default;
                if (SelectedSalesmanName != null)
                    SalesmanNameHolder = SelectedSalesmanName.Username;
            }

        }

        #region Methods
        private void Load()
        {
            SalesmanNameHolder = "";
            SelectedSalesmanName = null;
            SelectedSalesman = null;
            SelectedTargetPeriod = null;
            Target = 0m;
            LoadSalesmen();
            LoadTargetPeriods();
        }

        private void OnSalemanChanged()
        {
            if (SelectedSalesman == null) return;
            SalesmanTargets.Clear();
            Target = 0m;
            LoadTargetPeriods();
        }

        private void OnTargetPeriodChanged()
        {
            if (SelectedTargetPeriod == null) return;
            SalesmanTargets.Clear();
            Target = 0m;
            if (SelectedTargetPeriod.Id != Guid.Empty)
                LoadSalesmanTargets();
        }

        private void LoadTargetPeriods()
        {
            TargetPeriods.Clear();
            var targetPeriodNullo = new TargetPeriod(Guid.Empty)
            {
                Name = "-- Select Target --"
            };
            TargetPeriods.Add(targetPeriodNullo);
            SelectedTargetPeriod = targetPeriodNullo;
            using (StructureMap.IContainer c = NestedContainer)
            {
                Using<ITargetPeriodRepository>(c).GetAll().OrderBy(n => n.Name)
                    .ToList().ForEach(TargetPeriods.Add);
            }
        }

        private void LoadSalesmen()
        {
            Salesmen.Clear();
            var salesmanNullo = new Salesman
                                    {
                                        UserId = Guid.Empty,
                                        CostCentreId = Guid.Empty,
                                        Username = "--Select Salesman--"
                                    };
            Salesmen.Add(salesmanNullo);
            using (StructureMap.IContainer c = NestedContainer)
            {
                Using<IUserRepository>(c).GetAll()
                    .Where(n => n.UserType == UserType.DistributorSalesman)
                    .Select(n => new Salesman
                                     {
                                         UserId = n.Id,
                                         CostCentreId = n.CostCentre,
                                         Username = n.Username
                                     })
                    .ToList().ForEach(n => Salesmen.Add(n));
            }
            SelectedSalesman = salesmanNullo;
        }

        private void LoadSalesmanTargets()
        {
            if (SelectedSalesman.CostCentreId != Guid.Empty)
            {
                using (StructureMap.IContainer c = NestedContainer)
                {
                    var targets = Using<ITargetRepository>(c).GetAll()
                            .Where(n => n.CostCentre != null && n.CostCentre.Id == SelectedSalesman.CostCentreId
                                && SelectedTargetPeriod != null && n.TargetPeriod.Id == SelectedTargetPeriod.Id)
                            .ToList();
                    var items = targets.OrderBy(n => n.TargetPeriod.StartDate)
                        .Select(n =>
                                    {
                                        var item = new SalesmanTarget
                                                       {
                                                           Id = n.Id,
                                                           PeriodId = n.TargetPeriod.Id,
                                                           Period = n.TargetPeriod.Name,
                                                           From = n.TargetPeriod.StartDate,
                                                           To = n.TargetPeriod.EndDate,
                                                           Target = n.TargetValue,
                                                           IsQtyTarget = n.IsQuantityTarget,
                                                           EntityStatus = (int)n._Status,
                                                       };
                                        return item;
                                    });
                    items.ToList().ForEach(SalesmanTargets.Add);
                    var target = items.FirstOrDefault();
                    if (target != null)
                    {
                        Target = target.Target;
                        TargetId = target.Id;
                    }
                }
            }
        }

        public async void Save()
        {
            SalesmanTargets.Clear();
            AddNewTarget();
            using (var c = NestedContainer)
            {
                ResponseBool response = null;
                var proxy = Using<IDistributorServiceProxy>(c);
                CostCentreTargetItem targetItem = null;
                foreach (var target in SalesmanTargets)
                {
                    targetItem = new CostCentreTargetItem
                    {
                        MasterId = target.Id,
                        CostCentreMasterId = SelectedSalesmanName.CostCentre,
                        TargetPeriodMasterId = target.PeriodId,
                        IsQuantityTarget = target.IsQtyTarget,
                        TargetValue = target.Target
                    };
                }
                response = await proxy.TargetAddAsync(targetItem);
                MessageBox.Show(response.ErrorInfo, "Distributr:", MessageBoxButton.OK);
            }
            Load();
        }

        private void Cancel()
        {
            Load();
        }

        private void EditExistingTarget()
        {
        }

        private void AddNewTarget()
        {
            TargetId = Guid.NewGuid();
            var targetItem = new SalesmanTarget
                                 {
                                     Id = TargetId,
                                     PeriodId = SelectedTargetPeriod.Id,
                                     Period = SelectedTargetPeriod.Name,
                                     From = SelectedTargetPeriod.StartDate,
                                     To = SelectedTargetPeriod.EndDate,
                                     Target = Target,
                                     IsQtyTarget = SetAsQty,
                                 };

            SalesmanTargets.Add(targetItem);
        }

        public async void OnDeactivateTargetClicked()
        {
            if (MessageBox.Show("Are you sure you want to deactivate the selected target?"
                , "Distributr: Deactivate Outlet Targets"
                , MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                return;
            if (SelectedSalesman != null)
            {
                using (StructureMap.IContainer c = NestedContainer)
                {
                    ResponseBool response = null;
                    var proxy = Using<IDistributorServiceProxy>(c);
                    response = await proxy.TargetDeactivateAsync(SelectedSalesman.CostCentreId);

                    MessageBox.Show(response.ErrorInfo, "Distributr:", MessageBoxButton.OK);
                }
            }
        }

        private async void OnDeleteTargetClicked(SalesmanTarget target)
        {
            if (MessageBox.Show("Are you sure you want to delete the selected target?"
                , "Distributr: Delete Outlet Targets"
                , MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                return;
            if (SelectedSalesman != null)
            {
                using (StructureMap.IContainer c = NestedContainer)
                {
                    ResponseBool response = null;
                    var proxy = Using<IDistributorServiceProxy>(c);
                    response = await proxy.TargetDeleteAsync(TargetId);

                    MessageBox.Show(response.ErrorInfo, "Distributr:", MessageBoxButton.OK);
                }
            }
            Load();
        }
        #endregion

        #region Properties
        public RelayCommand OnSalesmanChangedCommand { get; set; }
        public RelayCommand OnTargetPeriodChangedCommand { get; set; }
        public RelayCommand AddNewTargetCommand { get; set; }
        public RelayCommand EditExistingTargetCommand { get; set; }
        public RelayCommand CancelEditCommand { get; set; }
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        public RelayCommand LoadCommand { get; set; }
        public RelayCommand<SalesmanTarget> OnEditTargetClickedCommand { get; set; }
        public RelayCommand<SalesmanTarget> OnDeleteTargetClickedCommand { get; set; }
        public RelayCommand SalemanDropDownOpenedCommand { get; set; }
        public RelayCommand PeriodDropDownOpenedCommand { get; set; }

        
        public ObservableCollection<Salesman> Salesmen { get; set; }
        public ObservableCollection<TargetPeriod> TargetPeriods { get; set; }
        public ObservableCollection<SalesmanTarget> SalesmanTargets { get; set; }
        public const string SelectedSalesmanNamePropertyName = "SelectedSalesmanName";


      
        public const string SalesmanNameHolderPropertyName = "SalesmanNameHolder";

        private string _salesmanNameHolder = "";
        public string SalesmanNameHolder
        {
            get
            {
                return _salesmanNameHolder;
            }

            set
            {
                if (_salesmanNameHolder == value)
                {
                    return;
                }

                RaisePropertyChanging(SalesmanNameHolderPropertyName);
                _salesmanNameHolder = value;
                RaisePropertyChanged(SalesmanNameHolderPropertyName);
            }
        }
        private User _user = new User(Guid.Empty){FirstName = "---Select Salesman---"};
        public User SelectedSalesmanName
        {
            get
            {
                return _user;
            }

            set
            {
                if (_user == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedSalesmanNamePropertyName);
                _user = value;
                RaisePropertyChanged(SelectedSalesmanNamePropertyName);
            }
        }




        public const string SelectedSalesmanPropertyName = "SelectedSalesman";
        private Salesman _selectedSalesman = null;
        public Salesman SelectedSalesman
        {
            get
            {
                return _selectedSalesman;
            }

            set
            {
                if (_selectedSalesman == value)
                {
                    return;
                }

                var oldValue = _selectedSalesman;
                _selectedSalesman = value;
                RaisePropertyChanged(SelectedSalesmanPropertyName);
            }
        }

        public const string SelectedTargetPeriodPropertyName = "SelectedTargetPeriod";
        private TargetPeriod _selectedTargetPeriod = null;
        public TargetPeriod SelectedTargetPeriod
        {
            get
            {
                return _selectedTargetPeriod;
            }

            set
            {
                if (_selectedTargetPeriod == value)
                {
                    return;
                }

                var oldValue = _selectedTargetPeriod;
                _selectedTargetPeriod = value;
                RaisePropertyChanged(SelectedTargetPeriodPropertyName);
            }
        }

        public const string SetAsQtyPropertyName = "SetAsQty";
        private bool _setAsQty = true;
        public bool SetAsQty
        {
            get
            {
                return _setAsQty;
            }

            set
            {
                if (_setAsQty == value)
                {
                    return;
                }

                var oldValue = _setAsQty;
                _setAsQty = value;
                SetAsAmt = !value;
                RaisePropertyChanged(SetAsQtyPropertyName);
            }
        }

        public const string SetAsAmtPropertyName = "SetAsAmt";
        private bool _setAsAmt = false;
        public bool SetAsAmt
        {
            get
            {
                return _setAsAmt;
            }

            set
            {
                if (_setAsAmt == value)
                {
                    return;
                }

                var oldValue = _setAsAmt;
                _setAsAmt = value;
                RaisePropertyChanged(SetAsAmtPropertyName);
            }
        }

        public const string TargetIdPropertyName = "TargetId";
        private Guid _targetId = Guid.Empty;
        public Guid TargetId
        {
            get
            {
                return _targetId;
            }

            set
            {
                if (_targetId == value)
                {
                    return;
                }

                var oldValue = _targetId;
                _targetId = value;
                RaisePropertyChanged(TargetIdPropertyName);
            }
        }

        public const string TargetPropertyName = "Target";
        private decimal _target = 0m;
        public decimal Target
        {
            get
            {
                return _target;
            }

            set
            {
                if (_target == value)
                {
                    return;
                }
                var oldValue = _target;
                _target = value;
                CachedTarget = oldValue;
                CanSave = true;
                RaisePropertyChanged(TargetPropertyName);
            }
        }

        private decimal _cachedTarget;
        public decimal CachedTarget
        {
            get { return _cachedTarget; }
            set { _cachedTarget = value; }
        }

        public const string CanSavePropertyName = "CanSave";
        private bool _canSave = false;
        public bool CanSave
        {
            get
            {
                return Target != CachedTarget;
            }

            set
            {
                if (_canSave == value)
                {
                    return;
                }

                var oldValue = _canSave;
                _canSave = value;
                RaisePropertyChanged(CanSavePropertyName);
            }
        }

        public const string CanCancelPropertyName = "CanCancel";
        private bool _canCancel = false;
        public bool CanCancel
        {
            get
            {
                return CanSave;
            }

            set
            {
                if (_canCancel == value)
                {
                    return;
                }

                var oldValue = _canCancel;
                _canCancel = value;
                RaisePropertyChanged(CanCancelPropertyName);
            }
        }
        #endregion

        #region Helper Classes
        public class Salesman
        {
            public Guid UserId { get; set; }
            public Guid CostCentreId { get; set; }
            public string Username { get; set; }
        }

        public class SalesmanTarget
        {
            public Guid Id { get; set; }
            public decimal Target { get; set; }
            public bool IsQtyTarget { get; set; }
            public string TargetSetAs
            {
                get { return IsQtyTarget ? "Quantity" : "Amount"; }
            }
            public Guid PeriodId { get; set; }
            public string Period { get; set; }
            public DateTime From { get; set; }
            public DateTime To { get; set; }
            public int EntityStatus { get; set; }
            public string HlkDeactivateContent { get; set; }
        }
        #endregion
    }
}
