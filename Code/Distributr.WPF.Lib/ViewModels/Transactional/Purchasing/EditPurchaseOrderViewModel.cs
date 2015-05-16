using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.Service.Utility;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.Services.WorkFlow.Discount;
using Distributr.WPF.Lib.Services.WorkFlow.Orders;
using Distributr.WPF.Lib.UI.Hierarchy;
using Distributr.WPF.Lib.WorkFlow.Orders;
using GalaSoft.MvvmLight.Command;
using System.Linq;
using Distributr.Core.Domain.Master.CostCentreEntities;

namespace Distributr.WPF.Lib.ViewModels.Transactional.Purchasing
{
    public class EditPurchaseOrderViewModel : DistributrViewModelBase
    {
        public EditPurchaseOrderViewModel()
        {
            Load = new RelayCommand(DoLoad);
            Cancel = new RelayCommand(DoCancel);
            SavePendingCommand = new RelayCommand(DoSavePending);
            ConfirmOrderCommand = new RelayCommand(DoConfirmOrder);
            
            LineItems = new ObservableCollection<EditPurchaseOrderItem>();
            _LineItems = new ObservableCollection<EditPurchaseOrderItem>();
        }


        public RelayCommand ConfirmOrderCommand { get; set; }
       
        void DoConfirmOrder()
        {
            using (var container = NestedContainer)
            {
                IOrderRepository _orderService = Using<IOrderRepository>(container);
                
                var  _confirmPurchaseOrderWFManager =
                    Using<IConfirmPurchaseOrderWFManager>(container);
                
                if (LineItems.Count == 0)
                {
                    MessageBox.Show(
                        "Make sure you have atleast 1 Product in the Purchase Order ref No." + OrderId.ToString(),
                        "Confirm Purchase Order", MessageBoxButton.OK);
                    return;
                }
                MessageBoxResult isConfirmed =
                    MessageBox.Show(
                        "Confirm " + GetLocalText("sl.po.name") + " ref No." + OrderId.ToString(),
                        "Confirm " + GetLocalText("sl.po.name"), MessageBoxButton.OKCancel);

                if (isConfirmed == MessageBoxResult.OK)
                {
                    DateTime start = DateTime.Now;
                    SavePendingOrder();
                    Order o = _orderService.GetById(OrderIdLookup) as Order;
                    _confirmPurchaseOrderWFManager.SubmitChanges(o);
                    TimeSpan diff = DateTime.Now.Subtract(start);
                    ButtonCancelName = "Close";
                    ConfirmNavigatingAway = false;
                    SendNavigationRequestMessage(
                        new Uri("/views/purchasing/listpurchaseorders.xaml?tab=tabItemPendingAproval", UriKind.Relative));

                }
            }

        }

        public RelayCommand Cancel { get; set; }
        void DoCancel()
        {
            if (ButtonCancelName == "Cancel")
            {
                MessageBoxResult isConfirmed = MessageBox.Show("Are you sure you want to cancel " + OrderId.ToString(),
                                                               GetLocalText("sl.po.name"), MessageBoxButton.OKCancel);
                ButtonCancelName = "Close";
                if (isConfirmed == MessageBoxResult.OK)
                {
                    ConfirmNavigatingAway = false;
                    SendNavigationRequestMessage(new Uri("/views/purchasing/listpurchaseorders.xaml?tab=tabItemIncomplete", UriKind.Relative));
                }
            }else
            {
                ConfirmNavigatingAway = false;
                SendNavigationRequestMessage(new Uri("/views/purchasing/listpurchaseorders.xaml?tab=tabItemIncomplete", UriKind.Relative));
                
            }

        }

        public RelayCommand SavePendingCommand { get; set; }
        void DoSavePending()
        {
           

            SavePendingOrder();
            ButtonCancelName = "Close";
            MessageBoxResult isConfirmed =
                MessageBox.Show( GetLocalText("sl.po.name")+
                    " saved. Press Okay to view incomplete  " + GetLocalText("sl.po.name") + " and Cancel to Continue working on current " + GetLocalText("sl.po.name"),
                     GetLocalText("sl.po.name"), MessageBoxButton.OKCancel);
            if (isConfirmed == MessageBoxResult.OK)
                SendNavigationRequestMessage(new Uri("/views/purchasing/listpurchaseorders.xaml?tab=tabItemIncomplete",UriKind.Relative));



        }

