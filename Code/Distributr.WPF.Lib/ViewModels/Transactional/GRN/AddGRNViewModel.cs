using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Distributr.Core.Domain.InventoryEntities;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CentreEntity;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Domain.Transactional.DocumentEntities.OrderDocumentEntities;
using Distributr.Core.Factory.Documents;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.InventoryRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.UI.Pages;
using GalaSoft.MvvmLight.Command;
using System.Linq;
using Distributr.Core.ClientApp;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Workflow.Impl.AuditLogs;
using Distributr.Core.Workflow.InventoryWorkflow;

namespace Distributr.WPF.Lib.ViewModels.Transactional.GRN
{
    public class AddGRNViewModel : DistributrViewModelBase
    {
        private ViewModelParameters vmparams;
        private List<ProductSerialNumbers> _inventorySerialNumbersList;
        private string _reason;

        public AddGRNViewModel()
        {
           using (StructureMap.IContainer c = NestedContainer)
            {
                vmparams = Using<IConfigService>(c).ViewModelParameters;
            }
            CancelCommand = new RelayCommand(RunCancel);
            ConfirmCommand = new RelayCommand(RunConfirm);
           
            RemoveLineItemCommand = new RelayCommand<AddGrnLineItemViewModel>(RemoveLineItem);
            ClearAndSetup = new RelayCommand(RunClearAndSetup);
            LoadGrn = new RelayCommand(RunLoadGrn);
          _inventorySerialNumbersList = new List<ProductSerialNumbers>();
            LineItems = new ObservableCollection<AddGrnLineItemViewModel>();
            AddGRNCommand = new RelayCommand(AddGRN);
            EditGRNCommand = new RelayCommand<AddGrnLineItemViewModel>(EditGRN);
            
          
            
            
        }


        public RelayCommand CancelCommand { get; set; }
        public RelayCommand ConfirmCommand { get; set; }
        public RelayCommand<AddGrnLineItemViewModel> RemoveLineItemCommand { get; set; }
        public RelayCommand AddGRNCommand { get; set; }
        public RelayCommand<AddGrnLineItemViewModel> EditGRNCommand { get; set; }
        public ObservableCollection<AddGrnLineItemViewModel> LineItems { get; set; }
        public RelayCommand ClearAndSetup { get; set; }
       


