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
using Distributr.WPF.Lib.UI.Pages;
using GalaSoft.MvvmLight.Command;
using StructureMap;

namespace Distributr.WPF.Lib.ViewModels.Admin.CommoditySuppliers
{
    public class ListCommoditySuppliersViewModel : ListingsViewModelBase
    {
        public ListCommoditySuppliersViewModel()
        {
            CommoditySupplierList = new ObservableCollection<VMCommoditySupplierItem>();
            MappingCommand = new RelayCommand<VMCommoditySupplierItem>(Mapping);
        }

        private async void Mapping(VMCommoditySupplierItem item)
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                
               Using<IFarmerOutletMapping>(c).SupplierToOutletToMappping(item.CommoditySupplier);
               
            }
           
        }

        #region properties

        public RelayCommand<VMCommoditySupplierItem> MappingCommand { get; set; }

        private IPagenatedList<CostCentre> _pagenatedCommSuppList;
        public ObservableCollection<VMCommoditySupplierItem> CommoditySupplierList { get; set; }

        public const string SelectedCommoditySupplierPropertyName = "SelectedCommoditySupplier";
        private VMCommoditySupplierItem _selectedCommoditySupplier = null;
        public VMCommoditySupplierItem SelectedCommoditySupplier
        {
            get { return _selectedCommoditySupplier; }

            set
            {
                if (_selectedCommoditySupplier == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedCommoditySupplierPropertyName);
                _selectedCommoditySupplier = value;
                RaisePropertyChanged(SelectedCommoditySupplierPropertyName);
            }
        }

        #endregion

        #region methods

        protected override void Load(bool isFirstLoad = false)
        {
            if (isFirstLoad)
                Setup();
            LoadList();
        }

        protected override void EditSelected()
        {
            if (SelectedCommoditySupplier == null) return;
            SendNavigationRequestMessage(
                new Uri(
                    "/views/admin/commoditysuppliers/editcommoditysupplier.xaml?" +
                    SelectedCommoditySupplier.CommoditySupplier.Id, UriKind.Relative));
        }

       

        protected override void ActivateSelected()
        {
            if (SelectedCommoditySupplier == null) return;
            if(SelectedCommoditySupplier.CommoditySupplier._Status == EntityStatus.Active)
            {
                using(var c = NestedContainer)
                {
                    if (Using<IMasterDataUsage>(c).CheckCommoditySupplierIsUsed(SelectedCommoditySupplier.CommoditySupplier,
                                                                  EntityStatus.Inactive))
                    {
                        MessageBox.Show(
                            "Commodity Supplier " + SelectedCommoditySupplier.CommoditySupplier.Name +
                            " has dependent commodity producers and/or commodity owners that must first be deactivated in order to continue.",
                            "Agrimanagr: Deactivate Commodity Supplier", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                }
            }
            ActivateOrDeactivateSelected(SelectedCommoditySupplier.CommoditySupplier.Id);
        }

        protected override void DeleteSelected()
        {
            if (SelectedCommoditySupplier == null) return;
            using (var c = NestedContainer)
            {
                if (Using<IMasterDataUsage>(c).CheckCommoditySupplierIsUsed(SelectedCommoditySupplier.CommoditySupplier,
                                                              EntityStatus.Deleted))
                {
                    MessageBox.Show(
                        "Commodity Supplier " + SelectedCommoditySupplier.CommoditySupplier.Name +
                        " has dependent commodity producers and/or commodity owners that must first be deleted in order to continue.",
                        "Agrimanagr: Delete Commodity Supplier", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
            DeleteSelected(SelectedCommoditySupplier.CommoditySupplier.Id);
        }

        protected override void GoToPage(PageDestinations page)
        {
            GoToPageBase(page, _pagenatedCommSuppList.PageCount);
        }

        protected override void ComboPageSizesSelectionChanged(int take)
        {
            ItemsPerPage = take;
            Load();
        }

        protected override void UpdatePagenationControl()
        {
            UpdatePagenationControlBase(_pagenatedCommSuppList.PageNumber, _pagenatedCommSuppList.PageCount, _pagenatedCommSuppList.TotalItemCount,
                                      _pagenatedCommSuppList.IsFirstPage, _pagenatedCommSuppList.IsLastPage);
        }

        private async void ActivateOrDeactivateSelected(Guid id)
        {
            string action = SelectedCommoditySupplier.CommoditySupplier._Status == EntityStatus.Active
                                ? "deactivate"
                                : "activate";
            if (
                    MessageBox.Show("Are you sure you want to "+action+" this commodity supplier account?",
                                    "Agrimanagr: Activate Commodity Supplier", MessageBoxButton.OKCancel) ==
                    MessageBoxResult.Cancel) return;
            using (StructureMap.IContainer c = NestedContainer)
            {
                IDistributorServiceProxy proxy = Using<IDistributorServiceProxy>(c);
                ResponseBool response = null;
                response = await proxy.CommoditySupplierActivateOrDeactivateAsync(id);
                MessageBox.Show(response.ErrorInfo, "Distributr: Manage Commodity Supplier", MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
        }

        private async void DeleteSelected(Guid id)
        {
            if (
                    MessageBox.Show("Are you sure you want to delete this commodity supplier account?",
                                    "Agrimanagr: Delete Commodity Supplier", MessageBoxButton.OKCancel) ==
                    MessageBoxResult.Cancel) return;
            using (StructureMap.IContainer c = NestedContainer)
            {
                var proxy = Using<IDistributorServiceProxy>(c);
                ResponseBool response = null;
                response = await proxy.CommoditySupplierDeleteAsync(id);
                if (response.Success)
                    Using<ICommoditySupplierRepository>(c).SetAsDeleted(SelectedCommoditySupplier.CommoditySupplier);
                MessageBox.Show(response.ErrorInfo, "Agrimanagr: Manage Commodity Supplier", MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
        }

        private void Setup()
        {
            PageTitle = "Listing Hub Accounts";
        }

        private void LoadList()
        {
            Application.Current.Dispatcher.BeginInvoke(
                new Action(
                    delegate
                        {
                            using (var c = NestedContainer)
                            {
                                _pagenatedCommSuppList = Using<ICommoditySupplierRepository>(c).GetAll(CurrentPage,
                                                                                                       ItemsPerPage,
                                                                                                       SearchText,
                                                                                                       ShowInactive);
                                CommoditySupplierList.Clear();
                                _pagenatedCommSuppList.Select((n, i) => Map(n as CommoditySupplier, i)).ToList().ForEach
                                    (CommoditySupplierList.Add);
                                UpdatePagenationControl();
                            }
                        }));
        }

        private VMCommoditySupplierItem Map(CommoditySupplier cs, int index)
        {
            VMCommoditySupplierItem mapped = new VMCommoditySupplierItem
                                                 {
                                                     CommoditySupplier = cs,
                                                     RowNumber = index + 1
                                                 };

            if (cs._Status == EntityStatus.Active)
                mapped.HlkDeactivateContent = "Deactivate";
            else if (cs._Status == EntityStatus.Inactive)
                mapped.HlkDeactivateContent = "Activate";
            return mapped;
        }

        #endregion

    }

    #region helpers

    public class VMCommoditySupplierItem
    {
        public CommoditySupplier CommoditySupplier { get; set; }
        public int RowNumber { get; set; }
        public string HlkDeactivateContent { get; set; }
    }

    #endregion

}
