using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Windows;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.DistributorTargetEntities;
using Distributr.Core.MasterDataDTO.DataContracts;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.DistributorTargetRepositories;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using Distributr.WPF.Lib.UI.Pages;
using GalaSoft.MvvmLight.Command;

namespace Distributr.WPF.Lib.ViewModels.Admin.Outlets
{
    public class EditOutletTargetsViewModel : DistributrViewModelBase
    {
        private Target target;
        public EditOutletTargetsViewModel()
        {
            RouteChangedCommand = new RelayCommand(RunRouteChangedCommand);
            OutletChangedCommand = new RelayCommand(RunOutletChangedCommand);
            TargetPeriodChangedCommand = new RelayCommand(RunTargetPeriodChangedCommand);
            AddNewCommand = new RelayCommand(RunAddNewCommand);
            CancelCommand = new RelayCommand(RunCancelCommand);
            EditCommand = new RelayCommand(RunEditCommand);
            RouteDropDownOpenedCommand = new RelayCommand(RouteDropDownOpened);
            OutletDropDownOpenedCommand = new RelayCommand(OutletDropDownOpened);
            PeriodDropDownOpenedCommand = new RelayCommand(PeriodDropDownOpened);


            SaveItem = new RelayCommand(Save);
            Routes = new ObservableCollection<Route>();
            RouteOutlets = new ObservableCollection<Outlet>();
            OutletTargets = new ObservableCollection<OutletTarget>();
            TargetPeriods = new ObservableCollection<TargetPeriod>();
            TargetInfo = new List<TargetSummary>();
        }

      /*  private bool CanSave()
        {
           /* return !string.IsNullOrEmpty(Name);#1#
            return int.MinValue (Target) ;
        }*/

        private void PeriodDropDownOpened()
        {
            using (var container = NestedContainer)
            {
              
                SelectedTargetPeriod = Using<IItemsLookUp>(container).SelectTargetPeriod(); //??default;
            }
          

        }

        private void OutletDropDownOpened()
        {

            if (SelectedRoute != null )
            using (var container = NestedContainer)
            {
                SelectedOutlet = Using<IItemsLookUp>(container).SelectOutletByRoute(SelectedRoute.Id); //??default;
            }
            if (SelectedRoute == null || SelectedRoute.Name == "--Please Select Route--")
            {
              if(  MessageBox.Show("Select Route First", "Distributr", MessageBoxButton.YesNo)==MessageBoxResult.Yes )
            RouteDropDownOpened();
            }
        }

        private void RouteDropDownOpened()
        {
            using (var container = NestedContainer)
            {
                SelectedRoute = Using<IItemsLookUp>(container).SelectRoute(); //??default;
            }
        }


        #region Properties

        public ObservableCollection<Route> Routes { get; set; }
        public ObservableCollection<Outlet> RouteOutlets { get; set; }
        public ObservableCollection<TargetPeriod> TargetPeriods { get; set; }


        public RelayCommand PeriodDropDownOpenedCommand { get; set; }
        public RelayCommand OutletDropDownOpenedCommand { get; set; }
        public RelayCommand SaveItem { get; set; }
        public RelayCommand RouteDropDownOpenedCommand { get; set; }
        public RelayCommand OutletChangedCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        public RelayCommand AddNewCommand { get; set; }
        public RelayCommand RouteChangedCommand { get; set; }
        public RelayCommand TargetChangedCommand { get; set; }
        public RelayCommand EditCommand { get; set; }
        public RelayCommand TargetPeriodChangedCommand { get; set; }
        public List<TargetSummary> TargetInfo { get; set; }

        public const string SelectedRoutePropertyName = "SelectedRoute";
        private Route _selectedRoute = new Route( Guid.Empty ){Name = "---Select Route---"};
        public Route SelectedRoute
        {
            get
            {
                return _selectedRoute;
            }

            set
            {
                if (_selectedRoute == value)
                {
                    return;
                }

                var oldValue = _selectedRoute;
                _selectedRoute = value;
                RaisePropertyChanged(SelectedRoutePropertyName);
            }
        }

