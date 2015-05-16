using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Factory.Documents;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.CreditNoteRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IInvoiceRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.Services.Service.Utility;
using GalaSoft.MvvmLight;
using System.Linq;
using Distributr.Core.ClientApp;
using GalaSoft.MvvmLight.Command;

namespace Distributr.WPF.Lib.ViewModels.Transactional.CN
{
    public class AddCreditNoteModel : DistributrViewModelBase
    {

        public AddCreditNoteModel()
        {
            CreditLineItem = new ObservableCollection<CreditNoteLineItemView>();
            ConfirmCommand = new RelayCommand(Confrim);
            CancelCommand = new RelayCommand(Cancel);
        }

        

        #region Properties
        private string OrderRef = "";

        public const string InvoiceRefPropertyName = "InvoiceRef";
        private string _invoiceRef = "new";
        public string InvoiceRef
        {
            get
            {
                return _invoiceRef;
            }

            set
            {
                if (_invoiceRef == value)
                {
                    return;
                }

                var oldValue = _invoiceRef;
                _invoiceRef = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(InvoiceRefPropertyName);


            }
        }
        public const string OrderIdPropertyName = " OrderId";
        private Guid _orderId = Guid.Empty;
        public Guid OrderId
        {
            get
            {
                return _orderId;
            }

            set
            {
                if (_orderId == value)
                {
                    return;
                }
                _orderId = value;
                // Update bindings, no broadcast
                RaisePropertyChanged(OrderIdPropertyName);


            }
        }
        public const string InvoiceIdPropertyName = "InvoiceId";
        private Guid _invoiceId = Guid.Empty;
        public Guid InvoiceId
        {
            get
            {
                return _invoiceId;
            }

            set
            {
                if (_invoiceId == value)
                {
                    return;
                }

                var oldValue = _invoiceId;
                _invoiceId = value;
                // Update bindings, no broadcast
                RaisePropertyChanged(InvoiceIdPropertyName);


            }
        }

        public const string InvoiceAmountPropertyName = "InvoiceAmount";
        private decimal _amount = 0;
        public decimal InvoiceAmount
        {
            get
            {
                return _amount;
            }

            set
            {
                if (_amount == value)
                {
                    return;
                }

                var oldValue = _amount;
                _amount = value;



                // Update bindings, no broadcast
                RaisePropertyChanged(InvoiceAmountPropertyName);


            }
        }

        public const string PreviousCreditAmountPropertyName = "PreviousCreditAmount";
        private decimal _pCreditAmount = 0;
        public decimal PreviousCreditAmount
        {
            get
            {
                return _pCreditAmount;
            }

            set
            {
                if (_pCreditAmount == value)
                {
                    return;
                }

                var oldValue = _pCreditAmount;
                _pCreditAmount = value;



                // Update bindings, no broadcast
                RaisePropertyChanged(PreviousCreditAmountPropertyName);


            }
        }

        public const string CreditAmountPropertyName = "CreditAmount";
        private decimal _creditAount = 0;
        public decimal CreditAmount
        {
            get
            {
                return _creditAount;
            }

            set
            {
                if (_creditAount == value)
                {
                    return;
                }

                var oldValue = _creditAount;
                _creditAount = value;



                // Update bindings, no broadcast
                RaisePropertyChanged(CreditAmountPropertyName);


            }
        }

        public RelayCommand ConfirmCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }

