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
using GalaSoft.MvvmLight.Command;

namespace Distributr.WPF.Lib.ViewModels.Admin.CommodityProducers
{
    public class ListCommodityProducersViewModel : ListingsViewModelBase
    {
        private IPagenatedList<CommodityProducer> _pagedCommodityProducers;
        private IDistributorServiceProxy _proxy;

        public ListCommodityProducersViewModel()
        {
            CommodityProducersList = new ObservableCollection<VMCommodityProducerItem>();
            CommoditySuppliersList = new ObservableCollection<CommoditySupplier>();
        }

        #region properties

        public ObservableCollection<CommoditySupplier> CommoditySuppliersList { get; set; }
        public ObservableCollection<VMCommodityProducerItem> CommodityProducersList { get; set; }

        public const string SelectedCommoditySupplierPropertyName = "SelectedCommoditySupplier";
        private CommoditySupplier _selectedCommodtitySupplier = null;
        public CommoditySupplier SelectedCommoditySupplier
        {
            get { return _selectedCommodtitySupplier; }

            set
            {
                if (_selectedCommodtitySupplier == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedCommoditySupplierPropertyName);
                _selectedCommodtitySupplier = value;
                RaisePropertyChanged(SelectedCommoditySupplierPropertyName);
            }
        }
         
        public const string SelectedCommodityProducerPropertyName = "SelectedCommodityProducer";
        private VMCommodityProducerItem _selectedCommodityProducer = null;
        public VMCommodityProducerItem SelectedCommodityProducer
        {
            get
            {
                return _selectedCommodityProducer;
            }

            set
            {
                if (_selectedCommodityProducer == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedCommodityProducerPropertyName);
                _selectedCommodityProducer = value;
                RaisePropertyChanged(SelectedCommodityProducerPropertyName);
            }
        }

        private CommoditySupplier _defaultCommoditySupplier;
        private CommoditySupplier DefaultCommoditySupplier
        {
            get
            {
                return _defaultCommoditySupplier ??
                       (_defaultCommoditySupplier =
                        new CommoditySupplier(Guid.Empty) {Name = "--Select commodity supplier--"});
            }
        }

        public RelayCommand ViewCommodityProducerCentresCommand
        {
            get
            {
                return _viewCommodityProducerCentresCommand ??
                       (_viewCommodityProducerCentresCommand = new RelayCommand(ViewCommodityProducerCentres));
            }
        }
        private RelayCommand _viewCommodityProducerCentresCommand;

        #endregion

        #region methods

        private void ViewCommodityProducerCentres()
        {
            throw new NotImplementedException();
        } 

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
                                var rawList = Using<ICommodityProducerRepository>(c).GetAll(ShowInactive)
                                    .Where(n => n.Code.ToLower().Contains(SearchText.ToLower()) ||
                                                n.Name.ToLower().Contains(SearchText.ToLower()));
                                if (SelectedCommoditySupplier != null && SelectedCommoditySupplier.Id != Guid.Empty)
                                {
                                    rawList = rawList.Where(n => n.CommoditySupplier.Id == SelectedCommoditySupplier.Id);
                                }
                                rawList = rawList.OrderBy(n => n.Name).ThenBy(n => n.Code);
                                _pagedCommodityProducers = new PagenatedList<CommodityProducer>(rawList.AsQueryable(),
                                                                                                CurrentPage,
                                                                                                ItemsPerPage,
                                                                                                rawList.Count());

