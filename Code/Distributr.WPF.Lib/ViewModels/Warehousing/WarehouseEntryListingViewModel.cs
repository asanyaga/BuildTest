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
    public class WarehouseEntryListingViewModel : ListingsViewModelBase//DistributrViewModelBase
    {
        private PagenatedList<CommodityWarehouseStorageNote> _pagedCommoditySupplierInventoryLevel;

        public WarehouseEntryListingViewModel()
        {
            WarehouseEntryList=new ObservableCollection<WarehouseEntryDocumentListItem>();
            LoadListingPageCommand = new RelayCommand(LoadPage);
            ExitCommand = new RelayCommand(ExitWarehouseListing);
            RowNumber = 0;

        }


        #region Members
        public ObservableCollection<WarehouseEntryDocumentListItem> WarehouseEntryList { get; set; }
        public RelayCommand LoadListingPageCommand { get; set; }
        protected override void Load(bool isFirstLoad = false)
        {
            LoadWarehouseEntriesList();
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

        public RelayCommand ExitCommand { get; set; }
        public int RowNumber;
        #endregion

        #region Methods

        private void LoadPage()
        {
            
            LoadWarehouseEntriesList();
        }

        private void LoadWarehouseEntriesList()
        {
            using (var c = ObjectFactory.Container.GetNestedContainer())
            {
                
                var query = new QueryDocument();
                query.Skip = ItemsPerPage * (CurrentPage - 1);
                query.Take = ItemsPerPage;
                query.ShowInactive = true;
                query.DocumentSourcingStatusId = (int) DocumentSourcingStatus.Confirmed;

                //var rawList = c.GetInstance<ICommodityWarehouseStorageRepository>().Query(query).OfType<CommodityWarehouseStorageNote>().ToList();
                //WarehouseEntryList.Clear();
                //RowNumber = 1;
                //rawList.ForEach(n=>WarehouseEntryList.Add(Map(n)));

                var rawList = c.GetInstance<ICommodityWarehouseStorageRepository>().Query(query);
                var data = rawList.Data.OfType<CommodityWarehouseStorageNote>();
                WarehouseEntryList.Clear();
                RowNumber = 1;
                // rawList.ForEach(n => WarehousePendingStorageList.Add(Map(n)));

                _pagedCommoditySupplierInventoryLevel = new PagenatedList<CommodityWarehouseStorageNote>(data.AsQueryable(),
                                                                                       CurrentPage,
                                                                                       ItemsPerPage,
                                                                                       rawList.Count, true);

                _pagedCommoditySupplierInventoryLevel.ForEach(n => WarehouseEntryList.Add(Map(n)));
                UpdatePagenationControl();
               
            }    
        }

        private WarehouseEntryDocumentListItem Map(CommodityWarehouseStorageNote document)
        {
            
            
            var item = new WarehouseEntryDocumentListItem();
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
                item.TotalWeight = commodityWarehouseStorageLineItem.Weight;
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


        private void ExitWarehouseListing()
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
                
                SendNavigationRequestMessage( new Uri("/Views/Warehousing/WarehouseExitFormPage.xaml", UriKind.Relative));
            }
        }
        #endregion


        public const string SelectedWarehouseListingPropertyName = "SelectedWarehouseListing";
        private WarehouseEntryDocumentListItem _selectedWarehouseListing = null;
        public WarehouseEntryDocumentListItem SelectedWarehouseListing
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

    public class WarehouseEntryDocumentListItem
    {
        public int RowNumber { get; set; }
        public Guid DocumentId { get; set; }
        public string DocumentReference { get; set; }
        public DateTime DocumentDateIssued { get; set; }
        public string CommodityOwner { get; set; }
        public string Commodity { get; set; }
        public string CommodityGrade { get; set; }
        public decimal TotalWeight { get; set; }
        public string Driver { get; set; }
        public string RegistrationNumber { get; set; }
        public string DocumentType { get; set; }

        
    }

    
}