        public const string SelectedOutletPropertyName = "SelectedOutlet";
        private Outlet _selectedOutlet = new Outlet( Guid.Empty){Name = "---Select Outlet---"};
        public Outlet SelectedOutlet
        {
            get
            {
                return _selectedOutlet;
            }

            set
            {
                if (_selectedOutlet == value)
                {
                    return;
                }

                var oldValue = _selectedOutlet;
                _selectedOutlet = value;
                RaisePropertyChanged(SelectedOutletPropertyName);
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

        public const string StartDatePropertyName = "StartDate";
        private DateTime _startDate = DateTime.Now;
        public DateTime StartDate
        {
            get
            {
                return _startDate;
            }

            set
            {
                if (_startDate == value)
                {
                    return;
                }

                var oldValue = _startDate;
                _startDate = value;
                RaisePropertyChanged(StartDatePropertyName);
            }
        }

        public const string EndDatePropertyName = "EndDate";
        private DateTime _endDate = DateTime.Now;
        public DateTime EndDate
        {
            get
            {
                return _endDate;
            }

            set
            {
                if (_endDate == value)
                {
                    return;
                }

                var oldValue = _endDate;
                _endDate = value;
                RaisePropertyChanged(EndDatePropertyName);
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
        [Required(ErrorMessage = "You must enter target.")]
        [Range(0, 999999999, ErrorMessage = "Enter a number greater than 0.")]
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
                RaisePropertyChanged(TargetPropertyName);
                SaveItem.RaiseCanExecuteChanged();
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

        public const string SelectedOutletTargetPropertyName = "SelectedOutletTarget";
        private OutletTarget _selectedOutletTarget = null;
        public OutletTarget SelectedOutletTarget
        {
            get
            {
                return _selectedOutletTarget;
            }

            set
            {
                if (_selectedOutletTarget == value)
                {
                    return;
                }

                var oldValue = _selectedOutletTarget;
                _selectedOutletTarget = value;
                RaisePropertyChanged(SelectedOutletTargetPropertyName);
            }
        }

        public ObservableCollection<OutletTarget> OutletTargets { get; set; }

        public const string AddingNewPropertyName = "AddingNew";
        private bool _addingNew = false;
        public bool AddingNew
        {
            get
            {
                return _addingNew;
            }

            set
            {
                if (_addingNew == value)
                {
                    return;
                }

                var oldValue = _addingNew;
                _addingNew = value;
                RaisePropertyChanged(AddingNewPropertyName);
            }
        }

        public const string IsEditingPropertyName = "IsEditing";
        private bool _isEditing = false;
        public bool IsEditing
        {
            get
            {
                return _isEditing;
            }

            set
            {
                if (_isEditing == value)
                {
                    return;
                }

                var oldValue = _isEditing;
                _isEditing = value;
                RaisePropertyChanged(IsEditingPropertyName);
            }
        }

        public const string ShowInactivePropertyName = "ShowInactive";
        private bool _showInactive = false;
        public bool ShowInactive
        {
            get
            {
                return _showInactive;
            }

            set
            {
                if (_showInactive == value)
                {
                    return;
                }

                var oldValue = _showInactive;
                _showInactive = value;
                RaisePropertyChanged(ShowInactivePropertyName);
            }
        }
        #endregion

        #region Methods

        private DateTime now;
        //private BusyWindow _busyPopup;

        public void Setup()
        {
            ShowInactive = false;
        }

        public void LoadTarget()
        {

            LoadTargetPeriods();
            now = DateTime.Now;
            if (TargetId == Guid.Empty)
            {
                CreateNewTarget();
            }
            else
            {
                using (StructureMap.IContainer c = NestedContainer)
                {
                    target = Using<ITargetRepository>(c).GetById(TargetId);
                    Target = target.TargetValue;
                    SetAsQty = target.IsQuantityTarget;
                    SelectedTargetPeriod = TargetPeriods.FirstOrDefault(n => n.Id == target.TargetPeriod.Id);
                    SelectedOutlet = RouteOutlets.FirstOrDefault(n => n.Id == target.CostCentre.Id);
                    SelectedRoute = Routes.FirstOrDefault(n => n.Id == SelectedOutlet.Route.Id);
                    MapPeriod();
                }
            }
        }

        void LoadTargetFromGrid()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                target = Using<ITargetRepository>(c).GetById(TargetId, true);
            }
            Target = target.TargetValue;
            SetAsQty = target.IsQuantityTarget;
            SelectedTargetPeriod = TargetPeriods.FirstOrDefault(n => n.Id == target.TargetPeriod.Id);
            MapPeriod();
            CreateTargetSummary();
        }

        void CreateNewTarget()
        {
            TargetId = Guid.NewGuid();
            target = new Target(TargetId);
            target._SetDateCreated(now);
        }

        public async void Save()
        {

            if (Target == 0)
            {
                MessageBox.Show("Select Target Period", "Distributor", MessageBoxButton.OKCancel);
            }
            if (true)
            {
                CostCentreTargetItem targetItem = null;
                bool isNew = false;
                if (TargetId == Guid.Empty)
                {
                    TargetId = Guid.NewGuid();
                    isNew = true;
                }

                targetItem = new CostCentreTargetItem
                                 {
                                     MasterId = TargetId,
                                     CostCentreMasterId = SelectedOutlet.Id,
                                     TargetPeriodMasterId = SelectedTargetPeriod.Id,
                                     IsQuantityTarget = !SetAsAmt,
                                     TargetValue = Target
                                 };
                if (isNew)
                {
                    using (StructureMap.IContainer c = NestedContainer)
                    {
                        ResponseBool response = null;
                        IDistributorServiceProxy proxy = Using<IDistributorServiceProxy>(c);
                        response = await proxy.TargetAddAsync(targetItem);

                        MessageBox.Show(response.ErrorInfo, "Distributr:", MessageBoxButton.OK);

                    }

                }
                else
                {
                    using (StructureMap.IContainer c = NestedContainer)
                    {
                        ResponseBool response = null;
                        IDistributorServiceProxy proxy = Using<IDistributorServiceProxy>(c);
                        response = await proxy.TargetUpdateAsync(targetItem);

                        MessageBox.Show(response.ErrorInfo, "Distributr:", MessageBoxButton.OK);

                    }
                }
                ClearViewModel();
            }
           
        }
      
        public void ClearViewModel()
        {
            AddingNew = false;
            IsEditing = false;
            target = null;
            TargetId = Guid.Empty;
            Target = 0m;
            SelectedOutlet = new Outlet(Guid.Empty) { Name = "---Select Outlet---" };
        }

        public void LoadRoutes()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                Routes.Clear();
                var route = new Route(Guid.Empty)
                    {
                        Name = GetLocalText("sl.target.defaultroute")
                        /*"-- Please Select A Route --"*/
                    };
                Routes.Add(route);
                Using<IRouteRepository>(c).GetAll().ToList().OrderBy(n => n.Name).ToList().ForEach(n => Routes.Add(n));
                SelectedRoute = route;
            }
        }

