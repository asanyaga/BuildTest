using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms.VisualStyles;
using Distributr.Core.ClientApp;
using Distributr.Core.Domain.InventoryEntities;
using Distributr.Core.Domain.Master.CommodityEntities;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities.LineItems;
using Distributr.Core.Factory.SourcingDocuments;
using Distributr.Core.Repository.InventoryRepository;
using Distributr.Core.Repository.Master.CommodityRepositories;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Transactional.SourcingDocumentRepositories;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.ViewModels.Utils;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Distributr.WPF.Lib.ViewModels.Transactional.InventoryTransfer
{
    public abstract class InventoryTransferBaseViewModel : DistributrViewModelBase
    {
        public ObservableCollection<InventoryTransitionLineItem> LineItem { get; set; }
        public ObservableCollection<InventoryTransitionLineItem> LineItemToTransfer { get; set; }
        public ObservableCollection<Commodity> CommodityLookUpList { get; set; }
        public ObservableCollection<CommodityGrade> GradeLookUpList { get; set; }
        public ObservableCollection<Store> StoreLookUpList { get; set; }
        
        public RelayCommand SelectedGradeChangedCommand { get; set; }
        public RelayCommand SelectedCommodityChangedCommand { get; set; }
        public RelayCommand SelectedStoreChangedCommand { get; set; }
        public RelayCommand AddBatchCommand { get; set; }
        public RelayCommand ReWeighCommand { get; set; }
        public RelayCommand TransferInventoryCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        public RelayCommand ItemSelectionChangedCommand { get; set; }
        public RelayCommand ItemChangedCommand { get; set; }

        public RelayCommand SelectedStoreToChangedCommand { get; set; }

        protected InventoryTransferBaseViewModel()
        {
            AddBatchCommand = new RelayCommand(AddBatch);
            ReWeighCommand = new RelayCommand(ReWeigh);
            TransferInventoryCommand = new RelayCommand(TransferInventory);
            CancelCommand = new RelayCommand(Cancel);
            SelectedCommodityChangedCommand = new RelayCommand(SelectedCommodityChanged);
            SelectedStoreChangedCommand = new RelayCommand(SelectedStoreChanged);
            SelectedGradeChangedCommand = new RelayCommand(SelectedGradeChanged);
            ItemSelectionChangedCommand = new RelayCommand(ItemSelectionChanged);
            ItemChangedCommand = new RelayCommand(ItemChanged);

            SelectedStoreToChangedCommand = new RelayCommand(SelectedStoreToChanged);

            LineItem = new ObservableCollection<InventoryTransitionLineItem>();
            LineItemToTransfer = new ObservableCollection<InventoryTransitionLineItem>();
            CommodityLookUpList = new ObservableCollection<Commodity>();
            GradeLookUpList = new ObservableCollection<CommodityGrade>();
            StoreLookUpList = new ObservableCollection<Store>();
            

            WeightToMove = 0;
            AvailableWeight = 0;
        }

        #region methods

        protected abstract void Cancel();

        

        protected abstract void TransferInventory();
        
        

        private void AddBatch()
        {
            if (SelectedCommodity == null || SelectedCommodity.Id.Equals(Guid.Empty) 
                || SelectedGrade == null || SelectedGrade.Id.Equals(Guid.Empty) 
                ||SelectedStoreFrom == null || SelectedStoreFrom.Id.Equals(Guid.Empty))
            {
                MessageBox.Show("No Items Selected to Transfer");
                return;
            }

            var temp = new List<InventoryTransitionLineItem>();
            decimal totalTemp = 0;
            foreach(var item in LineItem)
            {
                if(item.IsChecked)
                {
                    if (IsWeighed)
                    {
                        item.Weight = Weight;
                        totalTemp += Weight;
                        Weight = 0;
                        IsWeighed = false;
                    }
                    else
                    {
                        totalTemp += item.Weight;
                    }
                    LineItemToTransfer.Add(item);
                    temp.Add(item);
                }
            }

            foreach (var inventoryTransitionLineItem in temp)
            {
                LineItem.Remove(inventoryTransitionLineItem);
            }
            AvailableWeight -= totalTemp;
            WeightToMove += totalTemp;
            IsSelected = false;
        }

        private void ReWeigh()
        {
            var scale = WeighConfiguration.Load();
            string msg = "";

            if (scale == null)
            {
                MessageBox.Show("No weigh scale device not found ,Please Setup Device first", "Agrimanagr Info",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            var selected = LineItem.Count(l => l.IsChecked);
            if (selected != 1)
            {
                MessageBox.Show("Select ONLY ONE line item to weigh");
                Weight = 0;
                IsWeighed = false;
                return;
            }
            if (SerialPortHelper.IsDeviceReady)
            {

                Weight = SerialPortHelper.Read();


            }
            else if (SerialPortHelper.Init(out msg, scale.WeighScaleType, scale.Port,
                scale.BaudRate, scale.DataBits))
            {
                Weight = SerialPortHelper.Read();

            }
            else
            {
                AddLogEntry("Warehousing Weigh", "Error- " + msg);
                MessageBox.Show("The weight could not be recorded,check the audit log and make sure weighing scale is setup properly", "Agrimanagr Info", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            IsWeighed = Weight != 0;
        }

        private void ItemChanged()
        {
            if (!IsSelected) return;
            if (LineItem.Any(n => n.IsChecked == false))
            {
                IsSelected = false;
            }
        }

        private void ItemSelectionChanged()
        {
            //var temp = new List<InventoryTransitionLineItem>();
            if(IsSelected)
            {
                foreach (var item in LineItem)
                {
                    item.IsChecked = true;
                }
            }else
            {
                if (LineItem.Any(n => n.IsChecked == false)) return;
                foreach (var item in LineItem)
                {
                    item.IsChecked = false;
                }
            }
        }
        
        private void SelectedCommodityChanged()
        {
            LoadGrade();
        }

        private void SelectedStoreChanged()
        {
            LoadCommodity();
            //TODO: mark as not enebled
        }

        private void SelectedStoreToChanged()
        {
            //TODO: mark as not enebled
        }
        
        private void SelectedGradeChanged()
        {
            if (SelectedStoreFrom == null || SelectedStoreFrom.Id.Equals(Guid.Empty))
            {
               // MessageBox.Show("Store is not Selected !");
                return;
            }
            if (SelectedCommodity == null || SelectedCommodity.Id.Equals(Guid.Empty))
            {
               // MessageBox.Show("Commodity is not Selected !");
                return;
            }
            if (SelectedGrade == null || SelectedGrade.Id.Equals(Guid.Empty))
            {
               // MessageBox.Show("Grade is not Selected !");
                return;
            }
            
            LineItem.Clear();
            IsSelected = false;
            using (var c = NestedContainer)
            {
                var commodityStorage = Using<ICommodityStorageRepository>(c).GetAll();
                foreach (CommodityStorageNote sourcingDocument in commodityStorage
                    .Where(n => n.DocumentRecipientCostCentre.Id.Equals(SelectedStoreFrom.Id)))
                {
                    foreach (var items in sourcingDocument.LineItems)
                    {
                        if (items.Commodity.Id.Equals(SelectedCommodity.Id) &&
                             items.CommodityGrade.Id.Equals(SelectedGrade.Id))
                        {
                            if (items.LineItemStatus.Equals(SourcingLineItemStatus.Transfered)) continue;
                            var item = new InventoryTransitionLineItem()
                                           {
                                               BatchNumber = items.ContainerNo,
                                               Commodity = items.Commodity,
                                               Grade = items.CommodityGrade,
                                               IsChecked = false,
                                               StorageLineItem = items.Id,
                                               Weight = items.Weight,
                                               
                                           };
                            var t = LineItemToTransfer.All(trans => !trans.StorageLineItem.Equals(items.Id));
                            if(t)
                            {
                                LineItem.Add(item);
                                AvailableWeight += item.Weight;
                            }
                        }
                    }
                }
            }
        }

        public void SetUp()
        {
            ClearDetails();
            LoadStores();
        }

        public void ClearDetails()
        {
            WeightToMove = 0;
            AvailableWeight = 0;
            LineItemToTransfer.Clear();
            LineItem.Clear();
            SelectedStoreFrom = StoreLookUpList.FirstOrDefault();
        }

        protected void LoadStores()
        {
            StoreLookUpList.Clear();
            StoreLookUpList.Add(new Store(Guid.Empty)
            {
                Name = "-- Select Store --"
            });
            using (var c = NestedContainer)
            {
                var inventory = Using<ISourcingInventoryRepository>(c).GetAll();
                foreach (var sourcingInventory in inventory)
                {
                    var store = Using<IStoreRepository>(c).GetById(sourcingInventory.Warehouse.Id) as Store;
                    if (store == null) continue;
                    if(!StoreLookUpList.Contains(store))
                    {
                        StoreLookUpList.Add(store);
                    }

                    var commodityTransfer = Using<ICommodityTransferRepository>(c).GetAll();
                    foreach (CommodityTransferNote transferNote in commodityTransfer)
                    {
                        if (!transferNote.TransferNoteTypeId.Equals(CommodityTransferNote.CommodityTransferNoteTypeId.ToOtherStore)) continue;
                        var store1 = Using<IStoreRepository>(c).GetById(transferNote.DocumentRecipientCostCentre.Id) as Store;
                        if (store1 == null) continue;
                        if (!StoreLookUpList.Contains(store1))
                            {
                                StoreLookUpList.Add(store1);
                            }
                    }
                        
                }
            }
            SelectedStoreFrom = StoreLookUpList.FirstOrDefault();
        }

        private void LoadCommodity()
        {
            CommodityLookUpList.Clear();
            CommodityLookUpList.Add(new Commodity(Guid.Empty)
            {
                Name = "-- Select Commodity --",
            });
            using (var c = NestedContainer)
            {
                if (SelectedStoreFrom != null && SelectedStoreFrom.Id != Guid.Empty)
                {
                    var commodityStorage = Using<ICommodityStorageRepository>(c).GetAll();
                    foreach (CommodityStorageNote sourcingDocument in commodityStorage.Where(n => n.DocumentRecipientCostCentre.Id.Equals(SelectedStoreFrom.Id)))
                    {
                        foreach (var source in sourcingDocument.LineItems.Where(document => !document.LineItemStatus.Equals(SourcingLineItemStatus.Transfered))
                            .Select(n => n.Commodity.Id))
                        {
                            var item = Using<ICommodityRepository>(c).GetById(source);
                            if (!CommodityLookUpList.Contains(item))
                            {
                                CommodityLookUpList.Add(item);
                            }
                        }
                    }

                }
            }
            SelectedCommodity = CommodityLookUpList.FirstOrDefault();
        }

        private void LoadGrade()
        {
            GradeLookUpList.Clear();
            GradeLookUpList.Add(new CommodityGrade(Guid.Empty)
            {
                Name = "-- Select Grade --",
            });
            using (var c = NestedContainer)
            {
                if (SelectedStoreFrom != null && SelectedStoreFrom.Id != Guid.Empty
                    && SelectedCommodity != null && SelectedCommodity.Id != Guid.Empty)
                {
                    var commodityStorage = Using<ICommodityStorageRepository>(c).GetAll();
                    
                    foreach (var source in commodityStorage.Where(n => n.DocumentRecipientCostCentre.Id.Equals(SelectedStoreFrom.Id))
                        .Cast<CommodityStorageNote>().SelectMany(sourcingDocument => sourcingDocument.LineItems
                            .Where(document => !document.LineItemStatus.Equals(SourcingLineItemStatus.Transfered) && document.Commodity.Id.Equals(SelectedCommodity.Id)).Select(n => n.CommodityGrade.Id).Distinct()))
                    {
                        var item = Using<ICommodityRepository>(c).GetGradeByGradeId(source);
                        if (!GradeLookUpList.Contains(item))
                        {
                            GradeLookUpList.Add(item);
                        }
                        
                    }
                }
            }
            SelectedGrade = GradeLookUpList.FirstOrDefault();
        }

        protected string GetDocumentReference(string docRef, CostCentre issuerCostCentre)
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
                docRef = Using<IGetDocumentReference>(cont)
                    .GetDocReference(docRef, issuerCostCentre.Id, issuerCostCentre.Id);
            }
            return docRef;
        }
        
        #endregion

        #region properties
        
        public const string SelectedCommodityPropertyName = "SelectedCommodity";
        private Commodity _selectedCommodity;
        public Commodity SelectedCommodity
        {
            get
            {
                return _selectedCommodity;
            }

            set
            {
                if (_selectedCommodity == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedCommodityPropertyName);
                _selectedCommodity = value;
                RaisePropertyChanged(SelectedCommodityPropertyName);


            }
        }

        public const string SelectedGradePropertyName = "Grade";
        private CommodityGrade _selectedGrade = null;
        public CommodityGrade SelectedGrade
        {
            get
            {
                return _selectedGrade;
            }

            set
            {
                if (_selectedGrade == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedGradePropertyName);
                _selectedGrade = value;
                RaisePropertyChanged(SelectedGradePropertyName);
            }
        }

        public const string SelectedStoreFromPropertyName = "SelectedStoreFrom";
        private Store _selectedtStoreFrom = null;
        public Store SelectedStoreFrom
        {
            get
            {
                return _selectedtStoreFrom;
            }

            set
            {
                if (_selectedtStoreFrom == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedStoreFromPropertyName);
                _selectedtStoreFrom = value;
                RaisePropertyChanged(SelectedStoreFromPropertyName);
            }
        }
        
        public const string DocumentIdPropertyName = "DocumentId";
        private Guid _document = Guid.Empty;
        public Guid DocumentId
        {
            get
            {
                return _document;
            }

            set
            {
                if (_document == value)
                {
                    return;
                }

                RaisePropertyChanging(DocumentIdPropertyName);
                _document = value;
                RaisePropertyChanged(DocumentIdPropertyName);
            }
        }

        public const string AvailableWeightPropertyName = "AvailableWeight";
        private decimal _availableWeight;
        public decimal AvailableWeight
        {
            get
            {
                return _availableWeight;
            }

            set
            {
                if (_availableWeight == value)
                {
                    return;
                }

                RaisePropertyChanging(AvailableWeightPropertyName);
                _availableWeight = value;
                RaisePropertyChanged(AvailableWeightPropertyName);


            }
        }

        public const string WeightToMovePropertyName = "WeightToMove";
        private decimal _weightToMove;
        public decimal WeightToMove
        {
            get
            {
                return _weightToMove;
            }

            set
            {
                if (_weightToMove == value)
                {
                    return;
                }

                RaisePropertyChanging(WeightToMovePropertyName);
                _weightToMove = value;
                RaisePropertyChanged(WeightToMovePropertyName);


            }
        }

        public const string IsSelectedPropertyName = "IsSelected";
        private bool _isSelected = false;
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }

            set
            {
                if (_isSelected == value)
                {
                    return;
                }

                RaisePropertyChanging(IsSelectedPropertyName);
                _isSelected = value;
                RaisePropertyChanged(IsSelectedPropertyName);
            }
        }

        public const string IsWeighedPropertyName = "IsWeighed";

        private bool _isWeighed = false;
        public bool IsWeighed
        {
            get
            {
                return _isWeighed;
            }

            set
            {
                if (_isWeighed == value)
                {
                    return;
                }

                RaisePropertyChanging(IsWeighedPropertyName);
                _isWeighed = value;
                RaisePropertyChanged(IsWeighedPropertyName);
            }
        }

        public const string WeightPropertyName = "Weight";
        private decimal _weight;
        public decimal Weight
        {
            get
            {
                return _weight;
            }

            set
            {
                if (_weight == value)
                {
                    return;
                }

                RaisePropertyChanging(WeightPropertyName);
                _weight = value;
                RaisePropertyChanged(WeightPropertyName);
            }
        }
        #endregion

        public class InventoryTransitionLineItem : ViewModelBase
        {
            public const string GradePropertyName = "Grade";
            private CommodityGrade _grade = null;
            public CommodityGrade Grade
            {
                get
                {
                    return _grade;
                }

                set
                {
                    if (_grade == value)
                    {
                        return;
                    }

                    RaisePropertyChanging(GradePropertyName);
                    _grade = value;
                    RaisePropertyChanged(GradePropertyName);
                }
            }

            public const string CommodityPropertyName = "Commodity";
            private Commodity _commodity = null;
            public Commodity Commodity
            {
                get
                {
                    return _commodity;
                }

                set
                {
                    if (_commodity == value)
                    {
                        return;
                    }

                    RaisePropertyChanging(CommodityPropertyName);
                    _commodity = value;
                    RaisePropertyChanged(CommodityPropertyName);
                }
            }

            public const string WeightPropertyName = "Weight";
            private decimal _weight = 0;
            public decimal Weight
            {
                get
                {
                    return _weight;
                }

                set
                {
                    if (_weight == value)
                    {
                        return;
                    }

                    RaisePropertyChanging(WeightPropertyName);
                    _weight = value;
                    RaisePropertyChanged(WeightPropertyName);
                }
            }

            public const string BatchNumberPropertyName = "BatchNumber";
            private string _batchNumber = "";
            public string BatchNumber
            {
                get
                {
                    return _batchNumber;
                }

                set
                {
                    if (_batchNumber == value)
                    {
                        return;
                    }

                    RaisePropertyChanging(BatchNumberPropertyName);
                    _batchNumber = value;
                    RaisePropertyChanged(BatchNumberPropertyName);
                }
            }

            public const string IsCheckedPropertyName = "IsChecked";
            private bool _isChecked = false;
            public bool IsChecked
            {
                get
                {
                    return _isChecked;
                }

                set
                {
                    if (_isChecked == value)
                    {
                        return;
                    }

                    RaisePropertyChanging(IsCheckedPropertyName);
                    _isChecked = value;
                    RaisePropertyChanged(IsCheckedPropertyName);
                }
            }

            public const string StorageLineItemPropertyName = "StorageLineItem";
            private Guid _storageLineItem = Guid.Empty;
            public Guid StorageLineItem
            {
                get
                {
                    return _storageLineItem;
                }

                set
                {
                    if (_storageLineItem == value)
                    {
                        return;
                    }

                    RaisePropertyChanging(StorageLineItemPropertyName);
                    _storageLineItem = value;
                    RaisePropertyChanged(StorageLineItemPropertyName);
                }
            }

      
        }

        
    }
}
