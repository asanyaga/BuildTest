using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Distributr.Core.ClientApp;
using Distributr.Core.Domain.Master.CommodityEntities;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities.LineItems;
using Distributr.Core.Factory.SourcingDocuments;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Transactional.SourcingDocumentRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.UI.Pages;
using Distributr.WPF.Lib.ViewModels.Transactional.CommodityPurchase;
using Distributr.WPF.Lib.ViewModels.Transactional.Orders;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using log4net;

namespace Distributr.WPF.Lib.ViewModels.Transactional.CommodityReception
{
    public class StoreCommodityViewModel : DistributrViewModelBase
    {
        private CommodityStorageNote _storageNote;
        private ILog _logger = LogManager.GetLogger("App");
        public StoreCommodityViewModel()
        {
            LoadedDocuments = new ObservableCollection<ReceivedDeliveryNote>();
            LoadedDocumentsLineItemsList = new ObservableCollection<CommodyLineItemViewModel>();
            LoadedDocumentsLineItemsListCache = new ObservableCollection<CommodyLineItemViewModel>();
            LineItemsToStoreList = new ObservableCollection<CommodyLineItemViewModel>();
            StoreList = new ObservableCollection<Store>();
            ItemsToStore = new List<Guid>();

            AssignAllCommand = new RelayCommand(AssignAll);
            AssignSelectedCommand = new RelayCommand(AssignSelected);
            UnassignAllCommand = new RelayCommand(UnAssignAll);
            UnassignSelectedCommand = new RelayCommand(UnassignSelected);
            StorageCommodityPageLoadedCommand = new RelayCommand(LoadStorageWindow);
            CompleteStorageCommand = new RelayCommand(StoreItems);
            DropDownOpenedCommand = new RelayCommand<object>(DropDownOpened);
            CancelCommand = new RelayCommand(Cancel);
            SearchCommand = new RelayCommand(Search);
            ViewSelectedItemCommand = new RelayCommand<CommodityReceptionNote>(ViewSelectedItem);
        }

       

        #region Properties 
        public ObservableCollection<CommodyLineItemViewModel> LineItemsToStoreList { get; set; }
        public ObservableCollection<CommodyLineItemViewModel> LoadedDocumentsLineItemsList { get; set; }
        public ObservableCollection<CommodyLineItemViewModel> LoadedDocumentsLineItemsListCache { get; set; }
        public ObservableCollection<ReceivedDeliveryNote> LoadedDocuments { get; set; }
      
        public List<Guid> ItemsToStore { get; set; }
        public ObservableCollection<Store> StoreList { get; set; }
    
        public RelayCommand AssignAllCommand { get; set; }
        public RelayCommand AssignSelectedCommand { get; set; }
        public RelayCommand UnassignAllCommand { get; set; }
        public RelayCommand UnassignSelectedCommand { get; set; }
        public RelayCommand StorageCommodityPageLoadedCommand { get; set; }
        public RelayCommand CompleteStorageCommand { get; set; }
        public RelayCommand<object> DropDownOpenedCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        public RelayCommand SearchCommand { get; set; }
        public RelayCommand<CommodityReceptionNote> ViewSelectedItemCommand { get; set; }

        public event EventHandler RequestClose = (s, e) => { };

