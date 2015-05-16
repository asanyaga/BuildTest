using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Distributr.Core;
using Distributr.Core.ClientApp;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Transactional;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Domain.Transactional.DocumentEntities.OrderDocumentEntities;
using Distributr.Core.Factory.Documents;
using Distributr.Core.Repository.InventoryRepository;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.Impl.Services.Transactional.SaveAndContinue;
using Distributr.WPF.Lib.Service.Utility;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.UI.Pages;
using Distributr.WPF.Lib.ViewModels.Transactional.Orders;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace Distributr.WPF.Lib.ViewModels.Transactional.Order_Pos
{
    public class AddPOSViewModel : OrderBaseViewModel
    {
        protected List<MainOrderLineItem> _lineItemReturnables;

        public AddPOSViewModel()
        {
            _lineItemReturnables = new List<MainOrderLineItem>();
            ReceiveReturnablesCommand = new RelayCommand(ReceiveReturnables);
            CompleteSaleCommand = new RelayCommand(CompleteSale);
            SaveAndContinueLaterCommand = new RelayCommand(SaveAndContinueLater);
            AddPOSPageLoadedCommand = new RelayCommand(PageLoaded);
            PageUnloading = new RelayCommand(PageUnLoaded);
        }


        private void PageLoaded()
        {
            SetUp();
        }

        private void SaveAndContinueLater()
        {
            //throw new NotImplementedException();
        }


        private void CompleteSale()
        {
            Confirm();
        }

        private void ReceiveReturnables()
        {
            using (var container = NestedContainer)
            {
                var selected = Using<IOrderProductPage>(container).GetReturnables(SelectedOutlet, GetReturnable(),
                                                                                  OrderType.DistributorPOS);
                foreach (var popItem in selected)
                {
                    AddUpdateReturnableLineItem(popItem);

                }
            }
            CalculateSummary();
            //ReturnableNotReceived = false;
        }

        private void AddUpdateReturnableLineItem(ProductPopUpItem popItem)
        {
            using (var container = NestedContainer)
            {
                MainOrderLineItem mitem;
                if (_lineItemReturnables.Any(s => s.ProductId == popItem.Product.Id))
                {
                    mitem = _lineItemReturnables.First(s => s.ProductId == popItem.Product.Id);
                    mitem.Quantity = popItem.Quantity;

                }
                else
                {
                    mitem = new MainOrderLineItem();
                    mitem.Quantity = popItem.Quantity;
                    _lineItemReturnables.Add(mitem);
                }
                LineItemPricingInfo info = Using<IDiscountProWorkflow>(container).GetLineItemPricing(
                    popItem.Product.Id, popItem.Quantity, SelectedOutlet.Id);
                mitem.UnitPrice = info.UnitPrice;
                mitem.UnitVAT = info.VatValue;
                mitem.TotalAmount = -info.TotalPrice;
                mitem.TotalNet = -info.TotalNetPrice;
                mitem.TotalVAT = -info.TotalVatAmount;
                mitem.GrossAmount = -info.TotalPrice;
                mitem.UnitDiscount = info.ProductDiscount;
                mitem.TotalProductDiscount = -info.TotalProductDiscount;
                mitem.ProductName = popItem.Product.Description;
                mitem.ProductId = popItem.Product.Id;
                mitem.ProductType = "Returnable";
                mitem.CanChange = false;
                mitem.Quantity = mitem.Quantity*(-1);
                mitem.LineItemType = MainOrderLineItemType.Returned;
            }

        }

        private Dictionary<Guid, int> GetReturnable()
        {
            var returnables = new Dictionary<Guid, int>();
            using (var container = NestedContainer)
            {
                foreach (var item in LineItem)
                {
                    var product = Using<IProductRepository>(container).GetById(item.ProductId);
                    if (product is ReturnableProduct)
                        returnables.Add(product.Id, (Int32) item.Quantity);
                }
            }
            return returnables;
        }



        private void SetUp()
        {
            try
            {
                ClearVM();
                base.SetupCommand.Execute(null);
                using (var c = NestedContainer)
                {
                    var configService = Using<IConfigService>(c);
                    CanApproveOrder = configService.ViewModelParameters.CurrentUserRights.CanApproveOrders;
                    CanRemove = configService.ViewModelParameters.CurrentUserRights.CanEditOrder;
                    User salesmanUser =
                        Using<IUserRepository>(c).GetById(configService.ViewModelParameters.CurrentUserId);
                    CreatedByUser = salesmanUser == null ? "" : salesmanUser.Username;

                    var hasSaleProducts =
                        Using<IInventoryRepository>(c).GetByWareHouseId(configService.Load().CostCentreId).Where(
                            p => p.Balance > 0).Select(p => p.Product)
                            .OfType<SaleProduct>().Any();
                    if (!hasSaleProducts)
                    {
                        MessageBox.Show("There is no inventory for sale products/", "Distributr warning",
                                        MessageBoxButton.OK, MessageBoxImage.Warning);

                    }

                }
            }
            catch
            {
            }
            if (IsUnConfirmed)
            {
                IsUnConfirmed = false;
                LoadToContinue();
                GetCurrentDocumentRef();
                CalculateSummary();
            }
        }

        protected override void Cancel()
        {
            if (
                MessageBox.Show("All unsaved changes will be lost", "Distributr Warning", MessageBoxButton.OKCancel,
                                MessageBoxImage.Warning) == MessageBoxResult.OK)
            {
                string url = "/views/Order_Pos/ListPOSSales.xaml";
                NavigateCommand.Execute(url);
            }

        }

        protected override void Confirm()
        {
            GetCurrentDocumentRef();
            ValidateBeforeConfirmSale();
            if (IsValid)
            {
                using (var c = NestedContainer)
                {
                    var configService = Using<IConfigService>(c);
                    var mainOrderRepository = Using<IMainOrderRepository>(c);
                    Config config = configService.Load();
                    Guid costCentreApplicationid = config.CostCentreApplicationId;
                    CostCentre dcc = Using<ICostCentreRepository>(c).GetById(config.CostCentreId);
                    User user = Using<IUserRepository>(c).GetById(configService.ViewModelParameters.CurrentUserId);
                    var posWorkflow = Using<IOrderPosWorkflow>(c);
                    var mainOrderFactory = Using<IMainOrderFactory>(c);
                    string shipto = FormatShipToAddress(SelectedShipAddress);
                    MainOrder order = mainOrderFactory.Create(dcc, costCentreApplicationid, SelectedSalesman, user,
                                                              SelectedOutlet, OrderType.DistributorPOS,
                                                              OrderReferenceNo,
                                                              Guid.Empty, shipto, DateTime.Now, SaleDiscount, Note);
                    order.SaleDiscount = SaleDiscount;

                    foreach (var item in LineItem.Where(p => p.Quantity > 0))
                    {
                        if (item.LineItemType == MainOrderLineItemType.Returned)
                            continue;
                        var quantity = item.Quantity;
                        if (item.Product is ReturnableProduct)
                        {
                            quantity = LineItem.Where(p => p.ProductId == item.ProductId).Sum(p => p.Quantity);

                            if (quantity <= 0)
                                continue;
                        }

                        if (item.LineItemType == MainOrderLineItemType.Sale)
                        {

                            SubOrderLineItem lineItem = null;
                            if (item.UnitDiscount > 0)
                            {
                                lineItem = mainOrderFactory.CreateDiscountedLineItem(item.ProductId, quantity,
                                                                                     item.UnitPrice, item.ProductType,
                                                                                     item.UnitVAT, item.UnitDiscount);
                            }
                            else
                            {

                                lineItem = mainOrderFactory.CreateLineItem(item.ProductId, quantity,
                                                                           item.UnitPrice, item.ProductType,
                                                                           item.UnitVAT);
                            }
                            order.AddLineItem(lineItem);
                        }
                        else if (item.LineItemType == MainOrderLineItemType.Discount)
                        {
                            SubOrderLineItem lineItem = mainOrderFactory.CreateFOCLineItem(item.ProductId, quantity,
                                                                                           item.ProductType,
                                                                                           item.DiscountType);
                            order.AddLineItem(lineItem);

                        }
                    }
                    order.Confirm();
                    posWorkflow.Submit(order,config);
                    DateTime start = DateTime.Now;
                    Using<IOrderSaveAndContinueService>(c).MarkAsConfirmed(OrderId);
                    // Do some work
                    var payment = new List<PaymentInfo>();
                    //hold payment info to avoid lossing it due to async processing
                    PaymentInfoList.ForEach(payment.Add);
                    Task.Run(() =>
                                 {
                                     using (var co = NestedContainer)
                                     {
                                         var mainrepo = Using<IMainOrderRepository>(co);
                                         var posWF = Using<IOrderPosWorkflow>(co);
                                         order = mainrepo.GetById(order.Id);
                                         foreach (var item in order.PendingApprovalLineItems)
                                         {
                                             order.ApproveLineItem(item);
                                         }
                                         foreach (
                                             var paymentInfo in
                                                 payment.Where(p => p.PaymentModeUsed != PaymentMode.Credit))
                                         {
                                             order.AddOrderPaymentInfoLineItem(paymentInfo);
                                         }
                                         order.Approve();
                                         posWF.Submit(order,config);
                                         order = mainrepo.GetById(order.Id);
                                         order.Close();
                                         posWF.Submit(order,config);
                                         HandleFiscalPrinter(order.Id);
                                     }
                                 });
                    TimeSpan timeDiff = DateTime.Now - start;
                    double diff = timeDiff.TotalMilliseconds;
                   // HandleFiscalPrinter(order.Id);

                    const string uri = "/views/Order_Pos/ViewPOS.xaml";
                    Messenger.Default.Send<ViewModelMessage>(new ViewModelMessage {Id = order.Id});
                    NavigateCommand.Execute(uri);
                    ClearVM();
                }
            }
        }

        protected override void SaveAndContinue()
        {
            if (!LineItem.Any())
            {
                MessageBox.Show("Make sure you have atleast one item");
                return;

            }
            using (var c = NestedContainer)
            {
                var saveAndContinue = new OrderSaveAndContinueLater();
                saveAndContinue.OrderType = OrderType.DistributorPOS;
                saveAndContinue.Id = OrderId != Guid.Empty ? OrderId : Guid.NewGuid();
                saveAndContinue.OutletId = SelectedOutlet.Id;
                saveAndContinue.Outlet = SelectedOutlet.Name;
                saveAndContinue.RouteId = SelectedRoute.Id;
                saveAndContinue.SalesmanId = SelectedSalesman.Id;
                saveAndContinue.Salesman = SelectedSalesman.Name;
                saveAndContinue.ShipToAddressId = SelectedShipAddress.Id;
                saveAndContinue.Required = DateRequired;
                foreach (var item in LineItem)
                {
                    var i = new OrderSaveAndContinueLaterItem();
                    i.DiscountType = item.DiscountType;
                    i.LineItemType = item.LineItemType;
                    i.ProductId = item.ProductId;
                    i.ProductType = item.ProductType;
                    i.Quantity = item.Quantity;
                    i.UnitDiscount = item.UnitDiscount;
                    i.UnitPrice = item.UnitPrice;
                    i.UnitVat = item.UnitVAT;
                    saveAndContinue.LineItem.Add(i);

                }

                Using<IOrderSaveAndContinueService>(c).Save(saveAndContinue);
                MessageBox.Show("Sale saved successfully ");
                var uri = "/views/order_pos/ListPOSSales.xaml";
                NavigateCommand.Execute(uri);


            }
        }

        protected override void AddProduct()
        {
            if (ValidateBeforeAddProduct())
            {
                using (var container = NestedContainer)
                {
                    var selected = Using<IOrderProductPage>(container).GetProduct(SelectedOutlet,
                                                                                  OrderType.DistributorPOS, _lineItem);
                    foreach (var popItem in selected)
                    {
                        AddUpdateLineItem(popItem, false);
                    }
                }
                CanChange = false;
                CalculateSummary();
            }

        }

        protected override void EditProduct(MainOrderLineItem seleectedItem)
        {
            if (ValidateBeforeAddProduct())
            {
                using (var container = NestedContainer)
                {
                    var selected = Using<IOrderProductPage>(container).EditProduct(SelectedOutlet,
                                                                                   seleectedItem.ProductId,
                                                                                   seleectedItem.Quantity,
                                                                                   OrderType.DistributorPOS);
                    foreach (var popItem in selected)
                    {
                        AddUpdateLineItem(popItem, true);
                    }
                }
                CanChange = false;
                CalculateSummary();
            }
        }

        protected override void DeleteEditProduct(MainOrderLineItem obj)
        {
            MessageBoxResult confirm = MessageBox.Show("Are you sure you want to delete the lineitem",
                                                       "Distributr Point Of Sale", MessageBoxButton.OKCancel);
            if (confirm != MessageBoxResult.OK) return;
            DeleteLineItem(obj.ProductId);
            CalculateSummary();
        }

        protected override void GetCurrentDocumentRef()
        {
            using (var c = NestedContainer)
            {
                if (SelectedOutlet != null && SelectedOutlet.Id != Guid.Empty)
                {
                    OrderReferenceNo = Using<IGetDocumentReference>(c).GetDocReference("Sale", SelectedSalesman.Id,
                                                                                       SelectedOutlet.Id);
                }

            }
        }

        private void PageUnLoaded()
        {
        }

        #region utils

        private void ValidateBeforeConfirmSale()
        {
            IsValid = true;
            if (LineItem.Count < 1)
            {
                MessageBox.Show(
                    GetLocalText("sl.pos.completesale.messagebox.nolineitems")
                    /*"The sale must have at least 1 line item."*/
                    , "!" + GetLocalText("sl.pos.messagebox.caption") /*"Distributr: Point of Sale"*/
                    , MessageBoxButton.OK);
                IsValid = false;
            }
            if (IsReturnableRequired() && ReturnableNotReceived)
            {
                MessageBoxResult isResult = MessageBox.Show( /*"Are you sure you want to complete sales without receiving returnables."*/
                    GetLocalText("sl.pos.completesale.messagebox.notReceivedReturnables")
                    , "!" + GetLocalText("sl.pos.messagebox.caption") /*"Distributr: Point of Sale"*/
                    , MessageBoxButton.OKCancel);
                if (isResult == MessageBoxResult.Cancel)
                {
                    IsValid = false;
                }
            }
            if (IsValid)
                IsValid = ValidatePosPayment();

        }


        private bool ValidateBeforeAddProduct()
        {
            string msg = string.Empty;
            if (SelectedSalesman == null || SelectedSalesman.Id == Guid.Empty)
                msg += "\tSalesman is required !\n";
            if (SelectedRoute == null || SelectedRoute.Id == Guid.Empty)
                msg += "\tRoute is required !\n";
            if (SelectedOutlet == null || SelectedOutlet.Id == Guid.Empty)
                msg += "\tOutlet is required !\n";
            if (msg != string.Empty)
            {
                MessageBox.Show("Provide \n" + msg, "Saleman Order");
                return false;
            }
            return true;
        }

        private bool IsReturnableRequired()
        {
            using (StructureMap.IContainer container = NestedContainer)
            {
                foreach (var item in LineItem)
                {
                    Product product = Using<IProductRepository>(container).GetById(item.ProductId);
                    if (product is ReturnableProduct)
                    {
                        return LineItem.Where(n => n.ProductId == item.ProductId).Sum(s => s.TotalNet) > 0;
                    }
                }
            }
            return false;
        }


        private void CalculateSummary()
        {
            LineItem.Clear();
            _lineItemTemp.Clear();
            using (var container = NestedContainer)
            {
                var summaryService = Using<IProductPackagingSummaryService>(container);
                var pricingService = Using<IDiscountProWorkflow>(container);
                foreach (var item in _lineItem)
                {
                    summaryService.AddProduct(item.Product.Id, item.Quantity);

                }
                List<PackagingSummary> summary = summaryService.GetProductFinalSummary();
                foreach (var s in summary)
                {
                    MainOrderLineItem mitem = new MainOrderLineItem();
                    LineItemPricingInfo info = pricingService.GetLineItemPricing(s, SelectedOutlet.Id);
                    mitem.UnitPrice = info.UnitPrice;
                    mitem.UnitVAT = info.VatValue;
                    mitem.TotalAmount = info.TotalPrice;
                    mitem.TotalNet = info.TotalNetPrice;
                    mitem.TotalVAT = info.TotalVatAmount;
                    mitem.GrossAmount =info.TotalPrice.GetTruncatedValue();
                    mitem.UnitDiscount = info.ProductDiscount;
                    mitem.TotalProductDiscount = info.TotalProductDiscount;
                    mitem.ProductName = s.Product.Description;
                    mitem.ProductId = s.Product.Id;
                    mitem.Quantity = s.Quantity;
                    mitem.Product = s.Product;
                    mitem.ProductType = s.Product is SaleProduct ? "Sale" : "Returnable";
                    if (_lineItem.Any(p => p.Product.Id == s.Product.Id && p.IsFreeOfCharge))
                    {
                        mitem.ProductType = "Sale(Free of Charge)";
                    }

                    mitem.CanChange = s.Product is SaleProduct;
                    _lineItemTemp.Add(mitem);


                }
                CalculateFinalSummary();
                RefreshLineItems();
            }

        }

        private void RefreshLineItems()
        {
            int seqeunceNo = 1;
            foreach (var item in _lineItemTemp)
            {
                item.SequenceNo = seqeunceNo;
                LineItem.Add(item);
                seqeunceNo++;
            }
            foreach (var item in _lineItemReturnables)
            {
                item.SequenceNo = seqeunceNo;
                item.Quantity = item.Quantity;
                LineItem.Add(item);
                seqeunceNo++;
            }
        }

        public virtual void CalculateFinalSummary()
        {
            using (var container = NestedContainer)
            {

                IDiscountHelper _discountHelper = Using<IDiscountHelper>(container);
                var pricingService = Using<IDiscountProWorkflow>(container);
                _lineItemTemp = _discountHelper.CalculateForPos(_lineItemTemp, SelectedOutlet.Id);

                SaleDiscount = _discountHelper.CalculateSaleDiscount(_lineItemTemp.Sum(s => s.GrossAmount),
                                                                     SelectedOutlet.Id);
                TotalGross = _lineItemTemp.Sum(s => s.GrossAmount).GetTotalGross();
                TotalProductDoscount = _lineItemTemp.Sum(s => s.UnitDiscount*s.Quantity);
                TotalVat = _lineItemTemp.Sum(s => s.TotalVAT) + _lineItemReturnables.Sum(s => s.TotalVAT);

                _lineItemTemp.ForEach(x => x.TotalNet = x.TotalNet.GetTruncatedValue());
                _lineItemTemp.ForEach(x => x.TotalVAT = x.TotalVAT.GetTruncatedValue());

                TotalNet = ((_lineItemTemp.Sum(s => s.TotalNet).GetTruncatedValue() + _lineItemReturnables.Sum(s => s.TotalNet).GetTruncatedValue() + TotalVat).GetTotalGross() -
                           SaleDiscount).GetTotalGross();
                ReturnableValue = _lineItemReturnables.Sum(p => p.TotalAmount);
                SaleValue = TotalGross;
            }
        }



        private void ClearVM()
        {
            SelectedSalesman = DefaultSalesman;
            SelectedRoute = DefaultRoute;
            SelectedOutlet = DefaultOutlet;
            SelectedShipAddress = DefaultShipTo;
            IsValid = false;
            //  IsConfirmed = false;
            CreatedByUser = string.Empty;
            ReturnableValue = 0m;
            CanRemove = false;
            LoadForEditing = false;
            ReturnableNotReceived = true;
            SaleValue = 0m;
            Note = string.Empty;
            TotalGross = 0m;
            TotalNet = 0m;
            TotalVat = 0m;
            AmountPaid = 0m;
            PaymentInfoList.Clear();
            SaleDiscount = 0m;
            TotalProductDoscount = 0m;
            OrderReferenceNo = "New Sale";
            _lineItemReturnables.Clear();
            PaymentInfoItems.Clear();
            _lineItem.Clear();
            LineItem.Clear();
            CanChange = true;

        }

        #endregion

        #region properties

        public RelayCommand ReceiveReturnablesCommand { get; set; }
        public RelayCommand CompleteSaleCommand { get; set; }
        public RelayCommand SaveAndContinueLaterCommand { get; set; }
        public RelayCommand AddPOSPageLoadedCommand { get; set; }
        public RelayCommand PageUnloading { get; set; }


        public const string CreatedByUserPropertyName = "CreatedByUser";
        private string _createdByUser = "";

        public string CreatedByUser
        {
            get { return _createdByUser; }

            set
            {
                if (_createdByUser == value)
                    return;

                var oldValue = _createdByUser;
                _createdByUser = value;
                RaisePropertyChanged(CreatedByUserPropertyName);
            }
        }

        public const string IsReturnableReceivedPropertyName = "ReturnableNotReceived";
        private bool _returnableNotReceived = true;

        public bool ReturnableNotReceived
        {
            get { return _returnableNotReceived; }

            set
            {
                if (_returnableNotReceived == value)
                {
                    return;
                }
                _returnableNotReceived = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(IsReturnableReceivedPropertyName);
            }
        }


        public const string InvoiceDocumentPropertyName = "InvoiceDocument";
        private Invoice _invoiceDocument = null;

        public Invoice InvoiceDocument
        {
            get { return _invoiceDocument; }

            set
            {
                _invoiceDocument = value;
                RaisePropertyChanged(InvoiceDocumentPropertyName);
            }
        }


        public const string ReturnableValuePropertyName = "ReturnableValue";
        private decimal _returnableValue = 0;

        public decimal ReturnableValue
        {
            get { return _returnableValue; }

            set
            {
                if (_returnableValue == value)
                {
                    return;
                }

                RaisePropertyChanging(TotalNetPropertyName);
                _returnableValue = value;
                RaisePropertyChanged(ReturnableValuePropertyName);
            }
        }


        public const string IsValidPropertyName = "IsValid";
        private bool _isValid;

        public bool IsValid
        {
            get { return _isValid; }

            set
            {
                if (_isValid == value)
                {
                    return;
                }

                var oldValue = _isValid;
                _isValid = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(IsValidPropertyName);
            }
        }

        public const string CanRemovePropertyName = "CanRemove";
        private bool _canRemove;

        public bool CanRemove
        {
            get { return _canRemove; }

            set
            {
                if (_canRemove == value)
                {
                    return;
                }

                var oldValue = _canRemove;
                _canRemove = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(CanRemovePropertyName);
            }
        }

        public const string LoadForEditingPropertyName = "LoadForEditing";
        private bool _loadForEditing = true;

        public bool LoadForEditing
        {
            get { return _loadForEditing; }

            set
            {
                if (_loadForEditing == value)
                {
                    return;
                }

                var oldValue = _loadForEditing;
                _loadForEditing = value;
                //if (value)

                RaisePropertyChanged(LoadForEditingPropertyName);
            }
        }

        public const string SaleValuePropertyName = "SaleValue";
        private decimal _saleValue;

        public decimal SaleValue
        {
            get { return _saleValue; }

            set
            {
                if (_saleValue == value)
                {
                    return;
                }

                var oldValue = _saleValue;
                _saleValue = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(SaleValuePropertyName);
            }
        }


        #endregion

        public void Continue(SaleOrderContinueMessage obj)
        {
            OrderId = obj.Id;
            IsUnConfirmed = obj.IsUnConfirmed;
        }
    }
}
