using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Distributr.Core.Domain.FinancialEntities;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Master.SettingsEntities;
using Distributr.Core.Domain.Master.Util;
using Distributr.Core.Domain.Transactional;
using Distributr.Core.Factory.Documents;
using Distributr.Core.Repository.InventoryRepository;
using Distributr.Core.Repository.Master.SettingsRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.ReturnsRepositories;
using Distributr.Core.Workflow;
using Distributr.Core.Workflow.FinancialWorkflow;
using Distributr.Core.Workflow.InventoryWorkflow;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.Services.Util;
using Distributr.WPF.Lib.UI.Pages;
using Distributr.WPF.Lib.ViewModels.Admin;
using Distributr.WPF.Lib.ViewModels.Transactional.Orders;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using System.Collections.Generic;
using Distributr.Core.ClientApp;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Domain.InventoryEntities;
using GalaSoft.MvvmLight.Messaging;

namespace Distributr.WPF.Lib.ViewModels.Transactional.RN
{
    public class ListReturnsViewModel : ListingsViewModelBase
    {
        public ObservableCollection<ListProductSerialItem> SerialItems { get; set; }
        internal IPagenatedList<ReturnsNote> PagenatedReturnsList = null;
        
        public ListReturnsViewModel()
        {


           LoadReturnCommand = new RelayCommand(DoLoadReturn);
            RunSaveCommand = new RelayCommand(SaveCommand);
            RunConfirmCommand = new RelayCommand(ConfirmCommand);
            RunClearAndSetup = new RelayCommand(ClearAndSetup);
            TabSelectionChangedCommand =new RelayCommand<SelectionChangedEventArgs>(SelectedTabChanged);
            ViewSelectedItemCommand =new RelayCommand<ListRNItemsViewModel>(ViewSelectedItem);


            ReturnsList = new ObservableCollection<ListRNItemsViewModel>();
            SerialItems = new ObservableCollection<ListProductSerialItem>();
            ReturnsNoteItemList = new ObservableCollection<ListRNLineItemsViewModel>();

            Logging.Log("ListReturnsViewModel", LOGLEVEL.INFO);
        }

        

        #region Properties
        public ObservableCollection<ListRNItemsViewModel> ReturnsList { get; set; }
        public ObservableCollection<ListRNLineItemsViewModel> ReturnsNoteItemList { get; set; }
        public ICommand LoadReturnsCommand { get; set; }
        public RelayCommand LoadReturnCommand { get; set; }
        public RelayCommand RunSaveCommand { get; set; }
        public RelayCommand RunConfirmCommand { get; set; }
        public RelayCommand RunClearAndSetup { get; set; }
        public RelayCommand<SelectionChangedEventArgs> TabSelectionChangedCommand { get; set; }
        public RelayCommand<ListRNItemsViewModel> ViewSelectedItemCommand { get; set; }