        private void SavePendingOrder()
        {
            using (var container = NestedContainer)
            {
                IOrderRepository _orderService = Using<IOrderRepository>(container);
                
                if (OrderIdLookup == Guid.Empty)
                {
                    CreateNewOrder();
                }

                Order order = _orderService.GetById(OrderIdLookup) as Order;

                foreach (var item in LineItems)
                {
                    AddLineItem(item.ProductId, item.Product, item.UnitPrice, item.LineItemVatValue,
                                item.VatAmount, item.Vat, item.Qty, item.TotalPrice, item.IsEditable);
                }
                foreach (var item in _LineItems)
                {

                    if (item.LineItemId == Guid.Empty)
                    {
                        // order.AddLineItem(CreateNewLineItem(item, order));
                        _orderService.SaveLineItem(CreateNewLineItem(item, order), order.Id);
                    }
                    else if (!LineItems.Any(a => a.ProductId == item.ProductId))
                    {
                        OrderLineItem oli = order.PurchaseOrderLineItems.First(n => n.Id == item.LineItemId);
                        _orderService.DeleteLineItem(oli);
                    }
                    else
                    {
                        OrderLineItem oli = order.PurchaseOrderLineItems.First(n => n.Id == item.LineItemId);
                        oli.Qty = item.Qty;
                        _orderService.SaveLineItem(oli, order.Id);
                    }


                }

            }
        }

        void CreateNewOrder()
        {
            using (var container = NestedContainer)
            {
              
                ICostCentreRepository _costCentreService = Using<ICostCentreRepository>(container);
                IConfigService _configService = Using<IConfigService>(container);
                IConfirmPurchaseOrderWFManager _confirmPurchaseOrderWFManager =
                    Using<IConfirmPurchaseOrderWFManager>(container);
                IUserRepository _userService = Using<IUserRepository>(container);
               
                Order newOrder = null;
                CostCentre docIssuerCC = _costCentreService.GetById(_configService.Load().CostCentreId);
                CostCentre docIssueOnBehalf = docIssuerCC;
                User currentUser = _userService.GetById(_configService.ViewModelParameters.CurrentUserId);
                CostCentre parentCostCentre = _costCentreService.GetAll().OfType<Producer>().First();
                var paramaters = new OrderFactoryParameters
                                     {
                                         DateRequired = this.DateRequired,
                                         DocumentIssuerCostCentre = docIssuerCC,
                                         DocumentIssuerUser = currentUser,
                                         DocumentRecipientCostCentre = parentCostCentre,
                                         IssuedOnBehalfOfCostCentre = docIssuerCC,
                                         Status = DocumentStatus.New,
                                         DocumentReference = OrderId

                                     };
                newOrder = _confirmPurchaseOrderWFManager.PendingDocumentFactory(paramaters);
                newOrder.DateRequired = DateRequired;
                OrderIdLookup = newOrder.Id;
                _confirmPurchaseOrderWFManager.SaveNew(newOrder);
            }
        }

        OrderLineItem CreateNewLineItem(EditPurchaseOrderItem item, Order order)
        {
            using (var container = NestedContainer)
            {
                
                IConfirmPurchaseOrderWFManager _confirmPurchaseOrderWFManager =
                    Using<IConfirmPurchaseOrderWFManager>(container);

                IProductRepository _productService = Using<IProductRepository>(container);
               
                var p = new OrderLineItemFactoryParameters
                            {
                                Description = item.Product,
                                Product = _productService.GetById(item.ProductId),
                                Qty = item.Qty
                            };
                var oli = _confirmPurchaseOrderWFManager.PendingDocumentLineItemFactory(order, p);
                item.LineItemId = oli.Id;
                oli.Value = item.UnitPrice;
                oli.Qty = item.Qty;
                oli.LineItemVatValue = item.Vat;
                oli.LineItemType = OrderLineItemType.DuringConfirmation;
                return oli;
            }
        }

