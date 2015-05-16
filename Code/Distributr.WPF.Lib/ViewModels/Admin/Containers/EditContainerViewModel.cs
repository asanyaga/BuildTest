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
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.EquipmentEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.EquipmentRepository;
using Distributr.Core.Workflow.Impl.AuditLogs;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using Distributr.WPF.Lib.ViewModels.Utils;
using GalaSoft.MvvmLight.Command;

namespace Distributr.WPF.Lib.ViewModels.Admin.Containers
{
    public class EditContainerViewModel : DistributrViewModelBase
    {
        public EditContainerViewModel()
        {
            ContainerTypesList = new ObservableCollection<ContainerType>();
            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
        }

        #region properties

        public ObservableCollection<ContainerType> ContainerTypesList { get; set; }

        public RelayCommand SaveCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }

        public const string ContainerPropertyName = "Container";
        private SourcingContainer _container = null;

        public SourcingContainer Container
        {
            get { return _container; }

            set
            {
                if (_container == value)
                {
                    return;
                }

                RaisePropertyChanging(ContainerPropertyName);
                _container = value;
                RaisePropertyChanged(ContainerPropertyName);
            }
        }

        public const string SelectedContainerTypePropertyName = "SelectedContainerType";
        private ContainerType _selectedContainerType = null;
        [MasterDataDropDownValidation]
        public ContainerType SelectedContainerType
        {
            get { return _selectedContainerType; }

            set
            {
                if (_selectedContainerType == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedContainerTypePropertyName);
                _selectedContainerType = value;
                RaisePropertyChanged(SelectedContainerTypePropertyName);
            }
        }

        public const string PageTitlePropertyName = "PageTitle";
        private string _pageTitle = "Create Container";

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

        public const string CanEditPropertyName = "CanEdit";
        private bool _canEdit = false;

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

        public const string IsPrimaryContactPropertyName = "IsPrimaryContact";
        private bool _isPrimaryContact;
        public bool IsPrimaryContact
        {
            get
            {
                return _isPrimaryContact;
            }

            set
            {
                if (_isPrimaryContact == value)
                {
                    return;
                }

                _isPrimaryContact = value;
                RaisePropertyChanged(IsPrimaryContactPropertyName);
            }
        }

        public const string IsSecondaryContactPropertyName = "IsSecondaryContact";
        private bool _isSecondaryContact;
        public bool IsSecondaryContact
        {
            get
            {
                return _isSecondaryContact;
            }

            set
            {
                if (_isSecondaryContact == value)
                {
                    return;
                }

                _isSecondaryContact = value;
                RaisePropertyChanged(IsSecondaryContactPropertyName);
            }
        }

        #endregion

        #region methods

        private ContainerType _defaultContainerType;
        private ContainerType DefaultContainerType
        {
            get { return _defaultContainerType ?? (_defaultContainerType = new ContainerType(Guid.Empty){Name="--Select container type--"}); }
        }

        protected override void LoadPage(Page page)
        {
            Guid id = PresentationUtility.ParseIdFromUrl(page.NavigationService.CurrentSource);
            using (var c = NestedContainer)
            {
                if (id == Guid.Empty)
                {
                    PageTitle = "Create New Container";
                    Container = new SourcingContainer(Guid.NewGuid());
                    Container.EquipmentType = EquipmentType.Container;
                    Container.CostCentre =
                        Using<ICostCentreRepository>(c).GetById(GetConfigParams().CostCentreId) as Hub;
                }
                else
                {
                    PageTitle = "Edit Container";
                    var cont = Using<IEquipmentRepository>(c).GetById(id) as SourcingContainer;
                    Container = cont.DeepClone();
                }
            }
            Setup();
            if (Container._Status != EntityStatus.New)
            {
                SelectedContainerType =
                    ContainerTypesList.FirstOrDefault(n => n.Id == ((SourcingContainer) Container).ContainerType.Id);
            }
        }

        private void Setup()
        {
            LoadContainerTypesList();
        }

        private void LoadContainerTypesList()
        {
            ContainerTypesList.Clear();
            using (var c = NestedContainer)
            {
                ContainerTypesList.Add(DefaultContainerType);
                SelectedContainerType = DefaultContainerType;
                Using<IContainerTypeRepository>(c).GetAll().OrderBy(n => n.Name).ToList().ForEach(ContainerTypesList.Add);
            }
        }

        private async void Save()
        {
            Container.ContainerType = SelectedContainerType;
            using (var c = NestedContainer)
            {
                if (!IsValid()) return;
                IDistributorServiceProxy proxy = Using<IDistributorServiceProxy>(c);
                ResponseBool response = null;
                response = await proxy.ContainerAddAsync(Container);
                string log = string.Format("Created container: {0}; Code: {1}; of type {2}",
                                           Container.Name,
                                           Container.Code, Container.ContainerType.Name)
                    ;
                Using<IAuditLogWFManager>(c).AuditLogEntry("Container Management", log);

                MessageBox.Show(response.ErrorInfo, "Distributr: Add/ Edit Container", MessageBoxButton.OK,
                                MessageBoxImage.Information);

                if (response.Success)
                {
                    SendNavigationRequestMessage(
                        new Uri("views/admin/containers/listcontainers.xaml", UriKind.Relative));
                }
            }
        }

        private void Cancel()
        {
            if (
                MessageBox.Show("Unsaved changes will be lost. Do you want to continue?",
                                "Agrimanagr: Edit container details", MessageBoxButton.YesNo,
                                MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
            {
                SendNavigationRequestMessage(new Uri("views/admin/containers/listcontainers.xaml",
                                                     UriKind.Relative));
            }
        }

        #endregion

    }
}
