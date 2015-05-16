using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
//using Distributr.WPF.Lib.DistributorServiceProxy;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using Distributr.WPF.Lib.UI.Pages;
using GalaSoft.MvvmLight.Command;
using System.Linq;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.MasterDataDTO.DataContracts;

namespace Distributr.WPF.Lib.ViewModels.Admin.Outlets
{
    public class EditOutletPriorityViewModel : DistributrViewModelBase
    {
       
        private List<OutletPriority> outletPriorities;
        public EditOutletPriorityViewModel()
        {
           
            outletPriorities = new List<OutletPriority>();
            Routes = new ObservableCollection<Route>();
            RouteOutlets = new ObservableCollection<ListOutletItem>();
            SelectedRouteChangedCommand = new RelayCommand(RunSelectedRouteChanged);
            MoveOutletTopCommand = new RelayCommand(RunMoveOutletTopCommand);
            MoveOutletUpCommand = new RelayCommand(RunMoveOutletUpCommand);
            MoveOutletDownCommand = new RelayCommand(RunMoveOutletDownCommand);
            MoveOutletBottomCommand = new RelayCommand(RunMoveOutletBottomCommand);
            SaveCommand = new RelayCommand(RunSaveCommand);
            DeactivateCommand = new RelayCommand(RunDeactivateCommand);
            CancelCommand = new RelayCommand(RunCancelCommand);
            QuickSetPriorityCommand = new RelayCommand(RunQuickSetPriorityCommand);
            RouteDropDownOpenedCommand = new RelayCommand(RouteDropDownOpened);
        }

        private void RouteDropDownOpened()
        {
            using (var container = NestedContainer)
            {
                SelectedRoute = Using<IItemsLookUp>(container).SelectRoute(); //??default;
                ListOutletPriorities();
            }
        }

        #region Properties
        public ObservableCollection<Route> Routes { get; set; }
        public ObservableCollection<ListOutletItem> RouteOutlets { get; set; }

        public const string QuickSetRouteOutletsPropertyName = "QuickSetRouteOutlets";
        private ObservableCollection<ListOutletItem> _quickSetRouteOutlets;
        public ObservableCollection<ListOutletItem> QuickSetRouteOutlets
        {
            get
            {
                _quickSetRouteOutlets = new ObservableCollection<ListOutletItem>();
                var outlet = new ListOutletItem { Id = Guid.Empty, Name = "--Please Select An Outlet--", Priority = 0 };
                _quickSetRouteOutlets.Add(outlet);
                if (SelectedRouteOutlet == null)
                    SelectedRouteOutlet = outlet;
                RouteOutlets.OrderBy(n => n.Name).ToList().ForEach(_quickSetRouteOutlets.Add);
                return _quickSetRouteOutlets;
            }
        
             set
            {
                if (_quickSetRouteOutlets == value)
                {
                    return;
                }

                var oldValue = _selectedRoute;
                _quickSetRouteOutlets = value;
                RaisePropertyChanged(QuickSetRouteOutletsPropertyName);
            }
        }
        
        public RelayCommand SelectedRouteChangedCommand { get; set; }
        public RelayCommand MoveOutletTopCommand { get; set; }
        public RelayCommand MoveOutletUpCommand { get; set; }
        public RelayCommand MoveOutletDownCommand { get; set; }
        public RelayCommand MoveOutletBottomCommand { get; set; }
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand DeactivateCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        public RelayCommand QuickSetPriorityCommand { get; set; }
        public RelayCommand RouteDropDownOpenedCommand { get; set; }

        public const string SelectedRoutePropertyName = "SelectedRoute";
        private Route _selectedRoute = null;
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

        public const string SelectedRouteOutletPropertyName = "SelectedRouteOutlet";
        private ListOutletItem _selectedRouteOutlet = null;
        public ListOutletItem SelectedRouteOutlet
        {
            get
            {
                return _selectedRouteOutlet;
            }

            set
            {
                if (_selectedRouteOutlet == value)
                {
                    return;
                }

                var oldValue = _selectedRouteOutlet;
                _selectedRouteOutlet = value;
                RaisePropertyChanged(SelectedRouteOutletPropertyName);
            }
        }
         