        public RelayCommand Load { get; set; }

        private void DoLoad()
        {
            //check if order has orderId_productPackagingSummaryService
            _productPackagingSummaryService.ClearBuffer();
            _LineItems.Clear();
            if (OrderIdLookup == Guid.Empty)
            {
                //create purchase order
                PageTitle = GetLocalText("sl.po.name") + " -> Create a " +
                            GetLocalText("sl.po.name");
                OrderId = NewOrderId();
                Status = DocumentStatus.New.ToString();
                CreatedByUser = "TODO Current User";
                IsEditable = true;
                ButtonCancelName = "Cancel";
            }
            else //load order
            {
                PageTitle = GetLocalText("sl.po.name") + " -> View " +
                            GetLocalText("sl.po.name");
                Order o = GetEntityById(typeof (Order), OrderIdLookup) as Order;
                IsEditable = false;
                if (o.Status == DocumentStatus.New)
                    IsEditable = true;
                ButtonCancelName = "Close";
                Status = o.Status.ToString();
                OrderId = o.DocumentReference;
                DateRequired = o.DateRequired;
                DateSubmitted = o.DocumentDateIssued;
                CreatedByUser = o.DocumentIssuerUser == null ? string.Empty : o.DocumentIssuerUser.Username;
                LineItems.Clear();
                foreach (var item in o.PurchaseOrderLineItems)
                {
                    bool isEditable = false;
                    if (item.Product is SaleProduct || item.Product is ConsolidatedProduct)
                    {
                        _productPackagingSummaryService.AddProduct(item.Product.Id, item.Qty, false, false, true);
                        isEditable = true;
                    }

                    AddLineItem(item.Id, item.Product.Id, item.Product.Description, item.Value,
                                item.LineItemVatValue, item.LineItemVatTotal, 0,
                                item.Qty, item.LineItemTotal, isEditable);
                }

                RefreshList();
            }
        }

        string NewOrderId()
        {
            using (var container = NestedContainer)
            {
                IOrderRepository _orderService = Using<IOrderRepository>(container);
                ICostCentreRepository _costCentreService = Using<ICostCentreRepository>(container);
                IConfigService _configService = Using<IConfigService>(container);
                
                Guid costCentreId = _configService.Load().CostCentreId;
                Guid applicationId = _configService.Load().CostCentreApplicationId;
                int orderAppSeqId = _orderService.GetPurchaseOrderCount() + 1;
                CostCentre cc = _costCentreService.GetById(costCentreId);
                string date = DateTime.Now.ToString("yyyy.MM.dd.hh.mm.ss");
                string formatString = "PO_{0}_{1}_{2}";
                return string.Format(formatString, cc.Name, date, orderAppSeqId);
            }
        }

        #region Properties
        public const string OrderDatePropertyName = "OrderDate";
        private DateTime _orderDate = DateTime.Now;
        public DateTime OrderDate
        {
            get
            {
                return _orderDate;
            }

            set
            {
                if (_orderDate == value)
                {
                    return;
                }

                var oldValue = _orderDate;
                _orderDate = value;

               

                // Update bindings, no broadcast
                RaisePropertyChanged(OrderDatePropertyName);

                
            }
        }

        public const string OrderIdLookupPropertyName = "OrderIdLookup";
        private Guid _orderIdLookup = Guid.Empty;
        public Guid OrderIdLookup
        {
            get
            {
                return _orderIdLookup;
            }

            set
            {
                if (_orderIdLookup == value)
                {
                    return;
                }

                var oldValue = _orderIdLookup;
                _orderIdLookup = value;
                RaisePropertyChanged(OrderIdLookupPropertyName);
            }
        }

        public const string StatusPropertyName = "Status";
        private string _status = "";
        public string Status
        {
            get
            {
                return _status;
            }

            set
            {
                if (_status == value)
                    return;

                var oldValue = _status;
                _status = value;
                RaisePropertyChanged(StatusPropertyName);
            }
        }