        /// <summary>
        /// The <see cref="ReturnNoteId" /> property's name.
        /// </summary>
        public const string ReturnNoteIdPropertyName = "ReturnNoteId";
        private Guid _ReturnNoteId = Guid.Empty;
        /// <summary>
        /// Gets the ReturnNoteId property.
        
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public Guid ReturnNoteId
        {
            get
            {
                return _ReturnNoteId;
            }

            set
            {
                if (_ReturnNoteId == value)
                {
                    return;
                }

                var oldValue = _ReturnNoteId;
                _ReturnNoteId = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(ReturnNoteIdPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="SalesMan" /> property's name.
        /// </summary>
        public const string SalesManPropertyName = "SalesMan";
        private string _SalesMan = null;
        /// <summary>
        /// Gets the SalesMan property.
        
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public string SalesMan
        {
            get
            {
                return _SalesMan;
            }

            set
            {
                if (_SalesMan == value)
                {
                    return;
                }

                var oldValue = _SalesMan;
                _SalesMan = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(SalesManPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="ReturnsDate" /> property's name.
        /// </summary>
        public const string ReturnsDatePropertyName = "ReturnsDate";
        private string _ReturnsDate = null;
        /// <summary>
        /// Gets the ReturnsDate property.
        
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public string ReturnsDate
        {
            get
            {
                return _ReturnsDate;
            }

            set
            {
                if (_ReturnsDate == value)
                {
                    return;
                }

                var oldValue = _ReturnsDate;
                _ReturnsDate = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(ReturnsDatePropertyName);
            }
        }

        public const string TotalPropertyName = "Total";
        private decimal _total = 0;
        public decimal Total
        {
            get
            {
                return _total;
            }

            set
            {
                if (_total == value)
                {
                    return;
                }

                var oldValue = _total;
                _total = value;
                RaisePropertyChanged(TotalPropertyName);
            }
        }

        public const string TotalVisiblePropertyName = "TotalVisible";
        private bool _totalVisible = false;
        public bool TotalVisible
        {
            get
            {
                return _totalVisible;
            }

            set
            {
                if (_totalVisible == value)
                {
                    return;
                }

                var oldValue = _totalVisible;
                _totalVisible = value;
                RaisePropertyChanged(TotalVisiblePropertyName);
            }
        }

        public const string SelectedDocumentStatusPropertyName = "SelectedDocumentStatus";
        private DocumentStatus _OrderStatus = DocumentStatus.Confirmed;
        public DocumentStatus SelectedDocumentStatus
        {
            get
            {
                return _OrderStatus;
            }

            set
            {
                if (_OrderStatus == value)
                {
                    return;
                }

                var oldValue = _OrderStatus;
                _OrderStatus = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(SelectedDocumentStatusPropertyName);
            }
        }

        #endregion

        #region Methods
        private void SelectedTabChanged(SelectionChangedEventArgs e)
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(delegate
            {
                if (e.Source.GetType() != typeof(TabControl))
                    return;

                TabItem tabItem = e.AddedItems[0] as TabItem;
                LoadSelectedTab(tabItem);
                e.Handled = true;

            }));
        }

        private void LoadSelectedTab(TabItem selectedTab)
        {
           switch (selectedTab.Name)
            {
                case "IncompleteTabItem":
                    SelectedDocumentStatus=DocumentStatus.Confirmed;
                    break;
                case "ClosedTabItem":
                    SelectedDocumentStatus = DocumentStatus.Closed;
                    break;
            }
            DoLoadReturns();
            
        }
        private void ViewSelectedItem(ListRNItemsViewModel selectedItem)
        {
            Navigate(new Uri("/views/RN/ReceiveReturns.xaml?ReturnsId=" + selectedItem.Id, UriKind.Relative).ToString());
        }

        protected override void Load(bool isFirstLoad = false)
        {
            if(isFirstLoad)
                ClearAndSetup();
            DoLoadReturns();

        }
        protected override void GoToPage(PageDestinations page)
        {
            GoToPageBase(page, PagenatedReturnsList.PageCount);
        }

        protected override void ComboPageSizesSelectionChanged(int take)
        {
            ItemsPerPage = take;
            Load();
        }

        protected override void UpdatePagenationControl()
        {
            UpdatePagenationControlBase(PagenatedReturnsList.PageNumber, PagenatedReturnsList.PageCount,
                                       PagenatedReturnsList.TotalItemCount,
                                       PagenatedReturnsList.IsFirstPage, PagenatedReturnsList.IsLastPage);
        }
        #region unused inheritance

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

        #endregion

        void ClearAndSetup()
        {
            ReturnNoteId = Guid.Empty;
            SalesMan = null;
            ReturnsDate = null;
            ReturnsList.Clear();
            ReturnsNoteItemList.Clear();
            SelectedDocumentStatus=DocumentStatus.Confirmed;
            Logging.Log("ClearAndSetup", LOGLEVEL.INFO);
        }

        void DoLoadReturns()
        {
            using (var container = NestedContainer)
            {
                var _returnsNoteService = Using<IReturnsNoteRepository>(container);

                Logging.Log("Loading Returns", LOGLEVEL.INFO);
                List<ReturnsNote> returns =
                    _returnsNoteService.GetAll()
                        .OfType<ReturnsNote>()
                        .Where(n => n.ReturnsNoteType == ReturnsNoteType.SalesmanToDistributor)
                        .ToList();


                returns = returns.Where(n => n.Status == SelectedDocumentStatus).ToList();
                             

                PagenatedReturnsList = new PagenatedList<ReturnsNote>(returns.AsQueryable(), CurrentPage,
                                                                      ItemsPerPage,
                                                                      returns.Count());
                ReturnsList.Clear();
                if (PagenatedReturnsList != null && PagenatedReturnsList.Count > 0)
                {
                    var items = PagenatedReturnsList
                        .Select(n => new ListRNItemsViewModel
                                         {
                                             Id = n.Id,
                                             ReturnsDate =
                                                 n.DocumentDateIssued.ToString(
                                                     "dd-MMM-yyyy hh:mm:ss.sss tt"),
                                             SalesMan = n.DocumentIssuerUser.Username
                                         }).ToList();
                    items.ForEach(ReturnsList.Add);

                    UpdatePagenationControl();
                }
               
            }
        }

        public void EditActual(ReturnItemViewModel items)
        {
            var item = ReturnsNoteItemList.FirstOrDefault(n => n.Id == items.Id);
            if (item != null )
            {
                item.Actual = items.ActualExpected;
                List<ListProductSerialItem> existingSerials = SerialItems.Where(n => n.ProductId == items.ProductId).ToList();
                foreach (ListProductSerialItem list in existingSerials)
                {
                    SerialItems.Remove(list);
                }
                foreach (var toadd in items.SerialItems)
                {
                    SerialItems.Add(toadd);
                }
            }
        }

        void DoLoadReturn()
        {
            using (var container = NestedContainer)
            {
                var _returnsNoteService = Using<IReturnsNoteRepository>(container);


                ReturnsNote returnnote = null;
                if (ReturnNoteId != Guid.Empty)
                {
                    returnnote = _returnsNoteService.GetById(ReturnNoteId) as ReturnsNote;
                    SalesMan = returnnote.DocumentIssuerUser.Username;
                    ReturnsDate = returnnote.DocumentDateIssued.ToString("dd-MMM-yyyy hh:mm:ss.sss tt");
                    ReturnsNoteItemList.Clear();
                    returnnote._lineItems.ForEach(n => ReturnsNoteItemList.Add(new ListRNLineItemsViewModel()
                                                                                   {
                                                                                       Actual = n.Actual,
                                                                                       Expected =
                                                                                           n.Product == null
                                                                                               ? n.Value
                                                                                               : n.Qty,
                                                                                       Value = n.Value,
                                                                                       Id = n.Id,
                                                                                       Product =
                                                                                           n.Product == null
                                                                                               ? n.ReturnType.ToString()
                                                                                               : n.Product.Description,
                                                                                       ReturnType =
                                                                                           n.ReturnType.ToString() +
                                                                                           (n.LossType == 0
                                                                                                ? ""
                                                                                                : " " +
                                                                                                  n.LossType.ToString()),
                                                                                       ProductId =
                                                                                           n.Product == null
                                                                                               ? Guid.Empty
                                                                                               : n.Product.Id
                                                                                   }));

                    Total = 0;
                    TotalVisible = false;

                    foreach (var item in returnnote._lineItems.Where(n => n.ReturnType != ReturnsType.Inventory))
                    {
                        TotalVisible = true;
                        Total += item.Value;
                    }
                }
            }
        }

        void SaveCommand()
        {
            using (var container = NestedContainer)
            {
                IReturnsNoteRepository _returnsNoteService = Using<IReturnsNoteRepository>(container);
                IConfigService _configService = Using<IConfigService>(container);


                Logging.Log("Saving Returns", LOGLEVEL.INFO);
                var returnnote = _returnsNoteService.GetById(ReturnNoteId) as ReturnsNote;
                if (returnnote != null)
                {
                    foreach (ReturnsNoteLineItem item in returnnote._lineItems)
                    {
                        var updated = ReturnsNoteItemList.FirstOrDefault(n => n.Id == item.Id);
                        if (updated != null)
                        {
                            item.Actual = updated.Actual;
                        }
                    }
                    _returnsNoteService.Save(returnnote);
                }
            }
        }

        void ConfirmCommand()
        {
            using (var container = NestedContainer)
            {
                BasicConfig config = container.GetInstance<IConfigService>().Load();
                IReturnsNoteRepository _returnsNoteService = Using<IReturnsNoteRepository>(container);
                IInventorySerialsWorkFlow _inventorySerialsService = Using<IInventorySerialsWorkFlow>(container);
                var returnnote = _returnsNoteService.GetById(ReturnNoteId) as ReturnsNote;
               

                Logging.Log("Confirming Returns", LOGLEVEL.INFO);
                if (returnnote != null)
                {
                    bool canContinue = false;
                    if (ReturnsNoteItemList.All(s => s.ReturnType != ReturnsType.Inventory.ToString()) && ReturnsNoteItemList.All(s => s.Actual != s.Expected))
                    {
                        decimal underbankamout = ReturnsNoteItemList.Sum(s => s.Expected - s.Actual);
                        Messenger.Default.Send(new UnderBankingSetup
                                                   {
                                                       SalesmanId = returnnote.DocumentIssuerCostCentre.Id,
                                                       Amount = underbankamout
                                                   });

                        canContinue = Using<IUnderBankingPopUp>(container).AddUnderBanking();
                        if (!canContinue) return;
                    }
                   

                    //returnnote.Status = DocumentStatus.Closed;
                    foreach (ReturnsNoteLineItem item in returnnote._lineItems)
                    {
                        var updated = ReturnsNoteItemList.FirstOrDefault(n => n.Id == item.Id);
                        if (updated != null)
                        {
                            item.Actual = updated.Actual;
                        }
                    }
                    returnnote.Close();
                 
                   
                    var confirmReturnsNoteWfManager = Using<IConfirmReturnsNoteWFManager>(container);
                    confirmReturnsNoteWfManager.SubmitChanges(returnnote,config);
                    ProcessReturnNote(returnnote);

                    var serialItemsToSubmit = SerialItems
                        .Select(n => new InventorySerials(n.SerialsId)
                            {
                                CostCentreRef = new CostCentreRef { Id = returnnote.DocumentRecipientCostCentre.Id },
                                DocumentId = returnnote.Id,
                                ProductRef = new ProductRef { ProductId = n.ProductId, },
                                From = n.FromSerial,
                                To = n.ToSerial
                            }).ToList();
                    _inventorySerialsService.SubmitInventorySerials(serialItemsToSubmit);
                    SerialItems.Clear();
                    MessageBox.Show("Return completed successfully");
                }
            }
        }

        void ProcessReturnNote(ReturnsNote returnnote)
        {
            using (var container = NestedContainer)
            {
                BasicConfig config = container.GetInstance<IConfigService>().Load();
               
                var confirmInventoryAdjustmentNoteWfManager = Using<IInventoryAdjustmentNoteWfManager>(container);
                var confirmPaymentNoteWfManager = Using<IConfirmPaymentNoteWFManager>(container);

                if (returnnote._lineItems.All(s => s.ReturnType == ReturnsType.Inventory))
                {
                    var distributrAdjustInventory = CreateDistributrIAN(returnnote);
                    confirmInventoryAdjustmentNoteWfManager.SubmitChanges(distributrAdjustInventory,config);
                    var salesmanAdjustInventory = CreateSalesmanIAN(returnnote);
                    confirmInventoryAdjustmentNoteWfManager.SubmitChanges(salesmanAdjustInventory,config);
                }
                if (returnnote._lineItems.Any(s => s.ReturnType != ReturnsType.Inventory))
                {
                    var paymentNote = CreatePaymentReturnDocument(returnnote);
                    paymentNote.Confirm();
                    confirmPaymentNoteWfManager.SubmitChanges(paymentNote,config);
                    
                }
                
            }

        }

        public PaymentNote CreatePaymentReturnDocument(ReturnsNote returnnote)
        {
           
                using (var container = NestedContainer)
                {

                    IConfigService _configService = Using<IConfigService>(container);


                    PaymentNote note = new PaymentNote(Guid.NewGuid());

                    note.DocumentDateIssued = DateTime.Now;
                    note.DocumentIssuerCostCentre = returnnote.DocumentIssuerCostCentre;
                    note.DocumentIssuerCostCentreApplicationId = _configService.Load().CostCentreApplicationId;
                    note.DocumentIssuerUser = returnnote.DocumentIssuerUser;
                    note.DocumentRecipientCostCentre = returnnote.DocumentRecipientCostCentre;
                    note.DocumentReference = "";
                    note.DocumentType = DocumentType.PaymentNote;
                    note.PaymentNoteType = PaymentNoteType.Returns;


                    foreach (var item in returnnote._lineItems)
                    {
                        if (item.ReturnType == ReturnsType.Inventory && item.Product == null) continue;

                        PaymentNoteLineItem ianLineitem = new PaymentNoteLineItem(Guid.NewGuid())
                        {
                            Amount = item.Value,
                            Description = item.Reason,
                            LineItemSequenceNo = 0,
                            PaymentMode = (PaymentMode) (int) item.ReturnType,
                        };
                        note.AddLineItem(ianLineitem);
                    }

                    return note;
                }
            
            
        }

        private InventoryAdjustmentNote CreateSalesmanIAN(ReturnsNote returnnote)
        {
            using (var container = NestedContainer)
            {
                IConfigService _configService = Using<IConfigService>(container);
                IInventoryRepository _inventoryService = Using<IInventoryRepository>(container);


                InventoryAdjustmentNote note = Using<IInventoryAdjustmentNoteFactory>(container).Create(returnnote.DocumentIssuerCostCentre,
                    _configService.Load().CostCentreApplicationId,returnnote.DocumentRecipientCostCentre,returnnote.DocumentIssuerUser,
                    "",InventoryAdjustmentNoteType.Returns, Guid.Empty);

                List<InventoryAdjustmentNoteLineItem> ListianLineitem = new List<InventoryAdjustmentNoteLineItem>();
                foreach (var item in returnnote._lineItems)
                {
                    if (item.ReturnType != ReturnsType.Inventory && item.Product == null) continue;
                    var inventory = _inventoryService.GetByProductIdAndWarehouseId(item.Product.Id,
                                                                     returnnote.DocumentIssuerCostCentre.Id);
                    decimal expected = inventory != null ? inventory.Balance : 0;
                    InventoryAdjustmentNoteLineItem ianLineitem = Using<IInventoryAdjustmentNoteFactory>(container)
                                            .CreateLineItem(expected - item.Qty, item.Product.Id, expected, 0, item.Reason);
                    ListianLineitem.Add(ianLineitem);
                }
                foreach (var i in ListianLineitem)
                {
                    note.AddLineItem(i);
                }
                note.Confirm();
                //note._SetLineItems(ListianLineitem);
                return note;
            }
        }

        private InventoryAdjustmentNote CreateDistributrIAN(ReturnsNote returnnote)
        {
            using (var container = NestedContainer)
            {

                IConfigService _configService = Using<IConfigService>(container);


                var _inventoryService = Using<IInventoryRepository>(container);


                InventoryAdjustmentNote note = Using<IInventoryAdjustmentNoteFactory>(container).Create(
                    returnnote.DocumentRecipientCostCentre,_configService.Load().CostCentreApplicationId,
                    returnnote.DocumentRecipientCostCentre,returnnote.DocumentIssuerUser,"",
                    InventoryAdjustmentNoteType.Available, Guid.Empty);

                List<InventoryAdjustmentNoteLineItem> ListianLineitem = new List<InventoryAdjustmentNoteLineItem>();
                foreach (var item in returnnote._lineItems)
                {
                    if (item.ReturnType != ReturnsType.Inventory && item.Product == null) continue;
                    var inventory = _inventoryService.GetByProductIdAndWarehouseId(item.Product.Id,
                                                                     returnnote.DocumentRecipientCostCentre.Id);
                    decimal expected = inventory != null ? inventory.Balance : 0;
                    InventoryAdjustmentNoteLineItem ianLineitem = Using<IInventoryAdjustmentNoteFactory>(container)
                        .CreateLineItem(expected + item.Qty, item.Product.Id, expected, 0, item.Reason);
                    ListianLineitem.Add(ianLineitem);
                }
                foreach (var i in ListianLineitem)
                {
                    note.AddLineItem(i);
                }
                //note._SetLineItems(ListianLineitem);
                note.Confirm();
                return note;
            }

        }

      #endregion

        public class ListRNItemsViewModel : ViewModelBase
        {
            /// <summary>
            /// The <see cref="ReturnsDate" /> property's name.
            /// </summary>
            public const string ReturnsDatePropertyName = "ReturnsDate";
            private string _ReturnsDate = null;
            /// <summary>
            /// Gets the ReturnsDate property.
            
            /// Changes to that property's value raise the PropertyChanged event. 
            /// This property's value is broadcasted by the Messenger's default instance when it changes.
            /// </summary>
            public string ReturnsDate
            {
                get
                {
                    return _ReturnsDate;
                }

                set
                {
                    if (_ReturnsDate == value)
                    {
                        return;
                    }

                    var oldValue = _ReturnsDate;
                    _ReturnsDate = value;

                    // Update bindings, no broadcast
                    RaisePropertyChanged(ReturnsDatePropertyName);
                }
            }

            /// <summary>
            /// The <see cref="SalesMan" /> property's name.
            /// </summary>
            public const string SalesManPropertyName = "SalesMan";
            private string _SalesMan = null;
            /// <summary>
            /// Gets the SalesMan property.
            
            /// Changes to that property's value raise the PropertyChanged event. 
            /// This property's value is broadcasted by the Messenger's default instance when it changes.
            /// </summary>
            public string SalesMan
            {
                get
                {
                    return _SalesMan;
                }

                set
                {
                    if (_SalesMan == value)
                    {
                        return;
                    }

                    var oldValue = _SalesMan;
                    _SalesMan = value;

                    // Update bindings, no broadcast
                    RaisePropertyChanged(SalesManPropertyName);
                }
            }

            /// <summary>
            /// The <see cref="Id" /> property's name.
            /// </summary>
            public const string IdPropertyName = "Id";
            private Guid _Id = Guid.Empty;
            /// <summary>
            /// Gets the Id property.
            
            /// Changes to that property's value raise the PropertyChanged event. 
            /// This property's value is broadcasted by the Messenger's default instance when it changes.
            /// </summary>
            public Guid Id
            {
                get
                {
                    return _Id;
                }

                set
                {
                    if (_Id == value)
                    {
                        return;
                    }

                    var oldValue = _Id;
                    _Id = value;

                    // Update bindings, no broadcast
                    RaisePropertyChanged(IdPropertyName);
                }
            }

        }

        public class ListRNLineItemsViewModel : ViewModelBase
        {
            /// <summary>
            /// The <see cref="ReturnType" /> property's name.
            /// </summary>
            public const string ReturnTypePropertyName = "ReturnType";
            private string _ReturnType = null;
            /// <summary>
            /// Gets the ReturnType property.
            
            /// Changes to that property's value raise the PropertyChanged event. 
            /// This property's value is broadcasted by the Messenger's default instance when it changes.
            /// </summary>
            public string ReturnType
            {
                get
                {
                    return _ReturnType;
                }

                set
                {
                    if (_ReturnType == value)
                    {
                        return;
                    }

                    var oldValue = _ReturnType;
                    _ReturnType = value;

                    // Update bindings, no broadcast
                    RaisePropertyChanged(ReturnTypePropertyName);
                }
            }

            /// <summary>
            /// The <see cref="Product" /> property's name.
            /// </summary>
            public const string ProductPropertyName = "Product";
            private string _Product = null;
            /// <summary>
            /// Gets the Product property.
            
            /// Changes to that property's value raise the PropertyChanged event. 
            /// This property's value is broadcasted by the Messenger's default instance when it changes.
            /// </summary>
            public string Product
            {
                get
                {
                    return _Product;
                }

                set
                {
                    if (_Product == value)
                    {
                        return;
                    }

                    var oldValue = _Product;
                    _Product = value;

                    // Update bindings, no broadcast
                    RaisePropertyChanged(ProductPropertyName);
                }
            }

            /// <summary>
            /// The <see cref="Id" /> property's name.
            /// </summary>
            public const string IdPropertyName = "Id";
            private Guid _Id = Guid.Empty;
            /// <summary>
            /// Gets the Id property.
            
            /// Changes to that property's value raise the PropertyChanged event. 
            /// This property's value is broadcasted by the Messenger's default instance when it changes.
            /// </summary>
            public Guid Id
            {
                get
                {
                    return _Id;
                }

                set
                {
                    if (_Id == value)
                    {
                        return;
                    }

                    var oldValue = _Id;
                    _Id = value;

                    // Update bindings, no broadcast
                    RaisePropertyChanged(IdPropertyName);
                }
            }

            /// <summary>
            /// The <see cref="Expected" /> property's name.
            /// </summary>
            public const string ExpectedPropertyName = "Expected";
            private Decimal _Expected = 0;
            /// <summary>
            /// Gets the Expected property.
            
            /// Changes to that property's value raise the PropertyChanged event. 
            /// This property's value is broadcasted by the Messenger's default instance when it changes.
            /// </summary>
            public Decimal Expected
            {
                get
                {
                    return _Expected;
                }

                set
                {
                    if (_Expected == value)
                    {
                        return;
                    }

                    var oldValue = _Expected;
                    _Expected = value;

                    // Update bindings, no broadcast
                    RaisePropertyChanged(ExpectedPropertyName);
                }
            }

            /// <summary>
            /// The <see cref="Actual" /> property's name.
            /// </summary>
            public const string ActualPropertyName = "Actual";
            private Decimal _Actual = 0;
            /// <summary>
            /// Gets the Actual property.
            
            /// Changes to that property's value raise the PropertyChanged event. 
            /// This property's value is broadcasted by the Messenger's default instance when it changes.
            /// </summary>
            public Decimal Actual
            {
                get
                {
                    return _Actual;
                }

                set
                {
                    if (_Actual == value)
                    {
                        return;
                    }

                    var oldValue = _Actual;
                    _Actual = value;

                    // Update bindings, no broadcast
                    RaisePropertyChanged(ActualPropertyName);
                }
            }

            /// <summary>
            /// The <see cref="Value" /> property's name.
            /// </summary>
            public const string ValuePropertyName = "Value";
            private Decimal _Value = 0;
            /// <summary>
            /// Gets the Value property.
            
            /// Changes to that property's value raise the PropertyChanged event. 
            /// This property's value is broadcasted by the Messenger's default instance when it changes.
            /// </summary>
            public Decimal Value
            {
                get
                {
                    return _Value;
                }

                set
                {
                    if (_Value == value)
                    {
                        return;
                    }

                    var oldValue = _Value;
                    _Value = value;

                    // Update bindings, no broadcast
                    RaisePropertyChanged(ValuePropertyName);
                }
            }

            public const string ProductIdPropertyName = "ProductId";
            private Guid _productId = Guid.Empty;
            public Guid ProductId
            {
                get
                {
                    return _productId;
                }

                set
                {
                    if (_productId == value)
                    {
                        return;
                    }

                    var oldValue = _productId;
                    _productId = value;

                    // Update bindings, no broadcast
                    RaisePropertyChanged(ProductIdPropertyName);
                }
            }

        }

      
    }

    public class ReturnItemViewModel : DistributrViewModelBase
    {
        public RelayCommand AddSerialsCommand { get; set; }
        public ObservableCollection<ListProductSerialItem> SerialItems { get; set; }

        public ReturnItemViewModel()
        {
            AddSerialsCommand = new RelayCommand(AddSerial);
            SerialItems=new ObservableCollection<ListProductSerialItem>();
        }

        public void AddSerial()
        {
           
            string msg = "";
            if (string.IsNullOrEmpty(StartSerialNo) || string.IsNullOrEmpty(EndSerialNo))
            {
                msg = "Serial numbers cannot be Empty! ";
                MessageBox.Show(msg);
                return;
            }

            ListProductSerialItem item = null;
            item = new ListProductSerialItem
                    {
                        FromSerial = StartSerialNo,
                        ProductId = ProductId,
                        ToSerial = EndSerialNo,
                        SerialsId = Guid.NewGuid(),
                        LineItemId = Guid.NewGuid()
                    };
            if (SelectedSerialId != Guid.Empty)
            {
                ListProductSerialItem serials = SerialItems.FirstOrDefault(n => n.SerialsId == SelectedSerialId);
                item.SerialsId = serials.SerialsId;
                SerialItems.Remove(serials);
            }
            SerialItems.Add(item);
            StartSerialNo = "";
            EndSerialNo = "";
            SelectedSerialNumbers = item;
            SelectedSerialId = Guid.Empty;

        }

        public void EditSerial()
        {
            SelectedSerialId = SelectedSerialNumbers.SerialsId;
            StartSerialNo = SelectedSerialNumbers.FromSerial;
            EndSerialNo = SelectedSerialNumbers.ToSerial;

        }

        public void DeleteSerial()
        {
            if (MessageBox.Show("Are you sure you want to delete selected serial numbers?") == MessageBoxResult.Cancel)
                return;
            SerialItems.Remove(SelectedSerialNumbers);
            SelectedSerialNumbers = SerialItems.FirstOrDefault();
            StartSerialNo = "";
            EndSerialNo = "";
        }

        public void LoadData(ListReturnsViewModel.ListRNLineItemsViewModel item, List<ListProductSerialItem> serials)
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
                Id = item.Id;
                SerialItems.Clear();
                StartSerialNo = "";
                EndSerialNo = "";
                ReturnType = item.ReturnType;
                Product = !string.IsNullOrEmpty(item.Product) ? item.Product : "n/a";
                Expected = item.ReturnType == "Inventory" ? item.Expected : item.Value;
                ActualExpected = item.Actual;
                ProductId = item.ProductId;
                foreach (var itemToLoad in serials)
                {
                    SerialItems.Add(itemToLoad);
                }
                var allowBarCodeInput = Using<ISettingsRepository>(cont).GetByKey(SettingsKeys.AllowBarcodeInput);
                if (allowBarCodeInput != null)
                {
                    AllowBarCodeInput = allowBarCodeInput.Value == "1" ? true : false;
                }
            }
        }

        private bool isSpecial(string startToCheck)
        {
            const string specialCharacters = "!@#$%^&*()+=-[]\\;,./{}|\":<>?";
            foreach (char t in specialCharacters)
            {
                if (startToCheck.Contains(t))
                    return true;
            }
            return false;
        }

        public const string SelectedSerialNumbersPropertyName = "SelectedSerialNumbers";
        private ListProductSerialItem _selectedSerialNumbers = null;
        public ListProductSerialItem SelectedSerialNumbers
        {
            get
            {
                return _selectedSerialNumbers;
            }

            set
            {
                if (_selectedSerialNumbers == value)
                {
                    return;
                }

                var oldValue = _selectedSerialNumbers;
                _selectedSerialNumbers = value;

                RaisePropertyChanged(SelectedSerialNumbersPropertyName);
            }
        }

        public const string SelectedSerialIdPropertyName = "SelectedSerialId";
        private Guid _selectedSerialId = Guid.Empty;
        public Guid SelectedSerialId
        {
            get
            {
                return _selectedSerialId;
            }

            set
            {
                if (_selectedSerialId == value)
                {
                    return;
                }

                var oldValue = _selectedSerialId;
                _selectedSerialId = value;

                RaisePropertyChanged(SelectedSerialIdPropertyName);
            }
        }

        public const string ReturnTypePropertyName = "ReturnType";
        private string _returntype = "";
        public string ReturnType
        {
            get
            {
                return _returntype;
            }

            set
            {
                if (_returntype == value)
                {
                    return;
                }

                var oldValue = _returntype;
                _returntype = value;



                // Update bindings, no broadcast
                RaisePropertyChanged(ReturnTypePropertyName);


            }
        }

        public const string ProductIdPropertyName = "ProductId";
        private Guid _productId = Guid.Empty;
        public Guid ProductId
        {
            get
            {
                return _productId;
            }

            set
            {
                if (_productId == value)
                {
                    return;
                }

                var oldValue = _productId;
                _productId = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(ProductIdPropertyName);

            }
        }

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

                var oldValue = _id;
                _id = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(IdPropertyName);

            }
        }

