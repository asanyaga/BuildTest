using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.Core.Factory.SourcingDocuments;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.Services.Service.Utility;
using GalaSoft.MvvmLight.Command;

namespace Distributr.WPF.Lib.ViewModels.Transactional.InventoryTransfer
{
    public class IntraStoreTransferViewModel : InventoryTransferBaseViewModel
    {
        public ObservableCollection<Store> StoreLookUpListTo { get; set; }
        public IntraStoreTransferViewModel()
            : base()
        {
            StoreLookUpListTo = new ObservableCollection<Store>();
            LoadStoresTo();
            AddBatchCommand = new RelayCommand(AddBatch);
        }

        #region methods

        protected override void Cancel()
        {
            ClearDetails();
            SelectedStoreTo = StoreLookUpList.FirstOrDefault();
        }

        private void AddBatch()
        {
            if (SelectedCommodity == null || SelectedCommodity.Id.Equals(Guid.Empty)
                || SelectedGrade == null || SelectedGrade.Id.Equals(Guid.Empty)
                || SelectedStoreFrom == null || SelectedStoreFrom.Id.Equals(Guid.Empty))
            {
                MessageBox.Show("No Items Selected to Transfer");
                return;
            }

            if (SelectedStoreTo == null || SelectedStoreTo.Id.Equals(Guid.Empty))
            {
                MessageBox.Show("Please Select the Store to Transfer to");
                return;
            }

            var temp = new List<InventoryTransitionLineItem>();
            decimal totalTemp = 0;
            foreach (var item in LineItem)
            {
                if (item.IsChecked)
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

        
        protected override void TransferInventory()
        {
            if (!LineItemToTransfer.Any())
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
                try
                {
                    if (SelectedStoreTo == null || SelectedStoreTo.Id.Equals(Guid.Empty))
                    {
                        throw new Exception("Destination Store not selected");
                    }
                     commodityTransferNote = factory.Create(SelectedStoreFrom/*issuerCostCentre*/, SelectedStoreTo/*producer*/, config.CostCentreApplicationId, user,
                                        GetDocumentReference("CommodityTransfer", issuerCostCentre), Guid.Empty,
                                        DateTime.Now, DateTime.Now, "");
                    commodityTransferNote.WareHouseToStore = SelectedStoreTo;
                    commodityTransferNote.TransferNoteTypeId = CommodityTransferNote.CommodityTransferNoteTypeId.ToOtherStore;
                                                                     
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    return;
                }

                foreach (var item in LineItemToTransfer)
                {
                    var lineitem = factory.CreateLineItem(Guid.Empty,
                                                          item.StorageLineItem,
                                                          item.Grade != null ? item.Grade.Id : Guid.Empty,
                                                          item.Commodity.Id,
                                                          item.Weight,
                                                          item.BatchNumber, "");
                    commodityTransferNote.AddLineItem(lineitem);

                }
                DateTime now = DateTime.Now;
                var commodityStorageWfManager = Using<ICommodityStorageWFManager>(c);
                var commodityStorageNoteFactory = Using<ICommodityStorageNoteFactory>(c);

                var commodityStorageNote = commodityStorageNoteFactory.Create(issuerCostCentre,
                                                                              config.CostCentreApplicationId,
                                                                              SelectedStoreTo,
                                                                              user,
                                                                                GetDocumentReference("StorageNote", issuerCostCentre), Guid.Empty,
                                                                              now, now,
                                                                              "Commodity storage note");

                foreach (var item in LineItemToTransfer)
                {
                    var lineItem = commodityStorageNoteFactory.CreateLineItem(Guid.NewGuid(),
                                                                              item.Commodity.Id,
                                                                              item.Grade.Id,
                                                                              Guid.Empty,
                                                                              item.BatchNumber, item.Weight,
                                                                              item.BatchNumber);

                    commodityStorageNote.AddLineItem(lineItem);



                }

                commodityStorageNote.Confirm();
                commodityStorageWfManager.SubmitChanges(commodityStorageNote);
                commodityTransferNote.Confirm();
                workflowTransfer.SubmitChanges(commodityTransferNote);
                MessageBox.Show("Commodity Transfer No. " + commodityTransferNote.DocumentReference + " saved successfully");
            }
            SetUp();
            LoadStoresTo();
        }
        

        protected void LoadStoresTo()
        {
            StoreLookUpListTo.Clear();
            StoreLookUpListTo.Add(new Store(Guid.Empty)
            {
                Name = "-- Select Store --"
            });
            using (var c = NestedContainer)
            {
                var stores = Using<IStoreRepository>(c).GetAll();
                foreach (Store store in stores)
                {
                    if (store == null) continue;
                    /*if (!StoreLookUpList.Contains(store))
                    {
                        StoreLookUpListTo.Add(store);
                    }*/
                    StoreLookUpListTo.Add(store);
                }
            }
            SelectedStoreTo = StoreLookUpListTo.FirstOrDefault();
        }
        #endregion

        #region properties
        public const string SelectedStoreToPropertyName = "SelectedStoreTo";
        private Store _selectedtStoreTo = null;
        public Store SelectedStoreTo
        {
            get
            {
                return _selectedtStoreTo;
            }

            set
            {
                if (_selectedtStoreTo == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedStoreToPropertyName);
                _selectedtStoreTo = value;
                RaisePropertyChanged(SelectedStoreToPropertyName);
            }
        }
        #endregion
    }
}
