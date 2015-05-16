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
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.Util;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.WPF.Lib.Messages;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace Distributr.WPF.Lib.ViewModels.Admin.Producer
{
    public class MemberCommodityProducerViewModel : ListingsViewModelBase
    {
        private PagenatedList<CommodityProducer> _pagedCommodityProducers;
        private IDistributorServiceProxy _proxy;

        public MemberCommodityProducerViewModel()
        {
            ListOfCommodityProducers = new ObservableCollection<VmCommodityProducer>();
            AddProducerCommand = new RelayCommand(AddProducer);
            EditSelectedCommand = new RelayCommand<VmCommodityProducer>(EditProducer);
        }

        #region Class Members

       // public RelayCommand LoadListingPageCommand { get; set; }
        public RelayCommand AddProducerCommand { get; set; }
        public  RelayCommand<VmCommodityProducer> EditSelectedCommand { get; set; }
        public ObservableCollection<VmCommodityProducer> ListOfCommodityProducers { get; set; }

        #endregion

        #region Methods

        private void LoadMemberFarms()
        {
            ListOfCommodityProducers.Clear();
            using (var c = NestedContainer)
            {
                var listOfFarms = Using<ICommodityProducerRepository>(c).GetBySupplier(SupplierId);
                int rowNumber = 0;
                foreach (var l in listOfFarms)
                {
                    ListOfCommodityProducers.Add(new VmCommodityProducer
                        {
                            Id=l.Id,
                            RowNumber=++rowNumber,
                            Code=l.Code,
                            Name=l.Name,
                            Acrage=l.Acrage,
                            RegNo=l.RegNo,
                            PhysicalAddress=l.PhysicalAddress,
                            Description=l.Description,
                        }
                        );
                }
            }
        }

        public void SetSupplier(MemberProducerMessage messageTo)
        {
            SupplierId = messageTo.Id;
        }

        private void EditProducer(VmCommodityProducer producer)
        {
            Messenger.Default.Send(new EditCommodityProducerMessage
                {
                    SupplierId = SupplierId,
                    CommodityProducerId = producer.Id
                });
            SendNavigationRequestMessage(new Uri("/Views/Admin/Producer/CommodityProducer.xaml", UriKind.Relative));
        }

        private void AddProducer()
        {
            Messenger.Default.Send(new AddCommodityProducerMessage
                {
                    SupplierId = SupplierId
                });
            SendNavigationRequestMessage(new Uri("/Views/Admin/Producer/CommodityProducer.xaml", UriKind.Relative));
        }

        protected override void Load(bool isFirstLoad = false)
        {
            Application.Current.Dispatcher.BeginInvoke(
                new Action(
                    delegate
                    {
                        if (isFirstLoad)
                            LoadMemberFarms();
                        using (var c = NestedContainer)
                        {
                            var query = new QueryCommodityProducer();
                            query.Take = ItemsPerPage;
                            query.Skip = ItemsPerPage*(CurrentPage - 1);
                            query.SupplierId = SupplierId;
                            query.ShowInactive = ShowInactive;

                            if (!string.IsNullOrWhiteSpace(SearchText))
                                query.Name = SearchText;
                            var rawList = Using<ICommodityProducerRepository>(c).Query(query);

                            _pagedCommodityProducers = new PagenatedList<CommodityProducer>(rawList.Data.OfType<CommodityProducer>().AsQueryable(),
                                                                                      CurrentPage,
                                                                                      ItemsPerPage,
                                                                                      rawList.Count,true);
                            ListOfCommodityProducers.Clear();
                            int rownumber = 0;
                            _pagedCommodityProducers.ToList().ForEach(n =>
                                                                   ListOfCommodityProducers.Add(new VmCommodityProducer
                                                                   {
                                                                       Id = n.Id,
                                                                       Code = n.Code,
                                                                       Acrage = n.Acrage,
                                                                       Name = n.Name,
                                                                       RegNo = n.RegNo,
                                                                       PhysicalAddress = n.PhysicalAddress,
                                                                       Description = n.Description,
                                                                       Status=n._Status,
                                                                       Action=n._Status==EntityStatus.Active?"Deactivate":"Activate",
                                                                       RowNumber = ++rownumber
                                                                   }));
                            UpdatePagenationControl();
                        }
                    }));
        }

        protected override void EditSelected()
        {
            throw new NotImplementedException();
        }

        protected async override void ActivateSelected()
        {
            string action = SelectedCommodityProducer.Status == EntityStatus.Active
                               ? "deactivate"
                               : "activate";
            if (
                    MessageBox.Show("Are you sure you want to " + action + " this commodity Producer?",
                                    "Agrimanagr: Activate Commodity Producer", MessageBoxButton.YesNo) ==
                    MessageBoxResult.No) return;

            using (var c = NestedContainer)
            {
                if (SelectedCommodityProducer.Status == EntityStatus.Active)
                {
                    var commodityProducer = Using<ICommodityProducerRepository>(c).GetById(SelectedCommodityProducer.Id);

                    if (Using<IMasterDataUsage>(c).CommodityProducerHasPurchases(commodityProducer))
                    {
                        MessageBox.Show(
                            "Commodity Producer " + SelectedCommodityProducer.Name +
                            " has Purchases in the system and thus cannot be deactivated.",
                            "Agrimanagr: Deactivate Commodity Producer", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                }


                ResponseBool response = new ResponseBool() { Success = false };
                if (SelectedCommodityProducer == null) return;
                _proxy = Using<IDistributorServiceProxy>(c);
                response = await _proxy.CommodityProducerActivateOrDeactivateAsync(SelectedCommodityProducer.Id);

                MessageBox.Show(response.ErrorInfo, "Agrimangr: Manage Commodity Owner", MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
        }

        protected async override void DeleteSelected()
        {
            if (
               MessageBox.Show("Are you sure you want to delete this commodity Producer?",
                                    "Agrimanagr: Delete Commodity Producer", MessageBoxButton.YesNo) ==
                    MessageBoxResult.No) return;

            using (var c = NestedContainer)
            {
                if (SelectedCommodityProducer.Status == EntityStatus.Active)
                {

                    var commodityProducer = Using<ICommodityProducerRepository>(c).GetById(SelectedCommodityProducer.Id) ;

                    if (Using<IMasterDataUsage>(c).CommodityProducerHasPurchases(commodityProducer))
                    {
                        MessageBox.Show(
                            "Commodity Producer " + SelectedCommodityProducer.Name +
                            " has purchases in the system and thus cannot be deleted.",
                            "Agrimanagr: Delete Commodity Producer", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                }
                ResponseBool response = new ResponseBool() { Success = false };
                // 1 if (SelectedCommodityOwner == null) return;
                if (SelectedCommodityProducer == null) return;
                _proxy = Using<IDistributorServiceProxy>(c);
                response = await _proxy.CommodityProducerDeleteAsync(SelectedCommodityProducer.Id);
                if (response.Success)
                {
                    var commodityProducer = Using<ICommodityProducerRepository>(c).GetById(SelectedCommodityProducer.Id);
                    Using<ICommodityProducerRepository>(c).SetAsDeleted(commodityProducer);

                    MessageBox.Show(response.ErrorInfo, "Agrimangr:Manage Commodity Producer", MessageBoxButton.OK,
                                   MessageBoxImage.Information);
                    Load();
                }
                   
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

        #endregion

        #region Properties


        public const string SupplierIdPropertyName = "SupplierId";
        private Guid _supplierId = Guid.NewGuid();
        public Guid SupplierId
        {
            get { return _supplierId; }

            set
            {
                if (_supplierId == value)
                {
                    return;
                }

                RaisePropertyChanging(SupplierIdPropertyName);
                _supplierId = value;
                RaisePropertyChanged(SupplierIdPropertyName);
            }
        }

        public const string SelectedCommodityProducerPropertyName = "SelectedCommodityProducer";
        private VmCommodityProducer _selectedCommodityProducer = null;

        public VmCommodityProducer SelectedCommodityProducer
        {
            get { return _selectedCommodityProducer; }

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


        #endregion
    }

    public class VmCommodityProducer
    {
        public Guid Id { get; set; }
        public int RowNumber { get; set; }
        public string Code { get; set; }
        public string Acrage { get; set; }
        public string Name { get; set; }
        public string RegNo { get; set; }
        public string PhysicalAddress { get; set; }
        public string Description { get; set; }
        public EntityStatus Status { get; set; }
        public string Action { get; set; }
       
    }



}
