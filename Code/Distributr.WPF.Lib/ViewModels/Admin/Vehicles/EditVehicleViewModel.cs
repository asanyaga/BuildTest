using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.EquipmentEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.EquipmentRepository;
using Distributr.Core.Workflow.Impl.AuditLogs;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using Distributr.WPF.Lib.ViewModels.Utils;
using GalaSoft.MvvmLight.Command;

namespace Distributr.WPF.Lib.ViewModels.Admin.Vehicles
{
    public class EditVehicleViewModel : DistributrViewModelBase
    { 
        private IDistributorServiceProxy _proxy;

        #region properties

        private RelayCommand _saveCommand;
        public RelayCommand SaveCommand { get { return _saveCommand ?? (_saveCommand = new RelayCommand(Save)); } }
        private RelayCommand _cancelCommand;
        public RelayCommand CancelCommand { get { return _cancelCommand ?? (_cancelCommand = new RelayCommand(Cancel)); } }

        public const string VehiclePropertyName = "Vehicle";
        private Vehicle _vehicle = null;

        public Vehicle Vehicle
        {
            get { return _vehicle; }

            set
            {
                if (_vehicle == value)
                {
                    return;
                }

                RaisePropertyChanging(VehiclePropertyName);
                _vehicle = value;
                RaisePropertyChanged(VehiclePropertyName);
            }
        }

        public const string PageTitlePropertyName = "PageTitle";
        private string _pageTitle = "Edit Vehicle";

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

        #endregion


        #region methods

        protected override void LoadPage(Page page)
        {
            Guid storeId = PresentationUtility.ParseIdFromUrl(page.NavigationService.CurrentSource);
            using (var c = NestedContainer)
            {
                if (storeId == Guid.Empty)
                {
                    PageTitle = "Create Vehicle";
                    Vehicle = new Vehicle(Guid.NewGuid());
                    Vehicle.CostCentre = Using<ICostCentreRepository>(c).GetById(GetConfigParams().CostCentreId) as Hub;
                    Vehicle.EquipmentType = EquipmentType.Vehicle;
                }
                else
                {
                    var store = Using<IVehicleRepository>(c).GetById(storeId) as Vehicle;
                    Vehicle = store.DeepClone<Vehicle>();
                    PageTitle = "Edit Vehicle";
                }
            }
        }

        public async void Save()
        {
            if (!IsValid())
                return;
            using (var c = NestedContainer)
            {
                _proxy = Using<IDistributorServiceProxy>(c);
                ResponseBool response = null;
                response = await _proxy.VehicleAddAsync(Vehicle);

                string log = string.Format("Created vehicle: {0}; Code: {1};", Vehicle.Name,
                                           Vehicle.Code);
                Using<IAuditLogWFManager>(c).AuditLogEntry("Vehicle Management", log);

                MessageBox.Show(response.ErrorInfo, "Agrimanagr: Manage Vehicles", MessageBoxButton.OK,
                                MessageBoxImage.Information);

                if (response.Success)
                {
                    SendNavigationRequestMessage(
                        new Uri("views/admin/vehicles/listvehicles.xaml", UriKind.Relative));
                }
            }
        }

        private void Cancel()
        {
            if (
                MessageBox.Show("Unsaved changes will be lost. Do you want to continue?",
                                "Agrimanagr: Edit Vehicle", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) ==
                MessageBoxResult.Yes)
            {
                SendNavigationRequestMessage(
                    new Uri("views/admin/vehicles/listvehicles.xaml", UriKind.Relative));
            }
        }

        #endregion
    }
}