        public const string SelectedStorePropertyName = "SelectedStore";
        private Store _selectedStore = null;
        [MasterDataDropDownValidation]
        public Store SelectedStore
        {
            get
            {
                return _selectedStore;
            }

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
         
        public const string ItemsToStoreTotalWeightPropertyName = "ItemsToStoreTotalWeight";
        private decimal _itemsToStoreTotalWeight = 0m;
        public decimal ItemsToStoreTotalWeight
        {
            get
            {
                return _itemsToStoreTotalWeight;
            }

            set
            {
                if (_itemsToStoreTotalWeight == value)
                {
                    return;
                }

                RaisePropertyChanging(ItemsToStoreTotalWeightPropertyName);
                _itemsToStoreTotalWeight = value;
                RaisePropertyChanged(ItemsToStoreTotalWeightPropertyName);
            }
        }
         
        public const string SelectedLoadedDocumentPropertyName = "SelectedLoadedDocument";
        private ReceivedDeliveryNote _selectedLoadedDocument = null;
        public ReceivedDeliveryNote SelectedLoadedDocument
        {
            get
            {
                return _selectedLoadedDocument;
            }

            set
            {
                if (_selectedLoadedDocument == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedLoadedDocumentPropertyName);
                _selectedLoadedDocument = value;
                RaisePropertyChanged(SelectedLoadedDocumentPropertyName);
            }
        }
         
        public const string LoadedDocumentsTotalWeightPropertyName = "LoadedDocumentsTotalWeight";
        private decimal _loadedDocumentsTotalWeight = 0;
        public decimal LoadedDocumentsTotalWeight
        {
            get
            {
                return _loadedDocumentsTotalWeight;
            }

            set
            {
                if (_loadedDocumentsTotalWeight == value)
                {
                    return;
                }

                RaisePropertyChanging(LoadedDocumentsTotalWeightPropertyName);
                _loadedDocumentsTotalWeight = value;
                RaisePropertyChanged(LoadedDocumentsTotalWeightPropertyName);
            }
        }
         
        public const string SearchTextPropertyName = "SearchText";
        private string _searchText = "";
        public string SearchText
        {
            get
            {
                return _searchText;
            }

            set
            {
                if (_searchText == value)
                {
                    return;
                }

                RaisePropertyChanging(SearchTextPropertyName);
                _searchText = value;
                RaisePropertyChanged(SearchTextPropertyName);
                Search();
            }
        }

        public const string PageProgressPropertyName = "PageProgress";
        private string _pageProgress = "";
        public string PageProgress
        {
            get
            {
                return _pageProgress;
            }

            set
            {
                if (_pageProgress == value)
                {
                    return;
                }

                RaisePropertyChanging(PageProgressPropertyName);
                _pageProgress = value;
                RaisePropertyChanged(PageProgressPropertyName);
            }
        }
         
        public const string DocumentReferencePropertyName = "DocumentReference";
        private string _documentReference = "";
        public string DocumentReference
        {
            get
            {
                return _documentReference;
            }

            set
            {
                if (_documentReference == value)
                {
                    return;
                }

                RaisePropertyChanging(DocumentReferencePropertyName);
                _documentReference = value;
                RaisePropertyChanged(DocumentReferencePropertyName);
            }
        }

        private User currentUser;
        private CostCentre currentCostCenter;
        private Guid currentCostCentreAppId;

        Store _defaultStore = null;

        private Store DefaultStore
        {
            get
            {
                if (_defaultStore == null)
                    _defaultStore = new Store(Guid.Empty) {Name = "--Select store--"};
                return _defaultStore;
            }
        }

        #endregion

        #region Methods

        private void ViewSelectedItem(CommodityReceptionNote document)
        {
            const string uri = "/views/CommodityReception/DocumentDetails.xaml";
            string thisUrl = "/views/CommodityReception/StoreCommodity.xaml";
            Messenger.Default.Send<DocumentDetailMessage>(new DocumentDetailMessage { Id = document.Id, DocumentType = DocumentType.CommodityReceptionNote, MessageSourceUrl = thisUrl });

            NavigateCommand.Execute(uri);

            //Go=>todo:we need to close this window,view details,..then on back=return here...?
            this.RequestClose(this, EventArgs.Empty);
        }


        private void Cancel()
        {
            if (
                MessageBox.Show("Are you sure you want top cancel this process? Unsaved changes will be lost.",
                                "Agrimanagr: Store Commodity", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) ==
                MessageBoxResult.Yes)
            {
                this.RequestClose(this, EventArgs.Empty);
            }
        }

        public void ClearViewModel()
        {
            SelectedStore = null;
            SelectedLoadedDocument = null;
            LoadedDocumentsTotalWeight = 0;
            SearchText = string.Empty;

            LoadedDocuments.Clear();
            LoadedDocumentsLineItemsList.Clear();
            LoadedDocumentsLineItemsListCache.Clear();
            LineItemsToStoreList.Clear();
            StoreList.Clear();
            ItemsToStore.Clear();
        }

        public void GetItemsToStore(List<Guid> items)
        {
            if(ItemsToStore.Any())
                ItemsToStore.Clear();
            items.ForEach(ItemsToStore.Add);

        }
        private void DropDownOpened(object sender)
        {
           ComboBox comboBox = sender as ComboBox;
           switch (comboBox.Name)
           {
               case "cmbStore":
                   using (var container = NestedContainer)
                   {
                       SelectedStore = Using<IAgrimanagrComboPopUp>(container).ShowDlg(sender) as Store;
                      
                   }
                   break;
           }
        }
        public void SetUp()
        {
            ClearViewModel();
            Config config = GetConfigParams();
            currentUser = GetEntityById(typeof(User), GetConfigViewModelParameters().CurrentUserId) as User;
            currentCostCenter = GetEntityById(typeof(CostCentre), config.CostCentreId) as CostCentre;
            currentCostCentreAppId = config.CostCentreApplicationId;
            using (StructureMap.IContainer c = NestedContainer)
            {
               DocumentReference=  Using<IGetDocumentReference>(c).GetDocReference("StorageNote", currentCostCenter.Id, currentCostCenter.Id);
            }
          
            LoadStores();
            StoreList.Add(DefaultStore);
            SelectedStore = DefaultStore;
            
        }

        private void LoadStorageWindow()
        {
            
            foreach (var documentId in ItemsToStore)
            {
                LoadedDocuments.Add(
                    GetEntityById(typeof(ReceivedDeliveryNote), documentId) as ReceivedDeliveryNote);
            }
            if(LoadedDocuments.Any())
            {
                LoadedDocumentsCollectionChanged();
                Search();
            }
            
        }
       
        private void LoadStores()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                var stores = Using<IStoreRepository>(c).GetAll().ToList();
                stores.OrderBy(n => n.Name).ThenBy(n => n.CostCentreCode).ToList()
                    .ForEach(n => { if (StoreList.Select(q => q.Id).All(p => p != n.Id)) StoreList.Add(n as Store); });
            }
        }

