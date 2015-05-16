using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CentreEntity;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master.CentreRepositories;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Workflow.Impl.AuditLogs;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using Distributr.WPF.Lib.ViewModels.Admin.AgriUsers;
using Distributr.WPF.Lib.ViewModels.Utils;
using GalaSoft.MvvmLight.Command;

namespace Distributr.WPF.Lib.ViewModels.Admin.Centres
{
    public class EditCentreViewModel : DistributrViewModelBase
    {
        private IDistributorServiceProxy _proxy;

        public EditCentreViewModel()
        {
            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(CancelAll);

            RoutesList = new ObservableCollection<Route>();
            CentreTypesList = new ObservableCollection<CentreType>();
        }

        #region properties

        public ObservableCollection<Route> RoutesList { get; set; }
        public ObservableCollection<CentreType> CentreTypesList { get; set; }

        public RelayCommand SaveCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        public RelayCommand AssignSelectedCommand { get; set; }
        public RelayCommand AssignAllCommand { get; set; }
        public RelayCommand UnassignSelectedCommand { get; set; }
        public RelayCommand UnassignAllCommand { get; set; }

        public const string CentrePropertyName = "Centre";
        private Centre _centre = null;

        public Centre Centre
        {
            get { return _centre; }

            set
            {
                if (_centre == value)
                {
                    return;
                }

                RaisePropertyChanging(CentrePropertyName);
                _centre = value;
                RaisePropertyChanged(CentrePropertyName);
            }
        }

        public const string PageTitlePropertyName = "PageTitle";
        private string _pageTitle = "Edit Centre";
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

        public const string SelectedCentreTypePropertyName = "SelectedCentreType";
        private CentreType _selectedCentreType = null;
        [MasterDataDropDownValidation]
        public CentreType SelectedCentreType
        {
            get { return _selectedCentreType; }

            set
            {
                if (_selectedCentreType == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedCentreTypePropertyName);
                _selectedCentreType = value;
                RaisePropertyChanged(SelectedCentreTypePropertyName);
            }
        }

        public const string SelectedRoutePropertyName = "SelectedRoute";
        private Route _selectedRoute = null;
        [MasterDataDropDownValidation]
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

                RaisePropertyChanging(SelectedRoutePropertyName);
                _selectedRoute = value;
                RaisePropertyChanged(SelectedRoutePropertyName);
            }
        }

        private CentreType _defaultCentreType;

        private CentreType DefaultCentreType
        {
            get
            {
                return _defaultCentreType ??
                       (_defaultCentreType = new CentreType(Guid.Empty) {Name = "--Select centre type--"});
            }
        }

        private Route _defaultRoute;

        private Route DefaultRoute
        {
            get { return _defaultRoute ?? (_defaultRoute = new Route(Guid.Empty) {Name = "--Select route--"}); }
        }

        #endregion

        #region methods

        protected override void LoadPage(Page page)
        {
            Guid centreId = PresentationUtility.ParseIdFromUrl(page.NavigationService.CurrentSource);
            using (var c = NestedContainer)
            {
                if (centreId == Guid.Empty)
                {
                    Centre = new Centre(Guid.NewGuid());
                    Centre.Hub = Using<ICostCentreRepository>(c).GetById(GetConfigParams().CostCentreId) as Hub;
                    PageTitle = "Create Buying/Collection Centre";
                }
                else
                {
                    var centre = Using<ICentreRepository>(c).GetById(centreId);
                    Centre = centre.Clone<Centre>();
                    PageTitle = "Edit Buying/Collection Centre";
                }
                Setup();
                if (Centre._Status != EntityStatus.New)
                {
                    SelectedCentreType = CentreTypesList.FirstOrDefault(n => n.Id == Centre.CenterType.Id);
                    SelectedRoute = RoutesList.FirstOrDefault(n => n != null && n.Id == Centre.Route.Id);
                }
            }
        }

        private void Setup()
        {
            LoadCentreTypesList();
            LoadRoutesLists();
        }

        private void LoadRoutesLists()
        {
            using (var c = NestedContainer)
            {
                RoutesList.Clear();
                RoutesList.Add(DefaultRoute);
                SelectedRoute = DefaultRoute;
                Using<IRouteRepository>(c).GetAll().OrderBy(n => n.Name)
                    .ToList().ForEach(n => RoutesList.Add(n));
            }
        }

        private void LoadCentreTypesList()
        {
            using (var c = NestedContainer)
            {
                CentreTypesList.Clear();
                CentreTypesList.Add(DefaultCentreType);
                SelectedCentreType = DefaultCentreType;
                Using<ICentreTypeRepository>(c).GetAll().OrderBy(n => n.Name)
                    .ToList().ForEach(CentreTypesList.Add);
            }
        }

        private async void Save()
        {
            Centre.CenterType = SelectedCentreType;
            Centre.Route = SelectedRoute;
            if (!IsValid())
                return;
            using (var c = NestedContainer)
            {
                _proxy = Using<IDistributorServiceProxy>(c);
                ResponseBool response = null;

                response = await _proxy.CentreAddAsync(Centre);
                string log = string.Format("Created new centre: {0}; Code: {1}; And centre Type {2}", Centre.Name,
                                           Centre.Code,
                                           SelectedCentreType.Name);
                Using<IAuditLogWFManager>(c).AuditLogEntry("User Administration", log);

                MessageBox.Show(response.ErrorInfo, "Agrimanagr: Manage Centres", MessageBoxButton.OK,
                                MessageBoxImage.Information);
                if (response.Success)
                {
                    SendNavigationRequestMessage(new Uri("views/admin/centres/listcentres.xaml", UriKind.Relative));
                }
            }
        }

        private void CancelAll()
        {
            if (
                MessageBox.Show("Unsaved changes will be lost. Do you want to continue?", "Agrimanagr: Edit Centre",
                                MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
            {
                SendNavigationRequestMessage(new Uri("views/admin/centres/listcentres.xaml",
                                                     UriKind.Relative));
            }
        }

        #endregion

    }
}
