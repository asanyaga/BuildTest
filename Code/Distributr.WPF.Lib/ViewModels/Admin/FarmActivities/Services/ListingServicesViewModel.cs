using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.FarmActivities;
using Distributr.Core.Domain.Master.Util;
using Distributr.Core.Repository.Master.Agrimanagr;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.WPF.Lib.Messages;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using GalaSoft.MvvmLight.Messaging;

namespace Distributr.WPF.Lib.ViewModels.Admin.FarmActivities.Services
{
    public class ListingServicesViewModel:ListingsViewModelBase
    {
        private PagenatedList<CommodityProducerService> _pagedCommodityProducerService;
        private IDistributorServiceProxy _proxy;

        public ListingServicesViewModel()
        {
            ListOfCommodityProducerServices = new ObservableCollection<VmCommodityProducerService>();
        }

        #region Class Members

        public ObservableCollection<VmCommodityProducerService> ListOfCommodityProducerServices { get; set; }
        #endregion

        protected override void Load(bool isFirstLoad = false)
        {
            Application.Current.Dispatcher.BeginInvoke(
                new Action(
                    delegate
                    {
                       // if (isFirstLoad)

                            using (var c = NestedContainer)
                            {
                                var query = new QueryCommodityProducerService();
                                query.Take = ItemsPerPage;
                                query.Skip = ItemsPerPage * (CurrentPage - 1);
                                query.ShowInactive = ShowInactive;
                                if (!string.IsNullOrWhiteSpace(SearchText))
                                    query.Name = SearchText;

                                var rawList = Using<IServiceRepository>(c).Query(query);
                                _pagedCommodityProducerService = new PagenatedList<CommodityProducerService>(rawList.Data.OfType<CommodityProducerService>().AsQueryable(),
                                                                                          CurrentPage,
                                                                                          ItemsPerPage,
                                                                                          rawList.Count, true);

                                ListOfCommodityProducerServices.Clear();
                                int rownumber = 0;
                                _pagedCommodityProducerService.ToList().ForEach(n =>
                                                                       ListOfCommodityProducerServices.Add(new VmCommodityProducerService
                                                                       {
                                                                           Id = n.Id,
                                                                           Code = n.Code,
                                                                           Name = n.Name,
                                                                           Cost=n.Cost,
                                                                           Status = n._Status,
                                                                           Action = n._Status == EntityStatus.Active ? "Deactivate" : "Activate",
                                                                           RowNumber = ++rownumber
                                                                       }));
                                UpdatePagenationControl();
                            }
                    }));
        }

        protected override void EditSelected()
        {
            if (SelectedCommodityProducerService != null)
            {
                using (var c = NestedContainer)
                {

                    Messenger.Default.Send(new EditCommodityProducerServiceMessage
                    {
                        ServiceId = SelectedCommodityProducerService.Id
                    });
                }
                SendNavigationRequestMessage(
                    new Uri("/Views/Admin/Farmactivities/Services/EditServices.xaml", UriKind.Relative));
            }
        }

        protected async override void ActivateSelected()
        {
            string action = SelectedCommodityProducerService.Status == EntityStatus.Active
                               ? "deactivate"
                               : "activate";
            if (MessageBox.Show("Are you sure you want to " + action + " this Commodity Producer Service ?",
                                "Agrimanagr: Manage Commodity Producer Service ", MessageBoxButton.YesNo) ==
                MessageBoxResult.No) return;

            using (var c = NestedContainer)
            {
                ResponseBool response = new ResponseBool() { Success = false };
                if (SelectedCommodityProducerService == null) return;
                _proxy = Using<IDistributorServiceProxy>(c);
                response = await _proxy.CommodityProducerServiceActivateOrDeactivateAsync(SelectedCommodityProducerService.Id);

                MessageBox.Show(response.ErrorInfo, "Agrimangr: Manage Commodity Producer Service ", MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
        }

        protected async override void DeleteSelected()
        {
            if (
               MessageBox.Show("Are you sure you want to delete this Commodity Producer Service?",
                                    "Agrimanagr: Delete Commodity Producer Service", MessageBoxButton.YesNo) ==
                    MessageBoxResult.No) return;

            using (var c = NestedContainer)
            {
                if (SelectedCommodityProducerService.Status == EntityStatus.Active)
                {

                    var service = Using<IServiceRepository>(c).GetById(SelectedCommodityProducerService.Id);


                }
                var response = new ResponseBool() { Success = false };
                if (SelectedCommodityProducerService == null) return;
                _proxy = Using<IDistributorServiceProxy>(c);
                response = await _proxy.CommodityProducerServiceDeleteAsync(SelectedCommodityProducerService.Id);
                if (response.Success)
                {
                    var commodityProducerService =Using<IServiceRepository>(c).GetById(SelectedCommodityProducerService.Id);
                    Using<IServiceRepository>(c).SetAsDeleted(commodityProducerService);
                    MessageBox.Show(response.ErrorInfo, "Agrimangr:Manage Commodity Producer Service", MessageBoxButton.OK,
                                    MessageBoxImage.Information);
                }
                    
            }
        }

        protected override void GoToPage(PageDestinations page)
        {
            GoToPageBase(page, _pagedCommodityProducerService.PageCount);
        }

        protected override void ComboPageSizesSelectionChanged(int take)
        {
            ItemsPerPage = take;
            Load();
        }

        protected override void UpdatePagenationControl()
        {
            UpdatePagenationControlBase(_pagedCommodityProducerService.PageNumber, _pagedCommodityProducerService.PageCount, _pagedCommodityProducerService.TotalItemCount,
                                     _pagedCommodityProducerService.IsFirstPage, _pagedCommodityProducerService.IsLastPage);
        }


        #region Properties

        
        public const string IdPropertyName = "Id";
        private Guid _id = Guid.Empty;
        public Guid Id
        {
            get
            {
                return _id;
            }

            set
            {
                if (_id == value)
                {
                    return;
                }

                RaisePropertyChanging(IdPropertyName);
                _id = value;
                RaisePropertyChanged(IdPropertyName);
            }
        }

        public const string SelectedCommodityProducerServicePropertyName = "SelectedCommodityProducerService";
        private VmCommodityProducerService _selectedCommodityProducerService = null;
        public VmCommodityProducerService SelectedCommodityProducerService
        {
            get
            {
                return _selectedCommodityProducerService;
            }

            set
            {
                if (_selectedCommodityProducerService == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedCommodityProducerServicePropertyName);
                _selectedCommodityProducerService = value;
                RaisePropertyChanged(SelectedCommodityProducerServicePropertyName);
            }
        }
        #endregion
    }
    public class VmCommodityProducerService
    {
        public int RowNumber { get; set; }
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal Cost { get; set; }
        public EntityStatus Status { get; set; }
        public string Action { get; set; }

    }
}