        private void LoadedDocumentsCollectionChanged()
        {
            SelectedLoadedDocument = LoadedDocuments.First();
            LoadedDocumentsTotalWeight = LoadedDocuments.Sum(n => n.TotalNetWeight);
            foreach (var item in LoadedDocuments)
            {
                using (StructureMap.IContainer c = NestedContainer)
                {
                    var doc = Using<IReceivedDeliveryRepository>(c).GetById(item.Id) as ReceivedDeliveryNote;
                    if (doc != null)
                        foreach (var lineitem in doc.LineItems.Where(p=>p.LineItemStatus !=SourcingLineItemStatus.Stored))
                        {
                           LoadedDocumentsLineItemsListCache.Add(Map(lineitem,doc.Id));
                        }
                }
            }
        }

        private CommodyLineItemViewModel Map(ReceivedDeliveryLineItem lineItem,Guid documentid)
        {

            var item = new CommodyLineItemViewModel
                           {
                               Id = lineItem.Id,
                               Commodity = lineItem.Commodity??lineItem.CommodityGrade.Commodity,
                               Description = lineItem.Description,
                               ContainerNo = lineItem.ContainerNo,
                               CommodityGrade = lineItem.CommodityGrade,
                               ParentLineItemId = lineItem.ParentDocId,
                               NetWeight =lineItem.Weight,
                               DocumentId = documentid,
                               ContainerType = lineItem.ContainerType,
                             // NoOfContainers = lineItem.

                           };
            return item;
        }