      protected override void LoadPage(Page page)
        {
            ClearAndSetup.Execute(null);
            IRNId = PresentationUtility.ParseIdFromUrl(page.NavigationService.CurrentSource);
          if (IRNId == Guid.Empty)
                {
                 
                    ShowAddVisibility = Visibility.Visible;
                    ShowConfirmVisibility = Visibility.Visible;
                    OrderReferencesIsEnabled = true;
                    EnterLoadNoIsEnabled = true;
                    CancelAction = "Cancel";
                    RunLoad();
                }
                else
                {
                    CancelAction = "Back";
                    ShowAddVisibility = Visibility.Collapsed;
                    ShowConfirmVisibility = Visibility.Collapsed;
                    OrderReferencesIsEnabled = false;
                    EnterLoadNoIsEnabled = false;
                    RunLoadGrn();
                }
               
            
        }
       
        
       void BreakBulk()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                BasicConfig config = c.GetInstance<IConfigService>().Load();
                CostCentre thiscc = Using<ICostCentreRepository>(c) .GetById(Using<IConfigService>(c).Load().CostCentreId);
                var breakbulk = LineItems.Where(n => n.BreakBulk);
                foreach (var item in breakbulk)
                {
                    Using<IBreakBulkWorkflow>(c).BreakBulk(item.ProductId, thiscc.Id, item.Qty,config);
                }
            }
        }
         
    
        private bool ValidateBeforeConfirm()
        {
            if (LineItems.Count <= 0)
                {
                    MessageBox.Show("No products specified");
                    return false;
                }
                if (string.IsNullOrEmpty(OrderReferences))
                {
                    MessageBox.Show("Specify Order Reference");
                   return false;
                }
                if (string.IsNullOrEmpty(LoadNo))
                {
                    MessageBox.Show("Specify Load Number");
                 
                    return false;
                }
            return true;

        }

        private void EditGRN(AddGrnLineItemViewModel lineitem)
        {
            if (lineitem == null) return;
            using (var container = NestedContainer)
            {
                var serials = _inventorySerialNumbersList.Where(n => n.ProductId == lineitem.ProductId).ToList();
                var selected = Using<IGrnPopUp>(container).EditGrnItems(lineitem, serials);
                if (selected != null)                   
                    
                    UpdateModalItems(selected);
            }
        }

        private void AddGRN()
        {
            using (var container = NestedContainer)
            {
                var selected = Using<IGrnPopUp>(container).AddGrnItems();
                if (selected != null)
                    UpdateModalItems(selected);
            }
        }

        private void RunConfirm()
        {
           if(!ValidateBeforeConfirm())return;
            using (StructureMap.IContainer c = NestedContainer)
            {
                BasicConfig config = c.GetInstance<IConfigService>().Load();
                DateTime start = DateTime.Now;
                CostCentre thiscc = Using<ICostCentreRepository>(c).GetById(Using<IConfigService>(c).Load().CostCentreId);
                CostCentre producercc = Using<IProducerRepository>(c).GetProducer();
                Guid userId = Using<IConfigService>(c).ViewModelParameters.CurrentUserId;
                Guid irnId = Guid.NewGuid();
                var f = Using<IInventoryReceivedNoteFactory>(c);
                var irn = f.Create(thiscc, Using<IConfigService>(c).Load().CostCentreApplicationId,
                                                     thiscc, producercc,
                                                     LoadNo, OrderReferences, Using<IUserRepository>(c).GetById(userId),
                                                     DocumentReference, irnId);
                    
                foreach (var item in LineItems)
                {
                    InventoryReceivedNoteLineItem li = f.CreateLineItem(item.ProductId, item.Qty, item.UnitCost,
                                                                        item.Reason, 0);
                    li.Expected = item.Expected;
                        
                    irn.AddLineItem(li);
                }
                irn.Confirm();
                var producerIRNWorkflow = Using<IProducerIRNWFManager>(c);
                producerIRNWorkflow.SubmitChanges(irn,config);
                Using<IAuditLogWFManager>(c).AuditLogEntry("Create GRN",
                                               string.Format("Time taken: {0}", DateTime.Now.Subtract(start)));
               //BreakBulk();
              // producerIRNWorkflow.SubmitChanges(irn);
               start = DateTime.Now;
                CloseOrders();
                Using<IAuditLogWFManager>(c).AuditLogEntry("Close GRN Orders",
                                               string.Format("Time taken: {0}", DateTime.Now.Subtract(start)));

                Using<IInventorySerialsWorkFlow>(c).SubmitInventorySerials(_inventorySerialNumbersList
                                                                    .Select(n => new InventorySerials(n.SerialsId)
                                                                        {
                                                                            CostCentreRef =
                                                                                new CostCentreRef
                                                                                    {
                                                                                        Id =
                                                                                            irn.DocumentRecipientCostCentre
                                                                                               .Id
                                                                                    },
                                                                            DocumentId = irn.Id,
                                                                            ProductRef =
                                                                                new ProductRef
                                                                                    {
                                                                                        ProductId = n.ProductId,
                                                                                    },
                                                                            From = n.From,
                                                                            To = n.To
                                                                        }).ToList());
            }
            NavigateCommand.Execute(@"/views/grn/listgrn.xaml");
        }

        void CloseOrders()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                List<Guid> selectedOrderIds = vmparams.SelectedOrderIds;
                if(selectedOrderIds==null)return;
                foreach (Guid orderid in selectedOrderIds)
                {
                    MainOrder order = Using<IMainOrderRepository>(c).GetById(orderid);
                    order.Close();
                   Using<IPurchaseOrderWorkflow>(c).Submit(order);
                }
            }
        }

       
        public void RunLoad()
        {
            using (var c = NestedContainer)
            {
                
                IRNId = Guid.NewGuid();
                DocumentReference = NewGrnDocReference();

                LineItems.Clear();
                List<Guid> selectedOrderIds = vmparams.SelectedOrderIds;
                if (selectedOrderIds != null && selectedOrderIds.Any())
                {
                    List<MainOrder> orders = selectedOrderIds
                        .Select(selectedOrderId => Using<IMainOrderRepository>(c)
                            .GetById(selectedOrderId)).ToList();

                    orders.Select(n=>n.DocumentReference).Distinct().ToList().ForEach(n =>
                                       {
                                           if (string.IsNullOrEmpty(OrderReferences))
                                               OrderReferences += n;
                                           else
                                               OrderReferences += " , " + n;
                                       });


                    List<SubOrderLineItem> orderLineItems = orders.SelectMany(n => n.PendingDispatchLineItems).ToList();
                    var groupedItems = from o in orderLineItems
                                       group o by o.Product.Id
                                       into g
                                       select new
                                                  {
                                                      ProductId = g.Key,
                                                      product = g.First().Product,
                                                      Qty = g.Sum(n => n.ApprovedQuantity),
                                                      Desc = g.First().Product.Description,
                                                      Cost = 0
                                                    };
                    foreach (var item in
                        groupedItems.Where(item => item.product is SaleProduct || item.product is ConsolidatedProduct))
                    {
                        _productPackagingSummaryService.AddProduct(item.product.Id, item.Qty);
                    }
                    RefreshLineItem();
                }
            }
        }

      private void RefreshLineItem()
        {
            var temp = LineItems.ToList();
            LineItems.Clear();
            
            var prods = _productPackagingSummaryService.GetProductSummary();
            foreach (var item in prods)
            {
                var liitem = temp.FirstOrDefault(n => n.ProductId == item.Product.Id);
                bool consolidatedproduct = false;
                Product p = item.Product;
                var cost = 0m;
                //if (p.ProductPricings.Count > 0)
                cost = ProductPriceCalc(p); //p.TotalExFactoryValue(p.ProductPricings.FirstOrDefault().Tier);
                if (p is ConsolidatedProduct)
                    consolidatedproduct = true;
                AddLineItem(Guid.NewGuid(), item.Product.Id, item.Product.Description, cost, item.Quantity,
                            liitem == null ? item.Quantity : liitem.Expected,
                            item.Quantity*cost, item.IsEditable, consolidatedproduct,
                            liitem == null ? false : liitem.BreakBulk,
                            _reason, item.ParentProductId);
            }

            _productPackagingSummaryService.ClearBuffer();
            temp.Clear();
        }

        public RelayCommand LoadGrn { get; set; }

        private void RunLoadGrn()
        {
             using (StructureMap.IContainer c = NestedContainer)
             {
               InventoryReceivedNote irn = Using<IInventoryReceivedNoteRepository>(c).GetById(IRNId);
                 if (irn != null)
                 {
                     DocumentReference = irn.DocumentReference;
                     OrderReferences = irn.OrderReferences;
                     LoadNo = irn.LoadNo;
                     foreach (var item in irn.LineItems)
                     {
                         if (item.Product is SaleProduct || item.Product is ConsolidatedProduct)
                             _productPackagingSummaryService.AddProduct(item.Product.Id, item.Qty);
                     }
                     var prods = _productPackagingSummaryService.GetProductSummary();
                     foreach (var item in prods)
                     {
                         var li = irn.LineItems.FirstOrDefault(n => n.Product.Id == item.Product.Id);
                         AddLineItem(Guid.NewGuid(), item.Product.Id, item.Product.Description,
                                     li != null ? li.Value : 0,
                                     item.Quantity,
                                    li != null ? li.Expected: 0,
                                     item.Quantity*(li != null ? li.Value : 0), false, false, false,
                                     li != null ? li.Description : "", item.ParentProductId);
                     }
                 }
             }
        }

        private string NewGrnDocReference()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                Guid costCentreId = Using<IConfigService>(c) .Load().CostCentreId;
                Guid applicationId = Using<IConfigService>(c).Load().CostCentreApplicationId;
                int grnSequenceId = Using<IInventoryReceivedNoteRepository>(c).GetAll().Count() + 1;
                CostCentre cc = Using<ICostCentreRepository>(c).GetById(costCentreId);
                string date = DateTime.Now.ToString("yyyy.MM.dd");
                const string formatString = "Dist_GRN_{0}_{1}_{2}";
                return string.Format(formatString, cc.Name, date, grnSequenceId);
            }
        }

       

        void RunCancel()
        {
            if (CancelAction == "Back")
            {
                ConfirmNavigatingAway = false;
                SendNavigationRequestMessage(new Uri("/views/grn/listgrn.xaml", UriKind.Relative));
            }
            else
            {
                if (MessageBox.Show("Are you sure you want to cancel? \n Unsaved changes will be lost",
                                    "Receive Inventory", MessageBoxButton.OK) == MessageBoxResult.OK)
                {
                    SendNavigationRequestMessage(new Uri("/views/grn/listgrn.xaml", UriKind.Relative));
                }
            }
        }

       

        private void RunClearAndSetup()
        {
            IRNId = Guid.Empty;
            OrderReferences = null;
            DocumentReference = null;
            LoadNo = null;
            CreatedByUser = null;
            TotalCost = 0;
            IsEditable = true;
            DateReceived = DateTime.Now;
            LineItems.Clear();
            _inventorySerialNumbersList.Clear();
            _productPackagingSummaryService.ClearBuffer();
            CancelAction = "Cancel";
        }

        

        private void AddLineItem(Guid lineItemId, Guid productId, string productDesc, decimal unitCost, decimal qty,
            decimal expected, decimal lineItemTotal, bool isEditable, bool isconsolidatedProduct, bool breakBulk, string reason,Guid parentproductId)
        {
            var item = LineItems.FirstOrDefault(n => n.ProductId == productId);
            if (item != null)
            {
                item.ProductId = productId;
                item.Product = productDesc;
                item.UnitCost = unitCost;
                item.Qty = qty;
                item.Expected = expected;
                item.isConsolidatedProduct = isconsolidatedProduct;
                item.BreakBulk = breakBulk;
                item.Reason = reason;
                item.ParentProductId = parentproductId;
            }
            else
            {
                int sequenceNo = 1;
                if (LineItems.Any())
                {
                    sequenceNo = LineItems.Max(n => n.SequenceNo) + 1;
                }

                item = new AddGrnLineItemViewModel
                {
                    SequenceNo = sequenceNo,
                    ProductId = productId,
                    Product = productDesc,
                    UnitCost = unitCost,
                    Qty = qty,
                    Expected = expected,
                    IsEditable = isEditable,
                    LineItemId = lineItemId,
                    LineItemTotal = lineItemTotal,
                    isConsolidatedProduct = isconsolidatedProduct,
                    BreakBulk = breakBulk,
                    Reason = reason ,
                    ParentProductId = parentproductId
                };
                LineItems.Add(item);
            }
            CalcTotals();
        }

       
       

        void RemoveLineItem(AddGrnLineItemViewModel lineItem)
        {
            if (lineItem.ParentProductId != Guid.Empty)
            {
                if (MessageBox.Show("Are you sure you want to remove the selected product?", "Distributr: Confirm Action", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    var items = LineItems.Where(n => n.ParentProductId == lineItem.ParentProductId).ToList();
                    foreach (var itemtoDelete in items)
                    {
                        LineItems.Remove(itemtoDelete);
                    }
                    RefreshLineItem();
                    CalcTotals();
                }
            }
          
        }

        private void UpdateModalItems(GRNModalItems modalItems,bool Edit = false)
        {
            foreach (var item in modalItems.LineItems)
            {
                var p = GetEntityById(typeof (Product), item.SelectedProduct.ProductId) as Product;
                if (p is ConsolidatedProduct || p is SaleProduct)
                    _productPackagingSummaryService.AddProduct(p.Id, item.Qty, false, Edit, false);
            }
            _reason = modalItems.Reason;
          InsertSerials(modalItems.InventorySerials);
            //RefreshLineItem();
            EditLineItem(modalItems.LineItems);
        }
        protected void EditLineItem(List<GRNLineItems> modalItems)
        {
            foreach (var lineItem in modalItems)
            {
                var item = LineItems.FirstOrDefault(p => p.ProductId == lineItem.SelectedProduct.ProductId);
                if (item != null)
                {
                    item.Qty = lineItem.Qty;
                    item.Expected = lineItem.Expected;
                }
            }
            CalcTotals();
        }

        void InsertSerials(List<ProductSerialNumbers> inventorySerials)
        {
            inventorySerials.ForEach(n =>
                                         {
                                             var existing = _inventorySerialNumbersList.FirstOrDefault(e => e.SerialsId == n.SerialsId);
                                             if (existing == null)
                                                 _inventorySerialNumbersList.Add(n);
                                             else
                                             {
                                                 existing.From = n.From;
                                                 existing.To = n.To;
                                                 existing.LineItemId = n.LineItemId;
                                             }
                                         });
        }

        void CalcTotals()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                foreach (var item in LineItems)
                {
                    var vat = 0;
                    item.LineItemTotal = item.Qty*(item.UnitCost + vat);
                }
                TotalCost = LineItems.Sum(n => n.LineItemTotal);
            }
        }

       

        decimal ProductPriceCalc(Product product)
        {
           
            decimal prodprice = 0;
            if (product is ConsolidatedProduct)
                try
                {
                    prodprice =product.ExFactoryPrice;
                }
                catch
                {
                    prodprice = 0m;
                }
            else
                try
                {
                    prodprice = product.ExFactoryPrice;//.First(n => n.Tier.Id == tier.Id).CurrentExFactory;
                }
                catch
                {
                    prodprice = 0m;
                }


            return prodprice;
        }

       

        public const string IRNIdPropertyName = "IRNId";
        private Guid _irnId = Guid.Empty;
        public Guid IRNId
        {
            get
            {
                return _irnId;
            }

            set
            {
                if (_irnId == value)
                    return;

                var oldValue = _irnId;
                _irnId = value;

                RaisePropertyChanged(IRNIdPropertyName);
            }
        }

        public const string OrderReferencesPropertyName = "OrderReferences";
        private string _orderReferences = "";
        public string OrderReferences
        {
            get
            {
                return _orderReferences;
            }

            set
            {
                if (_orderReferences == value)
                    return;

                var oldValue = _orderReferences;
                _orderReferences = value;
                RaisePropertyChanged(OrderReferencesPropertyName);
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
                    return;
                var oldValue = _documentReference;
                _documentReference = value;
                RaisePropertyChanged(DocumentReferencePropertyName);
            }
        }


        public const string EnterLoadNoIsEnabledPropertyName = "EnterLoadNoIsEnabled";
        private bool _enterLoadNoIsEnabled;
        public bool EnterLoadNoIsEnabled
        {
            get
            {
                return _enterLoadNoIsEnabled;
            }

            set
            {
                if (_enterLoadNoIsEnabled == value)
                    return;

                _enterLoadNoIsEnabled = value;

                RaisePropertyChanged(EnterLoadNoIsEnabledPropertyName);
            }
        }

        public const string ShowConfirmVisibilityPropertyName = "ShowConfirmVisibility";
        private Visibility _showConfirmVisibility = Visibility.Visible;
        public Visibility ShowConfirmVisibility
        {
            get
            {
                return _showConfirmVisibility;
            }

            set
            {
                if (_showConfirmVisibility == value)
                    return;

                _showConfirmVisibility = value;

                RaisePropertyChanged(ShowConfirmVisibilityPropertyName);
            }
        }

        public const string ShowAddVisibilityPropertyName = "ShowAddVisibility";
        private Visibility _showAddVisibility=Visibility.Visible;
        public Visibility ShowAddVisibility
        {
            get
            {
                return _showAddVisibility;
            }

            set
            {
                if (_showAddVisibility == value)
                    return;

                _showAddVisibility = value;

                RaisePropertyChanged(ShowAddVisibilityPropertyName);
            }
        }

        

        public const string OrderReferencesIsEnabledPropertyName = " OrderReferencesIsEnabled";
        private bool _orderReferencesIsEnabled;
        public bool OrderReferencesIsEnabled
        {
            get
            {
                return _orderReferencesIsEnabled;
            }

            set
            {
                if (_orderReferencesIsEnabled == value)
                    return;

                _orderReferencesIsEnabled = value;

                RaisePropertyChanged(OrderReferencesIsEnabledPropertyName);
            }
        }

        public const string LoadNoPropertyName = "LoadNo";
        private string _loadNo = "";
        public string LoadNo
        {
            get
            {
                return _loadNo;
            }

            set
            {
                if (_loadNo == value)
                    return;

                var oldValue = _loadNo;
                _loadNo = value;

                RaisePropertyChanged(LoadNoPropertyName);
            }
        }

        public const string IsEditablePropertyName = "IsEditable";
        private bool _myProperty = true;
        public bool IsEditable
        {
            get
            {
                return _myProperty;
            }

            set
            {
                if (_myProperty == value)
                    return;
                var oldValue = _myProperty;
                _myProperty = value;
                RaisePropertyChanged(IsEditablePropertyName);
            }
        }

        public const string DateReceivedPropertyName = "DateReceived";
        private DateTime _dateReceived = DateTime.Now;
        public DateTime DateReceived
        {
            get
            {
                return _dateReceived;
            }

            set
            {
                if (_dateReceived == value)
                    return;
                var oldValue = _dateReceived;
                _dateReceived = value;
                RaisePropertyChanged(DateReceivedPropertyName);
            }
        }

        public const string CreatedByUserPropertyName = "CreatedByUser";
        private string _createdByUser = "";
        public string CreatedByUser
        {
            get
            {
                return _createdByUser;
            }

            set
            {
                if (_createdByUser == value)
                    return;
                var oldValue = _createdByUser;
                _createdByUser = value;
                RaisePropertyChanged(CreatedByUserPropertyName);
            }
        }

        public const string TotalCostPropertyName = "TotalCost";
        private decimal _totalCost = 0;
        public decimal TotalCost
        {
            get
            {
                return _totalCost;
            }

            set
            {
                if (_totalCost == value)
                    return;
                var oldValue = _totalCost;
                _totalCost = value;
                RaisePropertyChanged(TotalCostPropertyName);
            }
        }

        public const string CancelActionPropertyName = "CancelAction";
        private string _CancelAction = null;
        public string CancelAction
        {
            get
            {
                return _CancelAction;
            }

            set
            {
                if (_CancelAction == value)
                {
                    return;
                }

                var oldValue = _CancelAction;
                _CancelAction = value;


                // Update bindings, no broadcast
                RaisePropertyChanged(CancelActionPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="NS" /> property's name.
        /// </summary>
        public const string NSPropertyName = "NS";
        private NavigationService _NS;
        /// <summary>
        /// Gets the NS property.
        
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public NavigationService NS
        {
            get
            {
                return _NS;
            }

            set
            {
                if (_NS == value)
                {
                    return;
                }

                var oldValue = _NS;
                _NS = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(NSPropertyName);
            }
        }
    }

    #region Helper Classes
    public class AddGrnLineItemViewModel : DistributrViewModelBase
    {
        
        public const string LineItemIdPropertyName = "LineItemId";
        private Guid _lineItemId = Guid.Empty;
        public Guid LineItemId
        {
            get
            {
                return _lineItemId;
            }

            set
            {
                if (_lineItemId == value)
                    return;
                var oldValue = _lineItemId;
                _lineItemId = value;
                RaisePropertyChanged(LineItemIdPropertyName);
            }
        }

        public const string SequenceNoPropertyName = "SequenceNo";
        private int _sequenceNo = -1;
        public int SequenceNo
        {
            get
            {
                return _sequenceNo;
            }

            set
            {
                if (_sequenceNo == value)
                    return;
                var oldValue = _sequenceNo;
                _sequenceNo = value;
                RaisePropertyChanged(SequenceNoPropertyName);
            }
        }

        public Guid ProductId { get; set; }

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

        public const string QtyPropertyName = "Qty";
        private decimal _qty;
        public decimal Qty
        {
            get
            {
                return _qty;
            }

            set
            {
                if (_qty == value)
                {
                    return;
                }

                var oldValue = _qty;
                _qty = value;
                // Update bindings, no broadcast
                RaisePropertyChanged(QtyPropertyName);
            }
        }

        public const string ExpectedPropertyName = "Expected";
        private decimal _expected;
        public decimal Expected
        {
            get
            {
                return _expected;
            }

            set
            {
                if (_expected == value)
                    return;
                var oldValue = _expected;
                _expected = value;
                RaisePropertyChanged(ExpectedPropertyName);
            }
        }

        public const string IsEditablePropertyName = "IsEditable";
        private bool _myProperty = true;
        public bool IsEditable
        {
            get
            {
                return _myProperty;
            }

            set
            {
                if (_myProperty == value)
                    return;
                var oldValue = _myProperty;
                _myProperty = value;
                RaisePropertyChanged(IsEditablePropertyName);
            }
        }

        public const string UnitCostPropertyName = "UnitCost";
        private decimal _unitCost;
        public decimal UnitCost
        {
            get
            {
                return _unitCost;
            }

            set
            {
                if (_unitCost == value)
                    return;
                var oldValue = _unitCost;
                _unitCost = value;
                RaisePropertyChanged(UnitCostPropertyName);
            }
        }

        public const string LineItemTotalPropertyName = "LineItemTotal";
        private decimal _lineItemTotal;
        public decimal LineItemTotal
        {
            get
            {
                return _lineItemTotal;
            }

            set
            {
                if (_lineItemTotal == value)
                    return;
                var oldValue = _lineItemTotal;
                _lineItemTotal = value;
                RaisePropertyChanged(LineItemTotalPropertyName);
            }
        }

        public const string isConsolidatedProductPropertyName = "isConsolidatedProduct";
        private bool _isConsolidatedProduct;
        public bool isConsolidatedProduct
        {
            get
            {
                return _isConsolidatedProduct;
            }

            set
            {
                if (_isConsolidatedProduct == value)
                {
                    return;
                }

                var oldValue = _isConsolidatedProduct;
                _isConsolidatedProduct = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(isConsolidatedProductPropertyName);
            }
        }

        public const string BreakBulkPropertyName = "BreakBulk";
        private bool _BreakBulk;
        public bool BreakBulk
        {
            get
            {
                return _BreakBulk;
            }

            set
            {
                if (_BreakBulk == value)
                {
                    return;
                }

                var oldValue = _BreakBulk;
                _BreakBulk = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(BreakBulkPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="Reason" /> property's name.
        /// </summary>
        public const string ReasonPropertyName = "Reason";
        private string _Reason;
        /// <summary>
        /// Gets the Reason property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public string Reason
        {
            get
            {
                return _Reason;
            }

            set
            {
                if (_Reason == value)
                {
                    return;
                }

                var oldValue = _Reason;
                _Reason = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(ReasonPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="VatAmount" /> property's name.
        /// </summary>
        public const string VatAmountPropertyName = "VatAmount";
        private decimal _VatAmount;
        /// <summary>
        /// Gets the VatAmount property.
        
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public decimal VatAmount
        {
            get
            {
                return _VatAmount;
            }

            set
            {
                if (_VatAmount == value)
                {
                    return;
                }

                var oldValue = _VatAmount;
                _VatAmount = value;

                // Remove one of the two calls below
                throw new NotImplementedException();

                // Update bindings, no broadcast
                RaisePropertyChanged(VatAmountPropertyName);

                // Update bindings and broadcast change using GalaSoft.MvvmLight.Messenging
                RaisePropertyChanged(VatAmountPropertyName, oldValue, value, true);
            }
        }

        
        public const string ParentProductIdPropertyName = "ParentProductId";
        private Guid _parentproductId = Guid.Empty;
        public Guid ParentProductId
        {
            get
            {
                return _parentproductId;
            }

            set
            {
                if (_parentproductId == value)
                {
                    return;
                }

                var oldValue = _parentproductId;
                _parentproductId = value;
                // Update bindings, no broadcast
                RaisePropertyChanged(ParentProductIdPropertyName);
            }
        }
    }

    public class ProductSerialNumbers
    {
        public Guid SerialsId { get; set; }
        public Guid ProductId { get; set; }
        public Guid LineItemId { get; set; }
        public string From { get; set; }
        public string To { get; set; }
    }

    #endregion
}