        public const string EffectiveDatePropertyName = "EffectiveDate";
        private DateTime _effectiveDate = DateTime.Now;
        public DateTime EffectiveDate
        {
            get
            {
                return _effectiveDate;
            }

            set
            {
                if (_effectiveDate == value)
                {
                    return;
                }

                var oldValue = _effectiveDate;
                _effectiveDate = value;
                //IsDirty = true;
                RaisePropertyChanged(EffectiveDatePropertyName);
            }
        }

        public const string OutletPriorityToSetPropertyName = "OutletPriorityToSet";
        private int _outletPriorityToSet = 1;
        public int OutletPriorityToSet
        {
            get
            {
                return _outletPriorityToSet;
            }

            set
            {
                if (_outletPriorityToSet == value)
                {
                    return;
                }
                var oldValue = _outletPriorityToSet;
                _outletPriorityToSet = value;
                RaisePropertyChanged(OutletPriorityToSetPropertyName);
            }
        }

        public const string IsDirtyPropertyName = "IsDirty";
        private bool _isDirty = false;
        public bool IsDirty
        {
            get
            {
                return _isDirty;
            }

            set
            {
                if (_isDirty == value)
                {
                    return;
                }

                var oldValue = _isDirty;
                _isDirty = value;
                RaisePropertyChanged(IsDirtyPropertyName);
            }
        }

        public const string CanSetPriorityPropertyName = "CanSetPriority";
        private bool _canSetPriority = false;
        public bool CanSetPriority
        {
            get
            {
                return _canSetPriority;
            }

            set
            {
                if (_canSetPriority == value)
                {
                    return;
                }

                var oldValue = _canSetPriority;
                _canSetPriority = value;
                RaisePropertyChanged(CanSetPriorityPropertyName);
            }
        }

        #endregion

        #region Methods