        public void LoadTargetPeriods()
        {
            TargetPeriods.Clear();
            var targetPeriod = new TargetPeriod(Guid.Empty) { Name = GetLocalText("sl.target.defaulttagerperiod") /*"--Please Select A Target--"*/ };
            TargetPeriods.Add(targetPeriod);
            SelectedTargetPeriod = targetPeriod;
            using (StructureMap.IContainer c = NestedContainer)
            {
                Using<ITargetPeriodRepository>(c).GetAll().OrderBy(n => n.Name).ToList().ForEach(TargetPeriods.Add);
            }
        }

        public void LoadOutlets()
        {
            RouteOutlets.Clear();
            var outlet = new Outlet(Guid.Empty) { Name = GetLocalText("sl.target.defaultoutlet") /*"--Please Select Outlet--"*/};
            RouteOutlets.Add(outlet);
            SelectedOutlet = outlet;

            if (SelectedRoute.Id != Guid.Empty)
            {
                using (StructureMap.IContainer c = NestedContainer)
                {
                    Using<ICostCentreRepository>(c)
                        .GetAll()
                        .OfType<Outlet>()
                        .Where(n => n._Status == EntityStatus.Active)
                        .Where(n => n.Route != null && n.Route.Id == SelectedRoute.Id)
                        .OrderBy(n => n.Name)
                        .ToList()
                        .ForEach(RouteOutlets.Add);
                }
            }
        }