        private void Search()
        {
            SearchText = SearchText.Trim();
            LoadedDocumentsLineItemsList.Clear();

            if (SearchText == "")
            {
                foreach (var item in LoadedDocumentsLineItemsListCache.OrderBy(n => n.ContainerNo).ThenBy(n => n.Commodity.Name))
                {
                    LoadedDocumentsLineItemsList.Add(item);
                }
            }
            else
            {
                var items = LoadedDocumentsLineItemsListCache
                    .Where(n =>
                           n.Commodity.Name.ToLower().Contains(SearchText.ToLower()) ||
                           n.Description.ToLower().Contains(SearchText.ToLower()) ||
                           n.CommodityGrade.Name.ToLower().Contains(SearchText.ToLower()) ||
                           n.ContainerNo.ToLower().Contains(SearchText.ToLower())
                    ).OrderBy(n => n.ContainerNo).ThenBy(n => n.Commodity.Name);
                foreach (var item in items)
                {
                    LoadedDocumentsLineItemsList.Add(item);
                }
            }
            LoadedDocumentsTotalWeight = LoadedDocumentsLineItemsList.Sum(n => n.NetWeight);
        }

        void AssignAll()
        {
            Warn(LoadedDocumentsLineItemsList.Count(p => p.NetWeight <= 0));
            if (!ValidateStoreBeforeAssign()) return;
            foreach (var item in LoadedDocumentsLineItemsList.Where(n=>n.NetWeight > 0))
            {
                if (LineItemsToStoreList.All(n => n.Id != item.Id))
                {
                    LineItemsToStoreList.Add(item);
                    item.IsSelected = false;
                }
            }

            LoadedDocumentsLineItemsList.ToList()
                                        .ForEach(n =>
                                                     {
                                                         var item =
                                                             LoadedDocumentsLineItemsListCache.FirstOrDefault(
                                                                 q => q.Id == n.Id);
                                                         if (item != null)
                                                             LoadedDocumentsLineItemsListCache.Remove(item);
                                                     });
            AssignmentChanged();
        }

        void AssignSelected()
        {
            Warn(LoadedDocumentsLineItemsList.Count(p => p.NetWeight <= 0));
            if(!ValidateStoreBeforeAssign())return;
            foreach (var item in LoadedDocumentsLineItemsList.Where(n => n.IsSelected && n.NetWeight > 0))
            {  
                if (LineItemsToStoreList.All(n => n.Id != item.Id ))
                {
                    LineItemsToStoreList.Add(item);
                    item.IsSelected = false;
                }
            }
            foreach (var item in LineItemsToStoreList)
            {
                var remove = LoadedDocumentsLineItemsListCache.FirstOrDefault(n => n.Id == item.Id);
                if (remove != null)
                {
                    LoadedDocumentsLineItemsListCache.Remove(item);
                }
            }
            AssignmentChanged();
        }
        void Warn(int invalid)
        {
            if(invalid>0)
            MessageBox.Show(string.Format("{0}-Invalid items not assigned for storage",invalid));
        }

        void UnAssignAll()
        {
            foreach (var item in LineItemsToStoreList)
            {
                if (LoadedDocumentsLineItemsListCache.All(n => n.Id != item.Id))
                {
                    LoadedDocumentsLineItemsListCache.Add(item);
                    item.IsSelected = false;
                }
            }
            LineItemsToStoreList.Clear();
            SelectedStore = DefaultStore;
            AssignmentChanged();
        }

        void UnassignSelected()
        {
            foreach (var item in LineItemsToStoreList.Where(n => n.IsSelected))
            {
                if (LoadedDocumentsLineItemsListCache.All(n => n.Id != item.Id))
                {
                    LoadedDocumentsLineItemsListCache.Add(item);
                    item.IsSelected = false;
                }
            }
            foreach (var item in LoadedDocumentsLineItemsListCache)
            {
                var remove = LineItemsToStoreList.FirstOrDefault(n => n.Id == item.Id);
                if (remove != null)
                    LineItemsToStoreList.Remove(item);
            }
            AssignmentChanged();
        }

        void AssignmentChanged()
        {
            Search();
            ItemsToStoreTotalWeight = LineItemsToStoreList.Sum(n => n.NetWeight);
        }