                                CommodityProducersList.Clear();
                                _pagedCommodityProducers.Select((n, i) => Map(n, i + 1)).ToList().ForEach(n => CommodityProducersList.Add(n));
                                UpdatePagenationControl();
                            }
                        }));
        }

        VMCommodityProducerItem Map(CommodityProducer cp, int index)
        {
            var mapped = new VMCommodityProducerItem
                       {
                           CommodityProducer = cp,
                           RowNumber = index
                       };
            if (cp._Status == Core.Domain.Master.EntityStatus.Active)
                mapped.HlkDeactivateContent = "Deactivate";
            else if (cp._Status == Core.Domain.Master.EntityStatus.Inactive)
                mapped.HlkDeactivateContent = "Activate";
            return mapped;
        }

        void Setup()
        {
            PageTitle = "List Farms";
            LoadCommoditySupplierList();
        }

        protected override void EditSelected()
        {
            if (SelectedCommodityProducer != null)
                SendNavigationRequestMessage(
                    new Uri("/views/admin/commodityproducers/editcommodityproducer.xaml?" + SelectedCommodityProducer.CommodityProducer.Id,UriKind.Relative));
        }

        protected async override void ActivateSelected()
        {
            string action = SelectedCommodityProducer.CommodityProducer._Status == EntityStatus.Active
                               ? "deactivate"
                               : "activate";
            if (
                    MessageBox.Show("Are you sure you want to " + action + " this commodity producer?",
                                    "Agrimanagr: " + action + " Commodity Producer", MessageBoxButton.OKCancel) ==
                    MessageBoxResult.Cancel) return;

            using (var c = NestedContainer)
            {
                if (SelectedCommodityProducer.CommodityProducer._Status == EntityStatus.Active)
                {
                    if (
                        Using<IMasterDataUsage>(c).CommodityProducerHasPurchases(
                            SelectedCommodityProducer.CommodityProducer))
                    {
                        MessageBox.Show(
                            "Commodity producer " + SelectedCommodityProducer.CommodityProducer.Name +
                            " has purchases in the system and thus cannot be deleted.",
                            "Agrimanagr: Deactivate Commodity Producer", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                }
                ResponseBool response = new ResponseBool() {Success = false};
                if (SelectedCommodityProducer == null) return;
                _proxy = Using<IDistributorServiceProxy>(c);
                response = await _proxy.CommodityProducerActivateOrDeactivateAsync(SelectedCommodityProducer.CommodityProducer.Id);

                MessageBox.Show(response.ErrorInfo, "Agrimangr: Manage Commodity Producer", MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
        }

        protected async override void DeleteSelected()
        {
            if (
             MessageBox.Show("Are you sure you want to delete this commodity producer?",
                                    "Agrimanagr: Delete Commodity Producer", MessageBoxButton.OKCancel) ==
                    MessageBoxResult.Cancel) return;
            using (var c = NestedContainer)
            {
                if (Using<IMasterDataUsage>(c).CommodityProducerHasPurchases(SelectedCommodityProducer.CommodityProducer))
                {
                    MessageBox.Show(
                        "Commodity producer " + SelectedCommodityProducer.CommodityProducer.Name +
                        " has purchases in the system and thus cannot be deleted.",
                        "Agrimanagr: Delete Commodity Producer", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                ResponseBool response = new ResponseBool() { Success = false };
                if (SelectedCommodityProducer == null) return;
                _proxy = Using<IDistributorServiceProxy>(c);
                response = await _proxy.CommodityProducerDeleteAsync(SelectedCommodityProducer.CommodityProducer.Id);
                if (response.Success)
                    Using<ICommodityProducerRepository>(c).SetAsDeleted(SelectedCommodityProducer.CommodityProducer);
                MessageBox.Show(response.ErrorInfo, "Agrimangr: Manage Commodity Producer", MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
        }

        protected override void GoToPage(PageDestinations page)
        {
            GoToPageBase(page, _pagedCommodityProducers.PageCount);
        }

        protected override void ComboPageSizesSelectionChanged(int take)
        {
            ItemsPerPage = take;
            Load();
        }

        protected override void UpdatePagenationControl()
        {
            UpdatePagenationControlBase(_pagedCommodityProducers.PageNumber, _pagedCommodityProducers.PageCount, _pagedCommodityProducers.TotalItemCount,
                                      _pagedCommodityProducers.IsFirstPage, _pagedCommodityProducers.IsLastPage);
        }

        void LoadCommoditySupplierList()
        {
            CommoditySuppliersList.Clear();
            using (var c = NestedContainer)
            {
                CommoditySuppliersList.Add(DefaultCommoditySupplier);
                var list = Using<ICommoditySupplierRepository>(c).GetAll().OrderBy(n => n.Name).ToList();
                list.ForEach(n => CommoditySuppliersList.Add(n as CommoditySupplier));
                SelectedCommoditySupplier = DefaultCommoditySupplier;
            }
        }

        #endregion

    }

    public class VMCommodityProducerItem
    {
        public CommodityProducer CommodityProducer { get; set; }
        public int RowNumber { get; set; }
        public string HlkDeactivateContent { get; set; }
        public bool IsDirty { get; set; }
    }
}