        async void  RunSaveCommand()
        {
            DateTime now = DateTime.Now;
            if (EffectiveDate.Date < DateTime.Now.Date)
            {
                MessageBox.Show("Effective date should be later than or equal to today's date.",
                                "Distributor: Outlet Prioritization", MessageBoxButton.OK);
                EffectiveDate = DateTime.Now;
                return;
            }
            if (MessageBox.Show("Are you sure you want to save this prioritization?", "Distributr: Outlets Prioritization", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                return;
            using (StructureMap.IContainer c = NestedContainer)
            {
                try
                {

                    ResponseBool response = null;
                    IDistributorServiceProxy proxy = Using<IDistributorServiceProxy>(c);

                    List<OutletPriorityItem> outletPriorityItems = new List<OutletPriorityItem>();
                    foreach (ListOutletItem item in RouteOutlets)
                    {
                        Guid id = Guid.NewGuid();
                        OutletPriority existing = Using<IOutletPriorityRepository>(c).GetById(item.PriorityId, true);
                        if (existing != null)
                            id = existing.Id;

                        var itemToSave = new OutletPriorityItem
                                             {
                                                 OutletId = item.Id,
                                                 Priority = item.Priority,
                                                 RouteId = SelectedRoute.Id,
                                                 EffectiveDate = EffectiveDate,
                                                 MasterId = id, 
                                                 StatusId = (int)EntityStatus.Active,
                                                 DateCreated = DateTime.Now,
                                                 DateLastUpdated = DateTime.Now 
                                             };

                        outletPriorityItems.Add(itemToSave);

                    }

                    response = await proxy.OutletPriorityAddAsync(outletPriorityItems);

                    IsDirty = false;
                    SelectedRoute = Routes.FirstOrDefault(n => n.Id == Guid.Empty);
                    OutletPriorityToSet = 1;
                    CanSetPriority = false;
                    MessageBox.Show(response.ErrorInfo, "Distributr: Manage Routes", MessageBoxButton.OK,
                                    MessageBoxImage.Information);

                    if (response.Success)
                    {
                        ConfirmNavigatingAway = false;
                        SendNavigationRequestMessage(new Uri(@"\views\administration\outlets\listoutlets.xaml", UriKind.Relative));
                    }
                }
                catch
                {
                    MessageBox.Show("An error occurred while saving outlet priorities.","Distributr: Outlet Prioritization",MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }

        }

       async void RunDeactivateCommand()
        {
            if (MessageBox.Show("Are you sure you want to deactive this prioritization?", "Distributr: Outlets Prioritization", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                return;
            try
            {
                using (StructureMap.IContainer c = NestedContainer)
                {
                    foreach (var p in outletPriorities)
                    {
                        ResponseBool response = null;
                        IDistributorServiceProxy proxy = Using<IDistributorServiceProxy>(c);
                       
                        response = await proxy.OutletPriorityDeactivateAsync(p.Id);
                        MessageBox.Show(response.ErrorInfo, "Distributr:", MessageBoxButton.OK);
                    }
                    IsDirty = false;
                }
            }
            catch
            {
                MessageBox.Show("An error occurred while deactivating outlet priorities.", "Distributr: Outlet Prioritization",
                                MessageBoxButton.OK);
            }
        }

        void RunCancelCommand()
        {
            if (MessageBox.Show("Are you sure you want to cancel this prioritization and lose all unsaved changes?", "Distributr: Outlets Prioritization", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                return;
            SelectedRoute = Routes.FirstOrDefault(n => n.Id == Guid.Empty);
            IsDirty = false;
            CanSetPriority = false;
        }

        void ListOutletPriorities()
        {
            RouteOutlets.Clear();
            if (SelectedRoute == null || SelectedRoute.Id == Guid.Empty)
                return;
            using (StructureMap.IContainer c = NestedContainer)
            {
                List<OutletPriority> ops =
                    Using<IOutletPriorityRepository>(c).GetAll().Where(n => n.Route.Id == SelectedRoute.Id).ToList();
                List<Outlet> outlets =
                    Using<ICostCentreRepository>(c).GetAll()
                                      .OfType<Outlet>()
                                      .Where(n =>n.Route != null && n._Status == EntityStatus.Active && n.Route.Id == SelectedRoute.Id).ToList();

                ops = ops.Where(n => outlets.Any(o => o.Id == n.Outlet.Id)).ToList();

                List<ListOutletItem> items = ops.OrderBy(n => n.Priority)
                    .Select((n, i) => new ListOutletItem
                    {
                        Priority = i + 1, //n.Priority
                        Id = n.Outlet.Id,
                        Name = outlets.FirstOrDefault(o => o.Id == n.Outlet.Id).Name,//== null ? "Fake" : outlets.FirstOrDefault(o => o.Id == n.Outlet.Id).Name,
                        PriorityId = n.Id,
                    }).ToList();

                outlets.Where(n => !ops.Select(op => op.Outlet.Id).Contains(n.Id)).OrderBy(n => n.Name).ToList()
                       .ForEach(n =>
                           {
                               int priority = 0;
                               if (items.Count > 0)
                                   priority = items.Max(i => i.Priority) + 1;
                               else priority = 1;
                               ListOutletItem item = new ListOutletItem
                                   {
                                       Priority = priority,
                                       Name = n.Name,
                                       Id = n.Id,
                                       PriorityId = Guid.Empty
                                   };
                               items.Add(item);
                           });
                //TODO: WORK ON AN ELEGANT WAY =>THIS IS A HACK
                foreach (var item in items)
                {
                    RouteOutlets.Add(item);
                }
                ops.ForEach(outletPriorities.Add);
                if (ops.Count() > 0)
                    EffectiveDate = ops.Max(n => n.EffectiveDate);
                CanSetPriority = RouteOutlets.Count > 0;
            }
        }

        public void LoadRoutes()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                IsDirty = false;
                Routes.Clear();
                Route route = new Route(Guid.Empty) {Name = "--Please Select A Route--"};
                Routes.Add(route);
                SelectedRoute = route;

                Using<IRouteRepository>(c).GetAll().OrderBy(n => n.Name).ToList().ForEach(Routes.Add);
            }
        }

        void RunSelectedRouteChanged()
        {
            ListOutletPriorities();
        }

        void RunMoveOutletTopCommand()
        {
            MoveTopOrBottom(true);
        }

        void RunMoveOutletUpCommand()
        {
            MoveItemUpOrDown(true);
        }

        void RunMoveOutletDownCommand()
        {
            MoveItemUpOrDown(false);
        }

        void RunMoveOutletBottomCommand()
        {
            MoveTopOrBottom(false);
        }

        void RunQuickSetPriorityCommand()
        {
            if (SelectedRouteOutlet == null || SelectedRouteOutlet.Id == Guid.Empty)
                return;
            if (OutletPriorityToSet == SelectedRouteOutlet.Priority)
                return;

            IsDirty = true;
            bool moveUp = OutletPriorityToSet < SelectedRouteOutlet.Priority;
            int selectedItemIndex = SelectedRouteOutlet.Priority;

            var workingList = new ObservableCollection<ListOutletItem>();
            foreach (var item in RouteOutlets)
            {
                workingList.Add(item);
            }
            RouteOutlets.Clear();
            var selectedItem = workingList.FirstOrDefault(n => n.Priority == selectedItemIndex);

            if (moveUp)
            {
                foreach (var item in workingList.Where(n => n.Priority < selectedItem.Priority && n.Priority >= OutletPriorityToSet))
                {
                    item.Priority ++;
                }
            }
            else
            {
                foreach (var item in workingList.Where(n => n.Priority > selectedItem.Priority && n.Priority <= OutletPriorityToSet))
                {
                        item.Priority--;
                }
            }

            selectedItem.Priority = OutletPriorityToSet;

            foreach (var item in workingList.OrderBy(n => n.Priority))
            {
                RouteOutlets.Add(item);
            }

            SelectedRouteOutlet = RouteOutlets.First(n => n.Priority == OutletPriorityToSet);
        }

        void MoveItemUpOrDown(bool moveUp)
        {
            if (SelectedRouteOutlet == null || SelectedRouteOutlet.Id == Guid.Empty)
                return;

            int selectedItemIndex = SelectedRouteOutlet.Priority;

            if (moveUp)
            {
                if (RouteOutlets.Min(n => n.Priority) == selectedItemIndex)
                    return;
            }
            else
            {
                if (RouteOutlets.Max(n => n.Priority) == selectedItemIndex)
                    return;
            }

            IsDirty = true;
            var workingList = new ObservableCollection<ListOutletItem>();
            foreach (var item in RouteOutlets)
            {
                workingList.Add(item);
            }
            RouteOutlets.Clear();
            var selectedItem = workingList.FirstOrDefault(n => n.Priority == selectedItemIndex);
            int destinationRowNum = 0;
            if (moveUp)
                destinationRowNum = selectedItem.Priority - 1;
            else
                destinationRowNum = selectedItem.Priority + 1;

            var itemReplaced = workingList.FirstOrDefault(n => n.Priority == destinationRowNum);
            selectedItem.Priority = destinationRowNum;
            itemReplaced.Priority = selectedItemIndex;

            foreach (var item in workingList.OrderBy(n => n.Priority))
            {
                RouteOutlets.Add(item);
            }

            SelectedRouteOutlet = RouteOutlets.First(n => n.Priority == destinationRowNum);
        }

        void MoveTopOrBottom(bool moveTop)
        {
            if (SelectedRouteOutlet == null || SelectedRouteOutlet.Id == Guid.Empty)
                return;

            int selectedItemIndex = SelectedRouteOutlet.Priority;

            if (moveTop)
            {
                if (RouteOutlets.Min(n => n.Priority) == selectedItemIndex)
                    return;
            }
            else
            {
                if (RouteOutlets.Max(n => n.Priority) == selectedItemIndex)
                    return;
            }

            IsDirty = true;
            var workingList = new ObservableCollection<ListOutletItem>();
            foreach (var item in RouteOutlets)
            {
                workingList.Add(item);
            }
            RouteOutlets.Clear();
            var selectedItem = workingList.FirstOrDefault(n => n.Priority == selectedItemIndex);
            int destinationPriority = 0;
            if (moveTop)
                destinationPriority = workingList.Min(n => n.Priority);
            else
                destinationPriority = workingList.Max(n => n.Priority);

            if (moveTop)
            {
                foreach (var item in workingList.Where(n => n.Priority < selectedItem.Priority))
                {
                    item.Priority += 1;
                }
                selectedItem.Priority = workingList.Min(n => n.Priority);
            }
            else
            {
                foreach (var item in workingList.Where(n => n.Priority > selectedItem.Priority))
                {
                    item.Priority -= 1;
                }
                selectedItem.Priority = workingList.Max(n => n.Priority);
            }

            selectedItem.Priority = destinationPriority;

            foreach (var item in workingList.OrderBy(n => n.Priority))
            {
                RouteOutlets.Add(item);
            }

            SelectedRouteOutlet = RouteOutlets.First(n => n.Priority == destinationPriority);
        }

        public void ClearViewModel()
        {
            Routes.Clear();
            RouteOutlets.Clear();
            outletPriorities.Clear();
            SelectedRouteOutlet = null;
            SelectedRoute = null;
            IsDirty = false;
        }

        #endregion
    }
}