       bool ValidateStoreBeforeAssign()
        {
            if(SelectedStore==null ||SelectedStore==DefaultStore)
            {
                MessageBox.Show("Select a store for storage first", "Agrimanagr Warning", MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return false;
            }
           return true;
        }

        private void StoreItems()
        {

            if (!LineItemsToStoreList.Any())
            {
                MessageBox.Show("Please select items to store");
                return;
            }
   
            if (!IsValid()) return;
            
           try
                {
          
                    using (StructureMap.IContainer c = NestedContainer)
                    {

                        var DocumentRecipientCostCentre =
                            GetEntityById(typeof (CostCentre), SelectedStore.Id) as CostCentre;

                        DateTime now = DateTime.Now;
                        var commodityStorageWfManager = Using<ICommodityStorageWFManager>(c);
                        var commodityStorageNoteFactory = Using<ICommodityStorageNoteFactory>(c);

                        var commodityStorageNote = commodityStorageNoteFactory.Create(currentCostCenter,
                                                                                      currentCostCentreAppId,
                                                                                      DocumentRecipientCostCentre,
                                                                                      currentUser,
                                                                                      DocumentReference, Guid.Empty,
                                                                                      now, now,
                                                                                      "Commodity storage note");

                        foreach (var item in LineItemsToStoreList)
                        {
                            var lineItem = commodityStorageNoteFactory.CreateLineItem(item.Id,
                                                                                      item.Commodity.Id,
                                                                                      item.CommodityGrade.Id,
                                                                                      Guid.Empty,
                                                                                      item.ContainerNo, item.NetWeight,
                                                                                      item.Description);
                            if(lineItem.CommodityGrade==null)
                            {
                                MessageBox.Show(string.Format("Incorrect grade: {0} signed for product :{1}\n Storage failed",
                                                              item.CommodityGrade.Name, item.Commodity.Name),"Agrimanagr Error");
                                return;
                            }
                            commodityStorageNote.AddLineItem(lineItem);
           
                        
                        
                        }

                        commodityStorageNote.Confirm();
                        commodityStorageWfManager.SubmitChanges(commodityStorageNote);

                        foreach (var item in LineItemsToStoreList.GroupBy(p=>p.DocumentId))
                        {
                            //var docreception = Using<ICommodityReceptionRepository>(c).GetById(item.Key) as CommodityReceptionNote;
                            //foreach (var confirmeditem in docreception.LineItems.Where(p => p.LineItemStatus == SourcingLineItemStatus.Confirmed))
                            //{
                            //    if (item.Any(p => p.Id == confirmeditem.Id))
                            //        docreception.MarkAsStoredLineItem(confirmeditem);
                            //}
                            //Using<ICommodityReceptionWFManager>(c).SubmitChanges(docreception);

                            var docreception = Using<IReceivedDeliveryRepository>(c).GetById(item.Key) as ReceivedDeliveryNote;
                            foreach (var confirmeditem in docreception.LineItems.Where(p => p.LineItemStatus != SourcingLineItemStatus.Stored))
                            {
                                if (item.Any(p => p.Id == confirmeditem.Id))
                                    docreception.MarkAsStoredLineItem(confirmeditem);
                            }
                            Using<IReceivedDeliveryWorkflow>(c).SubmitChanges(docreception);
                            

                        }


                        string msg = "Purchase successfully made and received. Transaction number: " +
                                     commodityStorageNote.DocumentReference;
                        MessageBox.Show(msg, "Agrimanagr: Purchase " + commodityStorageNote.DocumentReference,
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Information);
                        AddLogEntry("Commodity Storage", msg);
                        this.RequestClose(this, EventArgs.Empty);
                    }
                    //scope.Complete();
                }
                catch (Exception e)
                {
                    string error = "Error occured while saving transaction.\n" + e.Message +
                                   (e.InnerException == null ? "" : e.InnerException.Message);
                    MessageBox.Show(error
                       , "Agrimanagr: Purchase Commodity",
                       MessageBoxButton.OK, MessageBoxImage.Error);
                    _logger.Info("Commodity Storage: " + error);
               
                }
           
          
        }

      
        #endregion

    }
}
