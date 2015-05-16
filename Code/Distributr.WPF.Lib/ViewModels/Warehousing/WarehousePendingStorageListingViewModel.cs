using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master.Util;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.Core.Repository.Master.CommodityOwnerRepository;
using Distributr.Core.Repository.Transactional.SourcingDocumentRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.WPF.Lib.Messages;
using Distributr.WPF.Lib.ViewModels.Admin;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using StructureMap;

namespace Distributr.WPF.Lib.ViewModels.Warehousing
{
    public class WarehousePendingStorageListingViewModel : ListingsViewModelBase//DistributrViewModelBase
    {
        private PagenatedList<CommodityWarehouseStorageNote> _pagedCommoditySupplierInventoryLevel;
        public WarehousePendingStorageListingViewModel()
        {
            WarehousePendingStorageList = new ObservableCollection<WarehousePendingStorageDocumentListItem>();
            LoadListingPageCommand = new RelayCommand(LoadPage);
            StoreCommand = new RelayCommand(StoreWarehouseListing);
            RowNumber=0;
        }


        #region Members
        public ObservableCollection<WarehousePendingStorageDocumentListItem> WarehousePendingStorageList { get; set; }
        public RelayCommand LoadListingPageCommand { get; set; }


        protected override void Load(bool isFirstLoad = false)
        {
            LoadWarehouseExitsList();
        }

        protected override void EditSelected()
        {
            throw new NotImplementedException();
        }

        protected override void ActivateSelected()
        {
            throw new NotImplementedException();
        }

        protected override void DeleteSelected()
        {
            throw new NotImplementedException();
        }

        protected override void GoToPage(PageDestinations page)
        {
            GoToPageBase(page, _pagedCommoditySupplierInventoryLevel.PageCount);
        }

        protected override void ComboPageSizesSelectionChanged(int take)
        {
            ItemsPerPage = take;
            LoadPage();
        }

        protected override void UpdatePagenationControl()
        {
            UpdatePagenationControlBase(_pagedCommoditySupplierInventoryLevel.PageNumber, _pagedCommoditySupplierInventoryLevel.PageCount, _pagedCommoditySupplierInventoryLevel.TotalItemCount,
                                      _pagedCommoditySupplierInventoryLevel.IsFirstPage, _pagedCommoditySupplierInventoryLevel.IsLastPage);
        }

        public RelayCommand StoreCommand { get; set; }
        public int RowNumber;
        #endregion

        #region Methods

        private void LoadPage()
        {
            LoadWarehouseExitsList();
        }

        private void LoadWarehouseExitsList()
        {
            using (var c = ObjectFactory.Container.GetNestedContainer())
            {
                var query = new QueryDocument();
                query.Skip = ItemsPerPage * (CurrentPage - 1);
                query.Take = ItemsPerPage;
                query.ShowInactive = true;
                query.DocumentSourcingStatusId =(int) DocumentSourcingStatus.Approved;


                var rawList = c.GetInstance<ICommodityWarehouseStorageRepository>().Query(query);
                var data = rawList.Data.OfType<CommodityWarehouseStorageNote>();
                WarehousePendingStorageList.Clear();
                RowNumber=1;
               // rawList.ForEach(n => WarehousePendingStorageList.Add(Map(n)));

                _pagedCommoditySupplierInventoryLevel = new PagenatedList<CommodityWarehouseStorageNote>(data.AsQueryable(),
                                                                                       CurrentPage,
                                                                                       ItemsPerPage,
                                                                                       rawList.Count, true);

                _pagedCommoditySupplierInventoryLevel.ForEach(n => WarehousePendingStorageList.Add(Map(n)));
                UpdatePagenationControl();
               
            }    
        }

        private WarehousePendingStorageDocumentListItem Map(CommodityWarehouseStorageNote document)
        {
            
            
            var item = new WarehousePendingStorageDocumentListItem();
            item.RowNumber = RowNumber;
            item.DocumentId = document.Id;
            item.DocumentReference = document.DocumentReference;
            item.DocumentDateIssued = document.DocumentDateIssued;
            item.Driver = document.DriverName;
            item.RegistrationNumber = document.VehiclRegNo;
           
            var commodityWarehouseStorageLineItem = document.LineItems.FirstOrDefault();

            if (commodityWarehouseStorageLineItem != null)
            {
                item.CommodityGrade = commodityWarehouseStorageLineItem.CommodityGrade != null ? commodityWarehouseStorageLineItem.CommodityGrade.Name : "";
                item.Commodity = commodityWarehouseStorageLineItem.Commodity.Name;
                item.InitialWeight = commodityWarehouseStorageLineItem.Weight;
                item.FinalWeight = commodityWarehouseStorageLineItem.FinalWeight;
                item.CommodityWeight = commodityWarehouseStorageLineItem.Weight -
                                       commodityWarehouseStorageLineItem.FinalWeight;

            }
          
            using(var c = ObjectFactory.Container.GetNestedContainer())
            {
                var commodityOwner = c.GetInstance<ICommodityOwnerRepository>().GetById(document.CommodityOwnerId);
                if(commodityOwner!=null)
                {
                    item.CommodityOwner = commodityOwner.FullName;
                } 
            }

            RowNumber++;
            return item;
        }


        private void StoreWarehouseListing()
        {
            if (SelectedWarehouseListing != null)
            {
                using (var c = ObjectFactory.Container.GetNestedContainer())
                {

                    Messenger.Default.Send(new WarehouseEntryUpdateMessage
                    {
                        DocumentId = SelectedWarehouseListing.DocumentId,

                    });
                }
                
                SendNavigationRequestMessage( new Uri("/Views/Warehousing/WarehouseStoreFormPage.xaml", UriKind.Relative));
            }
        }
        #endregion


        public const string SelectedWarehouseListingPropertyName = "SelectedWarehouseListing";
        private WarehousePendingStorageDocumentListItem _selectedWarehouseListing = null;
        public WarehousePendingStorageDocumentListItem SelectedWarehouseListing
        {
            get
            {
                return _selectedWarehouseListing;
            }

            set
            {
                if (_selectedWarehouseListing == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedWarehouseListingPropertyName);
                _selectedWarehouseListing = value;
                RaisePropertyChanged(SelectedWarehouseListingPropertyName);
            }
        }

    }

    public class WarehousePendingStorageDocumentListItem
    {
        public int RowNumber { get; set; }
        public Guid DocumentId { get; set; }
        public string DocumentReference { get; set; }
        public DateTime DocumentDateIssued { get; set; }
        public string CommodityOwner { get; set; }
        public string Commodity { get; set; }
        public string CommodityGrade { get; set; }
        public decimal InitialWeight { get; set; }
        public decimal FinalWeight { get; set; }
        public decimal CommodityWeight { get; set; }
        public string Driver { get; set; }
        public string RegistrationNumber { get; set; }
        public string DocumentType { get; set; }

    }

   
}