        void RunAddNewCommand()
        {
            ClearViewModel();
            AddingNew = true;
            IsEditing = true;
            TargetInfo.Clear();
            SelectedRoute = Routes.FirstOrDefault(n => n.Id == Guid.Empty);
            SelectedTargetPeriod = TargetPeriods.FirstOrDefault(n => n.Id == Guid.Empty);
            Target = 0;
        }

        void RunRouteChangedCommand()
        {
            if (SelectedRoute != null)
            {
                LoadOutlets();
            }
        }

        public void RunOutletChangedCommand()
        {
            OutletTargets.Clear();
            TargetInfo.Clear();
            if (SelectedOutlet != null)
            {
                if (SelectedOutlet.Id != Guid.Empty)
                {
                    using (StructureMap.IContainer c = NestedContainer)
                    {
                        var targets =
                            Using<ITargetRepository>(c).GetAll(ShowInactive)
                                          .Where(n => n.CostCentre != null && n.CostCentre.Id == SelectedOutlet.Id)
                                          .ToList();
                        var items = targets.OrderBy(n => n.TargetPeriod.StartDate)
                                           .Select(n =>
                                               {
                                                   var item = new OutletTarget
                                                       {
                                                           Id = n.Id,
                                                           PeriodId = n.TargetPeriod.Id,
                                                           Period = n.TargetPeriod.Name,
                                                           From = n.TargetPeriod.StartDate,
                                                           To = n.TargetPeriod.EndDate,
                                                           Target = n.TargetValue,
                                                           IsQtyTarget = n.IsQuantityTarget,
                                                           EntityStatus = (int) n._Status,
                                                       };
                                                   if (item.EntityStatus == (int) EntityStatus.Active)
                                                       item.HlkDeactivateContent =
                                                           GetLocalText("sl.target.grid.col.deactivate");
                                                   else if (item.EntityStatus == (int) EntityStatus.Inactive)
                                                       item.HlkDeactivateContent =
                                                           GetLocalText("sl.target.grid.col.activate");

                                                   return item;
                                               });
                        items.ToList().ForEach(OutletTargets.Add);
                        if (AddingNew)
                            SelectedOutletTarget = null;
                        else
                            SelectedOutletTarget = OutletTargets.FirstOrDefault();
                    }
                }
                else
                { SelectedTargetPeriod = TargetPeriods.FirstOrDefault(n => n.Id == Guid.Empty); }
            }
        }

        void MapPeriod()
        {
            if (SelectedTargetPeriod != null && SelectedTargetPeriod.Id != Guid.Empty)
            {
                StartDate = SelectedTargetPeriod.StartDate;
                EndDate = SelectedTargetPeriod.EndDate;
            }
        }

        public void RunTargetPeriodChangedCommand()
        {
           if (SelectedTargetPeriod != null)
            {
                if (SelectedTargetPeriod.Id != Guid.Empty)
                {
                    var target_ = OutletTargets.FirstOrDefault(n => n.PeriodId == SelectedTargetPeriod.Id);
                    if (target_ != null)
                    {
                        TargetId = target_.Id;
                        LoadTargetFromGrid();
                    }
                    else
                    {
                        TargetId = Guid.Empty;
                        Target = 0m;
                        SetAsQty = true;
                        TargetInfo.Clear();
                        MapPeriod();
                    }
                }
            }
        }

        public void RunSelectedTargetChangedCommand()
        {
            if (SelectedOutletTarget != null && SelectedOutletTarget.Id != Guid.Empty)
            {
                TargetId = SelectedOutletTarget.Id;
                LoadTargetFromGrid();
            }
        }

        public void RunTargetChangedCommand()
        {
            CreateTargetSummary();
        }

       

