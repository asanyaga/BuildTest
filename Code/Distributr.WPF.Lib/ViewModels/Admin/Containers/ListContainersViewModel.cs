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

namespace Distributr.WPF.Lib.ViewModels.Admin.Containers
{
    public class ListContainersViewModel : ListingsViewModelBase
    {
        private PagenatedList<SourcingContainer> _pagedList;
        private IDistributorServiceProxy _proxy;

        public ListContainersViewModel()
        {
            ContainersList = new ObservableCollection<VMContainerItem>();
            ContainerTypesList = new ObservableCollection<ContainerType>();
        }

        #region properties

        public ObservableCollection<VMContainerItem> ContainersList { get; set; }
        public ObservableCollection<ContainerType> ContainerTypesList { get; set; }

        public const string SelectedContainerPropertyName = "SelectedContainer";
        private VMContainerItem _selectedContainer = null;

        public VMContainerItem SelectedContainer
        {
            get { return _selectedContainer; }

            set
            {
                if (_selectedContainer == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedContainerPropertyName);
                _selectedContainer = value;
                RaisePropertyChanged(SelectedContainerPropertyName);
            }
        }

        public const string SelectedContainerTypePropertyName = "SelectedContainerType";
        private ContainerType _selectedContainerType = null;

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

        public const string CanManagePropertyName = "CanManage";
        private bool _canManage = false;

        public bool CanManage
        {
            get { return _canManage; }

            set
            {
                if (_canManage == value)
                {
                    return;
                }

                RaisePropertyChanging(CanManagePropertyName);
                _canManage = value;
                RaisePropertyChanged(CanManagePropertyName);
            }
        }

        private ContainerType _defaultContainerType;

        private ContainerType DefaultContainerType
        {
            get
            {
                return _defaultContainerType ??
                       (_defaultContainerType = new ContainerType(Guid.Empty) {Name = "--Select container type--"});
            }
        }

        #endregion


        #region methods

        protected override void Load(bool isFirstLoad = false)
        {
            if (isFirstLoad)
                Setup();
            LoadContainersList();
        }

        void LoadContainersList()
        {
            Application.Current.Dispatcher.BeginInvoke(
                new Action(
                    delegate
                        {
                            using (var c = NestedContainer)
                            {
                                var list = Using<IEquipmentRepository>(c).GetAll(ShowInactive).OfType<SourcingContainer>()
                                    .Where(n =>
                                           (n.Name.ToLower().Contains(SearchText.ToLower()) ||
                                            n.Code.ToLower().Contains(SearchText.ToLower())));

                                if (SelectedContainerType != null && SelectedContainerType.Id != Guid.Empty)
                                    list = list.Where(n => n.ContainerType.Id == SelectedContainerType.Id);
                                list = list.OrderBy(n => n.Name).ThenBy(n => n.Code);

                                _pagedList = new PagenatedList<SourcingContainer>(list.AsQueryable(), CurrentPage,
                                                                                  ItemsPerPage,
                                                                                  list.Count());
                                ContainersList.Clear();
                                _pagedList.Select(Map).ToList().ForEach(ContainersList.Add);
                                UpdatePagenationControl();
                            }
                        }));
        }

        private VMContainerItem Map(SourcingContainer container, int index)
        {
            var mapped = new VMContainerItem {Container = container, RowNumber = index};
            if (container._Status == EntityStatus.Active)
                mapped.HlkDeactivateContent = "Deactivate";
            if (container._Status == EntityStatus.Inactive)
                mapped.HlkDeactivateContent = "Activate";

            return mapped;
        }

        private void Setup()
        {
            CanManage = true;
            LoadContainerTypesList();
            PageTitle = "List Containers";
        }

        private void LoadContainerTypesList()
        {
            using (var c = NestedContainer)
            {
                ContainerTypesList.Clear();
                ContainerTypesList.Add(DefaultContainerType);
                SelectedContainerType = DefaultContainerType;
                Using<IContainerTypeRepository>(c).GetAll().OrderBy(n => n.Name).ToList().ForEach(ContainerTypesList.Add);
            }
        }

        protected override void EditSelected()
        {
            if (SelectedContainer != null)
                SendNavigationRequestMessage(
                    new Uri("/views/admin/containers/editcontainer.xaml?" + SelectedContainer.Container.Id,
                            UriKind.Relative));
        }

        protected override async void ActivateSelected()
        {
            using (var c = NestedContainer)
            {
                if (SelectedContainer.Container._Status == EntityStatus.Active)
                {

                    if (Using<IMasterDataUsage>(c).CheckSourcingContainerIsUsed(SelectedContainer.Container))
                    {
                        MessageBox.Show(
                            "Container " + SelectedContainer.Container.Name +
                            " has dependencies. Deactivate or delete dependencies to continue.",
                            "Agrimanagr: Deactivate Container", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }
                }

                string action = SelectedContainer.Container._Status == EntityStatus.Active
                             ? "deactivate"
                             : "activate";
                if (
                        MessageBox.Show("Are you sure you want to " + action + " this container?",
                                        "Agrimanagr: Activate Container", MessageBoxButton.OKCancel) ==
                        MessageBoxResult.Cancel) return;

                ResponseBool response = new ResponseBool() {Success = false};
                if (SelectedContainer == null) return;
                _proxy = Using<IDistributorServiceProxy>(c);
                response = await _proxy.ContainerActivateOrDeactivateAsync(SelectedContainer.Container.Id);

                MessageBox.Show(response.ErrorInfo, "Agrimangr: Manage Containers", MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
        }

        protected override async void DeleteSelected()
        {
            if (
                    MessageBox.Show("Are you sure you want to delete this container?",
                                    "Agrimanagr: Delete Container", MessageBoxButton.OKCancel) ==
                    MessageBoxResult.Cancel) return;
            using (var c = NestedContainer)
            {
                if (SelectedContainer.Container._Status == EntityStatus.Active)
                {

                    if (Using<IMasterDataUsage>(c).CheckSourcingContainerIsUsed(SelectedContainer.Container))
                    {
                        MessageBox.Show(
                            "Container " + SelectedContainer.Container.Name +
                            " has dependencies. Deactivate or delete dependencies to continue.",
                            "Agrimanagr: Deactivate Container", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }
                }

                ResponseBool response = new ResponseBool() {Success = false};
                if (SelectedContainer == null) return;
                _proxy = Using<IDistributorServiceProxy>(c);
                response = await _proxy.ContainerDeleteAsync(SelectedContainer.Container.Id);
                if (response.Success)
                    Using<IEquipmentRepository>(c).SetAsDeleted(SelectedContainer.Container);
                MessageBox.Show(response.ErrorInfo, "Agrimangr: Manage Containers", MessageBoxButton.OK,
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

    public class VMContainerItem
    {
        public SourcingContainer Container { get; set; }
        public int RowNumber { get; set; }
        public string HlkDeactivateContent { get; set; }
    }

    #endregion

}
