using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CentreEntity;
using Distributr.Core.Domain.Master.CommodityEntity;
using Distributr.Core.Domain.Master.Util;
using Distributr.Core.Repository.Master.CentreRepositories;
using Distributr.Core.Repository.Master.CommodityOwnerRepository;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.WPF.Lib.Services.Service.WSProxies;

namespace Distributr.WPF.Lib.ViewModels.Admin.Centres
{
    public class ListCentresViewModel : ListingsViewModelBase
    {
        private PagenatedList<Centre> _pagedList;
        private IDistributorServiceProxy _proxy;

        public ListCentresViewModel()
        {
            CentresList = new ObservableCollection<VMCentreItem>();
        }

        #region properties

        public ObservableCollection<VMCentreItem> CentresList { get; set; }

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

        public const string SelectedCentreItemPropertyName = "SelectedCentreItem";
        private VMCentreItem _selectedCentreItem = null;

        public VMCentreItem SelectedCentreItem
        {
            get { return _selectedCentreItem; }

            set
            {
                if (_selectedCentreItem == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedCentreItemPropertyName);
                _selectedCentreItem = value;
                RaisePropertyChanged(SelectedCentreItemPropertyName);
            }
        }

        #endregion


        #region mathods

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
                                var rawList = Using<ICentreRepository>(c).GetAll(ShowInactive)
                                    .Where(n => n.Code.ToLower().Contains(SearchText.ToLower()) ||
                                                n.Name.ToLower().Contains(SearchText.ToLower()));
                                rawList = rawList.OrderBy(n => n.Name).ThenBy(n => n.Code);
                                _pagedList = new PagenatedList<Centre>(rawList.AsQueryable(),
                                                                       CurrentPage,
                                                                       ItemsPerPage,
                                                                       rawList.Count());

                                CentresList.Clear();
                                _pagedList.Select((n, i) => Map(n, i + 1)).ToList().ForEach(
                                    n => CentresList.Add(n));
                                UpdatePagenationControl();
                            }
                        }));
        }

        private void Setup()
        {
            PageTitle = "List Hub Centres";
            CanManage = true;
        }

        private VMCentreItem Map(Centre centre, int i)
        {
            VMCentreItem mapped = new VMCentreItem()
                                      {
                                          Centre = centre,
                                          RowNumber = i
                                      };
            if (centre._Status == EntityStatus.Active)
                mapped.HlkDeactivateContent = "Deactivate";
            else if (centre._Status == EntityStatus.Inactive)
                mapped.HlkDeactivateContent = "Activate";

            return mapped;
        }

        protected override void EditSelected()
        {
            if (SelectedCentreItem != null)
                SendNavigationRequestMessage(
                    new Uri("/views/admin/centres/editcentre.xaml?" + SelectedCentreItem.Centre.Id,UriKind.Relative));
        }

        protected override async void ActivateSelected()
        {
            string action = SelectedCentreItem.Centre._Status == EntityStatus.Active
                                  ? "deactivate"
                                  : "activate";
            if (
                    MessageBox.Show("Are you sure you want to " + action + " this centre",
                                    "Agrimanagr: Activate Centre", MessageBoxButton.OKCancel) ==
                    MessageBoxResult.Cancel) return;
            using (var c = NestedContainer)
            {
                if(SelectedCentreItem.Centre._Status == EntityStatus.Active)
                {

                    if (Using<IMasterDataUsage>(c).CheckAgriCentreIsUsed(SelectedCentreItem.Centre,EntityStatus.Inactive))
                    {
                        MessageBox.Show(
                            "Centre " + SelectedCentreItem.Centre.Name +
                            " has been allocated to commodity producers. Unallocate first to deactivate this centre.",
                            "Agrimanagr: Deactivate Centre", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }
                }

                ResponseBool response = new ResponseBool() {Success = false};
                if (SelectedCentreItem == null) return;
                _proxy = Using<IDistributorServiceProxy>(c);
                response = await _proxy.CentreActivateOrDeactivateAsync(SelectedCentreItem.Centre.Id);

                MessageBox.Show(response.ErrorInfo, "Agrimangr: Manage Centre", MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
        }

        protected override async void DeleteSelected()
        {
            if (
                      MessageBox.Show("Are you sure you want to delete this centre",
                                      "Agrimanagr: Activate Centre", MessageBoxButton.OKCancel) ==
                      MessageBoxResult.Cancel) return;
            using (var c = NestedContainer)
            {
                if (SelectedCentreItem.Centre._Status == EntityStatus.Active)
                {

                    if (Using<IMasterDataUsage>(c).CheckAgriCentreIsUsed(SelectedCentreItem.Centre, EntityStatus.Deleted))
                    {
                        MessageBox.Show(
                            "Centre " + SelectedCentreItem.Centre.Name +
                            " has been allocated to commodity producers. Unallocate first to delete this centre.",
                            "Agrimanagr: Delete Centre", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }
                }

                ResponseBool response = new ResponseBool() {Success = false};
                if (SelectedCentreItem == null) return;
                _proxy = Using<IDistributorServiceProxy>(c);
                response = await _proxy.CentreDeleteAsync(SelectedCentreItem.Centre.Id);

                if(response.Success)
                {
                    Using<ICentreRepository>(c).SetAsDeleted(SelectedCentreItem.Centre);
                }

                MessageBox.Show(response.ErrorInfo, "Agrimangr: Manage Centre", MessageBoxButton.OK,
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

    public class VMCentreItem
    {
        public Centre Centre { get; set; }
        public int RowNumber { get; set; }
        public string HlkDeactivateContent { get; set; }
    }

    #endregion

}
