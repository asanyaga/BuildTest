using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Distributr.Core.Domain.Master.CommodityEntities;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities.LineItems;
using Distributr.Core.Factory.SourcingDocuments;
using Distributr.Core.Repository.Master.CommodityRepositories;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Transactional.SourcingDocumentRepositories;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.Services.WorkFlow.GetDocumentReferences;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Distributr.WPF.Lib.ViewModels.Transactional.InventoryTransfer
{
    public class InventoryTransferViewModelPopUp : DistributrViewModelBase
    {
        public ObservableCollection<InventoryTransition> LineItem { get; set; }
        public ObservableCollection<InventoryTransition> LineItemToTransfer { get; set; }
        public ObservableCollection<Commodity> CommodityLookUpList { get; set; }
        public ObservableCollection<CommodityGrade> GradeLookUpList { get; set; }
        public ObservableCollection<Store> StoreLookUpList { get; set; }

        public List<Guid> StorageItems { get; set; }

        public RelayCommand SelectedGradeChangedCommand { get; set; }
        public RelayCommand SelectedCommodityChangedCommand { get; set; }
        public RelayCommand SelectedStoreChangedCommand { get; set; }
        public RelayCommand AddBatchCommand { get; set; }
        public RelayCommand TransferInventoryCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        public RelayCommand ItemSelectionChangedCommand { get; set; }

        public event EventHandler RequestClose = (s, e) => { };

        public InventoryTransferViewModelPopUp()
        {
            AddBatchCommand = new RelayCommand(AddBatch);
            TransferInventoryCommand = new RelayCommand(TransferInvrnyory);
            CancelCommand = new RelayCommand(Cancel);
            SelectedCommodityChangedCommand = new RelayCommand(SelectedCommodityChanged);
            SelectedStoreChangedCommand = new RelayCommand(SelectedStoreChanged);
            SelectedGradeChangedCommand = new RelayCommand(SelectedGradeChanged);
            ItemSelectionChangedCommand = new RelayCommand(ItemSelectionChanged);

            LineItem = new ObservableCollection<InventoryTransition>();
            LineItemToTransfer = new ObservableCollection<InventoryTransition>();
            CommodityLookUpList = new ObservableCollection<Commodity>();
            GradeLookUpList = new ObservableCollection<CommodityGrade>();
            StoreLookUpList = new ObservableCollection<Store>();

            StorageItems = new List<Guid>();
        }

        private void SelectedCommodityChanged()
        {
            LoadGrade();
        }

        private void SelectedStoreChanged()
        {
            LoadCommodity();
        }

        private void LoadStore()
        {
            StoreLookUpList.Clear();
            StoreLookUpList.Add(new Store(Guid.Empty)
                                    {
                                        Name = "-- Select Store --"
                                    });
            using (var c = NestedContainer)
            {
                var stores = Using<IStoreRepository>(c).GetAll();
                foreach(Store store in stores)
                {
                    StoreLookUpList.Add(store);
                }
            }
            SelectedStore = StoreLookUpList.FirstOrDefault();
        }

        private void SelectedGradeChanged()
        {
            LineItem.Clear();
            using (var c = NestedContainer)
            {
                if (SelectedStore == null || SelectedStore.Id == Guid.Empty || SelectedCommodity == null ||
                    SelectedCommodity.Id == Guid.Empty || SelectedGrade == null || SelectedGrade.Id == Guid.Empty)
                    return;
                var commodityStorage = Using<ICommodityStorageRepository>(c).GetAll()
                    .Where(n => n.DocumentRecipientCostCentre.Id.Equals(SelectedStore.Id)).ToList();
                foreach (var item in commodityStorage.Cast<CommodityStorageNote>()
                    .Select(sourcingDocument => sourcingDocument.LineItems
                        .Where(n => n.Commodity.Id == SelectedCommodity.Id && n.CommodityGrade.Id == SelectedGrade.Id
                        && !n.LineItemStatus.Equals(SourcingLineItemStatus.Transfered)).ToList())
                    .SelectMany(items => items))
                {
                    LineItem.Add(new InventoryTransition()
                                     {
                                         BatchNumber = ""+(LineItem.Count + 1),
                                         Commodity = item.Commodity,
                                         Grade = item.CommodityGrade,
                                         Weight = item.Weight,
                                         StorageLineItem = item.Id
                                     });
                }
            }
        }

        public void SetUp()
        {
            ClearDetails();
            LoadStore();
        }

        public void ClearDetails()
        {
            LineItem.Clear();
            LineItemToTransfer.Clear();
        }

        private void LoadCommodity()
        {
            CommodityLookUpList.Clear();
            CommodityLookUpList.Add(new Commodity(Guid.Empty)
            {
                Name = "--Select Commodity--",
            });
            using (var c = NestedContainer)
            {
                if (SelectedStore != null && SelectedStore.Id != Guid.Empty)
                {
                    var commodityStorage = Using<ICommodityStorageRepository>(c).GetAll();//.GetById(SelectedStore.Id);
                    foreach (CommodityStorageNote sourcingDocument in commodityStorage.Where(n => n.DocumentRecipientCostCentre.Id.Equals(SelectedStore.Id)))
                    {
                        foreach (var source in sourcingDocument.LineItems.Where(document => !document.LineItemStatus.Equals(SourcingLineItemStatus.Transfered))
                            .Select(n => n.Commodity.Id).Distinct())
                        {
                            CommodityLookUpList.Add(Using<ICommodityRepository>(c).GetById(source));
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
                if (SelectedStore != null && SelectedStore.Id != Guid.Empty
                    && SelectedCommodity != null && SelectedCommodity.Id != Guid.Empty)
                {
                    var commodityStorage = Using<ICommodityStorageRepository>(c).GetAll();
                    foreach (var source in commodityStorage.Where(n => n.DocumentRecipientCostCentre.Id.Equals(SelectedStore.Id))
                        .Cast<CommodityStorageNote>().SelectMany(sourcingDocument => sourcingDocument.LineItems
                            .Where(document => !document.LineItemStatus.Equals(SourcingLineItemStatus.Transfered))                                                                    .Select(n => n.CommodityGrade.Id).Distinct()))
                    {
                        GradeLookUpList.Add(Using<ICommodityRepository>(c).GetGradeByGradeId(source));
                    }
                }
            }
            SelectedGrade = GradeLookUpList.FirstOrDefault();
        }

        public class InventoryTransition : ViewModelBase
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

        private void AddBatch()
        {
            var temp = new List<InventoryTransition>();
            foreach (var lineItem in LineItem)
            {
                if (!lineItem.IsChecked) continue;
                var item = lineItem;
                
                var tr = LineItemToTransfer.FirstOrDefault(n => n.Grade.Id == item.Grade.Id);
                temp.Add(item);
                if (tr != null)
                {
                    var wt = tr.Weight;
                    LineItemToTransfer.Remove(tr);
                    tr.Weight = tr.Weight + lineItem.Weight;
                    LineItemToTransfer.Add(tr);
                    
                }else
                {
                    LineItemToTransfer.Add(new InventoryTransition()
                                               {
                                                   BatchNumber = "" + (LineItemToTransfer.Count + 1),
                                                   Commodity = SelectedCommodity,
                                                   Grade = SelectedGrade,
                                                   Weight = lineItem.Weight
                                               });
                }
                StorageItems.Add(lineItem.StorageLineItem);
            }

            foreach (var tempLineItem in temp)
            {
                LineItem.Remove(tempLineItem);
            }
        }

        private void ItemSelectionChanged()
        {

        }

        private void Cancel()
        {
            RequestClose(this, EventArgs.Empty);
        }

        private void TransferInvrnyory()
        {
            if(!LineItemToTransfer.Any())
            {
                MessageBox.Show("No Items Selected to Transfer");
                return;
            }
            using (var c = NestedContainer)
            {
                var workflowTransfer = Using<ICommodityTransferWFManager>(c);
                var configService = Using<IConfigService>(c);
                var config = configService.Load();
                var issuerCostCentre = Using<ICostCentreRepository>(c).GetById(config.CostCentreId);
                var user = Using<IUserRepository>(c).GetById(configService.ViewModelParameters.CurrentUserId);
                var producer = Using<IProducerRepository>(c).GetById(issuerCostCentre.ParentCostCentre.Id);
                

                CommodityTransferNote commodityTransferNote = null;
                var factory = Using<ICommodityTransferNoteFactory>(c);
                commodityTransferNote = factory.Create(issuerCostCentre, producer, config.CostCentreApplicationId, user,
                                GetDocumentReference("CommodityTransfer", issuerCostCentre), Guid.Empty,
                                DateTime.Now, DateTime.Now, "");
                foreach (var item in LineItemToTransfer)
                {
                    var lineitem = factory.CreateLineItem(Guid.Empty,
                                                          item.Grade != null ? item.Grade.Id : Guid.Empty,
                                                          item.Commodity.Id,
                                                          item.Weight, "");
                    commodityTransferNote.AddLineItem(lineitem);

                }
                commodityTransferNote.Confirm();
                
                var storageC = Using<ICommodityStorageRepository>(c);
                foreach (var id in StorageItems)
                {
                    var items = storageC.GetAll();
                    foreach (CommodityStorageNote sourcingDocument in items)
                    {
                        var id1 = id;
                        var temp = sourcingDocument.LineItems.Where(n => n.Id == id1);
                        if(temp.Any())
                        {
                            commodityTransferNote.MarkAsTransferedLineItem(temp.First());
                            /*sourcingDocument.LineItems.First(n => n.Id == id1).LineItemStatus =
                                SourcingLineItemStatus.Stored;
                            storageC.Save(sourcingDocument);*/
                        }

                    }
                }
                workflowTransfer.SubmitChanges(commodityTransferNote);
                MessageBox.Show("Commodity Transfer No. " + commodityTransferNote.DocumentReference + " saved successfully");
            }
            RequestClose(this, EventArgs.Empty);
        }

        string GetDocumentReference(string docRef, CostCentre issuerCostCentre)
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
                docRef = Using<IGetDocumentReference>(cont)
                    .GetDocReference(docRef, issuerCostCentre.Id, issuerCostCentre.Id);
            }
            return docRef;
        }

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
        private CommodityGrade __selectedGrade = null;
        public CommodityGrade SelectedGrade
        {
            get
            {
                return __selectedGrade;
            }

            set
            {
                if (__selectedGrade == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedGradePropertyName);
                __selectedGrade = value;
                RaisePropertyChanged(SelectedGradePropertyName);
            }
        }

        public const string SelectedStorePropertyName = "Store";
        private Store _selectedtore = null;
        public Store SelectedStore
        {
            get
            {
                return _selectedtore;
            }

            set
            {
                if (_selectedtore == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedGradePropertyName);
                _selectedtore = value;
                RaisePropertyChanged(SelectedGradePropertyName);
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

        #endregion
    }
}
