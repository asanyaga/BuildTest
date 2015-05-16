using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.EquipmentEntities;
using Distributr.Core.Domain.Master.Util;
using Distributr.Core.Repository.Master.EquipmentRepository;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.WPF.Lib.Services.Service.WSProxies;

namespace Distributr.WPF.Lib.ViewModels.Admin.Vehicles
{
    public class ListVehiclesViewModel : ListingsViewModelBase
    {
        public ListVehiclesViewModel()
        {
            VehiclesList = new ObservableCollection<VMVehicleItem>();
        }

        #region properties

        public ObservableCollection<VMVehicleItem> VehiclesList { get; set; }

        public const string CanEditPropertyName = "CanEdit";
        private bool _canEdit = false;
        private PagenatedList<Vehicle> _pagedList;

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

        public const string SelectedVehiclePropertyName = "SelectedVehicle";
        private VMVehicleItem _selectedVehicle = null;
        private IDistributorServiceProxy _proxy;

        public VMVehicleItem SelectedVehicle
        {
            get { return _selectedVehicle; }

            set
            {
                if (_selectedVehicle == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedVehiclePropertyName);
                _selectedVehicle = value;
                RaisePropertyChanged(SelectedVehiclePropertyName);
            }
        }

        #endregion


        #region methods

        protected override void Load(bool isFirstLoad = false)
        {
            Application.Current.Dispatcher.BeginInvoke(
                new Action(
                    delegate
                        {
                            if (isFirstLoad)
                                Setup();
                            using (var c = NestedContainer)
                            {
                                var rawList = Using<IVehicleRepository>(c).GetAll(ShowInactive).OfType<Vehicle>()
                                    .Where(n => n.Code.ToLower().Contains(SearchText.ToLower()) ||
                                                n.Name.ToLower().Contains(SearchText.ToLower()) ||
                                                n.Make.ToLower().Contains(SearchText.ToLower()) ||
                                                n.Model.ToLower().Contains(SearchText.ToLower()) ||
                                                n.Description.ToLower().Contains(SearchText.ToLower())
                                    );
                                rawList = rawList.OrderBy(n => n.Name).ThenBy(n => n.Code);
                                _pagedList = new PagenatedList<Vehicle>(rawList.AsQueryable(),
                                                                        CurrentPage,
                                                                        ItemsPerPage,
                                                                        rawList.Count());

                                VehiclesList.Clear();
                                _pagedList.Select((n, i) => Map(n, i + 1)).ToList().ForEach(
                                    n => VehiclesList.Add(n));
                                UpdatePagenationControl();
                            }
                        }));
        }

        private VMVehicleItem Map(Vehicle vehicle, int i)
        {
            VMVehicleItem mapped = new VMVehicleItem
                                       {
                                           Vehicle = vehicle,
                                           RowNumber = i,
                                       };
            if (vehicle._Status == EntityStatus.Active)
                mapped.HlkDeactivateContent = "Deactivate";
            else if (vehicle._Status == EntityStatus.Inactive)
                mapped.HlkDeactivateContent = "Activate";

            return mapped;
        }


        private void Setup()
        {
            CanEdit = true;
        }

        protected override void EditSelected()
        {
            if (SelectedVehicle != null)
                SendNavigationRequestMessage(
                    new Uri("/views/admin/vehicles/editvehicle.xaml?" + SelectedVehicle.Vehicle.Id, UriKind.Relative));
        }

        protected override async void ActivateSelected()
        {
            string action = SelectedVehicle.Vehicle._Status == EntityStatus.Active
                                ? "deactivate"
                                : "activate";
            if (
                MessageBox.Show("Are you sure you want to " + action + " this vehicle?",
                                "Agrimanagr: Activate Vehicle", MessageBoxButton.OKCancel) ==
                MessageBoxResult.Cancel) return;
            using (var c = NestedContainer)
            {
                if (Using<IMasterDataUsage>(c).CheckVehicleIsUsed(SelectedVehicle.Vehicle, EntityStatus.Inactive))
                {
                    MessageBox.Show(
                        "Vehicle " + SelectedVehicle.Vehicle.Name +
                        " has been used in a transaction. Deactivate or delete dependencies to continue.",
                        "Agrimanagr: Deactivate Vehicle", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                ResponseBool response = new ResponseBool() {Success = false};
                if (SelectedVehicle == null) return;
                _proxy = Using<IDistributorServiceProxy>(c);
                response = await _proxy.VehicleActivateOrDeactivateAsync(SelectedVehicle.Vehicle.Id);

                MessageBox.Show(response.ErrorInfo, "Agrimangr: Manage Vehicle", MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
        }

        protected override async void DeleteSelected()
        {
            if (
                MessageBox.Show("Are you sure you want to delete this vehicle?",
                                "Agrimanagr: Delete Vehicle", MessageBoxButton.OKCancel) ==
                MessageBoxResult.Cancel) return;
            using (var c = NestedContainer)
            {
                if (Using<IMasterDataUsage>(c).CheckVehicleIsUsed(SelectedVehicle.Vehicle, EntityStatus.Deleted))
                {
                    MessageBox.Show(
                        "Vehicle " + SelectedVehicle.Vehicle.Name +
                        " has been used in a transaction. Delete dependencies to continue.",
                        "Agrimanagr: Delete Vehicle", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                ResponseBool response = new ResponseBool() {Success = false};
                if (SelectedVehicle == null) return;
                _proxy = Using<IDistributorServiceProxy>(c);
                response = await _proxy.VehicleDeleteAsync(SelectedVehicle.Vehicle.Id);
                if (response.Success)
                    Using<IVehicleRepository>(c).SetAsDeleted(SelectedVehicle.Vehicle);
                MessageBox.Show(response.ErrorInfo, "Agrimangr: Manage Vehicles", MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
        }

        protected override void GoToPage(PageDestinations page)
        {
            GoToPageBase(page, _pagedList.PageCount);
        }

        protected override void ComboPageSizesSelectionChanged(int take)
        {
            ItemsPerPage = take;
            Load();
        }

        protected override void UpdatePagenationControl()
        {
            UpdatePagenationControlBase(_pagedList.PageNumber, _pagedList.PageCount, _pagedList.TotalItemCount,
                                        _pagedList.IsFirstPage, _pagedList.IsLastPage);
        }

        #endregion

    }

    #region helpers

    public class VMVehicleItem
    {
        public Vehicle Vehicle { get; set; }
        public int RowNumber { get; set; }
        public string HlkDeactivateContent { get; set; }
    }

    #endregion
}