        public const string ProductPropertyName = "Product";
        private string _product = "";
        public string Product
        {
            get
            {
                return _product;
            }

            set
            {
                if (_product == value)
                {
                    return;
                }

                var oldValue = _product;
                _product = value;
                // Update bindings, no broadcast
                RaisePropertyChanged(ProductPropertyName);
            }
        }

        public const string ExpectedPropertyName = "Expected";
        private decimal _expected = 0;
        public decimal Expected
        {
            get
            {
                return _expected;
            }

            set
            {
                if (_expected == value)
                {
                    return;
                }

                var oldValue = _expected;
                _expected = value;

                RaisePropertyChanged(ExpectedPropertyName);
            }
        }

        public const string ActualExpectedPropertyName = "ActualExpected";
        private decimal _actualExpected = 0;
        public decimal ActualExpected
        {
            get
            {
                return _actualExpected;
            }

            set
            {
                if (_actualExpected == value)
                {
                    return;
                }

                var oldValue = _actualExpected;
                _actualExpected = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(ActualExpectedPropertyName);

            }
        }

        public const string StartSerialNoPropertyName = "StartSerialNo";
        private string _startSerialNo = "";
        public string StartSerialNo
        {
            get
            {
                return _startSerialNo;
            }

            set
            {
                if (_startSerialNo == value)
                {
                    return;
                }

                var oldValue = _startSerialNo;
                _startSerialNo = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(StartSerialNoPropertyName);

            }
        }

        public const string EndSerialNoPropertyName = "EndSerialNo";
        private string _endSerialNo = "";
        public string EndSerialNo
        {
            get
            {
                return _endSerialNo;
            }

            set
            {
                if (_endSerialNo == value)
                {
                    return;
                }

                var oldValue = _endSerialNo;
                _endSerialNo = value;
                // Update bindings, no broadcast
                RaisePropertyChanged(EndSerialNoPropertyName);

            }
        }

        public const string AllowBarCodeInputPropertyName = "AllowBarCodeInput";

        private bool _allowBarCodeInput = false;

        public bool AllowBarCodeInput
        {
            get
            {
                return _allowBarCodeInput;
            }

            set
            {
                if (_allowBarCodeInput == value)
                {
                    return;
                }

                var oldValue = _allowBarCodeInput;
                _allowBarCodeInput = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(AllowBarCodeInputPropertyName);
            }
        }

    }

    public class ListProductSerialItem
    {
        public Guid LineItemId { get; set; }
        public Guid SerialsId { get; set; }
        public Guid ProductId { get; set; }
        public string FromSerial { get; set; }
        public string ToSerial { get; set; }
    }
}