        public const string OrderIdPropertyName = "OrderId";
        private string _orderId = "";
        public string OrderId
        {
            get
            {
                return _orderId;
            }

            set
            {
                if (_orderId == value)
                    return;
                var oldValue = _orderId;
                _orderId = value;
                RaisePropertyChanged(OrderIdPropertyName);
            }
        }

        public const string DateRequiredPropertyName = "DateRequired";
        private DateTime _dateRequired = DateTime.Now;
        public DateTime DateRequired
        {
            get
            {
                return _dateRequired;
            }

            set
            {
                if (_dateRequired == value)
                    return;

                var oldValue = _dateRequired;
                _dateRequired = value;
                RaisePropertyChanged(DateRequiredPropertyName);
            }
        }

        public const string DateSubmittedPropertyName = "DateSubmitted";
        private DateTime _dateSubmitted = DateTime.MaxValue;
        public DateTime DateSubmitted
        {
            get
            {
                return _dateSubmitted;
            }

            set
            {
                if (_dateSubmitted == value)
                    return;
                var oldValue = _dateSubmitted;
                _dateSubmitted = value;
                RaisePropertyChanged(DateSubmittedPropertyName);
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

        public const string TotalNetPropertyName = "TotalNet";
        private decimal _totalNet = 0;
        public decimal TotalNet
        {
            get
            {
                return _totalNet;
            }

            set
            {
                if (_totalNet == value)
                    return;
                var oldValue = _totalNet;
                _totalNet = value;
                RaisePropertyChanged(TotalNetPropertyName);
            }
        }

        public const string TotalVatPropertyName = "TotalVat";
        private decimal _totalVat = 0;
        public decimal TotalVat
        {
            get
            {
                return _totalVat;
            }

            set
            {
                if (_totalVat == value)
                    return;
                var oldValue = _totalVat;
                _totalVat = value;
                RaisePropertyChanged(TotalVatPropertyName);
            }
        }

        public const string TotalGrossPropertyName = "TotalGross";
        private decimal _totalGross = 0;
        public decimal TotalGross
        {
            get
            {
                return _totalGross;
            }

            set
            {
                if (_totalGross == value)
                    return;
                var oldValue = _totalGross;
                _totalGross = value;
                RaisePropertyChanged(TotalGrossPropertyName);
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

        public const string PageTitlePropertyName = "PageTitle";
        private string _pageTitle = "Purchase order -> Create Purchase Order";
        public string PageTitle
        {
            get
            {
                return _pageTitle;
            }

            set
            {
                if (_pageTitle == value)
                {
                    return;
                }

                var oldValue = _pageTitle;
                _pageTitle = value;
                RaisePropertyChanged(PageTitlePropertyName);
            }
        }

       
        public const string ButtonCancelNamePropertyName = "ButtonCancelName";
        private string _ButtonCancelName = "Cancel";
        public string ButtonCancelName
        {
            get
            {
                return _ButtonCancelName;
            }

            set
            {
                if (_ButtonCancelName == value)
                {
                    return;
                }

                var oldValue = _ButtonCancelName;
                _ButtonCancelName = value;

              

                // Update bindings, no broadcast
                RaisePropertyChanged(ButtonCancelNamePropertyName);

              
            }
        }

        public ObservableCollection<EditPurchaseOrderItem> LineItems { get; set; }
        public ObservableCollection<EditPurchaseOrderItem> _LineItems { get; set; }
        #endregion

        public void AddLineItem(Guid lineItemId, Guid productId, string productDesc, decimal unitPrice,
            decimal vatValue, decimal vatAmount, decimal vat, decimal qty, decimal totalPrice, bool isEditable)
        {
            int sequenceNo = 1;
            if (_LineItems.Count() > 0)
            {
                sequenceNo = _LineItems.Max(n => n.SequenceNo) + 1;
            }
            EditPurchaseOrderItem li;
            if (_LineItems.Any(p => p.LineItemId == lineItemId))
            {
                li = _LineItems.First(p => p.LineItemId == lineItemId);
                li.Qty =  qty;
                li.LineItemVatValue = vatValue;
                li.VatAmount = vatAmount;
                li.TotalPrice = totalPrice;
            }
            else
            {
                li = new EditPurchaseOrderItem();
                _LineItems.Add(li);
                li.Qty =  qty;
                li.LineItemVatValue = vatValue;
                li.VatAmount = vatAmount;
                li.TotalPrice = totalPrice;
                li.LineItemId = lineItemId;
            }

            li.SequenceNo = sequenceNo;
            li.ProductId = productId;
            li.Product = productDesc;
            li.UnitPrice = unitPrice;
            li.IsEditable = isEditable;
            li.Vat = vat;

            CalcTotals();
        }
        public void AddLineItem(Guid productId, string productDesc, decimal unitPrice,
           decimal vatValue, decimal vatAmount, decimal vat, decimal qty, decimal totalPrice, bool isEditable)
        {
            int sequenceNo = 1;
            if (_LineItems.Count() > 0)
            {
                sequenceNo = _LineItems.Max(n => n.SequenceNo) + 1;
            }
            EditPurchaseOrderItem li;
            if (_LineItems.Any(p => p.ProductId == productId))
            {
                li = _LineItems.First(p => p.ProductId == productId);
                li.Qty = qty;
                li.LineItemVatValue = vatValue;
                li.VatAmount = vatAmount;
                li.TotalPrice = totalPrice;
            }
            else
            {
                li = new EditPurchaseOrderItem();
                _LineItems.Add(li);
                li.Qty = qty;
                li.LineItemVatValue = vatValue;
                li.VatAmount = vatAmount;
                li.TotalPrice = totalPrice;
            }

            li.SequenceNo = sequenceNo;
            li.ProductId = productId;
            li.Product = productDesc;
            li.UnitPrice = unitPrice;
            li.IsEditable = isEditable;
            li.Vat = vat;

            CalcTotals();
        }

        public void UpdateOrAddLineItemFromPoductSummary(List<ProductAddSummary> productsummariies, bool IsNew)
        {
            foreach (ProductAddSummary product in productsummariies)
            {

                if (product.IsEditable)
                    _productPackagingSummaryService.AddProduct(product.ProductId, product.Quantity, false, !IsNew,
                                                               true);

            }
            RefreshList();
        }

        private void RefreshList()
        {
            LineItems.Clear();
            List<PackagingSummary> summarypro = _productPackagingSummaryService.GetProductSummary();
            List<PackagingSummary> sumaryReturnable = summarypro.Where(s => s.Product is ReturnableProduct).ToList();
            _productPackagingSummaryService.ClearMixedPackReturnables();
            sumaryReturnable = _productPackagingSummaryService.GetMixedPackContainers(sumaryReturnable);
            foreach (PackagingSummary item in summarypro)
            {
                if (IsEditable)
                    UpdateOrAddLineItem(item.Product, item.Quantity, item.IsEditable, item.ParentProductId,
                                        LineItemType.Unit, false);
                else
                    UpdateOrAddLineItem(item.Product, item.Quantity, false, item.ParentProductId, LineItemType.Unit,
                                        false);
            }
            foreach (PackagingSummary item in sumaryReturnable)
            {

                UpdateOrAddLineItem(item.Product, item.Quantity, false, item.ParentProductId, LineItemType.Unit,true);
            }
        }

        private void UpdateOrAddLineItem(Product product, decimal quantity, bool isEditable, Guid parentProductId, LineItemType lineItemType,bool isEdit)
        {
            decimal UnitPrice = PriceCalc(product);
            decimal UnitVat = VatCalc(product);

            decimal net = UnitPrice * quantity;
            decimal vat = UnitPrice * UnitVat;//cn
            decimal VatAmount = net * UnitVat;
            decimal TotalPrice = net + VatAmount;

            int sequenceNo = 1;
            if (LineItems.Count() > 0)
            {
                sequenceNo = LineItems.Max(n => n.SequenceNo) + 1;
            }
            EditPurchaseOrderItem li;
            if (LineItems.Any(p => p.ProductId == product.Id && p.LineItemType==lineItemType) && isEdit==false)
            {

                li = LineItems.First(p => p.ProductId == product.Id && p.LineItemType == lineItemType);
                li.Qty = li.Qty + quantity;
                li.LineItemVatValue = li.LineItemVatValue + vat;
                li.VatAmount = li.VatAmount + VatAmount;
                li.TotalPrice = li.TotalPrice + TotalPrice;

            }
            else if (LineItems.Any(p => p.ProductId == product.Id && p.LineItemType == lineItemType) && isEdit==true)
            {
                li = LineItems.First(p => p.ProductId == product.Id && p.LineItemType == lineItemType);
                li.Qty = quantity;
                li.LineItemVatValue = vat;
                li.VatAmount = VatAmount;
                li.TotalPrice = TotalPrice;
            }
            else
            {
                li = new EditPurchaseOrderItem();
                LineItems.Add(li);
                li.Qty = quantity;
                li.LineItemVatValue = vat;
                li.VatAmount = VatAmount;
                li.TotalPrice = TotalPrice;
            }

            li.SequenceNo = sequenceNo;
            li.ProductId = product.Id;
            li.Product = product.Description;
            li.UnitPrice = UnitPrice;
            li.IsEditable = isEditable;
            li.Vat = vat;
            li.ParentProductId = parentProductId;
            li.LineItemType = lineItemType;

            CalcTotals();


        }
       
        decimal PriceCalc(Product product)
        {
          decimal UnitPrice =0m;
            //ProductPricingTier tier = _productPricingService.GetAll().FirstOrDefault().Tier;

            if (product is ConsolidatedProduct)
                try
                {
                    UnitPrice = product.ExFactoryPrice; // ((ConsolidatedProduct)product).TotalExFactoryValue(tier);
                }
                catch
                {
                    UnitPrice = 0m;
                }
            else
                try
                {
                    UnitPrice = product.ExFactoryPrice;// product.TotalExFactoryValue(tier);
                }
                catch
                {
                    UnitPrice = 0m;
                }
            return UnitPrice;
        }

        decimal VatCalc(Product product)
        {
            decimal vat = 0m;
            if (product is ReturnableProduct)
                return 0;
            //if (product is ConsolidatedProduct)
            //    vat = ((ConsolidatedProduct) product).ProductVATRate(_productPricingService.GetAll().FirstOrDefault().Tier);
            //else
            //{
                if (product.VATClass != null)
                {
                    vat = product.VATClass.CurrentRate;
                }
            //}
            return vat;
        }


        private int GetSequenceNo()
        {
            int sequenceNo = 1;
            if (LineItems.Count() > 0)
            {
                sequenceNo = LineItems.Max(n => n.SequenceNo) + 1;
            }
            return sequenceNo;
        }

        public void UpdateLineItem(int sequenceNo, decimal qty)
        {
            EditPurchaseOrderItem item = LineItems.First(n => n.SequenceNo == sequenceNo);
            item.Qty = qty;
            CalcTotals();
        }

        public void RemoveLineItem(Guid productId, LineItemType lit)
        {
            var delProduct =
                _productPackagingSummaryService.GetProductSummary().FirstOrDefault(
                    p => p.Product.Id == productId && p.IsEditable);
            string msg = "";
            foreach (
                PackagingSummary delitem in
                    _productPackagingSummaryService.GetProductSummaryByProduct(productId, delProduct.Quantity))
            {
                msg += string.Format("\n\t{0} of {1} will be deleted", delitem.Quantity, delitem.Product.Description);
            }
            MessageBoxResult isConfirmed =
                MessageBox.Show("Are sure you want to delete the following product(s)" + msg,
                                "Delete Purchase Order Line item", MessageBoxButton.OKCancel);

            if (isConfirmed == MessageBoxResult.OK)
            {
                _productPackagingSummaryService.RemoveProduct(productId);
            }
            RefreshList();
            CalcTotals();
        }

        private void CalcTotals()
        {
            TotalNet = LineItems.Sum(n => (n.Qty * n.UnitPrice));
            TotalVat = LineItems.Sum(n => n.VatAmount);
            TotalGross = LineItems.Sum(n => n.TotalPrice);
        }

        public void RunClearAndSetup()
        {
            OrderIdLookup = Guid.Empty;
            Status = "";
            OrderId = "";
            DateRequired = DateTime.Now;
            DateSubmitted = DateTime.MaxValue;
            CreatedByUser = "";
            LineItems.Clear();
            CalcTotals();
        }

    }

    public class EditPurchaseOrderItem : DistributrViewModelBase
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

        /// <summary>
        /// The <see cref="ProductId" /> property's name.
        /// </summary>
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

        public const string UnitPricePropertyName = "UnitPrice";
        private decimal _unitPrice = 0;
        public decimal UnitPrice
        {
            get
            {
                return _unitPrice;
            }
            set
            {
                if (_unitPrice == value)
                {
                    return;
                }
                var oldValue = _unitPrice;
                _unitPrice = value;
                RaisePropertyChanged(UnitPricePropertyName);
            }
        }

        public const string LineItemVatValuePropertyName = "LineItemVatValue";
        private decimal _lineItemVatValue = 0;
        public decimal LineItemVatValue
        {
            get
            {
                return _lineItemVatValue;
            }

            set
            {
                if (_lineItemVatValue == value)
                    return;
                var oldValue = _lineItemVatValue;
                _lineItemVatValue = value;
                RaisePropertyChanged(LineItemVatValuePropertyName);
            }
        }

        public const string VatAmountPropertyName = "VatAmount";
        private decimal _vatAmount = 0;
        public decimal VatAmount
        {
            get
            {
                return _vatAmount;
            }

            set
            {
                if (_vatAmount == value)
                {
                    return;
                }
                var oldValue = _vatAmount;
                _vatAmount = value;
                RaisePropertyChanged(VatAmountPropertyName);
            }
        }

        public const string VatPropertyName = "Vat";
        private decimal _vat = 0m;
        public decimal Vat
        {
            get
            {
                return _vat;
            }

            set
            {
                if (_vat == value)
                {
                    return;
                }

                var oldValue = _vat;
                _vat = value;
                RaisePropertyChanged(VatPropertyName);
            }
        }

        public const string TotalPricePropertyName = "TotalPrice";
        private decimal _totalPrice = 0;
        public decimal TotalPrice
        {
            get
            {
                return _totalPrice;
            }

            set
            {
                if (_totalPrice == value)
                {
                    return;
                }
                var oldValue = _totalPrice;
                _totalPrice = value;
                RaisePropertyChanged(TotalPricePropertyName);
            }
        }

        public const string QtyPropertyName = "Qty";
        private decimal _qty = 0;
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

       
        public const string LineItemTypePropertyName = "LineItemType";
        private LineItemType _lineItemType ;
        public LineItemType LineItemType
        {
            get
            {
                return _lineItemType;
            }

            set
            {
                if (_lineItemType == value)
                {
                    return;
                }

                var oldValue = _lineItemType;
                _lineItemType = value;
                // Update bindings, no broadcast
                RaisePropertyChanged(LineItemTypePropertyName);
                RaisePropertyChanged(EditTagPropertyName);

            }
        }

        
        public const string ParentProductIdPropertyName = "ParentProductId";
        private Guid _parentProductId = Guid.Empty;
        public Guid ParentProductId
        {
            get
            {
                return _parentProductId;
            }

            set
            {
                if (_parentProductId == value)
                {
                    return;
                }

                var oldValue = _parentProductId;
                _parentProductId = value;
                // Update bindings, no broadcast
                RaisePropertyChanged(ParentProductIdPropertyName);
                RaisePropertyChanged(EditTagPropertyName);

              
            }
        }

        public const string EditTagPropertyName = "EditTag";
        private string _editTag = "";
        public string EditTag
        {
            get
            {
                _editTag = ParentProductId.ToString() + "," + ((int)LineItemType).ToString();
               
                return _editTag;
            }

            set
            {
                if (_editTag == value)
                {
                    return;
                }

                var oldValue = _editTag;
                _editTag = value;


                // Update bindings, no broadcast
                RaisePropertyChanged(EditTagPropertyName);


            }
        }


        }

    

}
