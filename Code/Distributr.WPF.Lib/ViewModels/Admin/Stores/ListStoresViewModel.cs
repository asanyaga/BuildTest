using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.Util;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.WPF.Lib.Services.Service.WSProxies;

namespace Distributr.WPF.Lib.ViewModels.Admin.Stores
{
    public class ListStoresViewModel : ListingsViewModelBase
    {
        private PagenatedList<Store> _pagedList;
        private IDistributorServiceProxy _proxy;

        public ListStoresViewModel()
        {
            StoresList = new ObservableCollection<VMStoreItem>();
        }

        #region properties

        public ObservableCollection<VMStoreItem> StoresList { get; set; }

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

        public const string SelectedStorePropertyName = "SelectedStore";
        private VMStoreItem _selectedStore = null;

        public VMStoreItem SelectedStore
        {
            get { return _selectedStore; }

            set
            {
                if (_selectedStore == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedStorePropertyName);
                _selectedStore = value;
                RaisePropertyChanged(SelectedStorePropertyName);
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
                              var rawList = Using<IStoreRepository>(c).GetAll(ShowInactive).OfType<Store>()
                                   .Where(n => n.CostCentreCode.ToLower().Contains(SearchText.ToLower()) ||
                                                n.Name.ToLower().Contains(SearchText.ToLower()));
                                rawList = rawList.OrderBy(n => n.Name).ThenBy(n => n.CostCentreCode);
                                _pagedList = new PagenatedList<Store>(rawList.AsQueryable(),
                                                                      CurrentPage,
                                                                      ItemsPerPage,
                                                                      rawList.Count());

                                StoresList.Clear();
                                _pagedList.Select((n, i) => Map(n, i + 1)).ToList().ForEach(
                                    n => StoresList.Add(n));
                                UpdatePagenationControl();
                            }
                        }));
        }

        private void Setup()
        {
            PageTitle = "List Hub Stores";
            CanManage = true;
        }

        private VMStoreItem Map(Store store, int i)
        {
            VMStoreItem mapped = new VMStoreItem()
                                     {
                                         Store = store,
                                         RowNumber = i,
                                     };
            if (store._Status == EntityStatus.Active)
                mapped.HlkDeactivateContent = "Deactivate";
            else if (store._Status == EntityStatus.Inactive)
                mapped.HlkDeactivateContent = "Activate";

            return mapped;
        }

        protected override void EditSelected()
        {
            if (SelectedStore != null)
                SendNavigationRequestMessage(
                    new Uri("/views/admin/stores/editstore.xaml?" + SelectedStore.Store.Id, UriKind.Relative));
        }

        protected override async void ActivateSelected()
        {
            string action = SelectedStore.Store._Status == EntityStatus.Active
                              ? "deactivate"
                              : "activate";
            if (
                    MessageBox.Show("Are you sure you want to " + action + " this store?",
                                    "Agrimanagr: Activate Store", MessageBoxButton.OKCancel) ==
                    MessageBoxResult.Cancel) return;
            using (var c = NestedContainer)
            {
                if (SelectedStore.Store._Status == EntityStatus.Active)
                {
                    if (Using<IMasterDataUsage>(c).CheckStoreIsUsed(SelectedStore.Store, EntityStatus.Inactive))
                    {
                        MessageBox.Show(
                            "Store " + SelectedStore.Store.Name +
                            " has been used in a transaction. Deactivate or delete dependencies to continue.",
                            "Agrimanagr: Deactivate Store", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }
                }

                ResponseBool response = new ResponseBool() {Success = false};
                if (SelectedStore == null) return;
                _proxy = Using<IDistributorServiceProxy>(c);
                response = await _proxy.StoreActivateOrDeactivateAsync(SelectedStore.Store.Id);

                MessageBox.Show(response.ErrorInfo, "Agrimangr: Manage Store", MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
        }

        protected override async void DeleteSelected()
        {
            if (
                    MessageBox.Show("Are you sure you want to delete this store?",
                                    "Agrimanagr: Delete Store", MessageBoxButton.OKCancel) ==
                    MessageBoxResult.Cancel) return;
            using (var c = NestedContainer)
            {
                if (Using<IMasterDataUsage>(c).CheckStoreIsUsed(SelectedStore.Store, EntityStatus.Deleted))
                {
                    MessageBox.Show(
                        "Store " + SelectedStore.Store.Name +
                        " has been used in a transaction. Deactivate or delete dependencies to continue.",
                        "Agrimanagr: Delete Store", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                ResponseBool response = new ResponseBool() {Success = false};
                if (SelectedStore == null) return;
                _proxy = Using<IDistributorServiceProxy>(c);
                response = await _proxy.StoreDeleteAsync(SelectedStore.Store.Id);
                if (response.Success)
                    Using<IStoreRepository>(c).SetAsDeleted(SelectedStore.Store);
                MessageBox.Show(response.ErrorInfo, "Agrimangr: Manage Store", MessageBoxButton.OK,
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

    public class VMStoreItem
    {
        public Store Store { get; set; }
        public int RowNumber { get; set; }
        public string HlkDeactivateContent { get; set; }
    }

    #endregion

}