        public ObservableCollection<CreditNoteLineItemView> CreditLineItem { get; set; }
        private List<InvoiceLineItem> invoiceLineItemList;
        public void Setup(Guid invoiceId)
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                Reset();
                invoiceLineItemList = new List<InvoiceLineItem>();
                Invoice invo = Using<IInvoiceRepository>(c).GetById(invoiceId) as Invoice;
                decimal creditnotesAmount =
                    Using<ICreditNoteRepository>(c)
                    .GetAll()
                    .OfType<CreditNote>()
                    .Where(s => s.InvoiceId == invoiceId)
                    .Sum(o => o.Total);
                if (invo != null)
                {
                    InvoiceId = invo.Id;
                    OrderId = invo.OrderId;
                    InvoiceAmount = invo.TotalGross;
                    PreviousCreditAmount = creditnotesAmount;
                    InvoiceRef = invo.DocumentReference;
                    invoiceLineItemList = invo.LineItems;
                    Order o = Using<IOrderRepository>(c).GetById(invo.OrderId) as Order;
                    OrderRef = o.DocumentReference;
                }
            }
        }

        #endregion

        private void Reset()
        {
            CreditLineItem.Clear();
            InvoiceId = Guid.Empty;
            InvoiceAmount = 0;
            PreviousCreditAmount = 0;
            InvoiceRef = "";
            CreditAmount = 0;
            TempCreditLineItem.Clear();
        }

        private List<CreditNoteLineItemView> TempCreditLineItem = new List<CreditNoteLineItemView>();

        public void AddLineItem(AddCreditNoteLineViewModel item)
        {

            CreditNoteLineItemView line = TempCreditLineItem.FirstOrDefault(p => p.ProductId == item.ProductLookUp.ProductId);
            if (line == null)
            {
                line = new CreditNoteLineItemView();
                TempCreditLineItem.Add(line);
                line.ProductId = item.ProductLookUp.ProductId;
                line.ProductName = item.ProductLookUp.ProductName;
            }
            line.Quantity = item.QuantityRequired;
            line.UnitPrice = item.ProductLookUp.UnitPrice;
            line.Reason = item.Reason;
            refreshList();
        }

        public CreditNoteLineItemView LoadForEdit(Guid productId)
        {
            CreditNoteLineItemView line = TempCreditLineItem.FirstOrDefault(p => p.ProductId == productId);
            return line;

        }

        private void Cancel()
        {
            MessageBoxResult isConfirmed = MessageBox.Show("Are sure you want to cancel ", "Issue Credit Line item", MessageBoxButton.OKCancel);

            if (isConfirmed == MessageBoxResult.OK)
            {
                SendNavigationRequestMessage(new Uri("/views/CN/ListInvoices.xaml", UriKind.Relative));
            }
        }

        private void Confrim()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                
                if ((CreditAmount + PreviousCreditAmount) > InvoiceAmount)
                {
                    MessageBox.Show("Credit note(s) Amount cant be more than Invoice amount ", "Issue Credit Note",
                                    MessageBoxButton.OK);
                    return;
                }
                else if (CreditLineItem.Count == 0)
                {
                    MessageBox.Show("Add Credit note Line Item  ", "Issue Credit Note", MessageBoxButton.OK);
                    return;
                }
                else
                {

                    var creditNoteFactory = Using<ICreditNoteFactory>(c);
                    var configService = Using<IConfigService>(c);
                    var config = configService.Load();
                    var costcentreRepository = Using<ICostCentreRepository>(c);

                    string creditNoteReference = Using<IGetDocumentReference>(c).GetDocReference("CN", OrderRef);
                    var documentIssuerCostCentre = costcentreRepository.GetById(config.CostCentreId);

                    var documentIssuerCostCentreApplicationId = config.CostCentreApplicationId;
                    var documentIssuerUser =
                        Using<IUserRepository>(c).GetById(configService.ViewModelParameters.CurrentUserId);
                    var documentRecipientCostCentre = costcentreRepository.GetById(config.CostCentreId);

                    CreditNote note = creditNoteFactory.Create(documentIssuerCostCentre,
                                                               documentIssuerCostCentreApplicationId,
                                                               documentRecipientCostCentre, documentIssuerUser,
                                                               creditNoteReference, OrderId, InvoiceId);

                    var inventoryAdjustmentNoteFactory = Using<InventoryAdjustmentNoteFactory>(c);

                    var inventoryAdjustment = inventoryAdjustmentNoteFactory.Create(note.DocumentIssuerCostCentre,
                                                                                    note.
                                                                                        DocumentIssuerCostCentreApplicationId,
                                                                                    note.DocumentIssuerCostCentre,
                                                                                    note.DocumentIssuerUser,
                                                                                    "Adjustment Note",
                                                                                    InventoryAdjustmentNoteType.
                                                                                        Available, note.DocumentParentId);

                    foreach (var item in CreditLineItem)
                    {
                        var itnLineitem = creditNoteFactory.CreateLineItem(item.ProductId, item.Quantity,
                                                                           item.UnitPrice, item.Reason, 0, 0, 0);

                        note.AddLineItem(itnLineitem);
                        var adjustmentLineItem = inventoryAdjustmentNoteFactory.CreateLineItem(0, item.ProductId,
                                                                                               item.Quantity, 0,
                                                                                               "Adjust lineitem");
                        inventoryAdjustment.AddLineItem(adjustmentLineItem);

                    }


                    note.Confirm();
                    Using<IConfirmCreditNoteWFManager>(c).SubmitChanges(note,config);
                    inventoryAdjustment.Confirm();
                    Using<IInventoryAdjustmentNoteWfManager>(c).SubmitChanges(inventoryAdjustment,config);

                    MessageBox.Show("Credit Note Issued Succesfully");
                    SendNavigationRequestMessage(new Uri("/views/CN/ListInvoices.xaml", UriKind.Relative));


                }

            }
        }

        private void refreshList()
        {
            _productPackagingSummaryService.ClearBuffer();
            foreach (var i in TempCreditLineItem.ToList())
            {
                _productPackagingSummaryService.AddProduct(i.ProductId, i.Quantity, false, false, false);
            }
            CreditLineItem.Clear();
            foreach (PackagingSummary ps in _productPackagingSummaryService.GetProductSummary())
            {
                CreditNoteLineItemView view = new CreditNoteLineItemView();
                InvoiceLineItem invoicelineItem =
                    invoiceLineItemList.FirstOrDefault(p => p.Product.Id == ps.Product.Id);
                CreditNoteLineItemView TempLine =
                    TempCreditLineItem.FirstOrDefault(p => p.ProductId == ps.ParentProductId);
                decimal Unitprice = invoicelineItem != null
                                        ? invoicelineItem.Value + invoicelineItem.LineItemVatValue
                                        : 0;
                string Reason = TempLine != null ? TempLine.Reason : "";
                CreditLineItem.Add(view);
                view.ProductId = ps.Product.Id;
                view.ProductName = ps.Product.Description;
                view.Quantity = ps.Quantity;
                view.UnitPrice = Unitprice;
                view.TotalPrice = Unitprice*ps.Quantity;
                view.Reason = Reason;
                view.IsEditable = ps.IsEditable;
            }
            CreditAmount = CreditLineItem.Sum(s => s.TotalPrice);
        }

        public void RemoveLineItem(Guid ParentProductid)
        {
            MessageBoxResult isConfirmed = MessageBox.Show("Are sure you want to delete ", "Delete Credit Line item", MessageBoxButton.OKCancel);

            if (isConfirmed == MessageBoxResult.OK)
            {
                CreditNoteLineItemView TempLine = TempCreditLineItem.FirstOrDefault(p => p.ProductId == ParentProductid);
                TempCreditLineItem.Remove(TempLine);
                refreshList();
            }
        }
    }

    #region Helper Classes

    public class CreditNoteLineItemView : ViewModelBase
    {
        public string dProductName { get; set; }

        public const string ProductNamePropertyName = "ProductName";
        private string _ProductName = "";
        public string ProductName
        {
            get
            {
                return _ProductName;
            }

            set
            {
                if (_ProductName == value)
                {
                    return;
                }

                var oldValue = _ProductName;
                _ProductName = value;


                // Update bindings, no broadcast
                RaisePropertyChanged(ProductNamePropertyName);


            }
        }

        public const string ProductIdPropertyName = "ProductId";
        private Guid _ProductId = Guid.Empty;
        public Guid ProductId
        {
            get
            {
                return _ProductId;
            }

            set
            {
                if (_ProductId == value)
                {
                    return;
                }

                var oldValue = _ProductId;
                _ProductId = value;



                // Update bindings, no broadcast
                RaisePropertyChanged(ProductIdPropertyName);

            }
        }

        public const string QuantityPropertyName = "Quantity";
        private decimal _Quantity = 0;
        public decimal Quantity
        {
            get
            {
                return _Quantity;
            }

            set
            {
                if (_Quantity == value)
                {
                    return;
                }

                var oldValue = _Quantity;
                _Quantity = value;


                // Update bindings, no broadcast
                RaisePropertyChanged(QuantityPropertyName);

            }
        }

        public const string UnitPricePropertyName = "UnitPrice";
        private decimal _UnitPrice = 0;
        public decimal UnitPrice
        {
            get
            {
                return _UnitPrice;
            }

            set
            {
                if (_UnitPrice == value)
                {
                    return;
                }

                var oldValue = _UnitPrice;
                _UnitPrice = value;
                // Update bindings, no broadcast
                RaisePropertyChanged(UnitPricePropertyName);


            }
        }

        public const string TotalPricePropertyName = "TotalPrice";
        private decimal _TotalPrice = 0;
        public decimal TotalPrice
        {
            get
            {
                return _TotalPrice;
            }

            set
            {
                if (_TotalPrice == value)
                {
                    return;
                }

                var oldValue = _TotalPrice;
                _TotalPrice = value;


                // Update bindings, no broadcast
                RaisePropertyChanged(TotalPricePropertyName);


            }
        }

        public const string ReasonPropertyName = "Reason";
        private string _Reason = "";
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

        public const string IsEditablePropertyName = "IsEditable";
        private bool _IsEditable = true;
        public bool IsEditable
        {
            get
            {
                return _IsEditable;
            }

            set
            {
                if (_IsEditable == value)
                {
                    return;
                }

                var oldValue = _IsEditable;
                _IsEditable = value;


                // Update bindings, no broadcast
                RaisePropertyChanged(IsEditablePropertyName);


            }
        }
    }


    public class AddCreditNoteLineViewModel : DistributrViewModelBase
    {

        public AddCreditNoteLineViewModel()
        {
            ProductLookUpList = new ObservableCollection<CreditNoteProductLookUp>();
        }

        public const string ProductLookUpPropertyName = "ProductLookUp";
        private CreditNoteProductLookUp _productLookUp = null;
        public CreditNoteProductLookUp ProductLookUp
        {
            get
            {
                return _productLookUp;
            }

            set
            {
                if (_productLookUp == value)
                {
                    return;
                }

                var oldValue = _productLookUp;
                _productLookUp = value;



                // Update bindings, no broadcast
                RaisePropertyChanged(ProductLookUpPropertyName);


            }
        }

        public ObservableCollection<CreditNoteProductLookUp> ProductLookUpList { set; get; }

        public const string QuantityIssuedPropertyName = "QuantityIssued";
        private decimal _quantityIssued = 0;
        public decimal QuantityIssued
        {
            get
            {
                return _quantityIssued;
            }

            set
            {
                if (_quantityIssued == value)
                {
                    return;
                }

                var oldValue = _quantityIssued;
                _quantityIssued = value;



                // Update bindings, no broadcast
                RaisePropertyChanged(QuantityIssuedPropertyName);

            }
        }


        public const string QuantityRequiredPropertyName = "QuantityRequired";
        private decimal _quantityRequired = 0;
        public decimal QuantityRequired
        {
            get
            {
                return _quantityRequired;
            }

            set
            {
                if (_quantityRequired == value)
                {
                    return;
                }

                var oldValue = _quantityRequired;
                _quantityRequired = value;



                // Update bindings, no broadcast
                RaisePropertyChanged(QuantityRequiredPropertyName);


            }
        }


        public const string ReasonPropertyName = "Reason";
        private string _reason = "";

        public string Reason
        {
            get
            {
                return _reason;
            }

            set
            {
                if (_reason == value)
                {
                    return;
                }

                var oldValue = _reason;
                _reason = value;


                // Update bindings, no broadcast
                RaisePropertyChanged(ReasonPropertyName);

            }
        }


        public void Setup(Guid invoiceId)
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
                ClearModal();
                Invoice invoice = Using<IInvoiceRepository>(cont).GetById(invoiceId) as Invoice;
                var InvoiceLineItem = invoice.LineItems.ToList();
                foreach (InvoiceLineItem item in InvoiceLineItem)
                {
                    if (item.Product is SaleProduct || item.Product is ConsolidatedProduct)
                    {
                        ProductLookUpList.Add(Map(item));
                    }
                }
            }
        }

        private void ClearModal()
        {
            QuantityRequired = 0;
            QuantityIssued = 0;
            Reason = "";
            ProductLookUp = null;
            ProductLookUpList.Clear();
        }

        public void SetupProductQuantity()
        {
            QuantityIssued = ProductLookUp.InvoiceQuantity;
        }
        private CreditNoteProductLookUp Map(InvoiceLineItem s)
        {

            return new CreditNoteProductLookUp
                       {
                           ProductId = s.Product.Id,
                           ProductName = s.Product.Description,
                           InvoiceQuantity = s.Qty,
                           UnitPrice = s.Value,

                       };
        }
    }
    public class CreditNoteProductLookUp
    {
        public Guid ProductId { set; get; }
        public string ProductName { set; get; }
        public decimal InvoiceQuantity { set; get; }
        public decimal UnitPrice { set; get; }
    }

    #endregion
}