        void RunCancelCommand()
        {
            if (
                MessageBox.Show(/*"Are you sure you want to cancel and lose any unsaved changes?"*/
                                GetLocalText("sl.target.cancel.messagebox.text")
                , GetLocalText("sl.target.cancel.messagebox.caption")/*"Distributr: Set Outlet Targets"*/
                , MessageBoxButton.OKCancel)
                == MessageBoxResult.Cancel)
                return;
            ClearViewModel();
        }

        void CreateTargetSummary()
        {
            TargetInfo.Clear();
            TimeSpan ts = EndDate.Subtract(StartDate);
            int numDays = ts.Days;

            int numMonths = (int)(numDays / DateTime.DaysInMonth(DateTime.Today.Year, StartDate.Month));
            int numYears = (int)(numDays / ((new DateTime(DateTime.Today.Year, 12, 31)).DayOfYear));

            if (numDays > 0)
            {
                var dailyTarget = new TargetSummary
                                      {
                                          Period = GetLocalText("sl.target.summary.daily")/* "Daily Target"*/,
                                          Target = (Target / numDays)
                                      };

                TargetInfo.Add(dailyTarget);
            }
            if (numMonths > 0)
            {
                var monthlyTarget = new TargetSummary
                                        {
                                            Period = GetLocalText("sl.target.summary.monthly") /*"Monthly Target"*/,
                                            Target = (Target / numMonths)
                                        };

                TargetInfo.Add(monthlyTarget);
            }
            if (numYears > 0)
            {
                var yearlyTarget = new TargetSummary
                                       {
                                           Period = GetLocalText("sl.target.summary.yearly") /*"Yearly"*/,
                                           Target = (Target / numYears)
                                       };

                TargetInfo.Add(yearlyTarget);
            }
        }

        public void RunEditCommand()
        {
            IsEditing = true;
        }

      

        public async void DeactivateTarget()
        {
            if (
                MessageBox.Show( /*"Are you sure you want to deactivate the selected target?"*/
                    GetLocalText("sl.target.deactivate.messagebox.text")
                    ,GetLocalText("sl.target.deactivate.messagebox.caption")
                /*"Distributr: Deactivate Outlet Targets"*/
                    , MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                return;
            if (SelectedOutletTarget != null)
            {
                using (StructureMap.IContainer c = NestedContainer)
                {
                    ResponseBool response = null;
                    IDistributorServiceProxy proxy = Using<IDistributorServiceProxy>(c);
                    response = await proxy.TargetDeactivateAsync(SelectedOutletTarget.Id);

                    MessageBox.Show(response.ErrorInfo, "Distributr:", MessageBoxButton.OK);

                }
               
            }
        }

       
        public async void ActivateTarget()
        {
          
            if (
                MessageBox.Show("Are you sure you want to activate the selected target?"
               , "Distributr: Deactivate Outlet Targets"
               , MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                return;
            if (SelectedOutletTarget != null)
            {
                using (StructureMap.IContainer c = NestedContainer)
                {
                    ResponseBool response = null;
                    IDistributorServiceProxy proxy = Using<IDistributorServiceProxy>(c);
                    response = await proxy.TargetActivateAsync(SelectedOutletTarget.Id);

                    MessageBox.Show(response.ErrorInfo, "Distributr:", MessageBoxButton.OK);

                }
            }
        }

      
        public async void DeleteTarget()
        {
            if (
               MessageBox.Show("Are you sure you want to delete the selected target?"
                , "Distributr: Delete Outlet Targets"
                , MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                return;
            if (SelectedOutletTarget != null)
            {
                using (StructureMap.IContainer c = NestedContainer)
                {
                    ResponseBool response = null;
                    IDistributorServiceProxy proxy = Using<IDistributorServiceProxy>(c);
                    response = await proxy.TargetDeleteAsync(SelectedOutletTarget.Id);

                    MessageBox.Show(response.ErrorInfo, "Distributr:", MessageBoxButton.OK);

                }
            }
        }

     

        #endregion
    }

    #region Other Classes
    public class TargetSummary
    {
        public string Period { get; set; }
        public decimal Target { get; set; }
    }

    public class OutletTarget
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